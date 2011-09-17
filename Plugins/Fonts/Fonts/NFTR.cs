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
            ushort value = br.ReadUInt16();
            font.plgc.depth = (ushort)(value & 0xFF);
            font.plgc.rotateMode = (byte)((value >> 8) & 0x3);
            font.plgc.unknown2 = (byte)(value >> 10);

            font.plgc.tiles = new Byte[(font.plgc.block_size - 0x10) / font.plgc.tile_length][];
            for (int i = 0; i < font.plgc.tiles.Length; i++)
                font.plgc.tiles[i] = BytesToBits(br.ReadBytes(font.plgc.tile_length));

            Console.WriteLine("<b>Section: {0}</b>", new String(font.plgc.type));
            Console.WriteLine("Block size: 0x{0}", font.plgc.block_size.ToString("x"));
            Console.WriteLine("Tile width: {0}", font.plgc.tile_width.ToString());
            Console.WriteLine("Tile height: {0}", font.plgc.tile_height.ToString());
            Console.WriteLine("Tile length: {0}", font.plgc.tile_length.ToString());
            Console.WriteLine("Unknown: 0x{0}", font.plgc.unknown.ToString("x"));
            Console.WriteLine("Depth: {0}", font.plgc.depth.ToString());
            Console.WriteLine("Rotate mode: {0}", font.plgc.rotateMode.ToString());
            Console.WriteLine("Unknown 2: {0}", font.plgc.unknown2.ToString());

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
        public static void Write(sNFTR font, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Write the generic header
            bw.Write(font.header.type);
            bw.Write(font.header.endianess);
            bw.Write(font.header.unknown);
            bw.Write(font.header.file_size);
            bw.Write(font.header.block_size);
            bw.Write(font.header.num_blocks);

            // Write the FINF section
            bw.Write(font.fnif.type);
            bw.Write(font.fnif.block_size);
            bw.Write(font.fnif.unknown1);
            bw.Write(font.fnif.unknown2);
            bw.Write(font.fnif.offset_plgc);
            bw.Write(font.fnif.offset_hdwc);
            bw.Write(font.fnif.offset_pamc);
            if (font.fnif.block_size == 0x20)
                bw.Write(font.fnif.unknown3);

            // Write the PLGC section
            bw.Write(font.plgc.type);
            bw.Write(font.plgc.block_size);
            bw.Write(font.plgc.tile_width);
            bw.Write(font.plgc.tile_height);
            bw.Write(font.plgc.tile_length);
            bw.Write(font.plgc.unknown);
            bw.Write(font.plgc.depth);
            for (int i = 0; i < font.plgc.tiles.Length; i++)
                bw.Write(BitsToBytes(font.plgc.tiles[i]));

            // Write HDWC section
            bw.Write(font.hdwc.type);
            bw.Write(font.hdwc.block_size);
            bw.Write(font.hdwc.fist_code);
            bw.Write(font.hdwc.last_code);
            bw.Write(font.hdwc.unknown1);
            for (int i = 0; i < font.hdwc.info.Count; i++)
            {
                bw.Write(font.hdwc.info[i].pixel_start);
                bw.Write(font.hdwc.info[i].pixel_width);
                bw.Write(font.hdwc.info[i].pixel_length);
            }

            // Write the PAMC section
            for (int i = 0; i < font.pamc.Count; i++)
            {
                bw.Write(font.pamc[i].type);
                bw.Write(font.pamc[i].block_size);
                bw.Write(font.pamc[i].first_char);
                bw.Write(font.pamc[i].last_char);
                bw.Write(font.pamc[i].type_section);
                uint next_section = (uint)(bw.BaseStream.Position - 0x10 + font.pamc[i].block_size) + 0x08;
                bw.Write(font.pamc[i].next_section);

                switch (font.pamc[i].type_section)
                {
                    case 0:
                        sNFTR.PAMC.Type0 type0 = (sNFTR.PAMC.Type0)font.pamc[i].info;
                        bw.Write(type0.fist_char_code);
                        break;
                    case 1:
                        sNFTR.PAMC.Type1 type1 = (sNFTR.PAMC.Type1)font.pamc[i].info;
                        for (int j = 0; j < type1.char_code.Length; j++)
                            bw.Write(type1.char_code[j]);
                        break;
                    case 2:
                        sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)font.pamc[i].info;
                        bw.Write(type2.num_chars);
                        for (int j = 0; j < type2.num_chars; j++)
                        {
                            bw.Write(type2.chars_code[j]);
                            bw.Write(type2.chars[j]);
                        }
                        break;
                }

                int relleno = (int)(next_section - 0x08 - bw.BaseStream.Position);
                for (int r = 0; r < relleno; r++)
                    bw.Write((byte)0x00);
            }

            bw.Flush();
            bw.Close();
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
                //palette[i] = Color.FromArgb(255, colorIndex, colorIndex, colorIndex);
            }
            palette = palette.Reverse().ToArray();

            int bits = Convert.ToByte(new String('1', font.plgc.depth), 2);
            for (int i = 0; i < font.plgc.tiles[id].Length; i += font.plgc.depth)
            {
                Byte byteFromBits = 0;
                for (int b = 0; b < font.plgc.depth; b++)
                {
                    byteFromBits += (byte)(font.plgc.tiles[id][i + b] << b);
                }
                tileData.Add(byteFromBits);
            }

            for (int h = 0; h < image.Height / zoom; h++)
            {
                for (int w = 0; w < image.Width / zoom; w++)
                {
                    for (int hzoom = 0; hzoom < zoom; hzoom++)
                        for (int wzoom = 0; wzoom < zoom; wzoom++)
                            try
                            {
                                image.SetPixel(
                                    w * zoom + wzoom,
                                    h * zoom + hzoom,
                                    (palette[tileData[w + h * image.Width / zoom]]));
                            }
                            catch { break; }
                }
            }

            switch (font.plgc.rotateMode)
            {
                case 1: image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                case 2: image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                case 3: image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
            }

            image.MakeTransparent(Color.FromArgb(255, 255, 255));
            return image;
        }
        public static Bitmap Get_Char(byte[] tiles, int depth, int width, int height, int rotateMode, int zoom = 1)
        {
            Bitmap image = new Bitmap(width * zoom + 1, height * zoom + 1);
            List<Byte> tileData = new List<byte>();

            Color[] palette = new Color[(int)Math.Pow(2, depth)];
            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = 255 - (i * (255 / (palette.Length - 1)));
                palette[i] = Color.FromArgb(colorIndex, 0, 0, 0);
            }
            palette = palette.Reverse().ToArray();

            int bits = Convert.ToByte(new String('1', depth), 2);
            for (int i = 0; i < tiles.Length; i += depth)
            {
                Byte byteFromBits = 0;
                for (int b = 0; b < depth; b++)
                {
                    byteFromBits += (byte)(tiles[i + b] << b);
                }
                tileData.Add(byteFromBits);
            }


            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    for (int hzoom = 0; hzoom < zoom; hzoom++)
                        for (int wzoom = 0; wzoom < zoom; wzoom++)
                            image.SetPixel(
                                w * zoom + wzoom,
                                h * zoom + hzoom,
                                (palette[tileData[w + h * width]]));
                }
            }

            switch (rotateMode)
            {
                case 1: image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                case 2: image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                case 3: image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
            }

            return image;
        }
        public static Bitmap Get_Chars(sNFTR font, int maxWidth)
        {
            int char_x = maxWidth / font.plgc.tile_width;
            int char_y = (font.plgc.tiles.Length / char_x) + 1;
            Bitmap image = new Bitmap(char_x * font.plgc.tile_width + 1, char_y * font.plgc.tile_height + 1);
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

        public static Byte[] BytesToBits(Byte[] bytes)
        {
            List<Byte> bits = new List<byte>();

            for (int i = 0; i < bytes.Length; i++)
                for (int j = 7; j >= 0; j--)
                    bits.Add((byte)((bytes[i] >> j) & 1));

            return bits.ToArray();
        }
        public static Byte[] BitsToBytes(Byte[] bits)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes.Add(newByte);
            }

            return bytes.ToArray();
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
            public byte rotateMode;
            public byte unknown2;
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
