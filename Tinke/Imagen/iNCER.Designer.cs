namespace Tinke
{
    partial class iNCER
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
            "Sección"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Número de celdas");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Tipo de celdas");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Constante");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Tamaño bloque");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Desconocido 1");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Desconocido 2");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Desconocido (Sección UEXT)");
            this.imgBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listProp = new System.Windows.Forms.ListView();
            this.columnCampo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnValor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.comboCelda = new System.Windows.Forms.ComboBox();
            this.btnTodos = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.checkEntorno = new System.Windows.Forms.CheckBox();
            this.checkNumber = new System.Windows.Forms.CheckBox();
            this.checkCelda = new System.Windows.Forms.CheckBox();
            this.checkTransparencia = new System.Windows.Forms.CheckBox();
            this.checkImagen = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgBox
            // 
            this.imgBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(256, 256);
            this.imgBox.TabIndex = 0;
            this.imgBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listProp);
            this.groupBox1.Location = new System.Drawing.Point(262, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(247, 256);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Propiedades";
            // 
            // listProp
            // 
            this.listProp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnCampo,
            this.columnValor});
            this.listProp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProp.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8});
            this.listProp.Location = new System.Drawing.Point(3, 16);
            this.listProp.Name = "listProp";
            this.listProp.Size = new System.Drawing.Size(241, 237);
            this.listProp.TabIndex = 0;
            this.listProp.UseCompatibleStateImageBehavior = false;
            this.listProp.View = System.Windows.Forms.View.Details;
            // 
            // columnCampo
            // 
            this.columnCampo.Text = "Campo";
            this.columnCampo.Width = 151;
            // 
            // columnValor
            // 
            this.columnValor.Text = "Valor";
            this.columnValor.Width = 86;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Celda:";
            // 
            // comboCelda
            // 
            this.comboCelda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCelda.FormattingEnabled = true;
            this.comboCelda.Location = new System.Drawing.Point(46, 263);
            this.comboCelda.Name = "comboCelda";
            this.comboCelda.Size = new System.Drawing.Size(210, 21);
            this.comboCelda.TabIndex = 3;
            this.comboCelda.SelectedIndexChanged += new System.EventHandler(this.comboCelda_SelectedIndexChanged);
            // 
            // btnTodos
            // 
            this.btnTodos.Location = new System.Drawing.Point(46, 290);
            this.btnTodos.Name = "btnTodos";
            this.btnTodos.Size = new System.Drawing.Size(210, 23);
            this.btnTodos.TabIndex = 4;
            this.btnTodos.Text = "Mostrar todas las celdas";
            this.btnTodos.UseVisualStyleBackColor = true;
            this.btnTodos.Click += new System.EventHandler(this.btnTodos_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Tinke.Properties.Resources.picture_save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(6, 483);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 26);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Guardar";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkEntorno
            // 
            this.checkEntorno.AutoSize = true;
            this.checkEntorno.Location = new System.Drawing.Point(265, 259);
            this.checkEntorno.Name = "checkEntorno";
            this.checkEntorno.Size = new System.Drawing.Size(100, 17);
            this.checkEntorno.TabIndex = 6;
            this.checkEntorno.Text = "Mostrar entorno";
            this.checkEntorno.UseVisualStyleBackColor = true;
            this.checkEntorno.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkNumber
            // 
            this.checkNumber.AutoSize = true;
            this.checkNumber.Location = new System.Drawing.Point(405, 282);
            this.checkNumber.Name = "checkNumber";
            this.checkNumber.Size = new System.Drawing.Size(104, 17);
            this.checkNumber.TabIndex = 7;
            this.checkNumber.Text = "Mostrar números";
            this.checkNumber.UseVisualStyleBackColor = true;
            this.checkNumber.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkCelda
            // 
            this.checkCelda.AutoSize = true;
            this.checkCelda.Location = new System.Drawing.Point(265, 282);
            this.checkCelda.Name = "checkCelda";
            this.checkCelda.Size = new System.Drawing.Size(95, 17);
            this.checkCelda.TabIndex = 8;
            this.checkCelda.Text = "Mostrar celdas";
            this.checkCelda.UseVisualStyleBackColor = true;
            this.checkCelda.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkTransparencia
            // 
            this.checkTransparencia.AutoSize = true;
            this.checkTransparencia.Location = new System.Drawing.Point(405, 259);
            this.checkTransparencia.Name = "checkTransparencia";
            this.checkTransparencia.Size = new System.Drawing.Size(94, 17);
            this.checkTransparencia.TabIndex = 9;
            this.checkTransparencia.Text = "Transparencia";
            this.checkTransparencia.UseVisualStyleBackColor = true;
            this.checkTransparencia.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkImagen
            // 
            this.checkImagen.AutoSize = true;
            this.checkImagen.Checked = true;
            this.checkImagen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkImagen.Location = new System.Drawing.Point(265, 306);
            this.checkImagen.Name = "checkImagen";
            this.checkImagen.Size = new System.Drawing.Size(98, 17);
            this.checkImagen.TabIndex = 10;
            this.checkImagen.Text = "Mostrar imagen";
            this.checkImagen.UseVisualStyleBackColor = true;
            this.checkImagen.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // iNCER
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.checkImagen);
            this.Controls.Add(this.checkTransparencia);
            this.Controls.Add(this.checkCelda);
            this.Controls.Add(this.checkNumber);
            this.Controls.Add(this.checkEntorno);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnTodos);
            this.Controls.Add(this.comboCelda);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imgBox);
            this.Name = "iNCER";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listProp;
        private System.Windows.Forms.ColumnHeader columnCampo;
        private System.Windows.Forms.ColumnHeader columnValor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboCelda;
        private System.Windows.Forms.Button btnTodos;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox checkEntorno;
        private System.Windows.Forms.CheckBox checkNumber;
        private System.Windows.Forms.CheckBox checkCelda;
        private System.Windows.Forms.CheckBox checkTransparencia;
        private System.Windows.Forms.CheckBox checkImagen;
    }
}
