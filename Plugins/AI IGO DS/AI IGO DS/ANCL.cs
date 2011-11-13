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

namespace AI_IGO_DS
{
    public static class ANCL
    {
        public static void Leer(string archivo, IPluginHost pluginHost)
        {
            NCLR paleta = new NCLR();
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            // Cabecera típica
            paleta.header.id = br.ReadChars(4);
            paleta.header.endianess = 0xFFFE;
            paleta.header.constant = 0x0100;
            paleta.header.file_size = (uint)br.BaseStream.Length;
            paleta.header.header_size = 0x8;
            paleta.header.nSection = 1;
            // Paleta
            paleta.pltt.ID = paleta.header.id;
            paleta.pltt.length = paleta.header.file_size - 0x08;
            paleta.pltt.paletteLength = paleta.header.file_size - 0x08;
            paleta.pltt.nColors = br.ReadUInt16();
            paleta.pltt.depth = (br.ReadUInt16() == 0x04) ? System.Windows.Forms.ColorDepth.Depth4Bit : System.Windows.Forms.ColorDepth.Depth8Bit;
                        
            paleta.pltt.palettes = new NTFP[1];
            paleta.pltt.palettes[0].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)(paleta.pltt.paletteLength)));
            pluginHost.Set_NCLR(paleta);

            br.Close();
        }
    }
}
