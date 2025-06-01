using System.Windows.Forms;

namespace AutoFailover
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtServer = new System.Windows.Forms.TextBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnSelectBackupFile = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.btnSelectPatchFolder = new System.Windows.Forms.Button();
            this.btnRunPatch = new System.Windows.Forms.Button();
            this.txtPatchFolder = new System.Windows.Forms.TextBox();
            this.restoreCheck = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCheckDeadlock = new System.Windows.Forms.Button();
            this.btnAnalyzePerformance = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCheckUnusedIndexes = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(25, 24);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(138, 21);
            this.txtServer.TabIndex = 0;
            this.txtServer.Text = "ServerIP";
            this.txtServer.TextChanged += new System.EventHandler(this.txtServer_TextChanged);
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(25, 63);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(138, 29);
            this.btnBackup.TabIndex = 2;
            this.btnBackup.Text = "💾 Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(25, 127);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(138, 29);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "🔁 Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(25, 272);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(650, 286);
            this.txtLog.TabIndex = 5;
            this.txtLog.Text = "";
            // 
            // btnSelectBackupFile
            // 
            this.btnSelectBackupFile.Location = new System.Drawing.Point(25, 98);
            this.btnSelectBackupFile.Name = "btnSelectBackupFile";
            this.btnSelectBackupFile.Size = new System.Drawing.Size(138, 23);
            this.btnSelectBackupFile.TabIndex = 6;
            this.btnSelectBackupFile.Text = "BackupFile Select";
            this.btnSelectBackupFile.UseVisualStyleBackColor = true;
            this.btnSelectBackupFile.Click += new System.EventHandler(this.btnSelectBackupFile_Click);
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Location = new System.Drawing.Point(169, 100);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(333, 21);
            this.txtBackupPath.TabIndex = 7;
            this.txtBackupPath.Text = "BackupPath";
            // 
            // cmbDBList
            // 
            this.cmbDBList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBList.FormattingEnabled = true;
            this.cmbDBList.Location = new System.Drawing.Point(296, 24);
            this.cmbDBList.Name = "cmbDBList";
            this.cmbDBList.Size = new System.Drawing.Size(121, 20);
            this.cmbDBList.TabIndex = 8;
            // 
            // btnSelectPatchFolder
            // 
            this.btnSelectPatchFolder.Location = new System.Drawing.Point(25, 224);
            this.btnSelectPatchFolder.Name = "btnSelectPatchFolder";
            this.btnSelectPatchFolder.Size = new System.Drawing.Size(138, 29);
            this.btnSelectPatchFolder.TabIndex = 9;
            this.btnSelectPatchFolder.Text = "SelectPatchFolder";
            this.btnSelectPatchFolder.UseVisualStyleBackColor = true;
            this.btnSelectPatchFolder.Click += new System.EventHandler(this.btnSelectPatchFolder_Click);
            // 
            // btnRunPatch
            // 
            this.btnRunPatch.Location = new System.Drawing.Point(169, 224);
            this.btnRunPatch.Name = "btnRunPatch";
            this.btnRunPatch.Size = new System.Drawing.Size(121, 29);
            this.btnRunPatch.TabIndex = 10;
            this.btnRunPatch.Text = "RunPatch";
            this.btnRunPatch.UseVisualStyleBackColor = true;
            this.btnRunPatch.Click += new System.EventHandler(this.btnRunPatch_Click);
            // 
            // txtPatchFolder
            // 
            this.txtPatchFolder.Location = new System.Drawing.Point(296, 232);
            this.txtPatchFolder.Name = "txtPatchFolder";
            this.txtPatchFolder.Size = new System.Drawing.Size(206, 21);
            this.txtPatchFolder.TabIndex = 11;
            this.txtPatchFolder.Text = "패치실행폴더";
            // 
            // restoreCheck
            // 
            this.restoreCheck.Location = new System.Drawing.Point(169, 127);
            this.restoreCheck.Name = "restoreCheck";
            this.restoreCheck.Size = new System.Drawing.Size(138, 29);
            this.restoreCheck.TabIndex = 12;
            this.restoreCheck.Text = "RestoreCheck";
            this.restoreCheck.UseVisualStyleBackColor = true;
            this.restoreCheck.Click += new System.EventHandler(this.restoreCheck_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(261, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "DB :";
            // 
            // btnCheckDeadlock
            // 
            this.btnCheckDeadlock.Location = new System.Drawing.Point(537, 57);
            this.btnCheckDeadlock.Name = "btnCheckDeadlock";
            this.btnCheckDeadlock.Size = new System.Drawing.Size(138, 29);
            this.btnCheckDeadlock.TabIndex = 14;
            this.btnCheckDeadlock.Text = "CheckDeadlock";
            this.btnCheckDeadlock.UseVisualStyleBackColor = true;
            this.btnCheckDeadlock.Click += new System.EventHandler(this.btnCheckDeadlock_Click);
            // 
            // btnAnalyzePerformance
            // 
            this.btnAnalyzePerformance.Location = new System.Drawing.Point(537, 92);
            this.btnAnalyzePerformance.Name = "btnAnalyzePerformance";
            this.btnAnalyzePerformance.Size = new System.Drawing.Size(138, 29);
            this.btnAnalyzePerformance.TabIndex = 15;
            this.btnAnalyzePerformance.Text = "AnalyzePerformance";
            this.btnAnalyzePerformance.UseVisualStyleBackColor = true;
            this.btnAnalyzePerformance.Click += new System.EventHandler(this.btnAnalyzePerformance_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(535, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "Tuning";
            // 
            // btnCheckUnusedIndexes
            // 
            this.btnCheckUnusedIndexes.Location = new System.Drawing.Point(537, 127);
            this.btnCheckUnusedIndexes.Name = "btnCheckUnusedIndexes";
            this.btnCheckUnusedIndexes.Size = new System.Drawing.Size(138, 29);
            this.btnCheckUnusedIndexes.TabIndex = 17;
            this.btnCheckUnusedIndexes.Text = "CheckUnusedIndexes";
            this.btnCheckUnusedIndexes.UseVisualStyleBackColor = true;
            this.btnCheckUnusedIndexes.Click += new System.EventHandler(this.btnCheckUnusedIndexes_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(681, 529);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(80, 29);
            this.btnClearLog.TabIndex = 18;
            this.btnClearLog.Text = "ClearLog";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 580);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnCheckUnusedIndexes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAnalyzePerformance);
            this.Controls.Add(this.btnCheckDeadlock);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.restoreCheck);
            this.Controls.Add(this.txtPatchFolder);
            this.Controls.Add(this.btnRunPatch);
            this.Controls.Add(this.btnSelectPatchFolder);
            this.Controls.Add(this.cmbDBList);
            this.Controls.Add(this.txtBackupPath);
            this.Controls.Add(this.btnSelectBackupFile);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.txtServer);
            this.Name = "Form1";
            this.Text = "DBA Helper";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnSelectBackupFile;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.ComboBox cmbDBList;
        private Button btnSelectPatchFolder;
        private Button btnRunPatch;
        private TextBox txtPatchFolder;
        private Button restoreCheck;
        private Label label1;
        private Button btnCheckDeadlock;
        private Button btnAnalyzePerformance;
        private Label label2;
        private Button btnCheckUnusedIndexes;
        private Button btnClearLog;
    }
}

