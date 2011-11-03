namespace NINOKUNI
{
    partial class BMPControl
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
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.picBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Image = global::NINOKUNI.Properties.Resources.picture_edit;
            this.btnImport.Location = new System.Drawing.Point(323, 478);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 30);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "S01";
            this.btnImport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::NINOKUNI.Properties.Resources.picture_save;
            this.btnSave.Location = new System.Drawing.Point(419, 478);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "S00";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // picBox
            // 
            this.picBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBox.Location = new System.Drawing.Point(4, 4);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(256, 256);
            this.picBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBox.TabIndex = 0;
            this.picBox.TabStop = false;
            // 
            // BMPControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picBox);
            this.Name = "BMPControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBox;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnImport;
    }
}
