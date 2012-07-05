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
namespace Tinke.Dialog
{
    partial class FATExtract
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
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.groupOffset = new System.Windows.Forms.GroupBox();
            this.numericOffsetMult = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numSize = new System.Windows.Forms.NumericUpDown();
            this.numOffset = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddOffset = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numericOffsetLen = new System.Windows.Forms.NumericUpDown();
            this.checkOffsetBigEndian = new System.Windows.Forms.CheckBox();
            this.btnOffsetCalculate = new System.Windows.Forms.Button();
            this.groupOffsetRelative = new System.Windows.Forms.GroupBox();
            this.numericRelativeOffset = new System.Windows.Forms.NumericUpDown();
            this.radioRelativeFirstFile = new System.Windows.Forms.RadioButton();
            this.radioRelativeOffset = new System.Windows.Forms.RadioButton();
            this.groupOffsetType = new System.Windows.Forms.GroupBox();
            this.radioOffsetEnd = new System.Windows.Forms.RadioButton();
            this.radioOffsetStartEnd = new System.Windows.Forms.RadioButton();
            this.radioOffsetStartSize = new System.Windows.Forms.RadioButton();
            this.radioOffsetStart = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.numericOffsetStart = new System.Windows.Forms.NumericUpDown();
            this.groupNumFiles = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericNumFiles = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.checkNumBigEndian = new System.Windows.Forms.CheckBox();
            this.numericNumOffset = new System.Windows.Forms.NumericUpDown();
            this.btnNumCalculate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericNumLen = new System.Windows.Forms.NumericUpDown();
            this.btnAccept = new System.Windows.Forms.Button();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.btnHex = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkOmitZero = new System.Windows.Forms.CheckBox();
            this.groupOptions.SuspendLayout();
            this.groupOffset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetMult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetLen)).BeginInit();
            this.groupOffsetRelative.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRelativeOffset)).BeginInit();
            this.groupOffsetType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetStart)).BeginInit();
            this.groupNumFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumLen)).BeginInit();
            this.SuspendLayout();
            // 
            // groupOptions
            // 
            this.groupOptions.Controls.Add(this.groupOffset);
            this.groupOptions.Controls.Add(this.groupNumFiles);
            this.groupOptions.Location = new System.Drawing.Point(13, 13);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(529, 300);
            this.groupOptions.TabIndex = 0;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "S07";
            // 
            // groupOffset
            // 
            this.groupOffset.Controls.Add(this.checkOmitZero);
            this.groupOffset.Controls.Add(this.numericOffsetMult);
            this.groupOffset.Controls.Add(this.label8);
            this.groupOffset.Controls.Add(this.numSize);
            this.groupOffset.Controls.Add(this.numOffset);
            this.groupOffset.Controls.Add(this.label5);
            this.groupOffset.Controls.Add(this.label1);
            this.groupOffset.Controls.Add(this.btnAddOffset);
            this.groupOffset.Controls.Add(this.label6);
            this.groupOffset.Controls.Add(this.numericOffsetLen);
            this.groupOffset.Controls.Add(this.checkOffsetBigEndian);
            this.groupOffset.Controls.Add(this.btnOffsetCalculate);
            this.groupOffset.Controls.Add(this.groupOffsetRelative);
            this.groupOffset.Controls.Add(this.groupOffsetType);
            this.groupOffset.Controls.Add(this.label4);
            this.groupOffset.Controls.Add(this.numericOffsetStart);
            this.groupOffset.Location = new System.Drawing.Point(6, 105);
            this.groupOffset.Name = "groupOffset";
            this.groupOffset.Size = new System.Drawing.Size(517, 192);
            this.groupOffset.TabIndex = 1;
            this.groupOffset.TabStop = false;
            this.groupOffset.Text = "S0E";
            // 
            // numericOffsetMult
            // 
            this.numericOffsetMult.Hexadecimal = true;
            this.numericOffsetMult.Location = new System.Drawing.Point(205, 46);
            this.numericOffsetMult.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericOffsetMult.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericOffsetMult.Name = "numericOffsetMult";
            this.numericOffsetMult.Size = new System.Drawing.Size(76, 20);
            this.numericOffsetMult.TabIndex = 14;
            this.numericOffsetMult.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(193, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "x";
            // 
            // numSize
            // 
            this.numSize.Hexadecimal = true;
            this.numSize.Location = new System.Drawing.Point(347, 166);
            this.numSize.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numSize.Name = "numSize";
            this.numSize.Size = new System.Drawing.Size(76, 20);
            this.numSize.TabIndex = 12;
            // 
            // numOffset
            // 
            this.numOffset.Hexadecimal = true;
            this.numOffset.Location = new System.Drawing.Point(347, 143);
            this.numOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numOffset.Name = "numOffset";
            this.numOffset.Size = new System.Drawing.Size(76, 20);
            this.numOffset.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(289, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Offset:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(289, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Size:";
            // 
            // btnAddOffset
            // 
            this.btnAddOffset.Location = new System.Drawing.Point(429, 145);
            this.btnAddOffset.Name = "btnAddOffset";
            this.btnAddOffset.Size = new System.Drawing.Size(80, 35);
            this.btnAddOffset.TabIndex = 8;
            this.btnAddOffset.Text = "Add offset";
            this.btnAddOffset.UseVisualStyleBackColor = true;
            this.btnAddOffset.Click += new System.EventHandler(this.btnAddOffset_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "S10";
            // 
            // numericOffsetLen
            // 
            this.numericOffsetLen.Hexadecimal = true;
            this.numericOffsetLen.Location = new System.Drawing.Point(111, 46);
            this.numericOffsetLen.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericOffsetLen.Name = "numericOffsetLen";
            this.numericOffsetLen.Size = new System.Drawing.Size(76, 20);
            this.numericOffsetLen.TabIndex = 6;
            this.numericOffsetLen.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // checkOffsetBigEndian
            // 
            this.checkOffsetBigEndian.AutoSize = true;
            this.checkOffsetBigEndian.Location = new System.Drawing.Point(192, 23);
            this.checkOffsetBigEndian.Name = "checkOffsetBigEndian";
            this.checkOffsetBigEndian.Size = new System.Drawing.Size(45, 17);
            this.checkOffsetBigEndian.TabIndex = 5;
            this.checkOffsetBigEndian.Text = "S11";
            this.checkOffsetBigEndian.UseVisualStyleBackColor = true;
            // 
            // btnOffsetCalculate
            // 
            this.btnOffsetCalculate.Location = new System.Drawing.Point(9, 145);
            this.btnOffsetCalculate.Name = "btnOffsetCalculate";
            this.btnOffsetCalculate.Size = new System.Drawing.Size(126, 35);
            this.btnOffsetCalculate.TabIndex = 4;
            this.btnOffsetCalculate.Text = "S1A";
            this.btnOffsetCalculate.UseVisualStyleBackColor = true;
            this.btnOffsetCalculate.Click += new System.EventHandler(this.btnOffsetCalculate_Click);
            // 
            // groupOffsetRelative
            // 
            this.groupOffsetRelative.Controls.Add(this.numericRelativeOffset);
            this.groupOffsetRelative.Controls.Add(this.radioRelativeFirstFile);
            this.groupOffsetRelative.Controls.Add(this.radioRelativeOffset);
            this.groupOffsetRelative.Location = new System.Drawing.Point(9, 65);
            this.groupOffsetRelative.Name = "groupOffsetRelative";
            this.groupOffsetRelative.Size = new System.Drawing.Size(272, 74);
            this.groupOffsetRelative.TabIndex = 1;
            this.groupOffsetRelative.TabStop = false;
            this.groupOffsetRelative.Text = "S17";
            // 
            // numericRelativeOffset
            // 
            this.numericRelativeOffset.Hexadecimal = true;
            this.numericRelativeOffset.Location = new System.Drawing.Point(102, 27);
            this.numericRelativeOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericRelativeOffset.Name = "numericRelativeOffset";
            this.numericRelativeOffset.Size = new System.Drawing.Size(75, 20);
            this.numericRelativeOffset.TabIndex = 1;
            // 
            // radioRelativeFirstFile
            // 
            this.radioRelativeFirstFile.AutoSize = true;
            this.radioRelativeFirstFile.Location = new System.Drawing.Point(6, 51);
            this.radioRelativeFirstFile.Name = "radioRelativeFirstFile";
            this.radioRelativeFirstFile.Size = new System.Drawing.Size(44, 17);
            this.radioRelativeFirstFile.TabIndex = 2;
            this.radioRelativeFirstFile.Text = "S19";
            this.radioRelativeFirstFile.UseVisualStyleBackColor = true;
            this.radioRelativeFirstFile.CheckedChanged += new System.EventHandler(this.radioRelativeFirstFile_CheckedChanged);
            // 
            // radioRelativeOffset
            // 
            this.radioRelativeOffset.AutoSize = true;
            this.radioRelativeOffset.Checked = true;
            this.radioRelativeOffset.Location = new System.Drawing.Point(6, 27);
            this.radioRelativeOffset.Name = "radioRelativeOffset";
            this.radioRelativeOffset.Size = new System.Drawing.Size(44, 17);
            this.radioRelativeOffset.TabIndex = 0;
            this.radioRelativeOffset.TabStop = true;
            this.radioRelativeOffset.Text = "S18";
            this.radioRelativeOffset.UseVisualStyleBackColor = true;
            // 
            // groupOffsetType
            // 
            this.groupOffsetType.Controls.Add(this.radioOffsetEnd);
            this.groupOffsetType.Controls.Add(this.radioOffsetStartEnd);
            this.groupOffsetType.Controls.Add(this.radioOffsetStartSize);
            this.groupOffsetType.Controls.Add(this.radioOffsetStart);
            this.groupOffsetType.Location = new System.Drawing.Point(292, 19);
            this.groupOffsetType.Name = "groupOffsetType";
            this.groupOffsetType.Size = new System.Drawing.Size(217, 120);
            this.groupOffsetType.TabIndex = 2;
            this.groupOffsetType.TabStop = false;
            this.groupOffsetType.Text = "S12";
            // 
            // radioOffsetEnd
            // 
            this.radioOffsetEnd.AutoSize = true;
            this.radioOffsetEnd.Location = new System.Drawing.Point(23, 94);
            this.radioOffsetEnd.Name = "radioOffsetEnd";
            this.radioOffsetEnd.Size = new System.Drawing.Size(44, 17);
            this.radioOffsetEnd.TabIndex = 3;
            this.radioOffsetEnd.TabStop = true;
            this.radioOffsetEnd.Text = "S16";
            this.radioOffsetEnd.UseVisualStyleBackColor = true;
            // 
            // radioOffsetStartEnd
            // 
            this.radioOffsetStartEnd.AutoSize = true;
            this.radioOffsetStartEnd.Location = new System.Drawing.Point(23, 48);
            this.radioOffsetStartEnd.Name = "radioOffsetStartEnd";
            this.radioOffsetStartEnd.Size = new System.Drawing.Size(44, 17);
            this.radioOffsetStartEnd.TabIndex = 1;
            this.radioOffsetStartEnd.Text = "S14";
            this.radioOffsetStartEnd.UseVisualStyleBackColor = true;
            // 
            // radioOffsetStartSize
            // 
            this.radioOffsetStartSize.AutoSize = true;
            this.radioOffsetStartSize.Location = new System.Drawing.Point(23, 71);
            this.radioOffsetStartSize.Name = "radioOffsetStartSize";
            this.radioOffsetStartSize.Size = new System.Drawing.Size(44, 17);
            this.radioOffsetStartSize.TabIndex = 2;
            this.radioOffsetStartSize.Text = "S15";
            this.radioOffsetStartSize.UseVisualStyleBackColor = true;
            // 
            // radioOffsetStart
            // 
            this.radioOffsetStart.AutoSize = true;
            this.radioOffsetStart.Checked = true;
            this.radioOffsetStart.Location = new System.Drawing.Point(23, 24);
            this.radioOffsetStart.Name = "radioOffsetStart";
            this.radioOffsetStart.Size = new System.Drawing.Size(44, 17);
            this.radioOffsetStart.TabIndex = 0;
            this.radioOffsetStart.TabStop = true;
            this.radioOffsetStart.Text = "S13";
            this.radioOffsetStart.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "S0F";
            // 
            // numericOffsetStart
            // 
            this.numericOffsetStart.Hexadecimal = true;
            this.numericOffsetStart.Location = new System.Drawing.Point(111, 20);
            this.numericOffsetStart.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericOffsetStart.Name = "numericOffsetStart";
            this.numericOffsetStart.Size = new System.Drawing.Size(75, 20);
            this.numericOffsetStart.TabIndex = 0;
            this.numericOffsetStart.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // groupNumFiles
            // 
            this.groupNumFiles.Controls.Add(this.label7);
            this.groupNumFiles.Controls.Add(this.numericNumFiles);
            this.groupNumFiles.Controls.Add(this.label3);
            this.groupNumFiles.Controls.Add(this.checkNumBigEndian);
            this.groupNumFiles.Controls.Add(this.numericNumOffset);
            this.groupNumFiles.Controls.Add(this.btnNumCalculate);
            this.groupNumFiles.Controls.Add(this.label2);
            this.groupNumFiles.Controls.Add(this.numericNumLen);
            this.groupNumFiles.Location = new System.Drawing.Point(6, 19);
            this.groupNumFiles.Name = "groupNumFiles";
            this.groupNumFiles.Size = new System.Drawing.Size(517, 80);
            this.groupNumFiles.TabIndex = 0;
            this.groupNumFiles.TabStop = false;
            this.groupNumFiles.Text = "S08";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(227, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "S0C";
            // 
            // numericNumFiles
            // 
            this.numericNumFiles.Location = new System.Drawing.Point(369, 44);
            this.numericNumFiles.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericNumFiles.Name = "numericNumFiles";
            this.numericNumFiles.Size = new System.Drawing.Size(75, 20);
            this.numericNumFiles.TabIndex = 6;
            this.numericNumFiles.ValueChanged += new System.EventHandler(this.numericNumFiles_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "S0A";
            // 
            // checkNumBigEndian
            // 
            this.checkNumBigEndian.AutoSize = true;
            this.checkNumBigEndian.Location = new System.Drawing.Point(230, 19);
            this.checkNumBigEndian.Name = "checkNumBigEndian";
            this.checkNumBigEndian.Size = new System.Drawing.Size(46, 17);
            this.checkNumBigEndian.TabIndex = 2;
            this.checkNumBigEndian.Text = "S0B";
            this.checkNumBigEndian.UseVisualStyleBackColor = true;
            // 
            // numericNumOffset
            // 
            this.numericNumOffset.Hexadecimal = true;
            this.numericNumOffset.Location = new System.Drawing.Point(135, 18);
            this.numericNumOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericNumOffset.Name = "numericNumOffset";
            this.numericNumOffset.Size = new System.Drawing.Size(75, 20);
            this.numericNumOffset.TabIndex = 0;
            this.numericNumOffset.ValueChanged += new System.EventHandler(this.numericNumLen_ValueChanged);
            // 
            // btnNumCalculate
            // 
            this.btnNumCalculate.Location = new System.Drawing.Point(369, 15);
            this.btnNumCalculate.Name = "btnNumCalculate";
            this.btnNumCalculate.Size = new System.Drawing.Size(100, 23);
            this.btnNumCalculate.TabIndex = 3;
            this.btnNumCalculate.Text = "S0D";
            this.btnNumCalculate.UseVisualStyleBackColor = true;
            this.btnNumCalculate.Click += new System.EventHandler(this.btnNumCalculate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "S09";
            // 
            // numericNumLen
            // 
            this.numericNumLen.Hexadecimal = true;
            this.numericNumLen.Location = new System.Drawing.Point(135, 44);
            this.numericNumLen.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericNumLen.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericNumLen.Name = "numericNumLen";
            this.numericNumLen.Size = new System.Drawing.Size(75, 20);
            this.numericNumLen.TabIndex = 1;
            this.numericNumLen.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericNumLen.ValueChanged += new System.EventHandler(this.numericNumLen_ValueChanged);
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Image = global::Tinke.Properties.Resources.accept;
            this.btnAccept.Location = new System.Drawing.Point(13, 319);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(90, 40);
            this.btnAccept.TabIndex = 4;
            this.btnAccept.Text = "S1C";
            this.btnAccept.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.BackColor = System.Drawing.SystemColors.Control;
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.Location = new System.Drawing.Point(548, 20);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(192, 290);
            this.listBoxFiles.TabIndex = 2;
            // 
            // btnHex
            // 
            this.btnHex.Image = global::Tinke.Properties.Resources.calculator;
            this.btnHex.Location = new System.Drawing.Point(548, 319);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(192, 31);
            this.btnHex.TabIndex = 3;
            this.btnHex.Text = "S1B";
            this.btnHex.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::Tinke.Properties.Resources.cancel;
            this.btnCancel.Location = new System.Drawing.Point(110, 319);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // checkOmitZero
            // 
            this.checkOmitZero.AutoSize = true;
            this.checkOmitZero.Location = new System.Drawing.Point(153, 145);
            this.checkOmitZero.Name = "checkOmitZero";
            this.checkOmitZero.Size = new System.Drawing.Size(112, 17);
            this.checkOmitZero.TabIndex = 15;
            this.checkOmitZero.Text = "Omit zero size files";
            this.checkOmitZero.UseVisualStyleBackColor = true;
            // 
            // FATExtract
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(752, 365);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnHex);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.groupOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FATExtract";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "S06";
            this.groupOptions.ResumeLayout(false);
            this.groupOffset.ResumeLayout(false);
            this.groupOffset.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetMult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetLen)).EndInit();
            this.groupOffsetRelative.ResumeLayout(false);
            this.groupOffsetRelative.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericRelativeOffset)).EndInit();
            this.groupOffsetType.ResumeLayout(false);
            this.groupOffsetType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericOffsetStart)).EndInit();
            this.groupNumFiles.ResumeLayout(false);
            this.groupNumFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericNumLen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupOptions;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.NumericUpDown numericNumLen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNumCalculate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericNumOffset;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.CheckBox checkNumBigEndian;
        private System.Windows.Forms.GroupBox groupOffset;
        private System.Windows.Forms.GroupBox groupOffsetRelative;
        private System.Windows.Forms.NumericUpDown numericRelativeOffset;
        private System.Windows.Forms.RadioButton radioRelativeFirstFile;
        private System.Windows.Forms.RadioButton radioRelativeOffset;
        private System.Windows.Forms.GroupBox groupOffsetType;
        private System.Windows.Forms.RadioButton radioOffsetStartEnd;
        private System.Windows.Forms.RadioButton radioOffsetStartSize;
        private System.Windows.Forms.RadioButton radioOffsetStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericOffsetStart;
        private System.Windows.Forms.GroupBox groupNumFiles;
        private System.Windows.Forms.Button btnOffsetCalculate;
        private System.Windows.Forms.CheckBox checkOffsetBigEndian;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericOffsetLen;
        private System.Windows.Forms.RadioButton radioOffsetEnd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericNumFiles;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numSize;
        private System.Windows.Forms.NumericUpDown numOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddOffset;
        private System.Windows.Forms.NumericUpDown numericOffsetMult;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkOmitZero;
    }
}