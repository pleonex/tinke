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
            ReadLanguage();
            this.Location = new Point(10, 585);
            this.FormClosing += new FormClosingEventHandler(Debug_FormClosing);
            txtInfo.Navigating += new WebBrowserNavigatingEventHandler(txtInfo_Navigating);

            if (Type.GetType("Mono.Runtime") == null) // Mono gives problems with the ie control
            {
                txtInfo.DocumentText = "<html></html>";     // Blank document
                Add_Text("<b><h3>Tinke " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</h3></b>");
                txtInfo.Document.BackColor = SystemColors.GradientActiveCaption;
            }
        }

        private void txtInfo_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() != "about:blank")
                e.Cancel = true;
        }
        void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Add_Text(string mensaje)
        {
            if (Type.GetType ("Mono.Runtime") == null) // Mono give problems with the ie control
            {
                if (mensaje != "")
                    txtInfo.Document.Write ("<p style=\"font-size:x-small;\">" + mensaje + "</p>");
                txtInfo.Document.Body.ScrollTop = txtInfo.Document.Body.ScrollRectangle.Height;
            }
        }

        public void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Sistema");
                this.Text = xml.Element("S03").Value;

                xml = Tools.Helper.GetTranslation("Messages");
                contextMenuStrip1.Items[0].Text = xml.Element("S0E").Value;
                //clearLogToolStripMenuItem.Text = xml.Element("S26").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void s01ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Type.GetType("Mono.Runtime") == null) // Mono gives problems with the ie control
            {
                txtInfo.ShowSaveAsDialog();
            }
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DON'T WORK
            if (Type.GetType("Mono.Runtime") == null)
            {
                txtInfo.Navigate("about:blank");
                if (txtInfo.Document != null)
                {
                    txtInfo.Document.Write(string.Empty);
                }
                txtInfo.DocumentText = "<p style=\"font-size:x-small;\">Hello world!</p>";     // Blank document
                Add_Text("<b><h3>Tinke " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</h3></b>");
                txtInfo.Document.BackColor = SystemColors.GradientActiveCaption;
            }
        }
    }
}
