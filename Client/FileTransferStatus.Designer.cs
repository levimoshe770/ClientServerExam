namespace Client
{
    partial class FileTransferStatus
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
            this.pgComplete = new System.Windows.Forms.ProgressBar();
            this.lblPctg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pgComplete
            // 
            this.pgComplete.Location = new System.Drawing.Point(12, 12);
            this.pgComplete.Name = "pgComplete";
            this.pgComplete.Size = new System.Drawing.Size(374, 23);
            this.pgComplete.Step = 5;
            this.pgComplete.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgComplete.TabIndex = 0;
            this.pgComplete.Value = 50;
            // 
            // lblPctg
            // 
            this.lblPctg.AutoSize = true;
            this.lblPctg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPctg.Location = new System.Drawing.Point(406, 16);
            this.lblPctg.Name = "lblPctg";
            this.lblPctg.Size = new System.Drawing.Size(37, 16);
            this.lblPctg.TabIndex = 1;
            this.lblPctg.Text = "50%";
            // 
            // FileTransferStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 52);
            this.ControlBox = false;
            this.Controls.Add(this.lblPctg);
            this.Controls.Add(this.pgComplete);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileTransferStatus";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Transfer Status";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgComplete;
        private System.Windows.Forms.Label lblPctg;
    }
}