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

namespace SF_FEATHER
{
    public class SCx : MapBase
    {
        sSCx scx;

        public SCx(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            scx = new sSCx();

            scx.type = br.ReadChars(4);
            scx.unknown1 = br.ReadUInt32();
            scx.unknown2 = br.ReadUInt32();
            scx.unknown3 = br.ReadUInt32();    // Usually 0x00
            scx.unknown4 = br.ReadUInt32();

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            scx.size_mapData2 = br.ReadUInt32();   // Is alway the same?
            scx.size_mapData = br.ReadUInt32();

            NTFS[] map = new NTFS[scx.size_mapData / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, false, width, height);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(scx.type);
            bw.Write(scx.unknown1);
            bw.Write(scx.unknown2);
            bw.Write(scx.unknown3);
            bw.Write(scx.unknown4);

            bw.Write((ushort)Width);
            bw.Write((ushort)Height);
            bw.Write(Map.Length * 2);
            bw.Write(Map.Length * 2);

            for (int i = 0; i < Map.Length; i++)
                bw.Write(Actions.MapInfo(Map[i]));

            bw.Flush();
            bw.Close();
        }

        public struct sSCx
        {
            public char[] type;
            public uint unknown1;
            public uint unknown2;
            public uint unknown3;
            public uint unknown4;

            public uint size_mapData2;
            public uint size_mapData;
        }
    }
}
