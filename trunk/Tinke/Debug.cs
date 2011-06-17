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

            txtInfo.DocumentText = "<html></html>";     // Creamos un documento nuevo
            Añadir_Texto("<b><h3>Tinke " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</h3></b>");
            txtInfo.Document.BackColor = SystemColors.GradientActiveCaption;
        }

        public void Añadir_Texto(string mensaje)
        {
            if (mensaje != "")
                txtInfo.Document.Write ("<p style=\"font-size:x-small;\">" + mensaje + "</p>");
            txtInfo.Document.Body.ScrollTop = txtInfo.Document.Body.ScrollRectangle.Height;
        }

        public void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("Sistema");

            this.Text = xml.Element("S03").Value;
        }
    }
}
