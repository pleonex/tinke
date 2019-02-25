// ----------------------------------------------------------------------
// <copyright file="ALAR.cs" company="none">

// Copyright (C) 2019
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

// <author>pleoNeX, Daviex94, Priverop</author>
// <contact>https://github.com/priverop/</contact>
// <date>24/02/2019</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace JUS
{
    /// <summary>
    /// Specification from: https://web.archive.org/web/20100111220659/http://jumpstars.wikispaces.com/File+Formats
    /// </summary>
    public class ALAR
    {
        private const byte TYPE2ID = 0x02;
        private const byte TYPE3ID = 0x03;
        IPluginHost pluginHost;

        public ALAR(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public sFolder Unpack(sFile file)
        {

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = 4;
            byte type = br.ReadByte();

            if (type == TYPE2ID)
                return Unpack_Type2(file);
            if (type == TYPE3ID)
                return Unpack_Type3(file);

            br.BaseStream.Position = 0;
            byte[] magic = br.ReadBytes(4);

            br.Close();

            string magicString = new String(Encoding.ASCII.GetChars(magic));
            if (magicString == "DSCP")
                return Decompress_LZSS(file);

            return new sFolder();
        }

        public sFolder Unpack_Type2(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            // Read the header file
            char[] header = br.ReadChars(4);
            byte type = br.ReadByte();
            byte unk = br.ReadByte();
            ushort num_files = br.ReadUInt16();

            byte[] ids = new byte[8];
            for (int i = 0; i < 8; i++)
                ids[i] = br.ReadByte();

            // Index table
            uint name_offset = (uint)(0x10 + num_files * 0x10);
            for (int i = 0; i < num_files; i++)
            {
                uint unk1 = br.ReadUInt32();

                sFile cfile = new sFile
                {
                    path = file.path,
                    offset = br.ReadUInt32(),
                    size = br.ReadUInt32()
                };

                uint unk2 = br.ReadUInt32();

                long curpos = br.BaseStream.Position;
                br.BaseStream.Position = name_offset + 2;
                cfile.name = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(0x20))).Replace("\0", "");
                name_offset += cfile.size + 0x24;
                br.BaseStream.Position = curpos;

                unpacked.files.Add(cfile);
            }

            br.Close();
            return unpacked;
        }

        public sFolder Unpack_Type3(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            unpacked.folders = new List<sFolder>();

            // Read the header file
            char[] header = br.ReadChars(4);
            byte type = br.ReadByte();
            byte unk = br.ReadByte();
            uint num_files = br.ReadUInt32();
            ushort unk2 = br.ReadUInt16();
            uint array_count = br.ReadUInt32();
            ushort endFileIndex = br.ReadUInt16();
            ushort[] fileTableIndex = new ushort[array_count + 1];

            for (int i = 0; i < array_count + 1;i++)
            {
                fileTableIndex[i] = br.ReadUInt16();
            }

            // Index table
            foreach(ushort filePosition in fileTableIndex)
            {
                br.BaseStream.Position = filePosition;

                ushort fileID = br.ReadUInt16();
                ushort unk3 = br.ReadUInt16();

                sFile cfile = new sFile
                {
                    path = file.path,
                    offset = br.ReadUInt32(), // StartOfFile
                    size = br.ReadUInt32() // SizeOfFile
                };

                ushort unk4 = br.ReadUInt16();
                ushort unk5 = br.ReadUInt16();
                ushort unk6 = br.ReadUInt16();

                string filename = this.ReadNullTerminatedString(br);

                AddFile(unpacked, cfile, filename);
            }

            br.Close();
            return unpacked;
        }

        public sFolder Decompress_LZSS(sFile file)
        {
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            string temp = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Byte[] compressFile = new Byte[(new FileInfo(file.path).Length) - 4];
            Array.Copy(File.ReadAllBytes(file.path), 4, compressFile, 0, compressFile.Length);
            File.WriteAllBytes(temp, compressFile);

            // Get the decompressed file
            string fileIn = Path.GetDirectoryName(temp) + Path.DirectorySeparatorChar + "de" + Path.DirectorySeparatorChar + Path.GetFileName(temp);
            Directory.CreateDirectory(Path.GetDirectoryName(fileIn));
            DSDecmp.Main.Decompress(temp, fileIn, DSDecmp.Main.Get_Format(temp));
            File.Delete(temp);

            sFile fileOut = new sFile
            {
                path = fileIn,
                name = file.name,
                size = (uint)new FileInfo(fileIn).Length
            };

            unpacked.files.Add(fileOut);

            return unpacked;
        }

        private static void AddFile(sFolder folder, sFile file, string filePath)
        {
            const char SEPARATOR = '/';

            if (filePath.Contains(SEPARATOR))
            {
                string folderName = filePath.Substring(0, filePath.IndexOf(SEPARATOR));
                sFolder subfolder = new sFolder();
                foreach (sFolder f in folder.folders)
                {
                    if (f.name == folderName)
                    {
                        subfolder = f;
                    }
                }

                if (string.IsNullOrEmpty(subfolder.name))
                {
                    subfolder.name = folderName;
                    subfolder.folders = new List<sFolder>();
                    subfolder.files = new List<sFile>();
                    folder.folders.Add(subfolder);
                }

                AddFile(subfolder, file, filePath.Substring(filePath.IndexOf(SEPARATOR) + 1));
            }
            else
            {
                file.name = filePath;
                folder.files.Add(file);
            }
        }

        private string ReadNullTerminatedString(System.IO.BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }
    }
}
