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
    public static class SCx
    {
        public static void SC4_Read(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();    // Usually 0x00
            uint unknown4 = br.ReadUInt32();

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            uint size_mapData2 = br.ReadUInt32();   // Is alway the same?
            uint size_mapData = br.ReadUInt32();

            // Create a nitro map file
            NSCR map = new NSCR();
            map.header.file_size = size_mapData;
            map.section.data_size = size_mapData;
            map.section.height = height;
            map.section.width = width;

            map.section.mapData = new NTFS[size_mapData / 2];
            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                ushort parameters = br.ReadUInt16();

                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                map.section.mapData[i].xFlip = (byte)((parameters >> 10) & 1);
                map.section.mapData[i].yFlip = (byte)((parameters >> 11) & 1);
                map.section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
            }

            pluginHost.Set_NSCR(map);
            br.Close();
        }
        public static void SC8_Read(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();    // Usually 0x00
            uint unknown4 = br.ReadUInt32();

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            uint size_mapData2 = br.ReadUInt32();   // Is alway the same?
            uint size_mapData = br.ReadUInt32();

            // Create a nitro map file
            NSCR map = new NSCR();
            map.header.file_size = size_mapData;
            map.section.data_size = size_mapData;
            map.section.height = height;
            map.section.width = width;

            map.section.mapData = new NTFS[size_mapData / 2];
            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                ushort parameters = br.ReadUInt16();

                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                map.section.mapData[i].yFlip = (byte)((parameters >> 10) & 1);
                map.section.mapData[i].xFlip = (byte)((parameters >> 11) & 1);
                map.section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
            }

            pluginHost.Set_NSCR(map);
            br.Close();
        }
    }
}
