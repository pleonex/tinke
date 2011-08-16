using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class None
    {
        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        public static void DecompressNone(string filein, string fileout)
        {
            System.Xml.Linq.XElement xml = Basico.ObtenerTraduccion("Compression");
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception(xml.Element("S00").Value);
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0;
            int i;

            if (br.ReadByte() != NONE_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != NONE_TAG)
                    throw new InvalidDataException(String.Format(xml.Element("S12").Value, filein));
            }
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size != fstr.Length - 0x04)
                throw new InvalidDataException(xml.Element("S13").Value);

            BinaryWriter bw = new BinaryWriter(new FileStream(fileout, FileMode.Create));
            bw.Write(br.ReadBytes((int)decomp_size));
            bw.Flush();
            bw.Close();

            br.Close();
            fstr.Close();
            fstr.Dispose();

            Console.WriteLine(xml.Element("S14").Value, filein);
        }

    }
}
