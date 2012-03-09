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
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;
using PluginInterface.Images;

namespace SF_FEATHER
{
    public class CGx : ImageBase
    {
        const ushort WIDTH = 32;
        ColorFormat depth;

        public CGx(IPluginHost pluginHost, string file, int id, bool cg8)
            : base(pluginHost)
        {
            this.id = id;
            this.fileName = Path.GetFileName(file);

            depth = cg8 ? ColorFormat.colors256 : PluginInterface.Images.ColorFormat.colors16;
            Read(file);
        }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);  // CG4 
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();    // Usually 0
            uint unknown4 = br.ReadUInt32();    // Usually 0

            uint size_tiles = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();
            uint num_tiles = br.ReadUInt32();

            uint palColors = br.ReadUInt32();
            uint tileOffset = br.ReadUInt32();
            uint palOffset = br.ReadUInt32();
            uint mapOffset = br.ReadUInt32();    // If 0, it doesn't exist

            // Read tiles
            br.BaseStream.Position = tileOffset;
            int tile_size = (depth == ColorFormat.colors16 ? 0x20 : 0x40);
            Byte[] tiles = br.ReadBytes((int)num_tiles * tile_size);
            Set_Tiles(tiles, WIDTH, (int)(tiles.Length / WIDTH), depth, TileForm.Horizontal, false);
            if (depth == PluginInterface.Images.ColorFormat.colors16)
                Height *= 2;

            // Read palette
            br.BaseStream.Position = palOffset;
            Color[][] colors;
            if (depth == PluginInterface.Images.ColorFormat.colors16)
            {
                colors = new Color[palColors / 0x10][];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = pluginHost.BGR555ToColor(br.ReadBytes(32));
            }
            else
            {
                colors = new Color[1][];
                colors[0] = pluginHost.BGR555ToColor(br.ReadBytes((int)palColors * 2));
            }
            PaletteBase palette = new RawPalette(pluginHost, colors, false, depth);

            br.Close();

            pluginHost.Set_Palette(palette);
            pluginHost.Set_Image(this);
        }
        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }

    public class CGT : ImageBase
    {
        bool transparency;

        public CGT(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] id = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();    // Usually is 0x00
            uint unknown2 = br.ReadUInt32();    // Usually is 0x00
            uint unknown3 = br.ReadUInt32();    // Usually is 0x00

            uint format = br.ReadUInt32();
            uint tile_size = br.ReadUInt32();
            uint header_size = br.ReadUInt32();
            uint palette_size = br.ReadUInt32();

            uint palette_offset = br.ReadUInt32();
            uint unknown4 = br.ReadUInt32();
            uint unknown_offset = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();

            transparency = (br.ReadUInt32() == 0x00 ? false : true);
            transparency = false;

            int width = (int)Math.Pow(2, (double)(br.ReadUInt16() + 3));
            int height = (int)Math.Pow(2, (double)(br.ReadUInt16() + 3));
            uint unknown6 = br.ReadUInt32();

            byte[] data = br.ReadBytes((int)tile_size);
            

            br.BaseStream.Position = palette_offset;
            Color[][] palette = new Color[1][];
            palette[0] = pluginHost.BGR555ToColor(br.ReadBytes((int)palette_size));

            br.Close();

            Set_Tiles(data, width, height, (ColorFormat)format, TileForm.Lineal, false);
            PaletteBase pb = new RawPalette(pluginHost, palette, false, ColorFormat.colors256);
            pluginHost.Set_Palette(pb);
            pluginHost.Set_Image(this);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
