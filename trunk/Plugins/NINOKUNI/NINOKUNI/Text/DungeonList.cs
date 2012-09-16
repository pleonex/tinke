// ----------------------------------------------------------------------
// <copyright file="DungeonList.cs" company="none">

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
// <date>30/08/2012 2:05:39</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NINOKUNI.Text
{
    class DungeonList : IText
    {
        Encoding enc = Encoding.GetEncoding("shift_jis");
        sDungeonList[] dl;

        public void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            ushort num_block = br.ReadUInt16();
            ushort num_subTotal = br.ReadUInt16();
            dl = new sDungeonList[num_block];

            for (int i = 0; i < num_block; i++)
            {
                dl[i].title =Helper.Read_String(br.ReadBytes(0x40));

                ushort num_sub = br.ReadUInt16();
                dl[i].entries = new sDungeonList_Entry[num_sub];
                for (int j = 0; j < num_sub; j++)
                {
                    dl[i].entries[j].text = Helper.Read_String(br.ReadBytes(0x40));
                    dl[i].entries[j].script_name = Helper.Read_String(br.ReadBytes(0x08));
                }
            }

            br.Close();
            br = null;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((ushort)dl.Length);
            ushort num_subTotal = 0;
            for (int i = 0; i < dl.Length; i++) num_subTotal += (ushort)dl[i].entries.Length;
            bw.Write(num_subTotal);

            for (int i = 0; i < dl.Length; i++)
            {
                bw.Write(Helper.Write_String(dl[i].title, 0x40));
                bw.Write((ushort)dl[i].entries.Length);

                for (int j = 0; j < dl[i].entries.Length; j++)
                {
                    bw.Write(Helper.Write_String(dl[i].entries[j].text, 0x40));
                    bw.Write(Helper.Write_String(dl[i].entries[j].script_name, 0x08));
                }
            }

            bw.Flush();
            bw.Close();
            bw = null;
        }

        public void Import(string fileIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileIn);

            XmlNode root = doc.GetElementsByTagName("DungeonList")[0];
            int i = 0;
            foreach (XmlNode b in root.ChildNodes)
            {
                if (b.NodeType != XmlNodeType.Element || b.Name != "Block")
                    continue;

                int j = 0;
                foreach (XmlNode e in b.ChildNodes)
                {
                    if (e.NodeType != XmlNodeType.Element)
                        continue;

                    if (e.Name == "Title") dl[i].title = Helper.Reformat(e.InnerText, 6, false);
                    else if (e.Name == "Message") dl[i].entries[j++].text = Helper.Reformat(e.InnerText, 8, false);
                }
                i++;
            }

            root = null;
            doc = null;
        }
        public void Export(string fileOut)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("DungeonList");
            for (int i = 0; i < dl.Length; i++)
            {
                XmlElement b = doc.CreateElement("Block");
                XmlElement title = doc.CreateElement("Title");
                title.InnerText = Helper.Format(dl[i].title, 6);
                b.AppendChild(title);

                for (int j = 0; j < dl[i].entries.Length; j++)
                {
                    XmlElement msg = doc.CreateElement("Message");
                    msg.InnerText = Helper.Format(dl[i].entries[j].text, 8);
                    msg.SetAttribute("Script", dl[i].entries[j].script_name);
                    b.AppendChild(msg);
                }
                root.AppendChild(b);
            }

            doc.AppendChild(root);
            root = null;
            doc.Save(fileOut);
            doc = null;
        }

        public string[] Get_Text()
        {
            List<string> t = new List<string>();
            for (int i = 0; i < dl.Length; i++)
            {
                for (int j = 0; j < dl[i].entries.Length; j++)
                {
                    string text = "Title (" + i.ToString() + "): " + dl[i].title;
                    text += "\n\nMessage:\n" + dl[i].entries[j].text;
                    text += "\n\nScript: " + dl[i].entries[j].script_name;
                    t.Add(text);
                }
            }

            return t.ToArray();
        }

        struct sDungeonList
        {
            public string title;
            public sDungeonList_Entry[] entries;
        }
        struct sDungeonList_Entry
        {
            public string text;
            public string script_name;
        }
    }
}
