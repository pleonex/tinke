using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Tinke.Dialog
{
    public partial class SelectOffset : Form
    {
        public SelectOffset()
        {
            InitializeComponent();
            ReadLanguage();

            numericOffset.Select(0, 1);
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.GetTranslation("Dialog");

                this.Text = xml.Element("S04").Value;
                btnOK.Text = xml.Element("S01").Value;
                label1.Text = xml.Element("S05").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int Offset
        {
            get { return (int)numericOffset.Value; }
            set { numericOffset.Value = value; }
        }
    }
}
