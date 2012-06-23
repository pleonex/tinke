// ----------------------------------------------------------------------
// <copyright file="SpriteControl.designer.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/04/2012 14:29:12</date>
// -----------------------------------------------------------------------
namespace PluginInterface.Images
{
    partial class SpriteControl
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
            this.imgBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBank = new System.Windows.Forms.ComboBox();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.checkEntorno = new System.Windows.Forms.CheckBox();
            this.checkNumber = new System.Windows.Forms.CheckBox();
            this.checkCelda = new System.Windows.Forms.CheckBox();
            this.checkTransparencia = new System.Windows.Forms.CheckBox();
            this.checkImagen = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBgdTrans = new System.Windows.Forms.Button();
            this.btnBgd = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSetTrans = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkPalette = new System.Windows.Forms.CheckBox();
            this.btnOAMeditor = new System.Windows.Forms.Button();
            this.checkBatch = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBatch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgBox
            // 
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(512, 256);
            this.imgBox.TabIndex = 0;
            this.imgBox.TabStop = false;
            this.imgBox.DoubleClick += new System.EventHandler(this.imgBox_DoubleClick);
            this.imgBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.imgBox_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S01";
            // 
            // comboBank
            // 
            this.comboBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBank.FormattingEnabled = true;
            this.comboBank.Location = new System.Drawing.Point(70, 304);
            this.comboBank.Name = "comboBank";
            this.comboBank.Size = new System.Drawing.Size(183, 21);
            this.comboBank.TabIndex = 3;
            this.comboBank.SelectedIndexChanged += new System.EventHandler(this.comboBank_SelectedIndexChanged);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Enabled = false;
            this.btnShowAll.Location = new System.Drawing.Point(70, 331);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(183, 23);
            this.btnShowAll.TabIndex = 4;
            this.btnShowAll.Text = "S02";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Visible = false;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // btnSave
            // 
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(320, 463);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 40);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "S03";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // checkEntorno
            // 
            this.checkEntorno.AutoSize = true;
            this.checkEntorno.Location = new System.Drawing.Point(6, 22);
            this.checkEntorno.Name = "checkEntorno";
            this.checkEntorno.Size = new System.Drawing.Size(45, 17);
            this.checkEntorno.TabIndex = 6;
            this.checkEntorno.Text = "S0F";
            this.checkEntorno.UseVisualStyleBackColor = true;
            this.checkEntorno.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkNumber
            // 
            this.checkNumber.AutoSize = true;
            this.checkNumber.Location = new System.Drawing.Point(146, 45);
            this.checkNumber.Name = "checkNumber";
            this.checkNumber.Size = new System.Drawing.Size(45, 17);
            this.checkNumber.TabIndex = 7;
            this.checkNumber.Text = "S13";
            this.checkNumber.UseVisualStyleBackColor = true;
            this.checkNumber.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkCelda
            // 
            this.checkCelda.AutoSize = true;
            this.checkCelda.Location = new System.Drawing.Point(6, 45);
            this.checkCelda.Name = "checkCelda";
            this.checkCelda.Size = new System.Drawing.Size(45, 17);
            this.checkCelda.TabIndex = 8;
            this.checkCelda.Text = "S10";
            this.checkCelda.UseVisualStyleBackColor = true;
            this.checkCelda.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkTransparencia
            // 
            this.checkTransparencia.AutoSize = true;
            this.checkTransparencia.Checked = true;
            this.checkTransparencia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTransparencia.Location = new System.Drawing.Point(146, 22);
            this.checkTransparencia.Name = "checkTransparencia";
            this.checkTransparencia.Size = new System.Drawing.Size(45, 17);
            this.checkTransparencia.TabIndex = 9;
            this.checkTransparencia.Text = "S12";
            this.checkTransparencia.UseVisualStyleBackColor = true;
            this.checkTransparencia.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkImagen
            // 
            this.checkImagen.AutoSize = true;
            this.checkImagen.Checked = true;
            this.checkImagen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkImagen.Location = new System.Drawing.Point(6, 69);
            this.checkImagen.Name = "checkImagen";
            this.checkImagen.Size = new System.Drawing.Size(45, 17);
            this.checkImagen.TabIndex = 10;
            this.checkImagen.Text = "S11";
            this.checkImagen.UseVisualStyleBackColor = true;
            this.checkImagen.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 259);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "S15";
            // 
            // btnBgdTrans
            // 
            this.btnBgdTrans.Location = new System.Drawing.Point(89, 363);
            this.btnBgdTrans.Name = "btnBgdTrans";
            this.btnBgdTrans.Size = new System.Drawing.Size(80, 40);
            this.btnBgdTrans.TabIndex = 29;
            this.btnBgdTrans.Text = "S18";
            this.btnBgdTrans.UseVisualStyleBackColor = true;
            this.btnBgdTrans.Click += new System.EventHandler(this.btnBgdTrans_Click);
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(3, 363);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(80, 40);
            this.btnBgd.TabIndex = 27;
            this.btnBgd.Text = "S17";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(416, 463);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 40);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "S24";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSetTrans
            // 
            this.btnSetTrans.Location = new System.Drawing.Point(175, 363);
            this.btnSetTrans.Name = "btnSetTrans";
            this.btnSetTrans.Size = new System.Drawing.Size(80, 40);
            this.btnSetTrans.TabIndex = 31;
            this.btnSetTrans.Text = "S25";
            this.btnSetTrans.UseVisualStyleBackColor = true;
            this.btnSetTrans.Click += new System.EventHandler(this.btnSetTrans_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkCelda);
            this.groupBox1.Controls.Add(this.checkEntorno);
            this.groupBox1.Controls.Add(this.checkNumber);
            this.groupBox1.Controls.Add(this.checkTransparencia);
            this.groupBox1.Controls.Add(this.checkImagen);
            this.groupBox1.Location = new System.Drawing.Point(6, 409);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 100);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // checkPalette
            // 
            this.checkPalette.AutoSize = true;
            this.checkPalette.Checked = true;
            this.checkPalette.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkPalette.Location = new System.Drawing.Point(416, 440);
            this.checkPalette.Name = "checkPalette";
            this.checkPalette.Size = new System.Drawing.Size(86, 17);
            this.checkPalette.TabIndex = 34;
            this.checkPalette.Text = "Save palette";
            this.checkPalette.UseVisualStyleBackColor = true;
            this.checkPalette.CheckedChanged += new System.EventHandler(this.checkPalette_CheckedChanged);
            // 
            // btnOAMeditor
            // 
            this.btnOAMeditor.Location = new System.Drawing.Point(320, 417);
            this.btnOAMeditor.Name = "btnOAMeditor";
            this.btnOAMeditor.Size = new System.Drawing.Size(90, 40);
            this.btnOAMeditor.TabIndex = 35;
            this.btnOAMeditor.Text = "OAM Editor";
            this.btnOAMeditor.UseVisualStyleBackColor = true;
            this.btnOAMeditor.Click += new System.EventHandler(this.btnOAMeditor_Click);
            // 
            // checkBatch
            // 
            this.checkBatch.AutoSize = true;
            this.checkBatch.Location = new System.Drawing.Point(329, 319);
            this.checkBatch.Name = "checkBatch";
            this.checkBatch.Size = new System.Drawing.Size(155, 17);
            this.checkBatch.TabIndex = 36;
            this.checkBatch.Text = "Export / Import batch mode";
            this.checkBatch.UseVisualStyleBackColor = true;
            this.checkBatch.CheckedChanged += new System.EventHandler(this.checkBatch_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(326, 258);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 26);
            this.label3.TabIndex = 37;
            this.label3.Text = "Batch name (without extension)\r\n%s will be replace by the bank ID";
            // 
            // txtBatch
            // 
            this.txtBatch.Enabled = false;
            this.txtBatch.Location = new System.Drawing.Point(329, 293);
            this.txtBatch.Name = "txtBatch";
            this.txtBatch.Size = new System.Drawing.Size(177, 20);
            this.txtBatch.TabIndex = 38;
            // 
            // SpriteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtBatch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBatch);
            this.Controls.Add(this.btnOAMeditor);
            this.Controls.Add(this.checkPalette);
            this.Controls.Add(this.imgBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetTrans);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnBgdTrans);
            this.Controls.Add(this.btnBgd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnShowAll);
            this.Controls.Add(this.comboBank);
            this.Controls.Add(this.label1);
            this.Name = "SpriteControl";
            this.Size = new System.Drawing.Size(514, 514);
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBank;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox checkEntorno;
        private System.Windows.Forms.CheckBox checkNumber;
        private System.Windows.Forms.CheckBox checkCelda;
        private System.Windows.Forms.CheckBox checkTransparencia;
        private System.Windows.Forms.CheckBox checkImagen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBgdTrans;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSetTrans;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkPalette;
        private System.Windows.Forms.Button btnOAMeditor;
        private System.Windows.Forms.CheckBox checkBatch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBatch;
    }
}
