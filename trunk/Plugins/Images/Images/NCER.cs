using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;
using PluginInterface.Images;

namespace Images
{
    public class NCER : SpriteBase
    {
        sNCER ncer;

        public NCER(IPluginHost pluginHost, string file, int id) : base(pluginHost, file, id) { }

        public override void Read(string fileIn)
        {
            //System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("NCER");
            //Console.WriteLine("NCER {0}<pre>", Path.GetFileName(file));
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            ncer = new sNCER();

            // Generic header
            ncer.header.id = br.ReadChars(4);
            ncer.header.endianess = br.ReadUInt16();
            if (ncer.header.endianess == 0xFFFE)
                ncer.header.id.Reverse<char>();
            ncer.header.constant = br.ReadUInt16();
            ncer.header.file_size = br.ReadUInt32();
            ncer.header.header_size = br.ReadUInt16();
            ncer.header.nSection = br.ReadUInt16();

            // CEBK (CEll BanK)
            ncer.cebk.id = br.ReadChars(4);
            ncer.cebk.section_size = br.ReadUInt32();
            ncer.cebk.nBanks = br.ReadUInt16();
            ncer.cebk.tBank = br.ReadUInt16();
            ncer.cebk.constant = br.ReadUInt32();
            ncer.cebk.block_size = br.ReadUInt32() & 0xFF;
            ncer.cebk.unknown1 = br.ReadUInt32();
            ncer.cebk.unknown2 = br.ReadUInt64();
            ncer.cebk.banks = new sNCER.Bank[ncer.cebk.nBanks];

            //Console.WriteLine(xml.Element("S0B").Value + ": 0x{0:X}", ncer.cebk.block_size);
            //Console.WriteLine(xml.Element("S0C").Value + ": 0x{0:X}", ncer.cebk.unknown1);
            //Console.WriteLine(xml.Element("S09").Value + ": {0}", ncer.cebk.tBank.ToString());
            //Console.WriteLine(xml.Element("S08").Value + ": {0}", ncer.cebk.nBanks.ToString());

            uint tilePos = 0x00; // If unknown1 != 0x00

            #region Read banks
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                ncer.cebk.banks[i].nCells = br.ReadUInt16();
                ncer.cebk.banks[i].unknown1 = br.ReadUInt16();
                ncer.cebk.banks[i].cell_offset = br.ReadUInt32();

                if (ncer.cebk.tBank == 0x01)
                {
                    ncer.cebk.banks[i].xMax = br.ReadInt16();
                    ncer.cebk.banks[i].yMax = br.ReadInt16();
                    ncer.cebk.banks[i].xMin = br.ReadInt16();
                    ncer.cebk.banks[i].yMin = br.ReadInt16();
                }

                long posicion = br.BaseStream.Position;
                if (ncer.cebk.tBank == 0x00)
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 8 + ncer.cebk.banks[i].cell_offset;
                else
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 0x10 + ncer.cebk.banks[i].cell_offset;

                //Console.WriteLine("<br>--------------");
                //Console.WriteLine(xml.Element("S01").Value + " {0}:", i.ToString());
                //Console.WriteLine("|_" + xml.Element("S19").Value + ": {0}", ncer.cebk.banks[i].nCells.ToString());
                //Console.WriteLine("|_" + xml.Element("S1A").Value + ": {0}", ncer.cebk.banks[i].unknown1.ToString());
                //Console.WriteLine("|_" + xml.Element("S1B").Value + ": {0}", ncer.cebk.banks[i].cell_offset.ToString());

                ncer.cebk.banks[i].oams = new OAM[ncer.cebk.banks[i].nCells];
                #region Read cells
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    ncer.cebk.banks[i].oams[j].num_cell = (ushort)j;

                    ushort obj0 = br.ReadUInt16();
                    ushort obj1 = br.ReadUInt16();
                    ushort obj2 = br.ReadUInt16();

                    // Obj 0
                    ncer.cebk.banks[i].oams[j].obj0.yOffset = (sbyte)(obj0 & 0xFF);
                    ncer.cebk.banks[i].oams[j].obj0.rs_flag = (byte)((obj0 >> 8) & 1);
                    if (ncer.cebk.banks[i].oams[j].obj0.rs_flag == 0)
                        ncer.cebk.banks[i].oams[j].obj0.objDisable = (byte)((obj0 >> 9) & 1);
                    else
                        ncer.cebk.banks[i].oams[j].obj0.doubleSize = (byte)((obj0 >> 9) & 1);
                    ncer.cebk.banks[i].oams[j].obj0.objMode = (byte)((obj0 >> 10) & 3);
                    ncer.cebk.banks[i].oams[j].obj0.mosaic_flag = (byte)((obj0 >> 12) & 1);
                    ncer.cebk.banks[i].oams[j].obj0.depth = (byte)((obj0 >> 13) & 1);
                    ncer.cebk.banks[i].oams[j].obj0.shape = (byte)((obj0 >> 14) & 3);

                    // Obj 1
                    ncer.cebk.banks[i].oams[j].obj1.xOffset = obj1 & 0x01FF;
                    if (ncer.cebk.banks[i].oams[j].obj1.xOffset >= 0x100)
                        ncer.cebk.banks[i].oams[j].obj1.xOffset -= 0x200;
                    if (ncer.cebk.banks[i].oams[j].obj0.rs_flag == 0)
                    {
                        ncer.cebk.banks[i].oams[j].obj1.unused = (byte)((obj1 >> 9) & 7);
                        ncer.cebk.banks[i].oams[j].obj1.flipX = (byte)((obj1 >> 12) & 1);
                        ncer.cebk.banks[i].oams[j].obj1.flipY = (byte)((obj1 >> 13) & 1);
                    }
                    else
                        ncer.cebk.banks[i].oams[j].obj1.select_param = (byte)((obj1 >> 9) & 0x1F);
                    ncer.cebk.banks[i].oams[j].obj1.size = (byte)((obj1 >> 14) & 3);

                    // Obj 2
                    ncer.cebk.banks[i].oams[j].obj2.tileOffset = (uint)(obj2 & 0x03FF);
                    if (ncer.cebk.unknown1 != 0x00)
                        ncer.cebk.banks[i].oams[j].obj2.tileOffset += tilePos;
                    ncer.cebk.banks[i].oams[j].obj2.priority = (byte)((obj2 >> 10) & 3);
                    ncer.cebk.banks[i].oams[j].obj2.index_palette = (byte)((obj2 >> 12) & 0xF);

                    // Calculate the size
                    Size cellSize = pluginHost.Get_OAMSize(ncer.cebk.banks[i].oams[j].obj0.shape, ncer.cebk.banks[i].oams[j].obj1.size);
                    ncer.cebk.banks[i].oams[j].height = (ushort)cellSize.Height;
                    ncer.cebk.banks[i].oams[j].width = (ushort)cellSize.Width;
                    if (ncer.cebk.banks[i].oams[j].obj0.doubleSize == 1)
                    {
                        ncer.cebk.banks[i].oams[j].width *= 2;
                        ncer.cebk.banks[i].oams[j].height *= 2;
                    }

                    //Console.WriteLine("|_" + xml.Element("S1C").Value + " {0}:", j.ToString());
                    //Console.WriteLine("    " + xml.Element("S1D").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj0.yOffset.ToString());
                    //Console.WriteLine("    " + xml.Element("S1E").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj1.xOffset.ToString());
                    //Console.WriteLine("    " + xml.Element("S1F").Value + ": {0}", ncer.cebk.banks[i].cells[j].width.ToString());
                    //Console.WriteLine("    " + xml.Element("S20").Value + ": {0}", ncer.cebk.banks[i].cells[j].height.ToString());
                    //Console.WriteLine("    " + xml.Element("S21").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj2.index_palette.ToString());
                    //Console.WriteLine("    " + xml.Element("S22").Value + ": {0}", (obj2 & 0x03FF).ToString());
                    //Console.WriteLine("    " + xml.Element("S23").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj2.tileOffset.ToString());
                    //Console.WriteLine("    " + "Object priority" + ": {0}", ncer.cebk.banks[i].cells[j].obj2.priority.ToString());
                }
                #endregion

