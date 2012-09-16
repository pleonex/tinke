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
 * By: pleoNeX
 * 
 */
namespace Ekona.Images
{
    partial class PaletteControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picPalette = new System.Windows.Forms.PictureBox();
            this.numericPalette = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lblRGB = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericStartByte = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.comboDepth = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkHex = new System.Windows.Forms.CheckBox();
            this.btnUseThis = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.numFillColors = new System.Windows.Forms.NumericUpDown();
            this.btnFillColors = new System.Windows.Forms.Button();
            this.checkDuplicated = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartByte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFillColors)).BeginInit();
            this.SuspendLayout();
            // 
            // picPalette
            // 
            this.picPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPalette.Location = new System.Drawing.Point(0, 0);
            this.picPalette.Name = "picPalette";
            this.picPalette.Size = new System.Drawing.Size(160, 160);
            this.picPalette.TabIndex = 0;
            this.picPalette.TabStop = false;
            this.picPalette.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picPalette_MouseClick);
            // 
            // numericPalette
            // 
            this.numericPalette.Location = new System.Drawing.Point(101, 182);
            this.numericPalette.Name = "numericPalette";
            this.numericPalette.Size = new System.Drawing.Size(37, 20);
            this.numericPalette.TabIndex = 2;
            this.numericPalette.ValueChanged += new System.EventHandler(this.numericPalette_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "S01";
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(6, 208);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(160, 30);
            this.btnShow.TabIndex = 4;
            this.btnShow.Text = "S02";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(343, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 40);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "S03";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblRGB
            // 
            this.lblRGB.AutoSize = true;
            this.lblRGB.Location = new System.Drawing.Point(3, 163);
            this.lblRGB.Name = "lblRGB";
            this.lblRGB.Size = new System.Drawing.Size(33, 13);
            this.lblRGB.TabIndex = 7;
            this.lblRGB.Text = "RGB:";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(429, 3);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(80, 40);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "S04";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(313, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "S05";
            // 
            // numericStartByte
            // 
            this.numericStartByte.Location = new System.Drawing.Point(400, 95);
            this.numericStartByte.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericStartByte.Name = "numericStartByte";
            this.numericStartByte.Size = new System.Drawing.Size(106, 20);
            this.numericStartByte.TabIndex = 0;
            this.numericStartByte.ValueChanged += new System.EventHandler(this.numericStartByte_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(142, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "S07";
            // 
            // comboDepth
            // 
            this.comboDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDepth.FormattingEnabled = true;
            this.comboDepth.Items.AddRange(new object[] {
            "16 / 16 (4bpp)",
            "256 / 1 (8bpp)"});
            this.comboDepth.Location = new System.Drawing.Point(400, 121);
            this.comboDepth.Name = "comboDepth";
            this.comboDepth.Size = new System.Drawing.Size(106, 21);
            this.comboDepth.TabIndex = 10;
            this.comboDepth.SelectedIndexChanged += new System.EventHandler(this.comboDepth_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(313, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "S06";
            // 
            // checkHex
            // 
            this.checkHex.AutoSize = true;
            this.checkHex.Location = new System.Drawing.Point(400, 72);
            this.checkHex.Name = "checkHex";
            this.checkHex.Size = new System.Drawing.Size(46, 17);
            this.checkHex.TabIndex = 12;
            this.checkHex.Text = "S0B";
            this.checkHex.UseVisualStyleBackColor = true;
            this.checkHex.CheckedChanged += new System.EventHandler(this.checkHex_CheckedChanged);
            // 
            // btnUseThis
            // 
            this.btnUseThis.Location = new System.Drawing.Point(166, 3);
            this.btnUseThis.Name = "btnUseThis";
            this.btnUseThis.Size = new System.Drawing.Size(80, 40);
            this.btnUseThis.TabIndex = 13;
            this.btnUseThis.Text = "S0A";
            this.btnUseThis.UseVisualStyleBackColor = true;
            this.btnUseThis.Click += new System.EventHandler(this.btnUseThis_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(259, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "S0C";
            // 
            // numFillColors
            // 
            this.numFillColors.Location = new System.Drawing.Point(345, 206);
            this.numFillColors.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numFillColors.Name = "numFillColors";
            this.numFillColors.Size = new System.Drawing.Size(78, 20);
            this.numFillColors.TabIndex = 15;
            // 
            // btnFillColors
            // 
            this.btnFillColors.Location = new System.Drawing.Point(429, 194);
            this.btnFillColors.Name = "btnFillColors";
            this.btnFillColors.Size = new System.Drawing.Size(80, 40);
            this.btnFillColors.TabIndex = 16;
            this.btnFillColors.Text = "S0D";
            this.btnFillColors.UseVisualStyleBackColor = true;
            this.btnFillColors.Click += new System.EventHandler(this.btnFillColors_Click);
            // 
            // checkDuplicated
            // 
            this.checkDuplicated.AutoSize = true;
            this.checkDuplicated.Enabled = false;
            this.checkDuplicated.Location = new System.Drawing.Point(166, 72);
            this.checkDuplicated.Name = "checkDuplicated";
            this.checkDuplicated.Size = new System.Drawing.Size(128, 17);
            this.checkDuplicated.TabIndex = 17;
            this.checkDuplicated.Text = "Has duplicated colors";
            this.checkDuplicated.UseVisualStyleBackColor = true;
            // 
            // PaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.checkDuplicated);
            this.Controls.Add(this.btnFillColors);
            this.Controls.Add(this.numFillColors);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnUseThis);
            this.Controls.Add(this.checkHex);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboDepth);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericStartByte);
            this.Controls.Add(this.lblRGB);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericPalette);
            this.Controls.Add(this.picPalette);
            this.Name = "PaletteControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartByte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFillColors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPalette;
        private System.Windows.Forms.NumericUpDown numericPalette;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblRGB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericStartByte;
        internal System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboDepth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkHex;
        private System.Windows.Forms.Button btnUseThis;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numFillColors;
        private System.Windows.Forms.Button btnFillColors;
        private System.Windows.Forms.CheckBox checkDuplicated;
    }
}
