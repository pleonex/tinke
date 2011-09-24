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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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
        #region Paleta
        /// <summary>
        /// A partir de un array de bytes devuelve un array de colores.
        /// </summary>
        /// <param name="bytes">Bytes para convertir</param>
        /// <returns>Colores de la paleta.</returns>
        public static Color[] BGR555(byte[] bytes)
        {
            Color[] paleta = new Color[bytes.Length / 2];

            for (int i = 0; i < bytes.Length / 2; i++)
            {
                paleta[i] = BGR555(bytes[i * 2], bytes[i * 2 + 1]);
            }
            return paleta;
        }
        /// <summary>
        /// Convierte dos bytes en un color.
        /// </summary>
        /// <param name="byte1">Primer byte</param>
        /// <param name="byte2">Segundo byte</param>
        /// <returns>Color convertido</returns>
        public static Color BGR555(byte byte1, byte byte2)
        {
            int r, b, g;
            short bgr = BitConverter.ToInt16(new Byte[] { byte1, byte2 }, 0);

            r = (bgr & 0x001F) * 0x08;
            g = ((bgr & 0x03E0) >> 5) * 0x08;
            b = ((bgr & 0x7C00) >> 10) * 0x08;

            return System.Drawing.Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// Convert colors to byte with BGR555 encoding
        /// </summary>
        /// <param name="colores">Colors to convert</param>
        /// <returns>Bytes converted</returns>
        public static Byte[] ColorToBGR555(Color[] colores)
        {
            List<Byte> datos = new List<Byte>(colores.Length * 2);

            for (int i = 0; i < colores.Length; i++)
            {
                int r = colores[i].R / 8;
                int g = (colores[i].G / 8) << 5;
                int b = (colores[i].B / 8) << 10;

                ushort bgr = (ushort)(r + g + b);
                datos.AddRange(BitConverter.GetBytes(bgr));
            }

            return datos.ToArray();
        }

        public static TTLP Palette_4bppTo8bpp(TTLP palette)
        {
            TTLP newPalette = new TTLP();

            newPalette.ID = palette.ID;
            newPalette.length = palette.length;
            newPalette.unknown1 = palette.unknown1;

            // Get the colours of all the palettes in BGR555 encoding
            List<Color> paletteColor = new List<Color>();
            for (int i = 0; i < palette.palettes.Length; i++)
                paletteColor.AddRange(palette.palettes[i].colors);

            // Set the colours in one palette
            newPalette.palettes = new NTFP[1];
            newPalette.palettes[0].colors = paletteColor.ToArray();

            newPalette.nColors = (uint)newPalette.palettes[0].colors.Length;
            newPalette.paletteLength = newPalette.nColors * 2;
            newPalette.depth = System.Windows.Forms.ColorDepth.Depth8Bit;

            return newPalette;
        }

        public static TTLP Palette_8bppTo4bpp(TTLP palette)
        {
            TTLP newPalette = new TTLP();

            newPalette.ID = palette.ID;
            newPalette.length = palette.length;
            newPalette.unknown1 = palette.unknown1;
            newPalette.nColors = 0x10;
            newPalette.paletteLength = 0x20;
            newPalette.depth = System.Windows.Forms.ColorDepth.Depth4Bit;

            int isExact = (int)palette.nColors % 0x10;

            if (isExact == 0)
            {
                newPalette.palettes = new NTFP[palette.nColors / 0x10];
                for (int i = 0; i < newPalette.palettes.Length; i++)
                {
                    Color[] tempColor = new Color[0x10];
                    Array.Copy(palette.palettes[0].colors, i * 0x10, tempColor, 0, 0x10);
                    newPalette.palettes[i].colors = tempColor;
                }
            }
            else
            {
                newPalette.palettes = new NTFP[(palette.nColors / 0x10) + 1];
                for (int i = 0; i < newPalette.palettes.Length - 1; i++)
                {
                    Color[] tempColor = new Color[0x10];
                    Array.Copy(palette.palettes[0].colors, i * 0x10, tempColor, 0, 0x10);
                    newPalette.palettes[i].colors = tempColor;
                }
                Color[] temp = new Color[isExact];
                Array.Copy(palette.palettes[0].colors, palette.nColors / 0x10, temp, 0, isExact);
                newPalette.palettes[newPalette.palettes.Length - 1].colors = temp;
            }
           
            return newPalette;
        }
        #endregion

        #region Tiles
        /// <summary>
        /// Convierte una array de Tiles en bytes
        /// </summary>
        /// <param name="tiles">Tiles para convertir</param>
        /// <returns>Array de bytes</returns>
        public static byte[] TilesToBytes(byte[][] tiles, int startByte = 0)
        {
            List<byte> resul = new List<byte>();

            for (int i = 0; i < tiles.Length; i++)
                resul.AddRange(tiles[i]);

            Byte[] bytes = new Byte[resul.Count - startByte];
            Array.Copy(resul.ToArray(), startByte, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>
        /// Convierte una array de bytes en otra de tiles
        /// </summary>
        /// <param name="bytes">Bytes para convertir</param>
        /// <returns>Array de tiles</returns>
        public static byte[][] BytesToTiles(byte[] bytes)
        {
            List<byte[]> resul = new List<byte[]>();
            List<byte> temp = new List<byte>();

            for (int i = 0; i < bytes.Length / 64; i++)
            {
                for (int j = 0; j < 64; j++)
                    temp.Add(bytes[j + i * 64]);

                resul.Add(temp.ToArray());
                temp.Clear();
            }

            return resul.ToArray();
        }

        public static byte[][] BytesToTiles_NoChanged(byte[] bytes, int tilesX, int tilesY)
        {
            List<byte[]> tiles = new List<byte[]>();
            List<byte> temp = new List<byte>();

            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    // Get the tile data
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            temp.Add(bytes[wt * 8 + ht * tilesX * 64 + (w + h * 8 * tilesX)]);
                        }
                    }
                    // Set the tile data
                    tiles.Add(temp.ToArray());
                    temp.Clear();
                }
            }

            return tiles.ToArray();
        }

        public static void Change_Color(ref byte[][] tiles, int oldIndex, int newIndex)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    if (tiles[i][j] == oldIndex)
                        tiles[i][j] = (byte)newIndex;
                    else if (tiles[i][j] == newIndex)
                        tiles[i][j] = (byte)oldIndex;
                }
            }
        }
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
