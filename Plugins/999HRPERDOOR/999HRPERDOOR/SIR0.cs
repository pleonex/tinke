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
using Ekona;
using Ekona.Images;

namespace _999HRPERDOOR
{
    public class SIR0_Sprite : SpriteBase
    {
        SIR0_Info info;

        public SIR0_Sprite(IPluginHost pluginHost, string file, int id, string fileName)
            : base(file, id, pluginHost, fileName)
        {
        }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            info = new SIR0_Info();

            PaletteBase palette;
            ImageBase image;
            Ekona.Images.Bank bank;

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
            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes(0x200));
            palette = new RawPalette(colors, false, ColorFormat.colors256, "");

            // Read tiles
            br.BaseStream.Position = info.info3.tile_offset;
            byte[] tiles = new byte[info.info3.tile_size];
            tiles = br.ReadBytes((int)info.info3.tile_size);
            image = new RawImage(tiles, TileForm.Lineal, ColorFormat.colors256, 0x40,
                (int)(info.info3.tile_size / 0x40), false, "");

            // Read cell info
            uint bank_size;
            if (info.info3.unknown5 != 0x00)
                bank_size = info.info3.unknown5 - info.info3.cell_offset - 0x06;
            else
                bank_size = info.info1.info3_offset - info.info3.cell_offset - 0x06;

            br.BaseStream.Position = info.info3.cell_offset;
            bank = new Ekona.Images.Bank();
            bank.oams = new OAM[bank_size / 0x0A];
            for (int i = 0; i < bank.oams.Length; i++)
            {
                bank.oams[i].width = br.ReadUInt16();
                bank.oams[i].height = br.ReadUInt16();
                bank.oams[i].obj1.xOffset = br.ReadUInt16() - 0x80;
                bank.oams[i].obj0.yOffset = br.ReadUInt16() - 0x80;
                bank.oams[i].obj2.tileOffset = (uint)(br.ReadUInt16() / 0x20);
                bank.oams[i].num_cell = (ushort)i;
            }
            Set_Banks(new Ekona.Images.Bank[] { bank }, 0, false);
            br.Close();

            pluginHost.Set_Palette(palette);
            pluginHost.Set_Image(image);
            pluginHost.Set_Sprite(this);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }

    public class SIR0_Image : ImageBase
    {
        public SIR0_Image(IPluginHost pluginHost, string file, int id, string fileName)
            : base(file, id, pluginHost, fileName)
        {
        }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            char[] type = br.ReadChars(4);
            uint offset1 = br.ReadUInt32();
            uint offset2 = br.ReadUInt32();
            uint offset3 = br.ReadUInt32();     // Always 0x00 ?

            br.BaseStream.Position = offset1 + 0x1C;
            uint paletteOffset = br.ReadUInt32();

            br.BaseStream.Position = 0x10;
            byte[] tiles = br.ReadBytes((int)(paletteOffset - 0x10));

            br.BaseStream.Position = paletteOffset;
            Color[] colors = Actions.BGR555ToColor(br.ReadBytes(0x200));
            RawPalette palette = new RawPalette(new Color[][] { colors }, false, ColorFormat.colors256, "");
            pluginHost.Set_Palette(palette);

            br.Close();

            Set_Tiles(tiles, 0x100, tiles.Length / 0x100, ColorFormat.colors256, TileForm.Lineal, false, 8);
            pluginHost.Set_Image(this);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
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
