namespace EDGEWORTH
{
    partial class PackControl
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPack
            // 
            this.btnPack.Image = global::EDGEWORTH.Properties.Resources.package_go;
            this.btnPack.Location = new System.Drawing.Point(3, 3);
            this.btnPack.Name = "btnPack";
            this.btnPack.Size = new System.Drawing.Size(112, 36);
            this.btnPack.TabIndex = 0;
            this.btnPack.Text = "S00";
            this.btnPack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.button1_Click);
            // 
            // PackControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnPack);
            this.Name = "PackControl";
            this.Size = new System.Drawing.Size(512, 512);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPack;
    }
}
