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
using PluginInterface;
using PluginInterface.Images;

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

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".ANA"))
                return Format.Tile;
            else if (nombre.EndsWith(".APA"))
                return Format.Palette;
            else if (nombre.EndsWith(".ASC"))
                return Format.Map;
            else if (nombre.EndsWith(".NBM"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
            if (file.ToUpper().EndsWith(".APA"))
                new APA(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".ANA"))
                new ANA(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".ASC"))
                new ASC(pluginHost, file, id);
        }
        public Control Show_Info(string file, int id)
        {
            Read(file, id);

            if (file.ToUpper().EndsWith(".ANA"))
                return new ImageControl(pluginHost, false);

            if (file.ToUpper().EndsWith(".ASC"))
                return new ImageControl(pluginHost, true);

            if (file.ToUpper().EndsWith(".APA"))
                return new PaletteControl(pluginHost);

            if (file.ToUpper().EndsWith(".NBM"))
                return new NBM(pluginHost, file, id).Get_Control();

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id) { return new sFolder(); }
    }
}
