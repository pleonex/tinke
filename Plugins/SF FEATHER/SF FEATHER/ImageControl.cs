/*
 * Copyright (C) 2011  pleoNeX
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
 * By: pleoNeX
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
using PluginInterface;

namespace SF_FEATHER
{
    public partial class ImageControl : UserControl
    {
        IPluginHost pluginHost;
        NCLR palette;
        NCGR tile;
        NSCR map;
        bool isMap;

        public ImageControl()
        {
            InitializeComponent();
        }
        public ImageControl(IPluginHost pluginHost, bool isMap)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.isMap = isMap;

            palette = pluginHost.Get_NCLR();
            tile = pluginHost.Get_NCGR();

            if (isMap)
                map = pluginHost.Get_NSCR();

            UpdateImage();

            numericWidth.Value = tile.rahc.nTilesX * 8;
            numericHeight.Value = tile.rahc.nTilesY * 8;
            numericHeight.ValueChanged += new EventHandler(ChangeSize);
            numericWidth.ValueChanged += new EventHandler(ChangeSize);
        }

        private void UpdateImage()
        {
            if (isMap)
            {
                NCGR newTile = tile;
                newTile.rahc.nTilesX = (ushort)(map.section.width / 8);
                newTile.rahc.nTilesY = (ushort)(map.section.height / 8);
                newTile.rahc.tileData = pluginHost.Transform_NSCR(map, tile.rahc.tileData);
                picBox.Image = pluginHost.Bitmap_NCGR(newTile, palette);
            }
            else
                picBox.Image = pluginHost.Bitmap_NCGR(tile, palette);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp|" +
                       "Portable Network Graphic (*.png)|*.png|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
            {
                Bitmap image = (Bitmap)picBox.Image;
                if (o.FilterIndex == 1)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 2)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 3)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
            o.Dispose();

        }

        private void ChangeSize(object sender, EventArgs e)
        {
            tile.rahc.nTilesX = (ushort)(numericWidth.Value / 8);
            tile.rahc.nTilesY = (ushort)(numericHeight.Value / 8);

            UpdateImage();
        }
    }
}
