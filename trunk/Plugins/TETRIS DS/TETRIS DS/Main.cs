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
using System.Text;
using PluginInterface;
using PluginInterface.Images;

namespace TETRIS_DS
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
            if (gameCode == "ATRP")
                return true;
            return false;
        }

        public Format Get_Format(string name, byte[] magic, int id)
        {
            if (name.ToUpper().EndsWith(".CZ"))
                return Format.Tile;
            else if (name.ToUpper().EndsWith(".BDZ") || name.ToUpper().EndsWith(".BLZ"))
                return Format.Tile;
            else if (name.ToUpper().EndsWith(".SLZ"))
                return Format.Map;
            else if (name.ToUpper().EndsWith(".PLZ"))
                return Format.Palette;
            else if (name.ToUpper().EndsWith(".CLZ") || name.ToUpper().EndsWith(".CHR"))
                return Format.Tile;
            else if (name.ToUpper().EndsWith(".OBJS"))
                return Format.Cell;

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
            if (file.ToUpper().EndsWith(".CZ"))
                new CZ(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".BDZ") || file.ToUpper().EndsWith(".BLZ"))
                new BDZ(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".SLZ"))
                new SLZ(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".PLZ"))
                new PLZ(pluginHost, file, id);
            else if (file.ToUpper().EndsWith(".CLZ") || file.ToUpper().EndsWith(".CHR"))
                new CLZ(pluginHost ,file, id);
            else if (file.ToUpper().EndsWith(".OBJS"))
                new OBJS(pluginHost, file, id);
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            Read(file, id);

            if (file.ToUpper().EndsWith(".CZ") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.ToUpper().EndsWith(".SLZ") && pluginHost.Get_Image().Loaded && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette(), pluginHost.Get_Map());

            if ((file.ToUpper().EndsWith(".CLZ") || file.ToUpper().EndsWith(".CHR")) && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.ToUpper().EndsWith(".PLZ"))
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if ((file.ToUpper().EndsWith(".BDZ") || file.ToUpper().EndsWith(".BLZ")) && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.ToUpper().EndsWith(".OBJS"))
                return new SpriteControl(pluginHost);

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id) { return new sFolder(); }
    }
}
