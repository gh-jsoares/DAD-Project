namespace PuppetMaster
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbCommandLog = new System.Windows.Forms.TextBox();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.btnUploadFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbCommandLog
            // 
            this.tbCommandLog.Location = new System.Drawing.Point(49, 29);
            this.tbCommandLog.Multiline = true;
            this.tbCommandLog.Name = "tbCommandLog";
            this.tbCommandLog.ReadOnly = true;
            this.tbCommandLog.Size = new System.Drawing.Size(360, 336);
            this.tbCommandLog.TabIndex = 0;
            this.tbCommandLog.TextChanged += new System.EventHandler(this.tbCommandLog_TextChanged);
            // 
            // tbCommand
            // 
            this.tbCommand.Location = new System.Drawing.Point(49, 394);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(360, 23);
            this.tbCommand.TabIndex = 1;
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(602, 39);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(350, 23);
            this.tbFileName.TabIndex = 2;
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(415, 394);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(128, 23);
            this.btnSendCommand.TabIndex = 3;
            this.btnSendCommand.Text = "Send Command";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // btnUploadFile
            // 
            this.btnUploadFile.Location = new System.Drawing.Point(824, 68);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(128, 23);
            this.btnUploadFile.TabIndex = 3;
            this.btnUploadFile.Text = "Upload File";
            this.btnUploadFile.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 450);
            this.Controls.Add(this.btnUploadFile);
            this.Controls.Add(this.btnSendCommand);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.tbCommand);
            this.Controls.Add(this.tbCommandLog);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCommandLog;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.Button btnUploadFile;
    }
}

