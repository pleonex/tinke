// ----------------------------------------------------------------------
// <copyright file="BlogresControl.cs" company="none">

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
// <date>28/04/2012 13:49:48</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace INAZUMA11
{
    public partial class BlogresControl : UserControl
    {
        IPluginHost pluginHost;
        int id;
        Blogres[] brs;
        Encoding enc;

        public BlogresControl()
        {
            InitializeComponent();
        }
        public BlogresControl(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = id;
            enc = Encoding.GetEncoding("shift_jis");

            Read(file);
            numBlock.Maximum = brs.Length - 1;
            label2.Text = "of " + numBlock.Maximum.ToString();
            numBlock_ValueChanged(null, null);
        }

        private void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            brs = new Blogres[br.BaseStream.Length / 0x108];

            for (int i = 0; i < brs.Length; i++)
            {
                brs[i].unknown1 = br.ReadUInt16();
                brs[i].unknown2 = br.ReadByte();
                brs[i].text = new String(enc.GetChars(br.ReadBytes(0x105)));
            }

            br.Close();
        }
        private void Write()
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            for (int i = 0; i < brs.Length; i++)
            {
                bw.Write(brs[i].unknown1);
                bw.Write(brs[i].unknown2);

                int length = enc.GetByteCount(brs[i].text);
                bw.Write(enc.GetBytes(brs[i].text));
                while (length != 0x105)
                {
                    bw.Write((byte)0x00);
                    length++;
                }
            }

            bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            txtText.Text = brs[i].text.Replace("\n", "\r\n");
            numUnk1.Value = brs[i].unknown1;
            numUnk2.Value = brs[i].unknown2;
        }

        private void numUnk1_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            brs[i].unknown1 = (ushort)numUnk1.Value;
        }
        private void numUnk2_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            brs[i].unknown2 = (byte)numUnk2.Value;
        }
        private void txtText_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            brs[i].text = txtText.Text.Replace("\r\n", "\n");
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Write();
        }
    }

    public struct Blogres
    {
        public ushort unknown1;
        public byte unknown2;
        public string text;
    }
}
