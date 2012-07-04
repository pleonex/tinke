// ----------------------------------------------------------------------
// <copyright file="RUNEFACTORY3.cs" company="none">

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
// <date>29/06/2012 1:41:52</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class RUNEFACTORY3 : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "BRFE" || gameCode == "BRFJ" || gameCode == "BRFP")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (file.id == 0x14)
                return Format.Pack;
            else if (ext == "TEXT")
                return Format.Text;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.id != 0x14)
                return null;

            String fileOut = pluginHost.Get_TempFile();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write((ushort)0x0C); // Offset of file entries
            bw.Write((ushort)0x08); // Lenght of file entry
            bw.Write((uint)0x00);   // Always 0x00
            bw.Write((uint)unpacked.files.Count);

            // File entries
            uint offset = 0x00;
            uint[] offset_files = new uint[unpacked.files.Count];
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                offset_files[i] = offset;
                bw.Write(offset);
                bw.Write(unpacked.files[i].size);

                offset += unpacked.files[i].size;
            }
            bw.Flush();

            // File data
            uint first_file_offset = (uint)unpacked.files.Count * 0x8 + 0xC;
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;
                bw.Write(br.ReadBytes((int)unpacked.files[i].size));
                br.Close();
                bw.Flush();

                sFile newFile = unpacked.files[i];
                newFile.offset = offset_files[i] + first_file_offset;
                newFile.path = fileOut;
                unpacked.files[i] = newFile;
            }

            bw.Close();
            return fileOut;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.id != 0x14)
                return new sFolder();

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
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
                newFile.name = file.name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32() + first_file_offset;
                newFile.size = br.ReadUInt32();
                newFile.path = file.path;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;

        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

    }

}
