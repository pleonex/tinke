// ----------------------------------------------------------------------
// <copyright file="GENSUIKODS.cs" company="none">

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
// <date>30/04/2012 20:02:06</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class GENSUIKODS : IGamePlugin
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
            if (gameCode == "YG4E")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (BitConverter.ToUInt32(magic, 0) == 0x00015618)
                return Format.Pack;
            if (file.id == 0x16)
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.id == 0x16)
                return Unpack_Sound(file.path, file.name);

            return Unpack_Images(file.path);
            return new sFolder();
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        private sFolder Unpack_Images(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint unknown = br.ReadUInt32();     // Constant? 0x00015618
            ushort num_files = br.ReadUInt16();
            ushort header_size = br.ReadUInt16();
            uint unknown2 = br.ReadUInt32();    // Type of pack file
            if (unknown2 != 0)
            {
                System.Windows.Forms.MessageBox.Show("File not supported");
                return new sFolder();
            }

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.path = file;
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                uint decompressed_size = br.ReadUInt32();
                newFile.name = new String(br.ReadChars(0x20)).Replace("\0", "");

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        private sFolder Unpack_Sound(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint fat_size = br.ReadUInt32();
            uint num_files = fat_size / 0x08;

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32();

                if (i + 1 != num_files)
                    newFile.size = br.ReadUInt32() - newFile.offset;
                newFile.offset +=  fat_size + 4;
                if (i + 1 == num_files)
                    newFile.size = (uint)br.BaseStream.Length - newFile.offset;
                newFile.path = file;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }
}
