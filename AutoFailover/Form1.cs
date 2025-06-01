using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace AutoFailover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ApplyMulticolorVisualStyleTheme();
        }

        private void txtServer_TextChanged(object sender, EventArgs e)
        {
            // 예시: 텍스트가 바뀔 때 로그 출력
            Console.WriteLine("서버 텍스트가 변경됨: " + txtServer.Text);
            LoadDatabaseList();
        }

        //DB 리스트 불러오는 함수 추가

        private void LoadDatabaseList()
        {
            string server = txtServer.Text;
            if (string.IsNullOrWhiteSpace(server))
            {
                txtLog.AppendText("⚠ 서버명을 먼저 입력해주세요.\n");
                return;
            }

            string connectionString = $"Server={server};Integrated Security=true;";
            List<string> databases = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    DataTable dt = conn.GetSchema("Databases");

                    foreach (DataRow row in dt.Rows)
                    {
                        string dbName = row["database_name"].ToString();
                        // 시스템 DB 제외 필터 추가
                        if (dbName != "master" && dbName != "tempdb" && dbName != "model" && dbName != "msdb")
                            databases.Add(dbName);
                    }
                }

                cmbDBList.Items.Clear();
                cmbDBList.Items.AddRange(databases.ToArray());

                if (cmbDBList.Items.Count > 0)
                    cmbDBList.SelectedIndex = 0;

                txtLog.AppendText($"✔ DB 목록 불러오기 완료 ({databases.Count}개)\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ DB 목록 불러오기 실패: {ex.Message}\n");
            }
        }



        //Backup 기능
        private void btnBackup_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(dbName))
            {
                txtLog.AppendText("✘ 백업 실패: DB를 선택하세요.\n");
                return;
            }

            string backupDir = @"D:\DBBackups";
            string backupFile = Path.Combine(backupDir, $"{dbName}_{DateTime.Now:yyyyMMdd_HHmm}.bak");

            string connectionString = $"Server={server};Integrated Security=true;";
            string backupSql = $"BACKUP DATABASE [{dbName}] TO DISK = N'{backupFile}' WITH INIT, FORMAT";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(backupSql, conn);
                    cmd.ExecuteNonQuery();
                }
                txtLog.AppendText($"✔ 백업 완료: {backupFile}\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ 백업 실패: {ex.Message}\n");
            }
        }



        // 복구 백업파일 경로 선택 버튼 클릭 이벤트 핸들러
        private void btnSelectBackupFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Backup Files (*.bak)|*.bak";
                ofd.Title = "복구할 .bak 파일을 선택하세요";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = ofd.FileName;
                    txtLog.AppendText($"✔ 파일 선택됨: {ofd.FileName}\n");
                }
            }
        }


        //Restore 기능
        private void btnRestore_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();
            string backupFile = txtBackupPath.Text;
            string mdfDir = @"D:\DBBackups\SQL_DATA";
            string ldfDir = @"D:\DBBackups\SQL_LOG";

            if (string.IsNullOrWhiteSpace(dbName))
            {
                txtLog.AppendText("✘ 복구 실패: DB를 선택하세요.\n");
                return;
            }

            if (string.IsNullOrWhiteSpace(backupFile))
            {
                txtLog.AppendText("✘ 복구 실패: 백업 파일을 선택하세요.\n");
                return;
            }

            string mdfPath = Path.Combine(mdfDir, $"{dbName}.mdf");
            string ldfPath = Path.Combine(ldfDir, $"{dbName}_log.ldf");

            string connectionString = $"Server={server};Integrated Security=true;";

            string restoreCheckSql = $@"
            USE master;
            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{dbName}]
            FROM DISK = N'{backupFile}'
            WITH 
                MOVE '{dbName}' TO N'{mdfPath}',
                MOVE '{dbName}_log' TO N'{ldfPath}',
                REPLACE;
            ALTER DATABASE [{dbName}] SET MULTI_USER;";

            txtLog.AppendText("📦 복원 시작...\n");
            txtLog.AppendText($"🔹 서버: {server}\n");
            txtLog.AppendText($"🔹 DB명: {dbName}\n");
            txtLog.AppendText($"🔹 백업파일: {backupFile}\n");
            txtLog.AppendText($"🔹 복원경로: {mdfDir}, {ldfDir}\n");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(restoreCheckSql, conn);
                    cmd.ExecuteNonQuery();
                }
                txtLog.AppendText("✔ 복구 완료\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ 복구 실패: {ex.Message}\n");
            }
        }



        // 패치 실행 비동기
        private async Task RunPatchAsync(string batFile, string arguments)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = batFile,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(batFile),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = psi;
                proc.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        AppendLogThreadSafe($"   ✅ {e.Data}");
                };
                proc.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        AppendLogThreadSafe($"   ❌ {e.Data}");
                };

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                await Task.Run(() => proc.WaitForExit());
            }

            AppendLogThreadSafe("✔ 패치 실행 완료\n");
        }

        private void AppendLogThreadSafe(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => txtLog.AppendText(message + "\n")));
            }
            else
            {
                txtLog.AppendText(message + "\n");
            }
        }



        //패치 실행 이벤트
        private async void btnRunPatch_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();
            string patchFolder = txtPatchFolder.Text;

            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(dbName))
            {
                txtLog.AppendText("✘ 서버 또는 DB를 선택하세요.\n");
                return;
            }

            if (string.IsNullOrWhiteSpace(patchFolder) || !Directory.Exists(patchFolder))
            {
                txtLog.AppendText("✘ 패치 폴더 경로가 유효하지 않습니다.\n");
                return;
            }

            string[] batFiles = Directory.GetFiles(patchFolder, "*.bat");
            if (batFiles.Length == 0)
            {
                txtLog.AppendText("✘ 해당 폴더에 .bat 파일이 없습니다.\n");
                return;
            }

            string batFile = batFiles[0]; // 첫 번째 .bat
            txtLog.AppendText($"▶ 패치 실행 시작: {Path.GetFileName(batFile)}\n");

            await RunPatchAsync(batFile, $"\"{server}\" \"{dbName}\"");
        }




        // 패치 할 배치파일 폴더 선택
        private void btnSelectPatchFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "패치 .BAT 파일이 있는 폴더를 선택하세요";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPatchFolder.Text = fbd.SelectedPath;
                    txtLog.AppendText($"📂 패치 폴더 선택됨: {fbd.SelectedPath}\n");
                }
            }
        }

        //Daily Restore 상태 체크
        private void restoreCheck_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();
            string connectionString = $"Server={server};Integrated Security=true;";

            string restoreCheckSql = @"
             USE DBADMIN;
             SELECT dbname,restoredate FROM dbo.DBA_T_HISTORY_RESTORE;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(restoreCheckSql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtLog.AppendText("📋 Daily Restore Check 결과:\n");

                        // 열 이름 헤더 출력
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            txtLog.AppendText($"{reader.GetName(i),-20}");
                        }
                        txtLog.AppendText("\n");

                        // 데이터 행 출력
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string value = reader[i]?.ToString();
                                txtLog.AppendText($"{value,-20}");
                            }
                            txtLog.AppendText("\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ Restore 상태 확인 실패: {ex.Message}\n");
            }
        }

        // 현재 세션의 데드락/블로킹 상태 조회
        private void btnCheckDeadlock_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(dbName))
            {
                txtLog.AppendText("✘ 서버 또는 DB명을 입력하세요.\n");
                return;
            }

            string connectionString = $"Server={server};Database={dbName};Integrated Security=true;";

            string deadlockSql = @"
            SELECT 
                r.session_id,
                r.blocking_session_id,
                r.wait_type,
                r.wait_time,
                r.status,
                SUBSTRING(t.text, r.statement_start_offset / 2 + 1, 
                          (CASE WHEN r.statement_end_offset = -1 
                                THEN LEN(CONVERT(nvarchar(MAX), t.text)) * 2 
                                ELSE r.statement_end_offset END - r.statement_start_offset) / 2) AS query_text
            FROM sys.dm_exec_requests r
            JOIN sys.dm_exec_sessions s ON r.session_id = s.session_id
            CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) AS t
            WHERE r.blocking_session_id != 0
            ORDER BY r.wait_time DESC;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(deadlockSql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtLog.AppendText("📌 현재 블로킹/데드락 상태 조회:\n");

                        if (!reader.HasRows)
                        {
                            txtLog.AppendText("✔ 데드락/블로킹 세션 없음\n");
                            return;
                        }

                        while (reader.Read())
                        {
                            int sessionId = reader.GetInt32(0);
                            int blockingId = reader.GetInt32(1);
                            string waitType = reader.GetString(2);
                            int waitTime = reader.GetInt32(3);
                            string status = reader.GetString(4);
                            string query = reader.GetString(5);

                            txtLog.AppendText($"🔸 세션 {sessionId} ▶ 블로킹 세션 {blockingId}\n");
                            txtLog.AppendText($"   ⏱ 대기 시간: {waitTime}ms, 타입: {waitType}, 상태: {status}\n");
                            txtLog.AppendText($"   🧾 쿼리: {query}\n\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ 데드락 조회 실패: {ex.Message}\n");
            }
        }


        //느린 쿼리 또는 자원 많이 쓰는 쿼리 리스트
        private void btnAnalyzePerformance_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string connectionString = $"Server={server};Integrated Security=true;";

            string perfQuery = @"
        SELECT TOP 10
            qs.execution_count,
            qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
            qs.total_worker_time / qs.execution_count AS avg_cpu_time,
            qs.total_logical_reads / qs.execution_count AS avg_reads,
            qs.total_logical_writes / qs.execution_count AS avg_writes,
            SUBSTRING(qt.text, qs.statement_start_offset / 2 + 1,
                      (CASE WHEN qs.statement_end_offset = -1
                            THEN LEN(CONVERT(nvarchar(MAX), qt.text)) * 2
                            ELSE qs.statement_end_offset END - qs.statement_start_offset) / 2) AS query_text
        FROM sys.dm_exec_query_stats qs
        CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
        ORDER BY avg_elapsed_time DESC;
        ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(perfQuery, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtLog.AppendText("📊 성능 분석 결과 (TOP 10):\n");

                        if (!reader.HasRows)
                        {
                            txtLog.AppendText("✔ 분석 가능한 실행 기록이 없습니다.\n");
                            return;
                        }

                        while (reader.Read())
                        {
                            int count = Convert.ToInt32(reader.GetValue(0));
                            long avgTime = Convert.ToInt64(reader.GetValue(1));
                            long avgCpu = Convert.ToInt64(reader.GetValue(2));
                            long reads = Convert.ToInt64(reader.GetValue(3));
                            long writes = Convert.ToInt64(reader.GetValue(4));
                            string query = reader.GetString(5);


                            txtLog.AppendText($"▶ 실행횟수: {count} / 평균 시간: {avgTime}ms / CPU: {avgCpu} / Reads: {reads} / Writes: {writes}\n");
                            //txtLog.AppendText($"   🧾 쿼리: {query}\n\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ 성능 분석 실패: {ex.Message}\n");
            }
        }

        //사용되지 않는 인덱스 조회 기능
        private void btnCheckUnusedIndexes_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text;
            string dbName = cmbDBList.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(dbName))
            {
                txtLog.AppendText("✘ 서버 또는 DB명을 선택하세요.\n");
                return;
            }

            string connectionString = $"Server={server};Database={dbName};Integrated Security=true;";

            string unusedIndexSql = @"
            SELECT 
                OBJECT_NAME(ius.object_id) AS table_name,
                i.name AS index_name,
                i.type_desc,
                ius.user_seeks,
                ius.user_scans,
                ius.user_lookups,
                ius.user_updates
            FROM sys.dm_db_index_usage_stats AS ius
            JOIN sys.indexes AS i 
                ON i.object_id = ius.object_id AND i.index_id = ius.index_id
            WHERE ius.database_id = DB_ID()
              AND i.is_primary_key = 0
              AND i.is_unique = 0
              AND ius.user_seeks = 0
              AND ius.user_scans = 0
              AND ius.user_lookups = 0
            ORDER BY ius.user_updates DESC;
            ";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(unusedIndexSql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        txtLog.AppendText("🧹 사용되지 않는 인덱스 목록:\n");

                        if (!reader.HasRows)
                        {
                            txtLog.AppendText("✔ 모든 인덱스가 사용 중입니다.\n");
                            return;
                        }

                        while (reader.Read())
                        {
                            string table = reader["table_name"].ToString();
                            string index = reader["index_name"].ToString();
                            string type = reader["type_desc"].ToString();
                            int updates = Convert.ToInt32(reader["user_updates"]);

                            txtLog.AppendText($"🔸 테이블: {table} / 인덱스: {index} / 타입: {type} / 업데이트 횟수: {updates}\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"✘ 인덱스 분석 실패: {ex.Message}\n");
            }
        }


        // 로그 출력용 텍스트박스 초기화
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            txtLog.AppendText("📭 로그가 초기화되었습니다.\n");
        }


        //스타일 함수

        private void ApplyMulticolorVisualStyleTheme()
        {
            // 다크 배경
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.Font = new Font("Segoe UI", 9);

            // 로그 텍스트박스 스타일
            txtLog.BackColor = Color.FromArgb(30, 30, 30);
            txtLog.ForeColor = Color.FromArgb(230, 230, 230);
            txtLog.Font = new Font("Consolas", 9);
            txtLog.BorderStyle = BorderStyle.FixedSingle;

            // 공통 색 정의
            Color blue = Color.FromArgb(52, 152, 219);
            Color teal = Color.FromArgb(26, 188, 156);
            Color purple = Color.FromArgb(155, 89, 182);
            Color orange = Color.FromArgb(243, 156, 18);
            Color gray = Color.FromArgb(127, 140, 141);

            foreach (Control ctl in this.Controls)
            {
                if (ctl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                    Color btnColor = gray; // 기본색

                    // 버튼 이름 또는 텍스트에 따라 색상 결정
                    if (btn.Name.ToLower().Contains("backup") || btn.Name.ToLower().Contains("restore"))
                        btnColor = blue;
                    else if (btn.Name.ToLower().Contains("analyze") || btn.Name.ToLower().Contains("checkunused"))
                        btnColor = teal;
                    else if (btn.Name.ToLower().Contains("deadlock") || btn.Name.ToLower().Contains("index"))
                        btnColor = purple;
                    else if (btn.Name.ToLower().Contains("patch") || btn.Name.ToLower().Contains("run"))
                        btnColor = orange;
                    else if (btn.Name.ToLower().Contains("clear"))
                        btnColor = gray;

                    btn.BackColor = btnColor;

                    // 마우스 오버 효과
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = ControlPaint.Light(btnColor);
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = btnColor;
                    };
                }

                if (ctl is TextBox txt)
                {
                    txt.BackColor = Color.FromArgb(37, 37, 38);
                    txt.ForeColor = Color.White;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }

                if (ctl is ComboBox cmb)
                {
                    cmb.BackColor = Color.FromArgb(37, 37, 38);
                    cmb.ForeColor = Color.White;
                    cmb.FlatStyle = FlatStyle.Flat;
                }
            }
        }

    }

}