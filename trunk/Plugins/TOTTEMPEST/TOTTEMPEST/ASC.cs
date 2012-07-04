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
using Ekona;
using Ekona.Images;

namespace TOTTEMPEST
{
    public class ASC : MapBase
    {
        IPluginHost pluginHost;

        public ASC(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            uint file_size = (uint)br.BaseStream.Length;

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            NTFS[] map = new NTFS[(file_size - 4) / 2];

            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, true, width * 8, height * 8);
            pluginHost.Set_Map(this);
        }
        public override void Write(string fileout, ImageBase image, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write((ushort)(Width / 0x08));
            bw.Write((ushort)(Height / 0x08));

            for (int i = 0; i < Map.Length; i++)
                bw.Write(Actions.MapInfo(Map[i]));

            bw.Flush();
            bw.Close();
        }
    }
}
