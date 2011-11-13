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

namespace TETRIS_DS
{
    public static class SLZ
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            pluginHost.Decompress(file);
            string dec_file;
            sFolder dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<sFile>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Decompress(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            uint file_size = (uint)new FileInfo(dec_file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));			

            NSCR nscr = new NSCR();
            nscr.id = (uint)id;

            nscr.header.id = "SLZ ".ToCharArray();
            nscr.header.endianess = 0xFEFF;
            nscr.header.constant = 0x0100;
            nscr.header.file_size = file_size;
            nscr.header.header_size = 0x10;
            nscr.header.nSection = 1;

            nscr.section.id = "SLZ ".ToCharArray();
            nscr.section.section_size = file_size;
            nscr.section.width = 0x0100;
            nscr.section.height = 0x0100;
            nscr.section.padding = 0x00000000;
            nscr.section.data_size = file_size;
            nscr.section.mapData = new NTFS[file_size / 2];

            for (int i = 0; i < (file_size / 2); i++)
            {
                ushort parameters = br.ReadUInt16();

                nscr.section.mapData[i] = new NTFS();
                nscr.section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                nscr.section.mapData[i].xFlip = (byte)((parameters >> 10) & 1);
                nscr.section.mapData[i].yFlip = (byte)((parameters >> 11) & 1);
                nscr.section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
            }

            br.Close();
            pluginHost.Set_NSCR(nscr);
        }
    }
}
