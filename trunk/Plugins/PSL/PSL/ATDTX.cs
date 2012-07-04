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
    public partial class ATDTX : UserControl
    {
        IPluginHost pluginHost;
        int id;

        sFile[] imgs_;
        string[] imgs;

        sFile[] palettes_;
        string[] palettes;
        PaletteBase palette;

        public ATDTX()
        {
            InitializeComponent();
        }
        public ATDTX(IPluginHost pluginHost, int id)
        {
            InitializeComponent();
            this.pluginHost = pluginHost; ;
            this.id = id;

            imgs_ = LINK.Unpack(pluginHost.Search_File(id)).files.ToArray();
            imgs = new string[imgs_.Length];
            numFile.Maximum = imgs_.Length - 1;
            label3.Text = numFile.Maximum.ToString();

            palettes_ = LINK.Unpack(pluginHost.Search_File(0x1DC)).files.ToArray();
            palettes = new string[palettes_.Length];

            numFile_ValueChanged(null, null);
        }

        string Save_File(sFile file)
        {
            string outFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = file.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)file.size));
            br.Close();

            return outFile;
        }

        private void numFile_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numFile.Value;
            if (palettes[i] == null)
                palettes[i] = Save_File(palettes_[i]);

            BinaryReader br = new BinaryReader(File.OpenRead(palettes[i]));
            br.BaseStream.Position = 0x3340;
            Color[][] pal = new Color[1][] { Actions.BGR555ToColor(br.ReadBytes(0x40)) };
            br.Close();

            palette = new RawPalette(pal, false, ColorFormat.colors16);

            if (imgs[i] == null)
                imgs[i] = Save_File(imgs_[i]);

            numSprite.Maximum = (new FileInfo(imgs[i]).Length / 0x200) - 1;
            label4.Text = "of " + numSprite.Maximum.ToString();
            if (numSprite.Value != 0)
                numSprite.Value = 0;
            else
                numSprite_ValueChanged(null, null);
        }

        private void numSprite_ValueChanged(object sender, EventArgs e)
        {
            int i = (int)numFile.Value;
            int j = (int)numSprite.Value;

            BinaryReader br = new BinaryReader(File.OpenRead(imgs[i]));
            br.BaseStream.Position = j * 0x200;
            byte[] tiles = br.ReadBytes(0x200);
            br.Close();

            ImageBase image = new RawImage(tiles, TileForm.Lineal, ColorFormat.colors16, 0x20, 0x20, false);
            this.Controls.Remove(imageControl1);
            imageControl1 = new ImageControl(pluginHost, image, palette);
            this.Controls.Add(imageControl1);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder to save the sprites.";
            o.ShowNewFolderButton = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            this.Cursor = Cursors.WaitCursor;
            string folderOut = o.SelectedPath + Path.DirectorySeparatorChar;

            for (int i = 0; i < imgs.Length; i++)
            {
                if (palettes[i] == null)
                    palettes[i] = Save_File(palettes_[i]);

                BinaryReader br = new BinaryReader(File.OpenRead(palettes[i]));
                br.BaseStream.Position = 0x3340;
                Color[][] pal = new Color[1][] { Actions.BGR555ToColor(br.ReadBytes(0x40)) };
                br.Close();

                palette = new RawPalette(pal, false, ColorFormat.colors16);

                if (imgs[i] == null)
                    imgs[i] = Save_File(imgs_[i]);

                uint max_imgs = (uint)(new FileInfo(imgs[i]).Length / 0x200);

                
                br = new BinaryReader(File.OpenRead(imgs[i]));
                for (int j = 0; j < max_imgs; j++)
                {
                    br.BaseStream.Position = j * 0x200;
                    byte[] tiles = br.ReadBytes(0x200);

                    ImageBase image = new RawImage(tiles, TileForm.Lineal, ColorFormat.colors16, 0x20, 0x20, false);
                    image.Get_Image(palette).Save(folderOut + "Sprite" + i.ToString() + '_' + j.ToString() + ".png");
                }
                br.Close();

            }
            this.Cursor = Cursors.Default;
        }
    }
}
