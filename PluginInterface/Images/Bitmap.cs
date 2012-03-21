using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace PluginInterface.Images
{
    public class BMP : ImageBase
    {
        PaletteBase palette;

        public BMP(IPluginHost pluginHost, string file) : base(pluginHost, file, -1) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            if (new String(br.ReadChars(2)) != "BM")
                throw new NotSupportedException();

            br.BaseStream.Position = 0x0A;
            uint offsetImagen = br.ReadUInt32();

            br.BaseStream.Position += 0x04;
            uint width = br.ReadUInt32();
            uint height = br.ReadUInt32();

            br.BaseStream.Position += 0x02;
            uint bpp = br.ReadUInt16();
            ColorFormat format;
            if (bpp == 0x04)
                format = Images.ColorFormat.colors16;
            else if (bpp == 0x08)
                format = Images.ColorFormat.colors256;
            else
                throw new NotSupportedException();

            uint compression = br.ReadUInt32();
            uint data_size = br.ReadUInt32();

            br.BaseStream.Position += 0x8;
            uint num_colors = br.ReadUInt32();

            if (num_colors == 0x00)
                num_colors = (uint)(bpp == 0x04 ? 0x10 : 0x0100);

            br.BaseStream.Position += 0x04;
            Color[][] colors = new Color[1][];
            colors[0] = new Color[num_colors];
            for (int i = 0; i < num_colors; i++)
            {
                Byte[] color = br.ReadBytes(4);
                colors[0][i] = Color.FromArgb(color[2], color[1], color[0]);
            }
            // Get the colors with BGR555 encoding (not all colours from bitmap are allowed)
            byte[] temp = Actions.ColorToBGR555(colors[0]);
            colors[0] = Actions.BGR555ToColor(temp);
            palette = new RawPalette(pluginHost, colors, false, format);

            byte[] tiles = new byte[width * height];
            br.BaseStream.Position = offsetImagen;

            switch (bpp)
            {
                case 4:
                    int divisor = (int)width / 2;
                    if (width % 4 != 0)
                    {
                        int res;
                        Math.DivRem((int)width / 2, 4, out res);
                        divisor = (int)width / 2 + (4 - res);
                    }

                    tiles = new byte[tiles.Length * 2];
                    for (int h = (int)height - 1; h >= 0; h--)
                    {
                        for (int w = 0; w < width; w += 2)
                        {
                            byte b = br.ReadByte();
                            tiles[w + h * width] = (byte)(b >> 4);

                            if (w + 1 != width)
                                tiles[w + 1 + h * width] = (byte)(b & 0xF);
                        }
                        br.ReadBytes((int)(divisor - ((float)width / 2)));
                    }
                    tiles = pluginHost.Bit4ToBit8(tiles);
                    break;
                case 8:
                    divisor = (int)width;
                    if (width % 4 != 0)
                    {
                        int res;
                        Math.DivRem((int)width, 4, out res);
                        divisor = (int)width + (4 - res);
                    }

                    for (int h = (int)height - 1; h >= 0; h--)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            tiles[w + h * width] = br.ReadByte();
                        }
                        br.ReadBytes(divisor - (int)width);
                    }
                    break;
            }

            br.Close();
            Set_Tiles(tiles, (int)width, (int)height, format, Images.TileForm.Lineal, false);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public PaletteBase Palette
        {
            get { return palette; }
        }
    }
}
