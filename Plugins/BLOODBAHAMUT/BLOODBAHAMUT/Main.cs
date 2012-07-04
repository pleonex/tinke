/*
 * Copyright (C) 2011  pleoNeX, Tricky Upgrade
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
 * By: pleoNeX, Tricky Upgrade
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using Ekona;

namespace BLOODBAHAMUT
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
            if (gameCode == "CYJJ" || gameCode == "CS7J")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper().EndsWith(".DPK"))
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.name.ToUpper().EndsWith(".DPK"))
                return DPK.Pack(ref unpacked, file.path, file.id);

            return null;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".DPK"))
                return DPK.Unpack(file.path, pluginHost);

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
