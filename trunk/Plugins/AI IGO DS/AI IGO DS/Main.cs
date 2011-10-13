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
            else if (nombre.EndsWith("FNT.BIN") || nombre.EndsWith(".SRL") || nombre.EndsWith("FAT.BIN") ||
                nombre.EndsWith("ARM9.BIN") || nombre.EndsWith("ARM7.BIN"))
                return Format.System;
            else if (nombre.EndsWith(".BIN") || nombre.EndsWith(".R00"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".ANCL"))
                ANCL.Leer(archivo, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ANCG"))
                ANCG.Leer(archivo, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ANSC"))
                ANSC.Leer(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ATEX"))
                ATEX.Leer(archivo, pluginHost);
        }
        public Control Show_Info(string archivo, int id)
        {
            Read(archivo, id);

            if (archivo.ToUpper().EndsWith(".ANCL"))
                return new PaletteControl(pluginHost);
            if (archivo.ToUpper().EndsWith(".ANCG") && pluginHost.Get_NCLR().header.file_size != 0x00)
                return new BinControl(pluginHost, false);
            if (archivo.ToUpper().EndsWith(".ANSC") && pluginHost.Get_NCLR().header.file_size != 0x00 &&
                pluginHost.Get_NCGR().header.file_size != 0x00)
                return new BinControl(pluginHost, true);
            if (archivo.ToUpper().EndsWith(".BIN"))
                return BIN.Leer(archivo, pluginHost);
            if (archivo.ToUpper().EndsWith(".R00"))
                return R00.Leer(archivo, pluginHost);
            if (archivo.ToUpper().EndsWith(".ATEX") && pluginHost.Get_NCLR().header.file_size != 0x00)
                return new BinControl(pluginHost, false);

            return new Control();
        }

        public String Pack(sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id) { return new sFolder(); }
    }
}
