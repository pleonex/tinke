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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Ekona;

namespace DBK_ULTIMATE
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
            if (gameCode == "TDBJ")
                return true;

            return false;
        }


        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name == "archiveDBK.dsa")
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(sFile file) { }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        public sFolder Unpack(sFile file)
        {
            if (file.name == "archiveDBK.dsa")
                return Archive.Unpack_archiveDBK(pluginHost, file.path);

            return new sFolder();
        }
        public string Pack(ref sFolder unpacked, sFile file) { return null; }
    }
}
