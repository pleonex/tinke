// ----------------------------------------------------------------------
// <copyright file="MapOptions.Designer.cs" company="none">

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
// <date>28/04/2012 14:27:25</date>
// -----------------------------------------------------------------------
namespace Tinke.Dialog
{
    partial class MapOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapOptions));
            this.btnOk = new System.Windows.Forms.Button();
            this.numericWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericHeight = new System.Windows.Forms.NumericUpDown();
            this.checkFillTile = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericStartTile = new System.Windows.Forms.NumericUpDown();
            this.numericFillTile = new System.Windows.Forms.NumericUpDown();
            this.groupFill = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericMaxHeight = new System.Windows.Forms.NumericUpDown();
            this.numericMaxWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupSubImages = new System.Windows.Forms.GroupBox();
            this.numericSubPalette = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericSubStart = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.checkSubImage = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFillTile)).BeginInit();
            this.groupFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxWidth)).BeginInit();
            this.groupSubImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSubPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSubStart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Image = global::Tinke.Properties.Resources.accept;
            this.btnOk.Location = new System.Drawing.Point(15, 347);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 40);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "S06";
            this.btnOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // numericWidth
            // 
            this.numericWidth.Location = new System.Drawing.Point(131, 14);
            this.numericWidth.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericWidth.Name = "numericWidth";
            this.numericWidth.Size = new System.Drawing.Size(60, 20);
            this.numericWidth.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S01";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "S02";
            // 
            // numericHeight
            // 
            this.numericHeight.Location = new System.Drawing.Point(131, 38);
            this.numericHeight.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericHeight.Name = "numericHeight";
            this.numericHeight.Size = new System.Drawing.Size(60, 20);
            this.numericHeight.TabIndex = 4;
            // 
            // checkFillTile
            // 
            this.checkFillTile.AutoSize = true;
            this.checkFillTile.Location = new System.Drawing.Point(15, 75);
            this.checkFillTile.Name = "checkFillTile";
            this.checkFillTile.Size = new System.Drawing.Size(45, 17);
            this.checkFillTile.TabIndex = 5;
            this.checkFillTile.Text = "S03";
            this.checkFillTile.UseVisualStyleBackColor = true;
            this.checkFillTile.CheckedChanged += new System.EventHandler(this.checkFillTile_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "S04";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "S05";
            // 
            // numericStartTile
            // 
            this.numericStartTile.Hexadecimal = true;
            this.numericStartTile.Location = new System.Drawing.Point(134, 27);
            this.numericStartTile.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericStartTile.Name = "numericStartTile";
            this.numericStartTile.Size = new System.Drawing.Size(60, 20);
            this.numericStartTile.TabIndex = 8;
            this.numericStartTile.ValueChanged += new System.EventHandler(this.numericStartTile_ValueChanged);
            // 
            // numericFillTile
            // 
            this.numericFillTile.Hexadecimal = true;
            this.numericFillTile.Location = new System.Drawing.Point(134, 96);
            this.numericFillTile.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericFillTile.Name = "numericFillTile";
            this.numericFillTile.Size = new System.Drawing.Size(60, 20);
            this.numericFillTile.TabIndex = 9;
            // 
            // groupFill
            // 
            this.groupFill.Controls.Add(this.label6);
            this.groupFill.Controls.Add(this.numericMaxHeight);
            this.groupFill.Controls.Add(this.numericMaxWidth);
            this.groupFill.Controls.Add(this.label5);
            this.groupFill.Controls.Add(this.numericFillTile);
            this.groupFill.Controls.Add(this.label3);
            this.groupFill.Controls.Add(this.numericStartTile);
            this.groupFill.Controls.Add(this.label4);
            this.groupFill.Enabled = false;
            this.groupFill.Location = new System.Drawing.Point(15, 98);
            this.groupFill.Name = "groupFill";
            this.groupFill.Size = new System.Drawing.Size(371, 122);
            this.groupFill.TabIndex = 10;
            this.groupFill.TabStop = false;
            this.groupFill.Text = "S03";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(200, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "S08";
            // 
            // numericMaxHeight
            // 
            this.numericMaxHeight.Location = new System.Drawing.Point(305, 44);
            this.numericMaxHeight.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericMaxHeight.Name = "numericMaxHeight";
            this.numericMaxHeight.Size = new System.Drawing.Size(60, 20);
            this.numericMaxHeight.TabIndex = 12;
            this.numericMaxHeight.ValueChanged += new System.EventHandler(this.numericMaxSize_ValueChanged);
            // 
            // numericMaxWidth
            // 
            this.numericMaxWidth.Location = new System.Drawing.Point(305, 17);
            this.numericMaxWidth.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericMaxWidth.Name = "numericMaxWidth";
            this.numericMaxWidth.Size = new System.Drawing.Size(60, 20);
            this.numericMaxWidth.TabIndex = 11;
            this.numericMaxWidth.ValueChanged += new System.EventHandler(this.numericMaxSize_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(200, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "S07";
            // 
            // groupSubImages
            // 
            this.groupSubImages.Controls.Add(this.numericSubPalette);
            this.groupSubImages.Controls.Add(this.label8);
            this.groupSubImages.Controls.Add(this.numericSubStart);
            this.groupSubImages.Controls.Add(this.label7);
            this.groupSubImages.Enabled = false;
            this.groupSubImages.Location = new System.Drawing.Point(15, 264);
            this.groupSubImages.Name = "groupSubImages";
            this.groupSubImages.Size = new System.Drawing.Size(371, 77);
            this.groupSubImages.TabIndex = 11;
            this.groupSubImages.TabStop = false;
            this.groupSubImages.Text = "S1E";
            // 
            // numericSubPalette
            // 
            this.numericSubPalette.Location = new System.Drawing.Point(134, 54);
            this.numericSubPalette.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericSubPalette.Name = "numericSubPalette";
            this.numericSubPalette.Size = new System.Drawing.Size(60, 20);
            this.numericSubPalette.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "S1F";
            // 
            // numericSubStart
            // 
            this.numericSubStart.Hexadecimal = true;
            this.numericSubStart.Location = new System.Drawing.Point(134, 27);
            this.numericSubStart.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericSubStart.Name = "numericSubStart";
            this.numericSubStart.Size = new System.Drawing.Size(60, 20);
            this.numericSubStart.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "S1D";
            // 
            // checkSubImage
            // 
            this.checkSubImage.AutoSize = true;
            this.checkSubImage.Location = new System.Drawing.Point(15, 241);
            this.checkSubImage.Name = "checkSubImage";
            this.checkSubImage.Size = new System.Drawing.Size(46, 17);
            this.checkSubImage.TabIndex = 12;
            this.checkSubImage.Text = "S1E";
            this.checkSubImage.UseVisualStyleBackColor = true;
            this.checkSubImage.CheckedChanged += new System.EventHandler(this.checkSubImage_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::Tinke.Properties.Resources.cancel;
            this.btnCancel.Location = new System.Drawing.Point(131, 347);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 40);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "S0C";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MapOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(394, 392);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.checkSubImage);
            this.Controls.Add(this.groupSubImages);
            this.Controls.Add(this.groupFill);
            this.Controls.Add(this.checkFillTile);
            this.Controls.Add(this.numericHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericWidth);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapOptions";
            this.ShowInTaskbar = false;
            this.Text = "S00";
            ((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericFillTile)).EndInit();
            this.groupFill.ResumeLayout(false);
            this.groupFill.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxWidth)).EndInit();
            this.groupSubImages.ResumeLayout(false);
            this.groupSubImages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSubPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSubStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.NumericUpDown numericWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericHeight;
        private System.Windows.Forms.CheckBox checkFillTile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericStartTile;
        private System.Windows.Forms.NumericUpDown numericFillTile;
        private System.Windows.Forms.GroupBox groupFill;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericMaxHeight;
        private System.Windows.Forms.NumericUpDown numericMaxWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupSubImages;
        private System.Windows.Forms.NumericUpDown numericSubStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkSubImage;
        private System.Windows.Forms.NumericUpDown numericSubPalette;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCancel;
    }
}