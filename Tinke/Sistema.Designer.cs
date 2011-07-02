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
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem("S09");
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem("S0A");
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem("S0B");
            System.Windows.Forms.ListViewItem listViewItem22 = new System.Windows.Forms.ListViewItem("S0C");
            System.Windows.Forms.ListViewItem listViewItem23 = new System.Windows.Forms.ListViewItem("S0D");
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem("S0E");
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
            this.toolStripPlugin = new System.Windows.Forms.ToolStripDropDownButton();
            this.recargarPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripLanguage = new System.Windows.Forms.ToolStripSplitButton();
            this.btnDescomprimir = new System.Windows.Forms.Button();
            this.btnDesplazar = new System.Windows.Forms.Button();
            this.panelObj = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripDeleteChain = new System.Windows.Forms.ToolStripDropDownButton();
            this.borrarPaletaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarCeldasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.borrarAnimaciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.s10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripOpenAs = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.linkAboutBox = new System.Windows.Forms.LinkLabel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.checkSearch = new System.Windows.Forms.CheckBox();
            this.toolTipSearch = new System.Windows.Forms.ToolTip(this.components);
            this.lblSupport = new System.Windows.Forms.Label();
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
            this.iconos.Images.SetKeyName(7, "package_go.png");
            this.iconos.Images.SetKeyName(8, "pictures.png");
            this.iconos.Images.SetKeyName(9, "picture_link.png");
            this.iconos.Images.SetKeyName(10, "photo.png");
            this.iconos.Images.SetKeyName(11, "picture_save.png");
            this.iconos.Images.SetKeyName(12, "picture_delete.png");
            this.iconos.Images.SetKeyName(13, "film.png");
            this.iconos.Images.SetKeyName(14, "music.png");
            this.iconos.Images.SetKeyName(15, "picture_go.png");
            this.iconos.Images.SetKeyName(16, "font.png");
            // 
            // btnExtraer
            // 
            this.btnExtraer.ImageKey = "package_go.png";
            this.btnExtraer.ImageList = this.iconos;
            this.btnExtraer.Location = new System.Drawing.Point(525, 455);
            this.btnExtraer.Name = "btnExtraer";
            this.btnExtraer.Size = new System.Drawing.Size(110, 32);
            this.btnExtraer.TabIndex = 4;
            this.btnExtraer.Text = "S1B";
            this.btnExtraer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtraer.UseVisualStyleBackColor = true;
            this.btnExtraer.Click += new System.EventHandler(this.btnExtraer_Click);
            // 
            // btnSee
            // 
            this.btnSee.Enabled = false;
            this.btnSee.Image = global::Tinke.Properties.Resources.zoom;
            this.btnSee.Location = new System.Drawing.Point(409, 493);
            this.btnSee.Name = "btnSee";
            this.btnSee.Size = new System.Drawing.Size(110, 32);
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
            this.btnHex.Location = new System.Drawing.Point(525, 493);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(110, 32);
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
            listViewItem19,
            listViewItem20,
            listViewItem21,
            listViewItem22,
            listViewItem23,
            listViewItem24});
            this.listFile.Location = new System.Drawing.Point(409, 28);
            this.listFile.Name = "listFile";
            this.listFile.Size = new System.Drawing.Size(197, 202);
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
            this.toolStripPlugin,
            this.toolStripLanguage});
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
            this.toolStripDebug.Checked = true;
            this.toolStripDebug.CheckOnClick = true;
            this.toolStripDebug.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // toolStripPlugin
            // 
            this.toolStripPlugin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recargarPluginsToolStripMenuItem});
            this.toolStripPlugin.Image = ((System.Drawing.Image)(resources.GetObject("toolStripPlugin.Image")));
            this.toolStripPlugin.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripPlugin.Name = "toolStripPlugin";
            this.toolStripPlugin.Size = new System.Drawing.Size(54, 22);
            this.toolStripPlugin.Text = "S05";
            // 
            // recargarPluginsToolStripMenuItem
            // 
            this.recargarPluginsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recargarPluginsToolStripMenuItem.Image")));
            this.recargarPluginsToolStripMenuItem.Name = "recargarPluginsToolStripMenuItem";
            this.recargarPluginsToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.recargarPluginsToolStripMenuItem.Text = "S06";
            this.recargarPluginsToolStripMenuItem.Click += new System.EventHandler(this.recargarPluginsToolStripMenuItem_Click);
            // 
            // toolStripLanguage
            // 
            this.toolStripLanguage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripLanguage.Image")));
            this.toolStripLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLanguage.Name = "toolStripLanguage";
            this.toolStripLanguage.Size = new System.Drawing.Size(57, 22);
            this.toolStripLanguage.Text = "S1E";
            // 
            // btnDescomprimir
            // 
            this.btnDescomprimir.ImageKey = "compress.png";
            this.btnDescomprimir.ImageList = this.iconos;
            this.btnDescomprimir.Location = new System.Drawing.Point(409, 455);
            this.btnDescomprimir.Name = "btnDescomprimir";
            this.btnDescomprimir.Size = new System.Drawing.Size(110, 32);
            this.btnDescomprimir.TabIndex = 7;
            this.btnDescomprimir.Text = "S1A";
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
            this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDeleteChain,
            this.toolStripOpenAs});
            this.toolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.toolStrip2.Location = new System.Drawing.Point(409, 385);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip2.Size = new System.Drawing.Size(55, 46);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripDeleteChain
            // 
            this.toolStripDeleteChain.BackColor = System.Drawing.Color.Transparent;
            this.toolStripDeleteChain.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.borrarPaletaToolStripMenuItem,
            this.borrarTileToolStripMenuItem,
            this.borrarScreenToolStripMenuItem,
            this.borrarCeldasToolStripMenuItem,
            this.borrarAnimaciónToolStripMenuItem,
            this.s10ToolStripMenuItem});
            this.toolStripDeleteChain.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDeleteChain.Image")));
            this.toolStripDeleteChain.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDeleteChain.Name = "toolStripDeleteChain";
            this.toolStripDeleteChain.Size = new System.Drawing.Size(54, 20);
            this.toolStripDeleteChain.Text = "S10";
            // 
            // borrarPaletaToolStripMenuItem
            // 
            this.borrarPaletaToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarPaletaToolStripMenuItem.Image")));
            this.borrarPaletaToolStripMenuItem.Name = "borrarPaletaToolStripMenuItem";
            this.borrarPaletaToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.borrarPaletaToolStripMenuItem.Text = "S11";
            this.borrarPaletaToolStripMenuItem.Click += new System.EventHandler(this.borrarPaletaToolStripMenuItem_Click);
            // 
            // borrarTileToolStripMenuItem
            // 
            this.borrarTileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarTileToolStripMenuItem.Image")));
            this.borrarTileToolStripMenuItem.Name = "borrarTileToolStripMenuItem";
            this.borrarTileToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.borrarTileToolStripMenuItem.Text = "S12";
            this.borrarTileToolStripMenuItem.Click += new System.EventHandler(this.borrarTileToolStripMenuItem_Click);
            // 
            // borrarScreenToolStripMenuItem
            // 
            this.borrarScreenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarScreenToolStripMenuItem.Image")));
            this.borrarScreenToolStripMenuItem.Name = "borrarScreenToolStripMenuItem";
            this.borrarScreenToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.borrarScreenToolStripMenuItem.Text = "S13";
            this.borrarScreenToolStripMenuItem.Click += new System.EventHandler(this.borrarScreenToolStripMenuItem_Click);
            // 
            // borrarCeldasToolStripMenuItem
            // 
            this.borrarCeldasToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarCeldasToolStripMenuItem.Image")));
            this.borrarCeldasToolStripMenuItem.Name = "borrarCeldasToolStripMenuItem";
            this.borrarCeldasToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.borrarCeldasToolStripMenuItem.Text = "S14";
            this.borrarCeldasToolStripMenuItem.Click += new System.EventHandler(this.borrarCeldasToolStripMenuItem_Click);
            // 
            // borrarAnimaciónToolStripMenuItem
            // 
            this.borrarAnimaciónToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("borrarAnimaciónToolStripMenuItem.Image")));
            this.borrarAnimaciónToolStripMenuItem.Name = "borrarAnimaciónToolStripMenuItem";
            this.borrarAnimaciónToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.borrarAnimaciónToolStripMenuItem.Text = "S15";
            this.borrarAnimaciónToolStripMenuItem.Click += new System.EventHandler(this.borrarAnimaciónToolStripMenuItem_Click);
            // 
            // s10ToolStripMenuItem
            // 
            this.s10ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("s10ToolStripMenuItem.Image")));
            this.s10ToolStripMenuItem.Name = "s10ToolStripMenuItem";
            this.s10ToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.s10ToolStripMenuItem.Text = "S10";
            this.s10ToolStripMenuItem.Click += new System.EventHandler(this.s10ToolStripMenuItem_Click);
            // 
            // toolStripOpenAs
            // 
            this.toolStripOpenAs.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripOpenAs.BackColor = System.Drawing.Color.Transparent;
            this.toolStripOpenAs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
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
            this.toolStripMenuItem1.Size = new System.Drawing.Size(92, 22);
            this.toolStripMenuItem1.Text = "S17";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolAbrirComoItemPaleta_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(92, 22);
            this.toolStripMenuItem2.Text = "S18";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolAbrirComoItemTile_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem3.Image")));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(92, 22);
            this.toolStripMenuItem3.Text = "S19";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolAbrirComoItemScreen_Click);
            // 
            // linkAboutBox
            // 
            this.linkAboutBox.AutoSize = true;
            this.linkAboutBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkAboutBox.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkAboutBox.Location = new System.Drawing.Point(409, 233);
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
            this.txtSearch.Location = new System.Drawing.Point(410, 279);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(183, 20);
            this.txtSearch.TabIndex = 14;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = global::Tinke.Properties.Resources.zoom;
            this.btnSearch.Location = new System.Drawing.Point(599, 277);
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
            this.checkSearch.Location = new System.Drawing.Point(410, 306);
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
            this.lblSupport.Location = new System.Drawing.Point(409, 349);
            this.lblSupport.Name = "lblSupport";
            this.lblSupport.Size = new System.Drawing.Size(32, 17);
            this.lblSupport.TabIndex = 17;
            this.lblSupport.Text = "S30";
            // 
            // Sistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(644, 537);
            this.Controls.Add(this.lblSupport);
            this.Controls.Add(this.checkSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.linkAboutBox);
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripInfoRom;
        private System.Windows.Forms.ToolStripButton toolStripDebug;
        private System.Windows.Forms.ToolStripButton toolStripOpen;
        private System.Windows.Forms.Button btnDescomprimir;
        private System.Windows.Forms.ToolStripButton toolStripVentana;
        private System.Windows.Forms.Button btnDesplazar;
        private System.Windows.Forms.Panel panelObj;
        private System.Windows.Forms.ToolStripDropDownButton toolStripPlugin;
        private System.Windows.Forms.ToolStripMenuItem recargarPluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSplitButton toolStripLanguage;
        private System.Windows.Forms.LinkLabel linkAboutBox;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDeleteChain;
        private System.Windows.Forms.ToolStripMenuItem borrarPaletaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarTileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarCeldasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem borrarAnimaciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripOpenAs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem s10ToolStripMenuItem;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox checkSearch;
        private System.Windows.Forms.ToolTip toolTipSearch;
        private System.Windows.Forms.Label lblSupport;

    }
}

