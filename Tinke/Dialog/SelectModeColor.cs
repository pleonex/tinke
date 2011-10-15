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
                radioButton3.Text = xml.Element("S1D").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int Option
        {
            get
            {
                if (radioButton1.Checked)
                    return 1;
                else if (radioButton2.Checked)
                    return 2;
                else if (radioButton3.Checked)
                    return 3;
                else
                    return 0;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        radioButton1.Checked = true;
                        break;
                    case 2:
                        radioButton2.Checked = true;
                        break;
                    case 3:
                        radioButton3.Checked = true;
                        break;
                }
            }
        }
    }
}
