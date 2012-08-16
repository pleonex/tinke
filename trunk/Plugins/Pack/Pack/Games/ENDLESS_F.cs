// ----------------------------------------------------------------------
// <copyright file="ENDLESS_F.cs" company="none">

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
// <date>13/08/2012 16:18:05</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class ENDLESS_F : IGamePlugin
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
            if (gameCode == "CFRE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if ((file.name.EndsWith(".pk1") || file.name.EndsWith(".pk2") || file.name.EndsWith(".pk")) 
                && (magic[0] == 0 && magic[1] == 0))
                return Format.Pack;

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
        public sFolder Unpack(sFile file)
        {
            if (file.name.EndsWith(".pk1") || file.name.EndsWith(".pk2") || file.name.EndsWith(".pk"))
            {
                sFolder unpack = new sFolder();
                unpack.files = new List<sFile>();
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));

                ushort unk = br.ReadUInt16();
                if (unk != 0x00)
                    System.Windows.Forms.MessageBox.Show("Different to 0!");
                ushort num_files = br.ReadUInt16();

                for (int i = 0; i < num_files; i++)
                {
                    sFile cfile = new sFile();
                    cfile.name = "File" + i.ToString() + ".bin";
                    cfile.path = file.path;
                    cfile.offset = br.ReadUInt32();
                    cfile.size = br.ReadUInt32();
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
