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
namespace AI_IGO_DS
{
    partial class BinControl
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
            this.picBox = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericImage = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericWidth = new System.Windows.Forms.NumericUpDown();
            this.numericHeight = new System.Windows.Forms.NumericUpDown();
            this.trackZoom = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.checkTransparency = new System.Windows.Forms.CheckBox();
            this.btnBgd = new System.Windows.Forms.Button();
            this.btnBgdRem = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // picBox
            // 
            this.picBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox.Location = new System.Drawing.Point(4, 4);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(256, 192);
            this.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBox.TabIndex = 0;
            this.picBox.TabStop = false;
            this.picBox.DoubleClick += new System.EventHandler(this.picBox_DoubleClick);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::AI_IGO_DS.Properties.Resources.picture_save;
            this.btnSave.Location = new System.Drawing.Point(399, 472);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(108, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "S08";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 275);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S00";
            // 
            // numericImage
            // 
            this.numericImage.Location = new System.Drawing.Point(387, 273);
            this.numericImage.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericImage.Name = "numericImage";
            this.numericImage.Size = new System.Drawing.Size(60, 20);
            this.numericImage.TabIndex = 3;
            this.numericImage.ValueChanged += new System.EventHandler(this.numericImage_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 327);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "S02";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(309, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "S01";
            // 
            // numericWidth
            // 
            this.numericWidth.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericWidth.Location = new System.Drawing.Point(387, 299);
            this.numericWidth.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericWidth.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(60, 20);
            this.numericWidth.TabIndex = 7;
            this.numericWidth.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericWidth.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // numericHeight
            // 
            this.numericHeight.Increment = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.Location = new System.Drawing.Point(387, 325);
            this.numericHeight.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericHeight.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.Name = "numericHeight";
            this.numericHeight.Size = new System.Drawing.Size(60, 20);
            this.numericHeight.TabIndex = 6;
            this.numericHeight.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericHeight.ValueChanged += new System.EventHandler(this.numericSize_ValueChanged);
            // 
            // trackZoom
            // 
            this.trackZoom.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.trackZoom.LargeChange = 100;
            this.trackZoom.Location = new System.Drawing.Point(453, 239);
            this.trackZoom.Maximum = 800;
            this.trackZoom.Minimum = 50;
            this.trackZoom.Name = "trackZoom";
            this.trackZoom.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackZoom.Size = new System.Drawing.Size(45, 227);
            this.trackZoom.SmallChange = 50;
            this.trackZoom.TabIndex = 10;
            this.trackZoom.TickFrequency = 50;
            this.trackZoom.Value = 100;
            this.trackZoom.Scroll += new System.EventHandler(this.trackZoom_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(450, 223);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "S07";
            // 
            // checkTransparency
            // 
            this.checkTransparency.AutoSize = true;
            this.checkTransparency.Location = new System.Drawing.Point(312, 351);
            this.checkTransparency.Name = "checkTransparency";
            this.checkTransparency.Size = new System.Drawing.Size(45, 17);
            this.checkTransparency.TabIndex = 12;
            this.checkTransparency.Text = "S03";
            this.checkTransparency.UseVisualStyleBackColor = true;
            this.checkTransparency.CheckedChanged += new System.EventHandler(this.checkTransparency_CheckedChanged);
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(312, 374);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(135, 31);
            this.btnBgd.TabIndex = 13;
            this.btnBgd.Text = "S04";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // btnBgdRem
            // 
            this.btnBgdRem.Location = new System.Drawing.Point(312, 412);
            this.btnBgdRem.Name = "btnBgdRem";
            this.btnBgdRem.Size = new System.Drawing.Size(135, 31);
            this.btnBgdRem.TabIndex = 14;
            this.btnBgdRem.Text = "S05";
            this.btnBgdRem.UseVisualStyleBackColor = true;
            this.btnBgdRem.Click += new System.EventHandler(this.btnBgdRem_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(266, 472);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "S08";
            // 
            // BinControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnBgdRem);
            this.Controls.Add(this.btnBgd);
            this.Controls.Add(this.checkTransparency);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackZoom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericWidth);
            this.Controls.Add(this.numericHeight);
            this.Controls.Add(this.numericImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picBox);
            this.Name = "BinControl";
            this.Size = new System.Drawing.Size(510, 510);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericImage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.NumericUpDown numericHeight;
        private System.Windows.Forms.TrackBar trackZoom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkTransparency;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Button btnBgdRem;
        private System.Windows.Forms.Label label5;
    }
}
