using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NSCR : MapBase
    {
        sNSCR nscr;

        public NSCR(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            nscr = new sNSCR();

            // Generic header
            nscr.header.id = br.ReadChars(4);
            nscr.header.endianess = br.ReadUInt16();
            if (nscr.header.endianess == 0xFFFE)
                nscr.header.id.Reverse<char>();
            nscr.header.constant = br.ReadUInt16();
            nscr.header.file_size = br.ReadUInt32();
            nscr.header.header_size = br.ReadUInt16();
            nscr.header.nSection = br.ReadUInt16();

            // Read section
            nscr.nrcs.id = br.ReadChars(4);
            nscr.nrcs.section_size = br.ReadUInt32();
            nscr.nrcs.width = br.ReadUInt16();
            nscr.nrcs.height = br.ReadUInt16();
            nscr.nrcs.padding = br.ReadUInt32();
            nscr.nrcs.data_size = br.ReadUInt32();
            nscr.nrcs.mapData = new NTFS[nscr.nrcs.data_size / 2];

            for (int i = 0; i < nscr.nrcs.mapData.Length; i++)
                nscr.nrcs.mapData[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();

            Set_Map(nscr.nrcs.mapData, true, nscr.nrcs.width, nscr.nrcs.height);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            Update_Struct();
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            // Common header
            bw.Write(nscr.header.id);
            bw.Write(nscr.header.endianess);
            bw.Write(nscr.header.constant);
            bw.Write(nscr.header.file_size);
            bw.Write(nscr.header.header_size);
            bw.Write(nscr.header.nSection);

            // SCRN section
            bw.Write(nscr.nrcs.id);
            bw.Write(nscr.nrcs.section_size);
            bw.Write(nscr.nrcs.width);
            bw.Write(nscr.nrcs.height);
            bw.Write(nscr.nrcs.padding);
            bw.Write(nscr.nrcs.data_size);

            for (int i = 0; i < nscr.nrcs.mapData.Length; i++)
            {
                int npalette = nscr.nrcs.mapData[i].nPalette << 12;
                int yFlip = nscr.nrcs.mapData[i].yFlip << 11;
                int xFlip = nscr.nrcs.mapData[i].xFlip << 10;
                int data = npalette + yFlip + xFlip + nscr.nrcs.mapData[i].nTile;
                bw.Write((ushort)data);
            }

            bw.Flush();
            bw.Close();

        }

        private void Update_Struct()
        {
            nscr.nrcs.width = (ushort)Width;
            nscr.nrcs.height = (ushort)Height;
            nscr.nrcs.mapData = Map;
            nscr.nrcs.data_size = (uint)(Map.Length * 2);
            nscr.nrcs.section_size = nscr.nrcs.data_size + 0x14;
            nscr.header.file_size = nscr.nrcs.section_size + 0x10;
        }

        public struct sNSCR      // Nintendo SCreen Resource
        {
            public NitroHeader header;
            public NRCS nrcs;

            public struct NRCS
            {
                public char[] id;                   // NRCS = 0x4E524353
                public UInt32 section_size;
                public UInt16 width;
                public UInt16 height;
                public UInt32 padding;              // Always 0x0
                public UInt32 data_size;
                public NTFS[] mapData;
            }

        }
    }
}
