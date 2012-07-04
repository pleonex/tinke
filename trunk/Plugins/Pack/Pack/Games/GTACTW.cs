// ----------------------------------------------------------------------
// <copyright file="GTACTW.cs" company="none">

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
// <date>02/05/2012 22:06:59</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class GTACTW : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "YGXE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.id == 0xC1)
                return Format.System;
            if (file.id == 0xC2)
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.id == 0xC1)
                return Unpack_ROM(pluginHost.Search_File(0xC2), file.path);
            if (file.id == 0xC2)
                return Unpack_ROM(file.path, pluginHost.Search_File(0xC1));

            return new sFolder();
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }


        public sFolder Unpack_ROM(string wad, string toc)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(toc));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint fat_size = br.ReadUInt32(); // Size of the FAT section
            uint size_table = fat_size + 8;
            uint num_files = fat_size / 8;

            for (int i = 0; i < num_files; i++)
            {
                br.BaseStream.Position += 3;    // ID of the file

                uint offset = br.ReadUInt32();
                offset &= 0xFFFFFF; // It's only 3 bytes
                offset <<= 8;       // Blocks of 256 bytes (padded)
                br.BaseStream.Position--;

                ushort size_ptr = br.ReadUInt16();
                size_ptr <<= 2;         // They are uint values (4bytes)

                // Go to the size table to get the size
                long currPos = br.BaseStream.Position;

                br.BaseStream.Position = size_table + size_ptr;
                uint size = br.ReadUInt32();    // Get the size of the file

                br.BaseStream.Position = currPos;

                // Set file
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".bin";
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = wad;
                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }
}
