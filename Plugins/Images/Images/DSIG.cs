// ----------------------------------------------------------------------
// <copyright file="DSIG.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>06/07/2012 2:19:41</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class DSIG : ImageBase
    {
        sDSIG dsig;
        PaletteBase palette;

        public DSIG(sFile file) : base(file.path, file.id, file.name) { }
        public DSIG(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            dsig = new sDSIG();

            dsig.id = br.ReadChars(4);
            dsig.type = br.ReadByte();
            if (dsig.type != 0x02)

            dsig.unk4 = br.ReadByte();
            dsig.num_colors = br.ReadByte(); // Number of palettes of 16 colors
            dsig.unk1 = br.ReadByte();
            dsig.unk2 = br.ReadUInt16();
            dsig.unk3 = br.ReadUInt16();

            ColorFormat depth = (dsig.unk1 == 0 ? ColorFormat.colors16 : ColorFormat.colors256);
            if (dsig.unk1 != 0)
                System.Windows.Forms.MessageBox.Show("Found different depth!");
            TileForm form = (dsig.unk4 == 0x10 ? TileForm.Horizontal : TileForm.Lineal);

            Color[] colors = Actions.BGR555ToColor(br.ReadBytes(dsig.num_colors * 2));
            palette = new RawPalette(colors, false, depth, FileName);

            byte[] tiles = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            Set_Tiles(tiles, 0x100, 0xc0, depth, form, false);

            br.Close();
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public PaletteBase Palette
        {
            get { return palette; }
        }

        public struct sDSIG
        {
            public char[] id;
            public byte type;
            public byte unk4;
            public byte num_colors;
            public byte unk1;
            public ushort unk2;
            public ushort unk3;
        }
    }
}
