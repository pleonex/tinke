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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona;

namespace KIRBY_DRO
{
    public class Main : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }

        public bool IsCompatible()
        {
            if (gameCode == "AKWE")
                return true;
            else
                return false;
        }
        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper().EndsWith(".BIN"))
                return Format.FullImage;

            return Format.Unknown;
        }


        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".BIN"))
                return new Bin(file.path, file.id, pluginHost, file.name).Get_Control();

            return new Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
