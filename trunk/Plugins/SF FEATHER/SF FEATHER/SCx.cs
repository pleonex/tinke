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
using PluginInterface;
using PluginInterface.Images;

namespace SF_FEATHER
{
    public class SCx : MapBase
    {
        public SCx(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }

        public override void Read(string file)
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

            NTFS[] map = new NTFS[size_mapData / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = pluginHost.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, false, width, height);
            pluginHost.Set_Map(this);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
