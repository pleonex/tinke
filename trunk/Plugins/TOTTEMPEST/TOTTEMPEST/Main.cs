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
using System.Windows.Forms;
using Ekona;
using Ekona.Images;

namespace TOTTEMPEST
{
    public class Main : IGamePlugin
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
            if (gameCode == "ALEJ")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            file.name = file.name.ToUpper();

            if (file.name.EndsWith(".ANA"))
                return Format.Tile;
            else if (file.name.EndsWith(".APA"))
                return Format.Palette;
            else if (file.name.EndsWith(".ASC"))
                return Format.Map;
            else if (file.name.EndsWith(".NBM"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".APA"))
                new APA(pluginHost, file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".ANA"))
                new ANA(pluginHost, file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".ASC"))
                new ASC(pluginHost, file.path, file.id);
        }
        public Control Show_Info(sFile file)
        {
            Read(file);

            if (file.name.ToUpper().EndsWith(".ANA"))
                return new ImageControl(pluginHost, false);

            if (file.name.ToUpper().EndsWith(".ASC"))
                return new ImageControl(pluginHost, true);

            if (file.name.ToUpper().EndsWith(".APA"))
                return new PaletteControl(pluginHost);

            if (file.name.ToUpper().EndsWith(".NBM"))
                return new NBM(pluginHost, file.path, file.id).Get_Control();

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
