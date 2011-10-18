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
        }

        private void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            original = new SQ();
            original.blocks = new List<SQ.Block>();

            br.ReadUInt32();    // File ID
            uint size = br.ReadUInt16();

            for (int i = 0; br.BaseStream.Position < br.BaseStream.Length; i++)
            {
                SQ.Block block = new SQ.Block();

                if (size == 0x00)
                {
                    block.type = SQ.Type.Unknown;
                    size = 0xE;
                }
                else if (size == 0x0901)        // End Of File
                {
                    br.BaseStream.Position -= 2;
                    block.type = SQ.Type.Unknown;
                    size = 0xE;
                }
                else
                    block.type = SQ.Type.Text;

                listBlock.Items.Add("Block " + i.ToString());

                block.data = br.ReadBytes((int)size);
                original.blocks.Add(block);

                if (br.BaseStream.Position + 1 == br.BaseStream.Length)
                    break;
                size = br.ReadUInt16();
            }

            br.Close();
        }
        private void Write()
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newSQ_" + Path.GetRandomFileName();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((uint)0x001C080A);
            for (int i = 0; i < translated.blocks.Count; i++)
            {
                if (i + 1 != translated.blocks.Count && translated.blocks[i].type != SQ.Type.Unknown)
                    bw.Write((ushort)translated.blocks[i].data.Length);

                bw.Write(translated.blocks[i].data);
            }
            bw.Write((byte)0x00);

            bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void listBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (original.blocks[listBlock.SelectedIndex].type == SQ.Type.Text)
            {
                txtOriginal.Text = new String(Encoding.GetEncoding("shift_jis").GetChars(original.blocks[listBlock.SelectedIndex].data));
                txtTranslated.Text = new String(Encoding.GetEncoding("shift_jis").GetChars(translated.blocks[listBlock.SelectedIndex].data));
            }
            else
            {
                txtOriginal.Text = "Unknown\r\n" + BitConverter.ToString(original.blocks[listBlock.SelectedIndex].data);
                txtTranslated.Text = "";
            }
        }

    }

    public struct SQ
    {
        public List<Block> blocks;

        public struct Block
        {
            public Type type;
            public byte[] data;
        }
        public enum Type
        {
            Text,
            Unknown
        }
    }
}
