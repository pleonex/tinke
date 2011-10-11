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
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodingCombo = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripSelect = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripTop.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hexBox1
            // 
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.hexBox1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(0, 24);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(809, 268);
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
            this.menuStripTop.Size = new System.Drawing.Size(809, 24);
            this.menuStripTop.TabIndex = 1;
            this.menuStripTop.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::Tinke.Properties.Resources.disk;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToolStripMenuItem,
            this.selectRangeToolStripMenuItem,
            this.searchToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // gotoToolStripMenuItem
            // 
            this.gotoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1,
            this.goToolStripMenuItem});
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.gotoToolStripMenuItem.Text = "Goto (0x)";
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
            this.goToolStripMenuItem.Text = "Go";
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
            this.selectRangeToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.selectRangeToolStripMenuItem.Text = "Select range";
            // 
            // startOffsetToolStripMenuItem
            // 
            this.startOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startOffsetSelect});
            this.startOffsetToolStripMenuItem.Name = "startOffsetToolStripMenuItem";
            this.startOffsetToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.startOffsetToolStripMenuItem.Text = "Start offset (0x)";
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
            this.endOffsetToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.endOffsetToolStripMenuItem.Text = "End offset (0x)";
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
            this.relativeToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.relativeToolStripMenuItem.Text = "Relative";
            // 
            // goToolStripMenuItem1
            // 
            this.goToolStripMenuItem1.Image = global::Tinke.Properties.Resources.accept;
            this.goToolStripMenuItem1.Name = "goToolStripMenuItem1";
            this.goToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.goToolStripMenuItem1.Text = "Go";
            this.goToolStripMenuItem1.Click += new System.EventHandler(this.goToolStripMenuItem1_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodingToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // encodingToolStripMenuItem
            // 
            this.encodingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodingCombo});
            this.encodingToolStripMenuItem.Name = "encodingToolStripMenuItem";
            this.encodingToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.encodingToolStripMenuItem.Text = "Encoding";
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSelect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 291);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(809, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripSelect
            // 
            this.toolStripSelect.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSelect.Name = "toolStripSelect";
            this.toolStripSelect.Size = new System.Drawing.Size(57, 17);
            this.toolStripSelect.Text = "Selected: ";
            // 
            // VisorHex
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(809, 313);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.hexBox1);
            this.Controls.Add(this.menuStripTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripTop;
            this.Name = "VisorHex";
            this.Text = "S41";
            this.Resize += new System.EventHandler(this.VisorHex_Resize);
            this.menuStripTop.ResumeLayout(false);
            this.menuStripTop.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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



    }
}