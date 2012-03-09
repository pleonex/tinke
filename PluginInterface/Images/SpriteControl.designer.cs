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
            this.pictureBgd = new System.Windows.Forms.PictureBox();
            this.btnBgd = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblZoom = new System.Windows.Forms.Label();
            this.trackZoom = new System.Windows.Forms.TrackBar();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnSetTrans = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBgd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).BeginInit();
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
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "S01";
            // 
            // comboBank
            // 
            this.comboBank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBank.FormattingEnabled = true;
            this.comboBank.Location = new System.Drawing.Point(73, 263);
            this.comboBank.Name = "comboBank";
            this.comboBank.Size = new System.Drawing.Size(183, 21);
            this.comboBank.TabIndex = 3;
            this.comboBank.SelectedIndexChanged += new System.EventHandler(this.comboBank_SelectedIndexChanged);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Location = new System.Drawing.Point(73, 290);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(183, 23);
            this.btnShowAll.TabIndex = 4;
            this.btnShowAll.Text = "S02";
            this.btnShowAll.UseVisualStyleBackColor = true;
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
            this.label2.Location = new System.Drawing.Point(262, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "S15";
            // 
            // btnBgdTrans
            // 
            this.btnBgdTrans.Enabled = false;
            this.btnBgdTrans.Location = new System.Drawing.Point(176, 321);
            this.btnBgdTrans.Name = "btnBgdTrans";
            this.btnBgdTrans.Size = new System.Drawing.Size(80, 35);
            this.btnBgdTrans.TabIndex = 29;
            this.btnBgdTrans.Text = "S18";
            this.btnBgdTrans.UseVisualStyleBackColor = true;
            this.btnBgdTrans.Click += new System.EventHandler(this.btnBgdTrans_Click);
            // 
            // pictureBgd
            // 
            this.pictureBgd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBgd.Location = new System.Drawing.Point(135, 322);
            this.pictureBgd.Name = "pictureBgd";
            this.pictureBgd.Size = new System.Drawing.Size(35, 35);
            this.pictureBgd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBgd.TabIndex = 28;
            this.pictureBgd.TabStop = false;
            // 
            // btnBgd
            // 
            this.btnBgd.Location = new System.Drawing.Point(6, 322);
            this.btnBgd.Name = "btnBgd";
            this.btnBgd.Size = new System.Drawing.Size(118, 35);
            this.btnBgd.TabIndex = 27;
            this.btnBgd.Text = "S17";
            this.btnBgd.UseVisualStyleBackColor = true;
            this.btnBgd.Click += new System.EventHandler(this.btnBgd_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(463, 306);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "20";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(259, 307);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "1";
            // 
            // lblZoom
            // 
            this.lblZoom.AutoSize = true;
            this.lblZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZoom.Location = new System.Drawing.Point(345, 295);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(33, 17);
            this.lblZoom.TabIndex = 24;
            this.lblZoom.Text = "S16";
            // 
            // trackZoom
            // 
            this.trackZoom.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.trackZoom.LargeChange = 2;
            this.trackZoom.Location = new System.Drawing.Point(262, 322);
            this.trackZoom.Maximum = 20;
            this.trackZoom.Minimum = 1;
            this.trackZoom.Name = "trackZoom";
            this.trackZoom.Size = new System.Drawing.Size(226, 45);
            this.trackZoom.TabIndex = 23;
            this.trackZoom.Value = 1;
            this.trackZoom.Scroll += new System.EventHandler(this.trackZoom_Scroll);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(416, 463);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 40);
            this.btnImport.TabIndex = 30;
            this.btnImport.Text = "S24";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnSetTrans
            // 
            this.btnSetTrans.Location = new System.Drawing.Point(6, 363);
            this.btnSetTrans.Name = "btnSetTrans";
            this.btnSetTrans.Size = new System.Drawing.Size(118, 32);
            this.btnSetTrans.TabIndex = 31;
            this.btnSetTrans.Text = "S25";
            this.btnSetTrans.UseVisualStyleBackColor = true;
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
            // SpriteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.imgBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetTrans);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnBgdTrans);
            this.Controls.Add(this.pictureBgd);
            this.Controls.Add(this.btnBgd);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblZoom);
            this.Controls.Add(this.trackZoom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnShowAll);
            this.Controls.Add(this.comboBank);
            this.Controls.Add(this.label1);
            this.Name = "SpriteControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBgd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBgd;
        private System.Windows.Forms.Button btnBgd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.TrackBar trackZoom;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSetTrans;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
