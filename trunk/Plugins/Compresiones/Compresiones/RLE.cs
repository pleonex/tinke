using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class RLE
    {
        static int MAX_OUTSIZE = 0x200000;
        static bool showAlways = true;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        public static void DecompressRLE(string filein, string fileout)
        {
            /*  SWI 14h (GBA/NDS7/NDS9) - RLUnCompWram
                SWI 15h (GBA/NDS7/NDS9) - RLUnCompVram (NDS: with Callback)
                Expands run-length compressed data. The Wram function is faster, and writes in units of 8bits. For the Vram function the destination must be halfword aligned, data is written in units of 16bits.
                If the size of the compressed data is not a multiple of 4, please adjust it as much as possible by padding with 0. Align the source address to a 4Byte boundary.

                  r0  Source Address, pointing to data as such:
                       Data header (32bit)
                         Bit 0-3   Reserved
                         Bit 4-7   Compressed type (must be 3 for run-length)
                         Bit 8-31  Size of decompressed data
                       Repeat below. Each Flag Byte followed by one or more Data Bytes.
                       Flag data (8bit)
                         Bit 0-6   Expanded Data Length (uncompressed N-1, compressed N-3)
                         Bit 7     Flag (0=uncompressed, 1=compressed)
                       Data Byte(s) - N uncompressed bytes, or 1 byte repeated N times
                  r1  Destination Address
                  r2  Callback parameter (NDS SWI 15h only, see Callback notes below)
                  r3  Callback structure (NDS SWI 15h only, see Callback notes below)

                Return: No return value, Data written to destination address.*/

            System.Xml.Linq.XElement xml = Basico.ObtenerTraduccion("Compression");
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception(xml.Element("S00").Value);
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0, curr_size = 0;
            int i, rl;
            byte flag, b;
            bool compressed;

            if (br.ReadByte() != RLE_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != RLE_TAG)
                    throw new InvalidDataException(String.Format(xml.Element("S10").Value, filein));
            }
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format(xml.Element("S02").Value, filein, MAX_OUTSIZE));

            if (showAlways)
                Console.WriteLine(xml.Element("S03").Value, filein, decomp_size);

            #region decompress
            byte[] outdata = new byte[decomp_size];

            while (true)
            {
                // get tag
                try { flag = br.ReadByte(); }
                catch (EndOfStreamException) { break; }
                compressed = (flag & 0x80) > 0;
                rl = flag & 0x7F;
                if (compressed)
                    rl += 3;
                else
                    rl += 1;
                //curr_size += rl;
                if (compressed)
                {
                    try { b = br.ReadByte(); }
                    catch (EndOfStreamException) { break; }// throw new Exception(String.Format("Invalid RLE format in file {0:s}; incomplete data near EOF.", filein)); }
                    for (i = 0; i < rl; i++)
                        outdata[curr_size++] = b;
                }
                else
                    for (i = 0; i < rl; i++)
                        try { outdata[curr_size++] = br.ReadByte(); }
                        catch (EndOfStreamException) { break; }// throw new Exception(String.Format("Invalid RLE format in file {0:s}; incomplete data near EOF.", filein)); }

                if (curr_size > decomp_size)
                {
                    Console.WriteLine("curr_size > decomp_size; {0:x}>{1:x}", curr_size, decomp_size);
                    break;// throw new Exception(String.Format("File {0:s} is not a valid LZSS file; actual output size > output size in header", filein));
                }
                if (curr_size == decomp_size)
                    break;
            }
            #endregion


            BinaryWriter bw = new BinaryWriter(new FileStream(fileout, FileMode.Create));
            bw.Write(outdata);
            bw.Flush();
            bw.Close();

            br.Close();
            fstr.Close();
            fstr.Dispose();

            Console.WriteLine(xml.Element("S11").Value, filein);
        }

    }
}
