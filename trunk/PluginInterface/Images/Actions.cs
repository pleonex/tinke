﻿/*
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
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace PluginInterface.Images
{

    public enum TileForm
    {
        Lineal,
        Horizontal,
        Vertical
    }
    public enum ColorFormat : byte
    {
        A3I5 = 1,           // 8 bits-> 0-4: index; 5-7: alpha
        colors4 = 2,        // 2 bits for 4 colors
        colors16 = 3,       // 4 bits for 16 colors
        colors256 = 4,     // 8 bits for 256 colors
        texel4x4 = 5,       // 32bits, 2bits per Texel (only in textures)
        A5I3 = 6,           // 8 bits-> 0-2: index; 3-7: alpha
        direct = 7,         // 16bits, color with BGR555 encoding
        colors2 = 8          // 1 bit for 2 colors
    }

    public static class Actions
    {
        #region Palette
        /// <summary>
        /// Convert bytes encoding with BGR555 to colors
        /// </summary>
        /// <param name="bytes">Bytes encoded with BGR555</param>
        /// <returns>Colors</returns>
        public static Color[] BGR555ToColor(byte[] bytes)
        {
            Color[] colors = new Color[bytes.Length / 2];

            for (int i = 0; i < bytes.Length / 2; i++)
                colors[i] = BGR555ToColor(bytes[i * 2], bytes[i * 2 + 1]);

            return colors;
        }
        /// <summary>
        /// Convert two bytes encoded with BGR555 to a color
        /// </summary>
        public static Color BGR555ToColor(byte byte1, byte byte2)
        {
            int r, b, g;
            short bgr = BitConverter.ToInt16(new Byte[] { byte1, byte2 }, 0);

            r = (bgr & 0x001F) * 0x08;
            g = ((bgr & 0x03E0) >> 5) * 0x08;
            b = ((bgr & 0x7C00) >> 10) * 0x08;

            return Color.FromArgb(r, g, b);
        }

        public static Bitmap Get_Image(Color[] colors)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;

            int height = (colors.Length / 0x10);
            if (colors.Length % 0x10 != 0)
                height++;


            Bitmap palette = new Bitmap(160, height * 10, format);

            ColorPalette pal = palette.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                if (i >= colors.Length)
                    pal.Entries[i] = Color.Black;
                else
                    pal.Entries[i] = colors[i];
            }
            palette.Palette = pal;

            BitmapData data = palette.LockBits(new Rectangle(new Point(0, 0), palette.Size), ImageLockMode.WriteOnly, format);
            Byte[] tiles = new Byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, tiles, 0, tiles.Length);

            bool end = false;

            for (int i = 0; i < 16 & !end; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (colors.Length == j + 16 * i)
                    {
                        end = true;
                        break;
                    }

                    for (int k = 0; k < 10; k++)
                        for (int q = 0; q < 10; q++)
                            tiles[(j * 10 + q) + (i * 10 + k) * data.Stride] = (byte)(j + 16 * i);
                }
            }

            Marshal.Copy(tiles, 0, data.Scan0, tiles.Length);
            palette.UnlockBits(data);

            return palette;
        }

        public static Color[][] Palette_16To256(Color[][] palette)
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
        public static Color[][] Palette_256To16(Color[][] palette)
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

        #region Tiles
        /// <summary>
        /// Not recommend to use, the main problem is with map files, where 4bpp can use palette with 256 colors(16 palettes of 16 colors).
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="tiles"></param>
        /// <param name="color_format"></param>
        /// <param name="form">Only accept No tiled</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap Get_Image(Color[] colors, Byte[] tiles, ColorFormat color_format, TileForm form,
            int width, int height)
        {
            /*
             *   A3I5 = 1,           // 8 bits-> 0-4: index; 5-7: alpha
             *   colors4 = 2,        // 2 bits for 4 colors
             *   colors16 = 3,       // 4 bits for 16 colors
             *   colors256  = 4,     // 8 bits for 256 colors
             *   A5I3 = 6,           // 8 bits-> 0-2: index; 3-7: alpha
             *   direct = 7,         // 16bits, color with BGR555 encoding
             *   colors2= 8          // 1 bit for 2 colors
             */

            PixelFormat format = PixelFormat.Canonical;
            if (color_format == ColorFormat.colors16 || color_format == ColorFormat.colors4)
                format = PixelFormat.Format4bppIndexed;
            else if (color_format == ColorFormat.colors2)
                format = PixelFormat.Format1bppIndexed;
            else if (color_format == ColorFormat.direct)
                format = PixelFormat.Format16bppRgb555;
            else if (color_format == ColorFormat.colors256)
                format = PixelFormat.Format8bppIndexed;
            else if (color_format == ColorFormat.A3I5 || color_format == ColorFormat.A5I3)
                format = PixelFormat.Format32bppArgb;


            Bitmap image = new Bitmap(width, height, format);

            // Write the palette
            int length = 0;
            if (color_format == ColorFormat.colors16 || color_format == ColorFormat.colors4)
                length = 16;
            else if (color_format == ColorFormat.colors2)
                length = 2;
            else if (color_format == ColorFormat.colors256)
                length = 256;

            if (length != 0)
            {
                ColorPalette palette = image.Palette;
                for (int i = 0; i < length; i++)
                {
                    if (i < colors.Length)
                        palette.Entries[i] = colors[i];
                    else
                        palette.Entries[i] = Color.Black;
                }
                image.Palette = palette;
            }

            // Write tiles
            BitmapData data = image.LockBits(new Rectangle(new Point(0, 0), image.Size), ImageLockMode.WriteOnly, format);
            Byte[] tiles_img = new Byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, tiles_img, 0, tiles_img.Length);

            if (color_format == ColorFormat.colors16 || color_format == ColorFormat.colors2 ||
                color_format == ColorFormat.colors256 || color_format == ColorFormat.direct)
                tiles_img = tiles;
            else if (color_format == ColorFormat.A3I5 || color_format == ColorFormat.A5I3)
                tiles_img = AlphaIndexTo32ARGB(colors, tiles, color_format);
            else if (color_format == ColorFormat.colors4)
                tiles_img = Bpp2ToBpp4(tiles);

            Marshal.Copy(tiles, 0, data.Scan0, tiles_img.Length);
            image.UnlockBits(data);

            return image;
        }

        public static Byte[] AlphaIndexTo32ARGB(Color[] palette, byte[] data, ColorFormat format)
        {
            Byte[] direct = new byte[data.Length * 4];

            for (int i = 0; i < data.Length; i++)
            {
                Color color = Color.Transparent;
                if (format == ColorFormat.A3I5)
                {
                    int colorIndex = data[i] & 0x1F;
                    int alpha = (data[i] >> 5);
                    alpha = ((alpha * 4) + (alpha / 2)) * 8;
                    color = Color.FromArgb(alpha,
                        palette[colorIndex].R,
                        palette[colorIndex].G,
                        palette[colorIndex].B);
                }
                else if (format == ColorFormat.A5I3)
                {
                    int colorIndex = data[i] & 0x7;
                    int alpha = (data[i] >> 3);
                    alpha *= 8;
                    color = Color.FromArgb(alpha,
                        palette[colorIndex].R,
                        palette[colorIndex].G,
                        palette[colorIndex].B);
                }

                Byte[] argb32 = BitConverter.GetBytes(color.ToArgb());
                Array.Copy(argb32, 0, direct, i * 4, 4);
            }

            return direct;
        }
        public static Byte[] Bpp2ToBpp4(byte[] data)
        {
            Byte[] bpp4 = new byte[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                byte b1 = (byte)(data[i] & 0x3);
                b1 += (byte)(((data[i] >> 2) & 0x3) << 4);

                byte b2 = (byte)((data[i] >> 4) & 0x3);
                b2 += (byte)(((data[i] >> 6) & 0x3) << 4);

                bpp4[i * 2] = b1;
                bpp4[i * 2 + 1] = b2;
            }

            return bpp4;
        }

        public static Bitmap Get_Image(Byte[] tiles, Byte[] tile_pal, Color[][] palette, ColorFormat format,
            int width, int height)
        {
            if (tiles.Length == 0)
                return new Bitmap(1, 1);

            Bitmap image = new Bitmap(width, height);

            int pos = 0;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int num_pal = 0;
                    if (tile_pal.Length > w + h * width)
                        num_pal = tile_pal[w + h * width];

                    if (num_pal >= palette.Length)
                        num_pal = 0;

                    Color color = Get_Color(tiles, palette[num_pal], format, ref pos);

                    image.SetPixel(w, h, color);
                }
            }
            return image;
        }

        public static Color Get_Color(Byte[] data, Color[] palette, ColorFormat format, ref int pos)
        {
            Color color = Color.Transparent;
            int alpha, index;

            switch (format)
            {
                case ColorFormat.A3I5:
                    if (data.Length <= pos) break;
                    index = data[pos] & 0x1F;
                    alpha = (data[pos] >> 5);
                    alpha = ((alpha * 4) + (alpha / 2)) * 8;
                    if (palette.Length > index)
                        color = Color.FromArgb(alpha,
                            palette[index].R,
                            palette[index].G,
                            palette[index].B);

                    pos++;
                    break;
                case ColorFormat.A5I3:
                    if (data.Length <= pos) break;
                    index = data[pos] & 0x7;
                    alpha = (data[pos] >> 3);
                    alpha *= 8;
                    if (palette.Length > index)
                        color = Color.FromArgb(alpha,
                            palette[index].R,
                            palette[index].G,
                            palette[index].B);

                    pos++;
                    break;

                case ColorFormat.colors2:
                    if (data.Length <= (pos / 8)) break;
                    byte bit1 = data[pos / 8];
                    index = ByteToBits(bit1)[pos % 8];
                        if (palette.Length > index)
                    color = palette[index];
                    pos++;
                    break;
                case ColorFormat.colors4:
                    if (data.Length <= (pos / 4)) break;
                    byte bit2 = data[pos / 4];
                    index = ByteToBit2(bit2)[pos % 4];
                    if (palette.Length > index)
                        color = palette[index];
                    pos++;
                    break;
                case ColorFormat.colors16:
                    if (data.Length <= (pos / 2)) break;
                    byte bit4 = data[pos / 2];
                    index = ByteToBit4(bit4)[pos % 2];
                    if (palette.Length > index)
                        color = palette[index];
                    pos++;
                    break;
                case ColorFormat.colors256:
                    if (data.Length > pos && palette.Length > data[pos])
                        color = palette[data[pos]];
                    pos++;
                    break;

                case ColorFormat.direct:    // RGB555
                    if (pos + 2 >= data.Length)
                        break;

                    ushort byteColor = BitConverter.ToUInt16(data, pos);
                    color = Color.FromArgb(
                        ((byteColor >> 15) == 0 ? 255 : 0),
                        (byteColor & 0x1F) * 8,
                        ((byteColor >> 5) & 0x1F) * 8,
                        ((byteColor >> 10) & 0x1F) * 8);
                    pos += 2;
                    break;

                case ColorFormat.texel4x4:
                    throw new NotSupportedException("Compressed texel 4x4 not supported yet");
                default:
                    throw new FormatException("Unknown color format");
            }

            return color;
        }

        public static byte[] HorizontalToLineal(byte[] horizontal, int tilesX, int tilesY, int tile_width)
        {
            Byte[] lineal = new byte[horizontal.Length];

            int pos = 0;
            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    // Get the tile data
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < tile_width; w++)
                        {
                            if ((w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * 8 * tile_width >= lineal.Length)
                                continue;
                            if (pos >= lineal.Length)
                                continue;

                            lineal[pos++] = horizontal[(w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * 8 * tile_width];
                        }
                    }
                }
            }

            return lineal;
        }
        public static byte[] LinealToHorizontal(byte[] lineal, int tilesX, int tilesY, int tile_width)
        {
            byte[] horizontal = new byte[lineal.Length];

            int pos = 0;
            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    // Get the tile data
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < tile_width; w++)
                        {
                            if ((w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * 8 * tile_width >= lineal.Length)
                                continue;
                            if (pos >= lineal.Length)
                                continue;

                            horizontal[(w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * 8 * tile_width] = lineal[pos++];
                        }
                    }
                }
            }

            return horizontal;
        }

        public static Byte[] ByteToBits(Byte data)
        {
            List<Byte> bits = new List<byte>();

            for (int j = 7; j >= 0; j--)
                bits.Add((byte)((data >> j) & 1));

            return bits.ToArray();
        }
        public static Byte[] ByteToBit2(Byte data)
        {
            Byte[] bit2 = new byte[4];

            bit2[0] = (byte)(data & 0x3);
            bit2[1] = (byte)((data >> 2) & 0x3);
            bit2[2] = (byte)((data >> 4) & 0x3);
            bit2[3] = (byte)((data >> 6) & 0x3);

            return bit2;
        }
        public static Byte[] ByteToBit4(Byte data)
        {
            Byte[] bit4 = new Byte[2];

            bit4[0] = (byte)(data & 0x0F);
            bit4[1] = (byte)((data & 0xF0) >> 4);

            return bit4;
        }
        #endregion

        #region Map
        public static Byte[] Apply_Map(NTFS[] map, Byte[] tiles, out Byte[] tile_pal, int tile_width, int startInfo = 0)
        {
            int tilesize = tile_width * 8;
            int num_tiles = tiles.Length / tilesize;

            List<Byte> bytes = new List<byte>();
            tile_pal = new byte[(map.Length - startInfo) * 64];

            for (int i = startInfo; i < map.Length; i++)
            {

                if (map[i].nTile >= num_tiles)
                    map[i].nTile = 0;

                Byte[] currTile = new byte[tilesize];
                if (map[i].nTile * tilesize + tilesize > tiles.Length)
                    map[i].nTile = 0;

                Array.Copy(tiles, map[i].nTile * tilesize, currTile, 0, tilesize);

                if (map[i].xFlip == 1)
                    currTile = XFlip(currTile, tile_width);
                if (map[i].yFlip == 1)
                    currTile = YFlip(currTile, tile_width);

                bytes.AddRange(currTile);

                for (int t = 0; t < 64; t++)
                    tile_pal[i * 64 + t] = map[i].nPalette;
            }

            return bytes.ToArray();
        }
        public static Byte[] XFlip(Byte[] tile, int tile_width)
        {
            byte[] newTile = new byte[tile.Length];

            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < tile_width / 2; w++)
                {
                    newTile[w + h * tile_width] = tile[((tile_width - 1) - w) + h * tile_width];
                    newTile[((tile_width - 1) - w) + h * tile_width] = tile[w + h * tile_width];
                }
            }
            return newTile;
        }
        public static Byte[] YFlip(Byte[] tile, int tile_width)
        {
            byte[] newTile = new byte[tile.Length];

            for (int h = 0; h < 4; h++)
            {
                for (int w = 0; w < tile_width; w++)
                {
                    newTile[w + h * tile_width] = tile[w + (7 - h) * tile_width];
                    newTile[w + (7 - h) * tile_width] = tile[w + h * tile_width];
                }
            }
            return newTile;
        }
        #endregion
    }
}
