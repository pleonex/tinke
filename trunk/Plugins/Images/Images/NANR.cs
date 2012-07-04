using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NANR
    {
        IPluginHost pluginHost;
        string fileName;
        int id;

        sNANR nanr;

        public NANR(IPluginHost pluginHost, string file, int id)
        {
            this.pluginHost = pluginHost;
            fileName = Path.GetFileName(file);
            this.id = id;

            Read(file);
        }

        public void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            nanr = new sNANR();

            // Generic header
            nanr.header.id = br.ReadChars(4);
            nanr.header.endianess = br.ReadUInt16();
            if (nanr.header.endianess == 0xFFFE)
                nanr.header.id.Reverse<char>();
            nanr.header.constant = br.ReadUInt16();
            nanr.header.file_size = br.ReadUInt32();
            nanr.header.header_size = br.ReadUInt16();
            nanr.header.nSection = br.ReadUInt16();

            #region ABNK
            // ABNK (Animation BaNK)
            nanr.abnk.id = br.ReadChars(4);
            nanr.abnk.length = br.ReadUInt32();
            nanr.abnk.nBanks = br.ReadUInt16();
            nanr.abnk.tFrames = br.ReadUInt16();
            nanr.abnk.constant = br.ReadUInt32();
            nanr.abnk.offset1 = br.ReadUInt32();
            nanr.abnk.offset2 = br.ReadUInt32();
            nanr.abnk.padding = br.ReadUInt64();
            nanr.abnk.anis = new sNANR.Animation[nanr.abnk.nBanks];

            // Bank header
            for (int i = 0; i < nanr.abnk.nBanks; i++)
            {
                br.BaseStream.Position = 0x30 + i * 0x10;

                sNANR.Animation ani = new sNANR.Animation();
                ani.nFrames = br.ReadUInt32();
                ani.dataType = br.ReadUInt16();
                ani.unknown1 = br.ReadUInt16();
                ani.unknown2 = br.ReadUInt16();
                ani.unknown3 = br.ReadUInt16();
                ani.offset_frame = br.ReadUInt32();
                ani.frames = new sNANR.Frame[ani.nFrames];

                // Frame header
                for (int j = 0; j < ani.nFrames; j++)
                {
                    br.BaseStream.Position = 0x18 + nanr.abnk.offset1 + j * 0x08 + ani.offset_frame;

                    sNANR.Frame frame = new sNANR.Frame();
                    frame.offset_data = br.ReadUInt32();
                    frame.unknown1 = br.ReadUInt16();
                    frame.constant = br.ReadUInt16();

                    // Frame data
                    br.BaseStream.Position = 0x18 + nanr.abnk.offset2 + frame.offset_data;
                    frame.data.nCell = br.ReadUInt16();

                    ani.frames[j] = frame;
                }

                nanr.abnk.anis[i] = ani;
            }
            #endregion

            #region LABL
            br.BaseStream.Position = nanr.header.header_size + nanr.abnk.length;
            List<uint> offsets = new List<uint>();
            List<String> names = new List<string>();
            nanr.labl.names = new string[nanr.abnk.nBanks];

            nanr.labl.id = br.ReadChars(4);
            if (new String(nanr.labl.id) != "LBAL")
                goto Tercera;
            nanr.labl.section_size = br.ReadUInt32();

            // Offset
            for (int i = 0; i < nanr.abnk.nBanks; i++)
            {
                uint offset = br.ReadUInt32();
                if (offset >= nanr.labl.section_size - 8)
                {
                    br.BaseStream.Position -= 4;
                    break;
                }

                offsets.Add(offset);
            }
            nanr.labl.offset = offsets.ToArray();

            // Names
            for (int i = 0; i < nanr.labl.offset.Length; i++)
            {
                names.Add("");
                byte c = br.ReadByte();
                while (c != 0x00)
                {
                    names[i] += (char)c;
                    c = br.ReadByte();
                }
            }
        Tercera:
            for (int i = 0; i < nanr.abnk.nBanks; i++)
                if (names.Count > i)
                    nanr.labl.names[i] = names[i];
                else
                    nanr.labl.names[i] = i.ToString();
            #endregion

            #region UEXT
            nanr.uext.id = br.ReadChars(4);
            if (new String(nanr.uext.id) != "TXEU")
                goto Fin;

            nanr.uext.section_size = br.ReadUInt32();
            nanr.uext.unknown = br.ReadUInt32();
            #endregion

        Fin:
            br.Close();
        }

        public String[] Names
        {
            get { return nanr.labl.names; }
        }
        public sNANR Struct
        {
            get { return nanr; }
        }

        public struct sNANR
        {
            public NitroHeader header;
            public ABNK abnk;
            public LABL labl;
            public UEXT uext;

            public struct ABNK
            {
                public char[] id;
                public uint length;
                public ushort nBanks;
                public ushort tFrames;
                public uint constant;
                public uint offset1;
                public uint offset2;
                public ulong padding;
                public Animation[] anis;
            }
            public struct Animation
            {
                public uint nFrames;
                public ushort dataType;
                public ushort unknown1;
                public ushort unknown2;
                public ushort unknown3;
                public uint offset_frame;
                public Frame[] frames;
            }
            public struct Frame
            {
                public uint offset_data;
                public ushort unknown1;
                public ushort constant;
                public Frame_Data data;
            }
            public struct Frame_Data
            {
                public ushort nCell;
                // DataType 1
                public ushort[] transform; // See http://nocash.emubase.de/gbatek.htm#lcdiobgrotationscaling
                public short xDisplacement;
                public short yDisplacement;
                //DataType 2 (the Displacement above)
                public ushort constant; // 0xBEEF
            }

            public struct LABL
            {
                public char[] id;
                public UInt32 section_size;
                public UInt32[] offset;
                public string[] names;
            }
            public struct UEXT
            {
                public char[] id;
                public UInt32 section_size;
                public UInt32 unknown;
            }
        }

    }
}
