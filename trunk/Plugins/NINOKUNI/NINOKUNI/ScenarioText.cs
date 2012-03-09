using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace NINOKUNI
{
    public partial class ScenarioText : UserControl
    {
        string file;
        Sceneario sce;
        bool stop;

        public ScenarioText()
        {
            InitializeComponent();
        }
        public ScenarioText(string file)
        {
            InitializeComponent();

            this.file = file;
            ReadFile();

            numericBlock.Maximum = sce.blocks.Length;
            numericBlock.Value = 0;
        }

        private void ReadFile()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            sce = new Sceneario();
            sce.id = br.ReadUInt32();

            if (sce.id != 0x0006050A)
            {
                MessageBox.Show("Wrong ID!");
                br.Close();
                return;
            }

            List<Block> blocks = new List<Block>();
            int i = 1;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                Block block = new Block();
                block.size = br.ReadUInt32();

                List<Element> elements = new List<Element>();

                uint id = br.ReadUInt32();
                while (id != 0xFFFFFFFF)
                {
                    Element e = new Element();
                    e.id = id;
                    e.size = br.ReadByte();
                    e.text = new String(Encoding.GetEncoding(932).GetChars(br.ReadBytes(e.size)));
                    if (i == 2)
                        e.unk = br.ReadUInt16();
                    elements.Add(e);

                    id = br.ReadUInt32();
                }

                block.elements = elements.ToArray();
                blocks.Add(block);
                i++;
            }
            sce.blocks = blocks.ToArray();

            br.Close();
        }

        struct Sceneario
        {
            public uint id;
            public Block[] blocks;
        }
        struct Block
        {
            public uint size;
            public Element[] elements;
        }
        struct Element
        {
            public uint id;
            public byte size;
            public string text;
            public ushort unk;
        }

        private void numericElement_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            txtOri.Text = sce.blocks[(int)numericBlock.Value].elements[(int)numericElement.Value].text.Replace("\n", "\r\n");
            txtID.Text = sce.blocks[(int)numericBlock.Value].elements[(int)numericElement.Value].id.ToString("x");
            txtSize.Text = sce.blocks[(int)numericBlock.Value].elements[(int)numericElement.Value].size.ToString("x");
            if (numericBlock.Value == 1)
                txtUnk.Text = sce.blocks[(int)numericBlock.Value].elements[(int)numericElement.Value].unk.ToString("x");
        }

        private void numericBlock_ValueChanged(object sender, EventArgs e)
        {
            stop = true;
            numericElement.Maximum = sce.blocks[(int)numericBlock.Value].elements.Length;
            stop = false;
            numericElement.Value = 0;
        }
    }
}
