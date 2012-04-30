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
                    //if (ncer.cebk.banks[i].oams[j].obj0.doubleSize == 1)
                    //{
                    //    ncer.cebk.banks[i].oams[j].width *= 2;
                    //    ncer.cebk.banks[i].oams[j].height *= 2;
                    //}

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

            Set_Banks(Convert_Banks(), ncer.cebk.block_size, true);
        }
        OAM Get_LastOAM(sNCER.Bank bank)
        {
            for (int i = 0; i < bank.oams.Length; i++)
                if (bank.oams[i].num_cell == bank.oams.Length - 1)
                    return bank.oams[i];

            return new OAM();
        }
        int Comparision_Cell(OAM c1, OAM c2)
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
        int Comparision_Cell2(OAM c1, OAM c2)
        {
            if (c1.num_cell > c2.num_cell)
                return 1;
            else if (c1.num_cell < c2.num_cell)
                return -1;
            else return 0;
        }


        Bank[] Convert_Banks()
        {
            Bank[] banks = new Bank[ncer.cebk.banks.Length];
            for (int i = 0; i < banks.Length; i++)
            {
                banks[i].height = 0;
                banks[i].width = 0;
                banks[i].oams = ncer.cebk.banks[i].oams;
                if (ncer.labl.names.Length > i)
                    banks[i].name = ncer.labl.names[i];
            }
            return banks;
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            Update_Struct();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            // Generic header
            bw.Write(ncer.header.id);
            bw.Write(ncer.header.endianess);
            bw.Write(ncer.header.constant);
            bw.Write(ncer.header.file_size);
            bw.Write(ncer.header.header_size);
            bw.Write(ncer.header.nSection);

            // CEBK section (CEll BanK)
            bw.Write(ncer.cebk.id);
            bw.Write(ncer.cebk.section_size);
            bw.Write(ncer.cebk.nBanks);
            bw.Write(ncer.cebk.tBank);
            bw.Write(ncer.cebk.constant);
            bw.Write(ncer.cebk.block_size);
            bw.Write(0x00); // I don't like when it's different to 0 ;)
            bw.Write(ncer.cebk.unknown2);

            // Banks
            for (int i = 0; i < ncer.cebk.banks.Length; i++)
            {
                bw.Write(ncer.cebk.banks[i].nCells);
                bw.Write(ncer.cebk.banks[i].unknown1);
                bw.Write(ncer.cebk.banks[i].cell_offset);

                if (ncer.cebk.tBank == 1)
                {
                    bw.Write(ncer.cebk.banks[i].xMax);
                    bw.Write(ncer.cebk.banks[i].yMax);
                    bw.Write(ncer.cebk.banks[i].xMin);
                    bw.Write(ncer.cebk.banks[i].yMin);
                }
            }

            // OAMs
            for (int i = 0; i < ncer.cebk.banks.Length; i++)
            {
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    OAM oam = ncer.cebk.banks[i].oams[j];

                    // OBJ0
                    ushort obj0 = 0;
                    obj0 += (ushort)((sbyte)(oam.obj0.yOffset) & 0xFF);
                    obj0 += (ushort)((oam.obj0.rs_flag & 1) << 8);
                    if (oam.obj0.rs_flag == 0x00)
                        obj0 += (ushort)((oam.obj0.objDisable & 1) << 9);
                    else
                        obj0 += (ushort)((oam.obj0.doubleSize & 1) << 9);
                    obj0 += (ushort)((oam.obj0.objMode & 3) << 10);
                    obj0 += (ushort)((oam.obj0.mosaic_flag & 1) << 12);
                    obj0 += (ushort)((oam.obj0.depth & 1) << 13);
                    obj0 += (ushort)((oam.obj0.shape & 3) << 14);
                    bw.Write(obj0);

                    // OBJ1
                    ushort obj1 = 0;
                    if (oam.obj1.xOffset < 0)
                        oam.obj1.xOffset += 0x200;
                    obj1 += (ushort)(oam.obj1.xOffset & 0x1FF);
                    if (oam.obj0.rs_flag == 0)
                    {
                        obj1 += (ushort)((oam.obj1.unused & 0x7) << 9);
                        obj1 += (ushort)((oam.obj1.flipX & 1) << 12);
                        obj1 += (ushort)((oam.obj1.flipY & 1) << 13);
                    }
                    else
                        obj1 += (ushort)((oam.obj1.select_param & 0x1F) << 9);
                    obj1 += (ushort)((oam.obj1.size & 3) << 14);
                    bw.Write(obj1);

                    // OBJ2
                    ushort obj2 = 0;
                    obj2 += (ushort)(oam.obj2.tileOffset & 0x3FF);
                    obj2 += (ushort)((oam.obj2.priority & 3) << 10);
                    obj2 += (ushort)((oam.obj2.index_palette & 0xF) << 12);
                    bw.Write(obj2);
                }
            }

            while (bw.BaseStream.Position % 4 != 0)
                bw.Write((byte)0x00);

            // LBAL section
            if (new String(ncer.labl.id) == "LBAL")
            {
                bw.Write(ncer.labl.id);
                bw.Write(ncer.labl.section_size);
                for (int i = 0; i < ncer.labl.offset.Length; i++)
                    bw.Write(ncer.labl.offset[i]);
                for (int i = 0; i < ncer.labl.offset.Length; i++)
                    bw.Write((ncer.labl.names[i] + '\0').ToCharArray());
            }

            // UEXT section
            if (new string(ncer.uext.id) == "UEXT")
            {
                bw.Write(ncer.uext.id);
                bw.Write(ncer.uext.section_size);
                bw.Write(ncer.uext.unknown);
            }

            bw.Flush();
            bw.Close();
        }
        void Update_Struct()
        {
            // Update OAMs and LABL section
            uint offset_cells = 0;
            uint size = 0;

            for (int i = 0; i < Banks.Length; i++)
            {
                ncer.cebk.banks[i].nCells = (ushort)Banks[i].oams.Length;
                ncer.cebk.banks[i].cell_offset = offset_cells;
                offset_cells += (uint)(Banks[i].oams.Length * 6);

                size += (uint)(ncer.cebk.tBank == 0 ? 0x08 : 0x10);
                size += (uint)(6 * Banks[i].oams.Length);

                ncer.cebk.banks[i].oams = Banks[i].oams;
                List<OAM> oams = new List<OAM>();
                oams.AddRange(ncer.cebk.banks[i].oams);
                oams.Sort(Comparision_Cell2);
                ncer.cebk.banks[i].oams = oams.ToArray();
            }

            // Update the rest
            ncer.cebk.block_size = BlockSize;
            ncer.cebk.nBanks = (ushort)Banks.Length;
            ncer.cebk.section_size = 0x20 + size;
            if (ncer.cebk.section_size % 4 != 0)
                ncer.cebk.section_size += (4 - (ncer.cebk.section_size % 4));

            // Update the header
            ncer.header.file_size = 0x10 + ncer.cebk.section_size;
            if (new String(ncer.labl.id) == "LBAL")
                ncer.header.file_size += ncer.labl.section_size;
            if (new String(ncer.uext.id) == "UEXT")
                ncer.header.file_size += ncer.uext.section_size;
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
