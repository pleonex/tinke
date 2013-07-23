// ----------------------------------------------------------------------
// <copyright file="BlogpostControl.Designer.cs" company="none">

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
// <date>28/04/2012 12:46:03</date>
// -----------------------------------------------------------------------
namespace INAZUMA11
{
    partial class BlogpostControl
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numIndex = new System.Windows.Forms.NumericUpDown();
            this.txtText2 = new System.Windows.Forms.TextBox();
            this.txtText1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numUnk = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk)).BeginInit();
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
            this.label2.Location = new System.Drawing.Point(199, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "of ";
            // 
            // numBlock
            // 
            this.numBlock.Location = new System.Drawing.Point(103, 3);
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
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numIndex);
            this.groupBox1.Controls.Add(this.txtText2);
            this.groupBox1.Controls.Add(this.txtText1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numUnk);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(6, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 184);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Block data";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(220, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Index:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(253, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Text 2";
            // 
            // numIndex
            // 
            this.numIndex.Location = new System.Drawing.Point(304, 19);
            this.numIndex.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numIndex.Name = "numIndex";
            this.numIndex.Size = new System.Drawing.Size(90, 20);
            this.numIndex.TabIndex = 5;
            this.numIndex.ValueChanged += new System.EventHandler(this.numIndex_ValueChanged);
            // 
            // txtText2
            // 
            this.txtText2.Location = new System.Drawing.Point(252, 71);
            this.txtText2.Multiline = true;
            this.txtText2.Name = "txtText2";
            this.txtText2.Size = new System.Drawing.Size(240, 100);
            this.txtText2.TabIndex = 4;
            this.txtText2.TextChanged += new System.EventHandler(this.txtText2_TextChanged);
            // 
            // txtText1
            // 
            this.txtText1.Location = new System.Drawing.Point(6, 71);
            this.txtText1.Multiline = true;
            this.txtText1.Name = "txtText1";
            this.txtText1.Size = new System.Drawing.Size(240, 100);
            this.txtText1.TabIndex = 3;
            this.txtText1.TextChanged += new System.EventHandler(this.txtText1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Text 1";
            // 
            // numUnk
            // 
            this.numUnk.Location = new System.Drawing.Point(98, 19);
            this.numUnk.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numUnk.Name = "numUnk";
            this.numUnk.Size = new System.Drawing.Size(90, 20);
            this.numUnk.TabIndex = 1;
            this.numUnk.ValueChanged += new System.EventHandler(this.numUnk_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Unknown:";
            // 
            // btnWrite
            // 
            this.btnWrite.Image = global::INAZUMA11.Properties.Resources.page_white_edit;
            this.btnWrite.Location = new System.Drawing.Point(6, 219);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(80, 40);
            this.btnWrite.TabIndex = 4;
            this.btnWrite.Text = "Write file";
            this.btnWrite.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // BlogpostControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numBlock);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BlogpostControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numBlock;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numIndex;
        private System.Windows.Forms.TextBox txtText2;
        private System.Windows.Forms.TextBox txtText1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numUnk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWrite;
    }
}
