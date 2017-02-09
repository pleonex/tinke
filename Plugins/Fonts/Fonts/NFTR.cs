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
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Linq;
using Ekona;

namespace Fonts
{

    // Credits to CUE and Lyan53 in romxhack, thanks:
    // http://romxhack.esforos.com/fuentes-nftr-de-nds-t67
    public static class NFTR
    {
        const int CHARS_PER_LINE = 16;
        public const int BORDER_WIDTH = 2;
        static readonly Pen BORDER = new Pen(Color.Olive, BORDER_WIDTH);

        public static sNFTR Read(sFile cfile, string lang)
        {
            sNFTR font = new sNFTR();
            font.id = cfile.id;
            font.name = cfile.name;
            BinaryReader br = new BinaryReader(File.OpenRead(cfile.path));

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

            font.fnif.unknown1 = br.ReadByte();
            font.fnif.height = br.ReadByte();
            font.fnif.nullCharIndex = br.ReadUInt16();
            font.fnif.unknown4 = br.ReadByte();
            font.fnif.width = br.ReadByte();
            font.fnif.width_bis = br.ReadByte();
            font.fnif.encoding = br.ReadByte();

            font.fnif.offset_plgc = br.ReadUInt32();
            font.fnif.offset_hdwc = br.ReadUInt32();
            font.fnif.offset_pamc = br.ReadUInt32();

            if (font.fnif.block_size == 0x20)
            {
                font.fnif.height_font = br.ReadByte();
                font.fnif.widht_font = br.ReadByte();
                font.fnif.bearing_y = br.ReadByte();
                font.fnif.bearing_x = br.ReadByte();
            }

            // Character Graphics LP
            br.BaseStream.Position = font.fnif.offset_plgc - 0x08;
            font.plgc.type = br.ReadChars(4);
            font.plgc.block_size = br.ReadUInt32();
            font.plgc.tile_width = br.ReadByte();
            font.plgc.tile_height = br.ReadByte();
            font.plgc.tile_length = br.ReadUInt16();
            font.plgc.unknown = br.ReadUInt16();
            font.plgc.depth = br.ReadByte();
            font.plgc.rotateMode = br.ReadByte();

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
                info.pixel_start = br.ReadSByte();
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
                        type1.char_code = new ushort[pamc.last_char - pamc.first_char + 1];
                        for (int i = 0; i < type1.char_code.Length; i++)
                            type1.char_code[i] = br.ReadUInt16();

                        pamc.info = type1;
                        break;
                    case 2:
                        sNFTR.PAMC.Type2 type2 = new sNFTR.PAMC.Type2();
                        type2.num_chars = br.ReadUInt16();
                        type2.charInfo = new sNFTR.PAMC.Type2.CharInfo[type2.num_chars];

                        for (int i = 0; i < type2.num_chars; i++)
                        {
                            type2.charInfo[i].chars_code = br.ReadUInt16();
                            type2.charInfo[i].chars = br.ReadUInt16();
                        }
                        pamc.info = type2;
                        break;
                }

                font.pamc.Add(pamc);
                br.BaseStream.Position = nextOffset - 0x08;
            } while (nextOffset != 0x00 && (nextOffset - 0x08) < br.BaseStream.Length);

            //WriteInfo(font, lang);
            font.plgc.rotateMode = 0;

