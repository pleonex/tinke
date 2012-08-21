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
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class _4HEROSLIGHT : IGamePlugin
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
            if (gameCode == "BFXE" || gameCode == "CJKE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "NMDP")
                return Format.Pack;
            else if (ext == "SSAM")
                return Format.Pack;
            else if (file.name.ToUpper().EndsWith(".FLSC.LZ") && BitConverter.ToUInt32(magic, 0) < 0x10)
                return Format.Pack;

            return Format.Unknown;
        }

        public sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "NMDP")
                return Unpack_NMDP(file.path, file.name);
            else if (ext == "SSAM")
                return Unpack_SSAM(file.path);
            else if (file.name.ToUpper().EndsWith(".FLSC.LZ"))
                return Unpack_FLSC(file.path, file.name);

            return new sFolder();
        }
        public string Pack(ref sFolder unpacked, sFile file)
        {
            System.Windows.Forms.MessageBox.Show("Not done");
            return null;
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        public sFolder Unpack_NMDP(string fileIn, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);
            ushort header_size = br.ReadUInt16();

            br.BaseStream.Position = 0x10;
            uint unknown2 = br.ReadUInt32();

            uint unknown1 = br.ReadUInt32();

            sFile newFile = new sFile();
            newFile.name = name;
            newFile.size = br.ReadUInt32();
            newFile.offset = br.ReadUInt32();
            newFile.path = fileIn;

            unpacked.files.Add(newFile);

            br.Close();
            return unpacked;
        }
        public sFolder Unpack_SSAM(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);
            uint num_files = br.ReadUInt32();

            uint pos_base = num_files * 0x28 + 8;
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.offset = br.ReadUInt32() + pos_base;
                newFile.size = br.ReadUInt32();
                newFile.name = new String(br.ReadChars(0x20)).Replace("\0", "");
                newFile.path = fileIn;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public sFolder Unpack_FLSC(string fileIn, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = br.ReadUInt32();
            br.BaseStream.Position = 0x10;
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = fileIn;

                // Get the extension
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = newFile.offset;
                newFile.name += new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
                br.BaseStream.Position = currPos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }
}
