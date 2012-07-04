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
using Ekona;
using Ekona.Images;

namespace TOTTEMPEST
{
    public class APA : PaletteBase
    {
        IPluginHost pluginHost;

        public APA(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            Color[][] colors = new Color[(int)(br.BaseStream.Length / 0x20)][];
            // Get colors
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Actions.BGR555ToColor(br.ReadBytes((int)0x20));

            br.Close();

            Set_Palette(colors, ColorFormat.colors16, true);
            pluginHost.Set_Palette(this);
        }
        public override void Write(string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Write all the colors
            for (int i = 0; i < palette.Length; i++)
                bw.Write(Actions.ColorToBGR555(palette[i]));

            for (; bw.BaseStream.Length != 0x200; )
                bw.Write((byte)0x00);

            bw.Flush();
            bw.Close();
        }
    }
}
