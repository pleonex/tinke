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
namespace TXT
{
    partial class iTXT
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iTXT));
            this.txtBox = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.comboEncod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkWordWrap = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtBox
            // 
            this.txtBox.Location = new System.Drawing.Point(0, 0);
            this.txtBox.Name = "txtBox";
            this.txtBox.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.ImageKey = "page_save.png";
            this.btnSave.ImageList = this.imageList1;
            this.btnSave.Location = new System.Drawing.Point(408, 472);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(99, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "S00";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "page_save.png");
            // 
            // comboEncod
            // 
            this.comboEncod.FormattingEnabled = true;
            this.comboEncod.Items.AddRange(new object[] {
            "shift_jis",
            "utf-7",
            "unicode",
            "utf-32",
            "utf-8",
            "ascii",
            "utf-16",
            "IBM00858"});
            this.comboEncod.Location = new System.Drawing.Point(82, 480);
            this.comboEncod.Name = "comboEncod";
            this.comboEncod.Size = new System.Drawing.Size(121, 21);
            this.comboEncod.TabIndex = 2;
            this.comboEncod.SelectedIndexChanged += new System.EventHandler(this.comboEncod_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 483);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "S01";
            // 
            // checkWordWrap
            // 
            this.checkWordWrap.AutoSize = true;
            this.checkWordWrap.Location = new System.Drawing.Point(236, 483);
            this.checkWordWrap.Name = "checkWordWrap";
            this.checkWordWrap.Size = new System.Drawing.Size(45, 17);
            this.checkWordWrap.TabIndex = 4;
            this.checkWordWrap.Text = "S02";
            this.checkWordWrap.UseVisualStyleBackColor = true;
            this.checkWordWrap.CheckedChanged += new System.EventHandler(this.checkWordWrap_CheckedChanged);
            // 
            // iTXT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.checkWordWrap);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboEncod);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtBox);
            this.Name = "iTXT";
            this.Size = new System.Drawing.Size(510, 510);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ComboBox comboEncod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkWordWrap;
    }
}
