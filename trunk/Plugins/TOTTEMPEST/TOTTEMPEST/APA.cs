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
using PluginInterface;

namespace TOTTEMPEST
{
    public static class APA
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            uint file_size = (uint)new FileInfo(file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            NCLR nclr = new NCLR();
            nclr.id = (uint)id;
            // Common header
            nclr.header.id = "APA ".ToCharArray();
            nclr.header.constant = 0x0100;
            nclr.header.file_size = file_size;
            nclr.header.header_size = 0x10;
            // TTLP section
            nclr.pltt.ID = "PLTT".ToCharArray();
            nclr.pltt.length = file_size;
            nclr.pltt.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.paletteLength = 0x20;
            nclr.pltt.nColors = 0x10;
            nclr.pltt.palettes = new NTFP[file_size / 0x20];
            // Get colors
            for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                nclr.pltt.palettes[i].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)0x20));

            br.Close();
            pluginHost.Set_NCLR(nclr);
        }
        public static void Write(NCLR palette, string fileout, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Write all the colors
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                bw.Write(pluginHost.ColorToBGR555(palette.pltt.palettes[i].colors));

            for (; bw.BaseStream.Length != 0x200; )
                bw.Write((byte)0x00);

            bw.Flush();
            bw.Close();
        }
    }
}
