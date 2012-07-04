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
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace LASTWINDOW
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool IsCompatible()
        {
            if (gameCode == "YLUP")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.EndsWith(".PACK"))
                return Format.Compressed;

            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            return new Control();
        }

        public sFolder Unpack(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".PACK"))
                return PACK.Unpack(pluginHost, file.path);

            return new sFolder();
        }
        public String Pack(ref sFolder unpacked, sFile file) { return null; }
    }
}
