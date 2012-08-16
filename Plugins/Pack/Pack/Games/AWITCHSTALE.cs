// ----------------------------------------------------------------------
// <copyright file="AWITCHSTALE.cs" company="none">

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
// <date>13/08/2012 16:17:45</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class AWITCHSTALE : IGamePlugin
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
            if (gameCode == "CW3E")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.id == 0x3F)
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
        public sFolder Unpack(sFile file)
        {
            if (file.id == 0x3F)
            {
                sFolder unpack = new sFolder();
                unpack.files = new List<sFile>();
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));

                uint fat_size = br.ReadUInt32();
                uint num_files = fat_size / 4 - 1;  // Include a ptr to the fat section

                for (int i = 0; i < num_files; i++)
                {
                    sFile cfile = new sFile();
                    cfile.name = "File" + i.ToString() + ".bin";
                    cfile.path = file.path;

                    br.BaseStream.Position = 8 + i * 4;
                    cfile.offset = br.ReadUInt32();
                    br.BaseStream.Position = cfile.offset;
                    cfile.size = br.ReadUInt32();
                    cfile.offset += 4;

                    unpack.files.Add(cfile);
                }

                br.Close();
                br = null;

                return unpack;
            }

            return new sFolder();
        }

        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            throw new NotImplementedException();
        }

    }
}
