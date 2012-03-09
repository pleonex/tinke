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
        #region Palette
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

        public static int Remove_DuplicatedColors(ref Color[] palette, ref byte[][] tiles)
        {
            List<Color> colors = new List<Color>();
            int first_duplicated_color = -1;

            for (int i = 0; i < palette.Length; i++)
            {
                if (!colors.Contains(palette[i]))
                    colors.Add(palette[i]);
                else        // The color is duplicated
                {
                    int newIndex = colors.IndexOf(palette[i]);
                    Replace_Color(ref tiles, i, newIndex);
                    colors.Add(Color.FromArgb(248, 0, 248));

                    if (first_duplicated_color == -1)
                        first_duplicated_color = i;
                }
            }

            palette = colors.ToArray();
            return first_duplicated_color;
        }
        public static int Remove_NotUsedColors(ref Color[] palette, ref byte[][] tiles)
        {
            int first_notUsed_color = -1;
            List<bool> colors = new List<bool>();
            for (int i = 0; i < palette.Length; i++)
                colors.Add(false);

            for (int i = 0; i < tiles.Length; i++)
                for (int j = 0; j < tiles[i].Length; j++)
                    colors[tiles[i][j]] = true;

            for (int i = 0; i < colors.Count; i++)
                if (!colors[i])
                    first_notUsed_color = i;

            return first_notUsed_color;
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
        public static void Replace_Color(ref byte[][] tiles, int oldIndex, int newIndex)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    if (tiles[i][j] == oldIndex)
                        tiles[i][j] = (byte)newIndex;
                }
            }
        }

        #endregion

        #region Map
        public static NTFS MapInfo(ushort value)
        {
            NTFS mapInfo = new NTFS();
            
            mapInfo.nTile = (ushort)(value & 0x3FF);
            mapInfo.xFlip = (byte)((value >> 10) & 1);
            mapInfo.yFlip = (byte)((value >> 11) & 1);
            mapInfo.nPalette = (byte)((value >> 12) & 0xF);

            return mapInfo;
        }
        public static ushort MapInfo(NTFS map)
        {
            int npalette = map.nPalette << 12;
            int yFlip = map.yFlip << 11;
            int xFlip = map.xFlip << 10;
            int data = npalette + yFlip + xFlip + map.nTile;

            return (ushort)data;
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
