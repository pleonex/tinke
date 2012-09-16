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
using System.Windows.Forms;
using System.Drawing;
using Ekona;
using Ekona.Images;

// Copied from:
// http://llref.emutalk.net/docs/?file=xml/btx0.xml#xml-doc
// http://nocash.emubase.de/gbatek.htm#ds3dtextureattributes
// Credits to lowlines and Martin Korth, thanks :)


namespace _3DModels
{
    public static class BTX0
    {
        public static sBTX0 Read(string file, int id, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sBTX0 btx = new sBTX0();
            btx.id = id;
            btx.file = Path.GetTempFileName();
            File.Copy(file, btx.file, true);

            // Read header
            btx.header.type = br.ReadChars(4);
            btx.header.constant = br.ReadUInt32();
            btx.header.file_size = br.ReadUInt32();
            btx.header.header_size = br.ReadUInt16();
            btx.header.num_sections = br.ReadUInt16();
            btx.header.offset = new uint[btx.header.num_sections];
            for (int i = 0; i < btx.header.num_sections; i++)
                btx.header.offset[i] = br.ReadUInt32();

            if (btx.header.num_sections > 1)
                MessageBox.Show("There are more than one section?\nPlease, report this file");

            #region Read texture sections
            br.BaseStream.Position = btx.header.offset[0];
            sBTX0.Texture tex = new sBTX0.Texture();

            // Header
            tex.header.type = br.ReadChars(4);
            tex.header.section_size = br.ReadUInt32();
            tex.header.padding = br.ReadUInt32();
            tex.header.textData_size = br.ReadUInt16();
            tex.header.textInfo_offset = br.ReadUInt16();
            tex.header.padding2 = br.ReadUInt32();
            tex.header.textData_offset = br.ReadUInt32();
            tex.header.padding3 = br.ReadUInt32();
            tex.header.textCompressedData_size = (ushort)(br.ReadUInt16() << 3);
            tex.header.textCompressedInfo_offset = br.ReadUInt16();
            tex.header.padding4 = br.ReadUInt32();
            tex.header.textCompressedData_offset = br.ReadUInt32();
            tex.header.textCompressedInfoData_offset = br.ReadUInt32();
            tex.header.padding5 = br.ReadUInt32();
            tex.header.paletteData_size = (uint)(br.ReadUInt32() << 3);
            tex.header.paletteInfo_offset = br.ReadUInt32();
            tex.header.paletteData_offset = br.ReadUInt32();

            #region Texture Info
            br.BaseStream.Position = btx.header.offset[0] + tex.header.textInfo_offset;
            // Header
            tex.texInfo.dummy = br.ReadByte();
            tex.texInfo.num_objs = br.ReadByte();
            tex.texInfo.section_size = br.ReadUInt16();

            // Unknown block
            tex.texInfo.unknownBlock.header_size = br.ReadUInt16();
            tex.texInfo.unknownBlock.section_size = br.ReadUInt16();
            tex.texInfo.unknownBlock.constant = br.ReadUInt32();
            tex.texInfo.unknownBlock.unknown1 = new ushort[tex.texInfo.num_objs];
            tex.texInfo.unknownBlock.unknown2 = new ushort[tex.texInfo.num_objs];
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                tex.texInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                tex.texInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
            }

            // Info block
            tex.texInfo.infoBlock.header_size = br.ReadUInt16();
            tex.texInfo.infoBlock.data_size = br.ReadUInt16();

            tex.texInfo.infoBlock.infoData = new object[tex.texInfo.num_objs];
            tex.texture_data = new byte[tex.texInfo.num_objs][];
            uint compressedStartOffset = 0x00;
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                sBTX0.Texture.TextInfo texInfo = new sBTX0.Texture.TextInfo();
                texInfo.tex_offset = br.ReadUInt16();
                texInfo.parameters = br.ReadUInt16();
                texInfo.width2 = br.ReadByte();
                texInfo.unknown = br.ReadByte();
                texInfo.unknown2 = br.ReadByte();
                texInfo.unknown3 = br.ReadByte();

