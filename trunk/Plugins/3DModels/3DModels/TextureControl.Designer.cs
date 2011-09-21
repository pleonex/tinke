namespace _3DModels
{
    partial class TextureControl
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

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Section",
            "Texture Info"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Offset (hex)");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Repeat X");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Repeat Y");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Flip X");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Flip Y");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Width");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Height");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Format");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Color 0");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Transforms");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "Section",
            "Palette Info"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Offset (0x)");
            this.picTex = new System.Windows.Forms.PictureBox();
            this.picPalette = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listProp = new System.Windows.Forms.ListView();
            this.columnProperty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSetTransparent = new System.Windows.Forms.Button();
            this.listTextures = new System.Windows.Forms.ListBox();
            this.listPalettes = new System.Windows.Forms.ListBox();
            this.panelTex = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picTex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).BeginInit();
            this.panelTex.SuspendLayout();
            this.SuspendLayout();
            // 
            // picTex
            // 
            this.picTex.BackColor = System.Drawing.Color.Transparent;
            this.picTex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTex.Location = new System.Drawing.Point(0, 0);
            this.picTex.Name = "picTex";
            this.picTex.Size = new System.Drawing.Size(100, 100);
            this.picTex.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picTex.TabIndex = 0;
            this.picTex.TabStop = false;
            // 
            // picPalette
            // 
            this.picPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPalette.Location = new System.Drawing.Point(349, 275);
            this.picPalette.Name = "picPalette";
            this.picPalette.Size = new System.Drawing.Size(160, 160);
            this.picPalette.TabIndex = 1;
            this.picPalette.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Palette:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 259);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Texture:";
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProperty,
            this.columnValue});
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
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
            listViewItem11,
            listViewItem12,
            listViewItem13});
            this.listProp.Location = new System.Drawing.Point(0, 259);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(169, 251);
            this.listProp.TabIndex = 8;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnProperty
            // 
            this.columnProperty.Text = "Property";
            this.columnProperty.Width = 62;
            // 
            // columnValue
            // 
            this.columnValue.Text = "Value";
            this.columnValue.Width = 89;
            // 
            // btnSetTransparent
            // 
            this.btnSetTransparent.Location = new System.Drawing.Point(363, 441);
            this.btnSetTransparent.Name = "btnSetTransparent";
            this.btnSetTransparent.Size = new System.Drawing.Size(146, 30);
            this.btnSetTransparent.TabIndex = 9;
            this.btnSetTransparent.Text = "Set transparent color";
            this.btnSetTransparent.UseVisualStyleBackColor = true;
            this.btnSetTransparent.Click += new System.EventHandler(this.btnSetTransparent_Click);
            // 
            // listTextures
            // 
            this.listTextures.FormattingEnabled = true;
            this.listTextures.Location = new System.Drawing.Point(173, 275);
            this.listTextures.Name = "listTextures";
            this.listTextures.Size = new System.Drawing.Size(170, 108);
            this.listTextures.TabIndex = 11;
            // 
            // listPalettes
            // 
            this.listPalettes.FormattingEnabled = true;
            this.listPalettes.Location = new System.Drawing.Point(174, 401);
            this.listPalettes.Name = "listPalettes";
            this.listPalettes.Size = new System.Drawing.Size(169, 108);
            this.listPalettes.TabIndex = 12;
            // 
            // panelTex
            // 
            this.panelTex.BackColor = System.Drawing.Color.DarkGray;
            this.panelTex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTex.Controls.Add(this.picTex);
            this.panelTex.Location = new System.Drawing.Point(0, 0);
            this.panelTex.Name = "panelTex";
            this.panelTex.Size = new System.Drawing.Size(512, 256);
            this.panelTex.TabIndex = 13;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::_3DModels.Properties.Resources.disk;
            this.btnSave.Location = new System.Drawing.Point(408, 479);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(101, 30);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save texture";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(346, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Palette image";
            // 
            // TextureControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picPalette);
            this.Controls.Add(this.panelTex);
            this.Controls.Add(this.listProp);
            this.Controls.Add(this.listPalettes);
            this.Controls.Add(this.listTextures);
            this.Controls.Add(this.btnSetTransparent);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TextureControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picTex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).EndInit();
            this.panelTex.ResumeLayout(false);
            this.panelTex.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picTex;
        private System.Windows.Forms.PictureBox picPalette;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listProp;
        private System.Windows.Forms.ColumnHeader columnProperty;
        private System.Windows.Forms.ColumnHeader columnValue;
        private System.Windows.Forms.Button btnSetTransparent;
        private System.Windows.Forms.ListBox listTextures;
        private System.Windows.Forms.ListBox listPalettes;
        private System.Windows.Forms.Panel panelTex;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label3;
    }
}
