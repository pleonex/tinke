// ----------------------------------------------------------------------
// <copyright file="IMY.cs" company="none">

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
// <date>21/04/2012 2:13:37</date>
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

            Console.WriteLine("==========NEW DIG=============");
            Read(file);
            Console.WriteLine("==========END DIG=============");
        }


        public override void Read(string fileIn)
        {
            byte[] data = File.ReadAllBytes(fileIn);

            byte paletteType = data[5];
            Console.WriteLine("paletteType:" + paletteType);
            byte paletteSize = data[6];
            Console.WriteLine("paletteSize:" + paletteSize);
            short width = BitConverter.ToInt16(data, 8);
            Console.WriteLine("width:"+width);
            short height = BitConverter.ToInt16(data, 10);
            Console.WriteLine("height:" + height);

            int paletteEnd = paletteSize * 32 + 12;
            Console.WriteLine("paletteEnd:"+paletteEnd);

            int startPalette = 12;
            int position = startPalette;

            if (BitConverter.ToInt32(data, position) == 0)
            {
                position += 4;
                while (BitConverter.ToInt32(data, position) == 0) { position += 4; }
                startPalette = position;
            }
            Console.WriteLine("startPalette after while:"+startPalette);
            int paletteActualSize = paletteEnd - startPalette;
            Console.WriteLine("PaletteActualSize:" + paletteActualSize);

            position = startPalette;
            ColorFormat format;
            RawPalette palette;

            if (paletteType == 16)
            {
                Console.WriteLine("== Palete Type 16 ==");
                int paletteColors = 16;
                int paletteColorSize = paletteColors * 2;
                format = ColorFormat.colors16;
                Color[][] palettes;
                decimal paletteNumber = Math.Ceiling((decimal)paletteActualSize / paletteColorSize);
                Console.WriteLine("Number of Palettes:" +paletteNumber);

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
                Console.WriteLine("== Palete Type 256 ==");
                format = ColorFormat.colors256;
                byte[] aux = new byte[paletteActualSize];
                Array.Copy(data, position, aux, 0, paletteActualSize);
                Color[] p = Actions.BGR555ToColor(aux);
                palette = new RawPalette(new Color[][] { p }, false, format);

            }
            position = paletteEnd;

            Console.WriteLine("NumberOfPalettes:" + palette.NumberOfPalettes);

            pluginHost.Set_Palette(palette);

            Console.WriteLine("position before image:" + position);

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
