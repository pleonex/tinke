// ----------------------------------------------------------------------
// <copyright file="TextControl.Designer.cs" company="none">

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
// <date>21/04/2012 11:20:54</date>
// -----------------------------------------------------------------------
namespace TC_UTK
{
    partial class TextControl
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
            this.txtChar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataTable = new System.Windows.Forms.DataGridView();
            this.ColumnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnChar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSaveText = new System.Windows.Forms.Button();
            this.btnSaveTable = new System.Windows.Forms.Button();
            this.btnExportText = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // txtChar
            // 
            this.txtChar.Location = new System.Drawing.Point(3, 24);
            this.txtChar.Multiline = true;
            this.txtChar.Name = "txtChar";
            this.txtChar.ReadOnly = true;
            this.txtChar.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtChar.Size = new System.Drawing.Size(322, 439);
            this.txtChar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Text:";
            // 
            // dataTable
            // 
            this.dataTable.AllowUserToOrderColumns = true;
            this.dataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCode,
            this.ColumnChar});
            this.dataTable.Location = new System.Drawing.Point(331, 24);
            this.dataTable.Name = "dataTable";
            this.dataTable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataTable.Size = new System.Drawing.Size(178, 439);
            this.dataTable.TabIndex = 3;
            this.dataTable.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTable_CellEndEdit);
            this.dataTable.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataTable_RowsRemoved);
            // 
            // ColumnCode
            // 
            this.ColumnCode.HeaderText = "Code";
            this.ColumnCode.Name = "ColumnCode";
            this.ColumnCode.Width = 60;
            // 
            // ColumnChar
            // 
            this.ColumnChar.HeaderText = "Char";
            this.ColumnChar.Name = "ColumnChar";
            this.ColumnChar.Width = 53;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(328, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Table:";
            // 
            // btnSaveText
            // 
            this.btnSaveText.Enabled = false;
            this.btnSaveText.Location = new System.Drawing.Point(429, 469);
            this.btnSaveText.Name = "btnSaveText";
            this.btnSaveText.Size = new System.Drawing.Size(80, 40);
            this.btnSaveText.TabIndex = 5;
            this.btnSaveText.Text = "Save text";
            this.btnSaveText.UseVisualStyleBackColor = true;
            // 
            // btnSaveTable
            // 
            this.btnSaveTable.Location = new System.Drawing.Point(343, 469);
            this.btnSaveTable.Name = "btnSaveTable";
            this.btnSaveTable.Size = new System.Drawing.Size(80, 40);
            this.btnSaveTable.TabIndex = 6;
            this.btnSaveTable.Text = "Save table";
            this.btnSaveTable.UseVisualStyleBackColor = true;
            this.btnSaveTable.Click += new System.EventHandler(this.btnSaveTable_Click);
            // 
            // btnExportText
            // 
            this.btnExportText.Location = new System.Drawing.Point(257, 469);
            this.btnExportText.Name = "btnExportText";
            this.btnExportText.Size = new System.Drawing.Size(80, 40);
            this.btnExportText.TabIndex = 7;
            this.btnExportText.Text = "Export text";
            this.btnExportText.UseVisualStyleBackColor = true;
            this.btnExportText.Click += new System.EventHandler(this.btnExportText_Click);
            // 
            // TextControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnExportText);
            this.Controls.Add(this.btnSaveTable);
            this.Controls.Add(this.btnSaveText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataTable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChar);
            this.Name = "TextControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.dataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnChar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSaveText;
        private System.Windows.Forms.Button btnSaveTable;
        private System.Windows.Forms.Button btnExportText;
    }
}
