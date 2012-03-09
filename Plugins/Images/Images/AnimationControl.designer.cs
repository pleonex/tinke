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
 * By: pleoNeX
 * 
 */
namespace Images
{
    partial class AnimationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationControl));
            this.aniBox = new System.Windows.Forms.PictureBox();
            this.tempo = new System.Windows.Forms.Timer(this.components);
            this.btnPlay = new System.Windows.Forms.Button();
            this.imageMedia = new System.Windows.Forms.ImageList(this.components);
            this.comboAni = new System.Windows.Forms.ComboBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkImage = new System.Windows.Forms.CheckBox();
            this.checkTransparencia = new System.Windows.Forms.CheckBox();
            this.checkNumeros = new System.Windows.Forms.CheckBox();
            this.checkCeldas = new System.Windows.Forms.CheckBox();
            this.checkEntorno = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtTime = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblFullImage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.aniBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // aniBox
            // 
            this.aniBox.Location = new System.Drawing.Point(0, 0);
            this.aniBox.Name = "aniBox";
            this.aniBox.Size = new System.Drawing.Size(512, 256);
            this.aniBox.TabIndex = 0;
            this.aniBox.TabStop = false;
            this.aniBox.DoubleClick += new System.EventHandler(this.aniBox_DoubleClick);
            // 
            // tempo
            // 
            this.tempo.Enabled = true;
            this.tempo.Tick += new System.EventHandler(this.tempo_Tick);
            // 
            // btnPlay
            // 
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlay.ImageIndex = 0;
            this.btnPlay.ImageList = this.imageMedia;
            this.btnPlay.Location = new System.Drawing.Point(104, 264);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(25, 23);
            this.btnPlay.TabIndex = 2;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // imageMedia
            // 
            this.imageMedia.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageMedia.ImageStream")));
            this.imageMedia.TransparentColor = System.Drawing.Color.Transparent;
            this.imageMedia.Images.SetKeyName(0, "resultset_next.png");
            this.imageMedia.Images.SetKeyName(1, "resultset_first.png");
            this.imageMedia.Images.SetKeyName(2, "resultset_last.png");
            this.imageMedia.Images.SetKeyName(3, "stop.png");
            // 
            // comboAni
            // 
            this.comboAni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAni.FormattingEnabled = true;
            this.comboAni.Location = new System.Drawing.Point(359, 360);
            this.comboAni.Name = "comboAni";
            this.comboAni.Size = new System.Drawing.Size(121, 21);
            this.comboAni.TabIndex = 3;
            this.comboAni.SelectedIndexChanged += new System.EventHandler(this.comboAni_SelectedIndexChanged);
            // 
            // btnNext
            // 
            this.btnNext.ImageIndex = 2;
            this.btnNext.ImageList = this.imageMedia;
            this.btnNext.Location = new System.Drawing.Point(135, 264);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(35, 23);
            this.btnNext.TabIndex = 4;
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.ImageIndex = 1;
            this.btnPrevious.ImageList = this.imageMedia;
            this.btnPrevious.Location = new System.Drawing.Point(32, 264);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(35, 23);
            this.btnPrevious.TabIndex = 5;
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.ImageIndex = 3;
            this.btnStop.ImageList = this.imageMedia;
            this.btnStop.Location = new System.Drawing.Point(73, 264);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(25, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkImage);
            this.groupBox2.Controls.Add(this.checkTransparencia);
            this.groupBox2.Controls.Add(this.checkNumeros);
            this.groupBox2.Controls.Add(this.checkCeldas);
            this.groupBox2.Controls.Add(this.checkEntorno);
            this.groupBox2.Location = new System.Drawing.Point(2, 344);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(258, 114);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "S03";
            // 
            // checkImage
            // 
            this.checkImage.AutoSize = true;
            this.checkImage.Checked = true;
            this.checkImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkImage.Location = new System.Drawing.Point(7, 66);
            this.checkImage.Name = "checkImage";
            this.checkImage.Size = new System.Drawing.Size(45, 17);
            this.checkImage.TabIndex = 4;
            this.checkImage.Text = "S06";
            this.checkImage.UseVisualStyleBackColor = true;
            this.checkImage.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkTransparencia
            // 
            this.checkTransparencia.AutoSize = true;
            this.checkTransparencia.Location = new System.Drawing.Point(141, 20);
            this.checkTransparencia.Name = "checkTransparencia";
            this.checkTransparencia.Size = new System.Drawing.Size(45, 17);
            this.checkTransparencia.TabIndex = 3;
            this.checkTransparencia.Text = "S07";
            this.checkTransparencia.UseVisualStyleBackColor = true;
            this.checkTransparencia.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkNumeros
            // 
            this.checkNumeros.AutoSize = true;
            this.checkNumeros.Location = new System.Drawing.Point(141, 43);
            this.checkNumeros.Name = "checkNumeros";
            this.checkNumeros.Size = new System.Drawing.Size(45, 17);
            this.checkNumeros.TabIndex = 2;
            this.checkNumeros.Text = "S08";
            this.checkNumeros.UseVisualStyleBackColor = true;
            this.checkNumeros.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkCeldas
            // 
            this.checkCeldas.AutoSize = true;
            this.checkCeldas.Location = new System.Drawing.Point(7, 43);
            this.checkCeldas.Name = "checkCeldas";
            this.checkCeldas.Size = new System.Drawing.Size(45, 17);
            this.checkCeldas.TabIndex = 1;
            this.checkCeldas.Text = "S05";
            this.checkCeldas.UseVisualStyleBackColor = true;
            this.checkCeldas.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // checkEntorno
            // 
            this.checkEntorno.AutoSize = true;
            this.checkEntorno.Location = new System.Drawing.Point(7, 20);
            this.checkEntorno.Name = "checkEntorno";
            this.checkEntorno.Size = new System.Drawing.Size(45, 17);
            this.checkEntorno.TabIndex = 0;
            this.checkEntorno.Text = "S04";
            this.checkEntorno.UseVisualStyleBackColor = true;
            this.checkEntorno.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(4, 473);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "S1C";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtTime
            // 
            this.txtTime.BeepOnError = true;
            this.txtTime.Location = new System.Drawing.Point(76, 304);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(37, 20);
            this.txtTime.TabIndex = 9;
            this.txtTime.Text = "150";
            this.txtTime.TextChanged += new System.EventHandler(this.txtTime_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 307);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "S02";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 306);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "S01";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 364);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "S11";
            // 
            // lblFullImage
            // 
            this.lblFullImage.AutoSize = true;
            this.lblFullImage.Location = new System.Drawing.Point(110, 478);
            this.lblFullImage.Name = "lblFullImage";
            this.lblFullImage.Size = new System.Drawing.Size(27, 13);
            this.lblFullImage.TabIndex = 13;
            this.lblFullImage.Text = "S1E";
            // 
            // AnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblFullImage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.comboAni);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.aniBox);
            this.Name = "AnimationControl";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.aniBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox aniBox;
        private System.Windows.Forms.Timer tempo;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.ComboBox comboAni;
        private System.Windows.Forms.ImageList imageMedia;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkImage;
        private System.Windows.Forms.CheckBox checkTransparencia;
        private System.Windows.Forms.CheckBox checkNumeros;
        private System.Windows.Forms.CheckBox checkCeldas;
        private System.Windows.Forms.CheckBox checkEntorno;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.MaskedTextBox txtTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFullImage;
    }
}
