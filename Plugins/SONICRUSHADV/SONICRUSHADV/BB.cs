/*
 * Copyright (C) 2016
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
 * Plugin by: ccawley2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace SONICRUSHADV
{
    public static class BB
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            br.ReadUInt32(); // Signature
            uint num_files = br.ReadUInt32();
            for (int i = 0; i < num_files; i++)
            {
                uint startOffset = br.ReadUInt32();
                uint fileSize = br.ReadUInt32();
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = startOffset;

                sFile newFile = new sFile();
                newFile.name = "File " + (i + 1).ToString("X") + ".bin";
                newFile.offset = startOffset;
                newFile.path = file;
                newFile.size = fileSize;

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
            bw.Write((uint)(0x4242));
            bw.Write((uint)(unpackedFiles.files.Count)); // Pointer table size
            uint currOffset = 0x00; // Pointer table offset
            currOffset += (uint)(unpackedFiles.files.Count * 8) + 8;

            List<byte> buffer = new List<byte>();
            for (int i = 0; i < unpackedFiles.files.Count; i++)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(unpackedFiles.files[i].path));
                br.BaseStream.Position = unpackedFiles.files[i].offset;
                buffer.AddRange(BitConverter.GetBytes((uint)unpackedFiles.files[i].size));
                buffer.AddRange(br.ReadBytes((int)unpackedFiles.files[i].size));
                br.Close();

                sFile newFile = unpackedFiles.files[i];
                newFile.offset = currOffset;
                newFile.path = output;              
                unpackedFiles.files[i] = newFile;

                bw.Write(currOffset);
                bw.Write(unpackedFiles.files[i].size);
                currOffset += unpackedFiles.files[i].size;
            }

            // Write files
            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();
        }
    }

}
