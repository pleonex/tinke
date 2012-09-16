// ----------------------------------------------------------------------
// <copyright file="DownloadParam.cs" company="none">

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
// <date>30/08/2012 1:52:18</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace NINOKUNI.Text
{
    class DownloadParam : IText
    {
        Encoding enc = Encoding.GetEncoding("shift_jis");
        sDownloadParam[] dp;

        public void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            ushort num_block = br.ReadUInt16();
            dp = new sDownloadParam[num_block];
            for (int i = 0; i < num_block; i++)
            {
                dp[i].unk1 = br.ReadUInt16();
                dp[i].unk2 = br.ReadUInt16();
                dp[i].text = new String(enc.GetChars(br.ReadBytes(0x30)));
                dp[i].text = Helper.SJISToLatin(dp[i].text.Replace("\0", ""));
            }

            br.Close();
            br = null;
        }
        public void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((ushort)dp.Length);
            byte[] d, t;
            for (int i = 0; i < dp.Length; i++)
            {
                bw.Write(dp[i].unk1);
                bw.Write(dp[i].unk2);

                d = enc.GetBytes(Helper.LatinToSJIS(dp[i].text));
                t = new byte[0x30];
                if (d.Length >= 0x30)
                    System.Windows.Forms.MessageBox.Show("Invalid " + i.ToString() + " text. It's so big");
                else
                    Array.Copy(d, t, d.Length);

                bw.Write(t);
            }
            d = t = null;

            bw.Flush();
            bw.Close();
            bw = null;
        }

        public void Import(string fileIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileIn);

            XmlNode root = doc.GetElementsByTagName("DownloadParam")[0];
            int i = 0;
            foreach (XmlNode e in root.ChildNodes)
            {
                if (e.NodeType != XmlNodeType.Element || e.Name != "String")
                    continue;

                dp[i++].text = Helper.Reformat(e.InnerText, 4, false);
            }

            root = null;
            doc = null;
        }
        public void Export(string fileOut)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = doc.CreateElement("DownloadParam");
            for (int i = 0; i < dp.Length; i++)
            {
                XmlElement e = doc.CreateElement("String");
                e.InnerText = Helper.Format(dp[i].text, 4);
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
            string[] t = new string[dp.Length];
            for (int i = 0; i < t.Length; i++) t[i] = dp[i].text;
            return t;
        }

        struct sDownloadParam
        {
            public ushort unk1;
            public ushort unk2;      // ID?
            public string text;    // 0x30 bytes
        }
    }
}
