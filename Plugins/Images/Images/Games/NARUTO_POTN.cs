// ----------------------------------------------------------------------
// <copyright file="NARUTO_POTN.cs" company="none">

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
// <date>17/07/2012 14:52:12</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images.Games
{
    public class NARUTO_POTN : IGamePlugin
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
            if (gameCode == "YN5E")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.EndsWith(".acl"))
                return Format.Palette;
            else if (file.name.EndsWith(".acg"))
                return Format.Tile;
            else if (file.name.EndsWith(".asc"))
                return Format.Map;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            if (file.name.EndsWith(".acl"))
            {
                PaletteBase palette = new RawPalette(file.path, file.id, true, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return new PaletteControl(pluginHost);
            }

            if (file.name.EndsWith(".acg"))
            {
                ColorFormat depth = (pluginHost.Get_Palette().Loaded ? pluginHost.Get_Palette().Depth : ColorFormat.colors256);
                ImageBase image = new RawImage(file.path, file.id, TileForm.Horizontal, depth, true, 0, -1, file.name);
                pluginHost.Set_Image(image);
                return new ImageControl(pluginHost, false);
            }

            if (file.name.EndsWith(".asc"))
            {
                MapBase map = new Naruto_ASC(file);
                pluginHost.Set_Map(map);
                return new ImageControl(pluginHost, true);
            }

            return new System.Windows.Forms.Control();
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
        public sFolder Unpack(sFile file)
        {
            throw new NotImplementedException();
        }
    }

    public class Naruto_ASC : MapBase
    {
        public Naruto_ASC(sFile file) : base(file.path, file.id, file.name) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            NTFS[] maps = new NTFS[br.BaseStream.Length / 2];

            for (int i = 0; i < maps.Length; i++)
            {
                ushort v = br.ReadUInt16();
                maps[i].nTile = (ushort)(v & 0x1FF);
                maps[i].nPalette = (byte)(v >> 14);
            }

            br.Close();
            Set_Map(maps, false);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
