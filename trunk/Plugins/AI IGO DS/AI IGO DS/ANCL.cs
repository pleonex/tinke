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
using System.Drawing;
using PluginInterface;
using PluginInterface.Images;

namespace AI_IGO_DS
{
    public class ANCL : PaletteBase
    {
        public ANCL(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] header = br.ReadChars(4);
            ushort num_colors = br.ReadUInt16();
            ColorFormat depth = (ColorFormat)br.ReadUInt16();
                   
            Color[][] colors = new Color[1][];
            colors[0] = pluginHost.BGR555ToColor(br.ReadBytes((int)(br.BaseStream.Length - 0x08)));

            br.Close();
            Set_Palette(colors, false);
            pluginHost.Set_Palette(this);
        }
        public override void Write(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
