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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("ID");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Formato");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Offset");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Tamaño");
            this.treeFiles = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.listProp = new System.Windows.Forms.ListView();
            this.columnCampo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnExtract = new System.Windows.Forms.Button();
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
            listViewItem4});
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
            this.btnExtract.Size = new System.Drawing.Size(82, 35);
            this.btnExtract.TabIndex = 2;
            this.btnExtract.Text = "Extraer";
            this.btnExtract.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // iSDAT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
    }
}
