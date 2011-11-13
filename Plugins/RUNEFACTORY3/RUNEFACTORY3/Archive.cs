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

namespace RUNEFACTORY3
{
    public static class Archive
    {

        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            ushort file_entry_offset = br.ReadUInt16();
            ushort file_entry_length = br.ReadUInt16();
            br.ReadUInt32();    // Padding, always 0x00
            uint num_files = br.ReadUInt32();
            uint first_file_offset = num_files * file_entry_length + file_entry_offset;

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32() + first_file_offset;
                newFile.size = br.ReadUInt32();
                newFile.path = file;

                unpacked.files.Add(newFile);
            }
            
            br.Close();
            return unpacked;
        }
        public static void Pack(string fileOut, ref sFolder unpacked)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((ushort)0x0C); // Offset of file entries
            bw.Write((ushort)0x08); // Lenght of file entry
            bw.Write((uint)0x00);   // Always 0x00
            bw.Write((uint)unpacked.files.Count);

            // File entries
            uint offset = 0x00;
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                sFile newFile = unpacked.files[i];
                newFile.offset = offset;
                newFile.path = fileOut;
                unpacked.files[i] = newFile;

                bw.Write(offset);
                bw.Write(unpacked.files[i].size);
                offset += unpacked.files[i].size;
            }
            bw.Flush();

            // File data
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;
                bw.Write(br.ReadBytes((int)unpacked.files[i].size));
                br.Close();
                bw.Flush();
            }

            bw.Close();
        }
    }
}
