using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Sounds
{
    public partial class Waiting : Form
    {
        public Waiting()
        {
            InitializeComponent();
        }
        public Waiting(string code, string lang)
        {
            InitializeComponent();

            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "SoundLang.xml");
                xml = xml.Element(lang).Element("Messages");

                this.Text = xml.Element("S01").Value;
                lblText.Text = xml.Element("S00").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }
    }
}
