// ----------------------------------------------------------------------
// <copyright file="SubtitlesControl.cs" company="none">

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
// <date>28/04/2012 0:28:33</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace INAZUMA11
{
    public partial class SubtitlesControl : UserControl
    {
        int id;
        IPluginHost pluginHost;
        List<Subtitle> subs;
        List<Subtitle> subsNew;

        public SubtitlesControl()
        {
            InitializeComponent();
        }
        public SubtitlesControl(string fileIn, IPluginHost pluginHost, int id)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = id;

            subs = Read(fileIn);
            subsNew = Read(fileIn);

            numSub.Maximum = subsNew.Count - 1;
            label8.Text = "of " + numSub.Maximum.ToString();
            numSub_ValueChanged(null, null);
        }

        public List<Subtitle> Read(string fileIn)
        {
            List<Subtitle> subs = new List<Subtitle>();
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            while (br.BaseStream.Length > br.BaseStream.Position)
            {
                Subtitle sub = new Subtitle();
                sub.time_start = br.ReadUInt32();
                if (sub.time_start == 0xFFFFFFFF)
                    break;

                sub.time_end = br.ReadUInt32();
                uint length = br.ReadUInt32();  // It isn't always correct

                byte[] b = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    b[i] = br.ReadByte();
                    if (b[i] == 0x00)
                        break;
                }
                if ((br.BaseStream.Position % 4) != 0)
                    while ((br.BaseStream.Position % 4) != 0)
                        br.ReadByte();
                sub.text = new String(Encoding.GetEncoding("shift_jis").GetChars(b)).Replace("\0", "");

                subs.Add(sub);
            }

            br.Close();
            return subs;
        }
        public void Write()
        {
            string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            for (int i = 0; i < subsNew.Count; i++)
            {
                bw.Write(subsNew[i].time_start);
                bw.Write(subsNew[i].time_end);

                uint length = (uint)Encoding.GetEncoding("shift_jis").GetByteCount(subsNew[i].text);
                length += 4 - (length % 4);
                bw.Write(length);
                bw.Write(Encoding.GetEncoding("shift_jis").GetBytes(subsNew[i].text));
                bw.Write(new byte[4 - (bw.BaseStream.Position % 4)]);
            }
            bw.Write(0xFFFFFFFF);

            bw.Flush();
            bw.Close();

            pluginHost.ChangeFile(id, fileOut);
        }

        private void numSub_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numSub.Value;

            txtOld.Text = subs[i].text.Replace("\n", "\r\n");
            txtNew.Text = subsNew[i].text.Replace("\n", "\r\n");

            numStartTime.Value = subsNew[i].time_start;
            numEndTime.Value = subsNew[i].time_end;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            Write();
        }

        private void txtNew_TextChanged(object sender, EventArgs e)
        {
            int i = (int)numSub.Value;

            Subtitle sub = subsNew[i];
            sub.text = txtNew.Text.Replace("\r\n", "\n");
            subsNew[i] = sub;
        }
        private void numStartTime_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numSub.Value;

            Subtitle sub = subsNew[i];
            sub.time_start = (uint)numStartTime.Value;
            subsNew[i] = sub;
        }
        private void numEndTime_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numSub.Value;

            Subtitle sub = subsNew[i];
            sub.time_end = (uint)numEndTime.Value;
            subsNew[i] = sub;
        }

    }

    public struct Subtitle
    {
        public uint time_start;
        public uint time_end;
        public string text;
    }
}
