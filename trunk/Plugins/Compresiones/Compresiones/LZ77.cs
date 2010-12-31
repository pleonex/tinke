// Métodos obtenidos de DSDecmp

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class LZ77
    {
        static int MAX_OUTSIZE = 0x200000;
        const int N = 4096, F = 18;
        const byte THRESHOLD = 2;
        const int NIL = N;
        static bool showAlways = true;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;


        #region tag 0x10 LZ77
        public static void DecompressLZ77(string filein, string fileout)
        {
            /*  Data header (32bit)
                  Bit 0-3   Reserved
                  Bit 4-7   Compressed type (must be 1 for LZ77)
                  Bit 8-31  Size of decompressed data
                Repeat below. Each Flag Byte followed by eight Blocks.
                Flag data (8bit)
                  Bit 0-7   Type Flags for next 8 Blocks, MSB first
                Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
                  Bit 0-7   One data byte to be copied to dest
                Block Type 1 - Compressed - Copy N+3 Bytes from Dest-Disp-1 to Dest
                  Bit 0-3   Disp MSBs
                  Bit 4-7   Number of bytes to copy (minus 3)
                  Bit 8-15  Disp LSBs
             */
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Archivos más grandes de 2GB no pueden ser archivos RLE-comprimidos.");
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0, curr_size = 0;
            int flags, i, j, disp, n;
            bool flag;
            byte b;
            long cdest;

            if (br.ReadByte() != LZ77_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != LZ77_TAG)
                    throw new InvalidDataException(String.Format("El archivo {0:s} no es un archivo LZ77 válido", filein));
            }
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format("{0:s} será más largo que 0x{1:x} (0x{2:x}) y no puede ser descomprimido.", filein, MAX_OUTSIZE, decomp_size));
            else if (decomp_size == 0)
                for (i = 0; i < 4; i++)
                    decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE << 8)
                throw new Exception(String.Format("{0:s} será más largo que 0x{1:x} (0x{2:x}) y no puede ser descomprimido.", filein, MAX_OUTSIZE, decomp_size));

            if (showAlways)
                Console.WriteLine("Descomprimiendo {0:s}. (outsize: 0x{1:x})", filein, decomp_size);

            #region decompress

            byte[] outdata = new byte[decomp_size];

            while (curr_size < decomp_size)
            {
                try { flags = br.ReadByte(); }
                catch (EndOfStreamException) { break; }
                for (i = 0; i < 8; i++)
                {
                    flag = (flags & (0x80 >> i)) > 0;
                    if (flag)
                    {
                        disp = 0;
                        try { b = br.ReadByte(); }
                        catch (EndOfStreamException) { throw new Exception("Datos incompletos"); }
                        n = b >> 4;
                        disp = (b & 0x0F) << 8;
                        try { disp |= br.ReadByte(); }
                        catch (EndOfStreamException) { throw new Exception("Datos incompletos"); }
                        n += 3;
                        cdest = curr_size;
                        //Console.WriteLine("disp: 0x{0:x}", disp);
                        if (disp > curr_size)
                            throw new Exception("Cannot go back more than already written");
                        for (j = 0; j < n; j++)
                            outdata[curr_size++] = outdata[cdest - disp - 1 + j];
                        //curr_size += len;
                        if (curr_size > decomp_size)
                        {
                            //throw new Exception(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header", filein));
                            //Console.WriteLine(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header; {1:x} > {2:x}.", filein, curr_size, decomp_size));
                            break;
                        }
                    }
                    else
                    {
                        try { b = br.ReadByte(); }
                        catch (EndOfStreamException) { break; }// throw new Exception("Incomplete data"); }
                        try { outdata[curr_size++] = b; }
                        catch (IndexOutOfRangeException) { if (b == 0) break; }
                        //curr_size++;
                        if (curr_size > decomp_size)
                        {
                            //throw new Exception(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header", filein));
                            //Console.WriteLine(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header; {1:x} > {2:x}", filein, curr_size, decomp_size));
                            break;
                        }
                    }
                }

            }

            try
            {
                while (br.ReadByte() == 0) { } // if we read a non-zero, print that there is still some data
                Console.WriteLine("Too many data in file; current INPOS = {0:x}", br.BaseStream.Position - 1);
            }
            catch (EndOfStreamException) { }

            #endregion

            BinaryWriter bw = new BinaryWriter(new FileStream(fileout, FileMode.Create));
            bw.Write(outdata);
            bw.Flush();
            bw.Close();

            br.Close();
            br.Dispose();
            fstr.Close();
            fstr.Dispose();

            Console.WriteLine("LZ77 Descomprimido " + filein);

        }
        #endregion

    }
}