                texInfo.coord_transf = (byte)(texInfo.parameters & 14);
                texInfo.color0 = (byte)((texInfo.parameters >> 13) & 1);
                texInfo.format = (byte)((texInfo.parameters >> 10) & 7);
                texInfo.height = (byte)(8 << ((texInfo.parameters >> 7) & 7));
                texInfo.width = (byte)(8 << ((texInfo.parameters >> 4) & 7));
                texInfo.flip_Y = (byte)((texInfo.parameters >> 3) & 1);
                texInfo.flip_X = (byte)((texInfo.parameters >> 2) & 1);
                texInfo.repeat_Y = (byte)((texInfo.parameters >> 1) & 1);
                texInfo.repeat_X = (byte)(texInfo.parameters & 1);

                if (texInfo.width == 0x00)
                    switch (texInfo.unknown & 0x3)
                    {
                        case 2:
                            texInfo.width = 0x200;
                            break;
                        default:
                            texInfo.width = 0x100;
                            break;
                    }
                if (texInfo.height == 0x00)
                    switch ((texInfo.unknown >> 4) & 0x3)
                    {
                        case 2:
                            texInfo.height = 0x200;
                            break;
                        default:
                            texInfo.height = 0x100;
                            break;
                    }

                texInfo.depth = FormatDepth[texInfo.format];
                if (texInfo.format == 5)
                {
                    texInfo.compressedDataStart = compressedStartOffset;
                    compressedStartOffset += (uint)(texInfo.width * texInfo.height / 8);
                }

                tex.texInfo.infoBlock.infoData[i] = texInfo;
            }
            tex.texInfo.names = new string[tex.texInfo.num_objs];
            for (int i = 0; i < tex.texInfo.num_objs; i++)
                tex.texInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");
            #endregion

            #region Palette Info
            br.BaseStream.Position = btx.header.offset[0] + tex.header.paletteInfo_offset;
            // Header
            tex.palInfo.dummy = br.ReadByte();
            tex.palInfo.num_objs = br.ReadByte();
            tex.palInfo.section_size = br.ReadUInt16();

