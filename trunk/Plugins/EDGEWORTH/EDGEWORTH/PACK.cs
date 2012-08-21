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
using Ekona;

namespace EDGEWORTH
{
    public static class PACK
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = (br.ReadUInt32() / 0x04) - 1;
            br.ReadUInt32(); // Pointer table
            for (int i = 0; i < num_files; i++)
            {
                uint startOffset = br.ReadUInt32();
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = startOffset;

                sFile newFile = new sFile();
                newFile.name = "File " + (i + 1).ToString("X") + ".bin";
                newFile.offset = startOffset + 4;
                newFile.path = file;
                newFile.size = br.ReadUInt32();

                br.BaseStream.Position = currPos;
                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public static void Pack(string output, ref sFolder unpackedFiles)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(output));

            // Write pointers
            bw.Write((uint)((unpackedFiles.files.Count + 1) * 4)); // Pointer table size
            uint currOffset = 0x00; // Pointer table offset
            bw.Write(currOffset);
            currOffset += (uint)(unpackedFiles.files.Count + 1) * 4 + 4;

            List<byte> buffer = new List<byte>();
            for (int i = 0; i < unpackedFiles.files.Count; i++)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(unpackedFiles.files[i].path));
                br.BaseStream.Position = unpackedFiles.files[i].offset;
                buffer.AddRange(BitConverter.GetBytes((uint)unpackedFiles.files[i].size));
                buffer.AddRange(br.ReadBytes((int)unpackedFiles.files[i].size));
                br.Close();

                sFile newFile = unpackedFiles.files[i];
                newFile.offset = currOffset + 4;
                newFile.path = output;              
                unpackedFiles.files[i] = newFile;

                bw.Write(currOffset);
                currOffset += unpackedFiles.files[i].size + 4;
            }

            // Write files
            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();
        }
    }

}
