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
    partial class GBCS
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
            this.imgControl = new Ekona.Images.ImageControl();
            this.numImg = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numImg)).BeginInit();
            this.SuspendLayout();
            // 
            // imgControl
            // 
            this.imgControl.BackColor = System.Drawing.Color.Transparent;
            this.imgControl.Location = new System.Drawing.Point(0, 0);
            this.imgControl.Name = "imgControl";
            this.imgControl.Size = new System.Drawing.Size(512, 512);
            this.imgControl.TabIndex = 0;
            // 
            // numImg
            // 
            this.numImg.Location = new System.Drawing.Point(408, 244);
            this.numImg.Name = "numImg";
            this.numImg.Size = new System.Drawing.Size(58, 20);
            this.numImg.TabIndex = 1;
            this.numImg.ValueChanged += new System.EventHandler(this.numImg_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(328, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Image to load:";
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(226, 241);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(96, 23);
            this.btnExtract.TabIndex = 3;
            this.btnExtract.Text = "Extract all";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(472, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "of ";
            // 
            // GBCS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numImg);
            this.Controls.Add(this.imgControl);
            this.Name = "GBCS";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Ekona.Images.ImageControl imgControl;
        private System.Windows.Forms.NumericUpDown numImg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label label2;
    }
}
