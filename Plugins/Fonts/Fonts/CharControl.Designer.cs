namespace Fonts
{
    partial class CharControl
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
            this.picFont = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).BeginInit();
            this.SuspendLayout();
            // 
            // picFont
            // 
            this.picFont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picFont.Location = new System.Drawing.Point(0, 0);
            this.picFont.Name = "picFont";
            this.picFont.Size = new System.Drawing.Size(100, 50);
            this.picFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFont.TabIndex = 0;
            this.picFont.TabStop = false;
            // 
            // CharControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.picFont);
            this.Name = "CharControl";
            this.Size = new System.Drawing.Size(100, 50);
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFont;
    }
}
