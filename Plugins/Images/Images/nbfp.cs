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
	public class nbfp : PaletteBase
	{		
		public nbfp(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }
		
        public override void Read(string fileIn)
        {
            uint file_size = (uint)new FileInfo(fileIn).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Color data
            Color[][] palette = new Color[1][];
            palette[0] = pluginHost.BGR555ToColor(br.ReadBytes((int)br.BaseStream.Length));

            br.Close();
            Set_Palette(palette, false);
        }

        public override void Write_Palette(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
