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
            listsBlock.SelectedIndex = 0;

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
            original.sblocks = new SQ.Block[4];

            original.id = br.ReadUInt32();    // File ID

            // Read the first 4 blocks
            for (int i = 0; i < 4; i++)
            {
                original.sblocks[i].size = br.ReadUInt16();
                original.sblocks[i].text = new String(Encoding.GetEncoding(932).GetChars(
                                           br.ReadBytes((int)original.sblocks[i].size)));

                listsBlock.Items.Add("Block " + i.ToString());
            }

            original.unknown = br.ReadBytes(0xD);   // Unknown data
            original.num_fblocks = br.ReadByte();

            original.fblocks = new SQ.Block[original.num_fblocks];
            for (int i = 0; i < original.num_fblocks; i++)
            {
                original.fblocks[i].size = br.ReadUInt16();
                original.fblocks[i].text = new String(Encoding.GetEncoding(932).GetChars(
                                           br.ReadBytes((int)original.fblocks[i].size)));

                listfBlock.Items.Add("Block " + i.ToString());
            }

            original.num_final = br.ReadByte();
            original.final = new byte[original.num_final][];
            for (int i = 0; i < original.num_final; i++)
            {
                byte size = br.ReadByte();
                original.final[i] = br.ReadBytes(size + 4);
            }

            br.Close();
        }
        private void Write()
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newSQ_" + Path.GetRandomFileName();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void listBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOriginal.Text = original.sblocks[listsBlock.SelectedIndex].text.Replace("\n", "\r\n");
            txtTranslated.Text = translated.sblocks[listsBlock.SelectedIndex].text.Replace("\n", "\r\n");
        }
        private void listfBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOriginal.Text = original.fblocks[listfBlock.SelectedIndex].text.Replace("\n", "\r\n");
            txtTranslated.Text = translated.fblocks[listfBlock.SelectedIndex].text.Replace("\n", "\r\n");
        }

    }

    public struct SQ
    {
        public uint id;

        public Block[] sblocks; // First 4 blocks

        public byte[] unknown;  // 0x0D
        public byte num_fblocks;

        public Block[] fblocks; // Last block, variable length

        public byte num_final;
        public byte[][] final;

        public struct Block
        {
            public ushort size;
            public string text;
        }
   }
}
