/*
 * Copyright (C) 2016
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
 * Plugin by: ccawley2011
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace SONICRUSHADV
{
    class BAC : MapBase
    {
        // BAC format specifications available from http://www.romhacking.net/documents/669/
        IPluginHost pluginHost;
        PaletteBase palette;
        ImageBase image;
        bool isPalette;

        public BAC(string file, int id, IPluginHost pluginHost, string fileName = "") : base(file, id, fileName) { this.pluginHost = pluginHost; }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            uint header = br.ReadUInt32();
            uint block2 = br.ReadUInt32();
            uint block3 = br.ReadUInt32();
            uint block4 = br.ReadUInt32();
            uint block5 = br.ReadUInt32();
            uint block6 = br.ReadUInt32();
            uint block1 = br.ReadUInt32();

            // Palette
            br.BaseStream.Position = block5;
            uint block_size = br.ReadUInt32();
            uint pal_length = br.ReadUInt32();
            uint unknown = pal_length & 0xFF;
            pal_length = pal_length >> 8;
            ColorFormat depth = (pal_length < 0x200 ? ColorFormat.colors16 : ColorFormat.colors256);
            isPalette = false;

            if (pal_length > 0) {
               int num_colors = (depth == ColorFormat.colors16 ? 0x10 : 0x100);
               Color[][] colors = new Color[pal_length / (num_colors * 2)][];
               for (int i = 0; i < colors.Length; i++)
                   colors[i] = Actions.BGR555ToColor(br.ReadBytes((int)(num_colors * 2)));
               palette = new RawPalette(colors, false, depth, fileName);
               isPalette = true;
            }

            int width, height;
                width = 0x100;
                height = 0xc0;

            // Tile data
            br.BaseStream.Position = block6;
            block_size = br.ReadUInt32();
            uint tile_length = br.ReadUInt32();
            uint compression = pal_length & 0xFF;
            tile_length = tile_length >> 8;
            if (compression == 0x10)
               Console.WriteLine("Tiles use LZSS compression");

            Byte[] tiles = br.ReadBytes((int)tile_length);
            image = new RawImage(tiles, TileForm.Horizontal, depth, width, height, false, fileName);

            br.Close();
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public ImageControl Get_Control()
        {
            if (isPalette)
                return new ImageControl(pluginHost, image, palette);
            else
                Console.WriteLine("Image lacks a palette.");
                return null;
        }
    }
}
