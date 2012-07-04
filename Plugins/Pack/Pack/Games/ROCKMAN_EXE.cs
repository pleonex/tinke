// ----------------------------------------------------------------------
// <copyright file="ROCKMAN_EXE.cs" company="none">

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
// <date>11/05/2012 13:36:30</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ekona;

namespace Pack.Games
{
    public class ROCKMAN_EXE : IGamePlugin
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
            if (gameCode == "B6XJ")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.id == 0x461 || file.id == 0x462 || file.id == 0x45D || file.id == 0x45F)
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
        public sFolder Unpack(sFile file)
        {
            // There are only one type of pack file
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            uint num_files = (br.ReadUInt32() / 8) - 1; // The last entry indicate the end of file
            br.BaseStream.Position = 0;

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = file.name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32();
                newFile.path = file.path;

                uint size = br.ReadUInt32();
                if ((size >> 31) == 1)  // The file it's compressed with LZ11
                {
                    uint next_offset = br.ReadUInt32();
                    newFile.size = next_offset - newFile.offset;
                    br.BaseStream.Position -= 4;
                }
                else
                    newFile.size = size;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
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
