using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;
using PluginInterface.Images;

namespace Images
{
    public class NSCR : MapBase
    {
        sNSCR nscr;

        public NSCR(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }

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
                nscr.nrcs.mapData[i] = pluginHost.MapInfo(br.ReadUInt16());

            br.Close();

            Set_Map(nscr.nrcs.mapData, false, nscr.nrcs.width, nscr.nrcs.height);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            /*
             *             BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

                        // Common header
                        bw.Write(map.header.id);
                        bw.Write(map.header.endianess);
                        bw.Write(map.header.constant);
                        bw.Write(map.header.file_size);
                        bw.Write(map.header.header_size);
                        bw.Write(map.header.nSection);
                        // SCRN section
                        bw.Write(map.section.id);
                        bw.Write(map.section.section_size);
                        bw.Write(map.section.width);
                        bw.Write(map.section.height);
                        bw.Write(map.section.padding);
                        bw.Write(map.section.data_size);
                        for (int i = 0; i < map.section.mapData.Length; i++)
                        {
                            int npalette = map.section.mapData[i].nPalette << 12;
                            int yFlip = map.section.mapData[i].yFlip << 11;
                            int xFlip = map.section.mapData[i].xFlip << 10;
                            int data = npalette + yFlip + xFlip + map.section.mapData[i].nTile;
                            bw.Write((ushort)data);
                        }

                        bw.Flush();
                        bw.Close();
            */
        }

        public struct sNSCR      // Nintendo SCreen Resource
        {
            public Header header;
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
