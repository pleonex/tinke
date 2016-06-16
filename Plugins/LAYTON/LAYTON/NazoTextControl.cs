using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    using System.IO;
    using System.Security.Permissions;

    using Ekona;

    public partial class NazoTextControl : UserControl
    {
        IPluginHost pluginHost;
        int id;

        private ushort nazoId;
        private ushort nazoStrOffset;
        private string nazoName;
        private byte[] unkData;
        private string[] strings;

        private int prevIndex;

        public NazoTextControl(IPluginHost pluginHost, string fileIn, int id)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = id;
            LeerIdioma();

            Read(fileIn);
            for (int i = 0; i < this.strings.Length; i++) this.comboBox1.Items.Add("Message #" + i);
            this.textBoxName.Text = this.nazoName;
            this.textBoxId.Text = this.nazoId.ToString();
            this.prevIndex = this.comboBox1.SelectedIndex;
            this.comboBox1.SelectedIndex = 0;
        }

        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("Text");
                btnSave.Text = xml.Element("S00").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void Read(string fileIn)
        {
            Encoding enc = Encoding.GetEncoding("shift-jis");

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            br.BaseStream.Position = 0;
            this.nazoId = br.ReadUInt16();
            this.nazoStrOffset = br.ReadUInt16();
            this.nazoName = enc.GetString(br.ReadBytes(0x30)).TrimEnd('\0');
            this.unkData = br.ReadBytes(0xC);
            
            List<uint> offsets = new List<uint>();
            uint offset = br.ReadUInt32();
            bool end = false;
            while (!end)
            {
                offsets.Add(offset);
                offset = br.ReadUInt32();
                end = offset == 0;
            }

            offsets.Add((uint)(new FileInfo(fileIn).Length) - this.nazoStrOffset);

            this.strings = new string[offsets.Count - 1];
            for (int i = 0; i < this.strings.Length; i++)
            {
                br.BaseStream.Position = offsets[i] + this.nazoStrOffset;
                this.strings[i] = enc.GetString(br.ReadBytes((int)(offsets[i + 1] - offsets[i] - 1))).Replace("\x0A", "\r\n"); ;
            }

            br.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.prevIndex >= 0) this.strings[prevIndex] = this.txtBox.Text;
            this.txtBox.Text = this.strings[this.comboBox1.SelectedIndex];
            this.prevIndex = this.comboBox1.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.nazoName = this.textBoxName.Text;
            this.strings[this.comboBox1.SelectedIndex] = this.txtBox.Text;

            string tempFile = this.pluginHost.Get_TempFile();
            Encoding enc = Encoding.GetEncoding("shift-jis");
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(tempFile), enc);
            bw.BaseStream.SetLength(0);
            bw.Write(this.nazoId);
            bw.Write(this.nazoStrOffset);
            bw.Write(enc.GetBytes(this.nazoName));
            bw.BaseStream.SetLength(0x33);
            bw.BaseStream.Position = 0x33;
            bw.Write((byte)0);
            bw.Write(this.unkData);

            bw.BaseStream.Position = this.nazoStrOffset;
            uint[] offsets = new uint[this.strings.Length];
            for (int i = 0; i < this.strings.Length; i++)
            {
                offsets[i] = (uint)bw.BaseStream.Position - this.nazoStrOffset;
                bw.Write(enc.GetBytes(this.strings[i].Replace("\r\n", "\x0A")));
                bw.Write((byte)0);
            }

            bw.BaseStream.Position = 0x40;
            for (int i = 0; i < this.strings.Length; i++) bw.Write(offsets[i]);

            bw.Close();

            this.pluginHost.ChangeFile(this.id, tempFile);
        }
    }
}
