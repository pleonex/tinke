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
// <date>28/04/2012 14:56:19</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Ekona;

namespace DEATHNOTEDS
{
    public class Main : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "YDNJ")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (gameCode == "YDNJ" && file.id == 0x1)
                return Format.Pack;

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, sFile file)
        {
            System.Windows.Forms.MessageBox.Show("TODO ;)\nIf you need it please contact with me");
            return "";
        }
        public sFolder Unpack(sFile file)
        {
            if (gameCode == "YDNJ" && file.id == 0x01)
                return Packs.Unpack_data(file);

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
