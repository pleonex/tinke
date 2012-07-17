// ----------------------------------------------------------------------
// <copyright file="ALAR.cs" company="none">

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

// <author>pleoNeX, Daviex94</author>
// <email>benito356@gmail.com</email>
// <date>06/07/2012 1:03:49</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack
{
    /// <summary>
    /// Specification from: http://jumpstars.wikispaces.com/File+Formats
    /// </summary>
    public static class ALAR
    {
        public static sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = 4;
            byte type = br.ReadByte();
            br.Close();

            if (type == 0x02)
                return Unpack_Type2(file);

            return new sFolder();
        }

        public static sFolder Unpack_Type2(sFile file)
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

                sFile cfile = new sFile();
                cfile.path = file.path;
                cfile.offset = br.ReadUInt32();
                cfile.size = br.ReadUInt32();

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
    }
}
