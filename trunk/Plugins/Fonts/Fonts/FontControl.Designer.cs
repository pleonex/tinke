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
            this.txtBox = new System.Windows.Forms.TextBox();
            this.picText = new System.Windows.Forms.PictureBox();
            this.panelPicImage = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.panelCharEdit = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboEncoding = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).BeginInit();
            this.panelPicImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // picFont
            // 
            this.picFont.BackColor = System.Drawing.Color.Moccasin;
            this.picFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFont.Location = new System.Drawing.Point(0, 0);
            this.picFont.MaximumSize = new System.Drawing.Size(260, 512);
            this.picFont.Name = "picFont";
            this.picFont.Size = new System.Drawing.Size(260, 307);
            this.picFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFont.TabIndex = 0;
            this.picFont.TabStop = false;
            this.picFont.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picFont_MouseClick);
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
            // txtBox
            // 
            this.txtBox.Location = new System.Drawing.Point(4, 344);
            this.txtBox.Multiline = true;
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(505, 49);
            this.txtBox.TabIndex = 3;
            this.txtBox.TextChanged += new System.EventHandler(this.txtBox_TextChanged);
            // 
            // picText
            // 
            this.picText.BackColor = System.Drawing.SystemColors.Control;
            this.picText.Location = new System.Drawing.Point(4, 399);
            this.picText.Name = "picText";
            this.picText.Size = new System.Drawing.Size(505, 101);
            this.picText.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picText.TabIndex = 4;
            this.picText.TabStop = false;
            // 
            // panelPicImage
            // 
            this.panelPicImage.AutoScroll = true;
            this.panelPicImage.Controls.Add(this.picFont);
            this.panelPicImage.Location = new System.Drawing.Point(4, 31);
            this.panelPicImage.MaximumSize = new System.Drawing.Size(260, 307);
            this.panelPicImage.Name = "panelPicImage";
            this.panelPicImage.Size = new System.Drawing.Size(260, 307);
            this.panelPicImage.TabIndex = 5;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(430, 291);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 46);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply changes";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // panelCharEdit
            // 
            this.panelCharEdit.Location = new System.Drawing.Point(266, 31);
            this.panelCharEdit.Name = "panelCharEdit";
            this.panelCharEdit.Size = new System.Drawing.Size(243, 307);
            this.panelCharEdit.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Fonts.Properties.Resources.disk;
            this.btnSave.Location = new System.Drawing.Point(4, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 26);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save new font";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(169, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Encoding:";
            // 
            // comboEncoding
            // 
            this.comboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEncoding.FormattingEnabled = true;
            this.comboEncoding.Items.AddRange(new object[] {
            "shift-jis",
            "Unicode",
            "Unicode BigEndian"});
            this.comboEncoding.Location = new System.Drawing.Point(230, 4);
            this.comboEncoding.Name = "comboEncoding";
            this.comboEncoding.Size = new System.Drawing.Size(121, 21);
            this.comboEncoding.TabIndex = 10;
            this.comboEncoding.SelectedIndexChanged += new System.EventHandler(this.comboEncoding_SelectedIndexChanged);
            // 
            // FontControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.comboEncoding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.panelCharEdit);
            this.Controls.Add(this.panelPicImage);
            this.Controls.Add(this.picText);
            this.Controls.Add(this.txtBox);
            this.Controls.Add(this.comboBox1);
            this.Name = "FontControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).EndInit();
            this.panelPicImage.ResumeLayout(false);
            this.panelPicImage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFont;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.PictureBox picText;
        private System.Windows.Forms.Panel panelPicImage;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel panelCharEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboEncoding;
    }
}
