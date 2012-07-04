// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>21/04/2012 11:19:50</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace TC_UTK
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        List<int> pal_id;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;

            pal_id = new List<int>();
            pal_id.Add(0x07);
            pal_id.Add(0xD7);
            pal_id.Add(0xDA);
            pal_id.Add(0xDC);
            pal_id.Add(0xDE);
            pal_id.Add(0xE0);
            pal_id.Add(0xE2);
            pal_id.Add(0xE4);
            pal_id.Add(0xE6);
            pal_id.Add(0xF0);
            pal_id.Add(0xF2);
            pal_id.Add(0xF4);
            pal_id.Add(0xFC);
            pal_id.Add(0xFF);
            pal_id.Add(0x104);
            pal_id.Add(0x108);
            pal_id.Add(0x10A);
            pal_id.Add(0x10D);
            pal_id.Add(0x10F);
            pal_id.Add(0x111);
            pal_id.Add(0x113);
            pal_id.Add(0x118);
        }
        public bool IsCompatible()
        {
            if (gameCode == "YYKE")
                return true;
            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "VXDS")          // It's a video, but we can't open it yet
                return Format.Unknown;

            if ((file.id >= 0x12C & file.id <= 0x165) || (file.id >= 0x65D && file.id <= 0x68A) ||
                file.id == 0x16B || file.id == 0x16C)
                return Format.Text;
            if (((file.id >= 0x1E4 && file.id <= 0x1F4) || (file.id >= 0x1F7 && file.id <= 0x2FC))
                && !file.name.EndsWith(".adx"))
                return Format.Sound;

            if (file.id >= 0x01 && file.id <= 0x1E3 && ext != "TDT ")
            {
                if (magic[0] == 0x10 || magic[0] == 0x12)
                    return Format.FullImage;
                else if (magic[0] == 0x18 || magic[0] == 0x1A)
                    return Format.Tile;

                sFile currFile = pluginHost.Search_File((short)file.id);
                if (currFile.size == 512 || currFile.size == 128 || currFile.size == 32)
                    return Format.Palette;
                else if (magic[0] != 0)
                    return Format.FullImage;
            }

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
        public sFolder Unpack(sFile file)
        {
            return new sFolder();
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            if ((file.id >= 0x12C && file.id <= 0x165) || (file.id >= 0x65D && file.id <= 0x68A) ||
                file.id == 0x16B || file.id == 0x16C)
                return new TextControl(pluginHost, file.path);

            if ((file.id >= 0x1E4 && file.id <= 0x1F4) || (file.id >= 0x1F7 && file.id <= 0x2FC))
            {
                string[] p = new String[] {
                    file.path, "Sounds.Main",
                    file.name + ".adx",
                    "" };
                return (System.Windows.Forms.Control)pluginHost.Call_Plugin(p, file.id, 1);
            }


            if (file.id >= 0x01 && file.id <= 0x1E3)
                if (file.size == 512 || file.size == 128 || file.size == 32)
                {
                    RawPalette palette = new RawPalette(file.path, file.id, false, 0, -1, file.name);
                    pluginHost.Set_Palette(palette);
                    return new PaletteControl(pluginHost);
                }
                else
                    return new Images(pluginHost, file.path, file.id).Get_Control();

            return new System.Windows.Forms.Control();
        }

    }
}
