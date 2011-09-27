using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Linq;
using PluginInterface;

namespace Fonts
{
    public static class NFTR
    {
        public static sNFTR Read(string file, int id, string lang)
        {
            sNFTR font = new sNFTR();
            font.id = id;
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            // Read the standard header
            font.header.type = br.ReadChars(4);
            font.header.endianess = br.ReadUInt16();
            font.header.unknown = br.ReadUInt16();
            font.header.file_size = br.ReadUInt32();
            font.header.block_size = br.ReadUInt16();
            font.header.num_blocks = br.ReadUInt16();

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
            {
                font.plgc.tiles[i] = BytesToBits(br.ReadBytes(font.plgc.tile_length));
                if (font.plgc.rotateMode == 2)
                    font.plgc.tiles[i] = Rotate270(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);
                else if (font.plgc.rotateMode == 1)
                    font.plgc.tiles[i] = Rotate90(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);
                else if (font.plgc.rotateMode == 3)
                    font.plgc.tiles[i] = Rotate180(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);

            }


            // Character Width DH
            br.BaseStream.Position = font.fnif.offset_hdwc - 0x08;
            font.hdwc.type = br.ReadChars(4);
            font.hdwc.block_size = br.ReadUInt32();
            font.hdwc.fist_code = br.ReadUInt16();
            font.hdwc.last_code = br.ReadUInt16();
            font.hdwc.unknown1 = br.ReadUInt32();

            font.hdwc.info = new List<sNFTR.HDWC.Info>();
            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                sNFTR.HDWC.Info info = new sNFTR.HDWC.Info();
                info.pixel_start = br.ReadByte();
                info.pixel_width = br.ReadByte();
                info.pixel_length = br.ReadByte();
                font.hdwc.info.Add(info);
            }

            // Character MAP
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

                switch (pamc.type_section)
                {
                    case 0:
                        sNFTR.PAMC.Type0 type0 = new sNFTR.PAMC.Type0();
                        type0.fist_char_code = br.ReadUInt16();
                        pamc.info = type0;
                        break;
                    case 1:
                        sNFTR.PAMC.Type1 type1 = new sNFTR.PAMC.Type1();
                        type1.char_code = new ushort[(pamc.block_size - 0x14) / 2];
                        for (int i = 0; i < type1.char_code.Length; i++)
                            type1.char_code[i] = br.ReadUInt16();

                        pamc.info = type1;
                        break;
                    case 2:
                        sNFTR.PAMC.Type2 type2 = new sNFTR.PAMC.Type2();
                        type2.num_chars = br.ReadUInt16();
                        type2.chars_code = new ushort[type2.num_chars];
                        type2.chars = new ushort[type2.num_chars];

                        for (int i = 0; i < type2.num_chars; i++)
                        {
                            type2.chars_code[i] = br.ReadUInt16();
                            type2.chars[i] = br.ReadUInt16();
                        }
                        pamc.info = type2;
                        break;
                }

                font.pamc.Add(pamc);
                br.BaseStream.Position = nextOffset - 0x08;
            } while (nextOffset != 0x00);

            WriteInfo(font, lang);
            font.plgc.rotateMode = 0;

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

