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
using PluginInterface;
using PluginInterface.Images;

namespace AI_IGO_DS
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
            if (gameCode == "AIIJ")
                return true;

            return false;
        }
        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".ANCL"))
                return Format.Palette;
            else if (nombre.EndsWith(".ANCG"))
                return Format.Tile;
            else if (nombre.EndsWith(".ATEX"))
                return Format.Tile;
            else if (nombre.EndsWith(".ANSC"))
                return Format.Map;
            else if (nombre.EndsWith("FAT.BIN") || nombre.EndsWith("FNT.BIN") || nombre.EndsWith("ARM9.BIN") ||
                    nombre.EndsWith("ARM7.BIN"))
                return Format.System;
            else if (nombre.EndsWith(".BIN") || nombre.EndsWith(".R00"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".ANCL"))
                new ANCL(pluginHost, archivo, id);
            else if (archivo.ToUpper().EndsWith(".ANCG"))
                new ANCG(pluginHost, archivo, id);
            else if (archivo.ToUpper().EndsWith(".ANSC"))
                new ANSC(pluginHost, archivo, id);
            else if (archivo.ToUpper().EndsWith(".ATEX"))
                new ATEX(pluginHost, archivo, id);
        }
        public Control Show_Info(string archivo, int id)
        {
            Read(archivo, id);

            if (archivo.ToUpper().EndsWith(".ANCL"))
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if (archivo.ToUpper().EndsWith(".ANCG") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (archivo.ToUpper().EndsWith(".ANSC") && pluginHost.Get_Palette().Loaded && pluginHost.Get_Image().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette(), pluginHost.Get_Map());

            if (archivo.ToUpper().EndsWith(".ATEX") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (archivo.ToUpper().EndsWith(".BIN") && !archivo.EndsWith("fat.bin") && !archivo.EndsWith("fnt.bin") &&
                !archivo.EndsWith("arm9.bin") && !archivo.EndsWith("arm7.bin"))
                return new BinControl(pluginHost, new BIN(pluginHost, archivo, id));

            if (archivo.ToUpper().EndsWith(".R00"))
                return new R00(pluginHost, archivo, id).Get_Control();

            return new Control();
        }

        public String Pack(ref sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id) { return new sFolder(); }
    }
}
