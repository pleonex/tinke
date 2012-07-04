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
using Ekona;
using Ekona.Images;

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
        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper().EndsWith(".ANCL"))
                return Format.Palette;
            else if (file.name.ToUpper().EndsWith(".ANCG"))
                return Format.Tile;
            else if (file.name.ToUpper().EndsWith(".ATEX"))
                return Format.Tile;
            else if (file.name.ToUpper().EndsWith(".ANSC"))
                return Format.Map;
            else if (file.name.ToUpper().EndsWith("FAT.BIN") || file.name.ToUpper().EndsWith("FNT.BIN") ||
                    file.name.ToUpper().EndsWith("ARM9.BIN") || file.name.ToUpper().EndsWith("ARM7.BIN"))
                return Format.System;
            else if (file.name.ToUpper().EndsWith(".BIN") || file.name.ToUpper().EndsWith(".R00"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".ANCL"))
            {
                ANCL ancl = new ANCL(file.path, file.id, file.name);
                pluginHost.Set_Palette(ancl);
            }
            else if (file.name.ToUpper().EndsWith(".ANCG"))
            {
                ANCG ancg = new ANCG(file.path, file.id, file.name);
                pluginHost.Set_Image(ancg);
            }
            else if (file.name.ToUpper().EndsWith(".ANSC"))
            {
                ANSC ansc = new ANSC(file.path, file.id, file.name);
                pluginHost.Set_Map(ansc);
            }
            else if (file.name.ToUpper().EndsWith(".ATEX"))
            {
                ATEX atex = new ATEX(file.path, file.id, file.name);
                pluginHost.Set_Image(atex);
            }
        }
        public Control Show_Info(sFile file)
        {
            Read(file);

            if (file.name.ToUpper().EndsWith(".ANCL"))
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".ANCG") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".ANSC") && pluginHost.Get_Palette().Loaded && pluginHost.Get_Image().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette(), pluginHost.Get_Map());

            if (file.name.ToUpper().EndsWith(".ATEX") && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (file.name.ToUpper().EndsWith(".BIN") && !file.name.ToUpper().EndsWith("fat.bin") && !file.name.ToUpper().EndsWith("fnt.bin") &&
                !file.name.ToUpper().EndsWith("arm9.bin") && !file.name.ToUpper().EndsWith("arm7.bin"))
                return new BinControl(pluginHost, new BIN(file.path, file.id, file.name));

            if (file.name.ToUpper().EndsWith(".R00"))
                return new R00(pluginHost, file.path, file.id).Get_Control();

            return new Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
