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
using PluginInterface;

namespace SF_FEATHER
{
    public static class CGx
    {
        const ushort WIDTH = 32;

        public static void CG4_Read(string file, IPluginHost pluginHost)
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

            // Create the structure of a nitro tile file
            NCGR tile = new NCGR();
            tile.header.file_size = size_tiles;
            tile.order = TileOrder.Horizontal;
            tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            tile.rahc.size_tiledata = size_tiles;
            tile.rahc.nTilesX = WIDTH;
            tile.rahc.nTilesY = (ushort)(num_tiles / WIDTH);
            if (tile.rahc.nTilesY == 0)
                tile.rahc.nTilesY = 1;

            tile.rahc.nTiles = num_tiles;
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPalette = new byte[tile.rahc.nTiles];

            // Read tiles
            br.BaseStream.Position = tileOffset;
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
            {
                tile.rahc.tileData.tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(32));
            }

            // Create the structure of a nitro palette file
            br.BaseStream.Position = palOffset;
            NCLR palette = new NCLR();
            palette.header.file_size = palColors * 2;
            palette.pltt.nColors = palColors;
            palette.pltt.palettes = new NTFP[palColors / 0x10];
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                palette.pltt.palettes[i].colors = pluginHost.BGR555(br.ReadBytes(32));

            br.Close();

            pluginHost.Set_NCGR(tile);
            pluginHost.Set_NCLR(palette);
        }
        public static void CG8_Read(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);  // CG8 
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

            // Create the structure of a nitro tile file
            NCGR tile = new NCGR();
            tile.header.file_size = size_tiles;
            tile.order = TileOrder.Horizontal;
            tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            tile.rahc.size_tiledata = size_tiles;
            tile.rahc.nTilesX = WIDTH;
            tile.rahc.nTilesY = (ushort)(num_tiles / WIDTH);
            if (tile.rahc.nTilesY == 0)
                tile.rahc.nTilesY = 1;

            tile.rahc.nTiles = num_tiles;
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPalette = new byte[tile.rahc.nTiles];

            // Read tiles
            br.BaseStream.Position = tileOffset;
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
            {
                tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
            }

            // Create the structure of a nitro palette file
            br.BaseStream.Position = palOffset;
            NCLR palette = new NCLR();
            palette.header.file_size = palColors * 2;
            palette.pltt.nColors = palColors;
            palette.pltt.palettes = new NTFP[1];
            palette.pltt.palettes[0].colors = pluginHost.BGR555(br.ReadBytes((int)palColors * 2));

            br.Close();

            pluginHost.Set_NCGR(tile);
            pluginHost.Set_NCLR(palette);
        }

    }
}
