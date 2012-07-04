// ----------------------------------------------------------------------
// <copyright file="SUBARASHIKI.cs" company="none">

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
// <date>29/06/2012 2:06:04</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class SUBARASHIKI : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }
        public bool IsCompatible()
        {
            if (gameCode == "AWLJ" || gameCode == "AWLP")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            String ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "pack")
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
        public sFolder Unpack(sFile file)
        {
            // Copied from:
            // http://forum.xentax.com/viewtopic.php?f=18&t=3175&p=30296&hilit=pack+world+ends#p30296
            // Credits to McCuñao

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint num_files = br.ReadUInt32();
            uint data_size = br.ReadUInt32();
            uint unknown = br.ReadUInt32(); // Unknown always 0x00
            br.ReadBytes(16); // Padding always 0x000

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = file.name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32() + 0x20;
                newFile.size = br.ReadUInt32();
                newFile.path = file.path;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
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
