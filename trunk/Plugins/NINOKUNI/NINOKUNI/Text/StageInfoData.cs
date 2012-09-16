// ----------------------------------------------------------------------
// <copyright file="StageInfoData.cs" company="none">

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
// <date>29/08/2012 23:51:00</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NINOKUNI.Text
{
    class StageInfoData : IText
    {
        Encoding enc = Encoding.GetEncoding("shift_jis");
        sStageInfoData[] sid;

        public void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            uint num_block = br.ReadUInt32();
            sid = new sStageInfoData[num_block];
            for (int i = 0; i < num_block; i++)
            {
                sid[i].index = br.ReadUInt32();
                uint size = br.ReadUInt32();
                sid[i].text = new String(enc.GetChars(br.ReadBytes((int)size)));
                sid[i].text = Helper.SJISToLatin(sid[i].text.Replace("\0", ""));
            }

            br.Close();
            br = null;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(sid.Length);
            for (int i = 0; i < sid.Length; i++)
            {
                string t = Helper.LatinToSJIS(sid[i].text) + '\0';
                bw.Write(sid[i].index);
                bw.Write(enc.GetByteCount(t));
                bw.Write(enc.GetBytes(t));
            }

            bw.Flush();
            bw.Close();
            bw = null;
        }

        public void Import(string fileIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileIn);

            XmlNode root = doc.GetElementsByTagName("Comment_StageInfoData")[0];
            int j = 0;
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                if (root.ChildNodes[i].NodeType != XmlNodeType.Element || root.ChildNodes[i].Name != "String")
                    continue;

                if (i >= sid.Length)
                    break;

                sid[j++].text = Helper.Reformat(root.ChildNodes[i].InnerText, 4, false);
            }

            root = null;
            doc = null;
        }
        public void Export(string fileOut)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("Comment_StageInfoData");
            for (int i = 0; i < sid.Length; i++)
            {
                XmlElement e = doc.CreateElement("String");
                e.InnerText = Helper.Format(sid[i].text, 4);
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
            string[] t = new string[sid.Length];
            for (int i = 0; i < t.Length; i++) t[i] = sid[i].text;
            return t;
        }

        struct sStageInfoData
        {
            public uint index;
            public string text;
        }
    }
}
