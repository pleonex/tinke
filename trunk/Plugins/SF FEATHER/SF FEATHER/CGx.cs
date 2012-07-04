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
using Ekona;
using Ekona.Images;

namespace SF_FEATHER
{
    public class CGx : ImageBase
    {
        const ushort WIDTH = 32;
        ColorFormat depth;
        sCGx cgx;
        PaletteBase pb;

        public CGx(string file, int id, bool cg8, string fileName = "")
            : base()
        {
            this.id = id;
            if (fileName != "")
                this.fileName = fileName;
            else
                this.fileName = Path.GetFileName(file);

            depth = cg8 ? ColorFormat.colors256 : Ekona.Images.ColorFormat.colors16;
            Read(file);
        }

        public PaletteBase Palette
        {
            get { return pb; }
        }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            cgx = new sCGx();

            cgx.type = br.ReadChars(4);  // CG4 
            cgx.unknown1 = br.ReadUInt32();
            cgx.unknown2 = br.ReadUInt32();
            cgx.unknown3 = br.ReadUInt32();    // Usually 0

            cgx.unknown4 = br.ReadUInt32();    // Usually 0
            cgx.size_tiles = br.ReadUInt32();
            cgx.unknown5 = br.ReadUInt32();
            cgx.num_tiles = br.ReadUInt32();

            cgx.palColors = br.ReadUInt32();
            cgx.tileOffset = br.ReadUInt32();
            cgx.palOffset = br.ReadUInt32();
            cgx.unknonwnOffset = br.ReadUInt32();    // If 0, it doesn't exist

            // Read tiles
            br.BaseStream.Position = cgx.tileOffset;
            int tile_size = (depth == ColorFormat.colors16 ? 0x20 : 0x40);
            Byte[] tiles = br.ReadBytes((int)cgx.num_tiles * tile_size);
            Set_Tiles(tiles, WIDTH, (int)(tiles.Length / WIDTH), depth, TileForm.Horizontal, false);
            if (depth == Ekona.Images.ColorFormat.colors16)
                Height *= 2;

            // Read palette
            br.BaseStream.Position = cgx.palOffset;
            Color[][] colors;
            if (depth == Ekona.Images.ColorFormat.colors16)
            {
                colors = new Color[cgx.palColors / 0x10][];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = Actions.BGR555ToColor(br.ReadBytes(32));
            }
            else
            {
                colors = new Color[1][];
                colors[0] = Actions.BGR555ToColor(br.ReadBytes((int)cgx.palColors * 2));
            }
            PaletteBase palette = new RawPalette(colors, false, depth);

            br.BaseStream.Position = cgx.unknonwnOffset;
            cgx.unknown = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));

            br.Close();
        }
        public override void Write(string fileOut, PaletteBase palette)
        {
            if (depth != FormatColor)
                throw new NotImplementedException("The current file doesn't support this depth");

            // Update the struct
            cgx.size_tiles = (uint)Tiles.Length;
            if (depth == Ekona.Images.ColorFormat.colors16)
                cgx.num_tiles = cgx.size_tiles / 0x20;
            else
                cgx.num_tiles = cgx.size_tiles / 0x40;
            cgx.palColors = (uint)palette.NumberOfColors;
            cgx.palOffset = 0x30 + cgx.size_tiles;
            cgx.unknonwnOffset = cgx.palOffset + cgx.palColors * 2;

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(cgx.type);
            bw.Write(cgx.unknown1);
            bw.Write(cgx.unknown2);
            bw.Write(cgx.unknown3);

            bw.Write(cgx.unknown4);
            bw.Write(cgx.size_tiles);
            bw.Write(cgx.unknown5);
            bw.Write(cgx.num_tiles);

            bw.Write(cgx.palColors);
            bw.Write(cgx.tileOffset);
            bw.Write(cgx.palOffset);
            bw.Write(cgx.unknonwnOffset);

            bw.Write(Tiles);
            for (int i = 0; i < palette.NumberOfPalettes; i++)
                bw.Write(Actions.ColorToBGR555(palette.Palette[i]));

            bw.Flush();
            bw.Close();
        }

        public struct sCGx
        {
            public char[] type;
            public uint unknown1;
            public uint unknown2;
            public uint unknown3;
            public uint unknown4;

            public uint size_tiles;
            public uint unknown5;
            public uint num_tiles;

            public uint palColors;
            public uint tileOffset;
            public uint palOffset;
            public uint unknonwnOffset;

            public byte[] unknown;
        }
    }

    public class CGT : ImageBase
    {
        bool transparency;
        PaletteBase pb;

        public CGT(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public PaletteBase Palette
        {
            get { return pb; }
        }

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
            palette[0] = Actions.BGR555ToColor(br.ReadBytes((int)palette_size));

            br.Close();

            Set_Tiles(data, width, height, (ColorFormat)format, TileForm.Lineal, false);
            pb = new RawPalette(palette, false, ColorFormat.colors256);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
