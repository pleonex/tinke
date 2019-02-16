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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
 * 
 */

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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("S09");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("S0A");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("S0B");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("S0C");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("S0D");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("S0E");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("S40");
            this.iconos = new System.Windows.Forms.ImageList(this.components);
            this.btnExtract = new System.Windows.Forms.Button();
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
            this.toolStripLanguage = new System.Windows.Forms.ToolStripSplitButton();
            this.stripRefreshMsg = new System.Windows.Forms.ToolStripButton();
            this.btnUnpack = new System.Windows.Forms.Button();
            this.btnDesplazar = new System.Windows.Forms.Button();
            this.panelObj = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripOpenAs = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuComprimido = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripAbrirFat = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripAbrirTexto = new System.Windows.Forms.ToolStripMenuItem();
            this.callPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linkAboutBox = new System.Windows.Forms.LinkLabel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.checkSearch = new System.Windows.Forms.CheckBox();
            this.toolTipSearch = new System.Windows.Forms.ToolTip(this.components);
            this.lblSupport = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSaveROM = new System.Windows.Forms.Button();
            this.btnPack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
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
            this.iconos.Images.SetKeyName(7, "folder_go.png");
            this.iconos.Images.SetKeyName(8, "pictures.png");
            this.iconos.Images.SetKeyName(9, "picture_link.png");
            this.iconos.Images.SetKeyName(10, "photo.png");
            this.iconos.Images.SetKeyName(11, "picture_save.png");
            this.iconos.Images.SetKeyName(12, "picture_delete.png");
            this.iconos.Images.SetKeyName(13, "film.png");
            this.iconos.Images.SetKeyName(14, "music.png");
            this.iconos.Images.SetKeyName(15, "picture_go.png");
            this.iconos.Images.SetKeyName(16, "font.png");
            this.iconos.Images.SetKeyName(17, "script.png");
            this.iconos.Images.SetKeyName(18, "folder_add.png");
            this.iconos.Images.SetKeyName(19, "disk.png");
            this.iconos.Images.SetKeyName(20, "page_gear.png");
            this.iconos.Images.SetKeyName(21, "image.png");
            this.iconos.Images.SetKeyName(22, "map.png");
            this.iconos.Images.SetKeyName(23, "package_go.png");
            this.iconos.Images.SetKeyName(24, "package_add.png");
            // 
            // btnExtract
            // 
            this.btnExtract.ImageIndex = 7;
            this.btnExtract.ImageList = this.iconos;
            this.btnExtract.Location = new System.Drawing.Point(409, 409);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(110, 40);
            this.btnExtract.TabIndex = 4;
            this.btnExtract.Text = "S1B";
            this.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtraer_Click);
            // 
            // btnSee
            // 
            this.btnSee.Enabled = false;
            this.btnSee.Image = global::Tinke.Properties.Resources.zoom;
            this.btnSee.Location = new System.Drawing.Point(409, 501);
            this.btnSee.Name = "btnSee";
            this.btnSee.Size = new System.Drawing.Size(110, 40);
            this.btnSee.TabIndex = 2;
            this.btnSee.Text = "S1C";
            this.btnSee.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSee.UseVisualStyleBackColor = true;
            this.btnSee.Click += new System.EventHandler(this.BtnSee);
            // 
            // btnHex
            // 
            this.btnHex.Enabled = false;
            this.btnHex.Image = global::Tinke.Properties.Resources.calculator;
            this.btnHex.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHex.Location = new System.Drawing.Point(525, 501);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(110, 40);
            this.btnHex.TabIndex = 1;
            this.btnHex.Text = "S1D";
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
            listViewItem6,
            listViewItem7});
            this.listFile.Location = new System.Drawing.Point(409, 28);
            this.listFile.Name = "listFile";
            this.listFile.Size = new System.Drawing.Size(197, 169);
            this.listFile.TabIndex = 0;
            this.listFile.UseCompatibleStateImageBehavior = false;
            this.listFile.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "S07";
            this.columnHeader1.Width = 96;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "S08";
            this.columnHeader2.Width = 95;
            // 
            // treeSystem
            // 
            this.treeSystem.HideSelection = false;
            this.treeSystem.ImageIndex = 0;
            this.treeSystem.ImageList = this.iconos;
            this.treeSystem.Location = new System.Drawing.Point(0, 28);
            this.treeSystem.Name = "treeSystem";
            this.treeSystem.SelectedImageIndex = 0;
            this.treeSystem.Size = new System.Drawing.Size(403, 520);
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
            this.toolStripLanguage,
            this.stripRefreshMsg});
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
            this.toolStripOpen.Size = new System.Drawing.Size(45, 22);
            this.toolStripOpen.Text = "S01";
            this.toolStripOpen.Click += new System.EventHandler(this.toolStripOpen_Click);
            // 
            // toolStripInfoRom
            // 
            this.toolStripInfoRom.CheckOnClick = true;
            this.toolStripInfoRom.Image = ((System.Drawing.Image)(resources.GetObject("toolStripInfoRom.Image")));
            this.toolStripInfoRom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInfoRom.Name = "toolStripInfoRom";
            this.toolStripInfoRom.Size = new System.Drawing.Size(45, 22);
            this.toolStripInfoRom.Text = "S02";
            this.toolStripInfoRom.Click += new System.EventHandler(this.toolStripInfoRom_Click);
            // 
            // toolStripDebug
            // 
            this.toolStripDebug.CheckOnClick = true;
            this.toolStripDebug.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDebug.Image")));
            this.toolStripDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDebug.Name = "toolStripDebug";
            this.toolStripDebug.Size = new System.Drawing.Size(45, 22);
            this.toolStripDebug.Text = "S03";
            this.toolStripDebug.Click += new System.EventHandler(this.toolStripDebug_Click);
            // 
            // toolStripVentana
            // 
            this.toolStripVentana.CheckOnClick = true;
            this.toolStripVentana.Image = ((System.Drawing.Image)(resources.GetObject("toolStripVentana.Image")));
            this.toolStripVentana.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripVentana.Name = "toolStripVentana";
            this.toolStripVentana.Size = new System.Drawing.Size(45, 22);
            this.toolStripVentana.Text = "S04";
            this.toolStripVentana.Click += new System.EventHandler(this.toolStripVentana_Click);
            // 
            // toolStripLanguage
            // 
            this.toolStripLanguage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLanguage.Image")));
            this.toolStripLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLanguage.Name = "toolStripLanguage";
            this.toolStripLanguage.Size = new System.Drawing.Size(57, 22);
            this.toolStripLanguage.Text = "S1E";
            // 
            // stripRefreshMsg
            // 
            this.stripRefreshMsg.Image = ((System.Drawing.Image)(resources.GetObject("stripRefreshMsg.Image")));
            this.stripRefreshMsg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stripRefreshMsg.Name = "stripRefreshMsg";
            this.stripRefreshMsg.Size = new System.Drawing.Size(45, 22);
            this.stripRefreshMsg.Text = "S45";
            this.stripRefreshMsg.Click += new System.EventHandler(this.stripRefreshMsg_Click);
            // 
            // btnUnpack
            // 
            this.btnUnpack.ImageIndex = 23;
            this.btnUnpack.ImageList = this.iconos;
            this.btnUnpack.Location = new System.Drawing.Point(409, 455);
            this.btnUnpack.Name = "btnUnpack";
            this.btnUnpack.Size = new System.Drawing.Size(110, 40);
            this.btnUnpack.TabIndex = 7;
            this.btnUnpack.Text = "S1A";
            this.btnUnpack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnpack.UseVisualStyleBackColor = true;
            this.btnUnpack.Click += new System.EventHandler(this.btnUnpack_Click);
            // 
            // btnDesplazar
            // 
            this.btnDesplazar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDesplazar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesplazar.Location = new System.Drawing.Point(612, 28);
            this.btnDesplazar.Name = "btnDesplazar";
            this.btnDesplazar.Size = new System.Drawing.Size(30, 150);
            this.btnDesplazar.TabIndex = 9;
            this.btnDesplazar.Text = ">>>>>";
            this.btnDesplazar.UseVisualStyleBackColor = false;
            this.btnDesplazar.Click += new System.EventHandler(this.btnDesplazar_Click);
            // 
            // panelObj
            // 
            this.panelObj.AutoScroll = true;
            this.panelObj.BackColor = System.Drawing.Color.Transparent;
            this.panelObj.Location = new System.Drawing.Point(649, 25);
            this.panelObj.Name = "panelObj";
            this.panelObj.Size = new System.Drawing.Size(515, 515);
            this.panelObj.TabIndex = 10;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpenAs});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.toolStrip2.Location = new System.Drawing.Point(409, 319);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip2.Size = new System.Drawing.Size(55, 23);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripOpenAs
            // 
            this.toolStripOpenAs.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripOpenAs.BackColor = System.Drawing.Color.Transparent;
            this.toolStripOpenAs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuComprimido,
            this.toolStripAbrirFat,
            this.toolStripAbrirTexto,
            this.callPluginToolStripMenuItem});
            this.toolStripOpenAs.Image = global::Tinke.Properties.Resources.zoom;
            this.toolStripOpenAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenAs.Name = "toolStripOpenAs";
            this.toolStripOpenAs.Size = new System.Drawing.Size(54, 20);
            this.toolStripOpenAs.Text = "S16";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Text = "S17";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolAbrirComoItemPaleta_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "S18";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolAbrirComoItemTile_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem3.Image")));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem3.Text = "S19";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolAbrirComoItemScreen_Click);
            // 
            // toolStripMenuComprimido
            // 
            this.toolStripMenuComprimido.Image = global::Tinke.Properties.Resources.compress;
            this.toolStripMenuComprimido.Name = "toolStripMenuComprimido";
            this.toolStripMenuComprimido.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuComprimido.Text = "S2A";
            this.toolStripMenuComprimido.Click += new System.EventHandler(this.s2AToolStripMenuItem_Click);
            // 
            // toolStripAbrirFat
            // 
            this.toolStripAbrirFat.Image = global::Tinke.Properties.Resources.package;
            this.toolStripAbrirFat.Name = "toolStripAbrirFat";
            this.toolStripAbrirFat.Size = new System.Drawing.Size(152, 22);
            this.toolStripAbrirFat.Text = "S3D";
            this.toolStripAbrirFat.Click += new System.EventHandler(this.toolStripAbrirFat_Click);
            // 
            // toolStripAbrirTexto
            // 
            this.toolStripAbrirTexto.Image = global::Tinke.Properties.Resources.page_white_text;
            this.toolStripAbrirTexto.Name = "toolStripAbrirTexto";
            this.toolStripAbrirTexto.Size = new System.Drawing.Size(152, 22);
            this.toolStripAbrirTexto.Text = "S26";
            this.toolStripAbrirTexto.Click += new System.EventHandler(this.toolStripAbrirTexto_Click);
            // 
            // callPluginToolStripMenuItem
            // 
            this.callPluginToolStripMenuItem.Image = global::Tinke.Properties.Resources.plugin_go;
            this.callPluginToolStripMenuItem.Name = "callPluginToolStripMenuItem";
            this.callPluginToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.callPluginToolStripMenuItem.Text = "Call plugin";
            this.callPluginToolStripMenuItem.Click += new System.EventHandler(this.callPluginToolStripMenuItem_Click);
            // 
            // linkAboutBox
            // 
            this.linkAboutBox.AutoSize = true;
            this.linkAboutBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkAboutBox.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkAboutBox.Location = new System.Drawing.Point(406, 200);
            this.linkAboutBox.Name = "linkAboutBox";
            this.linkAboutBox.Size = new System.Drawing.Size(29, 13);
            this.linkAboutBox.TabIndex = 13;
            this.linkAboutBox.TabStop = true;
            this.linkAboutBox.Text = "S0F";
            this.linkAboutBox.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAboutBox_LinkClicked);
            // 
            // txtSearch
            // 
            this.txtSearch.ForeColor = System.Drawing.Color.Black;
            this.txtSearch.Location = new System.Drawing.Point(409, 239);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(183, 20);
            this.txtSearch.TabIndex = 14;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = global::Tinke.Properties.Resources.zoom;
            this.btnSearch.Location = new System.Drawing.Point(598, 236);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(43, 23);
            this.btnSearch.TabIndex = 15;
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // checkSearch
            // 
            this.checkSearch.AutoSize = true;
            this.checkSearch.Location = new System.Drawing.Point(409, 265);
            this.checkSearch.Name = "checkSearch";
            this.checkSearch.Size = new System.Drawing.Size(46, 17);
            this.checkSearch.TabIndex = 16;
            this.checkSearch.Text = "S2E";
            this.checkSearch.UseVisualStyleBackColor = true;
            // 
            // toolTipSearch
            // 
            this.toolTipSearch.AutoPopDelay = 10000;
            this.toolTipSearch.InitialDelay = 500;
            this.toolTipSearch.IsBalloon = true;
            this.toolTipSearch.ReshowDelay = 100;
            this.toolTipSearch.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // lblSupport
            // 
            this.lblSupport.AutoSize = true;
            this.lblSupport.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSupport.Location = new System.Drawing.Point(409, 289);
            this.lblSupport.Name = "lblSupport";
            this.lblSupport.Size = new System.Drawing.Size(32, 17);
            this.lblSupport.TabIndex = 17;
            this.lblSupport.Text = "S30";
            // 
            // btnImport
            // 
            this.btnImport.ImageIndex = 18;
            this.btnImport.ImageList = this.iconos;
            this.btnImport.Location = new System.Drawing.Point(525, 409);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(110, 40);
            this.btnImport.TabIndex = 18;
            this.btnImport.Text = "S32";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSaveROM
            // 
            this.btnSaveROM.ImageIndex = 19;
            this.btnSaveROM.ImageList = this.iconos;
            this.btnSaveROM.Location = new System.Drawing.Point(409, 363);
            this.btnSaveROM.Name = "btnSaveROM";
            this.btnSaveROM.Size = new System.Drawing.Size(110, 40);
            this.btnSaveROM.TabIndex = 19;
            this.btnSaveROM.Text = "S33";
            this.btnSaveROM.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveROM.UseVisualStyleBackColor = true;
            this.btnSaveROM.Click += new System.EventHandler(this.btnSaveROM_Click);
            // 
            // btnPack
            // 
            this.btnPack.ImageIndex = 24;
            this.btnPack.ImageList = this.iconos;
            this.btnPack.Location = new System.Drawing.Point(525, 455);
            this.btnPack.Name = "btnPack";
            this.btnPack.Size = new System.Drawing.Size(110, 40);
            this.btnPack.TabIndex = 20;
            this.btnPack.Text = "S42";
            this.btnPack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPack.UseVisualStyleBackColor = true;
            this.btnPack.Click += new System.EventHandler(this.btnPack_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(409, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "S2D";
            // 
            // Sistema
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(644, 547);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPack);
            this.Controls.Add(this.btnSaveROM);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblSupport);
            this.Controls.Add(this.checkSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.linkAboutBox);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.panelObj);
            this.Controls.Add(this.btnDesplazar);
            this.Controls.Add(this.btnUnpack);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.treeSystem);
            this.Controls.Add(this.listFile);
            this.Controls.Add(this.btnSee);
            this.Controls.Add(this.btnHex);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Sistema";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Sistema_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Sistema_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Sistema_KeyUp);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
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
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripInfoRom;
        private System.Windows.Forms.ToolStripButton toolStripDebug;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.Button btnUnpack;
        private System.Windows.Forms.ToolStripButton toolStripVentana;
        private System.Windows.Forms.Button btnDesplazar;
        private System.Windows.Forms.Panel panelObj;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSplitButton toolStripLanguage;
        private System.Windows.Forms.LinkLabel linkAboutBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripOpenAs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox checkSearch;
        private System.Windows.Forms.ToolTip toolTipSearch;
        private System.Windows.Forms.Label lblSupport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSaveROM;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuComprimido;
        private System.Windows.Forms.ToolStripMenuItem toolStripAbrirFat;
        private System.Windows.Forms.ToolStripMenuItem toolStripAbrirTexto;
        private System.Windows.Forms.Button btnPack;
        private System.Windows.Forms.ToolStripButton stripRefreshMsg;
        private System.Windows.Forms.ToolStripMenuItem callPluginToolStripMenuItem;
        private System.Windows.Forms.Label label1;

    }
}

