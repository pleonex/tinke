namespace NINOKUNI
{
    partial class ScenarioText
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
            this.numericBlock = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericElement = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.txtUnk = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericElement)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOri
            // 
            this.txtOri.Location = new System.Drawing.Point(4, 4);
            this.txtOri.Multiline = true;
            this.txtOri.Name = "txtOri";
            this.txtOri.ReadOnly = true;
            this.txtOri.Size = new System.Drawing.Size(492, 251);
            this.txtOri.TabIndex = 0;
            // 
            // numericBlock
            // 
            this.numericBlock.Location = new System.Drawing.Point(67, 294);
            this.numericBlock.Name = "numericBlock";
            this.numericBlock.Size = new System.Drawing.Size(70, 20);
            this.numericBlock.TabIndex = 1;
            this.numericBlock.ValueChanged += new System.EventHandler(this.numericBlock_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Block:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 323);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Element";
            // 
            // numericElement
            // 
            this.numericElement.Location = new System.Drawing.Point(67, 320);
            this.numericElement.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericElement.Name = "numericElement";
            this.numericElement.Size = new System.Drawing.Size(70, 20);
            this.numericElement.TabIndex = 4;
            this.numericElement.ValueChanged += new System.EventHandler(this.numericElement_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(255, 296);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(255, 323);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Size:";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(317, 294);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(100, 20);
            this.txtID.TabIndex = 7;
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(317, 320);
            this.txtSize.Name = "txtSize";
            this.txtSize.ReadOnly = true;
            this.txtSize.Size = new System.Drawing.Size(100, 20);
            this.txtSize.TabIndex = 8;
            // 
            // txtUnk
            // 
            this.txtUnk.Location = new System.Drawing.Point(317, 347);
            this.txtUnk.Name = "txtUnk";
            this.txtUnk.ReadOnly = true;
            this.txtUnk.Size = new System.Drawing.Size(100, 20);
            this.txtUnk.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 350);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Unknown:";
            // 
            // ScenarioText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtUnk);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericElement);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericBlock);
            this.Controls.Add(this.txtOri);
            this.Name = "ScenarioText";
            this.Size = new System.Drawing.Size(512, 512);
            ((System.ComponentModel.ISupportInitialize)(this.numericBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericElement)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOri;
        private System.Windows.Forms.NumericUpDown numericBlock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericElement;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.TextBox txtUnk;
        private System.Windows.Forms.Label label5;
    }
}
