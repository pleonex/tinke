namespace SDAT
{
    partial class NewAudioOptions
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAudioOptions));
            this.comboEncoding = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.checkLoop = new System.Windows.Forms.CheckBox();
            this.groupLoop = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericLoopLength = new System.Windows.Forms.NumericUpDown();
            this.numericLoopOffset = new System.Windows.Forms.NumericUpDown();
            this.numericBlockLen = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericVolume = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.radioSam = new System.Windows.Forms.RadioButton();
            this.radioSec = new System.Windows.Forms.RadioButton();
            this.radioMSec = new System.Windows.Forms.RadioButton();
            this.groupLoop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLoopLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLoopOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlockLen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // comboEncoding
            // 
            this.comboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEncoding.FormattingEnabled = true;
            this.comboEncoding.Items.AddRange(new object[] {
            "PCM-8",
            "PCM-16",
            "IMA-ADPCM"});
            this.comboEncoding.Location = new System.Drawing.Point(16, 29);
            this.comboEncoding.Name = "comboEncoding";
            this.comboEncoding.Size = new System.Drawing.Size(130, 21);
            this.comboEncoding.TabIndex = 0;
            this.comboEncoding.SelectedIndexChanged += new System.EventHandler(this.comboEncoding_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "S01";
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Image = global::SDAT.Properties.Resources.accept;
            this.btnAccept.Location = new System.Drawing.Point(160, 275);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(90, 40);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "S08";
            this.btnAccept.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkLoop
            // 
            this.checkLoop.AutoSize = true;
            this.checkLoop.Checked = true;
            this.checkLoop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkLoop.Location = new System.Drawing.Point(16, 69);
            this.checkLoop.Name = "checkLoop";
            this.checkLoop.Size = new System.Drawing.Size(45, 17);
            this.checkLoop.TabIndex = 3;
            this.checkLoop.Text = "S02";
            this.checkLoop.UseVisualStyleBackColor = true;
            this.checkLoop.CheckedChanged += new System.EventHandler(this.checkLoop_CheckedChanged);
            // 
            // groupLoop
            // 
            this.groupLoop.Controls.Add(this.radioMSec);
            this.groupLoop.Controls.Add(this.radioSec);
            this.groupLoop.Controls.Add(this.radioSam);
            this.groupLoop.Controls.Add(this.label6);
            this.groupLoop.Controls.Add(this.label3);
            this.groupLoop.Controls.Add(this.label2);
            this.groupLoop.Controls.Add(this.numericLoopLength);
            this.groupLoop.Controls.Add(this.numericLoopOffset);
            this.groupLoop.Location = new System.Drawing.Point(16, 93);
            this.groupLoop.Name = "groupLoop";
            this.groupLoop.Size = new System.Drawing.Size(234, 124);
            this.groupLoop.TabIndex = 4;
            this.groupLoop.TabStop = false;
            this.groupLoop.Text = "S03";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "S05";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "S04";
            // 
            // numericLoopLength
            // 
            this.numericLoopLength.Hexadecimal = true;
            this.numericLoopLength.Location = new System.Drawing.Point(158, 98);
            this.numericLoopLength.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericLoopLength.Name = "numericLoopLength";
            this.numericLoopLength.Size = new System.Drawing.Size(70, 20);
            this.numericLoopLength.TabIndex = 1;
            this.numericLoopLength.ValueChanged += new System.EventHandler(this.numericLoopLength_ValueChanged);
            // 
            // numericLoopOffset
            // 
            this.numericLoopOffset.Location = new System.Drawing.Point(158, 72);
            this.numericLoopOffset.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericLoopOffset.Name = "numericLoopOffset";
            this.numericLoopOffset.Size = new System.Drawing.Size(70, 20);
            this.numericLoopOffset.TabIndex = 0;
            this.numericLoopOffset.ValueChanged += new System.EventHandler(this.numericLoopOffset_ValueChanged);
            // 
            // numericBlockLen
            // 
            this.numericBlockLen.Location = new System.Drawing.Point(174, 223);
            this.numericBlockLen.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numericBlockLen.Name = "numericBlockLen";
            this.numericBlockLen.Size = new System.Drawing.Size(70, 20);
            this.numericBlockLen.TabIndex = 5;
            this.numericBlockLen.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numericBlockLen.ValueChanged += new System.EventHandler(this.numericBlockLen_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "S06";
            // 
            // numericVolume
            // 
            this.numericVolume.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericVolume.Location = new System.Drawing.Point(174, 249);
            this.numericVolume.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericVolume.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericVolume.Name = "numericVolume";
            this.numericVolume.Size = new System.Drawing.Size(70, 20);
            this.numericVolume.TabIndex = 7;
            this.numericVolume.Visible = false;
            this.numericVolume.ValueChanged += new System.EventHandler(this.numericVolume_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 251);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "S07";
            this.label5.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(65, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "S09";
            // 
            // radioSam
            // 
            this.radioSam.AutoSize = true;
            this.radioSam.Checked = true;
            this.radioSam.Location = new System.Drawing.Point(6, 50);
            this.radioSam.Name = "radioSam";
            this.radioSam.Size = new System.Drawing.Size(45, 17);
            this.radioSam.TabIndex = 5;
            this.radioSam.TabStop = true;
            this.radioSam.Text = "S0C";
            this.radioSam.UseVisualStyleBackColor = true;
            this.radioSam.CheckedChanged += new System.EventHandler(this.radioSam_CheckedChanged);
            // 
            // radioSec
            // 
            this.radioSec.AutoSize = true;
            this.radioSec.Location = new System.Drawing.Point(6, 28);
            this.radioSec.Name = "radioSec";
            this.radioSec.Size = new System.Drawing.Size(45, 17);
            this.radioSec.TabIndex = 6;
            this.radioSec.Text = "S0A";
            this.radioSec.UseVisualStyleBackColor = true;
            this.radioSec.CheckedChanged += new System.EventHandler(this.radioSec_CheckedChanged);
            // 
            // radioMSec
            // 
            this.radioMSec.AutoSize = true;
            this.radioMSec.Location = new System.Drawing.Point(136, 28);
            this.radioMSec.Name = "radioMSec";
            this.radioMSec.Size = new System.Drawing.Size(45, 17);
            this.radioMSec.TabIndex = 7;
            this.radioMSec.Text = "S0B";
            this.radioMSec.UseVisualStyleBackColor = true;
            this.radioMSec.CheckedChanged += new System.EventHandler(this.radioMSec_CheckedChanged);
            // 
            // NewAudioOptions
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(262, 327);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericVolume);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericBlockLen);
            this.Controls.Add(this.groupLoop);
            this.Controls.Add(this.checkLoop);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboEncoding);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewAudioOptions";
            this.ShowInTaskbar = false;
            this.Text = "S00";
            this.groupLoop.ResumeLayout(false);
            this.groupLoop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLoopLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericLoopOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlockLen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboEncoding;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.CheckBox checkLoop;
        private System.Windows.Forms.GroupBox groupLoop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericLoopLength;
        private System.Windows.Forms.NumericUpDown numericLoopOffset;
        private System.Windows.Forms.NumericUpDown numericBlockLen;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericVolume;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioSec;
        private System.Windows.Forms.RadioButton radioSam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioMSec;
    }
}