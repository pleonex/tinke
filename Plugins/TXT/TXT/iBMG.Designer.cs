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
    partial class iBMG
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("S03");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "S04", System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "INF1", System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))))}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("S05");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("S06");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("S07");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("S08");
            this.label1 = new System.Windows.Forms.Label();
            this.numericMsg = new System.Windows.Forms.NumericUpDown();
            this.listProp = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtMsg = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericMsg)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(334, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "S09";
            // 
            // numericMsg
            // 
            this.numericMsg.Location = new System.Drawing.Point(445, 276);
            this.numericMsg.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericMsg.Name = "numericMsg";
            this.numericMsg.Size = new System.Drawing.Size(61, 20);
            this.numericMsg.TabIndex = 6;
            this.numericMsg.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericMsg.ValueChanged += new System.EventHandler(this.numericMsg_ValueChanged);
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listProp.Location = new System.Drawing.Point(337, 0);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(170, 264);
            this.listProp.TabIndex = 5;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "S01";
            this.columnHeader1.Width = 89;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "S02";
            this.columnHeader2.Width = 75;
            // 
            // txtMsg
            // 
            this.txtMsg.Location = new System.Drawing.Point(3, 0);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ReadOnly = true;
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMsg.Size = new System.Drawing.Size(325, 296);
            this.txtMsg.TabIndex = 4;
            this.txtMsg.WordWrap = false;
            // 
            // iBMG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericMsg);
            this.Controls.Add(this.listProp);
            this.Controls.Add(this.txtMsg);
            this.Name = "iBMG";
            this.Size = new System.Drawing.Size(510, 510);
            ((System.ComponentModel.ISupportInitialize)(this.numericMsg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericMsg;
        private System.Windows.Forms.ListView listProp;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txtMsg;
    }
}