            br.Close();
            return font;
        }
        public static void Write(sNFTR font, string fileout)
        {
            // Calculate de size of each block
            font.plgc.block_size = (uint)(0x10 + font.plgc.tiles.Length * font.plgc.tile_length);
            if (font.plgc.block_size % 4 != 0)  // Padding
                font.plgc.block_size += (4 - (font.plgc.block_size % 4));

            font.hdwc.block_size = (uint)(0x10 + font.hdwc.info.Count * 3);
            if (font.hdwc.block_size % 4 != 0)  // Padding
                font.hdwc.block_size += (4 - (font.hdwc.block_size % 4));
               
            uint pacm_totalSize = 0x00;
            for (int i = 0; i < font.pamc.Count; i++)
                pacm_totalSize += font.pamc[i].block_size;
            font.header.file_size = font.header.block_size + font.fnif.block_size + font.plgc.block_size +
                font.hdwc.block_size + pacm_totalSize;

            // Calculate the new offset
            font.fnif.offset_plgc = font.header.block_size + font.fnif.block_size + 0x08;
            font.fnif.offset_hdwc = (font.fnif.offset_plgc - 0x08) + font.plgc.block_size + 0x08;
            font.fnif.offset_pamc = (font.fnif.offset_hdwc - 0x08) + font.hdwc.block_size + 0x08;

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
            bw.Write(font.fnif.height);
            bw.Write(font.fnif.nullCharIndex);
            bw.Write(font.fnif.unknown4);
            bw.Write(font.fnif.width);
            bw.Write(font.fnif.width_bis);
            bw.Write(font.fnif.encoding);
            bw.Write(font.fnif.offset_plgc);
            bw.Write(font.fnif.offset_hdwc);
            bw.Write(font.fnif.offset_pamc);
            if (font.fnif.block_size == 0x20)
            {
                bw.Write(font.fnif.height_font);
                bw.Write(font.fnif.widht_font);
                bw.Write(font.fnif.bearing_y);
                bw.Write(font.fnif.bearing_x);
            }

            // Padding
            int rem = (int)bw.BaseStream.Position % 4;
            if (rem != 0)
                for (; rem < 4; rem++)
                    bw.Write((byte)0x00);

            // Write the PLGC section
            bw.Write(font.plgc.type);
            bw.Write(font.plgc.block_size);
            bw.Write(font.plgc.tile_width);
            bw.Write(font.plgc.tile_height);
            bw.Write(font.plgc.tile_length);
            bw.Write(font.plgc.unknown);
            bw.Write(font.plgc.depth);
            bw.Write(font.plgc.rotateMode);
            for (int i = 0; i < font.plgc.tiles.Length; i++)
                bw.Write(BitsToBytes(font.plgc.tiles[i]));

            // Padding
            rem = (int)bw.BaseStream.Position % 4;
            if (rem != 0)
                for (; rem < 4; rem++)
                    bw.Write((byte)0x00);

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

            // Padding
            rem = (int)bw.BaseStream.Position % 4;
            if (rem != 0)
                for (; rem < 4; rem++)
                    bw.Write((byte)0x00);

            // Write the PAMC section
            for (int i = 0; i < font.pamc.Count; i++)
            {
                long currPos = bw.BaseStream.Position;
                uint next_section = (uint)(currPos + font.pamc[i].block_size) + 0x08;
                if (i + 1 == font.pamc.Count)
                    next_section = 0;
                bw.Write(font.pamc[i].type);
                bw.Write(font.pamc[i].block_size);
                bw.Write(font.pamc[i].first_char);
                bw.Write(font.pamc[i].last_char);
                bw.Write(font.pamc[i].type_section);
                bw.Write(next_section);

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
                            bw.Write(type2.charInfo[j].chars_code);
                            bw.Write(type2.charInfo[j].chars);
                        }
                        break;
                }
                bw.Write((ushort)0x00);
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
                Console.WriteLine(xml.Element("S1E").Value, font.fnif.height.ToString("x"));
                Console.WriteLine("Null char index: " + font.fnif.nullCharIndex.ToString());
                Console.WriteLine(xml.Element("S20").Value, font.fnif.unknown4.ToString("x"));
                Console.WriteLine(xml.Element("S21").Value, font.fnif.width.ToString("x"));
                Console.WriteLine(xml.Element("S22").Value, font.fnif.width_bis.ToString("x"));
                Console.WriteLine(xml.Element("S23").Value, font.fnif.encoding.ToString("x"));
                Console.WriteLine(xml.Element("S06").Value, font.fnif.offset_plgc.ToString("x"));
                Console.WriteLine(xml.Element("S07").Value, font.fnif.offset_hdwc.ToString("x"));
                Console.WriteLine(xml.Element("S08").Value, font.fnif.offset_pamc.ToString("x"));
                if (font.fnif.block_size == 0x20)
                {
                    Console.WriteLine(xml.Element("S24").Value, font.fnif.height_font.ToString("x"));
                    Console.WriteLine(xml.Element("S25").Value, font.fnif.widht_font.ToString("x"));
                    Console.WriteLine(xml.Element("S26").Value, font.fnif.bearing_y.ToString("x"));
                    Console.WriteLine(xml.Element("S09").Value, font.fnif.bearing_x.ToString("x"));
                }


                Console.WriteLine("<b>" + xml.Element("S02").Value + "</b>", new String(font.plgc.type));
                Console.WriteLine(xml.Element("S03").Value, font.plgc.block_size.ToString("x"));
                Console.WriteLine(xml.Element("S0A").Value, font.plgc.tile_width.ToString());
                Console.WriteLine(xml.Element("S0B").Value, font.plgc.tile_height.ToString());
                Console.WriteLine(xml.Element("S0C").Value, font.plgc.tile_length.ToString());
                Console.WriteLine(xml.Element("S0D").Value, font.plgc.unknown.ToString("x"));
                Console.WriteLine(xml.Element("S0E").Value, font.plgc.depth.ToString());
                Console.WriteLine(xml.Element("S0F").Value, font.plgc.rotateMode.ToString());
                //Console.WriteLine(xml.Element("S10").Value, font.plgc.unknown2.ToString());

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
                                Console.WriteLine("    |_" + xml.Element("S1C").Value, j.ToString(), type2.charInfo[j].chars_code.ToString("x"));
                                Console.WriteLine("    |_" + xml.Element("S1D").Value, j.ToString(), type2.charInfo[j].chars.ToString());
                            }
                            break;
                    }
                }

                Console.WriteLine("EOF</pre>");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        public static Color[] CalculatePalette(int depth, bool inverse)
        {
            Color[] palette = new Color[1 << depth];

            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = i *  (255 / (palette.Length - 1));
                if (inverse) colorIndex = 255 - colorIndex;
                palette[i] = Color.FromArgb(255, colorIndex, colorIndex, colorIndex);
            }

            return palette;
        }
        
        public static Bitmap Get_Char(sNFTR font, int id, Color[] palette, int zoom = 1)
        {
            return Get_Char(font.plgc.tiles[id], font.plgc.depth, font.plgc.tile_width, font.plgc.tile_height, font.plgc.rotateMode, palette, zoom);
        }
        public static Bitmap Get_Char(byte[] tiles, int depth, int width, int height, int rotateMode,
            Color[] palette, int zoom = 1)
        {
            Bitmap image = new Bitmap(width * zoom + 1, height * zoom + 1);
            List<Byte> tileData = new List<byte>();

            for (int i = 0; i <= tiles.Length - depth; i += depth)
            {
                Byte byteFromBits = 0;
                for (int b = depth - 1, j = 0; b >= 0; b--, j++)
                {
                    byteFromBits += (byte)(tiles[i + j] << b);
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
        public static Bitmap Get_Chars(sNFTR font, int maxWidth, Color[] palette, int zoom = 1)
        {
            int numChars = font.plgc.tiles.Length;

            // Get the image size
            int charWidth = font.plgc.tile_width * zoom + BORDER_WIDTH;
            int charHeight = font.plgc.tile_height * zoom + BORDER_WIDTH;

            int numColumns = maxWidth / (charWidth + BORDER_WIDTH);
            int numRows = (int)Math.Ceiling((double)numChars / numColumns);

            return ToImage(font, palette, charWidth, charHeight, numRows, numColumns, zoom);
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

        public static void ExportInfo(string fileOut, Dictionary<int, int> charTable, sNFTR font)
        {
            string enc_name = "utf-8";
            if (font.fnif.encoding == 2)
                enc_name = "shift_jis";
            else if (font.fnif.encoding == 1)
                enc_name = "utf-16";
            else if (font.fnif.encoding == 0)
                enc_name = "utf-8";
            else if (font.fnif.encoding == 3)
                enc_name = Encoding.GetEncoding(1252).EncodingName;

            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", enc_name, null);

            XElement root = new XElement("CharMap");

            foreach (int c in charTable.Keys)
            {
                string ch = "";
                byte[] codes = BitConverter.GetBytes(c).Reverse().ToArray();
                ch = new String(Encoding.GetEncoding(enc_name).GetChars(codes)).Replace("\0", "");

                int tileCode = charTable[c];
                if (tileCode >= font.hdwc.info.Count)
                    continue;
                sNFTR.HDWC.Info info = font.hdwc.info[tileCode];

                XElement chx = new XElement("CharInfo");
                chx.SetAttributeValue("Char", ch);
                chx.SetAttributeValue("Code", c.ToString("x"));
                chx.SetAttributeValue("Index", tileCode.ToString());
                chx.SetAttributeValue("Width", info.pixel_length.ToString());
                root.Add(chx);
            }

            doc.Add(root);
            doc.Save(fileOut);
        }

        public static void ExportFullInfo(string fileOut, sNFTR font)
        {
            string encName = null;
            switch (font.fnif.encoding)
	        {
                case 0: encName = "utf-8"; break;
                case 1: encName = "utf-16"; break;
                case 2: encName = "shift_jis"; break;
                case 3: encName = Encoding.GetEncoding(1252).EncodingName; break;
	        }

            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", encName, "yes");
            XElement root = new XElement("NFTR");

            // Export general info
            // FNIF data
            XElement xmlFnif = new XElement("FNIF");
            xmlFnif.Add(new XElement("Unknown1", font.fnif.unknown1));
            xmlFnif.Add(new XElement("Height", font.fnif.height));
            xmlFnif.Add(new XElement("NullCharIndex", font.fnif.nullCharIndex));
            xmlFnif.Add(new XElement("Unknown2", font.fnif.unknown4));
            xmlFnif.Add(new XElement("Width", font.fnif.width));
            xmlFnif.Add(new XElement("WidthBis", font.fnif.width_bis));
            xmlFnif.Add(new XElement("Encoding", encName));
            if (font.fnif.block_size == 0x20)
            {
                xmlFnif.Add(new XElement("GlyphHeight", font.fnif.height_font));
                xmlFnif.Add(new XElement("GlyphWidth", font.fnif.widht_font));
                xmlFnif.Add(new XElement("BearingY", font.fnif.bearing_y));
                xmlFnif.Add(new XElement("BearingX", font.fnif.bearing_x));
            }
            root.Add(xmlFnif);

            doc.Add(root);
            doc.Save(fileOut);
            root = null;
            doc = null;
        }

        public static void FromImage(Bitmap image, sNFTR font, Color[] palette)
        {
            int numChars = font.plgc.tiles.Length;

            // Get the image size
            int numColumns = (numChars < CHARS_PER_LINE) ? numChars : CHARS_PER_LINE;
            int numRows = (int)Math.Ceiling((double)numChars / numColumns);

            int charWidth = font.plgc.tile_width + BORDER_WIDTH;
            int charHeight = font.plgc.tile_height + BORDER_WIDTH;

            int width = numColumns * charWidth + BORDER_WIDTH;
            int height = numRows * charHeight + BORDER_WIDTH;

            if (width != image.Width || height != image.Height)
            {
                System.Windows.Forms.MessageBox.Show("Incorrect size.");
                return;
            }

            // Draw chars
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    int index = i * numColumns + j;
                    if (index >= numChars)
                        break;

                    int x = j * charWidth + BORDER_WIDTH;
                    int y = i * charHeight + BORDER_WIDTH;

                    Bitmap charImg = image.Clone(new Rectangle(x, y, charWidth - BORDER_WIDTH, charHeight - BORDER_WIDTH),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    font.plgc.tiles[index] = SetChar(charImg, font.plgc.depth, palette);
                }
            }
        }
        private static byte[] SetChar(Bitmap img, int encoding, Color[] palette)
        {
            int numPixels = img.Width * img.Height;
            int numBits = numPixels * encoding;
            if (numBits % 8 != 0)
                numBits += 8 - (numBits % 8);
            byte[] data = new byte[numBits];

            int bitsWritten = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color color = img.GetPixel(x, y);
                    int index = Array.FindIndex(palette, c => c.Equals(color));
                    if (index == -1)
                    {
                        Console.WriteLine("Color not found: {0}", color.ToString());
                        return data;
                    }

                    for (int b = encoding - 1; b >= 0; b--)
                        data[bitsWritten++] = (byte)((index >> b) & 0x01);
                }
            }

            return data;
        }
        
        private static Bitmap ToImage(sNFTR font, Color[] palette, int charWidth, int charHeight,
                                      int numRows, int numColumns, int zoom)
        {
            int numChars = font.plgc.tiles.Length;
            int width = numColumns * charWidth + BORDER_WIDTH;
            int height = numRows * charHeight + BORDER_WIDTH;

            Bitmap image = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(image);

            // Draw chars
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    int index = i * numColumns + j;
                    if (index >= numChars)
                        break;

                    int x = j * charWidth + BORDER_WIDTH;
                    int y = i * charHeight + BORDER_WIDTH;

                    int align = BORDER_WIDTH - (BORDER_WIDTH / 2);
                    graphic.DrawRectangle(BORDER, x - align, y - align, charWidth, charHeight);
                    graphic.DrawImage(Get_Char(font, index, palette, zoom), x, y);
                }
            }

            graphic.Dispose();
            graphic = null;
            return image;
        }
        public static Bitmap ToImage(sNFTR font, Color[] palette)
        {
            int numChars = font.plgc.tiles.Length;

            // Get the image size
            int numColumns = (numChars < CHARS_PER_LINE) ? numChars : CHARS_PER_LINE;
            int numRows = (int)Math.Ceiling((double)numChars / numColumns);

            int charWidth = font.plgc.tile_width + BORDER_WIDTH;
            int charHeight = font.plgc.tile_height + BORDER_WIDTH;

            return ToImage(font, palette, charWidth, charHeight, numRows, numColumns, 1);
        }
    }

    public struct sNFTR // Nitro FonT Resource
    {
        public string name;
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

            public byte unknown1;       // Usually 0x00
            public byte height;
            public ushort nullCharIndex;
            public byte unknown4;       // Usually 0x00
            public byte width;
            public byte width_bis;
            public byte encoding;       // Could be 0(utf-8), 1(utf-16), 2(s-jis) or 3(cp1252)

            public uint offset_plgc;
            public uint offset_hdwc;
            public uint offset_pamc;

            public byte height_font;
            public byte widht_font;
            public byte bearing_y;
            public byte bearing_x;       // Usually 0x00
        }
        public struct PLGC // Character Graphics LP
        {
            public char[] type;
            public uint block_size;
            public byte tile_width;
            public byte tile_height;
            public ushort tile_length;
            public ushort unknown;
            public byte depth;
            public byte rotateMode;
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
                public sbyte pixel_start;
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
                public CharInfo[] charInfo;

                public struct CharInfo
                {
                    public ushort chars_code;
                    public ushort chars;
                }
            }
        }
    }
}
