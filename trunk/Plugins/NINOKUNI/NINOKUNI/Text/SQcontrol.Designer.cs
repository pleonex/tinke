// ----------------------------------------------------------------------
// <copyright file="SQcontrol.Designer.cs" company="none">

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
// <date>29/04/2012 13:40:26</date>
// -----------------------------------------------------------------------
namespace NINOKUNI
{
    partial class SQcontrol
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
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTranslated = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.radioStart = new System.Windows.Forms.RadioButton();
            this.radioEnd = new System.Windows.Forms.RadioButton();
            this.numString = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numString)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOriginal
            // 
            this.txtOriginal.Location = new System.Drawing.Point(7, 20);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.ReadOnly = true;
            this.txtOriginal.Size = new System.Drawing.Size(500, 100);
            this.txtOriginal.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "S00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "S01";
            // 
            // txtTranslated
            // 
            this.txtTranslated.Location = new System.Drawing.Point(7, 152);
            this.txtTranslated.Multiline = true;
            this.txtTranslated.Name = "txtTranslated";
            this.txtTranslated.Size = new System.Drawing.Size(500, 100);
            this.txtTranslated.TabIndex = 3;
            this.txtTranslated.TextChanged += new System.EventHandler(this.txtTranslated_TextChanged);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::NINOKUNI.Properties.Resources.page_white_edit;
            this.btnSave.Location = new System.Drawing.Point(429, 423);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "S03";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(343, 469);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 40);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(429, 469);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(80, 40);
            this.btnImport.TabIndex = 10;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // radioStart
            // 
            this.radioStart.AutoSize = true;
            this.radioStart.Checked = true;
            this.radioStart.Location = new System.Drawing.Point(6, 23);
            this.radioStart.Name = "radioStart";
            this.radioStart.Size = new System.Drawing.Size(76, 17);
            this.radioStart.TabIndex = 11;
            this.radioStart.TabStop = true;
            this.radioStart.Text = "Start block";
            this.radioStart.UseVisualStyleBackColor = true;
            this.radioStart.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioEnd
            // 
            this.radioEnd.AutoSize = true;
            this.radioEnd.Location = new System.Drawing.Point(6, 47);
            this.radioEnd.Name = "radioEnd";
            this.radioEnd.Size = new System.Drawing.Size(73, 17);
            this.radioEnd.TabIndex = 12;
            this.radioEnd.TabStop = true;
            this.radioEnd.Text = "End block";
            this.radioEnd.UseVisualStyleBackColor = true;
            this.radioEnd.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // numString
            // 
            this.numString.Location = new System.Drawing.Point(66, 80);
            this.numString.Name = "numString";
            this.numString.Size = new System.Drawing.Size(60, 20);
            this.numString.TabIndex = 13;
            this.numString.ValueChanged += new System.EventHandler(this.numString_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(132, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "of ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "String: ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioStart);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numString);
            this.groupBox1.Controls.Add(this.radioEnd);
            this.groupBox1.Location = new System.Drawing.Point(7, 275);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(185, 111);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select string";
            // 
            // SQcontrol
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtTranslated);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOriginal);
            this.Name = "SQcontrol";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numString)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOriginal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTranslated;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.RadioButton radioStart;
        private System.Windows.Forms.RadioButton radioEnd;
        private System.Windows.Forms.NumericUpDown numString;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
