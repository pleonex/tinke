// ----------------------------------------------------------------------
// <copyright file="PCT.cs" company="none">

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
// <date>25/04/2012 20:10:58</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace Images.Games
{
    public class PCT : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new string(System.Text.Encoding.ASCII.GetChars(magic));
            if (ext == "STD ")
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }
        public bool IsCompatible()
        {
            if (gameCode == "TBWJ")
                return true;

            return false;
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            // BTM only support STD files
            new STD(pluginHost, file.path, file.id, file.name);
            return new ImageControl(pluginHost, true);
        }

        public sFolder Unpack(sFile file)
        {
            return new sFolder();
        }
        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }

    }

    class STD : MapBase
    {
        IPluginHost pluginHost;

        public STD(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.id = id;
            if (fileName == "")
                this.fileName = Path.GetFileName(file);
            else
                this.fileName = fileName;
            this.pluginHost = pluginHost;

            Read(file);
        }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Header
            char[] type = br.ReadChars(4);  // "STD "
            uint unknown_header = br.ReadUInt32();
            ushort clrformat = br.ReadUInt16();
            ColorFormat format = ColorFormat.colors256;
            if (clrformat == 0)
                format = ColorFormat.colors16;
            br.BaseStream.Position = 0x10;  // Unknown values

            // Palette
            ushort unknown_pal = br.ReadUInt16();
            ushort num_colors = br.ReadUInt16();
            Color[] colors = Actions.BGR555ToColor(br.ReadBytes(num_colors * 2));
            PaletteBase palette = new RawPalette(new Color[][] { colors }, false, format);

            // Map
            int tile_width = br.ReadUInt16() * 8;
            int tile_height = br.ReadUInt16() * 8;
            int tile_size = tile_width * tile_height;
            if (tile_height != tile_width)
                System.Windows.Forms.MessageBox.Show("Different tile size; height != width");

            int width = br.ReadUInt16() * tile_width;
            int height = br.ReadUInt16() * tile_height;
            uint unknown2_map = br.ReadUInt32();    // Padding ?
            NTFS[] map = new NTFS[width * height / tile_size];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new NTFS();
                uint mapv = br.ReadUInt32();
                map[i].nTile = (ushort)(mapv & 0xFFFFFF);
                map[i].nTile /= (ushort)(tile_size / 0x40);
                map[i].nPalette = (byte)(mapv >> 28);
            }
            Set_Map(map, false, width, height);

            // Image
            if (clrformat == 2)
                format = ColorFormat.A3I5;
            else if (clrformat == 3)
                format = ColorFormat.A5I3;
            else if (clrformat != 0 && clrformat != 1)
                System.Windows.Forms.MessageBox.Show("ClrFormat: " + clrformat.ToString());
            byte[] tiles = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            ImageBase image = new RawImage(tiles, TileForm.Horizontal, format, width, height, false);
            image.TileSize = tile_width;

            br.Close();

            pluginHost.Set_Palette(palette);
            pluginHost.Set_Image(image);
            pluginHost.Set_Map(this);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
        }
    }
}
