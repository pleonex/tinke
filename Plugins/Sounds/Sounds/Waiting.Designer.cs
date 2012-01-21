namespace Sounds
{
    partial class Waiting
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
            this.lblText = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(40, 13);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "<Text>";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(15, 50);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(257, 23);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress.TabIndex = 1;
            // 
            // Waiting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(280, 76);
            this.ControlBox = false;
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Waiting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "S01";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ProgressBar progress;
    }
}