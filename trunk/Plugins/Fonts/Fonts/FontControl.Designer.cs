/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
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
            this.comboChar = new System.Windows.Forms.ComboBox();
            this.txtBox = new System.Windows.Forms.TextBox();
            this.panelPicImage = new System.Windows.Forms.Panel();
            this.picFont = new System.Windows.Forms.PictureBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.panelCharEdit = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboEncoding = new System.Windows.Forms.ComboBox();
            this.btnPalette = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.picText = new System.Windows.Forms.PictureBox();
            this.btnChangeMap = new System.Windows.Forms.Button();
            this.btnAddChar = new System.Windows.Forms.Button();
            this.btnRemoveChar = new System.Windows.Forms.Button();
            this.btnExportInfo = new System.Windows.Forms.Button();
            this.btnToImage = new System.Windows.Forms.Button();
            this.btnFromImage = new System.Windows.Forms.Button();
            this.btnInversePalette = new System.Windows.Forms.Button();
            this.panelPicImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).BeginInit();
            this.SuspendLayout();
            // 
            // comboChar
            // 
            this.comboChar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboChar.FormattingEnabled = true;
            this.comboChar.Location = new System.Drawing.Point(388, 3);
            this.comboChar.Name = "comboChar";
            this.comboChar.Size = new System.Drawing.Size(121, 21);
            this.comboChar.TabIndex = 3;
            this.comboChar.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // txtBox
            // 
            this.txtBox.Location = new System.Drawing.Point(0, 371);
            this.txtBox.Multiline = true;
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(509, 50);
            this.txtBox.TabIndex = 0;
            this.txtBox.TextChanged += new System.EventHandler(this.txtBox_TextChanged);
            // 
            // panelPicImage
            // 
            this.panelPicImage.AutoScroll = true;
            this.panelPicImage.BackColor = System.Drawing.Color.Moccasin;
            this.panelPicImage.Controls.Add(this.picFont);
            this.panelPicImage.Location = new System.Drawing.Point(2, 29);
            this.panelPicImage.MaximumSize = new System.Drawing.Size(263, 310);
            this.panelPicImage.Name = "panelPicImage";
            this.panelPicImage.Size = new System.Drawing.Size(263, 310);
            this.panelPicImage.TabIndex = 14;
            // 
            // picFont
            // 
            this.picFont.BackColor = System.Drawing.Color.Transparent;
            this.picFont.Location = new System.Drawing.Point(0, 0);
            this.picFont.Name = "picFont";
            this.picFont.Size = new System.Drawing.Size(260, 310);
            this.picFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFont.TabIndex = 0;
            this.picFont.TabStop = false;
            this.picFont.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picFont_MouseClick);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(432, 290);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 46);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "S01";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // panelCharEdit
            // 
            this.panelCharEdit.AutoScroll = true;
            this.panelCharEdit.Location = new System.Drawing.Point(266, 29);
            this.panelCharEdit.Name = "panelCharEdit";
            this.panelCharEdit.Size = new System.Drawing.Size(243, 310);
            this.panelCharEdit.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "S02";
            // 
            // comboEncoding
            // 
            this.comboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEncoding.FormattingEnabled = true;
            this.comboEncoding.Items.AddRange(new object[] {
            "utf-8",
            "utf-16",
            "shift_jis",
            "CP1252"});
            this.comboEncoding.Location = new System.Drawing.Point(230, 4);
            this.comboEncoding.Name = "comboEncoding";
            this.comboEncoding.Size = new System.Drawing.Size(121, 21);
            this.comboEncoding.TabIndex = 11;
            this.comboEncoding.SelectedIndexChanged += new System.EventHandler(this.comboEncoding_SelectedIndexChanged);
            // 
            // btnPalette
            // 
            this.btnPalette.Image = global::Fonts.Properties.Resources.palette;
            this.btnPalette.Location = new System.Drawing.Point(4, 483);
            this.btnPalette.Name = "btnPalette";
            this.btnPalette.Size = new System.Drawing.Size(149, 26);
            this.btnPalette.TabIndex = 10;
            this.btnPalette.Text = "S05";
            this.btnPalette.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPalette.UseVisualStyleBackColor = true;
            this.btnPalette.Click += new System.EventHandler(this.btnPalette_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Fonts.Properties.Resources.disk;
            this.btnSave.Location = new System.Drawing.Point(0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(163, 26);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "S00";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // picText
            // 
            this.picText.BackColor = System.Drawing.SystemColors.Control;
            this.picText.Location = new System.Drawing.Point(2, 427);
            this.picText.Name = "picText";
            this.picText.Size = new System.Drawing.Size(505, 50);
            this.picText.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picText.TabIndex = 4;
            this.picText.TabStop = false;
            // 
            // btnChangeMap
            // 
            this.btnChangeMap.Image = global::Fonts.Properties.Resources.font_go;
            this.btnChangeMap.Location = new System.Drawing.Point(159, 483);
            this.btnChangeMap.Name = "btnChangeMap";
            this.btnChangeMap.Size = new System.Drawing.Size(142, 26);
            this.btnChangeMap.TabIndex = 9;
            this.btnChangeMap.Text = "S06";
            this.btnChangeMap.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChangeMap.UseVisualStyleBackColor = true;
            this.btnChangeMap.Click += new System.EventHandler(this.btnChangeMap_Click);
            // 
            // btnAddChar
            // 
            this.btnAddChar.Image = global::Fonts.Properties.Resources.font_add;
            this.btnAddChar.Location = new System.Drawing.Point(2, 342);
            this.btnAddChar.Name = "btnAddChar";
            this.btnAddChar.Size = new System.Drawing.Size(130, 23);
            this.btnAddChar.TabIndex = 5;
            this.btnAddChar.Text = "S03";
            this.btnAddChar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddChar.UseVisualStyleBackColor = true;
            this.btnAddChar.Click += new System.EventHandler(this.btnAddChar_Click);
            // 
            // btnRemoveChar
            // 
            this.btnRemoveChar.Image = global::Fonts.Properties.Resources.font_delete;
            this.btnRemoveChar.Location = new System.Drawing.Point(135, 342);
            this.btnRemoveChar.Name = "btnRemoveChar";
            this.btnRemoveChar.Size = new System.Drawing.Size(130, 23);
            this.btnRemoveChar.TabIndex = 6;
            this.btnRemoveChar.Text = "S04";
            this.btnRemoveChar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRemoveChar.UseVisualStyleBackColor = true;
            this.btnRemoveChar.Click += new System.EventHandler(this.btnRemoveChar_Click);
            // 
            // btnExportInfo
            // 
            this.btnExportInfo.Location = new System.Drawing.Point(379, 342);
            this.btnExportInfo.Name = "btnExportInfo";
            this.btnExportInfo.Size = new System.Drawing.Size(130, 23);
            this.btnExportInfo.TabIndex = 8;
            this.btnExportInfo.Text = "Export font info";
            this.btnExportInfo.UseVisualStyleBackColor = true;
            this.btnExportInfo.Click += new System.EventHandler(this.btnExportInfo_Click);
            // 
            // btnToImage
            // 
            this.btnToImage.Location = new System.Drawing.Point(415, 483);
            this.btnToImage.Name = "btnToImage";
            this.btnToImage.Size = new System.Drawing.Size(92, 26);
            this.btnToImage.TabIndex = 2;
            this.btnToImage.Text = "Export to image";
            this.btnToImage.UseVisualStyleBackColor = true;
            this.btnToImage.Click += new System.EventHandler(this.btnToImage_Click);
            // 
            // btnFromImage
            // 
            this.btnFromImage.Location = new System.Drawing.Point(307, 483);
            this.btnFromImage.Name = "btnFromImage";
            this.btnFromImage.Size = new System.Drawing.Size(102, 26);
            this.btnFromImage.TabIndex = 1;
            this.btnFromImage.Text = "Import from image";
            this.btnFromImage.UseVisualStyleBackColor = true;
            this.btnFromImage.Click += new System.EventHandler(this.btnFromImage_Click);
            // 
            // btnInversePalette
            // 
            this.btnInversePalette.Location = new System.Drawing.Point(266, 342);
            this.btnInversePalette.Name = "btnInversePalette";
            this.btnInversePalette.Size = new System.Drawing.Size(107, 23);
            this.btnInversePalette.TabIndex = 7;
            this.btnInversePalette.Text = "Inverse palette";
            this.btnInversePalette.UseVisualStyleBackColor = true;
            this.btnInversePalette.Click += new System.EventHandler(this.btnInversePalette_Click);
            // 
            // FontControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnInversePalette);
            this.Controls.Add(this.btnFromImage);
            this.Controls.Add(this.btnToImage);
            this.Controls.Add(this.btnExportInfo);
            this.Controls.Add(this.btnRemoveChar);
            this.Controls.Add(this.btnAddChar);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnChangeMap);
            this.Controls.Add(this.btnPalette);
            this.Controls.Add(this.comboEncoding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelCharEdit);
            this.Controls.Add(this.panelPicImage);
            this.Controls.Add(this.picText);
            this.Controls.Add(this.txtBox);
            this.Controls.Add(this.comboChar);
            this.Name = "FontControl";
            this.Size = new System.Drawing.Size(512, 512);
            this.panelPicImage.ResumeLayout(false);
            this.panelPicImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picText)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFont;
        private System.Windows.Forms.ComboBox comboChar;
        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.PictureBox picText;
        private System.Windows.Forms.Panel panelPicImage;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Panel panelCharEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboEncoding;
        private System.Windows.Forms.Button btnPalette;
        private System.Windows.Forms.Button btnChangeMap;
        private System.Windows.Forms.Button btnAddChar;
        private System.Windows.Forms.Button btnRemoveChar;
        private System.Windows.Forms.Button btnExportInfo;
        private System.Windows.Forms.Button btnToImage;
        private System.Windows.Forms.Button btnFromImage;
        private System.Windows.Forms.Button btnInversePalette;
    }
}
