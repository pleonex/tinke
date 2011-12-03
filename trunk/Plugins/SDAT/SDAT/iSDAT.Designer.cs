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
 * Programa utilizado: Visual Studio 2010
 * Fecha: 24/06/2011
 * 
 */
namespace SDAT
{
    partial class iSDAT
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iSDAT));
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("S02");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("S03");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("S04");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("S05");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("S06");
            this.treeFiles = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listProp = new System.Windows.Forms.ListView();
            this.columnCampo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnWav = new System.Windows.Forms.Button();
            this.btnMidi = new System.Windows.Forms.Button();
            this.checkLoop = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnChangeFile = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnUncompress = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReproducir = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnInfoSect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeFiles
            // 
            this.treeFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeFiles.HideSelection = false;
            this.treeFiles.ImageIndex = 2;
            this.treeFiles.ImageList = this.imageList;
            this.treeFiles.Location = new System.Drawing.Point(0, 0);
            this.treeFiles.Name = "treeFiles";
            this.treeFiles.SelectedImageIndex = 0;
            this.treeFiles.Size = new System.Drawing.Size(245, 510);
            this.treeFiles.TabIndex = 0;
            this.treeFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFiles_AfterSelect);
            this.treeFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeFiles_MouseDoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "folder.png");
            this.imageList.Images.SetKeyName(1, "music.png");
            this.imageList.Images.SetKeyName(2, "sound.png");
            this.imageList.Images.SetKeyName(3, "package_go.png");
            this.imageList.Images.SetKeyName(4, "bell.png");
            this.imageList.Images.SetKeyName(5, "resultset_next.png");
            this.imageList.Images.SetKeyName(6, "stop.png");
            this.imageList.Images.SetKeyName(7, "package.png");
            this.imageList.Images.SetKeyName(8, "page_white.png");
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCampo,
            this.columnValor});
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.listProp.Location = new System.Drawing.Point(251, 3);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(256, 116);
            this.listProp.TabIndex = 1;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnCampo
            // 
            this.columnCampo.Text = "S00";
            this.columnCampo.Width = 118;
            // 
            // columnValor
            // 
            this.columnValor.Text = "S01";
            this.columnValor.Width = 121;
            // 
            // btnWav
            // 
            this.btnWav.Enabled = false;
            this.btnWav.ImageIndex = 5;
            this.btnWav.Location = new System.Drawing.Point(407, 472);
            this.btnWav.Name = "btnWav";
            this.btnWav.Size = new System.Drawing.Size(100, 34);
            this.btnWav.TabIndex = 4;
            this.btnWav.Text = "S0A";
            this.btnWav.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWav.UseVisualStyleBackColor = true;
            this.btnWav.Click += new System.EventHandler(this.btnWav_Click);
            // 
            // btnMidi
            // 
            this.btnMidi.Enabled = false;
            this.btnMidi.Location = new System.Drawing.Point(408, 349);
            this.btnMidi.Name = "btnMidi";
            this.btnMidi.Size = new System.Drawing.Size(99, 35);
            this.btnMidi.TabIndex = 5;
            this.btnMidi.Text = "S09";
            this.btnMidi.UseVisualStyleBackColor = true;
            this.btnMidi.Visible = false;
            // 
            // checkLoop
            // 
            this.checkLoop.AutoSize = true;
            this.checkLoop.Enabled = false;
            this.checkLoop.Location = new System.Drawing.Point(251, 327);
            this.checkLoop.Name = "checkLoop";
            this.checkLoop.Size = new System.Drawing.Size(45, 17);
            this.checkLoop.TabIndex = 8;
            this.checkLoop.Text = "S06";
            this.checkLoop.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(251, 472);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(100, 34);
            this.btnCreate.TabIndex = 10;
            this.btnCreate.Text = "S0C";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnChangeFile
            // 
            this.btnChangeFile.Enabled = false;
            this.btnChangeFile.Image = global::SDAT.Properties.Resources.package_add;
            this.btnChangeFile.Location = new System.Drawing.Point(251, 350);
            this.btnChangeFile.Name = "btnChangeFile";
            this.btnChangeFile.Size = new System.Drawing.Size(100, 34);
            this.btnChangeFile.TabIndex = 11;
            this.btnChangeFile.Text = "S0D";
            this.btnChangeFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnChangeFile.UseVisualStyleBackColor = true;
            this.btnChangeFile.Click += new System.EventHandler(this.btnChangeFile_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Enabled = false;
            this.btnInfo.Image = global::SDAT.Properties.Resources.information;
            this.btnInfo.Location = new System.Drawing.Point(408, 391);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(99, 34);
            this.btnInfo.TabIndex = 12;
            this.btnInfo.Text = "S0E";
            this.btnInfo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnImport
            // 
            this.btnImport.Enabled = false;
            this.btnImport.Image = global::SDAT.Properties.Resources.sound_add;
            this.btnImport.Location = new System.Drawing.Point(407, 431);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(99, 35);
            this.btnImport.TabIndex = 9;
            this.btnImport.Text = "S0B";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnUncompress
            // 
            this.btnUncompress.Enabled = false;
            this.btnUncompress.ImageKey = "package.png";
            this.btnUncompress.ImageList = this.imageList;
            this.btnUncompress.Location = new System.Drawing.Point(251, 391);
            this.btnUncompress.Name = "btnUncompress";
            this.btnUncompress.Size = new System.Drawing.Size(100, 34);
            this.btnUncompress.TabIndex = 7;
            this.btnUncompress.Text = "S07";
            this.btnUncompress.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUncompress.UseVisualStyleBackColor = true;
            this.btnUncompress.Click += new System.EventHandler(this.btnUncompress_Click);
            // 
            // btnStop
            // 
            this.btnStop.ImageIndex = 6;
            this.btnStop.ImageList = this.imageList;
            this.btnStop.Location = new System.Drawing.Point(357, 432);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(45, 34);
            this.btnStop.TabIndex = 6;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnReproducir
            // 
            this.btnReproducir.Enabled = false;
            this.btnReproducir.ImageIndex = 5;
            this.btnReproducir.ImageList = this.imageList;
            this.btnReproducir.Location = new System.Drawing.Point(357, 392);
            this.btnReproducir.Name = "btnReproducir";
            this.btnReproducir.Size = new System.Drawing.Size(45, 34);
            this.btnReproducir.TabIndex = 3;
            this.btnReproducir.UseVisualStyleBackColor = true;
            this.btnReproducir.Click += new System.EventHandler(this.btnReproducir_Click);
            // 
            // btnExtract
            // 
            this.btnExtract.Enabled = false;
            this.btnExtract.ImageIndex = 3;
            this.btnExtract.ImageList = this.imageList;
            this.btnExtract.Location = new System.Drawing.Point(251, 431);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(100, 35);
            this.btnExtract.TabIndex = 2;
            this.btnExtract.Text = "S08";
            this.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // btnInfoSect
            // 
            this.btnInfoSect.Image = global::SDAT.Properties.Resources.tag_blue_edit;
            this.btnInfoSect.Location = new System.Drawing.Point(251, 126);
            this.btnInfoSect.Name = "btnInfoSect";
            this.btnInfoSect.Size = new System.Drawing.Size(100, 34);
            this.btnInfoSect.TabIndex = 13;
            this.btnInfoSect.Text = "Info section";
            this.btnInfoSect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInfoSect.UseVisualStyleBackColor = true;
            this.btnInfoSect.Click += new System.EventHandler(this.btnInfoSect_Click);
            // 
            // iSDAT
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnInfoSect);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnChangeFile);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.checkLoop);
            this.Controls.Add(this.btnUncompress);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnMidi);
            this.Controls.Add(this.btnWav);
            this.Controls.Add(this.btnReproducir);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.listProp);
            this.Controls.Add(this.treeFiles);
            this.Name = "iSDAT";
            this.Size = new System.Drawing.Size(510, 510);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeFiles;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ListView listProp;
        private System.Windows.Forms.ColumnHeader columnCampo;
        private System.Windows.Forms.ColumnHeader columnValor;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnReproducir;
        private System.Windows.Forms.Button btnWav;
        private System.Windows.Forms.Button btnMidi;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnUncompress;
        private System.Windows.Forms.CheckBox checkLoop;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnChangeFile;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnInfoSect;
    }
}
