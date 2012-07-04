// ----------------------------------------------------------------------
// <copyright file="USearchControl.cs" company="none">

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
// <date>28/04/2012 10:20:35</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace INAZUMA11
{
    public partial class USearchControl : UserControl
    {
        IPluginHost pluginHost;
        int id;
        USearch[] us;

        public USearchControl()
        {
            InitializeComponent();
        }
        public USearchControl(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.id = id;
            this.pluginHost = pluginHost;

            Read(file);
            numBlock.Maximum = us.Length - 1;
            label2.Text = "of " + numBlock.Maximum.ToString();
            numBlock_ValueChanged(null, null);
        }

        private void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            us = new USearch[br.BaseStream.Length / 0x38];

            for (int i = 0; i < us.Length; i++)
            {
                us[i].text1 = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(0x10)));
                us[i].text2 = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(0x18)));
                us[i].unknown = br.ReadBytes(0x10);
            }

            br.Close();
        }
        private void Write()
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            for (int i = 0; i < us.Length; i++)
            {
                int length = Encoding.GetEncoding("shift_jis").GetByteCount(us[i].text1);
                bw.Write(Encoding.GetEncoding("shift_jis").GetBytes(us[i].text1));
                while (length != 0x10)
                {
                    bw.Write((byte)0x00);
                    length++;
                }

                length = Encoding.GetEncoding("shift_jis").GetByteCount(us[i].text2);
                bw.Write(Encoding.GetEncoding("shift_jis").GetBytes(us[i].text2));
                while (length != 0x18)
                {
                    bw.Write((byte)0x00);
                    length++;
                }

                bw.Write(us[i].unknown);
            }

            bw.Flush();
            bw.Close();
            pluginHost.ChangeFile(id, fileOut);
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Write();
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;

            txtText1.Text = us[i].text1.Replace("\n", "\r\n");
            txtText2.Text = us[i].text2.Replace("\n", "\r\n");
            txtUnk.Text = BitConverter.ToString(us[i].unknown);
        }

        private void txtText1_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            us[i].text1 = txtText1.Text.Replace("\r\n", "\n");
        }
        private void txtText2_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numBlock.Value;
            us[i].text2 = txtText2.Text.Replace("\r\n", "\n");
        }
    }

    public struct USearch
    {
        public string text1;
        public string text2;
        public byte[] unknown;
    }
}
