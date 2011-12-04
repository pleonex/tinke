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
	public class nbfc : ImageBase
	{		
		public nbfc(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) {	}

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            int fileSize = (int)br.BaseStream.Length;

            Byte[][] tiles = new byte[fileSize / 0x40][];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = br.ReadBytes(0x40);

            int width = (tiles.Length < 0x20 ? 0x04 : 0x20);
            int height = tiles.Length / width;
            if (height == 0)
                height = 8;

            if (fileSize == 0x200)
                width = height = 0x04;

            br.Close();
            Set_Tiles(tiles, width, height, System.Windows.Forms.ColorDepth.Depth8Bit, TileOrder.Horizontal, false);
        }

        public override void Write_Tiles(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
