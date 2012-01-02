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
 * By: pleoNeX
 * 
 */
namespace Tinke
{
    partial class VisorHex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisorHex));
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.menuStripTop = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.goToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startOffsetSelect = new System.Windows.Forms.ToolStripTextBox();
            this.endOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endOffsetSelect = new System.Windows.Forms.ToolStripTextBox();
            this.relativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSearchBox = new System.Windows.Forms.ToolStripTextBox();
            this.rawBytesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftjisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodeBigEndianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultCharsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodingCombo = new System.Windows.Forms.ToolStripComboBox();
            this.openTabletblToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBasicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableGrid = new System.Windows.Forms.DataGridView();
            this.codeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStripTop.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // hexBox1
            // 
            this.hexBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 24);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(779, 268);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 0;
            this.hexBox1.UseFixedBytesPerLine = true;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.SelectionStartChanged += new System.EventHandler(this.hexBox1_SelectionLengthChanged);
            this.hexBox1.SelectionLengthChanged += new System.EventHandler(this.hexBox1_SelectionLengthChanged);
            // 
            // menuStripTop
            // 
            this.menuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStripTop.Location = new System.Drawing.Point(0, 0);
            this.menuStripTop.Name = "menuStripTop";
            this.menuStripTop.Size = new System.Drawing.Size(779, 24);
            this.menuStripTop.TabIndex = 1;
            this.menuStripTop.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "S00";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::Tinke.Properties.Resources.disk;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.saveToolStripMenuItem.Text = "S01";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToolStripMenuItem,
            this.selectRangeToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.toolsToolStripMenuItem.Text = "S02";
            // 
            // gotoToolStripMenuItem
            // 
            this.gotoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1,
            this.goToolStripMenuItem});
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.gotoToolStripMenuItem.Text = "S03";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.toolStripTextBox1.MaxLength = 8;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Text = "00000000";
            // 
            // goToolStripMenuItem
            // 
            this.goToolStripMenuItem.Image = global::Tinke.Properties.Resources.accept;
            this.goToolStripMenuItem.Name = "goToolStripMenuItem";
            this.goToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.goToolStripMenuItem.Text = "S05";
            this.goToolStripMenuItem.Click += new System.EventHandler(this.numericOffset_ValueChanged);
            // 
            // selectRangeToolStripMenuItem
            // 
            this.selectRangeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startOffsetToolStripMenuItem,
            this.endOffsetToolStripMenuItem,
            this.relativeToolStripMenuItem,
            this.goToolStripMenuItem1});
            this.selectRangeToolStripMenuItem.Name = "selectRangeToolStripMenuItem";
            this.selectRangeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.selectRangeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.selectRangeToolStripMenuItem.Text = "S04";
            // 
            // startOffsetToolStripMenuItem
            // 
            this.startOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startOffsetSelect});
            this.startOffsetToolStripMenuItem.Name = "startOffsetToolStripMenuItem";
            this.startOffsetToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.startOffsetToolStripMenuItem.Text = "S06";
            // 
            // startOffsetSelect
            // 
            this.startOffsetSelect.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.startOffsetSelect.MaxLength = 8;
            this.startOffsetSelect.Name = "startOffsetSelect";
            this.startOffsetSelect.Size = new System.Drawing.Size(100, 23);
            this.startOffsetSelect.Text = "00000000";
            // 
            // endOffsetToolStripMenuItem
            // 
            this.endOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.endOffsetSelect});
            this.endOffsetToolStripMenuItem.Name = "endOffsetToolStripMenuItem";
            this.endOffsetToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.endOffsetToolStripMenuItem.Text = "S07";
            // 
            // endOffsetSelect
            // 
            this.endOffsetSelect.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.endOffsetSelect.MaxLength = 8;
            this.endOffsetSelect.Name = "endOffsetSelect";
            this.endOffsetSelect.Size = new System.Drawing.Size(100, 23);
            this.endOffsetSelect.Text = "00000000";
            // 
            // relativeToolStripMenuItem
            // 
            this.relativeToolStripMenuItem.CheckOnClick = true;
            this.relativeToolStripMenuItem.Name = "relativeToolStripMenuItem";
            this.relativeToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.relativeToolStripMenuItem.Text = "S08";
            // 
            // goToolStripMenuItem1
            // 
            this.goToolStripMenuItem1.Image = global::Tinke.Properties.Resources.accept;
            this.goToolStripMenuItem1.Name = "goToolStripMenuItem1";
            this.goToolStripMenuItem1.Size = new System.Drawing.Size(92, 22);
            this.goToolStripMenuItem1.Text = "S05";
            this.goToolStripMenuItem1.Click += new System.EventHandler(this.goToolStripMenuItem1_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSearchBox,
            this.rawBytesToolStripMenuItem,
            this.shiftjisToolStripMenuItem,
            this.unicodeToolStripMenuItem,
            this.unicodeBigEndianToolStripMenuItem,
            this.defaultCharsToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.searchToolStripMenuItem.Text = "S09";
            // 
            // toolStripSearchBox
            // 
            this.toolStripSearchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripSearchBox.Name = "toolStripSearchBox";
            this.toolStripSearchBox.Size = new System.Drawing.Size(100, 23);
            // 
            // rawBytesToolStripMenuItem
            // 
            this.rawBytesToolStripMenuItem.Name = "rawBytesToolStripMenuItem";
            this.rawBytesToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.rawBytesToolStripMenuItem.Text = "Raw bytes";
            this.rawBytesToolStripMenuItem.Click += new System.EventHandler(this.rawBytesToolStripMenuItem_Click);
            // 
            // shiftjisToolStripMenuItem
            // 
            this.shiftjisToolStripMenuItem.Name = "shiftjisToolStripMenuItem";
            this.shiftjisToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.shiftjisToolStripMenuItem.Text = "Shift-jis";
            this.shiftjisToolStripMenuItem.Click += new System.EventHandler(this.shiftjisToolStripMenuItem_Click);
            // 
            // unicodeToolStripMenuItem
            // 
            this.unicodeToolStripMenuItem.Name = "unicodeToolStripMenuItem";
            this.unicodeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.unicodeToolStripMenuItem.Text = "Unicode";
            this.unicodeToolStripMenuItem.Click += new System.EventHandler(this.unicodeToolStripMenuItem_Click);
            // 
            // unicodeBigEndianToolStripMenuItem
            // 
            this.unicodeBigEndianToolStripMenuItem.Name = "unicodeBigEndianToolStripMenuItem";
            this.unicodeBigEndianToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.unicodeBigEndianToolStripMenuItem.Text = "Unicode Big Endian";
            this.unicodeBigEndianToolStripMenuItem.Click += new System.EventHandler(this.unicodeBigEndianToolStripMenuItem_Click);
            // 
            // defaultCharsToolStripMenuItem
            // 
            this.defaultCharsToolStripMenuItem.Name = "defaultCharsToolStripMenuItem";
            this.defaultCharsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.defaultCharsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.defaultCharsToolStripMenuItem.Text = "Default chars";
            this.defaultCharsToolStripMenuItem.Click += new System.EventHandler(this.defaultCharsToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodingToolStripMenuItem,
            this.createTableToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.optionsToolStripMenuItem.Text = "S0A";
            // 
            // encodingToolStripMenuItem
            // 
            this.encodingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodingCombo,
            this.openTabletblToolStripMenuItem});
            this.encodingToolStripMenuItem.Name = "encodingToolStripMenuItem";
            this.encodingToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.encodingToolStripMenuItem.Text = "S0B";
            // 
            // encodingCombo
            // 
            this.encodingCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encodingCombo.Items.AddRange(new object[] {
            "Default",
            "shift_jis",
            "utf-7",
            "utf-8",
            "ascii"});
            this.encodingCombo.Name = "encodingCombo";
            this.encodingCombo.Size = new System.Drawing.Size(121, 23);
            this.encodingCombo.SelectedIndexChanged += new System.EventHandler(this.comboBoxEncoding_SelectedIndexChanged);
            // 
            // openTabletblToolStripMenuItem
            // 
            this.openTabletblToolStripMenuItem.Name = "openTabletblToolStripMenuItem";
            this.openTabletblToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.openTabletblToolStripMenuItem.Text = "Open Table (.tbl)";
            this.openTabletblToolStripMenuItem.Click += new System.EventHandler(this.openTabletblToolStripMenuItem_Click);
            // 
            // createTableToolStripMenuItem
            // 
            this.createTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.saveToolStripMenuItem1,
            this.openOneToolStripMenuItem,
            this.createBasicToolStripMenuItem});
            this.createTableToolStripMenuItem.Name = "createTableToolStripMenuItem";
            this.createTableToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.createTableToolStripMenuItem.Text = "Create table";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // openOneToolStripMenuItem
            // 
            this.openOneToolStripMenuItem.Name = "openOneToolStripMenuItem";
            this.openOneToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.openOneToolStripMenuItem.Text = "Open one";
            this.openOneToolStripMenuItem.Click += new System.EventHandler(this.openOneToolStripMenuItem_Click);
            // 
            // createBasicToolStripMenuItem
            // 
            this.createBasicToolStripMenuItem.Name = "createBasicToolStripMenuItem";
            this.createBasicToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.createBasicToolStripMenuItem.Text = "Create basic";
            this.createBasicToolStripMenuItem.Click += new System.EventHandler(this.createBasicToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSelect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 291);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(779, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSelect
            // 
            this.toolStripSelect.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSelect.Name = "toolStripSelect";
            this.toolStripSelect.Size = new System.Drawing.Size(27, 17);
            this.toolStripSelect.Text = "S0C";
            // 
            // tableGrid
            // 
            this.tableGrid.AllowUserToOrderColumns = true;
            this.tableGrid.BackgroundColor = System.Drawing.Color.DarkGray;
            this.tableGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codeColumn,
            this.charColumn});
            this.tableGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableGrid.Location = new System.Drawing.Point(636, 24);
            this.tableGrid.Name = "tableGrid";
            this.tableGrid.Size = new System.Drawing.Size(143, 267);
            this.tableGrid.TabIndex = 3;
            this.tableGrid.Visible = false;
            this.tableGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.tableGrid_CellEndEdit);
            // 
            // codeColumn
            // 
            this.codeColumn.HeaderText = "Code";
            this.codeColumn.Name = "codeColumn";
            this.codeColumn.Width = 50;
            // 
            // charColumn
            // 
            this.charColumn.HeaderText = "Char";
            this.charColumn.Name = "charColumn";
            this.charColumn.Width = 50;
            // 
            // VisorHex
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(779, 313);
            this.Controls.Add(this.hexBox1);
            this.Controls.Add(this.tableGrid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStripTop;
            this.Name = "VisorHex";
            this.Text = "S41";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VisorHex_FormClosed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VisorHex_KeyUp);
            this.Resize += new System.EventHandler(this.VisorHex_Resize);
            this.menuStripTop.ResumeLayout(false);
            this.menuStripTop.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Be.Windows.Forms.HexBox hexBox1;
        private System.Windows.Forms.MenuStrip menuStripTop;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem goToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSelect;
        private System.Windows.Forms.ToolStripMenuItem selectRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox startOffsetSelect;
        private System.Windows.Forms.ToolStripMenuItem endOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox endOffsetSelect;
        private System.Windows.Forms.ToolStripMenuItem relativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodingToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox encodingCombo;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripSearchBox;
        private System.Windows.Forms.ToolStripMenuItem rawBytesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shiftjisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultCharsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unicodeBigEndianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTabletblToolStripMenuItem;
        private System.Windows.Forms.DataGridView tableGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn charColumn;
        private System.Windows.Forms.ToolStripMenuItem createTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createBasicToolStripMenuItem;



    }
}