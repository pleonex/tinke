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

namespace Images
{
    public class ntft : ImageBase
    {		
		public ntft(IPluginHost pluginHost, string archivo, int id) : base(pluginHost, archivo, id)
		{
		}
		
        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            int fileSize = (int)br.BaseStream.Length;

            Byte[][] tiles = new byte[1][];
            tiles[0] = br.ReadBytes(fileSize);

            int width = (fileSize < 0x100 ? fileSize : 0x0100);
            int height = fileSize / width;
            if (height == 0)
                height = 1;

            if (fileSize == 512)
                width = height = 32;

            br.Close();

            Set_Tiles(tiles, width, height, System.Windows.Forms.ColorDepth.Depth8Bit, TileOrder.NoTiled, false);
        }

        public override void Write_Tiles(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
