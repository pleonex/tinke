using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace TXT
{
    public partial class iTXT : UserControl
    {
        IPluginHost pluginHost;
        int id;
        byte[] text;

        public iTXT(byte[] text, IPluginHost pluginHost, int id)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.id = id;
            this.text = text;

            if ((text[0] == 0xFE && text[1] == 0xFF) || (text[0] == 0xFF && text[1] == 0xFE))
            {
                txtBox.Text = Descodificar(Encoding.Unicode);
                comboEncod.SelectedIndex = 1;
            }
            else
            {
                txtBox.Text = Descodificar(Encoding.UTF8);
                comboEncod.SelectedIndex = 0;
            }

            txtBox.Text = txtBox.Text.Replace("\n", "\r\n");
            txtBox.Text = txtBox.Text.Replace("\\n", "\r\n");

            LeerIdioma();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string tempFile = Path.GetTempFileName();

            text = Encoding.GetEncoding(comboEncod.Text).GetBytes(txtBox.Text);
            File.WriteAllBytes(tempFile, text);
            pluginHost.ChangeFile(id, tempFile);
        }

        private void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + "\\Plugins\\TXTLang.xml");
            xml = xml.Element(pluginHost.Get_Language());
            xml = xml.Element("TXT");

            btnSave.Text = xml.Element("S00").Value;
            label1.Text = xml.Element("S01").Value;
            checkWordWrap.Text = xml.Element("S02").Value;
        }

        private String Descodificar(Encoding encoding)
        {
            String texto = new String(encoding.GetChars(text));
            texto = texto.Replace("\n", "\r\n");
            return texto;
        }
        private void comboEncod_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBox.Text = Descodificar(Encoding.GetEncoding(comboEncod.Text));
        }

        private void checkWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            txtBox.WordWrap = checkWordWrap.Checked;
        }
    }
}
