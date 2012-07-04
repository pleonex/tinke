/*
 * Copyright (C) 2012  pleonex
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * Date: 16/03/2012
 *
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona;
using Ekona.Images;

namespace PSL
{
    public partial class GBCS : UserControl
    {
        string gbcs;
        Info[] infos;
        IPluginHost pluginHost;

        public GBCS()
        {
            InitializeComponent();
        }
        public GBCS(string infoPath, string gbcs, IPluginHost pluginHost)
        {
            InitializeComponent();

            Read(infoPath);
            this.gbcs = gbcs;
            this.pluginHost = pluginHost;

            numImg.Maximum = infos.Length - 1;
            label2.Text = "of " + numImg.Maximum.ToString();
            numImg_ValueChanged(null, null);
        }

        private void Read(string infoFile)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(infoFile));

            uint num_imgs = br.ReadUInt32();
            uint unknown = br.ReadUInt32();
            infos = new Info[num_imgs];

            for (int i = 0; i < num_imgs; i++)
            {
                infos[i].offset = br.ReadUInt32();
                infos[i].size = br.ReadUInt32();
            }

            br.Close();
        }

        public struct Info
        {
            public uint offset;
            public uint size;
        }

        private void numImg_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numImg.Value;
            BinaryReader br = new BinaryReader(File.OpenRead(gbcs));
            br.BaseStream.Position = infos[i].offset;

            char[] header = br.ReadChars(4);
            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            Color[][] palette = new Color[1][] { Actions.BGR555ToColor(br.ReadBytes(0x200)) };
            byte[] tiles = br.ReadBytes((int)(infos[i].size - 0x208));
            br.Close();

            RawPalette pal = new RawPalette(palette, false, ColorFormat.colors256);
            RawImage img = new RawImage(tiles, TileForm.Horizontal, ColorFormat.colors256, (int)width, (int)height, false);
            this.Controls.Remove(imgControl);
            this.imgControl = new ImageControl(pluginHost, img, pal);
            this.Controls.Add(imgControl);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder to extract the images";
            o.ShowNewFolderButton = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;
            string folderOut = o.SelectedPath + Path.DirectorySeparatorChar;

            this.Cursor = Cursors.WaitCursor;
            BinaryReader br = new BinaryReader(File.OpenRead(gbcs));
            for (int i = 0; i < infos.Length; i++)
            {
                br.BaseStream.Position = infos[i].offset;

                char[] header = br.ReadChars(4);
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();
                Color[][] palette = new Color[1][] { Actions.BGR555ToColor(br.ReadBytes(0x200)) };
                byte[] tiles = br.ReadBytes((int)(infos[i].size - 0x208));

                RawPalette pal = new RawPalette(palette, false, ColorFormat.colors256);
                RawImage img = new RawImage(tiles, TileForm.Horizontal, ColorFormat.colors256, (int)width, (int)height, false);
                img.Get_Image(pal).Save(folderOut + "Image" + i.ToString() + ".png");
            }
            br.Close();
            this.Cursor = Cursors.Default;
        }
    }
}
