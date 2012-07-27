// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>Daviex94</author>
// <email>david.iuffri94@hotmail.it</email>
// <date>22/07/2012 2:41:51</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace WITCHTALE
{
    public static class Packs
    {
        public static sFolder Unpack (sFile file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = (br.ReadUInt32() / 0x04) - 1;
            br.ReadUInt32(); // Pointer table
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File " + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32();
                newFile.path = file.path;
                newFile.size = (br.ReadUInt32() - newFile.offset);
                br.BaseStream.Position -= 4;

                unpacked.files.Add(newFile);
            }

            br.Close();

            return unpacked; ;
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
                buffer.AddRange(br.ReadBytes((int)unpackedFiles.files[i].size));
                br.Close();

                sFile newFile = unpackedFiles.files[i];
                newFile.offset = currOffset;
                newFile.path = output;
                unpackedFiles.files[i] = newFile;

                bw.Write(currOffset);
                currOffset += unpackedFiles.files[i].size;
            }

            // Write files
            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();
        }
    }
}
