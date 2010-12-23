using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class LZSS
    {
        static int MAX_OUTSIZE = 0x200000;
        const int N = 4096, F = 18;
        const byte THRESHOLD = 2;
        const int NIL = N;
        static bool showAlways = true;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        #region tag 0x11 LZSS
        public static void Decompress11LZS(string filein, string outflr, bool isOutFolder)
        {
            /*  Data header (32bit)
                  Bit 0-3   Reserved
                  Bit 4-7   Compressed type (must be 1 for LZ77)
                  Bit 8-31  Size of decompressed data. if 0, the next 4 bytes are decompressed length
                Repeat below. Each Flag Byte followed by eight Blocks.
                Flag data (8bit)
                  Bit 0-7   Type Flags for next 8 Blocks, MSB first
                Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
                  Bit 0-7   One data byte to be copied to dest
                Block Type 1 - Compressed - Copy LEN Bytes from Dest-Disp-1 to Dest
                    If Reserved is 0: - Default
                      Bit 0-3   Disp MSBs
                      Bit 4-7   LEN - 3
                      Bit 8-15  Disp LSBs
                    If Reserved is 1: - Higher compression rates for files with (lots of) long repetitions
                      Bit 4-7   Indicator
                        If Indicator > 1:
                            Bit 0-3    Disp MSBs
                            Bit 4-7    LEN - 1 (same bits as Indicator)
                            Bit 8-15   Disp LSBs
                        If Indicator is 1:
                            Bit 0-3 and 8-19   LEN - 0x111
                            Bit 20-31          Disp
                        If Indicator is 0:
                            Bit 0-3 and 8-11   LEN - 0x11
                            Bit 12-23          Disp
                      
             */
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Filer larger than 2GB cannot be LZSS-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            int decomp_size = 0, curr_size = 0;
            int i, j, disp, len;
            bool flag;
            byte b1, bt, b2, b3, flags;
            int cdest;

            int threshold = 1;

            if (br.ReadByte() != LZSS_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != LZSS_TAG)
                    throw new InvalidDataException(String.Format("File {0:s} is not a valid LZSS-11 file", filein));
            }
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed.", filein, MAX_OUTSIZE, decomp_size));
            else if (decomp_size == 0)
                for (i = 0; i < 4; i++)
                    decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE << 8)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed.", filein, MAX_OUTSIZE, decomp_size));

            if (showAlways)
                Console.WriteLine("Decompressing {0:s}. (outsize: 0x{1:x})", filein, decomp_size);


            byte[] outdata = new byte[decomp_size];


            while (curr_size < decomp_size)
            {
                try { flags = br.ReadByte(); }
                catch (EndOfStreamException) { break; }

                for (i = 0; i < 8 && curr_size < decomp_size; i++)
                {
                    flag = (flags & (0x80 >> i)) > 0;
                    if (flag)
                    {
                        try { b1 = br.ReadByte(); }
                        catch (EndOfStreamException) { throw new Exception("Incomplete data"); }

                        switch (b1 >> 4)
                        {
                            #region case 0
                            case 0:
                                // ab cd ef
                                // =>
                                // len = abc + 0x11 = bc + 0x11
                                // disp = def

                                len = b1 << 4;
                                try { bt = br.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                                len |= bt >> 4;
                                len += 0x11;

                                disp = (bt & 0x0F) << 8;
                                try { b2 = br.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                                disp |= b2;
                                break;
                            #endregion

                            #region case 1
                            case 1:
                                // ab cd ef gh
                                // => 
                                // len = bcde + 0x111
                                // disp = fgh
                                // 10 04 92 3F => disp = 0x23F, len = 0x149 + 0x11 = 0x15A

                                try { bt = br.ReadByte(); b2 = br.ReadByte(); b3 = br.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }

                                len = (b1 & 0xF) << 12; // len = b000
                                len |= bt << 4; // len = bcd0
                                len |= (b2 >> 4); // len = bcde
                                len += 0x111; // len = bcde + 0x111
                                disp = (b2 & 0x0F) << 8; // disp = f
                                disp |= b3; // disp = fgh
                                break;
                            #endregion

                            #region other
                            default:
                                // ab cd
                                // =>
                                // len = a + threshold = a + 1
                                // disp = bcd

                                len = (b1 >> 4) + threshold;

                                disp = (b1 & 0x0F) << 8;
                                try { b2 = br.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                                disp |= b2;
                                break;
                            #endregion
                        }

                        if (disp > curr_size)
                            throw new Exception("Cannot go back more than already written");

                        cdest = curr_size;

                        for (j = 0; j < len && curr_size < decomp_size; j++)
                            outdata[curr_size++] = outdata[cdest - disp - 1 + j];

                        if (curr_size > decomp_size)
                        {
                            //throw new Exception(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header", filein));
                            //Console.WriteLine(String.Format("File {0:s} is not a valid LZ77 file; actual output size > output size in header; {1:x} > {2:x}.", filein, curr_size, decomp_size));
                            break;
                        }
                    }
                    else
                    {
                        try { outdata[curr_size++] = br.ReadByte(); }
                        catch (EndOfStreamException) { break; }// throw new Exception("Incomplete data"); }

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
                Console.WriteLine("Too much data in file; current INPOS = {0:x}", br.BaseStream.Position - 1);
            }
            catch (EndOfStreamException) { }

            #region save
            if (isOutFolder)
            {
                string ext = "";
                for (i = 0; i < 4; i++)
                    if (char.IsLetterOrDigit((char)outdata[i]))
                        ext += (char)outdata[i];
                    else
                        break;
                if (ext.Length == 0)
                    ext = "dat";
                ext = "." + ext;
                filein = filein.Replace("\\", "/");
                outflr = outflr.Replace("\\", "/");
                string outfname = filein.Substring(filein.LastIndexOf("/") + 1);
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

                if (!outflr.EndsWith("/"))
                    outflr += "/";
                while (File.Exists(outflr + outfname + ext))
                    outfname += "_";/**/

                BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.Create));
                bw.Write(outdata);

                bw.Flush();
                bw.Close();
            }
            else
            {
                BinaryWriter bw = new BinaryWriter(new FileStream(outflr, FileMode.Create));
                bw.Write(outdata);
                bw.Flush();
                bw.Close();
            }
            #endregion

            Console.WriteLine("LZSS-11 Decompressed " + filein);

            br.Close();
            br.Dispose();
            fstr.Close();
            fstr.Dispose();
        }
        #endregion

    }
}
