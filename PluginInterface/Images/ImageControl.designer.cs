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
namespace PluginInterface.Images
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
            this.btnSetTrans = new System.Windows.Forms.Button();
            this.btnFotochoh = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.numPal = new System.Windows.Forms.NumericUpDown();
            this.checkHex = new System.Windows.Forms.CheckBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnBgdTrans = new System.Windows.Forms.Button();
            this.btnBgd = new System.Windows.Forms.Button();
            this.checkTransparency = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblZoom = new System.Windows.Forms.Label();
            this.trackZoom = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.comboDepth = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericStart = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.pic = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
            this.groupProp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericWidth
            // 
            this.numericWidth.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericWidth.Location = new System.Drawing.Point(48, 76);
            this.numericWidth.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericWidth.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(71, 20);
            this.numericWidth.TabIndex = 1;
            this.numericWidth.Value = new decimal(new int[] {
            8,
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
            this.label1.Text = "S12";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "S13";
            // 
            // numericHeight
            // 
            this.numericHeight.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.Location = new System.Drawing.Point(165, 76);
            this.numericHeight.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.numericHeight.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.Name = "numericHeight";
            this.numericHeight.Size = new System.Drawing.Size(71, 20);
            this.numericHeight.TabIndex = 4;
            this.numericHeight.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // groupProp
            // 
            this.groupProp.Controls.Add(this.label10);
            this.groupProp.Controls.Add(this.numTileSize);
            this.groupProp.Controls.Add(this.btnSetTrans);
            this.groupProp.Controls.Add(this.btnFotochoh);
            this.groupProp.Controls.Add(this.label9);
            this.groupProp.Controls.Add(this.numPal);
            this.groupProp.Controls.Add(this.checkHex);
            this.groupProp.Controls.Add(this.btnImport);
            this.groupProp.Controls.Add(this.btnBgdTrans);
            this.groupProp.Controls.Add(this.btnBgd);
            this.groupProp.Controls.Add(this.checkTransparency);
            this.groupProp.Controls.Add(this.label8);
            this.groupProp.Controls.Add(this.label7);
            this.groupProp.Controls.Add(this.lblZoom);
            this.groupProp.Controls.Add(this.trackZoom);
            this.groupProp.Controls.Add(this.label6);
            this.groupProp.Controls.Add(this.comboBox1);
            this.groupProp.Controls.Add(this.label4);
            this.groupProp.Controls.Add(this.btnSave);
            this.groupProp.Controls.Add(this.comboDepth);
            this.groupProp.Controls.Add(this.label3);
            this.groupProp.Controls.Add(this.numericStart);
            this.groupProp.Controls.Add(this.numericHeight);
            this.groupProp.Controls.Add(this.numericWidth);
            this.groupProp.Controls.Add(this.label2);
            this.groupProp.Controls.Add(this.label1);
            this.groupProp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupProp.Location = new System.Drawing.Point(0, 306);
            this.groupProp.Name = "groupProp";
            this.groupProp.Size = new System.Drawing.Size(512, 206);
            this.groupProp.TabIndex = 5;
            this.groupProp.TabStop = false;
            this.groupProp.Text = "S02";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(128, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Tile size:";
            // 
            // numTileSize
            // 
            this.numTileSize.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numTileSize.Location = new System.Drawing.Point(199, 141);
            this.numTileSize.Maximum = new decimal(new int[] {
            1024,
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
            // btnSetTrans
            // 
            this.btnSetTrans.Location = new System.Drawing.Point(254, 19);
            this.btnSetTrans.Name = "btnSetTrans";
            this.btnSetTrans.Size = new System.Drawing.Size(80, 40);
            this.btnSetTrans.TabIndex = 28;
            this.btnSetTrans.Text = "Set transparency";
            this.btnSetTrans.UseVisualStyleBackColor = true;
            this.btnSetTrans.Click += new System.EventHandler(this.btnSetTrans_Click);
            // 
            // btnFotochoh
            // 
            this.btnFotochoh.Location = new System.Drawing.Point(254, 160);
            this.btnFotochoh.Name = "btnFotochoh";
            this.btnFotochoh.Size = new System.Drawing.Size(80, 40);
            this.btnFotochoh.TabIndex = 27;
            this.btnFotochoh.Text = "Fotochoh";
            this.btnFotochoh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFotochoh.UseVisualStyleBackColor = true;
            this.btnFotochoh.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(128, 174);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Nº palette:";
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
            this.checkHex.Size = new System.Drawing.Size(87, 17);
            this.checkHex.TabIndex = 24;
            this.checkHex.Text = "Hexadecimal";
            this.checkHex.UseVisualStyleBackColor = true;
            this.checkHex.CheckedChanged += new System.EventHandler(this.checkHex_CheckedChanged);
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(426, 160);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(80, 40);
            this.btnImport.TabIndex = 23;
            this.btnImport.Text = "S21";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnBgdTrans
            // 
            this.btnBgdTrans.Enabled = false;
            this.btnBgdTrans.Location = new System.Drawing.Point(426, 19);
            this.btnBgdTrans.Name = "btnBgdTrans";
            this.btnBgdTrans.Size = new System.Drawing.Size(80, 40);
            this.btnBgdTrans.TabIndex = 22;
            this.btnBgdTrans.Text = "S20";
            this.btnBgdTrans.UseVisualStyleBackColor = true;
            this.btnBgdTrans.Click += new System.EventHandler(this.btnBgdTrans_Click);
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(340, 19);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(80, 40);
            this.btnBgd.TabIndex = 20;
            this.btnBgd.Text = "S1F";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // checkTransparency
            // 
            this.checkTransparency.AutoSize = true;
            this.checkTransparency.Location = new System.Drawing.Point(9, 175);
            this.checkTransparency.Name = "checkTransparency";
            this.checkTransparency.Size = new System.Drawing.Size(46, 17);
            this.checkTransparency.TabIndex = 19;
            this.checkTransparency.Text = "S1C";
            this.checkTransparency.UseVisualStyleBackColor = true;
            this.checkTransparency.CheckedChanged += new System.EventHandler(this.checkTransparency_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(468, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "20";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(269, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "1";
            // 
            // lblZoom
            // 
            this.lblZoom.AutoSize = true;
            this.lblZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZoom.Location = new System.Drawing.Point(344, 74);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(34, 17);
            this.lblZoom.TabIndex = 16;
            this.lblZoom.Text = "S1E";
            // 
            // trackZoom
            // 
            this.trackZoom.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.trackZoom.Enabled = false;
            this.trackZoom.LargeChange = 2;
            this.trackZoom.Location = new System.Drawing.Point(261, 101);
            this.trackZoom.Maximum = 20;
            this.trackZoom.Minimum = 1;
            this.trackZoom.Name = "trackZoom";
            this.trackZoom.Size = new System.Drawing.Size(226, 45);
            this.trackZoom.SmallChange = 50;
            this.trackZoom.TabIndex = 15;
            this.trackZoom.Value = 1;
            this.trackZoom.Scroll += new System.EventHandler(this.trackZoom_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "S14";
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
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(340, 160);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "S15";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            "2bpp (4 colors)"});
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
            this.label3.Text = "S11";
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupProp);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
            this.groupProp.ResumeLayout(false);
            this.groupProp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
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
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboDepth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.TrackBar trackZoom;
        private System.Windows.Forms.CheckBox checkTransparency;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Button btnBgdTrans;
        internal System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkHex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numPal;
        private System.Windows.Forms.Button btnFotochoh;
        private System.Windows.Forms.Button btnSetTrans;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numTileSize;
    }
}
