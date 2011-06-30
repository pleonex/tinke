﻿/*
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(iSDAT));
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("ID");
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("Formato");
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem("Offset");
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem("Tamaño");
            this.treeFiles = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listProp = new System.Windows.Forms.ListView();
            this.columnCampo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnExtract = new System.Windows.Forms.Button();
            this.btnReproducir = new System.Windows.Forms.Button();
            this.btnWav = new System.Windows.Forms.Button();
            this.btnMidi = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeFiles
            // 
            this.treeFiles.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeFiles.ImageIndex = 2;
            this.treeFiles.ImageList = this.imageList;
            this.treeFiles.Location = new System.Drawing.Point(0, 0);
            this.treeFiles.Name = "treeFiles";
            this.treeFiles.SelectedImageIndex = 0;
            this.treeFiles.Size = new System.Drawing.Size(245, 510);
            this.treeFiles.TabIndex = 0;
            this.treeFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFiles_AfterSelect);
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
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCampo,
            this.columnValor});
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20});
            this.listProp.Location = new System.Drawing.Point(251, 3);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(256, 104);
            this.listProp.TabIndex = 1;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnCampo
            // 
            this.columnCampo.Text = "Campo";
            this.columnCampo.Width = 118;
            // 
            // columnValor
            // 
            this.columnValor.Text = "Valor";
            this.columnValor.Width = 121;
            // 
            // btnExtract
            // 
            this.btnExtract.Enabled = false;
            this.btnExtract.ImageIndex = 3;
            this.btnExtract.ImageList = this.imageList;
            this.btnExtract.Location = new System.Drawing.Point(251, 472);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(93, 35);
            this.btnExtract.TabIndex = 2;
            this.btnExtract.Text = "Extraer";
            this.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // btnReproducir
            // 
            this.btnReproducir.Enabled = false;
            this.btnReproducir.ImageIndex = 5;
            this.btnReproducir.ImageList = this.imageList;
            this.btnReproducir.Location = new System.Drawing.Point(357, 432);
            this.btnReproducir.Name = "btnReproducir";
            this.btnReproducir.Size = new System.Drawing.Size(45, 34);
            this.btnReproducir.TabIndex = 3;
            this.btnReproducir.UseVisualStyleBackColor = true;
            this.btnReproducir.Click += new System.EventHandler(this.btnReproducir_Click);
            // 
            // btnWav
            // 
            this.btnWav.Enabled = false;
            this.btnWav.ImageIndex = 5;
            this.btnWav.Location = new System.Drawing.Point(415, 432);
            this.btnWav.Name = "btnWav";
            this.btnWav.Size = new System.Drawing.Size(92, 34);
            this.btnWav.TabIndex = 4;
            this.btnWav.Text = "Guardar WAV";
            this.btnWav.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWav.UseVisualStyleBackColor = true;
            this.btnWav.Click += new System.EventHandler(this.btnWav_Click);
            // 
            // btnMidi
            // 
            this.btnMidi.Enabled = false;
            this.btnMidi.Location = new System.Drawing.Point(415, 472);
            this.btnMidi.Name = "btnMidi";
            this.btnMidi.Size = new System.Drawing.Size(92, 35);
            this.btnMidi.TabIndex = 5;
            this.btnMidi.Text = "Guardar MIDI";
            this.btnMidi.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.ImageIndex = 6;
            this.btnStop.ImageList = this.imageList;
            this.btnStop.Location = new System.Drawing.Point(357, 472);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(45, 34);
            this.btnStop.TabIndex = 6;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // iSDAT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
    }
}