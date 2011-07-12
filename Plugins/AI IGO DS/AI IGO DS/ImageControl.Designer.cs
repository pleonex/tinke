namespace AI_IGO_DS
{
    partial class ImageControl
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
            this.picImage = new System.Windows.Forms.PictureBox();
            this.numericW = new System.Windows.Forms.NumericUpDown();
            this.numericH = new System.Windows.Forms.NumericUpDown();
            this.numericTile = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTile)).BeginInit();
            this.SuspendLayout();
            // 
            // picImage
            // 
            this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picImage.Location = new System.Drawing.Point(4, 4);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(256, 192);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // numericW
            // 
            this.numericW.Location = new System.Drawing.Point(320, 9);
            this.numericW.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericW.Name = "numericW";
            this.numericW.Size = new System.Drawing.Size(55, 20);
            this.numericW.TabIndex = 1;
            this.numericW.ValueChanged += new System.EventHandler(this.numeric_ValueChanged);
            // 
            // numericH
            // 
            this.numericH.Location = new System.Drawing.Point(452, 9);
            this.numericH.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericH.Name = "numericH";
            this.numericH.Size = new System.Drawing.Size(55, 20);
            this.numericH.TabIndex = 2;
            this.numericH.ValueChanged += new System.EventHandler(this.numeric_ValueChanged);
            // 
            // numericTile
            // 
            this.numericTile.Location = new System.Drawing.Point(320, 43);
            this.numericTile.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericTile.Name = "numericTile";
            this.numericTile.Size = new System.Drawing.Size(55, 20);
            this.numericTile.TabIndex = 3;
            this.numericTile.ValueChanged += new System.EventHandler(this.numeric_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(279, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(405, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(266, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Start tile:";
            // 
            // ImageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericTile);
            this.Controls.Add(this.numericH);
            this.Controls.Add(this.numericW);
            this.Controls.Add(this.picImage);
            this.Name = "ImageControl";
            this.Size = new System.Drawing.Size(510, 510);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.NumericUpDown numericW;
        private System.Windows.Forms.NumericUpDown numericH;
        private System.Windows.Forms.NumericUpDown numericTile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
