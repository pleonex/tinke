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
    public partial class SelectModeColor : Form
    {
        public SelectModeColor()
        {
            InitializeComponent();
            ReadLanguage();
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.ObtenerTraduccion("Dialog");

                this.Text = xml.Element("S00").Value;
                btnOK.Text = xml.Element("S01").Value;
                radioButton1.Text = xml.Element("S02").Value;
                radioButton2.Text = xml.Element("S03").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool IsOption1
        {
            get { return radioButton1.Checked; }
            set { radioButton1.Checked = value; }
        }
        public bool IsOption2
        {
            get { return radioButton2.Checked; }
            set { radioButton2.Checked = value; }
        }
    }
}
