// ----------------------------------------------------------------------
// <copyright file="Images.cs" company="none">

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
// <date>21/04/2012 11:16:21</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace TC_UTK
{
    class Images : MapBase
    {
        IPluginHost pluginHost;
        ImageBase img;
        PaletteBase pal;
        bool ismap;

        public Images(IPluginHost pluginHost, string file, int id)
            : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            ushort h = br.ReadUInt16();
            if (h == 0x10 || h == 0x12 || h == 0x18 || h == 0x1A)
            {
                br.Close();
                Read12(fileIn);
                return;
            }
            br.BaseStream.Position = 0;

            uint info_offset = br.ReadUInt32();
            uint data_size = br.ReadUInt32();
            byte[] data;

            if ((data_size >> 31) == 1)
            {
                // Data encoded
                data_size = data_size & 0xFFFFFF;
                data = br.ReadBytes((int)data_size);

                Encryption enc = new Encryption(data);
                data = enc.Decrypt();
            }
            else
                data = br.ReadBytes((int)data_size);

            br.BaseStream.Position = info_offset + 4;
            uint pal_offset = br.ReadUInt32();
            uint unk_offset = br.ReadUInt32();

            // Get palette
            br.BaseStream.Position = pal_offset + 4;
            Color[] colors = Actions.BGR555ToColor(br.ReadBytes(0x200));
            pal = new RawPalette(new Color[][] { colors }, false, ColorFormat.colors256);

            // Unknown (probably like a map)
            br.BaseStream.Position = unk_offset;
            int width = br.ReadByte() * 8;
            int height = br.ReadByte() * 8;

            img = new RawImage(data, TileForm.Horizontal, ColorFormat.colors256, width, height, false);
            ismap = false;

            br.Close();
        }
        public void Read12(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Read header
            ushort bpp = br.ReadUInt16();
            int width = br.ReadByte() * 8;
            int height = br.ReadByte() * 8;
            ushort map_offset = br.ReadUInt16();
            ushort map_size = br.ReadUInt16();
            ushort data_offset = br.ReadUInt16();
            ushort data_size = br.ReadUInt16();
            ushort pal_offset = br.ReadUInt16();
            ushort pal_size = br.ReadUInt16();

            // Read map
            br.BaseStream.Position = map_offset;
            NTFS[] map = new NTFS[map_size / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());
            Set_Map(map, false, width, height);

            // Read image data
            ColorFormat format = ColorFormat.colors256;
            if (bpp == 0x12 || bpp == 0x1A)
                format = ColorFormat.colors16;
            else if (bpp == 0x10 || bpp == 0x18)
                format = ColorFormat.colors256;

            br.BaseStream.Position = data_offset;
            byte[] data = br.ReadBytes(data_size);
            Encryption enc = new Encryption(data);
            data = enc.Decrypt();

            img = new RawImage(data, TileForm.Horizontal, format, width, height, false);

            // Read palette
            if ((bpp == 0x18 || bpp == 0x1A) && pluginHost.Get_Palette().Loaded)
                pal = pluginHost.Get_Palette();
            else
            {
                br.BaseStream.Position = pal_offset;
                Color[] colors = Actions.BGR555ToColor(br.ReadBytes(pal_size));
                pal = new RawPalette(new Color[][] { colors }, false, ColorFormat.colors16);
            }

            br.Close();
            ismap = true;

            Console.WriteLine("Image with bpp: {0}", bpp.ToString("x"));
        }

        public ImageControl Get_Control()
        {
            if (ismap)
                return new ImageControl(pluginHost, img, pal, this);
            else
                return new ImageControl(pluginHost, img, pal);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
