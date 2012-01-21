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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace SF_FEATHER
{
    public static class CGx
    {
        const ushort WIDTH = 32;

        public static void CG4_Read(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);  // CG4 
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();    // Usually 0
            uint unknown4 = br.ReadUInt32();    // Usually 0

            uint size_tiles = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();
            uint num_tiles = br.ReadUInt32();

            uint palColors = br.ReadUInt32();
            uint tileOffset = br.ReadUInt32();
            uint palOffset = br.ReadUInt32();
            uint mapOffset = br.ReadUInt32();    // If 0, it doesn't exist

            // Create the structure of a nitro tile file
            NCGR tile = new NCGR();
            tile.header.file_size = size_tiles;
            tile.order = TileOrder.Horizontal;
            tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            tile.rahc.size_tiledata = size_tiles;
            tile.rahc.nTilesX = WIDTH;
            tile.rahc.nTilesY = (ushort)(num_tiles / WIDTH);
            if (tile.rahc.nTilesY == 0)
                tile.rahc.nTilesY = 1;

            tile.rahc.nTiles = num_tiles;
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPalette = new byte[tile.rahc.nTiles];

            // Read tiles
            br.BaseStream.Position = tileOffset;
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
            {
                tile.rahc.tileData.tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(32));
            }

            // Create the structure of a nitro palette file
            br.BaseStream.Position = palOffset;
            NCLR palette = new NCLR();
            palette.header.file_size = palColors * 2;
            palette.pltt.nColors = palColors;
            palette.pltt.palettes = new NTFP[palColors / 0x10];
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                palette.pltt.palettes[i].colors = pluginHost.BGR555ToColor(br.ReadBytes(32));

            br.Close();

            pluginHost.Set_NCGR(tile);
            pluginHost.Set_NCLR(palette);
        }
        public static void CG8_Read(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] type = br.ReadChars(4);  // CG8 
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();    // Usually 0
            uint unknown4 = br.ReadUInt32();    // Usually 0

            uint size_tiles = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();
            uint num_tiles = br.ReadUInt32();

            uint palColors = br.ReadUInt32();
            uint tileOffset = br.ReadUInt32();
            uint palOffset = br.ReadUInt32();
            uint mapOffset = br.ReadUInt32();    // If 0, it doesn't exist

            // Create the structure of a nitro tile file
            NCGR tile = new NCGR();
            tile.header.file_size = size_tiles;
            tile.order = TileOrder.Horizontal;
            tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            tile.rahc.size_tiledata = size_tiles;
            tile.rahc.nTilesX = WIDTH;
            tile.rahc.nTilesY = (ushort)(num_tiles / WIDTH);
            if (tile.rahc.nTilesY == 0)
                tile.rahc.nTilesY = 1;

            tile.rahc.nTiles = num_tiles;
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPalette = new byte[tile.rahc.nTiles];

            // Read tiles
            br.BaseStream.Position = tileOffset;
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
            {
                tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
            }

            // Create the structure of a nitro palette file
            br.BaseStream.Position = palOffset;
            NCLR palette = new NCLR();
            palette.header.file_size = palColors * 2;
            palette.pltt.nColors = palColors;
            palette.pltt.palettes = new NTFP[1];
            palette.pltt.palettes[0].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)palColors * 2));

            br.Close();

            pluginHost.Set_NCGR(tile);
            pluginHost.Set_NCLR(palette);
        }

    }

    public class CGT
    {
        IPluginHost pluginHost;
        String file;
        int id;

        byte[] data;
        Color[] palette;
        int width;
        int height;
        uint format;
        bool transparency;

        public CGT(IPluginHost pluginHost, string file, int id)
        {
            this.pluginHost = pluginHost;
            this.file = file;
            this.id = id;

            Read();
        }

        public void Read()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            char[] id = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();    // Usually is 0x00
            uint unknown2 = br.ReadUInt32();    // Usually is 0x00
            uint unknown3 = br.ReadUInt32();    // Usually is 0x00

            format = br.ReadUInt32();
            uint tile_size = br.ReadUInt32();
            uint header_size = br.ReadUInt32();
            uint palette_size = br.ReadUInt32();

            uint palette_offset = br.ReadUInt32();
            uint unknown4 = br.ReadUInt32();
            uint unknown_offset = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();

            transparency = (br.ReadUInt32() == 0x00 ? false : true);
            transparency = false;

            width = (int)Math.Pow(2, (double)(br.ReadUInt16() + 3));
            height = (int)Math.Pow(2, (double)(br.ReadUInt16() + 3));
            uint unknown6 = br.ReadUInt32();

            data = br.ReadBytes((int)tile_size);

            br.BaseStream.Position = palette_offset;
            palette = pluginHost.BGR555ToColor(br.ReadBytes((int)palette_size));

            br.Close();
        }

        public Bitmap Draw_Texture()
        {
            if (format == 5)
                throw new NotSupportedException("Format 5 - Compressed texel");

            Bitmap imagen = new Bitmap(width, height);
            if (format == 3)                            // 16-color 4 bits
                data = pluginHost.Bit8ToBit4(data);
            else if (format == 2)                       // 4-color 2 bits
                data = Bit8ToBit2(data);

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    Color color = Color.Black;
                    try
                    {
                        if (format == 2 || format == 3 || format == 4)  // 2-4-8 bits per color
                            color = palette[data[w + h * width]];
                        else if (format == 1)                           // A3I5 8-bit
                        {
                            int colorIndex = data[w + h * width] & 0x1F;
                            int alpha = (data[w + h * width] >> 5);
                            alpha = ((alpha * 4) + (alpha / 2)) * 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (format == 6)           // A5I3 8-bit
                        {
                            int colorIndex = data[w + h * width] & 0x7;
                            int alpha = (data[w + h * width] >> 3);
                            alpha *= 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (format == 7)       // Direct texture 16-bit
                        {
                            ushort byteColor = BitConverter.ToUInt16(data, (w + h * width) * 2);
                            color = Color.FromArgb(
                                (byteColor >> 15) * 255,
                                (byteColor & 0x1F) * 8,
                                ((byteColor >> 5) & 0x1F) * 8,
                                ((byteColor >> 10) & 0x1F) * 8);
                        }
                    }
                    catch { }

                    imagen.SetPixel(w, h, color);
                }
            }

            if (transparency)
                imagen.MakeTransparent(palette[0]);

            return imagen;
        }

        private byte[] Bit8ToBit2(byte[] data)
        {
            List<Byte> bit2 = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                bit2.Add((byte)(data[i] & 0x3));
                bit2.Add((byte)((data[i] >> 2) & 0x3));
                bit2.Add((byte)((data[i] >> 4) & 0x3));
                bit2.Add((byte)((data[i] >> 6) & 0x3));
            }

            return bit2.ToArray();
        }


        public void Write_Tiles(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
