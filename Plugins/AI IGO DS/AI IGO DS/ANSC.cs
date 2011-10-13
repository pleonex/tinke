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
    public static class ANSC
    {
        public static void Leer(string archivo, int id, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            // Su formato es NTFS raw, sin información, nos la inventamos por tanto
            NSCR ansc = new NSCR();
            ansc.id = (uint)id;

            // Lee cabecera genérica
            ansc.header.id = br.ReadChars(4);
            ansc.header.endianess = 0xFEFF;
            ansc.header.constant = 0x0100;
            ansc.header.file_size = (uint)br.ReadUInt16() + 0x06;
            ansc.header.header_size = 0x10;
            ansc.header.nSection = 1;

            // Lee primera y única sección:
            ansc.section.id = "ANSC".ToCharArray();
            ansc.section.section_size = ansc.header.file_size;
            ansc.section.width = 0x0100;
            ansc.section.height = 0x00C0;
            ansc.section.padding = 0x00000000;
            ansc.section.data_size = ansc.header.file_size - 0x06;
            ansc.section.mapData = new NTFS[ansc.section.data_size / 2];

            for (int i = 0; i < ansc.section.mapData.Length; i++)
            {
                string bits = pluginHost.BytesToBits(br.ReadBytes(2));

                ansc.section.mapData[i] = new NTFS();
                ansc.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                ansc.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                ansc.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                ansc.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();

            pluginHost.Set_NSCR(ansc);
        }
    }
}
