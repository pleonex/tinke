namespace LAYTON
{
    partial class ScriptControl
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
            this.treeCommands = new System.Windows.Forms.TreeView();
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // treeCommands
            // 
            this.treeCommands.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeCommands.Location = new System.Drawing.Point(0, 0);
            this.treeCommands.Name = "treeCommands";
            this.treeCommands.Size = new System.Drawing.Size(144, 512);
            this.treeCommands.TabIndex = 0;
            this.treeCommands.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeCommands_AfterSelect);
            // 
            // txtOriginal
            // 
            this.txtOriginal.Location = new System.Drawing.Point(150, 0);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.Size = new System.Drawing.Size(359, 125);
            this.txtOriginal.TabIndex = 1;
            // 
            // ScriptControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtOriginal);
            this.Controls.Add(this.treeCommands);
            this.Name = "ScriptControl";
            this.Size = new System.Drawing.Size(512, 512);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeCommands;
        private System.Windows.Forms.TextBox txtOriginal;
    }
}
