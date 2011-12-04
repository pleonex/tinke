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
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Images
{
	public class nbfs : MapBase
	{
		
		public nbfs(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) {	}
		
        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            uint file_size = (uint)br.BaseStream.Length;

            NTFS[] map = new NTFS[file_size / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = pluginHost.MapInfo(br.ReadUInt16());

            int width = (map.Length * 8 >= 0x100 ? 0x100 : map.Length * 8);
            int height = (map.Length / (width / 8)) * 8;

            br.Close();
            Set_Map(map, false, width, height);
        }

        public override void Write_Map(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
