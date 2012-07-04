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
 *   By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace TOTTEMPEST
{
    public class ANA : ImageBase
    {
        IPluginHost pluginHost;

        public ANA(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            ushort num_tiles = br.ReadUInt16();
            ushort depth = br.ReadUInt16();
            ColorFormat format = (depth == 0x01 ? ColorFormat.colors256 : ColorFormat.colors16);
            if (br.BaseStream.Length - 4 == num_tiles * 0x40)
                format = ColorFormat.colors256;

            Byte[] tiles = br.ReadBytes(num_tiles * (0x20 + 0x20 * depth));

            int width = 64;
            if (num_tiles == 0x10)
                width = 32;
            int height = tiles.Length / width;
            if (depth == 0)
                height *= 2;

            br.Close();
            Set_Tiles(tiles, width, height, format, TileForm.Horizontal, true);
            pluginHost.Set_Image(this);

            // If the image is 8bpp, convert to 8bpp the palette (4bpp per default)
            PaletteBase palette = pluginHost.Get_Palette();
            if (palette.Loaded && format == ColorFormat.colors256)
            {
                palette.Depth = ColorFormat.colors256;
                pluginHost.Set_Palette(palette);
            }
        }
        public override void Write(string fileout, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            ushort num_tiles = (ushort)(Tiles.Length / 0x20);
            if (FormatColor == ColorFormat.colors256)
                num_tiles /= 2;

            bw.Write(num_tiles);
            bw.Write((ushort)(FormatColor == Ekona.Images.ColorFormat.colors16 ? 0x00 : 0x01));
            bw.Write(Tiles);

            bw.Flush();
            bw.Close();
        }
    }
}
