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

                texInfo.depth = FormatDepth[texInfo.format];

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
                palInfo.palette_offset = br.ReadUInt16();
                palInfo.unknown1 = br.ReadUInt16(); // Not used
                tex.palInfo.infoBlock.infoData[i] = palInfo;
            }
            tex.palInfo.names = new string[tex.palInfo.num_objs];
            for (int i = 0; i < tex.palInfo.num_objs; i++)
                tex.palInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");
            #endregion            

            btx.texture = tex;
            #endregion
            Write_Info(btx);

            br.Close();
            return btx;
        }
        private static void Write_Info(sBTX0 btx0)
        {
            Console.WriteLine("<b><pre>Read BTX0 files</b>");
            Console.WriteLine("\nTexture section\n<u>Header:</u>");
            Console.WriteLine("Texture Info offset: 0x{0}", btx0.texture.header.textInfo_offset.ToString("x"));
            Console.WriteLine("Texture Data offset: 0x{0}", btx0.texture.header.textData_offset.ToString("x"));
            Console.WriteLine("Texture Data size: 0x{0}", btx0.texture.header.textData_size.ToString("x"));
            Console.WriteLine("Compressed Texture Data offset: 0x{0}", btx0.texture.header.textCompressedData_offset.ToString("x"));
            Console.WriteLine("Compressed Texture Data size: 0x{0}", btx0.texture.header.textCompressedData_size.ToString("x"));
            Console.WriteLine("Compressed Info Offset: 0x{0}", btx0.texture.header.textCompressedInfo_offset.ToString("x"));
            Console.WriteLine("Compressed Info Data offset: 0x{0}", btx0.texture.header.textCompressedInfoData_offset.ToString("x"));
            Console.WriteLine("Palette Data offset: 0x{0}", btx0.texture.header.paletteData_offset.ToString("x"));
            Console.WriteLine("Palette Data size: 0x{0}", btx0.texture.header.paletteData_offset.ToString("x"));
            Console.WriteLine("Palette Info offset: 0x{0}", btx0.texture.header.paletteInfo_offset.ToString("x"));

            Console.WriteLine("<u>Texture Information:</u>");
            for (int i = 0; i < btx0.texture.texInfo.num_objs; i++)
            {
                sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[i];
                Console.WriteLine("*Objs: {0} ({1})", i.ToString(), btx0.texture.texInfo.names[i].Trim('\x0'));
                Console.WriteLine("|_ Offset 0x{0}", texInfo.tex_offset.ToString("x"));
                Console.WriteLine("|_ Parameters 0x{0}", texInfo.parameters.ToString("x"));
                Console.Write(" >> Repeat: X->{0}, Y->{1}", texInfo.repeat_X.ToString(), texInfo.repeat_Y.ToString());
                Console.Write(" | Flip: X-> {0}, Y->{1}", texInfo.flip_X.ToString(), texInfo.flip_Y.ToString());
                Console.Write(" | Size: {0}x{1}", texInfo.width.ToString(), texInfo.height.ToString());
                Console.Write(" | Format: {0} ({1})", texInfo.format.ToString(), (TextureFormat)texInfo.format);
                Console.Write(" | Color0: {0}", texInfo.color0.ToString());
                Console.WriteLine(" | Coordinates Transformation Modes: {0} ({1})", texInfo.coord_transf.ToString(), (TextureCoordTransf)texInfo.coord_transf);
                Console.WriteLine(" | Unknown1 {0} | Unknown2 {0} | Unknown3 {0}", texInfo.unknown.ToString(), texInfo.unknown2.ToString(), texInfo.unknown3.ToString());
            }

            Console.WriteLine("<u>Palette Information:</u>");
            for (int i = 0; i < btx0.texture.palInfo.num_objs; i++)
            {
                sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[i];
                Console.WriteLine("*Pal: {0} ({1})", i.ToString(), btx0.texture.palInfo.names[i].Trim('\x0'));
                Console.Write("|_Offset 0x{0}", palInfo.palette_offset.ToString("x"));
                Console.WriteLine();
            }
            Console.WriteLine("EOF</pre>");
        }

        //                                              0  1  2  3  4  5  6  7
        public static byte[] FormatDepth = new byte[] { 0, 8, 2, 4, 8, 0, 8, 16 };
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

                // Parameters
                public byte repeat_X;   // 0 = freeze; 1 = repeat
                public byte repeat_Y;   // 0 = freeze; 1 = repeat
                public byte flip_X;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                public byte flip_Y;     // 0 = no; 1 = flip each 2nd texture (requires repeat)
                public byte width;      // 8 << width
                public byte height;     // 8 << height
                public byte format;     // Texture format
                public byte color0; // 0 = displayed; 1 = transparent
                public byte coord_transf; // Texture coordination transformation mode
                public byte depth;
            }
            public struct PalInfo
            {
                public ushort palette_offset;
                public ushort unknown1;
            }
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
