// ----------------------------------------------------------------------
// <copyright file="PCM.cs" company="none">

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

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>21/08/2012 10:14:15</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace LAYTON
{
    public static class PCM
    {
        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sPCM pcm = new sPCM();

            pcm.header_size = br.ReadUInt32();  // ALWAYS 0x10
            pcm.file_size = br.ReadUInt32();
            pcm.nFiles = br.ReadUInt32();
            pcm.id = br.ReadChars(4);           // ALWAYS LPCK

            pcm.files = new KCPL_File[pcm.nFiles];
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            for (int i = 0; i < pcm.nFiles; i++)
            {
                long f_offset = br.BaseStream.Position;

                pcm.files[i].header_size = br.ReadUInt32();     // ALWAYS 0x20
                pcm.files[i].file_size = br.ReadUInt32();       // Include padding and header
                pcm.files[i].unknown = br.ReadUInt32();         // ALWAYS 0x00
                pcm.files[i].data_size = br.ReadUInt32();       // Size of the file
                pcm.files[i].name = new String(br.ReadChars(16)).Replace("\0", "");
                pcm.files[i].offset = (uint)(f_offset + pcm.files[i].header_size);
                br.BaseStream.Position = f_offset + pcm.files[i].file_size;

                sFile cfile = new sFile();
                cfile.name = pcm.files[i].name;
                cfile.path = file;
                cfile.offset = pcm.files[i].offset;
                cfile.size = pcm.files[i].data_size;
                unpacked.files.Add(cfile);
            }

            br.Close();
            br = null;

            return unpacked;
        }
        public static void Pack(string fileOut, sFolder unpack)
        {
            string tempFile = Path.GetTempFileName();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(tempFile));

            BinaryReader br;
            uint f_size;
            byte[] name;

            // Write file data
            for (int i = 0; i < unpack.files.Count; i++)
            {
                f_size = unpack.files[i].size;
                f_size += 0x10 - (f_size % 0x10);

                // Write header
                bw.Write(0x20);
                bw.Write(f_size + 0x20);
                bw.Write(0x00);
                bw.Write(unpack.files[i].size);

                name = Encoding.ASCII.GetBytes(unpack.files[i].name);
                for (int j = 0; j < 0x10; j++)
                    if (j < name.Length)
                        bw.Write(name[j]);
                    else
                        bw.Write((byte)0x00);

                br = new BinaryReader(File.OpenRead(unpack.files[i].path));
                br.BaseStream.Position = unpack.files[i].offset;
                bw.Write(br.ReadBytes((int)unpack.files[i].size));
                br.Close();
                br = null;

                do
                    bw.Write((byte)0x00);
                while (bw.BaseStream.Position % 0x10 != 0);
            }

            f_size = (uint)bw.BaseStream.Length;
            bw.Flush();
            bw.Close();
            bw = null;

            // Write final file
            bw = new BinaryWriter(File.OpenWrite(fileOut));
            bw.Write(0x10);
            bw.Write(f_size + 0x10);
            bw.Write(unpack.files.Count);
            bw.Write(new char[] { 'L', 'P', 'C', 'K' });
            bw.Write(File.ReadAllBytes(tempFile));
            bw.Flush();
            bw.Close();
            bw = null;

            File.Delete(tempFile);
        }
    }

    #region Structures
    public struct sPCM
    {
        public uint header_size;
        public uint file_size;
        public uint nFiles;
        public char[] id;
        public KCPL_File[] files;
    }
    public struct KCPL_File
    {
        public uint header_size;
        public uint file_size;
        public uint unknown;
        public uint data_size;
        public string name;
        public uint offset;
    }
    #endregion

}