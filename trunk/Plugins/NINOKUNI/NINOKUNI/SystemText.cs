using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace NINOKUNI
{
    public partial class SystemText : UserControl
    {
        int id;
        IPluginHost pluginHost;
        SysText systext;

        public SystemText()
        {
            InitializeComponent();
        }
        public SystemText(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.id = id;
            this.pluginHost = pluginHost;

            systext = ReadFile(file);
            lblNum.Text = "of " + systext.elements.Length.ToString();
            numElement.Maximum = systext.elements.Length - 1;

            numElement_ValueChanged(numElement, null);
        }

        private SysText ReadFile(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            SysText s = new SysText();

            s.num_element = br.ReadUInt16();
            s.elements = new SysText.Element[s.num_element];

            for (int i = 0; i < s.num_element; i++)
            {
                s.elements[i].id = br.ReadUInt32();
                s.elements[i].size = br.ReadUInt16();
                s.elements[i].text = new String(Encoding.GetEncoding(932).GetChars(br.ReadBytes((int)s.elements[i].size)));
            }

            br.Close();
            return s;
        }


        private void numElement_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numElement.Value;

            txtID.Text = systext.elements[i].id.ToString("x");
            txtOri.Text = systext.elements[i].text.Replace("\n", "\r\n");
            lblSizeOri.Text = "Size: " + txtOri.Text.Length.ToString();

            if (txtTrans.Text != "")
            {
                SysText.Element el = systext.elements[i];
                el.text = txtTrans.Text;
                el.size = (ushort)Encoding.GetEncoding(932).GetByteCount(el.text);
                systext.elements[i] = el;
            }

            txtTrans.Text = "";
            lblSizeTrans.Text = "Size: 0";
        }

        private void txtTrans_TextChanged(object sender, EventArgs e)
        {
            lblSizeTrans.Text = "Size: " + txtTrans.Text.Length.ToString();
        }

        public struct SysText
        {
            public ushort num_element;
            public Element[] elements;

            public struct Element
            {
                public uint id;
                public ushort size;
                public string text;
            }
        }
    }
}
