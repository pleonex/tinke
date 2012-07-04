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
    public partial class TexSprites : UserControl
    {
        string images;
        Info[] infos;
        IPluginHost pluginHost;

        public TexSprites()
        {
            InitializeComponent();
        }
        public TexSprites(IPluginHost pluginHost, string imagefile, string infofile)
        {
            if (!pluginHost.Get_Sprite().Loaded)
                throw new FileLoadException("Please, load first the NCER file");

            InitializeComponent();

            images = imagefile;
            ReadInfo(infofile);
            this.pluginHost = pluginHost;

            numImg.Maximum = infos.Length - 1;
            label2.Text = "of " + (infos.Length - 1).ToString();
            numImg_ValueChanged(null, null);
        }

        private void ReadInfo(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            uint num_imgs = br.ReadUInt32();
            uint img_size = br.ReadUInt32();    // Not needed...

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
            int offset = (int)infos[(int)numImg.Value].offset;
            int size = (int)infos[(int)numImg.Value].size;
            TileForm form = (images.EndsWith("Tex.dat") ? TileForm.Lineal : TileForm.Horizontal);

            RawImage image = new RawImage(images, -1, form, ColorFormat.colors256, false,
                offset + 0x220, size - 0x220);
            pluginHost.Set_Image(image);

            RawPalette palette = new RawPalette(images, -1, false,
                offset + 0x20, 0x200);
            pluginHost.Set_Palette(palette);

            this.Controls.Remove(spriteControl1);
            spriteControl1 = new SpriteControl(pluginHost);
            this.Controls.Add(spriteControl1);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            string folder;
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder where you want\nto extract all the images.";
            o.ShowNewFolderButton = true;
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            folder = o.SelectedPath + Path.DirectorySeparatorChar;

            SpriteBase sprite = pluginHost.Get_Sprite();
            this.Cursor = Cursors.WaitCursor;
            for (int i = 0; i < infos.Length; i++)
            {
                int offset = (int)infos[i].offset;
                int size = (int)infos[i].size;
                TileForm form = (images.EndsWith("Tex.dat") ? TileForm.Lineal : TileForm.Horizontal);

                RawImage image = new RawImage(images, -1, form, ColorFormat.colors256, false,
                    offset + 0x220, size - 0x220);

                RawPalette palette = new RawPalette(images, -1, false,
                    offset + 0x20, 0x200);

                sprite.Get_Image(image, palette, 0, 512, 256, false, false, false, true, true).Save(folder + "Image" + i.ToString() + ".png");
            }
            this.Cursor = Cursors.Default;
        }

    }
}