            // Unknown block
            tex.palInfo.unknownBlock.header_size = br.ReadUInt16();
            tex.palInfo.unknownBlock.section_size = br.ReadUInt16();
            tex.palInfo.unknownBlock.constant = br.ReadUInt32();
            tex.palInfo.unknownBlock.unknown1 = new ushort[tex.palInfo.num_objs];
            tex.palInfo.unknownBlock.unknown2 = new ushort[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
            {
                tex.palInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                tex.palInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
            }

            // Info block
            tex.palInfo.infoBlock.header_size = br.ReadUInt16();
            tex.palInfo.infoBlock.data_size = br.ReadUInt16();
            tex.palette_data = new byte[tex.palInfo.num_objs][];
            tex.palInfo.infoBlock.infoData = new object[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
            {
                sBTX0.Texture.PalInfo palInfo = new sBTX0.Texture.PalInfo();
                palInfo.palette_offset = (ushort)(br.ReadUInt16() & 0x1FFF);
                palInfo.unknown1 = br.ReadUInt16(); // Not used
                tex.palInfo.infoBlock.infoData[i] = palInfo;
            }
            tex.palInfo.names = new string[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
                tex.palInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");
            #endregion

            btx.texture = tex;
            #endregion
            Write_Info(btx, pluginHost.Get_Language());

            br.Close();
            return btx;
        }
        public static sBTX0.Texture Read_Section(ref BinaryReader br, uint texOffset, string lang)
        {
            sBTX0.Texture tex = new sBTX0.Texture();

            // Header
            tex.header.type = br.ReadChars(4);
            tex.header.section_size = br.ReadUInt32();
            tex.header.padding = br.ReadUInt32();
            tex.header.textData_size = br.ReadUInt16();
            tex.header.textInfo_offset = br.ReadUInt16();
            tex.header.padding2 = br.ReadUInt32();
            tex.header.textData_offset = br.ReadUInt32();
            tex.header.padding3 = br.ReadUInt32();
            tex.header.textCompressedData_size = (ushort)(br.ReadUInt16() << 3);
            tex.header.textCompressedInfo_offset = br.ReadUInt16();
            tex.header.padding4 = br.ReadUInt32();
            tex.header.textCompressedData_offset = br.ReadUInt32();
            tex.header.textCompressedInfoData_offset = br.ReadUInt32();
            tex.header.padding5 = br.ReadUInt32();
            tex.header.paletteData_size = (uint)(br.ReadUInt32() << 3);
            tex.header.paletteInfo_offset = br.ReadUInt32();
            tex.header.paletteData_offset = br.ReadUInt32();

            #region Texture Info
            br.BaseStream.Position = texOffset + tex.header.textInfo_offset;
            // Header
            tex.texInfo.dummy = br.ReadByte();
            tex.texInfo.num_objs = br.ReadByte();
            tex.texInfo.section_size = br.ReadUInt16();

            // Unknown block
            tex.texInfo.unknownBlock.header_size = br.ReadUInt16();
            tex.texInfo.unknownBlock.section_size = br.ReadUInt16();
            tex.texInfo.unknownBlock.constant = br.ReadUInt32();
            tex.texInfo.unknownBlock.unknown1 = new ushort[tex.texInfo.num_objs];
            tex.texInfo.unknownBlock.unknown2 = new ushort[tex.texInfo.num_objs];
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                tex.texInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                tex.texInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
            }

            // Info block
            tex.texInfo.infoBlock.header_size = br.ReadUInt16();
            tex.texInfo.infoBlock.data_size = br.ReadUInt16();

            tex.texInfo.infoBlock.infoData = new object[tex.texInfo.num_objs];
            tex.texture_data = new byte[tex.texInfo.num_objs][];
            uint compressedStartOffset = 0x00;
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                sBTX0.Texture.TextInfo texInfo = new sBTX0.Texture.TextInfo();
                texInfo.tex_offset = br.ReadUInt16();
                texInfo.parameters = br.ReadUInt16();
                texInfo.width2 = br.ReadByte();
                texInfo.unknown = br.ReadByte();
                texInfo.unknown2 = br.ReadByte();
                texInfo.unknown3 = br.ReadByte();

                texInfo.coord_transf = (byte)(texInfo.parameters & 14);
                texInfo.color0 = (byte)((texInfo.parameters >> 13) & 1);
                texInfo.format = (byte)((texInfo.parameters >> 10) & 7);
                texInfo.height = (byte)(8 << ((texInfo.parameters >> 7) & 7));
                texInfo.width = (byte)(8 << ((texInfo.parameters >> 4) & 7));
                texInfo.flip_Y = (byte)((texInfo.parameters >> 3) & 1);
                texInfo.flip_X = (byte)((texInfo.parameters >> 2) & 1);
                texInfo.repeat_Y = (byte)((texInfo.parameters >> 1) & 1);
                texInfo.repeat_X = (byte)(texInfo.parameters & 1);

                if (texInfo.width == 0x00)
                    switch (texInfo.unknown & 0x3)
                    {
                        case 2:
                            texInfo.width = 0x200;
                            break;
                        default:
                            texInfo.width = 0x100;
                            break;
                    }
                if (texInfo.height == 0x00)
                    switch ((texInfo.unknown >> 4) & 0x3)
                    {
                        case 2:
                            texInfo.height = 0x200;
                            break;
                        default:
                            texInfo.height = 0x100;
                            break;
                    }

                texInfo.depth = FormatDepth[texInfo.format];
                if (texInfo.format == 5)
                {
                    texInfo.compressedDataStart = compressedStartOffset;
                    compressedStartOffset += (uint)(texInfo.width * texInfo.height / 8);
                }

                tex.texInfo.infoBlock.infoData[i] = texInfo;
            }
            tex.texInfo.names = new string[tex.texInfo.num_objs];
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                tex.texInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");
            }
            #endregion

