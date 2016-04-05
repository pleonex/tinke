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
namespace Ekona.Images
{
    partial class ImageControl
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
            this.numericWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericHeight = new System.Windows.Forms.NumericUpDown();
            this.groupProp = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numTileSize = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numPal = new System.Windows.Forms.NumericUpDown();
            this.checkHex = new System.Windows.Forms.CheckBox();
            this.checkTransparency = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboDepth = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.checkMapCmp = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioSwapPal = new System.Windows.Forms.RadioButton();
            this.radioOriginalPal = new System.Windows.Forms.RadioButton();
            this.radioReplacePal = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.btnSetTrans = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnBgdRem = new System.Windows.Forms.Button();
            this.btnBgd = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
            this.groupProp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericWidth
            // 
            this.numericWidth.Location = new System.Drawing.Point(48, 76);
            this.numericWidth.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(71, 20);
            this.numericWidth.TabIndex = 1;
            this.numericWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericWidth.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S05";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "S06";
            // 
            // numericHeight
            // 
            this.numericHeight.Location = new System.Drawing.Point(165, 76);
            this.numericHeight.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericHeight.Name = "numericHeight";
            this.numericHeight.Size = new System.Drawing.Size(71, 20);
            this.numericHeight.TabIndex = 4;
            this.numericHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericHeight.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // groupProp
            // 
            this.groupProp.Controls.Add(this.label10);
            this.groupProp.Controls.Add(this.numTileSize);
            this.groupProp.Controls.Add(this.label9);
            this.groupProp.Controls.Add(this.numPal);
            this.groupProp.Controls.Add(this.checkHex);
            this.groupProp.Controls.Add(this.checkTransparency);
            this.groupProp.Controls.Add(this.label6);
            this.groupProp.Controls.Add(this.comboBox1);
            this.groupProp.Controls.Add(this.label4);
            this.groupProp.Controls.Add(this.comboDepth);
            this.groupProp.Controls.Add(this.label3);
            this.groupProp.Controls.Add(this.numericStart);
            this.groupProp.Controls.Add(this.numericHeight);
            this.groupProp.Controls.Add(this.numericWidth);
            this.groupProp.Controls.Add(this.label2);
            this.groupProp.Controls.Add(this.label1);
            this.groupProp.Location = new System.Drawing.Point(0, 306);
            this.groupProp.Name = "groupProp";
            this.groupProp.Size = new System.Drawing.Size(252, 206);
            this.groupProp.TabIndex = 5;
            this.groupProp.TabStop = false;
            this.groupProp.Text = "S02";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(128, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "S08";
            // 
            // numTileSize
            // 
            this.numTileSize.Location = new System.Drawing.Point(199, 141);
            this.numTileSize.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numTileSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTileSize.Name = "numTileSize";
            this.numTileSize.Size = new System.Drawing.Size(37, 20);
            this.numTileSize.TabIndex = 29;
            this.numTileSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numTileSize.ValueChanged += new System.EventHandler(this.numTileSize_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(128, 174);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "S09";
            // 
            // numPal
            // 
            this.numPal.Location = new System.Drawing.Point(199, 172);
            this.numPal.Name = "numPal";
            this.numPal.Size = new System.Drawing.Size(37, 20);
            this.numPal.TabIndex = 25;
            this.numPal.ValueChanged += new System.EventHandler(this.numPal_ValueChanged);
            // 
            // checkHex
            // 
            this.checkHex.AutoSize = true;
            this.checkHex.Location = new System.Drawing.Point(32, 46);
            this.checkHex.Name = "checkHex";
            this.checkHex.Size = new System.Drawing.Size(45, 17);
            this.checkHex.TabIndex = 24;
            this.checkHex.Text = "S04";
            this.checkHex.UseVisualStyleBackColor = true;
            this.checkHex.CheckedChanged += new System.EventHandler(this.checkHex_CheckedChanged);
            // 
            // checkTransparency
            // 
            this.checkTransparency.AutoSize = true;
            this.checkTransparency.Location = new System.Drawing.Point(9, 175);
            this.checkTransparency.Name = "checkTransparency";
            this.checkTransparency.Size = new System.Drawing.Size(46, 17);
            this.checkTransparency.TabIndex = 19;
            this.checkTransparency.Text = "S0A";
            this.checkTransparency.UseVisualStyleBackColor = true;
            this.checkTransparency.CheckedChanged += new System.EventHandler(this.checkTransparency_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "S07";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "S16",
            "S17"});
            this.comboBox1.Location = new System.Drawing.Point(116, 114);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(119, 21);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(128, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "BPP:";
            // 
            // comboDepth
            // 
            this.comboDepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDepth.FormattingEnabled = true;
            this.comboDepth.Items.AddRange(new object[] {
            "4bpp (16 colors)",
            "8bpp (256 colors)",
            "1bpp (2 colors)",
            "Direct (16 bpp)",
            "A3I5",
            "A5I3",
            "2bpp (4 colors)",
            "A4I4",
            "BGRA32",
            "ABGR32"});
            this.comboDepth.Location = new System.Drawing.Point(165, 19);
            this.comboDepth.Name = "comboDepth";
            this.comboDepth.Size = new System.Drawing.Size(83, 21);
            this.comboDepth.TabIndex = 8;
            this.comboDepth.SelectedIndexChanged += new System.EventHandler(this.comboDepth_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "S03";
            // 
            // numericStart
            // 
            this.numericStart.Location = new System.Drawing.Point(48, 20);
            this.numericStart.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericStart.Name = "numericStart";
            this.numericStart.Size = new System.Drawing.Size(71, 20);
            this.numericStart.TabIndex = 6;
            this.numericStart.ValueChanged += new System.EventHandler(this.numericStart_ValueChanged);
            // 
            // checkMapCmp
            // 
            this.checkMapCmp.AutoSize = true;
            this.checkMapCmp.Location = new System.Drawing.Point(258, 444);
            this.checkMapCmp.Name = "checkMapCmp";
            this.checkMapCmp.Size = new System.Drawing.Size(45, 17);
            this.checkMapCmp.TabIndex = 48;
            this.checkMapCmp.Text = "S0F";
            this.checkMapCmp.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioSwapPal);
            this.groupBox2.Controls.Add(this.radioOriginalPal);
            this.groupBox2.Controls.Add(this.radioReplacePal);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numThreshold);
            this.groupBox2.Location = new System.Drawing.Point(258, 352);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(254, 86);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "S16";
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
            this.radioSwapPal.Text = "S17";
            this.radioSwapPal.UseVisualStyleBackColor = true;
            // 
            // radioOriginalPal
            // 
            this.radioOriginalPal.AutoSize = true;
            this.radioOriginalPal.Location = new System.Drawing.Point(6, 42);
            this.radioOriginalPal.Name = "radioOriginalPal";
            this.radioOriginalPal.Size = new System.Drawing.Size(45, 17);
            this.radioOriginalPal.TabIndex = 39;
            this.radioOriginalPal.Text = "S0E";
            this.radioOriginalPal.UseVisualStyleBackColor = true;
            // 
            // radioReplacePal
            // 
            this.radioReplacePal.AutoSize = true;
            this.radioReplacePal.Location = new System.Drawing.Point(6, 65);
            this.radioReplacePal.Name = "radioReplacePal";
            this.radioReplacePal.Size = new System.Drawing.Size(44, 17);
            this.radioReplacePal.TabIndex = 40;
            this.radioReplacePal.Text = "S18";
            this.radioReplacePal.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(174, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 43;
            this.label7.Text = "S19";
            // 
            // numThreshold
            // 
            this.numThreshold.DecimalPlaces = 4;
            this.numThreshold.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numThreshold.Location = new System.Drawing.Point(177, 29);
            this.numThreshold.Maximum = new decimal(new int[] {
            442,
            0,
            0,
            0});
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(77, 20);
            this.numThreshold.TabIndex = 42;
            // 
            // btnSetTrans
            // 
            this.btnSetTrans.Location = new System.Drawing.Point(258, 306);
            this.btnSetTrans.Name = "btnSetTrans";
            this.btnSetTrans.Size = new System.Drawing.Size(80, 40);
            this.btnSetTrans.TabIndex = 28;
            this.btnSetTrans.Text = "S0B";
            this.btnSetTrans.UseVisualStyleBackColor = true;
            this.btnSetTrans.Click += new System.EventHandler(this.btnSetTrans_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(429, 469);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(80, 40);
            this.btnImport.TabIndex = 23;
            this.btnImport.Text = "S11";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnBgdRem
            // 
            this.btnBgdRem.Enabled = false;
            this.btnBgdRem.Location = new System.Drawing.Point(430, 306);
            this.btnBgdRem.Name = "btnBgdRem";
            this.btnBgdRem.Size = new System.Drawing.Size(80, 40);
            this.btnBgdRem.TabIndex = 22;
            this.btnBgdRem.Text = "S0D";
            this.btnBgdRem.UseVisualStyleBackColor = true;
            this.btnBgdRem.Click += new System.EventHandler(this.btnBgdTrans_Click);
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(344, 306);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(80, 40);
            this.btnBgd.TabIndex = 20;
            this.btnBgd.Text = "S0C";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(332, 469);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 40);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "S10";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 290);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "S01";
            // 
            // pic
            // 
            this.pic.BackColor = System.Drawing.Color.Transparent;
            this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pic.Location = new System.Drawing.Point(0, 0);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(100, 100);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            this.pic.DoubleClick += new System.EventHandler(this.pic_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pic);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 287);
            this.panel1.TabIndex = 7;
            // 
            // ImageControl
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.checkMapCmp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSetTrans);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupProp);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnBgdRem);
            this.Controls.Add(this.btnBgd);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
            this.groupProp.ResumeLayout(false);
            this.groupProp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericHeight;
        private System.Windows.Forms.GroupBox groupProp;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboDepth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkTransparency;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Button btnBgdRem;
        internal System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkHex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numPal;
        private System.Windows.Forms.Button btnSetTrans;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numTileSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioSwapPal;
        private System.Windows.Forms.RadioButton radioOriginalPal;
        private System.Windows.Forms.RadioButton radioReplacePal;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.CheckBox checkMapCmp;
    }
}
