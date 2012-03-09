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

namespace NINOKUNI
{
    public partial class SQcontrol : UserControl
    {
        IPluginHost pluginHost;
        int id;
        SQ original;
        SQ translated;

        public SQcontrol()
        {
            InitializeComponent();
        }
        public SQcontrol(IPluginHost pluginHost, string file, int id)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.id = id;
            Read(file);
            translated = original;
            listBlock.SelectedIndex = 0;

            ReadLanguage();
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "NINOKUNILang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("SQcontrol");

                label1.Text = xml.Element("S00").Value;
                label2.Text = xml.Element("S01").Value;
                label3.Text = xml.Element("S02").Value;
                btnSave.Text = xml.Element("S03").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            original = new SQ();
            original.blocks = new List<SQ.Block>();

            original.id = br.ReadUInt32();    // File ID

            // Read the first 3 blocks
            for (int i = 0; i < 3; i++)
            {
                SQ.Block block = new SQ.Block();
                block.size = br.ReadUInt16();
                block.text = new String(Encoding.GetEncoding(932).GetChars(br.ReadBytes((int)block.size)));

                listBlock.Items.Add("Block " + i.ToString());
                original.blocks.Add(block);
            }

            original.unknown = br.ReadBytes(0xF);   // Unknown data
            original.final_blocks = br.ReadByte();

            if (original.final_blocks != 2 && original.final_blocks != 3)
                MessageBox.Show("FB");

            for (int i = 0; i < original.final_blocks; i++)
            {
                SQ.Block block = new SQ.Block();
                block.size = br.ReadUInt16();
                block.text = new String(Encoding.GetEncoding(932).GetChars(br.ReadBytes((int)block.size)));

                listBlock.Items.Add("Block " + (i + 4).ToString());
                original.blocks.Add(block);
            }

            original.final = br.ReadByte();
            if (original.final == 0)
                original.final_data = new byte[0];
            else if (original.final == 1)
            {
                byte size = br.ReadByte();
                original.final_data = br.ReadBytes(size + 4);
            }
            else
                MessageBox.Show("FD");

            br.Close();
        }
        private void Write()
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newSQ_" + Path.GetRandomFileName();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            //bw.Write((uint)0x001C080A);
            //for (int i = 0; i < translated.blocks.Count; i++)
            //{
            //    if (i + 1 != translated.blocks.Count && translated.blocks[i].type != SQ.Type.Unknown)
            //        bw.Write((ushort)translated.blocks[i].data.Length);

            //    bw.Write(translated.blocks[i].data);
            //}
            //bw.Write((byte)0x00);

            //bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void listBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOriginal.Text = original.blocks[listBlock.SelectedIndex].text.Replace("\n", "\r\n");
            txtTranslated.Text = translated.blocks[listBlock.SelectedIndex].text;
        }

    }

    public struct SQ
    {
        public uint id;
        public List<Block> blocks;

        public byte[] unknown;  // 0x0F
        public byte final_blocks;

        public struct Block
        {
            public ushort size;
            public string text;
        }

        public byte final;
        public byte[] final_data;
    }
}