                // Sort the oam using the priority value
                List<OAM> oams = new List<OAM>();
                oams.AddRange(ncer.cebk.banks[i].oams);
                oams.Sort(Comparision_Cell);
                ncer.cebk.banks[i].oams = oams.ToArray();

                // Calculate the next tileOffset if unknonw1 != 0
                if (ncer.cebk.unknown1 != 0x00 && ncer.cebk.banks[i].nCells != 0x00)
                {
                    OAM last_oam = Get_LastOAM(ncer.cebk.banks[i]);

                    int ultimaCeldaSize = (int)(last_oam.height * last_oam.width);
                    ultimaCeldaSize /= (int)(64 << (byte)ncer.cebk.block_size);
                    if (last_oam.obj0.depth == 1)
                        ultimaCeldaSize *= 2;
                    if (ultimaCeldaSize == 0)
                        ultimaCeldaSize = 1;

                    tilePos += (uint)((last_oam.obj2.tileOffset - tilePos) + ultimaCeldaSize);

                    //if (ncer.cebk.unknown1 == 0x160 && i == 5) // I don't know why it works
                    //    tilePos -= 3;
                    //if (ncer.cebk.unknown1 == 0x110 && i == 4) // (ncer.cebk.unknown1 & FC0) >> 6 (maybe ?)
                    //    tilePos -= 7;
                }
                br.BaseStream.Position = posicion;
                //Console.WriteLine("--------------");
            }
            #endregion

