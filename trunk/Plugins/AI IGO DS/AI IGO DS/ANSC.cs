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
    public class ANSC : MapBase
    {
        public ANSC(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            char[] header = br.ReadChars(4);
            uint size = br.ReadUInt16();
            NTFS[] map = new NTFS[(size + 2) / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, false, 0x100, 0xB0);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
