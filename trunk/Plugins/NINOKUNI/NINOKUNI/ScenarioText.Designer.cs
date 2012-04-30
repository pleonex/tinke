// ----------------------------------------------------------------------
// <copyright file="ScenarioText.Designer.cs" company="none">

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
// <date>29/04/2012 13:39:40</date>
// -----------------------------------------------------------------------
namespace NINOKUNI
{
    partial class ScenarioText
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
            this.txtOri = new System.Windows.Forms.TextBox();
            this.numericBlock = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericElement = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.numID = new System.Windows.Forms.NumericUpDown();
            this.numUnk1 = new System.Windows.Forms.NumericUpDown();
            this.numUnk2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNew = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericElement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk2)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOri
            // 
            this.txtOri.Location = new System.Drawing.Point(0, 20);
            this.txtOri.MaxLength = 256;
            this.txtOri.Multiline = true;
            this.txtOri.Name = "txtOri";
            this.txtOri.ReadOnly = true;
            this.txtOri.Size = new System.Drawing.Size(496, 100);
            this.txtOri.TabIndex = 0;
            // 
            // numericBlock
            // 
            this.numericBlock.Location = new System.Drawing.Point(67, 259);
            this.numericBlock.Name = "numericBlock";
            this.numericBlock.Size = new System.Drawing.Size(70, 20);
            this.numericBlock.TabIndex = 1;
            this.numericBlock.ValueChanged += new System.EventHandler(this.numericBlock_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Block:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 287);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Element";
            // 
            // numericElement
            // 
            this.numericElement.Location = new System.Drawing.Point(67, 285);
            this.numericElement.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericElement.Name = "numericElement";
            this.numericElement.Size = new System.Drawing.Size(70, 20);
            this.numericElement.TabIndex = 4;
            this.numericElement.ValueChanged += new System.EventHandler(this.numericElement_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 261);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "ID:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(253, 313);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Unknown 2:";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(416, 469);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 40);
            this.btnExport.TabIndex = 11;
            this.btnExport.Text = "Export";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(416, 423);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(330, 469);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(80, 40);
            this.btnImport.TabIndex = 13;
            this.btnImport.Text = "Import";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // numID
            // 
            this.numID.Hexadecimal = true;
            this.numID.Location = new System.Drawing.Point(376, 259);
            this.numID.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numID.Name = "numID";
            this.numID.Size = new System.Drawing.Size(120, 20);
            this.numID.TabIndex = 14;
            this.numID.ValueChanged += new System.EventHandler(this.numID_ValueChanged);
            // 
            // numUnk1
            // 
            this.numUnk1.Hexadecimal = true;
            this.numUnk1.Location = new System.Drawing.Point(376, 285);
            this.numUnk1.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numUnk1.Name = "numUnk1";
            this.numUnk1.Size = new System.Drawing.Size(120, 20);
            this.numUnk1.TabIndex = 15;
            this.numUnk1.ValueChanged += new System.EventHandler(this.numUnk1_ValueChanged);
            // 
            // numUnk2
            // 
            this.numUnk2.Hexadecimal = true;
            this.numUnk2.Location = new System.Drawing.Point(376, 311);
            this.numUnk2.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numUnk2.Name = "numUnk2";
            this.numUnk2.Size = new System.Drawing.Size(120, 20);
            this.numUnk2.TabIndex = 16;
            this.numUnk2.ValueChanged += new System.EventHandler(this.numUnk2_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Unknown 1:";
            // 
            // txtNew
            // 
            this.txtNew.Location = new System.Drawing.Point(0, 145);
            this.txtNew.MaxLength = 256;
            this.txtNew.Multiline = true;
            this.txtNew.Name = "txtNew";
            this.txtNew.Size = new System.Drawing.Size(496, 100);
            this.txtNew.TabIndex = 18;
            this.txtNew.TextChanged += new System.EventHandler(this.txtNew_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Translated text:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Original text:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(143, 261);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "of 2";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(143, 287);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "of ";
            // 
            // ScenarioText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtNew);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numUnk2);
            this.Controls.Add(this.numUnk1);
            this.Controls.Add(this.numID);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericElement);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericBlock);
            this.Controls.Add(this.txtOri);
            this.Name = "ScenarioText";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numericBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericElement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUnk2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOri;
        private System.Windows.Forms.NumericUpDown numericBlock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericElement;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.NumericUpDown numID;
        private System.Windows.Forms.NumericUpDown numUnk1;
        private System.Windows.Forms.NumericUpDown numUnk2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNew;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}
