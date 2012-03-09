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
    public partial class MQuestText : UserControl
    {
        int id;
        IPluginHost pluginHost;
        MainQuest mq;

        public MQuestText()
        {
            InitializeComponent();
        }
        public MQuestText(IPluginHost pluginHost, string file, int id)
        {
            InitializeComponent();
            this.id = id;
            this.pluginHost = pluginHost;

            mq = ReadFile(file);
            numBlock.Maximum = mq.num_blocks - 1;
            lblBlockNum.Text = "of " + numBlock.Maximum;
            numBlock_ValueChanged(null, null);
        }

        private MainQuest ReadFile(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            MainQuest mq = new MainQuest();

            mq.num_blocks = br.ReadUInt16();
            mq.blocks = new MainQuest.Block[mq.num_blocks];

            for (int i = 0; i < mq.num_blocks; i++)
            {
                Console.WriteLine("Reading block " + i.ToString());
                mq.blocks[i].size = br.ReadUInt16();
                mq.blocks[i].id = br.ReadUInt32();

                List<MainQuest.Block.Element> elements = new List<MainQuest.Block.Element>();
                int pos = 4;
                while (pos + 1 != mq.blocks[i].size)
                {
                    Console.WriteLine("\tElement " + (elements.Count + 1).ToString());
                    MainQuest.Block.Element e = new MainQuest.Block.Element();
                    e.size = br.ReadUInt16();
                    e.text = new String(Encoding.GetEncoding(932).GetChars(br.ReadBytes((int)e.size)));

                    elements.Add(e);
                    pos += 2 + e.size;
                }
                br.ReadByte();  // 00

                mq.blocks[i].elements = elements.ToArray();
            }

            br.Close();
            return mq;
        }

        public struct MainQuest
        {
            public ushort num_blocks;
            public Block[] blocks;

            public struct Block
            {
                public ushort size;
                public uint id;
                public Element[] elements;

                public struct Element
                {
                    public ushort size;
                    public string text;
                }
            }
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            txtID.Text = mq.blocks[i].id.ToString("x");

            numElement.Maximum = mq.blocks[i].elements.Length - 1;
            lblNum.Text = "of " + numElement.Maximum;
            numElement.Value = 0;
            numElement_ValueChanged(null, null);
        }

        private void numElement_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            int j = (int)numElement.Value;

            txtOri.Text = mq.blocks[i].elements[j].text.Replace("\n", "\r\n");
            lblSizeOri.Text = "Size: " + txtOri.Text.Length;
            txtTrans.Text = "";
            lblSizeTrans.Text = "";
        }
    }
}
