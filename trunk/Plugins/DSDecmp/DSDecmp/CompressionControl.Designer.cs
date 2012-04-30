// ----------------------------------------------------------------------
// <copyright file="CompressionControl.Designer.cs" company="none">

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
// <date>28/04/2012 14:29:46</date>
// -----------------------------------------------------------------------
namespace DSDecmp
{
    partial class CompressionControl
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
            this.checkLookAhead = new System.Windows.Forms.CheckBox();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.btnSearchCompression = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxOlderCompress = new System.Windows.Forms.TextBox();
            this.txtBoxNewCompress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkLookAhead
            // 
            this.checkLookAhead.AutoSize = true;
            this.checkLookAhead.Location = new System.Drawing.Point(146, 97);
            this.checkLookAhead.Name = "checkLookAhead";
            this.checkLookAhead.Size = new System.Drawing.Size(45, 17);
            this.checkLookAhead.TabIndex = 0;
            this.checkLookAhead.Text = "S16";
            this.checkLookAhead.UseVisualStyleBackColor = true;
            this.checkLookAhead.CheckedChanged += new System.EventHandler(this.checkLookAhead_CheckedChanged);
            // 
            // comboFormat
            // 
            this.comboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFormat.FormattingEnabled = true;
            this.comboFormat.Items.AddRange(new object[] {
            "LZ10",
            "LZ11",
            "HUFF8",
            "HUFF4",
            "RLE"});
            this.comboFormat.Location = new System.Drawing.Point(9, 93);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(121, 21);
            this.comboFormat.TabIndex = 1;
            this.comboFormat.SelectedIndexChanged += new System.EventHandler(this.comboFormat_SelectedIndexChanged);
            // 
            // btnSearchCompression
            // 
            this.btnSearchCompression.Location = new System.Drawing.Point(124, 156);
            this.btnSearchCompression.Name = "btnSearchCompression";
            this.btnSearchCompression.Size = new System.Drawing.Size(100, 40);
            this.btnSearchCompression.TabIndex = 2;
            this.btnSearchCompression.Text = "S18";
            this.btnSearchCompression.UseVisualStyleBackColor = true;
            this.btnSearchCompression.Click += new System.EventHandler(this.btnSearchCompression_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(9, 156);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(90, 40);
            this.btnCompress.TabIndex = 3;
            this.btnCompress.Text = "S17";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "S15";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "S12";
            // 
            // txtBoxOlderCompress
            // 
            this.txtBoxOlderCompress.Location = new System.Drawing.Point(9, 27);
            this.txtBoxOlderCompress.Name = "txtBoxOlderCompress";
            this.txtBoxOlderCompress.ReadOnly = true;
            this.txtBoxOlderCompress.Size = new System.Drawing.Size(100, 20);
            this.txtBoxOlderCompress.TabIndex = 6;
            // 
            // txtBoxNewCompress
            // 
            this.txtBoxNewCompress.Location = new System.Drawing.Point(253, 27);
            this.txtBoxNewCompress.Name = "txtBoxNewCompress";
            this.txtBoxNewCompress.ReadOnly = true;
            this.txtBoxNewCompress.Size = new System.Drawing.Size(100, 20);
            this.txtBoxNewCompress.TabIndex = 7;
            this.txtBoxNewCompress.Text = "S14";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "S13";
            // 
            // CompressionControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBoxNewCompress);
            this.Controls.Add(this.txtBoxOlderCompress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.btnSearchCompression);
            this.Controls.Add(this.comboFormat);
            this.Controls.Add(this.checkLookAhead);
            this.Name = "CompressionControl";
            this.Size = new System.Drawing.Size(512, 512);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkLookAhead;
        private System.Windows.Forms.ComboBox comboFormat;
        private System.Windows.Forms.Button btnSearchCompression;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxOlderCompress;
        private System.Windows.Forms.TextBox txtBoxNewCompress;
        private System.Windows.Forms.Label label3;
    }
}
