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
            this.toolStripVentana = new System.Windows.Forms.ToolStripButton();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.recargarPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liberarPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargarPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDescomprimir = new System.Windows.Forms.Button();
            this.btnDesplazar = new System.Windows.Forms.Button();
            this.panelObj = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton2 = new System.Windows.Forms.ToolStripSplitButton();
            this.borrarPaletaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarCeldasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripAbrirComo = new System.Windows.Forms.ToolStripSplitButton();
            this.toolAbrirComoItemPaleta = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAbrirComoItemTile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAbrirComoItemScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconos
            // 
            this.iconos.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconos.ImageStream")));
            this.iconos.TransparentColor = System.Drawing.Color.Transparent;
            this.iconos.Images.SetKeyName(0, "folder.png");
            this.iconos.Images.SetKeyName(1, "page_white.png");
            this.iconos.Images.SetKeyName(2, "palette.png");
            this.iconos.Images.SetKeyName(3, "picture.png");
            this.iconos.Images.SetKeyName(4, "page_white_text.png");
            this.iconos.Images.SetKeyName(5, "compress.png");
            this.iconos.Images.SetKeyName(6, "package.png");
            this.iconos.Images.SetKeyName(7, "package_go.png");
            this.iconos.Images.SetKeyName(8, "pictures.png");
            this.iconos.Images.SetKeyName(9, "picture_link.png");
            this.iconos.Images.SetKeyName(10, "photo.png");
            this.iconos.Images.SetKeyName(11, "picture_save.png");
            this.iconos.Images.SetKeyName(12, "picture_delete.png");
            this.iconos.Images.SetKeyName(13, "film.png");
            this.iconos.Images.SetKeyName(14, "music.png");
            this.iconos.Images.SetKeyName(15, "picture_go.png");
            // 
            // btnExtraer
            // 
            this.btnExtraer.ImageKey = "package_go.png";
            this.btnExtraer.ImageList = this.iconos;
            this.btnExtraer.Location = new System.Drawing.Point(515, 455);
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
            this.btnSee.Location = new System.Drawing.Point(409, 493);
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
            this.btnHex.Location = new System.Drawing.Point(514, 493);
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
            this.treeSystem.Size = new System.Drawing.Size(403, 508);
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
            this.toolStripDebug,
            this.toolStripVentana,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(644, 25);
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
            this.toolStripInfoRom.Checked = true;
            this.toolStripInfoRom.CheckOnClick = true;
            this.toolStripInfoRom.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // toolStripVentana
            // 
            this.toolStripVentana.CheckOnClick = true;
            this.toolStripVentana.Image = ((System.Drawing.Image)(resources.GetObject("toolStripVentana.Image")));
            this.toolStripVentana.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripVentana.Name = "toolStripVentana";
            this.toolStripVentana.Size = new System.Drawing.Size(104, 22);
            this.toolStripVentana.Text = "Modo ventana";
            this.toolStripVentana.Click += new System.EventHandler(this.toolStripVentana_Click);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recargarPluginsToolStripMenuItem,
            this.liberarPluginsToolStripMenuItem,
            this.cargarPluginsToolStripMenuItem});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(75, 22);
            this.toolStripSplitButton1.Text = "Plugins";
            // 
            // recargarPluginsToolStripMenuItem
            // 
            this.recargarPluginsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recargarPluginsToolStripMenuItem.Image")));
            this.recargarPluginsToolStripMenuItem.Name = "recargarPluginsToolStripMenuItem";
            this.recargarPluginsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.recargarPluginsToolStripMenuItem.Text = "Recargar plugins";
            this.recargarPluginsToolStripMenuItem.Click += new System.EventHandler(this.recargarPluginsToolStripMenuItem_Click);
            // 
            // liberarPluginsToolStripMenuItem
            // 
            this.liberarPluginsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("liberarPluginsToolStripMenuItem.Image")));
            this.liberarPluginsToolStripMenuItem.Name = "liberarPluginsToolStripMenuItem";
            this.liberarPluginsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.liberarPluginsToolStripMenuItem.Text = "Liberar plugins";
            this.liberarPluginsToolStripMenuItem.Click += new System.EventHandler(this.liberarPluginsToolStripMenuItem_Click);
            // 
            // cargarPluginsToolStripMenuItem
            // 
            this.cargarPluginsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cargarPluginsToolStripMenuItem.Image")));
            this.cargarPluginsToolStripMenuItem.Name = "cargarPluginsToolStripMenuItem";
            this.cargarPluginsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.cargarPluginsToolStripMenuItem.Text = "Cargar plugins";
            this.cargarPluginsToolStripMenuItem.Click += new System.EventHandler(this.cargarPluginsToolStripMenuItem_Click);
            // 
            // btnDescomprimir
            // 
            this.btnDescomprimir.ImageKey = "compress.png";
            this.btnDescomprimir.ImageList = this.iconos;
            this.btnDescomprimir.Location = new System.Drawing.Point(409, 455);
            this.btnDescomprimir.Name = "btnDescomprimir";
            this.btnDescomprimir.Size = new System.Drawing.Size(97, 32);
            this.btnDescomprimir.TabIndex = 7;
            this.btnDescomprimir.Text = "Descomprimir";
            this.btnDescomprimir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDescomprimir.UseVisualStyleBackColor = true;
            this.btnDescomprimir.Click += new System.EventHandler(this.btnUncompress_Click);
            // 
            // btnDesplazar
            // 
            this.btnDesplazar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDesplazar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesplazar.Location = new System.Drawing.Point(612, 80);
            this.btnDesplazar.Name = "btnDesplazar";
            this.btnDesplazar.Size = new System.Drawing.Size(30, 150);
            this.btnDesplazar.TabIndex = 9;
            this.btnDesplazar.Text = ">>>>>";
            this.btnDesplazar.UseVisualStyleBackColor = false;
            this.btnDesplazar.Click += new System.EventHandler(this.btnDesplazar_Click);
            // 
            // panelObj
            // 
            this.panelObj.BackColor = System.Drawing.Color.Transparent;
            this.panelObj.Location = new System.Drawing.Point(649, 25);
            this.panelObj.Name = "panelObj";
            this.panelObj.Size = new System.Drawing.Size(512, 512);
            this.panelObj.TabIndex = 10;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton2});
            this.toolStrip2.Location = new System.Drawing.Point(409, 427);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip2.Size = new System.Drawing.Size(115, 25);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripSplitButton2
            // 
            this.toolStripSplitButton2.BackColor = System.Drawing.SystemColors.Highlight;
            this.toolStripSplitButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.borrarPaletaToolStripMenuItem,
            this.borrarTileToolStripMenuItem,
            this.borrarScreenToolStripMenuItem,
            this.borrarCeldasToolStripMenuItem});
            this.toolStripSplitButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton2.Image")));
            this.toolStripSplitButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton2.Name = "toolStripSplitButton2";
            this.toolStripSplitButton2.Size = new System.Drawing.Size(112, 22);
            this.toolStripSplitButton2.Text = "Borrar cadena";
            this.toolStripSplitButton2.ButtonClick += new System.EventHandler(this.toolStripSplitButton2_ButtonClick);
            // 
            // borrarPaletaToolStripMenuItem
            // 
            this.borrarPaletaToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarPaletaToolStripMenuItem.Image")));
            this.borrarPaletaToolStripMenuItem.Name = "borrarPaletaToolStripMenuItem";
            this.borrarPaletaToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.borrarPaletaToolStripMenuItem.Text = "Borrar paleta";
            this.borrarPaletaToolStripMenuItem.Click += new System.EventHandler(this.borrarPaletaToolStripMenuItem_Click);
            // 
            // borrarTileToolStripMenuItem
            // 
            this.borrarTileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarTileToolStripMenuItem.Image")));
            this.borrarTileToolStripMenuItem.Name = "borrarTileToolStripMenuItem";
            this.borrarTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.borrarTileToolStripMenuItem.Text = "Borrar tile";
            this.borrarTileToolStripMenuItem.Click += new System.EventHandler(this.borrarTileToolStripMenuItem_Click);
            // 
            // borrarScreenToolStripMenuItem
            // 
            this.borrarScreenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarScreenToolStripMenuItem.Image")));
            this.borrarScreenToolStripMenuItem.Name = "borrarScreenToolStripMenuItem";
            this.borrarScreenToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.borrarScreenToolStripMenuItem.Text = "Borrar screen";
            this.borrarScreenToolStripMenuItem.Click += new System.EventHandler(this.borrarScreenToolStripMenuItem_Click);
            // 
            // borrarCeldasToolStripMenuItem
            // 
            this.borrarCeldasToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarCeldasToolStripMenuItem.Image")));
            this.borrarCeldasToolStripMenuItem.Name = "borrarCeldasToolStripMenuItem";
            this.borrarCeldasToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.borrarCeldasToolStripMenuItem.Text = "Borrar celdas";
            this.borrarCeldasToolStripMenuItem.Click += new System.EventHandler(this.borrarCeldasToolStripMenuItem_Click);
            // 
            // toolStrip3
            // 
            this.toolStrip3.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.toolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAbrirComo});
            this.toolStrip3.Location = new System.Drawing.Point(531, 427);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip3.Size = new System.Drawing.Size(111, 25);
            this.toolStrip3.TabIndex = 12;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // toolStripAbrirComo
            // 
            this.toolStripAbrirComo.BackColor = System.Drawing.SystemColors.Highlight;
            this.toolStripAbrirComo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAbrirComoItemPaleta,
            this.toolAbrirComoItemTile,
            this.toolAbrirComoItemScreen});
            this.toolStripAbrirComo.Image = global::Tinke.Properties.Resources.zoom;
            this.toolStripAbrirComo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAbrirComo.Name = "toolStripAbrirComo";
            this.toolStripAbrirComo.Size = new System.Drawing.Size(108, 22);
            this.toolStripAbrirComo.Text = "Abrir como...";
            // 
            // toolAbrirComoItemPaleta
            // 
            this.toolAbrirComoItemPaleta.Image = ((System.Drawing.Image)(resources.GetObject("toolAbrirComoItemPaleta.Image")));
            this.toolAbrirComoItemPaleta.Name = "toolAbrirComoItemPaleta";
            this.toolAbrirComoItemPaleta.Size = new System.Drawing.Size(109, 22);
            this.toolAbrirComoItemPaleta.Text = "Paleta";
            this.toolAbrirComoItemPaleta.Click += new System.EventHandler(this.toolAbrirComoItemPaleta_Click);
            // 
            // toolAbrirComoItemTile
            // 
            this.toolAbrirComoItemTile.Image = ((System.Drawing.Image)(resources.GetObject("toolAbrirComoItemTile.Image")));
            this.toolAbrirComoItemTile.Name = "toolAbrirComoItemTile";
            this.toolAbrirComoItemTile.Size = new System.Drawing.Size(109, 22);
            this.toolAbrirComoItemTile.Text = "Tile";
            this.toolAbrirComoItemTile.Click += new System.EventHandler(this.toolAbrirComoItemTile_Click);
            // 
            // toolAbrirComoItemScreen
            // 
            this.toolAbrirComoItemScreen.Image = ((System.Drawing.Image)(resources.GetObject("toolAbrirComoItemScreen.Image")));
            this.toolAbrirComoItemScreen.Name = "toolAbrirComoItemScreen";
            this.toolAbrirComoItemScreen.Size = new System.Drawing.Size(109, 22);
            this.toolAbrirComoItemScreen.Text = "Screen";
            this.toolAbrirComoItemScreen.Click += new System.EventHandler(this.toolAbrirComoItemScreen_Click);
            // 
            // Sistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(644, 537);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.panelObj);
            this.Controls.Add(this.btnDesplazar);
            this.Controls.Add(this.btnDescomprimir);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnExtraer);
            this.Controls.Add(this.treeSystem);
            this.Controls.Add(this.listFile);
            this.Controls.Add(this.btnSee);
            this.Controls.Add(this.btnHex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Sistema";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.ToolStripMenuItem toolAbrirComoItemScreen;
        private System.Windows.Forms.ToolStripMenuItem toolAbrirComoItemTile;
        private System.Windows.Forms.ToolStripMenuItem toolAbrirComoItemPaleta;
        private System.Windows.Forms.ToolStripSplitButton toolStripAbrirComo;
        private System.Windows.Forms.ToolStrip toolStrip3;

        #endregion

        private System.Windows.Forms.TreeView treeSystem;
        private System.Windows.Forms.ImageList iconos;
        private System.Windows.Forms.ListView listFile;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.Button btnSee;
        private System.Windows.Forms.Button btnExtraer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripInfoRom;
        private System.Windows.Forms.ToolStripButton toolStripDebug;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.Button btnDescomprimir;
        private System.Windows.Forms.ToolStripButton toolStripVentana;
        private System.Windows.Forms.Button btnDesplazar;
        private System.Windows.Forms.Panel panelObj;
        private System.Windows.Forms.ToolStripDropDownButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem recargarPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liberarPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargarPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton2;
        private System.Windows.Forms.ToolStripMenuItem borrarPaletaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarTileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarCeldasToolStripMenuItem;

    }
}