        public static void WriteInfo(sNFTR font, string lang)
        {
            try
            {
                XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "FontLang.xml");
                xml = xml.Element(lang).Element("NFTR");

                Console.WriteLine(xml.Element("S00").Value);
                Console.WriteLine("<pre>");
                Console.WriteLine(xml.Element("S01").Value, font.header.num_blocks.ToString());


                Console.WriteLine("<b>" + xml.Element("S02").Value + "</b>", new String(font.fnif.type));
                Console.WriteLine(xml.Element("S03").Value, font.fnif.block_size.ToString("x"));
                Console.WriteLine(xml.Element("S04").Value, font.fnif.unknown1.ToString("x"));
                Console.WriteLine(xml.Element("S05").Value, font.fnif.unknown2.ToString("x"));
                Console.WriteLine(xml.Element("S06").Value, font.fnif.offset_plgc.ToString("x"));
                Console.WriteLine(xml.Element("S07").Value, font.fnif.offset_hdwc.ToString("x"));
                Console.WriteLine(xml.Element("S08").Value, font.fnif.offset_pamc.ToString("x"));
                if (font.fnif.block_size == 0x20)
                    Console.WriteLine(xml.Element("S09").Value, font.fnif.unknown3.ToString("x"));


                Console.WriteLine("<b>" + xml.Element("S02").Value + "</b>", new String(font.plgc.type));
                Console.WriteLine(xml.Element("S03").Value, font.plgc.block_size.ToString("x"));
                Console.WriteLine(xml.Element("S0A").Value, font.plgc.tile_width.ToString());
                Console.WriteLine(xml.Element("S0B").Value, font.plgc.tile_height.ToString());
                Console.WriteLine(xml.Element("S0C").Value, font.plgc.tile_length.ToString());
                Console.WriteLine(xml.Element("S0D").Value, font.plgc.unknown.ToString("x"));
                Console.WriteLine(xml.Element("S0E").Value, font.plgc.depth.ToString());
                Console.WriteLine(xml.Element("S0F").Value, font.plgc.rotateMode.ToString());
                Console.WriteLine(xml.Element("S10").Value, font.plgc.unknown2.ToString());

                Console.WriteLine("<b>" + xml.Element("S02").Value + "</b>", new String(font.hdwc.type));
                Console.WriteLine(xml.Element("S03").Value, font.hdwc.block_size.ToString("x"));
                Console.WriteLine(xml.Element("S11").Value, font.hdwc.fist_code.ToString("x"));
                Console.WriteLine(xml.Element("S12").Value, font.hdwc.last_code.ToString("x"));
                Console.WriteLine(xml.Element("S13").Value, font.hdwc.unknown1.ToString("x"));

                Console.WriteLine("<b>" + xml.Element("S02").Value + "</b>", "PAMC");
                for (int i = 0; i < font.pamc.Count; i++)
                {
                    sNFTR.PAMC pamc = font.pamc[i];
                    Console.WriteLine("<u>" + xml.Element("S02").Value + "</u>", i.ToString());
                    Console.WriteLine("  |__" + xml.Element("S03").Value, pamc.block_size.ToString("x"));
                    Console.WriteLine("  |_" + xml.Element("S14").Value, pamc.first_char.ToString("x"));
                    Console.WriteLine("  |_" + xml.Element("S15").Value, pamc.last_char.ToString("x"));
                    Console.WriteLine("  |_" + xml.Element("S16").Value, pamc.type_section.ToString());

                    switch (pamc.type_section)
                    {
                        case 0:
                            sNFTR.PAMC.Type0 type0 = (sNFTR.PAMC.Type0)pamc.info;
                            Console.WriteLine("    \\_" + xml.Element("S17").Value, 0);
                            Console.WriteLine("    \\_" + xml.Element("S18").Value, type0.fist_char_code.ToString("x"));
                            break;

                        case 1:
                            sNFTR.PAMC.Type1 type1 = (sNFTR.PAMC.Type1)pamc.info;
                            Console.WriteLine("    \\_" + xml.Element("S17").Value, 1);
                            Console.WriteLine("    \\_" + xml.Element("S19").Value, type1.char_code.Length.ToString());
                            for (int j = 0; j < type1.char_code.Length; j++)
                                Console.WriteLine("    |_" + xml.Element("S1A").Value, j.ToString(), type1.char_code[j].ToString("x"),
                                        Encoding.GetEncoding("shift-jis").GetChars(
                                        BitConverter.GetBytes(pamc.first_char + j).Reverse().ToArray()).Reverse().ToArray()[0]);
                            break;

                        case 2:
                            sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)pamc.info;
                            Console.WriteLine("    \\_" + xml.Element("S17").Value, 2);
                            Console.WriteLine("    \\_" + xml.Element("S1B").Value, type2.num_chars.ToString());
                            for (int j = 0; j < type2.num_chars; j++)
                            {
                                Console.WriteLine("    |_" + xml.Element("S1C").Value, j.ToString(), type2.chars_code[j].ToString("x"));
                                Console.WriteLine("    |_" + xml.Element("S1D").Value, j.ToString(), type2.chars[j].ToString());
                            }
                            break;
                    }
                }

                Console.WriteLine("EOF</pre>");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
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

        public static Byte[] Rotate270(Byte[] bytes,int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[(h + width * (height - 1) - w * width) * depth + i] = original[i];
                }
            }

            return rotated;
        }
        public static Byte[] Rotate90(Byte[] bytes, int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[((width - 1 - h) + w * width) * depth + i] = original[i];
                }
            }

            return rotated;
        }
        public static Byte[] Rotate180(Byte[] bytes, int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[((width - 1) + (width -1) * height - w - h * width) * depth + i] = original[i];
                }
            }

            return rotated;
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
