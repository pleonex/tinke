// ----------------------------------------------------------------------
// <copyright file="IMY.cs" company="none">

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
// <date>21/04/2012 2:13:37</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace HETALIA
{
    class IMY : ImageBase
    {
        IPluginHost pluginHost;

        public IMY(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = fileName;

            Read(file);
        }


        public override void Read(string fileIn)
        {
            byte[] data = File.ReadAllBytes(fileIn);
            data = Encryption.Image.Decrypt(data);

            int width = BitConverter.ToInt16(data, 8);
            int height = BitConverter.ToInt16(data, 0x0C);
            int num_colors = BitConverter.ToInt16(data, 0xE);
            int img_pos = num_colors * 2 + 0x20;

            // Get image
            byte[] tiles = new byte[width * height];
            Array.Copy(data, img_pos, tiles, 0, tiles.Length);

            ColorFormat format = ColorFormat.colors256;
            if (num_colors == 0x10)
            {
                format = ColorFormat.colors16;
                width *= 2;
            }
            Set_Tiles(tiles, width, height, format, TileForm.Lineal, false);

            // Get palette
            byte[] pal = new byte[num_colors * 2];
            Array.Copy(data, 0x20, pal, 0, pal.Length);
            Color[] colors = Actions.BGR555ToColor(pal);
            RawPalette palette = new RawPalette(new Color[][] { colors }, false, format);

            pluginHost.Set_Palette(palette);
            pluginHost.Set_Image(this);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
        }
    }
}
