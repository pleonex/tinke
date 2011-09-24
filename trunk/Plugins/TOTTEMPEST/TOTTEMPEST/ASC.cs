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
    public static class ASC
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            uint file_size = (uint)new FileInfo(file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            NSCR nscr = new NSCR();
            nscr.id = (uint)id;

            // Common header
            nscr.header.id = "ASC ".ToCharArray();
            nscr.header.endianess = 0xFEFF;
            nscr.header.constant = 0x0100;
            nscr.header.file_size = file_size;
            nscr.header.header_size = 0x10;
            nscr.header.nSection = 1;

            // NTFS data
            nscr.section.id = "ASC ".ToCharArray();
            nscr.section.section_size = file_size;
            nscr.section.width = (ushort)(br.ReadUInt16() * 8);
            nscr.section.height = (ushort)(br.ReadUInt16() * 8);
            nscr.section.padding = 0x00000000;
            nscr.section.data_size = file_size - 4;
            nscr.section.mapData = new NTFS[nscr.section.data_size / 2];

            for (int i = 0; i < nscr.section.mapData.Length; i++)
            {
                string bits = pluginHost.BytesToBits(br.ReadBytes(2));

                nscr.section.mapData[i] = new NTFS();
                nscr.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                nscr.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                nscr.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                nscr.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();
            pluginHost.Set_NSCR(nscr);
        }
        public static void Write(NSCR map, string fileout, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write((ushort)(map.section.width / 0x08));
            bw.Write((ushort)(map.section.height / 0x08));
            // Write NTFS
            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                int npalette = map.section.mapData[i].nPalette << 12;
                int yFlip = map.section.mapData[i].yFlip << 11;
                int xFlip = map.section.mapData[i].xFlip << 10;
                int data = npalette + yFlip + xFlip + map.section.mapData[i].nTile;
                bw.Write((ushort)data);
            }

            bw.Flush();
            bw.Close();
        }
    }
}
