// ----------------------------------------------------------------------
// <copyright file="BlogresControl.Designer.cs" company="none">

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
// <date>28/04/2012 13:49:35</date>
// -----------------------------------------------------------------------
namespace INAZUMA11
{
    partial class BlogresControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numBlock = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numUnk2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numUnk1 = new System.Windows.Forms.NumericUpDown();
            this.btnWrite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Num. of block:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "of ";
            // 
            // numBlock
            // 
            this.numBlock.Location = new System.Drawing.Point(86, 3);
            this.numBlock.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numBlock.Name = "numBlock";
            this.numBlock.Size = new System.Drawing.Size(90, 20);
            this.numBlock.TabIndex = 2;
            this.numBlock.ValueChanged += new System.EventHandler(this.numBlock_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtText);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numUnk2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numUnk1);
            this.groupBox1.Location = new System.Drawing.Point(6, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 161);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Block data";
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(9, 71);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(488, 80);
            this.txtText.TabIndex = 5;
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Text:";
            // 
            // numUnk2
            // 
            this.numUnk2.Hexadecimal = true;
            this.numUnk2.Location = new System.Drawing.Point(323, 21);
            this.numUnk2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numUnk2.Name = "numUnk2";
            this.numUnk2.Size = new System.Drawing.Size(90, 20);
            this.numUnk2.TabIndex = 3;
            this.numUnk2.ValueChanged += new System.EventHandler(this.numUnk2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(226, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Unknown:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Unknown:";
            // 
            // numUnk1
            // 
            this.numUnk1.Hexadecimal = true;
            this.numUnk1.Location = new System.Drawing.Point(105, 19);
            this.numUnk1.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numUnk1.Name = "numUnk1";
            this.numUnk1.Size = new System.Drawing.Size(90, 20);
            this.numUnk1.TabIndex = 0;
            this.numUnk1.ValueChanged += new System.EventHandler(this.numUnk1_ValueChanged);
            // 
            // btnWrite
            // 
            this.btnWrite.Image = global::INAZUMA11.Properties.Resources.page_white_edit;
            this.btnWrite.Location = new System.Drawing.Point(6, 207);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(80, 40);
            this.btnWrite.TabIndex = 4;
            this.btnWrite.Text = "Write file";
            this.btnWrite.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // BlogresControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numBlock);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BlogresControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numBlock;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numUnk2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUnk1;
        private System.Windows.Forms.Button btnWrite;
    }
}
