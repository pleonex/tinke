// ----------------------------------------------------------------------
// <copyright file="ScenarioText.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>29/04/2012 13:38:40</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using Ekona;

namespace NINOKUNI
{
    public partial class ScenarioText : UserControl
    {
        string fileName;
        int id;
        IPluginHost pluginHost;
        Scenario sce;
        Scenario sce_old;
        Encoding enc;

        public ScenarioText()
        {
            InitializeComponent();
        }
        public ScenarioText(IPluginHost pluginHost, string file, int id)
        {
            InitializeComponent();

            this.fileName = Path.GetFileNameWithoutExtension(file).Substring(12);
            this.pluginHost = pluginHost;
            this.id = id;
            enc = Encoding.GetEncoding("shift_jis");

            sce = Read(file);
            sce_old = Read(file);

            numericBlock.Maximum = 2;
            numericBlock_ValueChanged(null, null);
        }

        private Scenario Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            Scenario sce = new Scenario();
            sce.type = br.ReadUInt32(); // Must be 0x0006050A

            sce.blocks = new Scenario.Block[3];
            for (int i = 0; i < 3; i++)
            {
                sce.blocks[i].size = br.ReadUInt32();

                List<Scenario.Element> elements = new List<Scenario.Element>();
                for (; ; )
                {
                    Scenario.Element e = new Scenario.Element();
                    e.id = br.ReadUInt32();
                    if (e.id == 0xFFFFFFFF)
                        break;
                    e.size = br.ReadByte();

                    if (i == 0)
                    {
                        e.text = Get_Furigana(Encoding.GetEncoding(932).GetChars(br.ReadBytes(e.size)));
                        e.text = Helper.SJISToLatin(e.text);
                    }
                    else if (i == 1)
                    {
                        e.text = Get_Furigana(Encoding.GetEncoding(932).GetChars(br.ReadBytes(e.size)));
                        e.text = Helper.SJISToLatin(e.text);
                        e.unk = br.ReadUInt16();
                    }
                    else if (i == 2)
                    {
                        e.unk = br.ReadUInt16();
                        e.text = Get_Furigana(Encoding.GetEncoding(932).GetChars(br.ReadBytes(e.size - 4)));
                        e.text = Helper.SJISToLatin(e.text);
                        e.unk2 = br.ReadUInt16();
                    }

                    elements.Add(e);
                }

                sce.blocks[i].elements = elements.ToArray();
            }

            br.Close();
            return sce;
        }
        public void Write(string fileOut)
        {
            Update_Block();
            if (File.Exists(fileOut))
                File.Delete(fileOut);

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(0x0006050A);   // Header

            for (int i = 0; i < sce.blocks.Length; i++)
            {
                bw.Write(sce.blocks[i].size);
                for (int j = 0; j < sce.blocks[i].elements.Length; j++)
                {
                    bw.Write(sce.blocks[i].elements[j].id);
                    bw.Write(sce.blocks[i].elements[j].size);

                    if (i == 2)
                        bw.Write(sce.blocks[i].elements[j].unk);

                    bw.Write(enc.GetBytes(Helper.LatinToSJIS(Set_Furigana(sce.blocks[i].elements[j].text))));

                    if (i == 1)
                        bw.Write(sce.blocks[i].elements[j].unk);
                    else if (i == 2)
                        bw.Write(sce.blocks[i].elements[j].unk2);
                }
                bw.Write(0xFFFFFFFF);
            }

            bw.Flush();
            bw.Close();
        }
        private void Update_Block()
        {
            Scenario.Block block;
            int size = 0;

            // Block 0
            block = sce.blocks[0];
            for (int i = 0; i < block.elements.Length; i++)
            {
                block.elements[i].size = (byte)enc.GetByteCount(Helper.LatinToSJIS(Set_Furigana(block.elements[i].text)));
                size += block.elements[i].size + 5; // Text + ID + Size
            }
            size += 4;  // 0xFFFFFFFF
            block.size = (uint)size;
            sce.blocks[0] = block;

            // Block 1
            block = sce.blocks[1];
            size = 0;
            for (int i = 0; i < block.elements.Length; i++)
            {
                block.elements[i].size = (byte)enc.GetByteCount(Helper.LatinToSJIS(Set_Furigana(block.elements[i].text)));
                size += block.elements[i].size + 7; // Text + ID + Size + Unknown
            }
            size += 4;  // 0xFFFFFFFF
            block.size = (uint)size;
            sce.blocks[1] = block;

            // Block 2
            block = sce.blocks[2];
            size = 0;
            for (int i = 0; i < block.elements.Length; i++)
            {
                block.elements[i].size = (byte)(enc.GetByteCount(Helper.LatinToSJIS(Set_Furigana(block.elements[i].text))) + 4);
                size += block.elements[i].size + 5; // Text + ID + Size
            }
            size += 4;  // 0xFFFFFFFF
            block.size = (uint)size;
            sce.blocks[2] = block;
        }
        private string Get_Furigana(char[] text)
        {
            bool furigana = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\x01')
                {
                    text[i] = '【';
                    furigana = true;
                }
                else if (text[i] == '\n')
                {
                    if (furigana)
                    {
                        text[i] = ':';
                        furigana = false;
                    }
                    else
                        text[i] = '】';
                }
            }

