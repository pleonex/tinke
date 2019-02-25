// ----------------------------------------------------------------------
// <copyright file="DIG.cs" company="none">

// Copyright (C) 2019
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

// <author>Priverop</author>
// <contact>https://github.com/priverop/</contact>
// <date>25/02/2019</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace JUS
{
    class DIG : ImageBase
    {
        IPluginHost pluginHost;

        public DIG(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = fileName;

            Read(file);
        }

        public override void Read(string fileIn)
        {
            byte[] data = File.ReadAllBytes(fileIn);

            byte paletteType = data[5];
            byte paletteSize = data[6];

            short width = BitConverter.ToInt16(data, 8);
            short height = BitConverter.ToInt16(data, 10);

            int paletteEnd = paletteSize * 32 + 12;

            int startPalette = 12;
            int position = startPalette;

            if (BitConverter.ToInt32(data, position) == 0)
            {
                position += 4;
                while (BitConverter.ToInt32(data, position) == 0) { position += 4; }
                startPalette = position;
            }

            int paletteActualSize = paletteEnd - startPalette;

            position = startPalette;
            ColorFormat format;
            RawPalette palette;

            if (paletteType == 16)
            {
                int paletteColors = 16;
                int paletteColorSize = paletteColors * 2;
                format = ColorFormat.colors16;
                Color[][] palettes;
                decimal paletteNumber = Math.Ceiling((decimal)paletteActualSize / paletteColorSize);

                palettes = new Color[(int)paletteNumber][];

                for (int i = 0; i < paletteNumber; i++)
                {
                    byte[] aux = new byte[paletteColorSize];
                    Array.Copy(data, position, aux, 0, paletteColorSize);

                    palettes[i] = Actions.BGR555ToColor(aux);
                }

                palette = new RawPalette(palettes, false, format);
            }
            else
            {
                format = ColorFormat.colors256;
                byte[] aux = new byte[paletteActualSize];
                Array.Copy(data, position, aux, 0, paletteActualSize);
                Color[] p = Actions.BGR555ToColor(aux);
                palette = new RawPalette(new Color[][] { p }, false, format);

            }
            position = paletteEnd;

            pluginHost.Set_Palette(palette);

            // Get image
            byte[] tiles = new byte[data.Length - position];
            Array.Copy(data, position, tiles, 0, data.Length - position);
            Set_Tiles(tiles, width, height, format, TileForm.Horizontal, false);

            pluginHost.Set_Image(this);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
        }
    }
}
