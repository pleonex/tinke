namespace Fonts
{
    partial class FontControl
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.picChar = new System.Windows.Forms.PictureBox();
            this.txtBox = new System.Windows.Forms.TextBox();
            this.picText = new System.Windows.Forms.PictureBox();
            this.panelPicImage = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).BeginInit();
            this.panelPicImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // picFont
            // 
            this.picFont.BackColor = System.Drawing.Color.Moccasin;
            this.picFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFont.Location = new System.Drawing.Point(0, 0);
            this.picFont.MaximumSize = new System.Drawing.Size(256, 512);
            this.picFont.Name = "picFont";
            this.picFont.Size = new System.Drawing.Size(256, 256);
            this.picFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFont.TabIndex = 0;
            this.picFont.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(388, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // picChar
            // 
            this.picChar.BackColor = System.Drawing.Color.Moccasin;
            this.picChar.Location = new System.Drawing.Point(266, 31);
            this.picChar.Name = "picChar";
            this.picChar.Size = new System.Drawing.Size(243, 93);
            this.picChar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picChar.TabIndex = 2;
            this.picChar.TabStop = false;
            // 
            // txtBox
            // 
            this.txtBox.Location = new System.Drawing.Point(4, 301);
            this.txtBox.Multiline = true;
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(505, 49);
            this.txtBox.TabIndex = 3;
            this.txtBox.TextChanged += new System.EventHandler(this.txtBox_TextChanged);
            // 
            // picText
            // 
            this.picText.Location = new System.Drawing.Point(4, 357);
            this.picText.Name = "picText";
            this.picText.Size = new System.Drawing.Size(505, 143);
            this.picText.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picText.TabIndex = 4;
            this.picText.TabStop = false;
            // 
            // panelPicImage
            // 
            this.panelPicImage.AutoScroll = true;
            this.panelPicImage.Controls.Add(this.picFont);
            this.panelPicImage.Location = new System.Drawing.Point(4, 31);
            this.panelPicImage.MaximumSize = new System.Drawing.Size(256, 256);
            this.panelPicImage.Name = "panelPicImage";
            this.panelPicImage.Size = new System.Drawing.Size(256, 256);
            this.panelPicImage.TabIndex = 5;
            // 
            // FontControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelPicImage);
            this.Controls.Add(this.picText);
            this.Controls.Add(this.txtBox);
            this.Controls.Add(this.picChar);
            this.Controls.Add(this.comboBox1);
            this.Name = "FontControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picChar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).EndInit();
            this.panelPicImage.ResumeLayout(false);
            this.panelPicImage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFont;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox picChar;
        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.PictureBox picText;
        private System.Windows.Forms.Panel panelPicImage;
    }
}
