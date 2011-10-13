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
namespace AI_IGO_DS
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
            this.picPaleta = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPaleta)).BeginInit();
            this.SuspendLayout();
            // 
            // picPaleta
            // 
            this.picPaleta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPaleta.Location = new System.Drawing.Point(4, 4);
            this.picPaleta.Name = "picPaleta";
            this.picPaleta.Size = new System.Drawing.Size(160, 160);
            this.picPaleta.TabIndex = 0;
            this.picPaleta.TabStop = false;
            // 
            // PaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.picPaleta);
            this.Name = "PaletteControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picPaleta)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picPaleta;
    }
}