            #region Palette Info
            br.BaseStream.Position = texOffset + tex.header.paletteInfo_offset;
            // Header
            tex.palInfo.dummy = br.ReadByte();
            tex.palInfo.num_objs = br.ReadByte();
            tex.palInfo.section_size = br.ReadUInt16();

            // Unknown block
            tex.palInfo.unknownBlock.header_size = br.ReadUInt16();
            tex.palInfo.unknownBlock.section_size = br.ReadUInt16();
            tex.palInfo.unknownBlock.constant = br.ReadUInt32();
            tex.palInfo.unknownBlock.unknown1 = new ushort[tex.palInfo.num_objs];
            tex.palInfo.unknownBlock.unknown2 = new ushort[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
            {
                tex.palInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                tex.palInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
            }

            // Info block
            tex.palInfo.infoBlock.header_size = br.ReadUInt16();
            tex.palInfo.infoBlock.data_size = br.ReadUInt16();
            tex.palette_data = new byte[tex.palInfo.num_objs][];
            tex.palInfo.infoBlock.infoData = new object[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
            {
                sBTX0.Texture.PalInfo palInfo = new sBTX0.Texture.PalInfo();
                palInfo.palette_offset = (ushort)(br.ReadUInt16() & 0x1FFF);
                palInfo.unknown1 = br.ReadUInt16(); // Not used
                tex.palInfo.infoBlock.infoData[i] = palInfo;
            }
            tex.palInfo.names = new string[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
                tex.palInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");
            #endregion

            Write_Info(tex, lang);

            return tex;
        }

        private static void Write_Info(sBTX0 btx0, string lang)
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "3DModelsLang.xml");
                xml = xml.Element(lang).Element("BTX0");

                Console.WriteLine("<pre><b>" + xml.Element("S00").Value + "</b>");
                Console.WriteLine(xml.Element("S01").Value);
                Console.WriteLine("<u>" + xml.Element("S02").Value + "</u>");
                Console.WriteLine(xml.Element("S03").Value, btx0.texture.header.textInfo_offset.ToString("x"));
                Console.WriteLine(xml.Element("S04").Value, btx0.texture.header.textData_offset.ToString("x"));
                Console.WriteLine(xml.Element("S05").Value, btx0.texture.header.textData_size.ToString("x"));
                Console.WriteLine(xml.Element("S06").Value, btx0.texture.header.textCompressedData_offset.ToString("x"));
                Console.WriteLine(xml.Element("S07").Value, btx0.texture.header.textCompressedData_size.ToString("x"));
                Console.WriteLine(xml.Element("S08").Value, btx0.texture.header.textCompressedInfo_offset.ToString("x"));
                Console.WriteLine(xml.Element("S09").Value, btx0.texture.header.textCompressedInfoData_offset.ToString("x"));
                Console.WriteLine(xml.Element("S0A").Value, btx0.texture.header.paletteData_offset.ToString("x"));
                Console.WriteLine(xml.Element("S0B").Value, btx0.texture.header.paletteData_offset.ToString("x"));
                Console.WriteLine(xml.Element("S0C").Value, btx0.texture.header.paletteInfo_offset.ToString("x"));

                Console.WriteLine("<u>" + xml.Element("S0D").Value + "</u>");
                for (int i = 0; i < btx0.texture.texInfo.num_objs; i++)
                {
                    sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[i];
                    Console.WriteLine(xml.Element("S0E").Value, i.ToString(), btx0.texture.texInfo.names[i].Trim('\x0'));
                    Console.WriteLine(xml.Element("S0F").Value, texInfo.tex_offset.ToString("x"));
                    Console.WriteLine(xml.Element("S10").Value, texInfo.parameters.ToString("x"));
                    Console.Write(xml.Element("S11").Value, texInfo.repeat_X.ToString(), texInfo.repeat_Y.ToString());
                    Console.Write(xml.Element("S12").Value, texInfo.flip_X.ToString(), texInfo.flip_Y.ToString());
                    Console.Write(xml.Element("S13").Value, texInfo.width.ToString(), texInfo.height.ToString());
                    Console.Write(xml.Element("S14").Value, texInfo.format.ToString(), (TextureFormat)texInfo.format);
                    Console.WriteLine(xml.Element("S15").Value, texInfo.color0.ToString());
                    Console.Write(xml.Element("S16").Value, texInfo.coord_transf.ToString(), (TextureCoordTransf)texInfo.coord_transf);
                    Console.WriteLine(xml.Element("S17").Value, texInfo.unknown.ToString(), texInfo.unknown2.ToString(), texInfo.unknown3.ToString());
                }

                Console.WriteLine("<u>" + xml.Element("S18").Value + "</u>");
                for (int i = 0; i < btx0.texture.palInfo.num_objs; i++)
                {
                    sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[i];
                    Console.WriteLine(xml.Element("S19").Value, i.ToString(), btx0.texture.palInfo.names[i].Trim('\x0'));
                    Console.Write(xml.Element("S1A").Value, palInfo.palette_offset.ToString("x"));
                    Console.WriteLine();
                }
                Console.WriteLine(xml.Element("S1B").Value + "</pre>");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }
        private static void Write_Info(sBTX0.Texture btx0, string lang)
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "3DModelsLang.xml");
                xml = xml.Element(lang).Element("BTX0");

                Console.WriteLine("<pre><b>" + xml.Element("S01").Value + "</b>");
                Console.WriteLine("<u>" + xml.Element("S0D").Value + "</u>");
                for (int i = 0; i < btx0.texInfo.num_objs; i++)
                {
                    sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texInfo.infoBlock.infoData[i];
                    Console.WriteLine(xml.Element("S0E").Value, i.ToString(), btx0.texInfo.names[i].Trim('\x0'));
                    Console.WriteLine(xml.Element("S0F").Value, texInfo.tex_offset.ToString("x"));
                    Console.WriteLine(xml.Element("S10").Value, texInfo.parameters.ToString("x"));
                    Console.Write(xml.Element("S11").Value, texInfo.repeat_X.ToString(), texInfo.repeat_Y.ToString());
                    Console.Write(xml.Element("S12").Value, texInfo.flip_X.ToString(), texInfo.flip_Y.ToString());
                    Console.Write(xml.Element("S13").Value, texInfo.width.ToString(), texInfo.height.ToString());
                    Console.Write(xml.Element("S14").Value, texInfo.format.ToString(), (TextureFormat)texInfo.format);
                    Console.WriteLine(xml.Element("S15").Value, texInfo.color0.ToString());
                    Console.Write(xml.Element("S16").Value, texInfo.coord_transf.ToString(), (TextureCoordTransf)texInfo.coord_transf);
                    Console.WriteLine(xml.Element("S17").Value, texInfo.unknown.ToString(), texInfo.unknown2.ToString(), texInfo.unknown3.ToString());
                }

                Console.WriteLine("<u>" + xml.Element("S18").Value + "</u>");
                for (int i = 0; i < btx0.palInfo.num_objs; i++)
                {
                    sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.palInfo.infoBlock.infoData[i];
                    Console.WriteLine(xml.Element("S19").Value, i.ToString(), btx0.palInfo.names[i].Trim('\x0'));
                    Console.Write(xml.Element("S1A").Value, palInfo.palette_offset.ToString("x"));
                    Console.WriteLine();
                }
                Console.WriteLine(xml.Element("S1B").Value + "</pre>");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        public static System.Drawing.Bitmap GetTexture(IPluginHost pluginHost, sBTX0 btx0, int num)
        {
            if (num >= btx0.texture.texInfo.names.Length)
                num = 0;

            int num_pal;
            bool foundPal = false;
            for (num_pal = 0; num_pal < btx0.texture.palInfo.num_objs; num_pal++)
            {
                if (btx0.texture.palInfo.names[num_pal] == btx0.texture.texInfo.names[num])
                {
                    foundPal = true;
                    break;
                }
                else if (btx0.texture.palInfo.names[num_pal].Replace("_pl", "") == btx0.texture.texInfo.names[num])
                {
                    foundPal = true;
                    break;
                }
            }

            if (!foundPal)
                num_pal = 0;

            return GetTexture(pluginHost, btx0, num, num_pal);
        }
        public static Bitmap GetTexture(IPluginHost pluginHost, sBTX0 btx0, int num_tex, int num_pal)
        {
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];

            // Get texture data
            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            if (texInfo.format != 5)
                br.BaseStream.Position = texInfo.tex_offset * 8 + btx0.header.offset[0] + btx0.texture.header.textData_offset;
            else
                br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.textCompressedData_offset + texInfo.tex_offset * 8;
            Byte[] tile_data = br.ReadBytes((int)(texInfo.width * texInfo.height * texInfo.depth / 8));

            // Get palette data
            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset;
            br.BaseStream.Position += palInfo.palette_offset * 8;
            Byte[] palette_data = br.ReadBytes((int)PaletteSize[texInfo.format]);
            Color[] palette = Actions.BGR555ToColor(palette_data);
            br.Close();

            Bitmap tex;
            if (texInfo.format != 5)
                tex = Draw_Texture(pluginHost, btx0, tile_data, texInfo, palette);
            else
                tex = Draw_CompressedTexture(pluginHost, btx0, tile_data, texInfo, num_pal);

            return tex;
        }


        private static Bitmap Draw_Texture(IPluginHost pluginHost, sBTX0 btx0,
            byte[] data, sBTX0.Texture.TextInfo info, Color[] palette)
        {
            Bitmap imagen = new Bitmap(info.width, info.height);
            if (info.format == 3) // 16-color 4 bits
                data = Ekona.Helper.BitsConverter.BytesToBit4(data);
            else if (info.format == 2) // 4-color 2 bits
                data = Bit8ToBit2(data);

            for (int h = 0; h < info.height; h++)
            {
                for (int w = 0; w < info.width; w++)
                {
                    Color color = Color.Black;
                    try
                    {
                        if (info.format == 2 || info.format == 3 || info.format == 4) // 2-4-8 bits per color
                            color = palette[data[w + h * info.width]];
                        else if (info.format == 1) // A3I5 8-bit
                        {
                            int colorIndex = data[w + h * info.width] & 0x1F;
                            int alpha = (data[w + h * info.width] >> 5);
                            alpha = ((alpha * 4) + (alpha / 2)) * 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (info.format == 6) // A5I3 8-bit
                        {
                            int colorIndex = data[w + h * info.width] & 0x7;
                            int alpha = (data[w + h * info.width] >> 3);
                            alpha *= 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (info.format == 7) // Direct texture 16-bit (not tested)
                        {
                            ushort byteColor = BitConverter.ToUInt16(data, (w + h * info.width) * 2);
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

            if (info.color0 == 1)
                imagen.MakeTransparent(palette[0]);

            return imagen;
        }
        private static Bitmap Draw_CompressedTexture(IPluginHost pluginHost, sBTX0 btx0,
            byte[] data, sBTX0.Texture.TextInfo info, int num_pal)
        {
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];

            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.textCompressedInfoData_offset + info.compressedDataStart;

            Bitmap image = new Bitmap(info.width, info.height);

            for (int h = 0; h < info.height; h += 4)
            {
                for (int w = 0; w < info.width; w += 4)
                {
                    uint texData = BitConverter.ToUInt32(data, w + h * info.width / 4);

                    // Get palette data for this block
                    ushort pal_info = br.ReadUInt16();
                    int pal_offset = pal_info & 0x3FFF;
                    int pal_mode = (pal_info >> 14);

                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset + palInfo.palette_offset * 8;
                    br.BaseStream.Position += pal_offset * 4;
                    if (br.BaseStream.Position >= br.BaseStream.Length)
                        br.BaseStream.Position -= pal_offset * 4;

                    Color[] palette = Actions.BGR555ToColor(br.ReadBytes(0x08));
                    br.BaseStream.Position = currPos;

                    for (int hTex = 0; hTex < 4; hTex++)
                    {
                        byte texel_row = (byte)((texData >> (hTex * 8)) & 0xFF);
                        for (int wTex = 0; wTex < 4; wTex++)
                        {
                            byte texel = (byte)((texel_row >> (wTex * 2)) & 0x3);

                            #region Get color from Texel and mode values
                            Color color = Color.Black;
                            if (palette.Length < 4 && pal_mode != 1 && pal_mode != 3)
                                goto Draw;

                            switch (pal_mode)
                            {
                                case 0:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = palette[2];
                                    else if (texel == 3) color = Color.FromArgb(0, 0, 0, 0);  // Transparent color
                                    break;

                                case 1:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = SumColors(palette[0], palette[1], 1, 1);
                                    else if (texel == 3) color = Color.FromArgb(0, 0, 0, 0); // Transparent color
                                    break;

                                case 2:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = palette[2];
                                    else if (texel == 3) color = palette[3];
                                    break;

                                case 3:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = SumColors(palette[0], palette[1], 5, 3);
                                    else if (texel == 3) color = SumColors(palette[0], palette[1], 3, 5);
                                    break;
                            }
                            #endregion

                        Draw:
                            image.SetPixel(
                                w + wTex,
                                h + hTex,
                                color);
                        }
                    }
                }
            }

            br.Close();
            return image;
        }
        private static Color SumColors(Color a, Color b, int wa, int wb)
        {
            return Color.FromArgb(
                (a.R * wa + b.R * wb) / (wa + wb),
                (a.G * wa + b.G * wb) / (wa + wb),
                (a.B * wa + b.B * wb) / (wa + wb));
        }
        private static byte[] Bit8ToBit2(byte[] data)
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

        public static void Find_IDs(out int num_tex, out int num_pal, string tex_name, string pal_name, sBTX0.Texture texture)
        {
            num_tex = num_pal = 0;
            for (int i = 0; i < texture.texInfo.num_objs; i++)
                if (texture.texInfo.names[i] == tex_name)
                    num_tex = i;
            for (int i = 0; i < texture.palInfo.num_objs; i++)
                if (texture.palInfo.names[i] == pal_name)
                    num_pal = i;
        }
        public static void Match_Textures(ref sBMD0.Model.ModelData.Material.MatDef[] mats, string[] palNames, string[] texNames)
        {

            for (int i = 0; i < mats.Length; i++)
            {

                if (mats[i].palName == null && mats[i].texName != "")     // Search palette
                {
                    for (int p = 0; p < palNames.Length; p++)
                    {
                        if (palNames[p] == mats[i].texName || palNames[p].Replace("_pl", "") == mats[i].texName)
                        {
                            mats[i].palName = palNames[p];
                            mats[i].palID = (byte)p;
                            break;
                        }
                    }
                }
                else if (mats[i].texName == null && mats[i].palName != "")        // Search texture
                {
                    for (int t = 0; t < texNames.Length; t++)
                    {
                        if (texNames[t] == mats[i].palName || texNames[t] == mats[i].palName.Replace("_pl", ""))
                        {
                            mats[i].texName = texNames[t];
                            mats[i].texID = (byte)t;
                            break;
                        }
                    }
                }
                //else if (mats[i].texName != mats[i].palName && mats[i].texName != mats[i].palName.Replace("_pl", ""))
                //{
                //    for (int p = 0; p < palNames.Length; p++)
                //    {
                //        if (palNames[p] == mats[i].texName || palNames[p].Replace("_pl", "") == mats[i].texName)
                //        {
                //            mats[i].palName = palNames[p];
                //            mats[i].palID = (byte)p;
                //        }
                //    }

                //    //for (int t = 0; t < texNames.Length; t++)
                //    //{
                //    //    if (texNames[t] == mats[i].palName || texNames[t] == mats[i].palName.Replace("_pl", ""))
                //    //    {
                //    //        mats[i].texName = texNames[t];
                //    //        mats[i].texID = (byte)t;
                //    //    }
                //    //}
                //}
            }
        }

        //                                              0  1  2  3  4  5  6  7
        public static byte[] FormatDepth = new byte[] { 0, 8, 2, 4, 8, 2, 8, 16 };

        static int[] PaletteSize = new int[] { 0x00, 0x40, 0x08, 0x20, 0x200, 0x200, 0x10, 0x00 };
    }

    public struct sBTX0
    {
        public int id;
        public string file;
        public Header header;
        public Texture texture;

        public struct Header
        {
            public char[] type;
            public uint constant;
            public uint file_size;
            public ushort header_size;
            public ushort num_sections;
            public uint[] offset;
        }
        public struct Texture
        {
            public Header header;
            public Info3D texInfo;
            public Info3D palInfo;

            public byte[][] texture_data;
            public byte[] texture_compressedData;
            public byte[] texture_compressedInfoData;
            public byte[][] palette_data;

            public struct Header
            {
                public char[] type;
                public uint section_size;
                public uint padding;
                public ushort textData_size;
                public ushort textInfo_offset;
                public uint padding2;
                public uint textData_offset;
                public uint padding3;
                public ushort textCompressedData_size;
                public ushort textCompressedInfo_offset;
                public uint padding4;
                public uint textCompressedData_offset;
                public uint textCompressedInfoData_offset;
                public uint padding5;
                public uint paletteData_size;
                public uint paletteInfo_offset;
                public uint paletteData_offset;
            }

            public struct TextInfo
            {
                public ushort tex_offset;
                public ushort parameters;
                public byte width2;
                public byte unknown;
                public byte unknown2;
                public byte unknown3;

                // Parameters
                public byte repeat_X;   // 0 = freeze; 1 = repeat
                public byte repeat_Y;   // 0 = freeze; 1 = repeat
                public byte flip_X;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                public byte flip_Y;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                public ushort width;      // 8 << width
                public ushort height;     // 8 << height
                public byte format;     // Texture format
                public byte color0; // 0 = displayed; 1 = transparent
                public byte coord_transf; // Texture coordination transformation mode

                public byte depth;
                public uint compressedDataStart;
            }
            public struct PalInfo
            {
                public ushort palette_offset;
                public ushort unknown1;
            }
        }
    }
    public struct Info3D
    {
        public byte dummy;
        public byte num_objs;
        public ushort section_size;
        public UnknownBlock unknownBlock;
        public Info infoBlock;
        public string[] names;

        public struct UnknownBlock
        {
            public ushort header_size;
            public ushort section_size;
            public uint constant; // 0x017F

            public ushort[] unknown1;
            public ushort[] unknown2;
        }
        public struct Info
        {
            public ushort header_size;
            public ushort data_size;
            public object[] infoData; // TextInfo or PalInfo
        }
    }

    public enum TextureFormat : byte
    {
        No_Texture = 0,
        A3I5 = 1,
        Color4 = 2,
        Color16 = 3,
        Color256 = 4,
        Compressed_Texel4x4 = 5,
        A5I3 = 6,
        Direct_Texture = 7

    }
    public enum TextureCoordTransf : byte
    {
        Not_Transform = 0,
        TexCoord_source = 1,
        Normal_source = 2,
        Vertex_source = 3
    }
}
