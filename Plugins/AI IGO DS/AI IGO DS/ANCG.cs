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
using Ekona;
using Ekona.Images;

namespace AI_IGO_DS
{
    public class ANCG : ImageBase
    {
        public ANCG(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            char[] header = br.ReadChars(4);
            uint tiles_size = br.ReadUInt32();
            byte[] tiles = br.ReadBytes((int)tiles_size);

            br.Close();
            Set_Tiles(tiles, 0x40, tiles.Length / 0x20, ColorFormat.colors16, TileForm.Horizontal, false);
        }
        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
