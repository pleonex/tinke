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

namespace _999HRPERDOOR
{
    public static class SIR0
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            SIR0_Info info = new SIR0_Info();
            NCLR palette = new NCLR();
            NCGR tiles = new NCGR();
            NCER cells = new NCER();

            // Read header
            char[] file_id = br.ReadChars(4);
            uint offset1 = br.ReadUInt32();
            uint offset2 = br.ReadUInt32();
            uint unknown = br.ReadUInt32();
            String name = "";
            do
            {
                byte c = br.ReadByte();
                if (c == 0xAA)
                    c = 0x00;
                name += (char)c;
            } while (name[name.Length - 1] != '\x0');

            // Read info section
            // Info 1
            br.BaseStream.Position = offset1;
            info.info1 = new SIR0_Info.Info1();
            info.info1.constant = br.ReadUInt32();
            info.info1.reserved = br.ReadBytes(0x3C);
            info.info1.info3_offset = br.ReadUInt32();
            info.info1.reserved2 = br.ReadBytes((int)(offset2 - offset1));
            // Info 2
            br.BaseStream.Position = offset2;
            info.info2 = new SIR0_Info.Info2();
            info.info2.unknown = br.ReadBytes(0x10);
            // Info 3
            br.BaseStream.Position = info.info1.info3_offset;
            info.info3 = new SIR0_Info.Info3();
            info.info3.tile_size = br.ReadUInt32();
            info.info3.unknown1 = br.ReadUInt32();
            info.info3.unknown2 = br.ReadUInt32();
            info.info3.unknown3 = br.ReadUInt32();
            info.info3.unknown4 = br.ReadUInt32();
            info.info3.palette_offset = br.ReadUInt32();
            info.info3.tile_offset = br.ReadUInt32();
            info.info3.cell_offset = br.ReadUInt32();
            info.info3.unknown5 = br.ReadUInt32();
            info.info3.reserved = br.ReadBytes(0x3C);

            // Read palette
            br.BaseStream.Position = info.info3.palette_offset;
            palette.header.file_size = 0x200;
            palette.header.id = "SIR0".ToCharArray();
            palette.id = (uint)id;
            palette.pltt.ID = "SIR0".ToCharArray();
            palette.pltt.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            palette.pltt.nColors = 0x100;
            palette.pltt.paletteLength = 0x200;
            palette.pltt.length = 0x200;
            palette.pltt.palettes = new NTFP[1];
            palette.pltt.palettes[0].colors = pluginHost.BGR555(br.ReadBytes(0x200));

            // Read tiles
            br.BaseStream.Position = info.info3.tile_offset;
            tiles.id = (uint)id;
            tiles.header.id = "SIR0".ToCharArray();
            tiles.rahc.id = "SIR0".ToCharArray();
            tiles.header.file_size = info.info3.tile_size;
            tiles.rahc.size_section = info.info3.tile_size;
            tiles.rahc.size_tiledata = info.info3.tile_size;
            tiles.order = TileOrder.NoTiled;
            tiles.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            tiles.rahc.nTiles = info.info3.tile_size;
            tiles.rahc.nTilesX = 0x40;
            tiles.rahc.nTilesY = (ushort)(tiles.rahc.nTiles / tiles.rahc.nTilesX);
            tiles.rahc.tileData.nPalette = new byte[info.info3.tile_size];
            tiles.rahc.tileData.tiles = new byte[1][];
            tiles.rahc.tileData.tiles[0] = br.ReadBytes((int)info.info3.tile_size);
            for (int i = 0; i < info.info3.tile_size; i++)
                tiles.rahc.tileData.nPalette[i] = 0;

            // Read cell info
            uint cell_size;
            if (info.info3.unknown5 != 0x00)
                cell_size = info.info3.unknown5 - info.info3.cell_offset - 0x06;
            else
                cell_size = info.info1.info3_offset - info.info3.cell_offset - 0x06;
            br.BaseStream.Position = info.info3.cell_offset;
            cells.id = (uint)id;
            cells.header.id = "SIR0".ToCharArray();
            cells.cebk.id = "SIR0".ToCharArray();
            cells.header.file_size = cell_size;
            cells.cebk.section_size = cell_size;
            cells.cebk.block_size = 1;
            cells.cebk.nBanks = 1;
            cells.cebk.tBank = 0;
            cells.cebk.banks = new Bank[1];
            cells.cebk.banks[0].nCells = (ushort)(cell_size / 0x0A);
            cells.cebk.banks[0].cells = new Cell[cells.cebk.banks[0].nCells];
            for (int i = 0; i < cells.cebk.banks[0].nCells; i++)
            {
                cells.cebk.banks[0].cells[i].width = br.ReadUInt16();
                cells.cebk.banks[0].cells[i].height = br.ReadUInt16();
                cells.cebk.banks[0].cells[i].xOffset = br.ReadUInt16() - 0x80;
                cells.cebk.banks[0].cells[i].yOffset = br.ReadUInt16() - 0x80;
                cells.cebk.banks[0].cells[i].tileOffset = (uint)(br.ReadUInt16() / 0x20);
                cells.cebk.banks[0].cells[i].num_cell = (ushort)i;
            }

            br.Close();
            pluginHost.Set_NCLR(palette);
            pluginHost.Set_NCGR(tiles);
            pluginHost.Set_NCER(cells);
        }
    }

    public struct SIR0_Info
    {
        public Info1 info1;
        public Info2 info2;
        public Info3 info3;

        public struct Info1
        {
            public uint constant; // 0x10
            public byte[] reserved; // 0x3C bytes
            public uint info3_offset;
            public byte[] reserved2; // 0x7C bytes
        }
        public struct Info2
        {
            public byte[] unknown; // 0x10 bytes
        }
        public struct Info3
        {
            public uint cell_offset;
            public uint tile_size;
            public uint tile_offset;
            public uint palette_offset;
            public uint unknown1;
            public uint unknown2;
            public uint unknown3;
            public uint unknown4;
            public uint unknown5;
            public byte[] reserved; // 0x3C bytes
        }
    }
}
