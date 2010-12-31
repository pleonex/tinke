namespace LAYTON
{
    partial class InfoPicture
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
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblNImgs = new System.Windows.Forms.Label();
            this.txtNImgs = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabImags = new System.Windows.Forms.TabControl();
            this.groupImage = new System.Windows.Forms.GroupBox();
            this.txtTipo = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(6, 22);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(31, 13);
            this.lblTipo.TabIndex = 40;
            this.lblTipo.Text = "Tipo:";
            // 
            // lblNImgs
            // 
            this.lblNImgs.AutoSize = true;
            this.lblNImgs.Location = new System.Drawing.Point(167, 22);
            this.lblNImgs.Name = "lblNImgs";
            this.lblNImgs.Size = new System.Drawing.Size(61, 13);
            this.lblNImgs.TabIndex = 41;
            this.lblNImgs.Text = "Nº de imgs:";
            // 
            // txtNImgs
            // 
            this.txtNImgs.Location = new System.Drawing.Point(234, 19);
            this.txtNImgs.Name = "txtNImgs";
            this.txtNImgs.ReadOnly = true;
            this.txtNImgs.Size = new System.Drawing.Size(100, 20);
            this.txtNImgs.TabIndex = 42;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabImags);
            this.groupBox1.Location = new System.Drawing.Point(6, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(373, 245);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Información de las imágenes";
            // 
            // tabImags
            // 
            this.tabImags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabImags.Location = new System.Drawing.Point(3, 16);
            this.tabImags.Name = "tabImags";
            this.tabImags.SelectedIndex = 0;
            this.tabImags.Size = new System.Drawing.Size(367, 226);
            this.tabImags.TabIndex = 0;
            // 
            // groupImage
            // 
            this.groupImage.Controls.Add(this.txtTipo);
            this.groupImage.Controls.Add(this.groupBox1);
            this.groupImage.Controls.Add(this.txtNImgs);
            this.groupImage.Controls.Add(this.lblNImgs);
            this.groupImage.Controls.Add(this.lblTipo);
            this.groupImage.Location = new System.Drawing.Point(0, 0);
            this.groupImage.Name = "groupImage";
            this.groupImage.Size = new System.Drawing.Size(385, 292);
            this.groupImage.TabIndex = 41;
            this.groupImage.TabStop = false;
            this.groupImage.Text = "Información del archivo:";
            // 
            // txtTipo
            // 
            this.txtTipo.Location = new System.Drawing.Point(43, 19);
            this.txtTipo.Name = "txtTipo";
            this.txtTipo.ReadOnly = true;
            this.txtTipo.Size = new System.Drawing.Size(100, 20);
            this.txtTipo.TabIndex = 44;
            // 
            // InfoPicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupImage);
            this.Name = "InfoPicture";
            this.Size = new System.Drawing.Size(385, 292);
            this.groupBox1.ResumeLayout(false);
            this.groupImage.ResumeLayout(false);
            this.groupImage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label lblNImgs;
        private System.Windows.Forms.TextBox txtNImgs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabImags;
        private System.Windows.Forms.GroupBox groupImage;
        private System.Windows.Forms.TextBox txtTipo;

    }
}
