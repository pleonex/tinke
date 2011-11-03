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
namespace Fonts
{
    partial class MapChar
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapChar));
            this.btnSave = new System.Windows.Forms.Button();
            this.numericSection = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridMapInfo = new System.Windows.Forms.DataGridView();
            this.ColumnImage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCharCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddSect = new System.Windows.Forms.Button();
            this.btnRemoveSec = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.numericLastChar = new System.Windows.Forms.NumericUpDown();
            this.numericFirstChar = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericType = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTotalSec = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericSection)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMapInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLastChar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFirstChar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericType)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(305, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "S03";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // numericSection
            // 
            this.numericSection.Location = new System.Drawing.Point(64, 7);
            this.numericSection.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericSection.Name = "numericSection";
            this.numericSection.Size = new System.Drawing.Size(50, 20);
            this.numericSection.TabIndex = 1;
            this.numericSection.ValueChanged += new System.EventHandler(this.numericSection_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridMapInfo);
            this.groupBox1.Controls.Add(this.btnAddSect);
            this.groupBox1.Controls.Add(this.btnRemoveSec);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericLastChar);
            this.groupBox1.Controls.Add(this.numericFirstChar);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 207);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "S04";
            // 
            // dataGridMapInfo
            // 
            this.dataGridMapInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridMapInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMapInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnImage,
            this.ColumnCharCode});
            this.dataGridMapInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridMapInfo.Location = new System.Drawing.Point(3, 79);
            this.dataGridMapInfo.Name = "dataGridMapInfo";
            this.dataGridMapInfo.RowTemplate.DefaultCellStyle.Format = "N0";
            this.dataGridMapInfo.RowTemplate.DefaultCellStyle.NullValue = "0";
            this.dataGridMapInfo.Size = new System.Drawing.Size(374, 125);
            this.dataGridMapInfo.TabIndex = 11;
            this.dataGridMapInfo.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridMapInfo_CellEndEdit);
            // 
            // ColumnImage
            // 
            this.ColumnImage.HeaderText = "S0A";
            this.ColumnImage.Name = "ColumnImage";
            // 
            // ColumnCharCode
            // 
            this.ColumnCharCode.HeaderText = "S0B";
            this.ColumnCharCode.Name = "ColumnCharCode";
            // 
            // btnAddSect
            // 
            this.btnAddSect.Location = new System.Drawing.Point(165, 50);
            this.btnAddSect.Name = "btnAddSect";
            this.btnAddSect.Size = new System.Drawing.Size(100, 23);
            this.btnAddSect.TabIndex = 10;
            this.btnAddSect.Text = "S08";
            this.btnAddSect.UseVisualStyleBackColor = true;
            this.btnAddSect.Click += new System.EventHandler(this.btnAddSect_Click);
            // 
            // btnRemoveSec
            // 
            this.btnRemoveSec.Location = new System.Drawing.Point(268, 50);
            this.btnRemoveSec.Name = "btnRemoveSec";
            this.btnRemoveSec.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveSec.TabIndex = 9;
            this.btnRemoveSec.Text = "S09";
            this.btnRemoveSec.UseVisualStyleBackColor = true;
            this.btnRemoveSec.Click += new System.EventHandler(this.btnRemoveSec_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "S07";
            // 
            // numericLastChar
            // 
            this.numericLastChar.Location = new System.Drawing.Point(318, 24);
            this.numericLastChar.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericLastChar.Name = "numericLastChar";
            this.numericLastChar.Size = new System.Drawing.Size(50, 20);
            this.numericLastChar.TabIndex = 4;
            this.numericLastChar.ValueChanged += new System.EventHandler(this.numericChar_ValueChanged);
            // 
            // numericFirstChar
            // 
            this.numericFirstChar.Location = new System.Drawing.Point(183, 24);
            this.numericFirstChar.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericFirstChar.Name = "numericFirstChar";
            this.numericFirstChar.Size = new System.Drawing.Size(50, 20);
            this.numericFirstChar.TabIndex = 3;
            this.numericFirstChar.ValueChanged += new System.EventHandler(this.numericChar_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "S06";
            // 
            // numericType
            // 
            this.numericType.Location = new System.Drawing.Point(46, 24);
            this.numericType.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericType.Name = "numericType";
            this.numericType.ReadOnly = true;
            this.numericType.Size = new System.Drawing.Size(50, 20);
            this.numericType.TabIndex = 1;
            this.numericType.ValueChanged += new System.EventHandler(this.numericType_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "S05";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "S01";
            // 
            // lblTotalSec
            // 
            this.lblTotalSec.AutoSize = true;
            this.lblTotalSec.Location = new System.Drawing.Point(120, 9);
            this.lblTotalSec.Name = "lblTotalSec";
            this.lblTotalSec.Size = new System.Drawing.Size(26, 13);
            this.lblTotalSec.TabIndex = 4;
            this.lblTotalSec.Text = "S02";
            // 
            // MapChar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(380, 243);
            this.Controls.Add(this.lblTotalSec);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericSection);
            this.Controls.Add(this.btnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MapChar";
            this.ShowInTaskbar = false;
            this.Text = "S00";
            this.Resize += new System.EventHandler(this.MapChar_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numericSection)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMapInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLastChar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFirstChar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.NumericUpDown numericSection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalSec;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericLastChar;
        private System.Windows.Forms.NumericUpDown numericFirstChar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAddSect;
        private System.Windows.Forms.Button btnRemoveSec;
        private System.Windows.Forms.DataGridView dataGridMapInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCharCode;
    }
}