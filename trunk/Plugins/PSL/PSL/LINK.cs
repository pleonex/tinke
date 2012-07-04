/*
 * Copyright (C) 2012  pleonex
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
 *
 * Creado por SharpDevelop.
 * Fecha: 16/03/2012
 *
 */
using System;
using System.Collections.Generic;
using System.IO;
using Ekona;

namespace PSL
{

    public static class LINK
    {
        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint num_files = br.ReadUInt32();
            uint block_size = br.ReadUInt32();
            uint padding = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString();
                newFile.offset = br.ReadUInt32() * block_size;
                newFile.size = br.ReadUInt32();
                if (newFile.size == 0)
                    newFile.size = block_size;
                newFile.path = file;

                if (num_files == 6)
                {
                    if (i == 0)
                        newFile.name += ".nanr";
                    else if (i == 1)
                        newFile.name += ".ncgr";	// Tile form -> Lineal
                    else if (i == 2)
                        newFile.name += ".ncer";
                    else if (i == 3)
                        newFile.name += ".ncgr";	// Tile form -> Horizontal
                    else if (i == 4)
                        newFile.name += ".nclr";
                    else if (i == 5)
                        newFile.name += ".nscr";
                }

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static void Pack(string fileIn, string fileOut, ref sFolder unpacked)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            br.BaseStream.Position = 0x08;
            uint block_size = br.ReadUInt32();
            br.Close();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(new char[] { 'L', 'I', 'N', 'K' });
            bw.Write(unpacked.files.Count);
            bw.Write(block_size);
            bw.Write(0x00);

            unpacked.files.Sort(SortFiles);

            uint offset = block_size;
            List<byte> buffer = new List<byte>();
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;
                buffer.AddRange(br.ReadBytes((int)unpacked.files[i].size));
                while (buffer.Count % block_size != 0)
                    buffer.Add(0x00);
                br.Close();

                bw.Write(offset / block_size);
                bw.Write(unpacked.files[i].size);

                sFile upFile = unpacked.files[i];
                upFile.offset = offset;
                upFile.path = fileOut;
                unpacked.files[i] = upFile;

                offset += unpacked.files[i].size;
                if (offset % block_size != 0)  // Padding
                    offset += (block_size - (offset % block_size));
            }
            byte[] rem = new byte[0];
            if (bw.BaseStream.Position % block_size != 0)
                rem = new byte[block_size - (bw.BaseStream.Position % block_size)];
            bw.Write(rem);
            bw.Flush();

            bw.Write(buffer.ToArray());
            bw.Flush();

            bw.Close();
        }

        private static int SortFiles(sFile f1, sFile f2)
        {
            return f1.id.CompareTo(f2.id);
        }

    }
}
