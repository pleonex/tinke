// ----------------------------------------------------------------------
// <copyright file="SPDL.cs" company="none">

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
// <date>30/07/2012 13:52:27</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
{
    public static class SPDL
    {
        public static sFolder Unpack(sFile cfile)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(cfile.path));

            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);    // spdl
            uint unknown = br.ReadUInt32();     // 0x00 ?
            uint file_size = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();

            br.BaseStream.Position += 0x10;     // 0x00 ?

            string file_name = new string(br.ReadChars(0x10)).Replace("\0", "");
            uint num_files = br.ReadUInt32();
            uint fat_size = br.ReadUInt32();
            br.BaseStream.Position += 0x08;     // Padding ?

            br.BaseStream.Position += 0x10; num_files--;    // First always to 0

            for (int i = 0; i < num_files; i++)
            {
                ushort id1 = br.ReadUInt16();   // Incremental id
                ushort id2 = br.ReadUInt16();   // Should be the same ?

                string ext = new string(br.ReadChars(4).Reverse().ToArray());
                uint size = br.ReadUInt32();
                uint offset = br.ReadUInt32();

                sFile newFile = new sFile();
                newFile.name = Path.GetFileNameWithoutExtension(file_name) + '.' + ext;
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = cfile.path;

                unpacked.files.Add(newFile);
            }

            br.Close();
            br = null;
            return unpacked;
        }

		public static void Pack(string file, string original, ref sFolder unpacked, IPluginHost pluginHost)
		{
			BinaryWriter bw = new BinaryWriter(File.OpenWrite(file));
			BinaryReader br = new BinaryReader(File.OpenRead(original));

			// Copy header
			bw.Write(br.ReadBytes(0x50));

			// Write file info
			uint offset = 0x800;
			for (int i = 0; i < unpacked.files.Count; i++) {
				bw.Write(br.ReadBytes(0x08));
				bw.Write(unpacked.files[i].size);	br.BaseStream.Position += 4;
				bw.Write(offset);					br.BaseStream.Position += 4;

				offset += unpacked.files[i].size;
				if (offset % 0x800 != 0)
					offset += 0x800 - (offset % 0x800);
			}

			br.Close();

			// Write files
			bw.Write(new byte[0x800 - bw.BaseStream.Position]);
			for (int i = 0; i < unpacked.files.Count; i++) {
				sFile cfile = unpacked.files[i];

				uint startOffset = (uint)bw.BaseStream.Position;

				// Write info
				BinaryReader brfile = new BinaryReader(File.OpenRead(cfile.path));
				brfile.BaseStream.Position = cfile.offset;
				bw.Write(brfile.ReadBytes((int)cfile.size));
				brfile.Close();

				// Write padding
				while (bw.BaseStream.Position % 0x800 != 0)
					bw.Write((byte)0);

				// Update file info
				cfile.offset = startOffset;
				cfile.path = file;
				unpacked.files[i] = cfile;
			}

			// Update file size
			bw.BaseStream.Position = 0x08;
			bw.Write((uint)bw.BaseStream.Length);

			bw.Close();
		}
    }
}
