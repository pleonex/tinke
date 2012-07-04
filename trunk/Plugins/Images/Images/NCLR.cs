using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NCLR : PaletteBase
    {
        sNCLR nclr;

        public NCLR(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            nclr = new sNCLR();

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Generic header
            nclr.header.id = br.ReadChars(4);
            nclr.header.endianess = br.ReadUInt16();
            if (nclr.header.endianess == 0xFFFE)
                nclr.header.id.Reverse<char>();
            nclr.header.constant = br.ReadUInt16();
            nclr.header.file_size = br.ReadUInt32();
            nclr.header.header_size = br.ReadUInt16();
            nclr.header.nSection = br.ReadUInt16();

            // PLTT section
            TTLP pltt = new TTLP();

            pltt.ID = br.ReadChars(4);
            pltt.length = br.ReadUInt32();
            pltt.depth = (ColorFormat)br.ReadUInt16();
            pltt.unknown1 = br.ReadUInt16();
            pltt.unknown2 = br.ReadUInt32();

            pltt.pal_length = br.ReadUInt32();
            if (pltt.pal_length == 0 || pltt.pal_length > pltt.length)
                pltt.pal_length = pltt.length - 0x18;

            uint colors_startOffset = br.ReadUInt32();
            pltt.num_colors = (uint)((pltt.depth == ColorFormat.colors16) ? 0x10 : 0x100);
            if (pltt.pal_length / 2 < pltt.num_colors)
                pltt.num_colors = pltt.pal_length / 2;
            pltt.palettes = new Color[pltt.pal_length / (pltt.num_colors * 2)][];

            br.BaseStream.Position = 0x18 + colors_startOffset;
            for (int i = 0; i < pltt.palettes.Length; i++)
                pltt.palettes[i] = Actions.BGR555ToColor(br.ReadBytes((int)pltt.num_colors * 2));

            nclr.pltt = pltt;

            // PMCP section
            if (nclr.header.nSection == 1 || br.BaseStream.Position >= br.BaseStream.Length)
                goto End;

            PMCP pmcp = new PMCP();
            pmcp.ID = br.ReadChars(4);
            pmcp.blockSize = br.ReadUInt32();
            pmcp.unknown1 = br.ReadUInt16();
            pmcp.unknown2 = br.ReadUInt16();
            pmcp.unknown3 = br.ReadUInt32();
            pmcp.first_palette_num = br.ReadUInt16();

            nclr.pmcp = pmcp;

        End:
            br.Close();
            Set_Palette(pltt.palettes, pltt.depth, true);
        }

        public override void Write(string fileOut)
        {
            Update_Struct();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(nclr.header.id);
            bw.Write(nclr.header.endianess);
            bw.Write(nclr.header.constant);
            bw.Write(nclr.header.file_size);
            bw.Write(nclr.header.header_size);
            bw.Write(nclr.header.nSection);

            bw.Write(nclr.pltt.ID);
            bw.Write(nclr.pltt.length);
            bw.Write((ushort)(nclr.pltt.depth));
            bw.Write(nclr.pltt.unknown1);
            bw.Write(nclr.pltt.unknown2);
            bw.Write(nclr.pltt.pal_length);
            bw.Write(0x10);                     // Colors start offset from 0x14

            for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                bw.Write(Actions.ColorToBGR555(nclr.pltt.palettes[i]));

            bw.Flush();
            bw.Close();
        }

        private void Update_Struct()
        {
            nclr.pltt.palettes = Palette;
            nclr.pltt.depth = Depth;

            nclr.pltt.pal_length = 0;
            for (int i = 0; i < nclr.pltt.palettes.Length; i++)
                nclr.pltt.pal_length += (uint)(nclr.pltt.palettes[i].Length * 2);
            nclr.pltt.length = nclr.pltt.pal_length + 0x18;
            nclr.header.file_size = nclr.pltt.length + 0x10;
        }

        public struct sNCLR      // Nintendo CoLor Resource
        {
            public NitroHeader header;
            public TTLP pltt;
            public PMCP pmcp;
        }
        public struct TTLP  // PaLeTTe
        {
            public char[] ID;
            public UInt32 length;
            public ColorFormat depth;
            public UInt16 unknown1;
            public UInt32 unknown2;    // padding?
            public UInt32 pal_length;
            public UInt32 num_colors;    // Number of colors
            public Color[][] palettes;
        }
        public struct PMCP
        {
            public char[] ID;
            public uint blockSize;
            public ushort unknown1;
            public ushort unknown2;     // always BEEF?
            public uint unknown3;
            public ushort first_palette_num;
        }
    }
}
