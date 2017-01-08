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
 * 
 */
namespace LAYTON
{
    partial class InfoImage
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPartes = new System.Windows.Forms.TabControl();
            this.lblNPartes = new System.Windows.Forms.Label();
            this.txtNPartes = new System.Windows.Forms.TextBox();
            this.lblTamanoImg = new System.Windows.Forms.Label();
            this.txtTamanoImg = new System.Windows.Forms.TextBox();
            this.txtImgs = new System.Windows.Forms.TextBox();
            this.lblImgs = new System.Windows.Forms.Label();
            this.lblAncho = new System.Windows.Forms.Label();
            this.txtAlto = new System.Windows.Forms.TextBox();
            this.lblAlto = new System.Windows.Forms.Label();
            this.txtAncho = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabPartes);
            this.groupBox1.Location = new System.Drawing.Point(4, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 122);
            this.groupBox1.TabIndex = 45;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "S07";
            // 
            // tabPartes
            // 
            this.tabPartes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPartes.Location = new System.Drawing.Point(3, 16);
            this.tabPartes.Name = "tabPartes";
            this.tabPartes.SelectedIndex = 0;
            this.tabPartes.Size = new System.Drawing.Size(345, 103);
            this.tabPartes.TabIndex = 35;
            // 
            // lblNPartes
            // 
            this.lblNPartes.AutoSize = true;
            this.lblNPartes.Location = new System.Drawing.Point(1, 58);
            this.lblNPartes.Name = "lblNPartes";
            this.lblNPartes.Size = new System.Drawing.Size(26, 13);
            this.lblNPartes.TabIndex = 37;
            this.lblNPartes.Text = "S05";
            // 
            // txtNPartes
            // 
            this.txtNPartes.Location = new System.Drawing.Point(100, 55);
            this.txtNPartes.Name = "txtNPartes";
            this.txtNPartes.ReadOnly = true;
            this.txtNPartes.Size = new System.Drawing.Size(60, 20);
            this.txtNPartes.TabIndex = 38;
            // 
            // lblTamanoImg
            // 
            this.lblTamanoImg.AutoSize = true;
            this.lblTamanoImg.Location = new System.Drawing.Point(166, 6);
            this.lblTamanoImg.Name = "lblTamanoImg";
            this.lblTamanoImg.Size = new System.Drawing.Size(26, 13);
            this.lblTamanoImg.TabIndex = 47;
            this.lblTamanoImg.Text = "S02";
            // 
            // txtTamanoImg
            // 
            this.txtTamanoImg.Location = new System.Drawing.Point(280, 3);
            this.txtTamanoImg.Name = "txtTamanoImg";
            this.txtTamanoImg.ReadOnly = true;
            this.txtTamanoImg.Size = new System.Drawing.Size(60, 20);
            this.txtTamanoImg.TabIndex = 46;
            // 
            // txtImgs
            // 
            this.txtImgs.Location = new System.Drawing.Point(100, 3);
            this.txtImgs.Name = "txtImgs";
            this.txtImgs.ReadOnly = true;
            this.txtImgs.Size = new System.Drawing.Size(60, 20);
            this.txtImgs.TabIndex = 40;
            // 
            // lblImgs
            // 
            this.lblImgs.AutoSize = true;
            this.lblImgs.Location = new System.Drawing.Point(1, 6);
            this.lblImgs.Name = "lblImgs";
            this.lblImgs.Size = new System.Drawing.Size(26, 13);
            this.lblImgs.TabIndex = 39;
            this.lblImgs.Text = "S01";
            // 
            // lblAncho
            // 
            this.lblAncho.AutoSize = true;
            this.lblAncho.Location = new System.Drawing.Point(1, 32);
            this.lblAncho.Name = "lblAncho";
            this.lblAncho.Size = new System.Drawing.Size(26, 13);
            this.lblAncho.TabIndex = 41;
            this.lblAncho.Text = "S03";
            // 
            // txtAlto
            // 
            this.txtAlto.Location = new System.Drawing.Point(280, 29);
            this.txtAlto.Name = "txtAlto";
            this.txtAlto.ReadOnly = true;
            this.txtAlto.Size = new System.Drawing.Size(60, 20);
            this.txtAlto.TabIndex = 43;
            // 
            // lblAlto
            // 
            this.lblAlto.AutoSize = true;
            this.lblAlto.Location = new System.Drawing.Point(166, 32);
            this.lblAlto.Name = "lblAlto";
            this.lblAlto.Size = new System.Drawing.Size(26, 13);
            this.lblAlto.TabIndex = 44;
            this.lblAlto.Text = "S04";
            // 
            // txtAncho
            // 
            this.txtAncho.Location = new System.Drawing.Point(100, 29);
            this.txtAncho.Name = "txtAncho";
            this.txtAncho.ReadOnly = true;
            this.txtAncho.Size = new System.Drawing.Size(60, 20);
            this.txtAncho.TabIndex = 42;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(166, 58);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(26, 13);
            this.lblName.TabIndex = 48;
            this.lblName.Text = "S06";
            this.lblName.Visible = false;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(280, 55);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(60, 20);
            this.txtName.TabIndex = 49;
            this.txtName.Visible = false;
            // 
            // InfoImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblNPartes);
            this.Controls.Add(this.txtNPartes);
            this.Controls.Add(this.lblTamanoImg);
            this.Controls.Add(this.txtTamanoImg);
            this.Controls.Add(this.txtImgs);
            this.Controls.Add(this.lblImgs);
            this.Controls.Add(this.lblAncho);
            this.Controls.Add(this.txtAlto);
            this.Controls.Add(this.lblAlto);
            this.Controls.Add(this.txtAncho);
            this.Name = "InfoImage";
            this.Size = new System.Drawing.Size(365, 205);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabPartes;
        private System.Windows.Forms.Label lblNPartes;
        private System.Windows.Forms.TextBox txtNPartes;
        private System.Windows.Forms.Label lblTamanoImg;
        private System.Windows.Forms.TextBox txtTamanoImg;
        private System.Windows.Forms.TextBox txtImgs;
        private System.Windows.Forms.Label lblImgs;
        private System.Windows.Forms.Label lblAncho;
        private System.Windows.Forms.TextBox txtAlto;
        private System.Windows.Forms.Label lblAlto;
        private System.Windows.Forms.TextBox txtAncho;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
    }
}
