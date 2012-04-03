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
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using PluginInterface;

namespace Tinke
{
    public static class Convertir
    {
        #region Palette

        public static Color[][] Palette_4bppTo8bpp(Color[][] palette)
        {
            // Get the colours of all the palettes in BGR555 encoding
            List<Color> paletteColor = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                paletteColor.AddRange(palette[i]);

            // Set the colours in one palette
            Color[][] newPal = new Color[1][];
            newPal[0] = paletteColor.ToArray();

            return newPal;
        }
        public static Color[][] Palette_8bppTo4bpp(Color[][] palette)
        {
            Color[][] newPal;

            int isExact = (int)palette[0].Length % 0x10;

            if (isExact == 0)
            {
                newPal = new Color[palette[0].Length / 0x10][];
                for (int i = 0; i < newPal.Length; i++)
                {
                    newPal[i] = new Color[0x10];
                    Array.Copy(palette[0], i * 0x10, newPal[i], 0, 0x10);
                }
            }
            else
            {
                newPal = new Color[(palette[0].Length / 0x10) + 1][];
                for (int i = 0; i < newPal.Length - 1; i++)
                {
                    newPal[i] = new Color[0x10];
                    Array.Copy(palette[0], i * 0x10, newPal[i], 0, 0x10);
                }
                Color[] temp = new Color[isExact];
                Array.Copy(palette[0], palette[0].Length / 0x10, temp, 0, isExact);
                newPal[newPal.Length - 1] = temp;
            }

            return newPal;
        }

        #endregion

        #region Map
        #endregion

        /// <summary>
        /// Convierte una array de bytes en formato 4-bit a otro en formato 8-bit
        /// </summary>
        /// <param name="bits4">Datos de entrada en formato 4-bit (valor máximo 15 (0xF))</param>
        /// <returns>Devuelve una array de bytes en 8-bit</returns>
        public static Byte[] Bit4ToBit8(byte[] bits4)
        {
            List<byte> bits8 = new List<byte>();

            for (int i = 0; i < bits4.Length; i += 2)
            {
                int byte1 = bits4[i];
                int byte2 = bits4[i + 1] << 4;
                bits8.Add((byte)(byte1 + byte2));
            }

            return bits8.ToArray();
        }
        /// <summary>
        /// Convierte una array de bytes en formato 8-bit a otro en formato 4-bit
        /// </summary>
        /// <param name="bits8">Datos de entrada en formato 8-bit</param>
        /// <returns>Devuelve una array de bytes en 4-bit</returns>
        public static Byte[] Bit8ToBit4(byte[] bits8)
        {
            List<byte> bits4 = new List<byte>();

            for (int i = 0; i < bits8.Length; i++)
            {
                bits4.Add((byte)(bits8[i] & 0x0F));
                bits4.Add((byte)((bits8[i] & 0xF0) >> 4));
            }

            return bits4.ToArray();
        }
    }
}