            #region LABL
            br.BaseStream.Position = ncer.header.header_size + ncer.cebk.section_size;
            List<uint> offsets = new List<uint>();
            List<String> names = new List<string>();
            ncer.labl.names = new string[ncer.cebk.nBanks];

            ncer.labl.id = br.ReadChars(4);
            if (new String(ncer.labl.id) != "LBAL")
                goto Tercera;
            ncer.labl.section_size = br.ReadUInt32();

            // Name offset
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                uint offset = br.ReadUInt32();
                if (offset >= ncer.labl.section_size - 8)
                {
                    br.BaseStream.Position -= 4;
                    break;
                }

                offsets.Add(offset);
            }
            ncer.labl.offset = offsets.ToArray();

            // Names
            for (int i = 0; i < ncer.labl.offset.Length; i++)
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
            for (int i = 0; i < ncer.cebk.nBanks; i++)
                if (names.Count > i)
                    ncer.labl.names[i] = names[i];
                else
                    ncer.labl.names[i] = i.ToString();
            #endregion

            #region UEXT
            ncer.uext.id = br.ReadChars(4);
            if (new String(ncer.uext.id) != "TXEU")
                goto Fin;

            ncer.uext.section_size = br.ReadUInt32();
            ncer.uext.unknown = br.ReadUInt32();
            #endregion

        Fin:
            br.Close();
            //Console.WriteLine("</pre>EOF");

            Set_Banks(Convert_Banks(), ncer.cebk.block_size, false);
        }
        private OAM Get_LastOAM(sNCER.Bank bank)
        {
            for (int i = 0; i < bank.oams.Length; i++)
                if (bank.oams[i].num_cell == bank.oams.Length - 1)
                    return bank.oams[i];

            return new OAM();
        }
        private int Comparision_Cell(OAM c1, OAM c2)
        {
            if (c1.obj2.priority < c2.obj2.priority)
                return 1;
            else if (c1.obj2.priority > c2.obj2.priority)
                return -1;
            else   // Same priority
            {
                if (c1.num_cell < c2.num_cell)
                    return 1;
                else if (c1.num_cell > c2.num_cell)
                    return -1;
                else // Same cell
                    return 0;
            }
        }
        private Bank[] Convert_Banks()
        {
            Bank[] banks = new Bank[ncer.cebk.banks.Length];
            for (int i = 0; i < banks.Length; i++)
            {
                banks[i].height = 0;
                banks[i].width = 0;
                banks[i].oams = ncer.cebk.banks[i].oams;
                if (ncer.labl.names.Length < i)
                    banks[i].name = ncer.labl.names[i];
            }
            return banks;
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public struct sNCER       // Nintendo CEll Resource
        {
            public Header header;
            public CEBK cebk;
            public LABL labl;
            public UEXT uext;

            public struct CEBK
            {
                public char[] id;
                public UInt32 section_size;
                public UInt16 nBanks;
                public UInt16 tBank;            // type of banks, 0 ó 1
                public UInt32 constant;
                public UInt32 block_size;
                public UInt32 unknown1;
                public UInt64 unknown2;         // padding?
                public Bank[] banks;
            }
            public struct Bank
            {
                public UInt16 nCells;
                public UInt16 unknown1;
                public UInt32 cell_offset;
                public OAM[] oams;

                // Extended mode
                public short xMax;
                public short yMax;
                public short xMin;
                public short yMin;
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
