namespace Tinke
{
    partial class Debug
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debug));
            this.txtInfo = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.AllowWebBrowserDrop = false;
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.IsWebBrowserContextMenuEnabled = false;
            this.txtInfo.Location = new System.Drawing.Point(0, 0);
            this.txtInfo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScriptErrorsSuppressed = true;
            this.txtInfo.Size = new System.Drawing.Size(644, 152);
            this.txtInfo.TabIndex = 0;
            this.txtInfo.WebBrowserShortcutsEnabled = false;
            // 
            // Debug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 152);
            this.ControlBox = false;
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Debug";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mensajes";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser txtInfo;


    }
}