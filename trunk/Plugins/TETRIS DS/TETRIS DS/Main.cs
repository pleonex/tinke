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
using Ekona;
using Ekona.Images;

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

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper().EndsWith(".CZ"))
                return Format.Tile;
            else if (file.name.ToUpper().EndsWith(".BDZ") || file.name.ToUpper().EndsWith(".BLZ"))
                return Format.Tile;
            else if (file.name.ToUpper().EndsWith(".SLZ"))
                return Format.Map;
            else if (file.name.ToUpper().EndsWith(".PLZ"))
                return Format.Palette;
            else if (file.name.ToUpper().EndsWith(".CLZ") || file.name.ToUpper().EndsWith(".CHR"))
                return Format.Tile;
            else if (file.name.ToUpper().EndsWith(".OBJS"))
                return Format.Cell;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".CZ"))
                new CZ(pluginHost, file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".BDZ") || file.name.ToUpper().EndsWith(".BLZ"))
                new BDZ(pluginHost, file.path, file.id);
            else if (file.name .ToUpper().EndsWith(".SLZ"))
                new SLZ(pluginHost, file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".PLZ"))
                new PLZ(pluginHost, file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".CLZ") || file.name.ToUpper().EndsWith(".CHR"))
                new CLZ(pluginHost ,file.path, file.id);
            else if (file.name.ToUpper().EndsWith(".OBJS"))
                new OBJS(pluginHost, file.path, file.id);
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            Read(file);

            if (file.name.ToUpper().EndsWith(".CZ") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".SLZ") && pluginHost.Get_Image().Loaded && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette(), pluginHost.Get_Map());

            if ((file.name.ToUpper().EndsWith(".CLZ") || file.name.ToUpper().EndsWith(".CHR")) && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".PLZ"))
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if ((file.name.ToUpper().EndsWith(".BDZ") || file.name.ToUpper().EndsWith(".BLZ")) && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".OBJS"))
                return new SpriteControl(pluginHost);

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
