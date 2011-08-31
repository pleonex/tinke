using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Fonts
{
    public static class NFTR
    {
        public static sNFTR Read(string file, int id)
        {
            sNFTR font = new sNFTR();
            font.id = id;
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            Console.WriteLine("Reading NFTR font file...");
            Console.WriteLine("<pre>");
            // Read the standard header
            font.header.type = br.ReadChars(4);
            font.header.endianess = br.ReadUInt16();
            font.header.unknown = br.ReadUInt16();
            font.header.file_size = br.ReadUInt32();
            font.header.block_size = br.ReadUInt16();
            font.header.num_blocks = br.ReadUInt16();
            Console.WriteLine("# blocks: {0}", font.header.num_blocks.ToString());

            // Font INFo section
            font.fnif.type = br.ReadChars(4);
            font.fnif.block_size = br.ReadUInt32();
            font.fnif.unknown1 = br.ReadUInt32();
            font.fnif.unknown2 = br.ReadUInt32();
            font.fnif.offset_plgc = br.ReadUInt32();
            font.fnif.offset_hdwc = br.ReadUInt32();
            font.fnif.offset_pamc = br.ReadUInt32();
            if (font.fnif.block_size == 0x20)
                font.fnif.unknown3 = br.ReadUInt32();
            Console.WriteLine("<b>Sección: {0}</b>", new String(font.fnif.type));
            Console.WriteLine("Block size: 0x{0}", font.fnif.block_size.ToString("x"));
            Console.WriteLine("Unknown 1: 0x{0}", font.fnif.unknown1.ToString("x"));
            Console.WriteLine("Unknown 2: 0x{0}", font.fnif.unknown2.ToString("x"));
            Console.WriteLine("PLGC offset: 0x{0}", font.fnif.offset_plgc.ToString("x"));
            Console.WriteLine("HDWC offset: 0x{0}", font.fnif.offset_hdwc.ToString("x"));
            Console.WriteLine("PAMC offset: 0x{0}", font.fnif.offset_pamc.ToString("x"));
            if (font.fnif.block_size == 0x20)
                Console.WriteLine("Unknown 3: 0x{0}", font.fnif.unknown3.ToString("x"));

            // Character Graphics LP
            br.BaseStream.Position = font.fnif.offset_plgc - 0x08;
            font.plgc.type = br.ReadChars(4);
            font.plgc.block_size = br.ReadUInt32();
            font.plgc.tile_width = br.ReadByte();
            font.plgc.tile_height = br.ReadByte();
            font.plgc.tile_length = br.ReadUInt16();
            font.plgc.unknown = br.ReadUInt16();
            font.plgc.depth = br.ReadUInt16();

            font.plgc.tiles = new Byte[(font.plgc.block_size - 0x10) / font.plgc.tile_length][];
            for (int i = 0; i < font.plgc.tiles.Length; i++)
                font.plgc.tiles[i] = br.ReadBytes(font.plgc.tile_length);

            Console.WriteLine("<b>Section: {0}</b>", new String(font.plgc.type));
            Console.WriteLine("Block size: 0x{0}", font.plgc.block_size.ToString("x"));
            Console.WriteLine("Tile width: {0}", font.plgc.tile_width.ToString());
            Console.WriteLine("Tile height: {0}", font.plgc.tile_height.ToString());
            Console.WriteLine("Tile length: {0}", font.plgc.tile_length.ToString());
            Console.WriteLine("Unknown: 0x{0}", font.plgc.unknown.ToString("x"));
            Console.WriteLine("Depth: {0}", font.plgc.depth.ToString());

            // Character Width DH
            br.BaseStream.Position = font.fnif.offset_hdwc - 0x08;
            font.hdwc.type = br.ReadChars(4);
            font.hdwc.block_size = br.ReadUInt32();
            font.hdwc.fist_code = br.ReadUInt16();
            font.hdwc.last_code = br.ReadUInt16();
            font.hdwc.unknown1 = br.ReadUInt32();

            Console.WriteLine("<b>Section: {0}</b>", new String(font.hdwc.type));
            Console.WriteLine("Block size: 0x{0}", font.hdwc.block_size.ToString("x"));
            Console.WriteLine("Fist code: 0x{0}", font.hdwc.fist_code.ToString("x"));
            Console.WriteLine("Last code: 0x{0}", font.hdwc.last_code.ToString("x"));
            Console.WriteLine("Unknown 1: 0x{0}", font.hdwc.unknown1.ToString("x"));

            Console.WriteLine("Tile information");
            font.hdwc.info = new List<sNFTR.HDWC.Info>();
            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                sNFTR.HDWC.Info info = new sNFTR.HDWC.Info();
                info.pixel_start = br.ReadByte();
                info.pixel_width = br.ReadByte();
                info.pixel_length = br.ReadByte();
                font.hdwc.info.Add(info);
                Console.WriteLine("<u>Tile {0}</u>", i.ToString());
                Console.WriteLine("  |_Pixel start: {0}", info.pixel_start.ToString());
                Console.WriteLine("  |_Pixel width: {0}", info.pixel_width.ToString());
                Console.WriteLine("  |_Pixel length: {0}", info.pixel_length.ToString());
            }

            // Character MAP
            Console.WriteLine("<b>Sections: PAMC</b>");
            br.BaseStream.Position = font.fnif.offset_pamc - 0x08;
            font.pamc = new List<sNFTR.PAMC>();
            uint nextOffset = 0x00;
            do
            {
                sNFTR.PAMC pamc = new sNFTR.PAMC();
                pamc.type = br.ReadChars(4);
                pamc.block_size = br.ReadUInt32();
                pamc.first_char = br.ReadUInt16();
                pamc.last_char = br.ReadUInt16();
                pamc.type_section = br.ReadUInt32();
                nextOffset = br.ReadUInt32();
                pamc.next_section = nextOffset;

                Console.WriteLine("<u>Section {0}</u>", font.pamc.Count.ToString());
                Console.WriteLine("  |__Block size: 0x{0}", pamc.block_size.ToString("x"));
                Console.WriteLine("  |_First char: 0x{0}", pamc.first_char.ToString("x"));
                Console.WriteLine("  |_Last char: 0x{0}", pamc.last_char.ToString("x"));
                Console.WriteLine("  |_Type: {0}", pamc.type_section.ToString());

                switch (pamc.type_section)
                {
                    case 0:
                        sNFTR.PAMC.Type0 type0 = new sNFTR.PAMC.Type0();
                        type0.fist_char_code = br.ReadUInt16();
                        pamc.info = type0;
                        Console.WriteLine("    \\_Info type 0");
                        Console.WriteLine("    \\_First char code: 0x{0}", type0.fist_char_code.ToString("x"));
                        break;
                    case 1:
                        sNFTR.PAMC.Type1 type1 = new sNFTR.PAMC.Type1();
                        type1.char_code = new ushort[(pamc.block_size - 0x14) / 2];
                        Console.WriteLine("    \\_Info type 1");
                        Console.WriteLine("    \\_Number of elements: {0}", type1.char_code.Length.ToString());
                        for (int i = 0; i < type1.char_code.Length; i++)
                        {
                            type1.char_code[i] = br.ReadUInt16();
                            Console.WriteLine("    |_Char code {0}: 0x{1} ({2})", i.ToString(), type1.char_code[i].ToString("x"),
                                Encoding.GetEncoding("shift-jis").GetChars(
                                BitConverter.GetBytes(pamc.first_char + i).Reverse().ToArray()).Reverse().ToArray()[0]);
                        }
                        pamc.info = type1;
                        break;
                    case 2:
                        sNFTR.PAMC.Type2 type2 = new sNFTR.PAMC.Type2();
                        type2.num_chars = br.ReadUInt16();
                        type2.chars_code = new ushort[type2.num_chars];
                        type2.chars = new ushort[type2.num_chars];

                        Console.WriteLine("    \\_Info type 2");
                        Console.WriteLine("    \\_Number of chars: {0}", type2.num_chars.ToString());
                        for (int i = 0; i < type2.num_chars; i++)
                        {
                            type2.chars_code[i] = br.ReadUInt16();
                            type2.chars[i] = br.ReadUInt16();
                            Console.WriteLine("    |_Char code {0}: 0x{1}", i.ToString(), type2.chars_code[i].ToString("x"));
                            Console.WriteLine("    |_Char {0}: {1}", i.ToString(), type2.chars[i].ToString());
                        }
                        pamc.info = type2;
                        break;
                }

                font.pamc.Add(pamc);
                br.BaseStream.Position = nextOffset - 0x08;
            } while (nextOffset != 0x00);

            Console.WriteLine("</pre>");
            br.Close();
            return font;
        }

        public static Bitmap Get_Char(sNFTR font, int id, int zoom = 1)
        {
            Bitmap image = new Bitmap(font.plgc.tile_width * zoom, font.plgc.tile_height * zoom);
            List<Byte> tileData = new List<byte>();

            Color[] palette = new Color[(int)Math.Pow(2, font.plgc.depth)];
            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = 255 - (i * (255 / (palette.Length - 1)));
                palette[i] = Color.FromArgb(colorIndex, 0, 0, 0);
            }
            palette = palette.Reverse().ToArray();

            int bits = Convert.ToByte(new String('1', font.plgc.depth), 2);
            for (int i = 0; i < font.plgc.tiles[id].Length; i++)
            {
                for (int j = 8 - font.plgc.depth; j >= 0; j -= font.plgc.depth)
                {
                    tileData.Add((byte)((font.plgc.tiles[id][i] >> j) & bits));
                }
            }

            for (int h = 0; h < image.Height / zoom; h++)
            {
                for (int w = 0; w < image.Width / zoom; w++)
                {
                    for (int hzoom = 0; hzoom < zoom; hzoom++)
                        for (int wzoom = 0; wzoom < zoom; wzoom++)
                            image.SetPixel(
                                w * zoom + wzoom,
                                h * zoom + hzoom,
                                (palette[tileData[w + h * image.Width / zoom]]));
                }
            }

            return image;
        }
        public static Bitmap Get_Char(byte[] tiles, int depth, int width, int height, int zoom = 1)
        {
            Bitmap image = new Bitmap(width * zoom, height * zoom);
            List<Byte> tileData = new List<byte>();

            Color[] palette = new Color[(int)Math.Pow(2, depth)];
            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = 255 - (i * (255 / (palette.Length - 1)));
                palette[i] = Color.FromArgb(colorIndex, 0, 0, 0);
            }
            palette = palette.Reverse().ToArray();

            int bits = Convert.ToByte(new String('1', depth), 2);
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 8 - depth; j >= 0; j -= depth)
                {
                    tileData.Add((byte)((tiles[i] >> j) & bits));
                }
            }

            for (int h = 0; h < image.Height / zoom; h++)
            {
                for (int w = 0; w < image.Width / zoom; w++)
                {
                    for (int hzoom = 0; hzoom < zoom; hzoom++)
                        for (int wzoom = 0; wzoom < zoom; wzoom++)
                            image.SetPixel(
                                w * zoom + wzoom,
                                h * zoom + hzoom,
                                (palette[tileData[w + h * image.Width / zoom]]));
                }
            }

            return image;
        }
        public static Bitmap Get_Chars(sNFTR font, int maxWidth)
        {
            int char_x = maxWidth / font.plgc.tile_width;
            int char_y = (font.plgc.tiles.Length / char_x) + 1;
            Bitmap image = new Bitmap(char_x * font.plgc.tile_width, char_y * font.plgc.tile_height);
            Graphics graphic = Graphics.FromImage(image);

            int w, h;
            w = h = 0;
            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                graphic.DrawRectangle(Pens.Red, w, h, font.plgc.tile_width, font.plgc.tile_height);
                graphic.DrawImageUnscaled(Get_Char(font, i), w, h);
                w += font.plgc.tile_width;
                if (w + font.plgc.tile_width > maxWidth)
                {
                    w = 0;
                    h += font.plgc.tile_height;
                }
            }

            return image;
        }
    }

    public struct sNFTR // Nitro FonT Resource
    {
        public int id;
        public StandardHeader header;
        public FNIF fnif;
        public PLGC plgc;
        public HDWC hdwc;
        public List<PAMC> pamc;

        public struct StandardHeader
        {
            public char[] type;
            public ushort unknown;
            public ushort endianess;
            public uint file_size;
            public ushort block_size;
            public ushort num_blocks;
        }
        public struct FNIF // Font INFo
        {
            public char[] type;
            public uint block_size;
            public uint unknown1;
            public uint unknown2;
            public uint offset_plgc;
            public uint offset_hdwc;
            public uint offset_pamc;
            public uint unknown3;
        }
        public struct PLGC // Character Graphics LP
        {
            public char[] type;
            public uint block_size;
            public byte tile_width;
            public byte tile_height;
            public ushort tile_length;
            public ushort unknown;
            public ushort depth;
            public byte[][] tiles; // tiles of each char
        }
        public struct HDWC // Character Width DH
        {
            public char[] type;
            public uint block_size;
            public ushort fist_code;
            public ushort last_code;
            public uint unknown1; // always 0x00
            public List<Info> info;

            public struct Info
            {
                public byte pixel_start;
                public byte pixel_width;
                public byte pixel_length;
            }
        }
        public struct PAMC // Character MAP
        {
            public char[] type;
            public uint block_size;
            public ushort first_char;
            public ushort last_char;
            public uint type_section;
            public uint next_section;
            public Object info;

            public struct Type0
            {
                public ushort fist_char_code;
            }
            public struct Type1
            {
                public ushort[] char_code;
            }
            public struct Type2
            {
                public ushort num_chars;

                public ushort[] chars_code;
                public ushort[] chars;
            }
        }
    }
}
