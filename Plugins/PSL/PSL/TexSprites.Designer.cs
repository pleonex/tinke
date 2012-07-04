// ----------------------------------------------------------------------
// <copyright file="TexSprites.Designer.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/04/2012 15:01:11</date>
// -----------------------------------------------------------------------
namespace PSL
{
    partial class TexSprites
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
            this.spriteControl1 = new Ekona.Images.SpriteControl();
            this.btnExtract = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numImg = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numImg)).BeginInit();
            this.SuspendLayout();
            // 
            // spriteControl1
            // 
            this.spriteControl1.BackColor = System.Drawing.Color.Transparent;
            this.spriteControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spriteControl1.Location = new System.Drawing.Point(0, 0);
            this.spriteControl1.Name = "spriteControl1";
            this.spriteControl1.Size = new System.Drawing.Size(512, 512);
            this.spriteControl1.TabIndex = 0;
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(6, 34);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(101, 23);
            this.btnExtract.TabIndex = 7;
            this.btnExtract.Text = "Extract all";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(152, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "of ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Image to load:";
            // 
            // numImg
            // 
            this.numImg.Location = new System.Drawing.Point(83, 8);
            this.numImg.Name = "numImg";
            this.numImg.Size = new System.Drawing.Size(63, 20);
            this.numImg.TabIndex = 4;
            this.numImg.ValueChanged += new System.EventHandler(this.numImg_ValueChanged);
            // 
            // TexSprites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numImg);
            this.Controls.Add(this.spriteControl1);
            this.Name = "TexSprites";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Ekona.Images.SpriteControl spriteControl1;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numImg;
    }
}
