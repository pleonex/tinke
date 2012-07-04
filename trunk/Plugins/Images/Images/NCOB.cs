using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NCOB : SpriteBase
    {
        sNCOB ncob;
        ImageBase img;

        public NCOB(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string fileIn)
        {
            ncob = new sNCOB();
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Read the header
            ncob.generic.id = br.ReadChars(4);
            ncob.generic.endianess = br.ReadUInt16();
            ncob.generic.constant = br.ReadUInt16();
            ncob.generic.file_size = br.ReadUInt32();
            ncob.generic.header_size = br.ReadUInt16();
            ncob.generic.nSection = br.ReadUInt16();

            for (int i = 0; i < ncob.generic.nSection; i++)
            {
                string type = new String(br.ReadChars(4));

                switch (type)
                {
                    case "CELL":
                        ncob.cell.type = "CELL".ToCharArray();
                        ncob.cell.size = br.ReadUInt32();
                        ncob.cell.num_banks = br.ReadUInt32();
                        ncob.cell.banks = new Ekona.Images.Bank[ncob.cell.num_banks];

                        for (int b = 0; b < ncob.cell.num_banks; b++)
                        {
                            ncob.cell.banks[b] = new Ekona.Images.Bank();
                            ncob.cell.banks[b].oams = new OAM[br.ReadUInt32()];

                            for (int o = 0; o < ncob.cell.banks[b].oams.Length; o++)
                            {
                                OAM oam = new OAM();
                                oam.obj1.xOffset = br.ReadInt16();
                                oam.obj0.yOffset = br.ReadInt16();
                                ushort unk1 = br.ReadUInt16();
                                if (unk1 != 0)
                                    System.Windows.Forms.MessageBox.Show("Unk1 different to 0");
                                oam.obj1.flipX = br.ReadByte();
                                oam.obj1.flipY = br.ReadByte();
                                uint unk2 = br.ReadUInt32();
                                if (unk2 != 0)
                                    System.Windows.Forms.MessageBox.Show("Unk2 different to 0");
                                oam.obj0.shape = br.ReadByte();
                                oam.obj1.size = br.ReadByte();
                                oam.obj2.priority = br.ReadByte();
                                oam.obj2.index_palette = br.ReadByte();
                                oam.obj2.tileOffset = br.ReadUInt32();

                                oam.width = (ushort)Actions.Get_OAMSize(oam.obj0.shape, oam.obj1.size).Width;
                                oam.height = (ushort)Actions.Get_OAMSize(oam.obj0.shape, oam.obj1.size).Height;
                                oam.num_cell = (ushort)o;

                                ncob.cell.banks[b].oams[o] = oam;
                            }
                        }
                        break;

                    case "CHAR":
                        ncob.chars.type = "CHAR".ToCharArray();
                        ncob.chars.size = br.ReadUInt32();
                        ncob.chars.unknown = br.ReadUInt32();
                        ncob.chars.data_size = br.ReadUInt32();
                        ncob.chars.data = br.ReadBytes((int)ncob.chars.data_size);

                        break;

                    default:
                        uint size = br.ReadUInt32();
                        br.BaseStream.Position += size - 8;
                        break;
                }
            }

            br.Close();

            img = new RawImage(ncob.chars.data, TileForm.Horizontal, ColorFormat.colors16,
                                            0x20, ncob.chars.data.Length / 0x20, false);
            Set_Banks(ncob.cell.banks, 0, false);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public ImageBase Image
        {
            get { return img; }
        }

        public struct sNCOB
        {
            public NitroHeader generic;
            public CELL cell;
            public CHAR chars;
            public GRP grp;
            public ANIM anim;
            public ACTL actl;
            public MODE mode;
            public LABL labl;
            public CMNT cmnt;
            public CCMT ccmt;
            public ECMT ecmt;
            public FCMT fcmt;
            public CLBL clbl;
            public EXTR extr;
            public LINK link;

            public struct CELL
            {
                public char[] type;
                public uint size;
                public uint num_banks;
                public Ekona.Images.Bank[] banks;
            }
            public struct CHAR
            {
                public char[] type;
                public uint size;
                public uint unknown;
                public uint data_size;
                public byte[] data;
            }
            public struct GRP
            {
                public char[] type;
                public uint size;
                public uint num_element;
                public uint unknown;

                public ulong[] data;
            }
            public struct ANIM
            {
                public char[] type;
                public uint size;
                public byte[] unknown;
            }
            public struct ACTL
            {
                public char[] type;
                public uint size;
                public uint num_element;

                public byte[][] unknown;    // 0x0C per block
            }
            public struct MODE
            {
                public char[] type;
                public uint size;
                public uint unknown1;
                public uint unknown2;
            }
            public struct LABL
            {
                public char[] type;
                public uint size;
                public uint num_element;
                public string[] names;  // 0x40 per name
            }
            public struct CMNT
            {
                public char[] type;
                public uint size;
                public uint unknown;
            }
            public struct CCMT
            {
                public char[] type;
                public uint size;
                public uint num_element;

                public ulong[] unknown;
            }
            public struct ECMT
            {
                public char[] type;
                public uint size;
                public uint num_element;

                public uint[] size_e;
                public string[] name;       // SJIS
            }
            public struct FCMT
            {
                public char[] type;
                public uint size;

                public byte[] data;
            }
            public struct CLBL
            {
                public char[] type;
                public uint size;
                public uint num_element;
                public uint[] data;
            }
            public struct EXTR
            {
                public char[] type;
                public uint size;
                public uint unknown;
            }
            public struct LINK
            {
                public char[] type;
                public uint size;
                public string link;
            }
        }
    }
}
