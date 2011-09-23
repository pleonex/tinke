namespace Images
{
    partial class PaletteControl
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("S05");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("S06");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("S07");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("S08");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("S09");
            this.paletaBox = new System.Windows.Forms.PictureBox();
            this.nPaleta = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.groupProp = new System.Windows.Forms.GroupBox();
            this.listProp = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSave = new System.Windows.Forms.Button();
            this.lblRGB = new System.Windows.Forms.Label();
            this.groupModificar = new System.Windows.Forms.GroupBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnConverter = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericStartByte = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.paletaBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaleta)).BeginInit();
            this.groupProp.SuspendLayout();
            this.groupModificar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartByte)).BeginInit();
            this.SuspendLayout();
            // 
            // paletaBox
            // 
            this.paletaBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletaBox.Location = new System.Drawing.Point(0, 0);
            this.paletaBox.Name = "paletaBox";
            this.paletaBox.Size = new System.Drawing.Size(160, 160);
            this.paletaBox.TabIndex = 0;
            this.paletaBox.TabStop = false;
            this.paletaBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletaBox_MouseClick);
            // 
            // nPaleta
            // 
            this.nPaleta.Location = new System.Drawing.Point(123, 182);
            this.nPaleta.Name = "nPaleta";
            this.nPaleta.Size = new System.Drawing.Size(37, 20);
            this.nPaleta.TabIndex = 2;
            this.nPaleta.ValueChanged += new System.EventHandler(this.nPaleta_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "S01";
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(346, 166);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(160, 30);
            this.btnShow.TabIndex = 4;
            this.btnShow.Text = "S0B";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // groupProp
            // 
            this.groupProp.Controls.Add(this.listProp);
            this.groupProp.Location = new System.Drawing.Point(242, 4);
            this.groupProp.Name = "groupProp";
            this.groupProp.Size = new System.Drawing.Size(267, 156);
            this.groupProp.TabIndex = 5;
            this.groupProp.TabStop = false;
            this.groupProp.Text = "S02";
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnValor});
            this.listProp.Dock = System.Windows.Forms.DockStyle.Top;
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.listProp.Location = new System.Drawing.Point(3, 16);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(261, 134);
            this.listProp.TabIndex = 0;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnName
            // 
            this.columnName.Text = "S03";
            this.columnName.Width = 172;
            // 
            // columnValor
            // 
            this.columnValor.Text = "S04";
            this.columnValor.Width = 82;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(245, 166);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 30);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "S0A";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblRGB
            // 
            this.lblRGB.AutoSize = true;
            this.lblRGB.Location = new System.Drawing.Point(3, 163);
            this.lblRGB.Name = "lblRGB";
            this.lblRGB.Size = new System.Drawing.Size(33, 13);
            this.lblRGB.TabIndex = 7;
            this.lblRGB.Text = "RGB:";
            // 
            // groupModificar
            // 
            this.groupModificar.Controls.Add(this.btnImport);
            this.groupModificar.Controls.Add(this.btnConverter);
            this.groupModificar.Controls.Add(this.label2);
            this.groupModificar.Controls.Add(this.numericStartByte);
            this.groupModificar.Location = new System.Drawing.Point(245, 226);
            this.groupModificar.Name = "groupModificar";
            this.groupModificar.Size = new System.Drawing.Size(261, 93);
            this.groupModificar.TabIndex = 8;
            this.groupModificar.TabStop = false;
            this.groupModificar.Text = "S11";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(149, 54);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(106, 30);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "S13";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnConverter
            // 
            this.btnConverter.Location = new System.Drawing.Point(6, 54);
            this.btnConverter.Name = "btnConverter";
            this.btnConverter.Size = new System.Drawing.Size(137, 30);
            this.btnConverter.TabIndex = 2;
            this.btnConverter.Text = "S14";
            this.btnConverter.UseVisualStyleBackColor = true;
            this.btnConverter.Click += new System.EventHandler(this.btnConverter_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "S12";
            // 
            // numericStartByte
            // 
            this.numericStartByte.Location = new System.Drawing.Point(160, 22);
            this.numericStartByte.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericStartByte.Name = "numericStartByte";
            this.numericStartByte.Size = new System.Drawing.Size(95, 20);
            this.numericStartByte.TabIndex = 0;
            this.numericStartByte.ValueChanged += new System.EventHandler(this.numericStartByte_ValueChanged);
            // 
            // PaletteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.groupModificar);
            this.Controls.Add(this.lblRGB);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupProp);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nPaleta);
            this.Controls.Add(this.paletaBox);
            this.Name = "PaletteControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.paletaBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaleta)).EndInit();
            this.groupProp.ResumeLayout(false);
            this.groupModificar.ResumeLayout(false);
            this.groupModificar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericStartByte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox paletaBox;
        private System.Windows.Forms.NumericUpDown nPaleta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.GroupBox groupProp;
        private System.Windows.Forms.ListView listProp;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnValor;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblRGB;
        private System.Windows.Forms.GroupBox groupModificar;
        private System.Windows.Forms.Button btnConverter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericStartByte;
        internal System.Windows.Forms.Button btnImport;
    }
}
