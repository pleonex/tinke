namespace Nintendo
{
    partial class iNCGR
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
        	System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x06",
        	        	        	"Desconocido"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x0E",
        	        	        	"Nº secciones"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x10",
        	        	        	"ID Sección 1"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x14",
        	        	        	"Tamaño"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x18",
        	        	        	"Tiles Y"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x1A",
        	        	        	"Tiles X"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x1C",
        	        	        	"Formato"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x20",
        	        	        	"Desconocido"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x24",
        	        	        	"Desconocido"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x28",
        	        	        	"Tamaño píxels datos"}, -1);
        	System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
        	        	        	"0x2C",
        	        	        	"Desconocido"}, -1);
        	this.numericWidth = new System.Windows.Forms.NumericUpDown();
        	this.label1 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.numericHeight = new System.Windows.Forms.NumericUpDown();
        	this.groupProp = new System.Windows.Forms.GroupBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.comboDepth = new System.Windows.Forms.ComboBox();
        	this.label3 = new System.Windows.Forms.Label();
        	this.numericStart = new System.Windows.Forms.NumericUpDown();
        	this.listInfo = new System.Windows.Forms.ListView();
        	this.columnPos = new System.Windows.Forms.ColumnHeader();
        	this.columnCampo = new System.Windows.Forms.ColumnHeader();
        	this.columnValor = new System.Windows.Forms.ColumnHeader();
        	this.btnSave = new System.Windows.Forms.Button();
        	this.pic = new System.Windows.Forms.PictureBox();
        	((System.ComponentModel.ISupportInitialize)(this.numericWidth)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericHeight)).BeginInit();
        	this.groupProp.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericStart)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// numericWidth
        	// 
        	this.numericWidth.Increment = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericWidth.Location = new System.Drawing.Point(49, 219);
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
        	this.numericWidth.Size = new System.Drawing.Size(55, 20);
        	this.numericWidth.TabIndex = 1;
        	this.numericWidth.Value = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(6, 221);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(41, 13);
        	this.label1.TabIndex = 2;
        	this.label1.Text = "Ancho:";
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(147, 221);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(28, 13);
        	this.label2.TabIndex = 3;
        	this.label2.Text = "Alto:";
        	// 
        	// numericHeight
        	// 
        	this.numericHeight.Increment = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericHeight.Location = new System.Drawing.Point(181, 219);
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
        	this.numericHeight.Size = new System.Drawing.Size(55, 20);
        	this.numericHeight.TabIndex = 4;
        	this.numericHeight.Value = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// groupProp
        	// 
        	this.groupProp.Controls.Add(this.label4);
        	this.groupProp.Controls.Add(this.btnSave);
        	this.groupProp.Controls.Add(this.comboDepth);
        	this.groupProp.Controls.Add(this.label3);
        	this.groupProp.Controls.Add(this.numericStart);
        	this.groupProp.Controls.Add(this.listInfo);
        	this.groupProp.Controls.Add(this.numericHeight);
        	this.groupProp.Controls.Add(this.numericWidth);
        	this.groupProp.Controls.Add(this.label2);
        	this.groupProp.Controls.Add(this.label1);
        	this.groupProp.Dock = System.Windows.Forms.DockStyle.Right;
        	this.groupProp.Location = new System.Drawing.Point(269, 0);
        	this.groupProp.Name = "groupProp";
        	this.groupProp.Size = new System.Drawing.Size(243, 512);
        	this.groupProp.TabIndex = 5;
        	this.groupProp.TabStop = false;
        	this.groupProp.Text = "Propiedades";
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(147, 195);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(28, 13);
        	this.label4.TabIndex = 9;
        	this.label4.Text = "bpp:";
        	// 
        	// comboDepth
        	// 
        	this.comboDepth.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
        	this.comboDepth.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
        	this.comboDepth.FormattingEnabled = true;
        	this.comboDepth.Items.AddRange(new object[] {
        	        	        	"4 bpp",
        	        	        	"8 bpp"});
        	this.comboDepth.Location = new System.Drawing.Point(181, 192);
        	this.comboDepth.Name = "comboDepth";
        	this.comboDepth.Size = new System.Drawing.Size(54, 21);
        	this.comboDepth.TabIndex = 8;
        	this.comboDepth.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(6, 195);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(35, 13);
        	this.label3.TabIndex = 7;
        	this.label3.Text = "Inicio:";
        	// 
        	// numericStart
        	// 
        	this.numericStart.Location = new System.Drawing.Point(49, 193);
        	this.numericStart.Maximum = new decimal(new int[] {
        	        	        	1024,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericStart.Name = "numericStart";
        	this.numericStart.Size = new System.Drawing.Size(55, 20);
        	this.numericStart.TabIndex = 6;
        	// 
        	// listInfo
        	// 
        	this.listInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnPos,
        	        	        	this.columnCampo,
        	        	        	this.columnValor});
        	this.listInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        	this.listInfo.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
        	        	        	listViewItem1,
        	        	        	listViewItem2,
        	        	        	listViewItem3,
        	        	        	listViewItem4,
        	        	        	listViewItem5,
        	        	        	listViewItem6,
        	        	        	listViewItem7,
        	        	        	listViewItem8,
        	        	        	listViewItem9,
        	        	        	listViewItem10,
        	        	        	listViewItem11});
        	this.listInfo.Location = new System.Drawing.Point(7, 20);
        	this.listInfo.Name = "listInfo";
        	this.listInfo.Size = new System.Drawing.Size(229, 167);
        	this.listInfo.TabIndex = 5;
        	this.listInfo.UseCompatibleStateImageBehavior = false;
        	this.listInfo.View = System.Windows.Forms.View.Details;
        	// 
        	// columnPos
        	// 
        	this.columnPos.Text = "Posición";
        	// 
        	// columnCampo
        	// 
        	this.columnCampo.Text = "Campo";
        	// 
        	// columnValor
        	// 
        	this.columnValor.Text = "Valor";
        	this.columnValor.Width = 81;
        	// 
        	// btnSave
        	// 
        	this.btnSave.Location = new System.Drawing.Point(7, 474);
        	this.btnSave.Name = "btnSave";
        	this.btnSave.Size = new System.Drawing.Size(79, 32);
        	this.btnSave.TabIndex = 6;
        	this.btnSave.Text = "Guardar";
        	this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
        	this.btnSave.UseVisualStyleBackColor = true;
        	this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        	// 
        	// pic
        	// 
        	this.pic.BackColor = System.Drawing.Color.Transparent;
        	this.pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.pic.Location = new System.Drawing.Point(0, 0);
        	this.pic.Name = "pic";
        	this.pic.Size = new System.Drawing.Size(100, 50);
        	this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        	this.pic.TabIndex = 0;
        	this.pic.TabStop = false;
        	// 
        	// iNCGR
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoScroll = true;
        	this.BackColor = System.Drawing.Color.Transparent;
        	this.Controls.Add(this.pic);
        	this.Controls.Add(this.groupProp);
        	this.Name = "iNCGR";
        	this.Size = new System.Drawing.Size(512, 512);
        	this.SizeChanged += new System.EventHandler(this.iNCGR_SizeChanged);
        	((System.ComponentModel.ISupportInitialize)(this.numericWidth)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericHeight)).EndInit();
        	this.groupProp.ResumeLayout(false);
        	this.groupProp.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericStart)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
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
        private System.Windows.Forms.ListView listInfo;
        private System.Windows.Forms.ColumnHeader columnPos;
        private System.Windows.Forms.ColumnHeader columnCampo;
        private System.Windows.Forms.ColumnHeader columnValor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboDepth;
    }
}
