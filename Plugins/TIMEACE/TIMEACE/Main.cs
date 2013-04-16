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

// <author>Daviex94</author>
// <email>david.iuffri94@hotmail.it</email>
// <date>05/07/2012 2:41:51</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Ekona;
using Ekona.Images;

namespace TIMEACE
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
            if (gameCode == "AE3E")
                return true;

            return false;
        }
        
        public Format Get_Format(sFile file, byte[] magic)
        {
            if (this.IsImage(file.id))
            {
                return Format.FullImage;
            }

            if (file.name.EndsWith(".cfg"))
            {
                return Format.Text;
            }

            if (file.name.EndsWith(".area") || file.name.EndsWith(".unit") ||
                file.name.EndsWith(".spawn"))
            {
                return Format.Text;
            }

            if (file.name.EndsWith(".fsf") || file.name.EndsWith(".fss"))
            {
                return Format.Text;
            }

            if (file.name.EndsWith(".tmpl") || file.name.EndsWith(".cam") ||
                file.name.EndsWith(".ncam"))
            {
                return Format.Text;
            }

            if (file.name.EndsWith(".fca") || file.name.EndsWith(".col"))
            {
                return Format.Text;
            }

            return Format.Unknown;
        }

        private bool IsImage(int id)
        {
            if ((id >= 0x0BE && id <= 0x10B) ||
                (id >= 0x10C && id <= 0x1B1))
                return true;

            return false;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }

        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }

        public sFolder Unpack(sFile file)
        {
            throw new NotImplementedException();
        }

        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            if (this.IsImage(file.id))
            {
                return this.ShowImage(file);
            }

            return new TextEditor(this.pluginHost, file);
        }

        private System.Windows.Forms.Control ShowImage(sFile file)
        {
            #region Palette
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.ReadUInt32(); // 4 Bytes Stupid
            uint num_colors = 256;
            byte[] pal = br.ReadBytes((int)num_colors * 2);
            Color[] colors = Actions.BGR555ToColor(pal);
            PaletteBase palette = new RawPalette(colors, false, ColorFormat.colors256, file.name);
            br.ReadUInt32(); // 4 Bytes Stupid
            #endregion

            #region Map
            NTFS[] map_info = new NTFS[1024];
            for (int i = 0; i < 1024; i++)
            {
                ushort value = br.ReadUInt16();
                map_info[i] = Actions.MapInfo(value);
            }
            MapBase map = new RawMap(map_info, 256, 192, false, file.name);
            #endregion

            #region Tiles
            uint size_section = (uint)(br.ReadUInt32() * 64);
            Console.WriteLine("Size section: " + size_section.ToString("x"));
            byte[] readsize = br.ReadBytes((int)size_section);
            ImageBase image = new RawImage(readsize, TileForm.Horizontal, ColorFormat.colors256, 256, 192, false, file.name);
            #endregion

            br.Close();

            pluginHost.Set_Palette(palette);
            pluginHost.Set_Image(image);
            pluginHost.Set_Map(map);
            
            return new ImageControl(pluginHost, image, palette, map);
        }
    }
}
