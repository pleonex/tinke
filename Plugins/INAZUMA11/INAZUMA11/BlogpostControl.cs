// ----------------------------------------------------------------------
// <copyright file="BlogpostControl.cs" company="none">

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
// <date>28/04/2012 12:45:55</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace INAZUMA11
{
    public partial class BlogpostControl : UserControl
    {
        IPluginHost pluginHost;
        int id;
        Blogpost[] bps;

        Encoding enc;

        public BlogpostControl()
        {
            InitializeComponent();
        }
        public BlogpostControl(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = id;
            enc = Encoding.GetEncoding("shift_jis");

            Read(file);
            numBlock.Maximum = bps.Length - 1;
            label2.Text = "of " + numBlock.Maximum.ToString();
            numBlock_ValueChanged(null, null);
        }

        private void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            bps = new Blogpost[br.BaseStream.Length / 0x248];

            for (int i = 0; i < bps.Length; i++)
            {
                bps[i].unknown = br.ReadUInt16();
                bps[i].header_text = new String(enc.GetChars(br.ReadBytes(0x42)));
                bps[i].text = new String(enc.GetChars(br.ReadBytes(0x202)));
                bps[i].index = br.ReadUInt16();
            }

            br.Close();
        }
        private void Write()
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            for (int i = 0; i < bps.Length; i++)
            {
                bw.Write(bps[i].unknown);

                int length = enc.GetByteCount(bps[i].header_text);
                bw.Write(enc.GetBytes(bps[i].header_text));
                while (length != 0x42)
                {
                    bw.Write((byte)0x00);
                    length++;
                }

                length = enc.GetByteCount(bps[i].text);
                bw.Write(enc.GetBytes(bps[i].text));
                while (length != 0x202)
                {
                    bw.Write((byte)0x00);
                    length++;
                }

                bw.Write(bps[i].index);
            }

            bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;

            txtText1.Text = bps[i].header_text.Replace("\n", "\r\n");
            txtText2.Text = bps[i].text.Replace("\n", "\r\n");
            numUnk.Value = bps[i].unknown;
            numIndex.Value = bps[i].index;
        }

        private void numUnk_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            bps[i].unknown = (ushort)numUnk.Value;
        }
        private void numIndex_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            bps[i].index = (ushort)numIndex.Value;
        }
        private void txtText1_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            bps[i].header_text = txtText1.Text.Replace("\r\n", "\n");
        }
        private void txtText2_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            bps[i].text = txtText2.Text.Replace("\r\n", "\n");
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Write();
        }
    }

    public struct Blogpost
    {
        public ushort unknown;
        public string header_text;
        public string text;
        public ushort index;
    }
}