            return new string(text);
        }
        private string Set_Furigana(string s)
        {
            char[] text = s.ToCharArray();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '【')
                    text[i] = '\x01';
                else if (text[i] == ':')
                    text[i] = '\n';
                else if (text[i] == '】')
                    text[i] = '\n';
            }

            return new string(text);
        }

        private void numericElement_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;
            int j = (int)numericElement.Value;

            txtOri.Text = sce_old.blocks[i].elements[j].text.Replace("\n", "\r\n");
            txtNew.Text = sce.blocks[i].elements[j].text.Replace("\n", "\r\n");
            numID.Value = sce.blocks[i].elements[j].id;

            if (i == 0)
            {
                numUnk1.Enabled = false;
                numUnk2.Enabled = false;
            }
            else if (i == 1)
            {
                numUnk1.Enabled = true;
                numUnk1.Value = sce.blocks[i].elements[j].unk;
                numUnk2.Enabled = false;
            }
            else if (i == 2)
            {
                numUnk1.Enabled = true;
                numUnk1.Value = sce.blocks[i].elements[j].unk;
                numUnk2.Enabled = true;
                numUnk2.Value = sce.blocks[i].elements[j].unk2;
            }
        }
        private void numericBlock_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;

            numericElement.Maximum = sce.blocks[i].elements.Length - 1;
            label9.Text = "of " + numericElement.Maximum.ToString();
            numericElement_ValueChanged(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Write(fileOut);
            pluginHost.ChangeFile(id, fileOut);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.AddExtension = true;
            o.CheckFileExists = true;
            o.DefaultExt = ".xml";
            o.FileName = fileName + ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            Import(o.FileName);
            numericBlock_ValueChanged(null, null);

            // Write file
            btnSave_Click(null, null);
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".xml";
            o.FileName = fileName + ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            if (File.Exists(o.FileName))
                File.Delete(o.FileName);

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("Scenario");

            for (int i = 0; i < sce.blocks[0].elements.Length; i++)
            {
                XmlElement el = doc.CreateElement("String");
                el.SetAttribute("ID", sce.blocks[0].elements[i].id.ToString("x"));

                // Format the text
                string text = sce.blocks[0].elements[i].text;
                text = text.Replace("\n", "\n    ");
                if (text.Contains("\n"))
                    text = "\n    " + text + "\n  ";

                el.InnerText = text;
                root.AppendChild(el);
            }

            doc.AppendChild(root);
            doc.Save(o.FileName);
        }
        public void Import(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNode root = doc.ChildNodes[1];
            int j = 0;
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                if (root.ChildNodes[i].NodeType != XmlNodeType.Element)
                    continue;

                string text = root.ChildNodes[i].InnerText;
                if (text.Contains("\n"))
                {
                    text = text.Remove(0, 5);
                    text = text.Remove(text.Length - 3);
                    text = text.Replace("\n    ", "\n");
                }

                sce.blocks[0].elements[j].text = text;
                sce.blocks[0].elements[j].id = Convert.ToUInt32(root.ChildNodes[i].Attributes["ID"].Value, 16);
                j++;
            }
        }

        private void txtNew_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;
            int j = (int)numericElement.Value;
            sce.blocks[i].elements[j].text = txtNew.Text.Replace("\r\n", "\n");
        }
        private void numUnk1_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;
            int j = (int)numericElement.Value;
            sce.blocks[i].elements[j].unk = (ushort)numUnk1.Value;
        }
        private void numID_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;
            int j = (int)numericElement.Value;
            sce.blocks[i].elements[j].id = (uint)numID.Value;
        }
        private void numUnk2_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numericBlock.Value;
            int j = (int)numericElement.Value;
            sce.blocks[i].elements[j].unk2 = (ushort)numUnk2.Value;
        }
    }

    struct Scenario
    {
        public uint type;
        public Block[] blocks;

        public struct Block
        {
            public uint size;
            public Element[] elements;
        }
        public struct Element
        {
            public uint id;
            public byte size;
            public string text;
            public ushort unk;
            public ushort unk2;
        }
    }
}
