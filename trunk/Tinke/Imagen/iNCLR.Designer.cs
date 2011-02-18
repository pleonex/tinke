﻿namespace Tinke
{
    partial class iNCLR
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Número de paletas");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Profundidad del color");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Padding");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Nº de colores por paleta");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Tamaño de paleta");
            this.paletaBox = new System.Windows.Forms.PictureBox();
            this.nPaleta = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.groupProp = new System.Windows.Forms.GroupBox();
            this.listProp = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.paletaBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaleta)).BeginInit();
            this.groupProp.SuspendLayout();
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
            // 
            // nPaleta
            // 
            this.nPaleta.Location = new System.Drawing.Point(78, 166);
            this.nPaleta.Name = "nPaleta";
            this.nPaleta.Size = new System.Drawing.Size(82, 20);
            this.nPaleta.TabIndex = 2;
            this.nPaleta.ValueChanged += new System.EventHandler(this.nPaleta_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Nº de paleta:";
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(326, 166);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(180, 30);
            this.btnShow.TabIndex = 4;
            this.btnShow.Text = "Mostrar todas las paletas";
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
            this.groupProp.Text = "Propiedades";
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
            this.columnName.Text = "Campo";
            this.columnName.Width = 172;
            // 
            // columnValor
            // 
            this.columnValor.Text = "Valor";
            this.columnValor.Width = 82;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Tinke.Properties.Resources.picture_save;
            this.btnSave.Location = new System.Drawing.Point(245, 168);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Guardar";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // iNCLR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupProp);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nPaleta);
            this.Controls.Add(this.paletaBox);
            this.Name = "iNCLR";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.paletaBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPaleta)).EndInit();
            this.groupProp.ResumeLayout(false);
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
    }
}
