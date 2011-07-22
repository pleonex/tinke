using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace TXT
{
    public partial class iBMG : UserControl
    {
        IPluginHost pluginHost;
        sBMG bmg;
        string[] msg;
        string[] traducciones;

        public iBMG(IPluginHost pluginHost, sBMG bmg)
        {
            this.pluginHost = pluginHost;
            InitializeComponent();
            LeerIdioma();

            this.bmg = bmg;
            Informacion();
        }
        private void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + "\\Plugins\\TXTLang.xml");
            xml = xml.Element(pluginHost.Get_Language()).Element("BMG");

            columnHeader1.Text = xml.Element("S01").Value;
            columnHeader2.Text = xml.Element("S02").Value;
            listProp.Items[0].Text = xml.Element("S03").Value;
            listProp.Items[1].Text = xml.Element("S04").Value;
            listProp.Items[2].Text = xml.Element("S05").Value;
            listProp.Items[3].Text = xml.Element("S06").Value;
            listProp.Items[4].Text = xml.Element("S07").Value;
            listProp.Items[5].Text = xml.Element("S08").Value;
            label1.Text = xml.Element("S09").Value;
            traducciones = new String[2];
            traducciones[0] = xml.Element("S0A").Value;
            traducciones[1] = xml.Element("S0B").Value;
        }

        private void Informacion()
        {
            listProp.Items[0].SubItems.Add(bmg.uft16 ? traducciones[0] : traducciones[1]);
            listProp.Items[2].SubItems.Add(bmg.inf1.nMsg.ToString());
            listProp.Items[3].SubItems.Add(bmg.inf1.offsetLength.ToString());
            listProp.Items[4].SubItems.Add(bmg.inf1.unknown1.ToString());
            listProp.Items[5].SubItems.Add(bmg.inf1.unknown2.ToString());

            msg = bmg.dat1.msgs;
            numericMsg.Maximum = bmg.inf1.nMsg;
            txtMsg.Text = msg[0];
        }

        private void numericMsg_ValueChanged(object sender, EventArgs e)
        {
            txtMsg.Text = msg[(int)numericMsg.Value - 1];
        }
    }
}
