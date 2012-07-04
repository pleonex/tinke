/*
 * Copyright (C) 2012  pleonex
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
 * Date: 16/03/2012
 *
 */
namespace PSL
{
    partial class ATDTX
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
            this.imageControl1 = new Ekona.Images.ImageControl();
            this.numFile = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.numSprite = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSprite)).BeginInit();
            this.SuspendLayout();
            // 
            // imageControl1
            // 
            this.imageControl1.BackColor = System.Drawing.Color.Transparent;
            this.imageControl1.Location = new System.Drawing.Point(0, 0);
            this.imageControl1.Name = "imageControl1";
            this.imageControl1.Size = new System.Drawing.Size(512, 512);
            this.imageControl1.TabIndex = 0;
            // 
            // numFile
            // 
            this.numFile.Location = new System.Drawing.Point(409, 4);
            this.numFile.Name = "numFile";
            this.numFile.Size = new System.Drawing.Size(60, 20);
            this.numFile.TabIndex = 1;
            this.numFile.ValueChanged += new System.EventHandler(this.numFile_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(366, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "File:";
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(369, 56);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(140, 23);
            this.btnExtract.TabIndex = 3;
            this.btnExtract.Text = "Extract all";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // numSprite
            // 
            this.numSprite.Location = new System.Drawing.Point(409, 30);
            this.numSprite.Name = "numSprite";
            this.numSprite.Size = new System.Drawing.Size(60, 20);
            this.numSprite.TabIndex = 4;
            this.numSprite.ValueChanged += new System.EventHandler(this.numSprite_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Sprite:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(475, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "of ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(475, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "of ";
            // 
            // ATDTX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSprite);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numFile);
            this.Controls.Add(this.imageControl1);
            this.Name = "ATDTX";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSprite)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Ekona.Images.ImageControl imageControl1;
        private System.Windows.Forms.NumericUpDown numFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.NumericUpDown numSprite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
