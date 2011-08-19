namespace NARC
{
    partial class iNARC
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCompress = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblNumFiles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(3, 3);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(100, 40);
            this.btnCompress.TabIndex = 0;
            this.btnCompress.Text = "S00";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "S01";
            // 
            // lblNumFiles
            // 
            this.lblNumFiles.AutoSize = true;
            this.lblNumFiles.Location = new System.Drawing.Point(143, 75);
            this.lblNumFiles.Name = "lblNumFiles";
            this.lblNumFiles.Size = new System.Drawing.Size(0, 13);
            this.lblNumFiles.TabIndex = 2;
            // 
            // iNARC
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblNumFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCompress);
            this.Name = "iNARC";
            this.Size = new System.Drawing.Size(510, 510);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNumFiles;
    }
}
