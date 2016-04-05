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
using System.Drawing;

namespace Ekona.Images
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
        colors256 = 4,      // 8 bits for 256 colors
        texel4x4 = 5,       // 32bits, 2bits per Texel (only in textures)
        A5I3 = 6,           // 8 bits-> 0-2: index; 3-7: alpha
        direct = 7,         // 16bits, color with BGR555 encoding
        colors2 = 8,        // 1 bit for 2 colors
        BGRA32 = 9,   // 32 bits -> ABGR
        A4I4 = 10,
        ABGR32 = 11
    }

    public enum ColorEncoding : byte
    {
        BGR555 = 1,
        BGR = 2,
        RGB = 3
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
        /// <summary>
        /// Convert colors to byte with BGR555 encoding
        /// </summary>
        /// <param name="colores">Colors to convert</param>
        /// <returns>Bytes converted</returns>
        public static Byte[] ColorToBGR555(Color[] colors)
        {
            byte[] data = new byte[colors.Length * 2];

            for (int i = 0; i < colors.Length; i++)
            {
                byte[] bgr = ColorToBGR555(colors[i]);
                data[i * 2] = bgr[0];
                data[i * 2 + 1] = bgr[1];
            }

            return data;
        }
        public static Byte[] ColorToBGRA555(Color color)
        {
            byte[] d = new byte[2];

            int r = color.R / 8;
            int g = (color.G / 8) << 5;
            int b = (color.B / 8) << 10;
            int a = (color.A / 255) << 15;

            ushort bgra = (ushort)(r + g + b + a);
            Array.Copy(BitConverter.GetBytes(bgra), d, 2);

            return d;
        }
        public static Byte[] ColorToBGR555(Color color)
        {
            byte[] d = new byte[2];

            int r = color.R / 8;
            int g = (color.G / 8) << 5;
            int b = (color.B / 8) << 10;

            ushort bgr = (ushort)(r + g + b);
            Array.Copy(BitConverter.GetBytes(bgr), d, 2);

            return d;
        }

        public static Bitmap Get_Image(Color[] colors)
        {
            int height = (colors.Length / 0x10);
            if (colors.Length % 0x10 != 0)
                height++;

            Bitmap palette = new Bitmap(160, height * 10);

            bool end = false;
            for (int i = 0; i < 16 & !end; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (colors.Length <= j + 16 * i)
                    {
                        end = true;
                        break;
                    }

                    for (int k = 0; k < 10; k++)
                        for (int q = 0; q < 10; q++)
                            palette.SetPixel((j * 10 + q), (i * 10 + k), colors[j + 16 * i]);
                }
            }

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
            int width, int height, int start = 0)
        {
            if (tiles.Length == 0)
                return new Bitmap(1, 1);

            Bitmap image = new Bitmap(width, height);

            int pos = start;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    int num_pal = 0;
                    if (tile_pal.Length <= w + h * width)
                        num_pal = 0;
                    else
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
                case ColorFormat.A4I4:
                    if (data.Length <= pos) break;
                    index = data[pos] & 0xF;
                    alpha = (data[pos] >> 4);
                    alpha *= 16;
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
                    index = Helper.BitsConverter.ByteToBits(bit1)[pos % 8];
                    if (palette.Length > index)
                        color = palette[index];
                    pos++;
                    break;
                case ColorFormat.colors4:
                    if (data.Length <= (pos / 4)) break;
                    byte bit2 = data[pos / 4];
                    index = Helper.BitsConverter.ByteToBit2(bit2)[pos % 4];
                    if (palette.Length > index)
                        color = palette[index];
                    pos++;
                    break;
                case ColorFormat.colors16:
                    if (data.Length <= (pos / 2)) break;
                    byte bit4 = data[pos / 2];
                    index = Helper.BitsConverter.ByteToBit4(bit4)[pos % 2];
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
                        ((byteColor >> 15) == 1 ? 255 : 0),
                        (byteColor & 0x1F) * 8,
                        ((byteColor >> 5) & 0x1F) * 8,
                        ((byteColor >> 10) & 0x1F) * 8);
                    pos += 2;
                    break;

                case ColorFormat.BGRA32:
                    if (pos + 4 >= data.Length)
                        break;

                    color = Color.FromArgb(data[pos+3], data[pos+0], data[pos+1], data[pos+2]);
                    pos += 4;
                    break;

                case ColorFormat.ABGR32:
                    if (pos + 4 >= data.Length)
                        break;

                    color = Color.FromArgb(data[pos+0], data[pos+1], data[pos+2], data[pos+3]);
                    pos += 4;
                    break;

                case ColorFormat.texel4x4:
                    throw new NotSupportedException("Compressed texel 4x4 not supported yet");
                default:
                    throw new FormatException("Unknown color format");
            }

            return color;
        }

        public static byte[] HorizontalToLineal(byte[] horizontal, int width, int height, int bpp, int tile_size)
        {
            Byte[] lineal = new byte[horizontal.Length];
            int tile_width = tile_size * bpp / 8;   // Calculate the number of byte per line in the tile
            // pixels per line * bits per pixel / 8 bits per byte
            int tilesX = width / tile_size;
            int tilesY = height / tile_size;

            int pos = 0;
            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    // Get the tile data
                    for (int h = 0; h < tile_size; h++)
                    {
                        for (int w = 0; w < tile_width; w++)
                        {
                            if ((w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * tile_size * tile_width >= lineal.Length)
                                continue;
                            if (pos >= lineal.Length)
                                continue;

                            lineal[pos++] = horizontal[(w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * tile_size * tile_width];
                        }
                    }
                }
            }

            return lineal;
        }
        public static byte[] LinealToHorizontal(byte[] lineal, int width, int height, int bpp, int tile_size)
        {
            byte[] horizontal = new byte[lineal.Length];
            int tile_width = tile_size * bpp / 8;   // Calculate the number of byte per line in the tile
            // pixels per line * bits per pixel / 8 bits per byte
            int tilesX = width / tile_size;
            int tilesY = height / tile_size;

            int pos = 0;
            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    // Get the tile data
                    for (int h = 0; h < tile_size; h++)
                    {
                        for (int w = 0; w < tile_width; w++)
                        {
                            if ((w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * tile_size * tile_width >= lineal.Length)
                                continue;
                            if (pos >= lineal.Length)
                                continue;

                            horizontal[(w + h * tile_width * tilesX) + wt * tile_width + ht * tilesX * tile_size * tile_width] = lineal[pos++];
                        }
                    }
                }
            }

            return horizontal;
        }

        public static int Remove_DuplicatedColors(ref Color[] palette, ref byte[] tiles)
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
        public static int Remove_NotUsedColors(ref Color[] palette, ref byte[] tiles)
        {
            int first_notUsed_color = -1;

            bool[] colors = new bool[palette.Length];
            for (int i = 0; i < palette.Length; i++)
                colors[i] = false;

            for (int i = 0; i < tiles.Length; i++)
                colors[tiles[i]] = true;

            for (int i = 0; i < colors.Length; i++)
                if (!colors[i])
                    first_notUsed_color = i;

            return first_notUsed_color;
        }
        public static void Change_Color(ref byte[] tiles, int oldIndex, int newIndex, ColorFormat format)
        {
            if (format == ColorFormat.colors16) // Yeah, I should improve it
                tiles = Helper.BitsConverter.BytesToBit4(tiles);
            else if (format != ColorFormat.colors256)
                throw new NotSupportedException("Only supported 4bpp and 8bpp images.");

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == oldIndex)
                    tiles[i] = (byte)newIndex;
                else if (tiles[i] == newIndex)
                    tiles[i] = (byte)oldIndex;
            }

            if (format == ColorFormat.colors16)
                tiles = Helper.BitsConverter.Bits4ToByte(tiles);
        }
        public static void Swap_Color(ref byte[] tiles, ref Color[] palette, int oldIndex, int newIndex, ColorFormat format)
        {
            if (format == ColorFormat.colors16) // Yeah, I should improve it
                tiles = Helper.BitsConverter.BytesToBit4(tiles);
            else if (format != ColorFormat.colors256)
                throw new NotSupportedException("Only supported 4bpp and 8bpp images.");

            Color old_color = palette[oldIndex];
            palette[oldIndex] = palette[newIndex];
            palette[newIndex] = old_color;

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == oldIndex)
                    tiles[i] = (byte)newIndex;
                else if (tiles[i] == newIndex)
                    tiles[i] = (byte)oldIndex;
            }

            if (format == ColorFormat.colors16)
                tiles = Helper.BitsConverter.Bits4ToByte(tiles);
        }
        public static void Replace_Color(ref byte[] tiles, int oldIndex, int newIndex)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == oldIndex)
                    tiles[i] = (byte)newIndex;
            }
        }
        public static void Swap_Palette(ref byte[] tiles, Color[] newp, Color[] oldp, ColorFormat format, decimal threshold = 0)
        {
            if (format == ColorFormat.colors16) // Yeah, I should improve it
                tiles = Helper.BitsConverter.BytesToBit4(tiles);
            else if (format != ColorFormat.colors256)
                throw new NotSupportedException("Only supported 4bpp and 8bpp images.");

            List<Color> notfound = new List<Color>();
            List<Color> newplist = new List<Color>(newp);

            for (int i = 0; i < tiles.Length; i++)
            {
                Color px = oldp[tiles[i]];
                int id = newplist.IndexOf(px);

                if (px == Color.Transparent && id == -1)
                    id = 0;

                if (id == -1)
                    id = FindNextColor(px, newp, threshold);

                if (id == -1)
                {
                    // If the color is not found, maybe is that the pixel own to another cell (overlapping cells).
                    // For this reason, there are two ways to do that:
                    // 1º Get the original hidden color from the original file                               <- In mind
                    // 2º Set this pixel as transparent to show the pixel from the other cell (tiles[i] = 0) <- Done!
                    // If there isn't overlapping cells, throw exception                                     <- In mind
                    notfound.Add(px);
                    id = 0;
                }

                tiles[i] = (byte)id;
            }

            //if (notfound.Count > 0)
            //    throw new NotSupportedException("Color not found in the original palette!");

            if (format == ColorFormat.colors16)
                tiles = Helper.BitsConverter.Bits4ToByte(tiles);
        }

        public static Size Get_Size(int fileSize, int bpp)
        {
            int width, height;
            int num_pix = fileSize * 8 / bpp;

            // If the image it's a square
            if (Math.Pow((int)(Math.Sqrt(num_pix)), 2) == num_pix)
                width = height = (int)Math.Sqrt(num_pix);
            else
            {
                width = (num_pix < 0x100 ? num_pix : 0x0100);
                height = num_pix / width;
            }

            if (height == 0)
                height = 1;
            if (width == 0)
                width = 1;

            return new Size(width, height);
        }

        public static uint Add_Image(ref byte[] data, byte[] newData, uint blockSize)
        {
            // Add the image to the end of the data
            // Return the offset where the data is added
            List<byte> result = new List<byte>();
            result.AddRange(data);

            while (result.Count % blockSize != 0)
                result.Add(0x00);

            uint offset = (uint)result.Count;

            result.AddRange(newData);
            while (result.Count % blockSize != 0)
                result.Add(0x00);

            data = result.ToArray();
            return offset;
        }

        public static uint Add_Image(ref byte[] data, byte[] newData, uint partOffset, uint partSize, uint blockSize, out uint addedLength)
        {
            // Add the image to the end of the partition data
            // Return the offset where the data has been inserted
            List<byte> result = new List<byte>(data);
            uint offset = partOffset + partSize;

            addedLength = (partSize % blockSize != 0) ? blockSize - partSize % blockSize : 0;
            if (offset == result.Count) result.AddRange(new byte[addedLength]);
            else result.InsertRange((int)offset, new byte[addedLength]);
            offset += addedLength;

            if (offset == result.Count) result.AddRange(newData);
            else result.InsertRange((int)offset, newData);
            addedLength += (uint)newData.Length;

            data = result.ToArray();
            return offset;
        }

        public static int FindNextColor(Color c, Color[] palette, decimal threshold = 0)
        {
            int id = -1;
            decimal minDistance = decimal.MaxValue;

            // Skip the first color since it used to be the transparent color and we
            // don't want that as the best match if possible.
            for (int i = 1; i < palette.Length; i++)
            {
                double x = palette[i].R - c.R;
                double y = palette[i].G - c.G;
                double z = palette[i].B - c.B;
                decimal distance = (decimal)Math.Sqrt(x * x + y * y + z * z);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    id = i;
                }
            }

            // If the distance it's bigger than wanted, remove the best match
            if (minDistance > threshold)
                id = -1;

            // If still it doesn't found the color try with the first one.
            if (id == -1)
            {
                double x = palette[0].R - c.R;
                double y = palette[0].G - c.G;
                double z = palette[0].B - c.B;
                decimal distance = (decimal)Math.Sqrt(x * x + y * y + z * z);

                if (distance <= threshold)
                    id = 0;
            }

            if (id == -1)
                Console.WriteLine("Color not found: {0} (distance: {1})", c, minDistance);

            return id;
        }

        public static void Indexed_Image(Bitmap img, ColorFormat cf, out byte[] tiles, out Color[] palette)
        {
            // It's a slow method but it should work always
            int width = img.Width;
            int height = img.Height;

            List<Color> coldif = new List<Color>();
            int[,] data = new int[width * height, 2];

            // Get the indexed data
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Color pix = img.GetPixel(w, h);
                    Color apix = Color.FromArgb(pix.R, pix.G, pix.B);   // Without alpha value

                    if (pix.A == 0)
                        apix = Color.Transparent;

                    // Add the color to the provisional palette
                    if (!coldif.Contains(apix))
                        coldif.Add(apix);

                    // Get the index and save the alpha value
                    data[w + h * width, 0] = coldif.IndexOf(apix);  // Index
                    data[w + h * width, 1] = pix.A;                 // Alpha value
                }
            }

            int max_colors = 0;     // Maximum colors per palette
            int bpc = 0;            // Bits per color
            switch (cf)
            {
                case ColorFormat.A3I5: max_colors = 32; bpc = 8; break;
                case ColorFormat.colors4: max_colors = 4; bpc = 2; break;
                case ColorFormat.colors16: max_colors = 16; bpc = 4; break;
                case ColorFormat.colors256: max_colors = 256; bpc = 8; break;
                case ColorFormat.texel4x4: throw new NotSupportedException("Texel 4x4 not supported yet.");
                case ColorFormat.A5I3: max_colors = 8; bpc = 8; break;
                case ColorFormat.direct: max_colors = 0; bpc = 16; break;
                case ColorFormat.colors2: max_colors = 2; bpc = 1; break;
                case ColorFormat.A4I4: max_colors = 16; bpc = 8; break;
            }

            // Not dithering method for now, I hope you input a image with less than the maximum colors
            if (coldif.Count > max_colors && cf != ColorFormat.direct)
                throw new NotSupportedException("The image has more colors than permitted.\n" +
                     (coldif.Count + 1).ToString() + " unique colors!");

            // Finally get the set the tile array with the correct format
            tiles = new byte[width * height * bpc / 8];
            for (int i = 0, j = 0; i < tiles.Length; )
            {
                switch (cf)
                {
                    case ColorFormat.colors2:
                    case ColorFormat.colors4:
                    case ColorFormat.colors16:
                    case ColorFormat.colors256:
                        for (int b = 0; b < 8; b += bpc)
                            if (j < data.Length)
                                tiles[i] |= (byte)(data[j++, 0] << b);

                        i++;
                        break;

                    case ColorFormat.A3I5:
                        byte alpha1 = (byte)(data[j, 1] * 8 / 256);
                        byte va1 = (byte)data[j++, 0];
                        va1 |= (byte)(alpha1 << 5);
                        tiles[i++] = va1;
                        break;
                    case ColorFormat.A4I4:
                        byte alpha3 = (byte)(data[j, 1] * 16 / 256);
                        byte va3 = (byte)data[j++, 0];
                        va3 |= (byte)(alpha3 << 4);
                        tiles[i++] = va3;
                        break;
                    case ColorFormat.A5I3:
                        byte alpha2 = (byte)(data[j, 1] * 32 / 256);
                        byte va2 = (byte)data[j++, 0];
                        va2 |= (byte)(alpha2 << 3);
                        tiles[i++] = va2;
                        break;

                    case ColorFormat.direct:
                        byte[] v = ColorToBGRA555(Color.FromArgb(data[j, 1], coldif[data[j++, 0]]));
                        tiles[i++] = v[0];
                        tiles[i++] = v[1];
                        break;

                    case ColorFormat.texel4x4:
                        // Not supported
                        break;
                }
            }

            palette = coldif.ToArray();
        }
        #endregion

        #region Map
        public static Byte[] Apply_Map(NTFS[] map, Byte[] tiles, out Byte[] tile_pal, int bpp, int tile_size, int startInfo = 0)
        {
            int tile_length = tile_size * tile_size * bpp / 8;
            int num_tiles = tiles.Length / tile_length;

            List<Byte> bytes = new List<byte>();
            tile_pal = new byte[(map.Length - startInfo) * tile_size * tile_size];

            for (int i = startInfo; i < map.Length; i++)
            {
                if (map[i].nTile >= num_tiles)
                    map[i].nTile = 0;

                Byte[] currTile = new byte[tile_length];
                if (map[i].nTile * tile_length + tile_length > tiles.Length)
                    map[i].nTile = 0;

                if (tile_length < tiles.Length)
                    Array.Copy(tiles, map[i].nTile * tile_length, currTile, 0, tile_length);

                if (map[i].xFlip == 1)
                    currTile = XFlip(currTile, tile_size, bpp);
                if (map[i].yFlip == 1)
                    currTile = YFlip(currTile, tile_size, bpp);

                bytes.AddRange(currTile);

                for (int t = 0; t < tile_size * tile_size; t++)
                    tile_pal[i * tile_size * tile_size + t] = map[i].nPalette;
            }

            return bytes.ToArray();
        }
        public static Byte[] XFlip(Byte[] tile, int tile_size, int bpp)
        {
            byte[] newTile = new byte[tile.Length];
            int tile_width = tile_size * bpp / 8;

            for (int h = 0; h < tile_size; h++)
            {
                for (int w = 0; w < tile_width / 2; w++)
                {
                    byte b = tile[((tile_width - 1) - w) + h * tile_width];
                    newTile[w + h * tile_width] = Reverse_Bits(b, bpp);

                    b = tile[w + h * tile_width];
                    newTile[((tile_width - 1) - w) + h * tile_width] = Reverse_Bits(b, bpp);
                }
            }
            return newTile;
        }
        public static Byte Reverse_Bits(byte b, int length)
        {
            byte rb = 0;

            if (length == 4)
                rb = (byte)((b << 4) + (b >> 4));
            else if (length == 8)
                return b;

            return rb;
        }
        public static Byte[] YFlip(Byte[] tile, int tile_size, int bpp)
        {
            byte[] newTile = new byte[tile.Length];
            int tile_width = tile_size * bpp / 8;

            for (int h = 0; h < tile_size / 2; h++)
            {
                for (int w = 0; w < tile_width; w++)
                {
                    newTile[w + h * tile_width] = tile[w + (tile_size - 1 - h) * tile_width];
                    newTile[w + (tile_size - 1 - h) * tile_width] = tile[w + h * tile_width];
                }
            }
            return newTile;
        }

        public static NTFS[] Create_BasicMap(int num_tiles, int startTile = 0, byte palette = 0)
        {
            NTFS[] map = new NTFS[num_tiles];

            for (int i = startTile; i < num_tiles; i++)
            {
                map[i] = new NTFS();
                map[i].nPalette = palette;
                map[i].yFlip = 0;
                map[i].xFlip = 0;
                //if (i >= startFillTile)
                //    map[i].nTile = (ushort)fillTile;
                //else
                map[i].nTile = (ushort)(i + startTile);
            }

            return map;
        }
        public static NTFS[] Create_Map(ref byte[] data, int bpp, int tile_size, byte palette = 0)
        {
            int ppt = tile_size * tile_size; // pixels per tile
            int tile_length = ppt * bpp / 8;

            // Divide the data in tiles
            byte[][] tiles = new byte[data.Length / tile_length][];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new byte[tile_length];
                Array.Copy(data, i * tiles[i].Length, tiles[i], 0, tiles[i].Length);
            }

            NTFS[] map = new NTFS[tiles.Length];
            List<byte[]> newtiles = new List<byte[]>();
            for (int i = 0; i < map.Length; i++)
            {
                map[i].nPalette = palette;
                map[i].xFlip = 0;
                map[i].yFlip = 0;

                int index = -1;
                byte flipX = 0;
                byte flipY = 0;

                for (ushort t = 0; t < newtiles.Count; t++)
                {
                    if (Compare_Array(newtiles[t], tiles[i]))
                    {
                        index = t;
                        break;
                    }
                    if (Compare_Array(newtiles[t], XFlip(tiles[i], tile_size, bpp)))
                    {
                        index = t;
                        flipX = 1;
                        break;
                    }
                    if (Compare_Array(newtiles[t], YFlip(tiles[i], tile_size, bpp)))
                    {
                        index = t;
                        flipY = 1;
                        break;
                    }
                    if (Compare_Array(newtiles[t], YFlip(XFlip(tiles[i], tile_size, bpp), tile_size, bpp)))
                    {
                        index = t;
                        flipX = 1;
                        flipY = 1;
                        break;
                    }
                }

                if (index > -1)
                    map[i].nTile = (ushort)index;
                else
                {
                    map[i].nTile = (ushort)newtiles.Count;
                    newtiles.Add(tiles[i]);
                }
                map[i].xFlip = flipX;
                map[i].yFlip = flipY;
            }

            // Save the new tiles
            data = new byte[newtiles.Count * tile_length];
            for (int i = 0; i < newtiles.Count; i++)
                for (int j = 0; j < newtiles[i].Length; j++)
                    data[j + i * tile_length] = newtiles[i][j];
            return map;
        }
        public static bool Compare_Array(byte[] d1, byte[] d2)
        {
            if (d1.Length != d2.Length)
                return false;

            for (int i = 0; i < d1.Length; i++)
                if (d1[i] != d2[i])
                    return false;

            return true;
        }

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

        #region OAM
        public static Size Get_OAMSize(byte shape, byte size)
        {
            Size imageSize = new Size();

            switch (shape)
            {
                case 0x00:  // Square
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(8, 8);
                            break;
                        case 0x01:
                            imageSize = new Size(16, 16);
                            break;
                        case 0x02:
                            imageSize = new Size(32, 32);
                            break;
                        case 0x03:
                            imageSize = new Size(64, 64);
                            break;
                    }
                    break;
                case 0x01:  // Horizontal
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(16, 8);
                            break;
                        case 0x01:
                            imageSize = new Size(32, 8);
                            break;
                        case 0x02:
                            imageSize = new Size(32, 16);
                            break;
                        case 0x03:
                            imageSize = new Size(64, 32);
                            break;
                    }
                    break;
                case 0x02:  // Vertical
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(8, 16);
                            break;
                        case 0x01:
                            imageSize = new Size(8, 32);
                            break;
                        case 0x02:
                            imageSize = new Size(16, 32);
                            break;
                        case 0x03:
                            imageSize = new Size(32, 64);
                            break;
                    }
                    break;
            }

            return imageSize;
        }

        public static Bitmap Get_Image(Bank bank, uint blockSize, ImageBase img, PaletteBase pal, int max_width, int max_height,
                                       bool draw_grid, bool draw_cells, bool draw_numbers, bool trans, bool image, int currOAM = -1,
                                       int zoom = 1, int[] index = null)
        {
            Size size = new Size(max_width * zoom, max_height * zoom);
            Bitmap bank_img = new Bitmap(size.Width, size.Height);
            Graphics graphic = Graphics.FromImage(bank_img);

            if (bank.oams.Length == 0)
            {
                graphic.DrawString("No OAM", SystemFonts.CaptionFont, Brushes.Black, new PointF(max_width / 2, max_height / 2));
                return bank_img;
            }

            if (draw_grid)
            {
                for (int i = (0 - size.Width); i < size.Width; i += 8)
                {
                    graphic.DrawLine(Pens.LightBlue, (i + size.Width / 2) * zoom, 0, (i + size.Width / 2) * zoom, size.Height * zoom);
                    graphic.DrawLine(Pens.LightBlue, 0, (i + size.Height / 2) * zoom, size.Width * zoom, (i + size.Height / 2) * zoom);
                }
                graphic.DrawLine(Pens.Blue, (max_width / 2) * zoom, 0, (max_width / 2) * zoom, max_height * zoom);
                graphic.DrawLine(Pens.Blue, 0, (max_height / 2) * zoom, max_width * zoom, (max_height / 2) * zoom);
            }


            Image cell;
            for (int i = 0; i < bank.oams.Length; i++)
            {
                bool draw = false;
                if (index == null)
                    draw = true;
                else
                    for (int k = 0; k < index.Length; k++)
                        if (index[k] == i)
                            draw = true;
                if (!draw)
                    continue;
    
                if (bank.oams[i].width == 0x00 || bank.oams[i].height == 0x00)
                    continue;

                uint tileOffset = bank.oams[i].obj2.tileOffset;
                tileOffset = (uint)(tileOffset << (byte)blockSize);

                if (image)
                {
                    ImageBase cell_img = new TestImage();
                    cell_img.Set_Tiles((byte[])img.Tiles.Clone(), bank.oams[i].width, bank.oams[i].height, img.FormatColor,
                                       img.FormTile, false);
                    cell_img.StartByte = (int)(tileOffset * 0x20 + bank.data_offset);

                    byte num_pal = bank.oams[i].obj2.index_palette;
                    if (num_pal >= pal.NumberOfPalettes)
                        num_pal = 0;
                    for (int j = 0; j < cell_img.TilesPalette.Length; j++)
                        cell_img.TilesPalette[j] = num_pal;

                    cell = cell_img.Get_Image(pal);
                    //else
                    //{
                    //    tileOffset /= (blockSize / 2);
                    //    int imageWidth = img.Width;
                    //    int imageHeight = img.Height;

                    //    int posX = (int)(tileOffset % imageWidth);
                    //    int posY = (int)(tileOffset / imageWidth);

                    //    if (img.ColorFormat == ColorFormat.colors16)
                    //        posY *= (int)blockSize * 2;
                    //    else
                    //        posY *= (int)blockSize;
                    //    if (posY >= imageHeight)
                    //        posY = posY % imageHeight;

                    //    cells[i] = ((Bitmap)img.Get_Image(pal)).Clone(new Rectangle(posX * zoom, posY * zoom, bank.oams[i].width * zoom, bank.oams[i].height * zoom),
                    //                                                System.Drawing.Imaging.PixelFormat.DontCare);
                    //}

                    #region Flip
                    if (bank.oams[i].obj1.flipX == 1 && bank.oams[i].obj1.flipY == 1)
                        cell.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    else if (bank.oams[i].obj1.flipX == 1)
                        cell.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (bank.oams[i].obj1.flipY == 1)
                        cell.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    #endregion

                    if (trans)
                        ((Bitmap)cell).MakeTransparent(pal.Palette[num_pal][0]);

                    graphic.DrawImageUnscaled(cell, size.Width / 2 + bank.oams[i].obj1.xOffset * zoom, size.Height / 2 + bank.oams[i].obj0.yOffset * zoom);
                }

                if (draw_cells)
                    graphic.DrawRectangle(Pens.Black, size.Width / 2 + bank.oams[i].obj1.xOffset * zoom, size.Height / 2 + bank.oams[i].obj0.yOffset * zoom,
                        bank.oams[i].width * zoom, bank.oams[i].height * zoom);
                if (i == currOAM)
                    graphic.DrawRectangle(new Pen(Color.Red, 3), size.Width / 2 + bank.oams[i].obj1.xOffset * zoom, size.Height / 2 + bank.oams[i].obj0.yOffset * zoom,
                        bank.oams[i].width * zoom, bank.oams[i].height * zoom);
                if (draw_numbers)
                    graphic.DrawString(bank.oams[i].num_cell.ToString(), SystemFonts.CaptionFont, Brushes.Black, size.Width / 2 + bank.oams[i].obj1.xOffset * zoom,
                        size.Height / 2 + bank.oams[i].obj0.yOffset * zoom);
            }

            return bank_img;
        }

        public static Byte[] Get_OAMdata(OAM oam, byte[] image, ColorFormat format)
        {
            if (format == ColorFormat.colors16)
                image = Helper.BitsConverter.BytesToBit4(image);

            List<byte> data = new List<byte>();
            int y1 = 128 + oam.obj0.yOffset;
            int y2 = y1 + oam.height;
            int x1 = 256 + oam.obj1.xOffset;
            int x2 = x1 + oam.width;

            for (int ht = 0; ht < 256; ht++)
                for (int wt = 0; wt < 512; wt++)
                    if (ht >= y1 && ht < y2)
                        if (wt >= x1 && wt < x2)
                            data.Add(image[wt + ht * 512]);

            if (format == ColorFormat.colors16)
                return Helper.BitsConverter.Bits4ToByte(data.ToArray());
            else
                return data.ToArray();
        }
        public static int Comparision_OAM(OAM c1, OAM c2)
        {
            if (c1.obj2.priority < c2.obj2.priority)
                return 1;
            else if (c1.obj2.priority > c2.obj2.priority)
                return -1;
            else   // Same priority
            {
                if (c1.num_cell < c2.num_cell)
                    return 1;
                else if (c1.num_cell > c2.num_cell)
                    return -1;
                else // Same cell
                    return 0;
            }
        }

        public static ushort[] OAMInfo(OAM oam)
        {
            ushort[] obj = new ushort[3];

            // OBJ0
            obj[0] = 0;
            obj[0] += (ushort)((sbyte)(oam.obj0.yOffset) & 0xFF);
            obj[0] += (ushort)((oam.obj0.rs_flag & 1) << 8);
            if (oam.obj0.rs_flag == 0x00)
                obj[0] += (ushort)((oam.obj0.objDisable & 1) << 9);
            else
                obj[0] += (ushort)((oam.obj0.doubleSize & 1) << 9);
            obj[0] += (ushort)((oam.obj0.objMode & 3) << 10);
            obj[0] += (ushort)((oam.obj0.mosaic_flag & 1) << 12);
            obj[0] += (ushort)((oam.obj0.depth & 1) << 13);
            obj[0] += (ushort)((oam.obj0.shape & 3) << 14);

            // OBJ1
            obj[1] = 0;
            if (oam.obj1.xOffset < 0)
                oam.obj1.xOffset += 0x200;
            obj[1] += (ushort)(oam.obj1.xOffset & 0x1FF);
            if (oam.obj0.rs_flag == 0)
            {
                obj[1] += (ushort)((oam.obj1.unused & 0x7) << 9);
                obj[1] += (ushort)((oam.obj1.flipX & 1) << 12);
                obj[1] += (ushort)((oam.obj1.flipY & 1) << 13);
            }
            else
                obj[1] += (ushort)((oam.obj1.select_param & 0x1F) << 9);
            obj[1] += (ushort)((oam.obj1.size & 3) << 14);

            // OBJ2
            obj[2] = 0;
            obj[2] += (ushort)(oam.obj2.tileOffset & 0x3FF);
            obj[2] += (ushort)((oam.obj2.priority & 3) << 10);
            obj[2] += (ushort)((oam.obj2.index_palette & 0xF) << 12);

            return obj;
        }
        public static OAM OAMInfo(ushort[] obj)
        {
            OAM oam = new OAM();

            // Obj 0
            oam.obj0.yOffset = (sbyte)(obj[0] & 0xFF);
            oam.obj0.rs_flag = (byte)((obj[0] >> 8) & 1);
            if (oam.obj0.rs_flag == 0)
                oam.obj0.objDisable = (byte)((obj[0] >> 9) & 1);
            else
                oam.obj0.doubleSize = (byte)((obj[0] >> 9) & 1);
            oam.obj0.objMode = (byte)((obj[0] >> 10) & 3);
            oam.obj0.mosaic_flag = (byte)((obj[0] >> 12) & 1);
            oam.obj0.depth = (byte)((obj[0] >> 13) & 1);
            oam.obj0.shape = (byte)((obj[0] >> 14) & 3);

            // Obj 1
            oam.obj1.xOffset = obj[1] & 0x01FF;
            if (oam.obj1.xOffset >= 0x100)
                oam.obj1.xOffset -= 0x200;
            if (oam.obj0.rs_flag == 0)
            {
                oam.obj1.unused = (byte)((obj[1] >> 9) & 7);
                oam.obj1.flipX = (byte)((obj[1] >> 12) & 1);
                oam.obj1.flipY = (byte)((obj[1] >> 13) & 1);
            }
            else
                oam.obj1.select_param = (byte)((obj[1] >> 9) & 0x1F);
            oam.obj1.size = (byte)((obj[1] >> 14) & 3);

            // Obj 2
            oam.obj2.tileOffset = (uint)(obj[2] & 0x03FF);
            oam.obj2.priority = (byte)((obj[2] >> 10) & 3);
            oam.obj2.index_palette = (byte)((obj[2] >> 12) & 0xF);

            Size size = Get_OAMSize(oam.obj0.shape, oam.obj1.size);
            oam.width = (ushort)size.Width;
            oam.height = (ushort)size.Height;

            return oam;
        }
        public static OAM OAMInfo(ushort v1, ushort v2, ushort v3)
        {
            return OAMInfo(new ushort[] { v1, v2, v3 });
        }
        #endregion
    }
}
