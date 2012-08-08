// ----------------------------------------------------------------------
// <copyright file="SpriteControl.designer.cs" company="none">

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
// <date>28/04/2012 14:29:12</date>
// -----------------------------------------------------------------------
namespace Ekona.Images
{
    partial class SpriteControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBank = new System.Windows.Forms.ComboBox();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.checkGrid = new System.Windows.Forms.CheckBox();
            this.checkNumber = new System.Windows.Forms.CheckBox();
            this.checkCellBorder = new System.Windows.Forms.CheckBox();
            this.checkTransparency = new System.Windows.Forms.CheckBox();
            this.checkImage = new System.Windows.Forms.CheckBox();
            this.btnBgdTrans = new System.Windows.Forms.Button();
            this.btnBgd = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSetTrans = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOAMeditor = new System.Windows.Forms.Button();
            this.checkBatch = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBatch = new System.Windows.Forms.TextBox();
            this.radioOriginalPal = new System.Windows.Forms.RadioButton();
            this.radioReplacePal = new System.Windows.Forms.RadioButton();
            this.radioSwapPal = new System.Windows.Forms.RadioButton();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.radioImgAdd = new System.Windows.Forms.RadioButton();
            this.radioImgReplace = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgBox
            // 
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(512, 256);
            this.imgBox.TabIndex = 0;
            this.imgBox.TabStop = false;
            this.imgBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.imgBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S01";
            // 
            // comboBank
            // 
            this.comboBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBank.FormattingEnabled = true;
            this.comboBank.Location = new System.Drawing.Point(72, 263);
            this.comboBank.Name = "comboBank";
            this.comboBank.Size = new System.Drawing.Size(183, 21);
            this.comboBank.TabIndex = 3;
            this.comboBank.SelectedIndexChanged += new System.EventHandler(this.comboBank_SelectedIndexChanged);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Location = new System.Drawing.Point(3, 290);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(80, 40);
            this.btnShowAll.TabIndex = 4;
            this.btnShowAll.Text = "S02";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // btnExport
            // 
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(262, 471);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 40);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "S06";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkGrid
            // 
            this.checkGrid.AutoSize = true;
            this.checkGrid.Location = new System.Drawing.Point(6, 19);
            this.checkGrid.Name = "checkGrid";
            this.checkGrid.Size = new System.Drawing.Size(46, 17);
            this.checkGrid.TabIndex = 6;
            this.checkGrid.Text = "S0C";
            this.checkGrid.UseVisualStyleBackColor = true;
            this.checkGrid.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkNumber
            // 
            this.checkNumber.AutoSize = true;
            this.checkNumber.Location = new System.Drawing.Point(147, 42);
            this.checkNumber.Name = "checkNumber";
            this.checkNumber.Size = new System.Drawing.Size(45, 17);
            this.checkNumber.TabIndex = 7;
            this.checkNumber.Text = "S10";
            this.checkNumber.UseVisualStyleBackColor = true;
            this.checkNumber.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkCellBorder
            // 
            this.checkCellBorder.AutoSize = true;
            this.checkCellBorder.Location = new System.Drawing.Point(6, 42);
            this.checkCellBorder.Name = "checkCellBorder";
            this.checkCellBorder.Size = new System.Drawing.Size(47, 17);
            this.checkCellBorder.TabIndex = 8;
            this.checkCellBorder.Text = "S0D";
            this.checkCellBorder.UseVisualStyleBackColor = true;
            this.checkCellBorder.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkTransparency
            // 
            this.checkTransparency.AutoSize = true;
            this.checkTransparency.Checked = true;
            this.checkTransparency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTransparency.Location = new System.Drawing.Point(147, 19);
            this.checkTransparency.Name = "checkTransparency";
            this.checkTransparency.Size = new System.Drawing.Size(45, 17);
            this.checkTransparency.TabIndex = 9;
            this.checkTransparency.Text = "S0F";
            this.checkTransparency.UseVisualStyleBackColor = true;
            this.checkTransparency.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkImage
            // 
            this.checkImage.AutoSize = true;
            this.checkImage.Checked = true;
            this.checkImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkImage.Location = new System.Drawing.Point(7, 65);
            this.checkImage.Name = "checkImage";
            this.checkImage.Size = new System.Drawing.Size(46, 17);
            this.checkImage.TabIndex = 10;
            this.checkImage.Text = "S0E";
            this.checkImage.UseVisualStyleBackColor = true;
            this.checkImage.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // btnBgdTrans
            // 
            this.btnBgdTrans.Enabled = false;
            this.btnBgdTrans.Location = new System.Drawing.Point(175, 290);
            this.btnBgdTrans.Name = "btnBgdTrans";
            this.btnBgdTrans.Size = new System.Drawing.Size(80, 40);
            this.btnBgdTrans.TabIndex = 29;
            this.btnBgdTrans.Text = "S09";
            this.btnBgdTrans.UseVisualStyleBackColor = true;
            this.btnBgdTrans.Click += new System.EventHandler(this.btnBgdTrans_Click);
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(261, 290);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(80, 40);
            this.btnBgd.TabIndex = 27;
            this.btnBgd.Text = "S08";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(358, 471);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 40);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "S07";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSetTrans
            // 
            this.btnSetTrans.Location = new System.Drawing.Point(89, 290);
            this.btnSetTrans.Name = "btnSetTrans";
            this.btnSetTrans.Size = new System.Drawing.Size(80, 40);
            this.btnSetTrans.TabIndex = 31;
            this.btnSetTrans.Text = "S0A";
            this.btnSetTrans.UseVisualStyleBackColor = true;
            this.btnSetTrans.Click += new System.EventHandler(this.btnSetTrans_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkCellBorder);
            this.groupBox1.Controls.Add(this.checkGrid);
            this.groupBox1.Controls.Add(this.checkNumber);
            this.groupBox1.Controls.Add(this.checkTransparency);
            this.groupBox1.Controls.Add(this.checkImage);
            this.groupBox1.Location = new System.Drawing.Point(6, 426);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 83);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "S0B";
            // 
            // btnOAMeditor
            // 
            this.btnOAMeditor.Location = new System.Drawing.Point(262, 425);
            this.btnOAMeditor.Name = "btnOAMeditor";
            this.btnOAMeditor.Size = new System.Drawing.Size(90, 40);
            this.btnOAMeditor.TabIndex = 35;
            this.btnOAMeditor.Text = "S05";
            this.btnOAMeditor.UseVisualStyleBackColor = true;
            this.btnOAMeditor.Click += new System.EventHandler(this.btnOAMeditor_Click);
            // 
            // checkBatch
            // 
            this.checkBatch.AutoSize = true;
            this.checkBatch.Location = new System.Drawing.Point(347, 315);
            this.checkBatch.Name = "checkBatch";
            this.checkBatch.Size = new System.Drawing.Size(45, 17);
            this.checkBatch.TabIndex = 36;
            this.checkBatch.Text = "S04";
            this.checkBatch.UseVisualStyleBackColor = true;
            this.checkBatch.CheckedChanged += new System.EventHandler(this.checkBatch_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(344, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "S03";
            // 
            // txtBatch
            // 
            this.txtBatch.Enabled = false;
            this.txtBatch.Location = new System.Drawing.Point(347, 291);
            this.txtBatch.Name = "txtBatch";
            this.txtBatch.Size = new System.Drawing.Size(159, 20);
            this.txtBatch.TabIndex = 38;
            // 
            // radioOriginalPal
            // 
            this.radioOriginalPal.AutoSize = true;
            this.radioOriginalPal.Location = new System.Drawing.Point(6, 42);
            this.radioOriginalPal.Name = "radioOriginalPal";
            this.radioOriginalPal.Size = new System.Drawing.Size(44, 17);
            this.radioOriginalPal.TabIndex = 39;
            this.radioOriginalPal.Text = "S13";
            this.radioOriginalPal.UseVisualStyleBackColor = true;
            // 
            // radioReplacePal
            // 
            this.radioReplacePal.AutoSize = true;
            this.radioReplacePal.Location = new System.Drawing.Point(6, 65);
            this.radioReplacePal.Name = "radioReplacePal";
            this.radioReplacePal.Size = new System.Drawing.Size(44, 17);
            this.radioReplacePal.TabIndex = 40;
            this.radioReplacePal.Text = "S14";
            this.radioReplacePal.UseVisualStyleBackColor = true;
            // 
            // radioSwapPal
            // 
            this.radioSwapPal.AutoSize = true;
            this.radioSwapPal.Checked = true;
            this.radioSwapPal.Location = new System.Drawing.Point(6, 19);
            this.radioSwapPal.Name = "radioSwapPal";
            this.radioSwapPal.Size = new System.Drawing.Size(44, 17);
            this.radioSwapPal.TabIndex = 41;
            this.radioSwapPal.TabStop = true;
            this.radioSwapPal.Text = "S11";
            this.radioSwapPal.UseVisualStyleBackColor = true;
            // 
            // numThreshold
            // 
            this.numThreshold.DecimalPlaces = 4;
            this.numThreshold.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numThreshold.Location = new System.Drawing.Point(223, 16);
            this.numThreshold.Maximum = new decimal(new int[] {
            442,
            0,
            0,
            0});
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(77, 20);
            this.numThreshold.TabIndex = 42;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "S16";
            // 
            // radioImgAdd
            // 
            this.radioImgAdd.AutoSize = true;
            this.radioImgAdd.Checked = true;
            this.radioImgAdd.Location = new System.Drawing.Point(6, 14);
            this.radioImgAdd.Name = "radioImgAdd";
            this.radioImgAdd.Size = new System.Drawing.Size(44, 17);
            this.radioImgAdd.TabIndex = 44;
            this.radioImgAdd.TabStop = true;
            this.radioImgAdd.Text = "S18";
            this.radioImgAdd.UseVisualStyleBackColor = true;
            // 
            // radioImgReplace
            // 
            this.radioImgReplace.AutoSize = true;
            this.radioImgReplace.Location = new System.Drawing.Point(6, 35);
            this.radioImgReplace.Name = "radioImgReplace";
            this.radioImgReplace.Size = new System.Drawing.Size(44, 17);
            this.radioImgReplace.TabIndex = 45;
            this.radioImgReplace.Text = "S19";
            this.radioImgReplace.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioSwapPal);
            this.groupBox2.Controls.Add(this.radioOriginalPal);
            this.groupBox2.Controls.Add(this.radioReplacePal);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numThreshold);
            this.groupBox2.Location = new System.Drawing.Point(3, 334);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(334, 86);
            this.groupBox2.TabIndex = 46;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "S15";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioImgAdd);
            this.groupBox3.Controls.Add(this.radioImgReplace);
            this.groupBox3.Location = new System.Drawing.Point(343, 334);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(163, 59);
            this.groupBox3.TabIndex = 47;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "S17";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 266);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "of ";
            // 
            // SpriteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtBatch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBatch);
            this.Controls.Add(this.btnOAMeditor);
            this.Controls.Add(this.imgBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetTrans);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnBgdTrans);
            this.Controls.Add(this.btnBgd);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnShowAll);
            this.Controls.Add(this.comboBank);
            this.Controls.Add(this.label1);
            this.Name = "SpriteControl";
            this.Size = new System.Drawing.Size(514, 514);
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBank;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox checkGrid;
        private System.Windows.Forms.CheckBox checkNumber;
        private System.Windows.Forms.CheckBox checkCellBorder;
        private System.Windows.Forms.CheckBox checkTransparency;
        private System.Windows.Forms.CheckBox checkImage;
        private System.Windows.Forms.Button btnBgdTrans;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSetTrans;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOAMeditor;
        private System.Windows.Forms.CheckBox checkBatch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBatch;
        private System.Windows.Forms.RadioButton radioOriginalPal;
        private System.Windows.Forms.RadioButton radioReplacePal;
        private System.Windows.Forms.RadioButton radioSwapPal;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioImgAdd;
        private System.Windows.Forms.RadioButton radioImgReplace;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
    }
}
