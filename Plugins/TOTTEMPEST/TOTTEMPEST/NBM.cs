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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace TOTTEMPEST
{
    public static class NBM
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            uint file_size = (uint)(new FileInfo(file).Length);
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCLR nclr = new NCLR();
            NCGR ncgr = new NCGR();

            // Read header values
            ushort depth = br.ReadUInt16();
            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            ushort tileFlag = br.ReadUInt16();

            // Palette
            // Common header
            nclr.header.id = "NBM ".ToCharArray();
            nclr.header.constant = 0x0100;
            nclr.header.file_size = (uint)(depth == 0x01 ? 0x100 : 0x10);
            nclr.header.header_size = 0x02;
            // TTLP section
            nclr.pltt.ID = "NBM ".ToCharArray();
            nclr.pltt.length = nclr.header.file_size;
            nclr.pltt.depth = (depth == 0x01 ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit);
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.paletteLength = (uint)(depth == 0x01 ? 0x200 : 0x20);
            nclr.pltt.nColors = (uint)(depth == 0x01 ? 0x100 : 0x10);
            nclr.pltt.palettes = new NTFP[1];
            // Get colors
            for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                nclr.pltt.palettes[i].colors = pluginHost.BGR555(br.ReadBytes((int)nclr.pltt.paletteLength));

            // Tiles
            // Common header
            ncgr.id = (uint)id;
            ncgr.header.id = "NBM ".ToCharArray();
            ncgr.header.nSection = 1;
            ncgr.header.constant = 0x0100;
            ncgr.header.file_size = file_size;

            // RAHC section
            ncgr.order = (tileFlag == 0x00 ? TileOrder.NoTiled : TileOrder.Horizontal);
            ncgr.rahc.nTiles = (ushort)(width * height / 0x40);
            ncgr.rahc.depth = (depth == 0x01 ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit);

            ncgr.rahc.nTilesX = (ushort)(width);
            ncgr.rahc.nTilesY = (ushort)(height);

            ncgr.rahc.tiledFlag = tileFlag;
            ncgr.rahc.size_section = file_size;

            // Tile data
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPalette = new byte[1];
            ncgr.rahc.tileData.tiles = new byte[1][];

            if (ncgr.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                ncgr.rahc.tileData.tiles[0] = pluginHost.BytesTo4BitsRev(br.ReadBytes(width * height / 2));
            else
                ncgr.rahc.tileData.tiles[0] = br.ReadBytes(width * height);
            ncgr.rahc.tileData.nPalette[0] = 0;

            br.Close();
            pluginHost.Set_NCLR(nclr);
            pluginHost.Set_NCGR(ncgr);
        }
        public static void Write(NCLR palette, NCGR tile, string fileout, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Header
            bw.Write((ushort)(tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth8Bit ? 0x01 : 0x00));
            bw.Write(tile.rahc.nTilesX);
            bw.Write(tile.rahc.nTilesY);
            bw.Write((ushort)0x00);  // Tiled flag?, always 0x00 and no tiled

            // Palette section
            bw.Write(pluginHost.ColorToBGR555(palette.pltt.palettes[0].colors));
            if (palette.pltt.depth == System.Windows.Forms.ColorDepth.Depth4Bit) // If 4bpp there is an unused extra palette
                bw.Write(extraPaletteData);

            // Tile section
            if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                bw.Write(pluginHost.Bit4ToBit8(tile.rahc.tileData.tiles[0]));
            else
                bw.Write(tile.rahc.tileData.tiles[0]);

            bw.Flush();
            bw.Close();
        }

        private static byte[] extraPaletteData = {
	        0x44, 0x02, 0x00, 0x42, 0x24, 0x00, 0x20, 0x44, 0x44, 0x02, 0x42, 0x44,
	        0x44, 0x24, 0x00, 0x20, 0x44, 0x02, 0x42, 0x44, 0x44, 0x24, 0x00, 0x42,
	        0x44, 0x02, 0x42, 0x44, 0x44, 0x24, 0x20, 0x44, 0x44, 0x02, 0x20, 0x44,
	        0x44, 0x02
            };

    }
}
