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
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace KIRBY_DRO
{
    class Bin : MapBase
    {
        IPluginHost pluginHost;
        PaletteBase palette;
        ImageBase image;
        bool isMap;

        public Bin(string file, int id, IPluginHost pluginHost, string fileName = "") : base(file, id, fileName) { this.pluginHost = pluginHost; }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            uint header = br.ReadUInt32();

            uint pal_length = br.ReadUInt32();
            uint tile_length = br.ReadUInt32();
            uint map_length = br.ReadUInt32();
            ColorFormat depth = (pal_length < 0x200 ? ColorFormat.colors16 : ColorFormat.colors256);
            isMap = true;

            int width, height;
            if (header == 0x18)
            {
                width = (ushort)br.ReadUInt32();
                height = (ushort)br.ReadUInt32();
            }
            else
            {
                width = 0x200;
                height = 0xc0;
                isMap = false;
            }

            // Palette
            int num_colors = (depth == ColorFormat.colors16 ? 0x10 : 0x100);
            Color[][] colors = new Color[pal_length / (num_colors * 2)][];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Actions.BGR555ToColor(br.ReadBytes((int)(num_colors * 2)));
            palette = new RawPalette(colors, false, depth, fileName);

            // Tile data
            Byte[] tiles = br.ReadBytes((int)tile_length);
            image = new RawImage(tiles, TileForm.Horizontal, depth, width, height, false, fileName);

            // Map
            if (isMap)
            {
                NTFS[] maps = new NTFS[map_length / 2];
                for (int i = 0; i < maps.Length; i++)
                    maps[i] = Actions.MapInfo(br.ReadUInt16());
                Set_Map(maps, false);
            }

            br.Close();
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public ImageControl Get_Control()
        {
            if (isMap)
                return new ImageControl(pluginHost, image, palette, this);
            else
                return new ImageControl(pluginHost, image, palette);
        }
    }
}
