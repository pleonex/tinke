// ----------------------------------------------------------------------
// <copyright file="BlockText.cs" company="none">

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
// <date>30/08/2012 2:07:37</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NINOKUNI.Text
{
    public class BlockText : IText
    {
        sBlock[] blocks;
        bool hasNumBlock, encoded, wOriginal;
        int textSize, dataSize;
        string fileName;

        public BlockText(bool hasNumBlock, bool encoded, bool wOriginal, int textSize, int dataSize, string fileName)
        {
            this.hasNumBlock = hasNumBlock;
            this.encoded = encoded;
            this.wOriginal = wOriginal;
            this.textSize = textSize;
            this.dataSize = dataSize;
            this.fileName = fileName;
        }

        public void Read(string fileIn)
        {
            if (encoded)
            {
                byte[] d = File.ReadAllBytes(fileIn);
                for (int i = 0; i < d.Length; i++)
                    d[i] = (byte)~d[i];
                File.WriteAllBytes(fileIn, d);
            }

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            uint num_block = 0;
            if (hasNumBlock)
                num_block = br.ReadUInt16();
            else
                num_block = (uint)(br.BaseStream.Length / (textSize + dataSize));

            blocks = new sBlock[num_block];
            for (int i = 0; i < num_block; i++)
            {
                blocks[i].text = Helper.Read_String(br.ReadBytes(textSize));
                blocks[i].data = br.ReadBytes(dataSize);
            }

            br.Close();
            br = null;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            if (hasNumBlock)
                bw.Write((ushort)blocks.Length);

            for (int i = 0; i < blocks.Length; i++)
            {
                bw.Write(Helper.Write_String(blocks[i].text, (uint)textSize));
                bw.Write(blocks[i].data);
            }

            bw.Flush();
            bw.Close();
            bw = null;

            if (encoded)
            {
                byte[] d = File.ReadAllBytes(fileOut);
                for (int i = 0; i < d.Length; i++)
                    d[i] = (byte)~d[i];
                File.WriteAllBytes(fileOut, d);
            }
        }

        public void Import(string fileIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileIn);

            XmlNode root = doc.GetElementsByTagName(fileName.Replace(" ", "_"))[0];
            int i = 0;
            foreach (XmlNode e in root.ChildNodes)
            {
                if (e.NodeType != XmlNodeType.Element || e.Name != "String")
                    continue;

                if (i >= blocks.Length)
                    break;

                blocks[i++].text = Helper.Reformat(e.InnerText, 4, false);
            }

            root = null;
            doc = null;
        }
        public void Export(string fileOut)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement(fileName.Replace(" ", "_"));
            for (int i = 0; i < blocks.Length; i++)
            {
                XmlElement e = doc.CreateElement("String");
                e.InnerText = Helper.Format(blocks[i].text, 4);
                if (wOriginal && !blocks[i].text.Contains('\n'))
                    e.SetAttribute("Original", blocks[i].text);
                root.AppendChild(e);
                e = null;
            }

            doc.AppendChild(root);
            doc.Save(fileOut);
            root = null;
            doc = null;
        }

        public string[] Get_Text()
        {
            string[] t = new string[blocks.Length];
            for (int i = 0; i < blocks.Length; i++) t[i] = blocks[i].text;
            return t;
        }

        struct sBlock
        {
            public string text;
            public byte[] data;
        }
    }
}
