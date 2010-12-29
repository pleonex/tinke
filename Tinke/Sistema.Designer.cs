namespace Tinke
{
    partial class Sistema
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sistema));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Nombre");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("ID");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Offset");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Tamaño");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Tipo");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Ruta");
            this.btnDeleteChain = new System.Windows.Forms.Button();
            this.iconos = new System.Windows.Forms.ImageList(this.components);
            this.btnExtraer = new System.Windows.Forms.Button();
            this.btnSee = new System.Windows.Forms.Button();
            this.btnHex = new System.Windows.Forms.Button();
            this.listFile = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeSystem = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripInfoRom = new System.Windows.Forms.ToolStripButton();
            this.toolStripDebug = new System.Windows.Forms.ToolStripButton();
            this.btnDescomprimir = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDeleteChain
            // 
            this.btnDeleteChain.ImageKey = "picture_delete.png";
            this.btnDeleteChain.ImageList = this.iconos;
            this.btnDeleteChain.Location = new System.Drawing.Point(409, 275);
            this.btnDeleteChain.Name = "btnDeleteChain";
            this.btnDeleteChain.Size = new System.Drawing.Size(98, 32);
            this.btnDeleteChain.TabIndex = 5;
            this.btnDeleteChain.Text = "Borrar cadena";
            this.btnDeleteChain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteChain.UseVisualStyleBackColor = true;
            this.btnDeleteChain.Click += new System.EventHandler(this.btnDeleteChain_Click);
            // 
            // iconos
            // 
            this.iconos.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconos.ImageStream")));
            this.iconos.TransparentColor = System.Drawing.Color.Transparent;
            this.iconos.Images.SetKeyName(0, "folder.png");
            this.iconos.Images.SetKeyName(1, "page_white.png");
            this.iconos.Images.SetKeyName(2, "palette.png");
            this.iconos.Images.SetKeyName(3, "image.png");
            this.iconos.Images.SetKeyName(4, "page_white_text.png");
            this.iconos.Images.SetKeyName(5, "compress.png");
            this.iconos.Images.SetKeyName(6, "package.png");
            this.iconos.Images.SetKeyName(7, "package_go.png");
            this.iconos.Images.SetKeyName(8, "images.png");
            this.iconos.Images.SetKeyName(9, "image_link.png");
            this.iconos.Images.SetKeyName(10, "photo.png");
            this.iconos.Images.SetKeyName(11, "picture_save.png");
            this.iconos.Images.SetKeyName(12, "picture_delete.png");
            this.iconos.Images.SetKeyName(13, "film.png");
            this.iconos.Images.SetKeyName(14, "music.png");
            // 
            // btnExtraer
            // 
            this.btnExtraer.ImageKey = "package_go.png";
            this.btnExtraer.ImageList = this.iconos;
            this.btnExtraer.Location = new System.Drawing.Point(514, 275);
            this.btnExtraer.Name = "btnExtraer";
            this.btnExtraer.Size = new System.Drawing.Size(92, 32);
            this.btnExtraer.TabIndex = 4;
            this.btnExtraer.Text = "Extraer";
            this.btnExtraer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtraer.UseVisualStyleBackColor = true;
            this.btnExtraer.Click += new System.EventHandler(this.btnExtraer_Click);
            // 
            // btnSee
            // 
            this.btnSee.Enabled = false;
            this.btnSee.Image = ((System.Drawing.Image)(resources.GetObject("btnSee.Image")));
            this.btnSee.Location = new System.Drawing.Point(409, 313);
            this.btnSee.Name = "btnSee";
            this.btnSee.Size = new System.Drawing.Size(98, 32);
            this.btnSee.TabIndex = 2;
            this.btnSee.Text = "Ver";
            this.btnSee.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSee.UseVisualStyleBackColor = true;
            this.btnSee.Click += new System.EventHandler(this.BtnSee);
            // 
            // btnHex
            // 
            this.btnHex.Enabled = false;
            this.btnHex.Image = global::Tinke.Properties.Resources.calculator;
            this.btnHex.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHex.Location = new System.Drawing.Point(514, 313);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(92, 32);
            this.btnHex.TabIndex = 1;
            this.btnHex.Text = "Hexadecimal";
            this.btnHex.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
            // 
            // listFile
            // 
            this.listFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listFile.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6});
            this.listFile.Location = new System.Drawing.Point(409, 28);
            this.listFile.Name = "listFile";
            this.listFile.Size = new System.Drawing.Size(197, 202);
            this.listFile.TabIndex = 0;
            this.listFile.UseCompatibleStateImageBehavior = false;
            this.listFile.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Campo";
            this.columnHeader1.Width = 72;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Valor";
            this.columnHeader2.Width = 116;
            // 
            // treeSystem
            // 
            this.treeSystem.ImageIndex = 0;
            this.treeSystem.ImageList = this.iconos;
            this.treeSystem.Location = new System.Drawing.Point(0, 28);
            this.treeSystem.Name = "treeSystem";
            this.treeSystem.SelectedImageIndex = 0;
            this.treeSystem.Size = new System.Drawing.Size(403, 319);
            this.treeSystem.TabIndex = 0;
            this.treeSystem.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeSystem_AfterSelect);
            this.treeSystem.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeSystem_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpen,
            this.toolStripInfoRom,
            this.toolStripDebug});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(614, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripOpen
            // 
            this.toolStripOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripOpen.Image")));
            this.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpen.Name = "toolStripOpen";
            this.toolStripOpen.Size = new System.Drawing.Size(83, 22);
            this.toolStripOpen.Text = "Abrir ROM";
            this.toolStripOpen.Click += new System.EventHandler(this.toolStripOpen_Click);
            // 
            // toolStripInfoRom
            // 
            this.toolStripInfoRom.CheckOnClick = true;
            this.toolStripInfoRom.Image = ((System.Drawing.Image)(resources.GetObject("toolStripInfoRom.Image")));
            this.toolStripInfoRom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInfoRom.Name = "toolStripInfoRom";
            this.toolStripInfoRom.Size = new System.Drawing.Size(144, 22);
            this.toolStripInfoRom.Text = "Información del juego";
            this.toolStripInfoRom.Click += new System.EventHandler(this.toolStripInfoRom_Click);
            // 
            // toolStripDebug
            // 
            this.toolStripDebug.CheckOnClick = true;
            this.toolStripDebug.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDebug.Image")));
            this.toolStripDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDebug.Name = "toolStripDebug";
            this.toolStripDebug.Size = new System.Drawing.Size(113, 22);
            this.toolStripDebug.Text = "Mensajes debug";
            this.toolStripDebug.Click += new System.EventHandler(this.toolStripDebug_Click);
            // 
            // btnDescomprimir
            // 
            this.btnDescomprimir.ImageKey = "compress.png";
            this.btnDescomprimir.ImageList = this.iconos;
            this.btnDescomprimir.Location = new System.Drawing.Point(410, 237);
            this.btnDescomprimir.Name = "btnDescomprimir";
            this.btnDescomprimir.Size = new System.Drawing.Size(97, 32);
            this.btnDescomprimir.TabIndex = 7;
            this.btnDescomprimir.Text = "Descomprimir";
            this.btnDescomprimir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDescomprimir.UseVisualStyleBackColor = true;
            this.btnDescomprimir.Click += new System.EventHandler(this.btnUncompress_Click);
            // 
            // Sistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(614, 352);
            this.Controls.Add(this.btnDescomprimir);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnDeleteChain);
            this.Controls.Add(this.btnExtraer);
            this.Controls.Add(this.treeSystem);
            this.Controls.Add(this.listFile);
            this.Controls.Add(this.btnSee);
            this.Controls.Add(this.btnHex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Sistema";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeSystem;
        private System.Windows.Forms.ImageList iconos;
        private System.Windows.Forms.ListView listFile;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.Button btnSee;
        private System.Windows.Forms.Button btnExtraer;
        private System.Windows.Forms.Button btnDeleteChain;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripInfoRom;
        private System.Windows.Forms.ToolStripButton toolStripDebug;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.Button btnDescomprimir;

    }
}

