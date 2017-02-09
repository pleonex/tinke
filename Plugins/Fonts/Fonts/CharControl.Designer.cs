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
            this.numericWidth = new System.Windows.Forms.NumericUpDown();
            this.numericLength = new System.Windows.Forms.NumericUpDown();
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.trackPalette = new System.Windows.Forms.TrackBar();
            this.picPaletteColour = new System.Windows.Forms.PictureBox();
            this.txtCharCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelPic = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPaletteColour)).BeginInit();
            this.panelPic.SuspendLayout();
            this.SuspendLayout();
            // 
            // picFont
            // 
            this.picFont.BackColor = System.Drawing.Color.Moccasin;
            this.picFont.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picFont.Location = new System.Drawing.Point(0, 0);
            this.picFont.Name = "picFont";
            this.picFont.Size = new System.Drawing.Size(243, 200);
            this.picFont.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picFont.TabIndex = 0;
            this.picFont.TabStop = false;
            this.picFont.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picFont_MouseDown);
            this.picFont.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picFont_MouseMove);
            this.picFont.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picFont_MouseUp);
            // 
            // numericWidth
            // 
            this.numericWidth.Location = new System.Drawing.Point(55, 207);
            this.numericWidth.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(50, 20);
            this.numericWidth.TabIndex = 1;
            this.numericWidth.ValueChanged += new System.EventHandler(this.numericWidth_ValueChanged);
            // 
            // numericLength
            // 
            this.numericLength.Location = new System.Drawing.Point(55, 234);
            this.numericLength.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericLength.Name = "numericLength";
            this.numericLength.Size = new System.Drawing.Size(50, 20);
            this.numericLength.TabIndex = 2;
            this.numericLength.ValueChanged += new System.EventHandler(this.numericLength_ValueChanged);
            // 
            // numericStart
            // 
            this.numericStart.Location = new System.Drawing.Point(190, 206);
            this.numericStart.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.numericStart.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            -2147483648});
            this.numericStart.Name = "numericStart";
            this.numericStart.Size = new System.Drawing.Size(50, 20);
            this.numericStart.TabIndex = 3;
            this.numericStart.ValueChanged += new System.EventHandler(this.numericStart_ValueChanged);
            // 
            // trackPalette
            // 
            this.trackPalette.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.trackPalette.Location = new System.Drawing.Point(55, 260);
            this.trackPalette.Name = "trackPalette";
            this.trackPalette.Size = new System.Drawing.Size(104, 45);
            this.trackPalette.TabIndex = 5;
            this.trackPalette.Scroll += new System.EventHandler(this.trackPalette_Scroll);
            // 
            // picPaletteColour
            // 
            this.picPaletteColour.Location = new System.Drawing.Point(4, 260);
            this.picPaletteColour.Name = "picPaletteColour";
            this.picPaletteColour.Size = new System.Drawing.Size(45, 45);
            this.picPaletteColour.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPaletteColour.TabIndex = 6;
            this.picPaletteColour.TabStop = false;
            // 
            // txtCharCode
            // 
            this.txtCharCode.Location = new System.Drawing.Point(190, 233);
            this.txtCharCode.Name = "txtCharCode";
            this.txtCharCode.ReadOnly = true;
            this.txtCharCode.Size = new System.Drawing.Size(49, 20);
            this.txtCharCode.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "S00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "S02";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "S01";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(113, 236);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "S03";
            // 
            // panelPic
            // 
            this.panelPic.AutoScroll = true;
            this.panelPic.Controls.Add(this.picFont);
            this.panelPic.Location = new System.Drawing.Point(0, 0);
            this.panelPic.Name = "panelPic";
            this.panelPic.Size = new System.Drawing.Size(243, 203);
            this.panelPic.TabIndex = 12;
            // 
            // CharControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelPic);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCharCode);
            this.Controls.Add(this.picPaletteColour);
            this.Controls.Add(this.trackPalette);
            this.Controls.Add(this.numericStart);
            this.Controls.Add(this.numericLength);
            this.Controls.Add(this.numericWidth);
            this.MinimumSize = new System.Drawing.Size(243, 308);
            this.Name = "CharControl";
            this.Size = new System.Drawing.Size(243, 308);
            ((System.ComponentModel.ISupportInitialize)(this.picFont)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPaletteColour)).EndInit();
            this.panelPic.ResumeLayout(false);
            this.panelPic.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picFont;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.NumericUpDown numericLength;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.TrackBar trackPalette;
        private System.Windows.Forms.PictureBox picPaletteColour;
        private System.Windows.Forms.TextBox txtCharCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelPic;
    }
}
