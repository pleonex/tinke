namespace _3DModels
{
    partial class TextureControl
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
            this.picTex = new System.Windows.Forms.PictureBox();
            this.picPalette = new System.Windows.Forms.PictureBox();
            this.numericPalette = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericTexture = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTexName = new System.Windows.Forms.Label();
            this.lblPalName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picTex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTexture)).BeginInit();
            this.SuspendLayout();
            // 
            // picTex
            // 
            this.picTex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTex.Location = new System.Drawing.Point(3, 57);
            this.picTex.Name = "picTex";
            this.picTex.Size = new System.Drawing.Size(100, 100);
            this.picTex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picTex.TabIndex = 0;
            this.picTex.TabStop = false;
            // 
            // picPalette
            // 
            this.picPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPalette.Location = new System.Drawing.Point(349, 349);
            this.picPalette.Name = "picPalette";
            this.picPalette.Size = new System.Drawing.Size(160, 160);
            this.picPalette.TabIndex = 1;
            this.picPalette.TabStop = false;
            // 
            // numericPalette
            // 
            this.numericPalette.Location = new System.Drawing.Point(349, 323);
            this.numericPalette.Name = "numericPalette";
            this.numericPalette.Size = new System.Drawing.Size(50, 20);
            this.numericPalette.TabIndex = 2;
            this.numericPalette.ValueChanged += new System.EventHandler(this.numericPalette_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(346, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "# of palette:";
            // 
            // numericTexture
            // 
            this.numericTexture.Location = new System.Drawing.Point(3, 31);
            this.numericTexture.Name = "numericTexture";
            this.numericTexture.Size = new System.Drawing.Size(50, 20);
            this.numericTexture.TabIndex = 4;
            this.numericTexture.ValueChanged += new System.EventHandler(this.numericTexture_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "# of texture:";
            // 
            // lblTexName
            // 
            this.lblTexName.AutoSize = true;
            this.lblTexName.Location = new System.Drawing.Point(59, 33);
            this.lblTexName.Name = "lblTexName";
            this.lblTexName.Size = new System.Drawing.Size(72, 13);
            this.lblTexName.TabIndex = 6;
            this.lblTexName.Text = "Texture name";
            // 
            // lblPalName
            // 
            this.lblPalName.AutoSize = true;
            this.lblPalName.Location = new System.Drawing.Point(405, 325);
            this.lblPalName.Name = "lblPalName";
            this.lblPalName.Size = new System.Drawing.Size(69, 13);
            this.lblPalName.TabIndex = 7;
            this.lblPalName.Text = "Palette name";
            // 
            // TextureControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblPalName);
            this.Controls.Add(this.lblTexName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericTexture);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericPalette);
            this.Controls.Add(this.picPalette);
            this.Controls.Add(this.picTex);
            this.Name = "TextureControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picTex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTexture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picTex;
        private System.Windows.Forms.PictureBox picPalette;
        private System.Windows.Forms.NumericUpDown numericPalette;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericTexture;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTexName;
        private System.Windows.Forms.Label lblPalName;
    }
}
