using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Tinke
{
    public partial class Debug : Form
    {
        public Debug()
        {
            InitializeComponent();
            LeerIdioma();
            this.Location = new Point(10, 585);
            this.FormClosing += new FormClosingEventHandler(Debug_FormClosing);

            if (Type.GetType("Mono.Runtime") == null) // Evitamos Mono que da problemas
            {
                txtInfo.DocumentText = "<html></html>";     // Creamos un documento nuevo
                Añadir_Texto("<b><h3>Tinke " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</h3></b>");
                txtInfo.Document.BackColor = SystemColors.GradientActiveCaption;
            }
        }

        void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Añadir_Texto(string mensaje)
        {
            if (Type.GetType ("Mono.Runtime") == null) // Evitamos Mono que da problemas
            {
                if (mensaje != "")
                    txtInfo.Document.Write ("<p style=\"font-size:x-small;\">" + mensaje + "</p>");
                txtInfo.Document.Body.ScrollTop = txtInfo.Document.Body.ScrollRectangle.Height;
            }
        }

        public void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Sistema");
            this.Text = xml.Element("S03").Value;

            xml = Tools.Helper.GetTranslation("Messages");
            contextMenuStrip1.Items[0].Text = xml.Element("S0E").Value;
        }

        private void s01ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Type.GetType("Mono.Runtime") == null) // Evitamos Mono que da problemas
            {
                txtInfo.ShowSaveAsDialog();
            }
        }
    }
}
