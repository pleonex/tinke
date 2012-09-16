// ----------------------------------------------------------------------
// <copyright file="SQcontrol.cs" company="none">

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
// <date>29/04/2012 13:40:16</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Ekona;

namespace NINOKUNI
{
    public partial class SQcontrol : UserControl
    {
        IPluginHost pluginHost;
        string fileName;
        int id;
        SQ original;
        SQ translated;
        Encoding enc;

        public SQcontrol()
        {
            InitializeComponent();
        }
        public SQcontrol(IPluginHost pluginHost, string file, int id)
        {
            InitializeComponent();
            this.fileName = Path.GetFileName(file).Substring(12);
            this.pluginHost = pluginHost;
            this.id = id;
            enc = Encoding.GetEncoding(932);

            original = Read(file);
            translated = Read(file);

            ReadLanguage();
            radio_CheckedChanged(null, null);
            //TEST();
        }

        private void TEST()
        {
            // Test method to know if it's working with all the files
            // it could be useful later to import all of them in one time (batch mode)
            string folder = @"G:\nds\projects\ninokuni\Quest\";
            string[] files = Directory.GetFiles(folder, "*.sq");

            for (int i = 0; i < files.Length; i++)
            {
                SQ temp = Read(files[i]);
                string temp_xml = files[i] + ".xml";
                Export_XML(temp_xml, temp);
                Import_XML(temp_xml, ref temp);
                string temp_sq = files[i] + ".nSQ";
                Write(temp_sq, temp);

                if (!Compare(files[i], temp_sq))
                    MessageBox.Show("Test " + files[i]);
                File.Delete(temp_sq);
            }
            MessageBox.Show("Final");
        }
        private bool Compare(string f1, string f2)
        {
            byte[] b1 = File.ReadAllBytes(f1);
            byte[] b2 = File.ReadAllBytes(f2);

            if (b1.Length != b2.Length)
                return false;

            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;

            return true;
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "Ninokuni.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("SQcontrol");

                label1.Text = xml.Element("S00").Value;
                label2.Text = xml.Element("S01").Value;
                //label3.Text = xml.Element("S02").Value;
                btnSave.Text = xml.Element("S03").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        public SQ Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            SQ original = new SQ();
            original.sblocks = new SQ.Block[4];

            original.id = br.ReadUInt32();    // File ID

            // Read the first 4 blocks
            for (int i = 0; i < 4; i++)
            {
                original.sblocks[i].size = br.ReadUInt16();
                original.sblocks[i].text = new String(enc.GetChars(br.ReadBytes((int)original.sblocks[i].size)));
                original.sblocks[i].text = Helper.SJISToLatin(original.sblocks[i].text);
            }

            original.unknown = br.ReadBytes(0xD);   // Unknown data
            original.num_fblocks = br.ReadByte();

            original.fblocks = new SQ.Block[original.num_fblocks];
            for (int i = 0; i < original.num_fblocks; i++)
            {
                original.fblocks[i].size = br.ReadUInt16();
                original.fblocks[i].text = new String(enc.GetChars(br.ReadBytes((int)original.fblocks[i].size)));
                original.fblocks[i].text = Helper.SJISToLatin(original.fblocks[i].text);
            }

            original.num_final = br.ReadByte();
            original.final = new byte[original.num_final][];
            for (int i = 0; i < original.num_final; i++)
            {
                byte size = br.ReadByte();
                original.final[i] = br.ReadBytes(size + 4);
            }

            br.Close();
            return original;
        }
        public void Write(string fileOut, SQ translated)
        {
            Update_Blocks(ref translated);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(translated.id);

            // First 4 blocks
            for (int i = 0; i < translated.sblocks.Length; i++)
            {
                bw.Write(translated.sblocks[i].size);
                bw.Write(enc.GetBytes(Helper.LatinToSJIS(translated.sblocks[i].text)));
            }

            bw.Write(translated.unknown);
            bw.Write(translated.num_fblocks);

            // Write final blocks
            for (int i = 0; i < translated.fblocks.Length; i++)
            {
                bw.Write(translated.fblocks[i].size);
                bw.Write(enc.GetBytes(Helper.LatinToSJIS(translated.fblocks[i].text)));
            }

            bw.Write(translated.num_final);
            for (int i = 0; i < translated.final.Length; i++)
            {
                bw.Write((byte)(translated.final[i].Length - 4));
                bw.Write(translated.final[i]);
            }

            bw.Flush();
            bw.Close();
        }
        private void Update_Blocks(ref SQ translated)
        {
            for (int i = 0; i < translated.sblocks.Length; i++)
                translated.sblocks[i].size = (ushort)enc.GetByteCount(Helper.LatinToSJIS(translated.sblocks[i].text));
            for (int i = 0; i < translated.fblocks.Length; i++)
                translated.fblocks[i].size = (ushort)enc.GetByteCount(Helper.LatinToSJIS(translated.fblocks[i].text));
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

            Export_XML(o.FileName, translated);
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

            Import_XML(o.FileName, ref translated);

            // Write file
            btnSave_Click(null, null);

            numString_ValueChanged(null, null);
        }
        private void Export_XML(string fileOut, SQ translated)
        {
            if (File.Exists(fileOut))
                File.Delete(fileOut);

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement root = doc.CreateElement("SubQuest");

            XmlElement sBlocks = doc.CreateElement("StartBlocks");
            for (int i = 0; i < translated.sblocks.Length; i++)
            {
                XmlElement s = doc.CreateElement("String");

                string text = translated.sblocks[i].text;
                text = text.Replace('<', '【');
                text = text.Replace('>', '】');
                text = text.Replace("\n", "\n      ");
                if (text.Contains("\n"))
                    text = "\n      " + text + "\n    ";

                s.InnerText = text;
                sBlocks.AppendChild(s);
            }
            root.AppendChild(sBlocks);

            XmlElement fBlocks = doc.CreateElement("FinalBlocks");
            for (int i = 0; i < translated.fblocks.Length; i++)
            {
                XmlElement s = doc.CreateElement("String");

                string text = translated.fblocks[i].text;
                text = text.Replace('<', '【');
                text = text.Replace('>', '】');
                text = text.Replace("\n", "\n      ");
                if (text.Contains("\n"))
                    text = "\n      " + text + "\n    ";

                s.InnerText = text;
                fBlocks.AppendChild(s);
            }
            root.AppendChild(fBlocks);

            doc.AppendChild(root);
            doc.Save(fileOut);
        }
        public void Import_XML(string fileIn, ref SQ translated)
        {
            XDocument doc = XDocument.Load(fileIn);
            XElement root = doc.Element("SubQuest");

            XElement sBlocks = root.Element("StartBlocks");
            int i = 0;
            foreach (XElement e in sBlocks.Elements("String"))
            {
                string text = e.Value;
                if (text.Contains("\n"))
                {
                    text = text.Remove(0, 7);
                    text = text.Remove(text.Length - 5);
                    text = text.Replace("\n      ", "\n");
                }
                text = text.Replace('【', '<');
                text = text.Replace('】', '>');

                translated.sblocks[i++].text = text;
            }

            XElement fBlocks = root.Element("FinalBlocks");
            i = 0;
            foreach (XElement e in fBlocks.Elements("String"))
            {
                string text = e.Value;
                if (text.Contains("\n"))
                {
                    text = text.Remove(0, 7);
                    text = text.Remove(text.Length - 5);
                    text = text.Replace("\n      ", "\n");
                }
                text = text.Replace('【', '<');
                text = text.Replace('】', '>');

                translated.fblocks[i++].text = text;
            }

            sBlocks = null;
            fBlocks = null;
            root = null;
            doc = null;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + ".SQ";
            Write(fileOut, translated);
            pluginHost.ChangeFile(id, fileOut);
        }

        private void numString_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numString.Value;
            if (radioStart.Checked)
            {
                txtOriginal.Text = original.sblocks[i].text.Replace("\n", "\r\n");
                txtTranslated.Text = translated.sblocks[i].text.Replace("\n", "\r\n");
            }
            else
            {
                txtOriginal.Text = original.fblocks[i].text.Replace("\n", "\r\n");
                txtTranslated.Text = translated.fblocks[i].text.Replace("\n", "\r\n");
            }
        }
        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (radioStart.Checked)
                numString.Maximum = translated.sblocks.Length - 1;
            else
                numString.Maximum = translated.fblocks.Length - 1;

            label5.Text = "of " + numString.Maximum;
            numString_ValueChanged(null, null);
        }
        private void txtTranslated_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numString.Value;
            if (radioStart.Checked)
                translated.sblocks[i].text = txtTranslated.Text.Replace("\r\n", "\n");
            else
                translated.fblocks[i].text = txtTranslated.Text.Replace("\r\n", "\n");
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
