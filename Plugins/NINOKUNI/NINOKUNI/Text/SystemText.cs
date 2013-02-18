// ----------------------------------------------------------------------
// <copyright file="SystemText.cs" company="none">

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
// <date>29/04/2012 13:40:46</date>
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
    public partial class SystemText : UserControl
    {
        int id;
        IPluginHost pluginHost;
        string fileName;
        SysText systext;
        SysText systext_old;
        Encoding enc;

        public SystemText()
        {
            InitializeComponent();
        }
        public SystemText(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.id = id;
            this.pluginHost = pluginHost;
            this.fileName = Path.GetFileNameWithoutExtension(file).Substring(12);
            enc = Encoding.GetEncoding("shift_jis");

            systext = Read(file);
            systext_old = Read(file);

            numElement.Maximum = systext.elements.Length - 1;
            lblNum.Text = "of " + numElement.Maximum.ToString();
            numElement_ValueChanged(numElement, null);
        }

        private SysText Read(string file)
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
                s.elements[i].text = Helper.SJISToLatin(s.elements[i].text);
            }

            br.Close();
            return s;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(systext.num_element);

            for (int i = 0; i < systext.num_element; i++)
            {
                systext.elements[i].size = (ushort)enc.GetByteCount(Helper.LatinToSJIS(systext.elements[i].text));

                bw.Write(systext.elements[i].id);
                bw.Write(systext.elements[i].size);
                bw.Write(enc.GetBytes(Helper.LatinToSJIS(systext.elements[i].text)));
            }

            bw.Flush();
            bw.Close();
        }

        private void numElement_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numElement.Value;

            numID.Value = systext.elements[i].id;
            txtOri.Text = systext_old.elements[i].text.Replace("\n", "\r\n");
            txtTrans.Text = systext.elements[i].text.Replace("\n", "\r\n");
        }

        private void txtTrans_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numElement.Value;
            systext.elements[i].text = txtTrans.Text.Replace("\r\n", "\n");
        }
        private void numID_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numElement.Value;
            systext.elements[i].id = (uint)numID.Value;
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
            numElement_ValueChanged(null, null);

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
            XmlElement root = doc.CreateElement("System");

            for (int i = 0; i < systext.elements.Length; i++)
            {
                XmlElement el = doc.CreateElement("String");
                el.SetAttribute("ID", systext.elements[i].id.ToString("x"));

                string text = systext.elements[i].text;
                text = text.Replace('<', '【');
                text = text.Replace('>', '】');
                text = text.Replace("\n", "\n    ");
                if (text.Contains("\n"))
                    text = "\n    " + text + "\n  ";

                el.InnerText = text;
                root.AppendChild(el);
            }

            doc.AppendChild(root);
            doc.Save(o.FileName);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Write(fileOut);
            pluginHost.ChangeFile(id, fileOut);
        }
        public void Import(string file)
        {
            XDocument doc = XDocument.Load(file);
            XElement root = doc.Element("System");

            int i = 0;
            foreach (XElement el in root.Elements("String"))
            {
                systext.elements[i].id = Convert.ToUInt32(el.Attribute("ID").Value, 16);

                string text = el.Value;
                if (text.Contains("\n"))
                {
                    text = text.Remove(0, 5);
                    text = text.Remove(text.Length - 3);
                    text = text.Replace("\n    ", "\n");
                }
                text = text.Replace('【', '<');
                text = text.Replace('】', '>');

                systext.elements[i].text = text;
                i++;
            }
        }

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
