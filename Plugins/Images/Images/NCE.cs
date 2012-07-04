using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;

namespace Images
{
    public static class NCE
    {
        //public static NCER Read(string file, IPluginHost pluginHost)
        //{
        //    BinaryReader br = new BinaryReader(File.OpenRead(file));
        //    NCER nce = new NCER();

        //    br.BaseStream.Position += 4;
        //    uint num_banks = br.ReadUInt32() / 0x08;
        //    br.BaseStream.Position = 0;

        //    nce.cebk.block_size = 0;
        //    nce.cebk.nBanks = (ushort)num_banks;
        //    nce.labl.names = new string[nce.cebk.nBanks];
        //    nce.cebk.banks = new Bank[num_banks];
        //    for (int i = 0; i < num_banks; i++)
        //    {
        //        nce.cebk.banks[i] = new Bank();
        //        nce.cebk.banks[i].nCells = br.ReadUInt16();
        //        nce.cebk.banks[i].unknown1 = br.ReadUInt16();
        //        nce.cebk.banks[i].cell_offset = br.ReadUInt32();

        //        long nextBank_pos = br.BaseStream.Position;
        //        br.BaseStream.Position = nce.cebk.banks[i].cell_offset;

        //        nce.cebk.banks[i].cells = new Cell[nce.cebk.banks[i].nCells];
        //        for (int c = 0; c < nce.cebk.banks[i].nCells; c++)
        //        {
        //            nce.cebk.banks[i].cells[c] = new Cell();
        //            nce.cebk.banks[i].cells[c].obj0.yOffset = br.ReadByte();
        //            nce.cebk.banks[i].cells[c].obj0.shape = (byte)(br.ReadByte() >> 6);
        //            nce.cebk.banks[i].cells[c].obj1.xOffset = br.ReadByte();
        //            nce.cebk.banks[i].cells[c].obj1.size = (byte)(br.ReadByte() >> 6);
        //            nce.cebk.banks[i].cells[c].obj2.tileOffset = br.ReadByte();
        //            nce.cebk.banks[i].cells[c].obj2.index_palette = (byte)(br.ReadByte() >> 4);

        //            Size size = pluginHost.Size_NCER(
        //                nce.cebk.banks[i].cells[c].obj0.shape,
        //                nce.cebk.banks[i].cells[c].obj1.size);
        //            nce.cebk.banks[i].cells[c].height = (ushort)size.Height;
        //            nce.cebk.banks[i].cells[c].width = (ushort)size.Width;
        //        }

        //        br.BaseStream.Position = nextBank_pos;

        //        nce.labl.names[i] = "Bank " + i.ToString();
        //    }


        //    br.Close();
        //    return nce;
        //}
    }
}
