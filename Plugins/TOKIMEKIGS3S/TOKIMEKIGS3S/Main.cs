// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

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
// <date>29/06/2012 2:10:31</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ekona;

namespace TOKIMEKIGS3S
{
    public class Main : IGamePlugin
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
            if (gameCode == "B3SJ")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            String ext = new string(Encoding.ASCII.GetChars(magic));

            if (file.name.ToUpper().EndsWith(".LZS"))
                return Format.Compressed;
            else if (ext == "RESC")
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.name.ToUpper().EndsWith(".LZS"))
                return LZS.Compress(unpacked.files[0].path, file.path, pluginHost);
            else if (file.name.ToUpper().EndsWith(".RESC"))
                return RESC.Pack(file.path, ref unpacked, pluginHost);

            return null;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".LZS"))
            {
                sFolder decompressed = new sFolder();
                decompressed.files = new List<sFile>();
                decompressed.files.Add(LZS.Decompress(file.path, pluginHost));

                return decompressed;
            }
            else if (file.name.ToUpper().EndsWith(".RESC"))
            {
                return RESC.Unpack(file.path, pluginHost);
            }

            return new sFolder();
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
