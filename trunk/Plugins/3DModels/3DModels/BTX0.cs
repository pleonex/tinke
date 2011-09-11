using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace _3DModels
{
    public static class BTX0
    {
        public static sBTX0 Read(string file, int id, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sBTX0 btx = new sBTX0();
            btx.id = id;
            
            // Read header
            btx.header.type = br.ReadChars(4);
            btx.header.constant = br.ReadUInt32();
            btx.header.file_size = br.ReadUInt32();
            btx.header.header_size = br.ReadUInt16();
            btx.header.num_sections = br.ReadUInt16();
            btx.header.offset = new uint[btx.header.num_sections];
            for (int i = 0; i < btx.header.num_sections; i++)
                btx.header.offset[i] = br.ReadUInt32();

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
            for (int i = 0; i < tex.texInfo.num_objs; i++)
            {
                sBTX0.Texture.TextInfo texInfo = new sBTX0.Texture.TextInfo();
                texInfo.tex_offset = (ushort)(br.ReadUInt16() << 3);
                texInfo.parameters = br.ReadUInt16();
                texInfo.width2 = br.ReadByte();
                texInfo.unknown = br.ReadByte();
                texInfo.unknown2 = br.ReadByte();
                texInfo.unknown3 = br.ReadByte();

                texInfo.paletteID = (byte)(((texInfo.parameters >> 13) & 1));
                texInfo.format = (byte)((texInfo.parameters >> 10) & 3);
                texInfo.height = (byte)(8 << ((texInfo.parameters >> 7) & 3));
                texInfo.width = (byte)(8 << ((texInfo.parameters >> 4) & 3));
                texInfo.depth = FormatTable[texInfo.format];
                Console.WriteLine("BTX depth: {0}", texInfo.format.ToString());

                tex.texInfo.infoBlock.infoData[i] = texInfo;

                // Read the texture data
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = btx.header.offset[0] + tex.header.textData_offset + texInfo.tex_offset;
                int texLength = texInfo.width * texInfo.height * texInfo.depth / 8;
                tex.texture_data[i] = br.ReadBytes(texLength);
                br.BaseStream.Position = currPos;
            }
            tex.texInfo.names = new string[tex.texInfo.num_objs];
            for (int i = 0; i < tex.texInfo.num_objs; i++)
                tex.texInfo.names[i] = new String(br.ReadChars(0x10));
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
                palInfo.palette_offset = (ushort)(br.ReadUInt16() << 3);
                palInfo.unknown1 = br.ReadUInt16();
                tex.palInfo.infoBlock.infoData[i] = palInfo;

                // Read the palette data
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = tex.header.paletteData_offset + btx.header.offset[0] + palInfo.palette_offset;
                tex.palette_data[i] = br.ReadBytes((int)tex.header.paletteData_size);
                br.BaseStream.Position = currPos;
            }
            tex.palInfo.names = new string[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
                tex.palInfo.names[i] = new String(br.ReadChars(0x10));
            #endregion            

            btx.texture = tex;
            #endregion

            br.Close();
            return btx;
        }

        public static byte[] FormatTable = new byte[] {
            0, 8, 2, 4, 8, 2, 8, 16
        };
    }

    public struct sBTX0
    {
        public int id;
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

            public struct TextInfo
            {
                public ushort tex_offset;
                public ushort parameters;
                public byte width2;
                public byte unknown;
                public byte unknown2;
                public byte unknown3;

                public byte depth;
                public byte paletteID;
                public byte format;
                public byte height;
                public byte width;
            }
            public struct PalInfo
            {
                public ushort palette_offset;
                public ushort unknown1;
            }
        }
    }
}
