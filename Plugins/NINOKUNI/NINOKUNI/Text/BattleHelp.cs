// ----------------------------------------------------------------------
// <copyright file="BattleHelp.cs" company="none">

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
// <date>28/08/2012 12:23:36</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NINOKUNI.Text
{
    class BattleHelp : IText
    {
        Encoding enc = Encoding.GetEncoding("shift_jis");
        string[] text;

        public void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            ushort num_block = br.ReadUInt16();
            text = new string[num_block];
            
            for (int i = 0; i < num_block; i++)
            {
                text[i] = new String(enc.GetChars(br.ReadBytes(0x80)));
                text[i] = text[i].Replace("\0", "");
                text[i] = text[i].Replace("\\n", "\n");
                text[i] = Helper.SJISToLatin(text[i]);
            }

            br.Close();
            br = null;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((ushort)text.Length);

            byte[] t, d;
            for (int i = 0; i < text.Length; i++)
            {
                t = enc.GetBytes(Helper.LatinToSJIS(text[i].Replace("\n", "\\n")));
                d = new byte[0x80];

                if (t.Length >= 0x80)
                    System.Windows.Forms.MessageBox.Show("Invalid " + i.ToString() + " text. It's so big.");
                else
                    Array.Copy(t, d, t.Length);

                bw.Write(d);
            }
            t = d = null;

            bw.Flush();
            bw.Close();
            bw = null;
        }

        public void Import(string fileIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileIn);

            XmlNode root = doc.GetElementsByTagName("BattleHelp")[0];
            List<string> items = new List<string>();
            for (int i = 0; i < text.Length; i++)
            {
                if (root.ChildNodes[i].NodeType != XmlNodeType.Element)
                    continue;

                string t = root.ChildNodes[i].InnerText;
                t = Helper.Reformat(t, 4, false);
                items.Add(t);
            }
            text = items.ToArray();
            items.Clear();
            items = null;

            root = null;
            doc = null;
        }
        public void Export(string fileOut)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("BattleHelp");
            for (int i = 0; i < text.Length; i++)
            {
                XmlElement e = doc.CreateElement("String");
                e.InnerText = Helper.Format(text[i], 4);
                root.AppendChild(e);
                e = null;
            }

            doc.AppendChild(root);
            root = null;
            doc.Save(fileOut);
            doc = null;
        }

        public string[] Get_Text()
        {
            return text;
        }
    }
}
