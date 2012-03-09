namespace NINOKUNI
{
    partial class SystemText
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
            this.txtOri = new System.Windows.Forms.TextBox();
            this.txtTrans = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numElement = new System.Windows.Forms.NumericUpDown();
            this.lblNum = new System.Windows.Forms.Label();
            this.lblSizeOri = new System.Windows.Forms.Label();
            this.lblSizeTrans = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numElement)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOri
            // 
            this.txtOri.Location = new System.Drawing.Point(9, 29);
            this.txtOri.MaxLength = 65535;
            this.txtOri.Multiline = true;
            this.txtOri.Name = "txtOri";
            this.txtOri.ReadOnly = true;
            this.txtOri.Size = new System.Drawing.Size(500, 125);
            this.txtOri.TabIndex = 0;
            // 
            // txtTrans
            // 
            this.txtTrans.Location = new System.Drawing.Point(9, 206);
            this.txtTrans.MaxLength = 65535;
            this.txtTrans.Multiline = true;
            this.txtTrans.Name = "txtTrans";
            this.txtTrans.Size = new System.Drawing.Size(500, 125);
            this.txtTrans.TabIndex = 1;
            this.txtTrans.TextChanged += new System.EventHandler(this.txtTrans_TextChanged);
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(87, 463);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(100, 20);
            this.txtID.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(429, 469);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save changes";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 466);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 493);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Element:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Translated:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Original:";
            // 
            // numElement
            // 
            this.numElement.Location = new System.Drawing.Point(87, 489);
            this.numElement.Name = "numElement";
            this.numElement.Size = new System.Drawing.Size(100, 20);
            this.numElement.TabIndex = 9;
            this.numElement.ValueChanged += new System.EventHandler(this.numElement_ValueChanged);
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Location = new System.Drawing.Point(193, 493);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(19, 13);
            this.lblNum.TabIndex = 10;
            this.lblNum.Text = "of ";
            // 
            // lblSizeOri
            // 
            this.lblSizeOri.AutoSize = true;
            this.lblSizeOri.Location = new System.Drawing.Point(40, 157);
            this.lblSizeOri.Name = "lblSizeOri";
            this.lblSizeOri.Size = new System.Drawing.Size(33, 13);
            this.lblSizeOri.TabIndex = 11;
            this.lblSizeOri.Text = "Size: ";
            // 
            // lblSizeTrans
            // 
            this.lblSizeTrans.AutoSize = true;
            this.lblSizeTrans.Location = new System.Drawing.Point(43, 338);
            this.lblSizeTrans.Name = "lblSizeTrans";
            this.lblSizeTrans.Size = new System.Drawing.Size(33, 13);
            this.lblSizeTrans.TabIndex = 12;
            this.lblSizeTrans.Text = "Size: ";
            // 
            // SystemText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblSizeTrans);
            this.Controls.Add(this.lblSizeOri);
            this.Controls.Add(this.lblNum);
            this.Controls.Add(this.numElement);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.txtTrans);
            this.Controls.Add(this.txtOri);
            this.Name = "SystemText";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numElement)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOri;
        private System.Windows.Forms.TextBox txtTrans;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numElement;
        private System.Windows.Forms.Label lblNum;
        private System.Windows.Forms.Label lblSizeOri;
        private System.Windows.Forms.Label lblSizeTrans;
    }
}
