namespace NINOKUNI
{
    partial class MQuestText
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
            this.lblSizeTrans = new System.Windows.Forms.Label();
            this.lblSizeOri = new System.Windows.Forms.Label();
            this.lblNum = new System.Windows.Forms.Label();
            this.numElement = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtTrans = new System.Windows.Forms.TextBox();
            this.txtOri = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblBlockNum = new System.Windows.Forms.Label();
            this.numBlock = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numElement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSizeTrans
            // 
            this.lblSizeTrans.AutoSize = true;
            this.lblSizeTrans.Location = new System.Drawing.Point(42, 333);
            this.lblSizeTrans.Name = "lblSizeTrans";
            this.lblSizeTrans.Size = new System.Drawing.Size(33, 13);
            this.lblSizeTrans.TabIndex = 23;
            this.lblSizeTrans.Text = "Size: ";
            // 
            // lblSizeOri
            // 
            this.lblSizeOri.AutoSize = true;
            this.lblSizeOri.Location = new System.Drawing.Point(39, 152);
            this.lblSizeOri.Name = "lblSizeOri";
            this.lblSizeOri.Size = new System.Drawing.Size(33, 13);
            this.lblSizeOri.TabIndex = 22;
            this.lblSizeOri.Text = "Size: ";
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Location = new System.Drawing.Point(199, 436);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(19, 13);
            this.lblNum.TabIndex = 21;
            this.lblNum.Text = "of ";
            // 
            // numElement
            // 
            this.numElement.Location = new System.Drawing.Point(93, 432);
            this.numElement.Name = "numElement";
            this.numElement.Size = new System.Drawing.Size(100, 20);
            this.numElement.TabIndex = 20;
            this.numElement.ValueChanged += new System.EventHandler(this.numElement_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Original:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Translated:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 436);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Element:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 461);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "ID:";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(93, 458);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(100, 20);
            this.txtID.TabIndex = 15;
            // 
            // txtTrans
            // 
            this.txtTrans.Location = new System.Drawing.Point(8, 201);
            this.txtTrans.MaxLength = 65535;
            this.txtTrans.Multiline = true;
            this.txtTrans.Name = "txtTrans";
            this.txtTrans.Size = new System.Drawing.Size(500, 125);
            this.txtTrans.TabIndex = 14;
            // 
            // txtOri
            // 
            this.txtOri.Location = new System.Drawing.Point(8, 24);
            this.txtOri.MaxLength = 65535;
            this.txtOri.Multiline = true;
            this.txtOri.Name = "txtOri";
            this.txtOri.ReadOnly = true;
            this.txtOri.Size = new System.Drawing.Size(500, 125);
            this.txtOri.TabIndex = 13;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(429, 469);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 24;
            this.btnSave.Text = "Save changes";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 486);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Block:";
            // 
            // lblBlockNum
            // 
            this.lblBlockNum.AutoSize = true;
            this.lblBlockNum.Location = new System.Drawing.Point(199, 486);
            this.lblBlockNum.Name = "lblBlockNum";
            this.lblBlockNum.Size = new System.Drawing.Size(19, 13);
            this.lblBlockNum.TabIndex = 26;
            this.lblBlockNum.Text = "of ";
            // 
            // numBlock
            // 
            this.numBlock.Location = new System.Drawing.Point(93, 484);
            this.numBlock.Name = "numBlock";
            this.numBlock.Size = new System.Drawing.Size(100, 20);
            this.numBlock.TabIndex = 27;
            this.numBlock.ValueChanged += new System.EventHandler(this.numBlock_ValueChanged);
            // 
            // MQuestText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.numBlock);
            this.Controls.Add(this.lblBlockNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblSizeTrans);
            this.Controls.Add(this.lblSizeOri);
            this.Controls.Add(this.lblNum);
            this.Controls.Add(this.numElement);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.txtTrans);
            this.Controls.Add(this.txtOri);
            this.Name = "MQuestText";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numElement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSizeTrans;
        private System.Windows.Forms.Label lblSizeOri;
        private System.Windows.Forms.Label lblNum;
        private System.Windows.Forms.NumericUpDown numElement;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtTrans;
        private System.Windows.Forms.TextBox txtOri;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblBlockNum;
        private System.Windows.Forms.NumericUpDown numBlock;
    }
}
