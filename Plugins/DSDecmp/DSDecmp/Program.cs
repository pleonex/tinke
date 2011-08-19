//Copyright (c) 2010 Nick Kraayenbrink
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace DSDecmp
{
    unsafe class Program
    {
        static uint MAX_OUTSIZE = 0xA00000;
        static bool showAlways = true;

        const int LZ10_TAG = 0x10,
                  LZ11_TAG = 0x11,
                  HUFF_TAG = 0x20, // actually 0x28 and 0x24
                  RLE_TAG = 0x30,
                  LZ40_TAG = 0x40,
                  NONE_TAG = 0x00;

        static bool CopyErrors = false;
        static bool AllowHuff = true;
        static bool AllowRLE = true;
        static bool AllowNone = true;
        static bool AllowLZ10 = true;
        static bool AllowLZ11 = true;
        static bool AllowLZ40 = true;
        static bool AllowOVL = true;
        static bool ForceOVL = false;

        static void Main(string[] args)
        {

            if (args.Length == 0) { Usage(); return; }
            if (args[0] == "-ce")
            {
                CopyErrors = true;
                string[] newArgs = new string[args.Length - 1];
                Array.Copy(args, 1, newArgs, 0, newArgs.Length);
                args = newArgs;
            }
            if (args.Length == 0) { Usage(); return; }
            if (args[0].StartsWith("-n"))
            {
                string rest = args[0].Substring(2);
                AllowHuff = !rest.Contains("h");
                AllowLZ10 = !rest.Contains("0");
                AllowLZ11 = !rest.Contains("1");
                AllowLZ40 = !rest.Contains("4");
                AllowNone = !rest.Contains("n");
                AllowRLE = !rest.Contains("r");
                AllowOVL = !rest.Contains("o");
                string[] newArgs = new string[args.Length - 1];
                Array.Copy(args, 1, newArgs, 0, newArgs.Length);
                args = newArgs;
            }
            else if (args[0] == "-ovl")
            {
                ForceOVL = true;
                string[] newArgs = new string[args.Length - 1];
                Array.Copy(args, 1, newArgs, 0, newArgs.Length);
                args = newArgs;
            }

            if (args.Length == 1)
            {
                if (Directory.Exists(args[0])) // only directory given? output to same directory
                    args = new string[] { args[0], args[0] };
                else if (File.Exists(args[0])) // only file given? output to same dir as file
                    args = new string[] { args[0], Directory.GetParent(args[0]).FullName };
            }

            if (args.Length != 2 && args.Length != 3)
            {
                Usage();
                return;
            }
            if (args.Length == 3)
                MAX_OUTSIZE = uint.Parse(args[2], System.Globalization.NumberStyles.HexNumber);/**/


            args[0] = makeSlashes(args[0]);
            args[1] = makeSlashes(args[1]);
            /**/

            if (!Directory.Exists(args[1]))
                Directory.CreateDirectory(args[1]);
            if (File.Exists(args[0]))/**/
                Decompress(args[0], args[1]);
            else
                DecompressFolder(args[0], args[1]);/**/
        }

        private static void Usage()
        {
            Console.WriteLine("useage: DSDecmp (-ce) (-n[h014nro] | -ovl) infile [outfolder [maxlen]]");
            Console.WriteLine("or: DSDecmp (-ce) (-n[h014nro] | -ovl) infolder [outfolder [maxlen]]");
            Console.WriteLine("maxlen is optional and hexadecimal, and all files that would be larger than maxlen when decompressed are ignored");
            Console.WriteLine("Adding the -ce flag will copy every file that generates an error while processing to the output dir, and does not wait for user confirmation.");
            Console.WriteLine("Adding the -n flag with any number of the characters h,0,1,n, or r will disable compression formats of the corresponding letter;");
            Console.WriteLine("h - Huffman");
            Console.WriteLine("0 - LZ 0x10");
            Console.WriteLine("1 - LZ 0x11");
            Console.WriteLine("4 - LZ 0x40");
            Console.WriteLine("n - None-compression (ie: 0x00 first byte, next 3 bytes file size - 4)");
            Console.WriteLine("r - Run-Length Encoding");
            Console.WriteLine("o - LZ Overlay compression");
            Console.WriteLine();
            Console.WriteLine("Providing the -ovl flag (the -n and -ovl flags cannot appear together) will "
                              + "try to decompress the given file(s) with the DS's overlay compression. Normally, "
                              + "this format is only recognized if the file name is 'arm9.bin' or "
                              + "'overlay_X.bin' (with X any number).");
        }

        private static void WriteDebug(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
            if (showAlways)
                Console.WriteLine(s);
        }

        #region method: DecompressFolder
        private static void DecompressFolder(string inflr, string outflr)
        {
            showAlways = false; // only print errors/failures

            if (!outflr.EndsWith("/") && !outflr.EndsWith("\\"))
                outflr += "/";
            StreamWriter sw = null;
            if (!Directory.Exists(inflr))
            {
                Console.WriteLine("No such file or folder: " + inflr);
                return;
            }
            string[] files = Directory.GetFiles(inflr);
            foreach (string fname in files)
                try
                {
                    Decompress(makeSlashes(fname), outflr);
                }
                catch (Exception e)
                {
                    if (sw == null)
                        sw = new StreamWriter(new FileStream(outflr + "lzsslog.txt", FileMode.Create));
                    Console.WriteLine(e.Message);
                    sw.WriteLine(e.Message);
                    string copied = fname.Replace(inflr, outflr);
                    if (!File.Exists(copied))
                        File.Copy(fname, copied);
                }
            Console.WriteLine("Done decompressing files in folder " + inflr);
            if (sw != null)
            {
                Console.WriteLine("Errors have been logged to " + outflr + "lzsslog.txt");
                sw.Flush();
                sw.Close();
            }
        }
        #endregion

        #region Method: Decompress
        static void Decompress(string filein, string outflr)
        {
            // check if we need to decompress the file using Overlay compression first
            string filename = Path.GetFileName(filein);
            if (AllowOVL)
            {
                if (filename == "arm9.bin"
                    || Regex.Match(filename, "overlay_[0-9]+\\.bin").Success
                    || ForceOVL)
                {
                    try
                    {
                        DecompressLZOverlay(filein, outflr);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not properly decompress {0:s};", filein);
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        if (CopyErrors)
                            CopyFile(filein, outflr);
                        else
                            Console.ReadLine();
                    }
                    return;
                }
            }


            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Files larger than 2GB cannot be decompressed by this program.");
            BinaryReader br = new BinaryReader(fstr);

            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ10_TAG >> 4:
                        if (tag == LZ10_TAG && AllowLZ10)
                            DecompressLZ77(filein, outflr);
                        else if (tag == LZ11_TAG && AllowLZ11)
                            Decompress11LZS(filein, outflr);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case LZ40_TAG >> 4:
                        if (AllowLZ40 && tag == LZ40_TAG) // LZ40 tag must match completely
                            DecompressLZ40(filein, outflr);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case RLE_TAG >> 4:
                        if (AllowRLE && tag == RLE_TAG) // RLE tag must match completely
                            DecompressRLE(filein, outflr);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case NONE_TAG >> 4:
                        if (AllowNone && tag == NONE_TAG)// NONE tag must match completely
                            DecompressNone(filein, outflr);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case HUFF_TAG >> 4:
                        if (AllowHuff) // huff tag only needs t match the first 4 bits
                            // throws InvalidDataException if first 4 bits matched by accident
                            DecompressHuffman(filein, outflr);
                        else
                            CopyFile(filein, outflr);
                        break;
                    default: CopyFile(filein, outflr); break;
                }
            }
            catch (InvalidDataException)
            {
                CopyFile(filein, outflr);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not properly decompress {0:s};", filein);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                if (CopyErrors)
                    CopyFile(filein, outflr);
                else
                    Console.ReadLine();
            }
        }
        #endregion

        #region Method: CopyFile
        /// <summary>
        /// Copies a file
        /// </summary>
        /// <param name="filein">The input file</param>
        /// <param name="outflr">The output folder. (the file keeps its name, other files get overwritten)</param>
        static void CopyFile(string filein, string outflr)
        {
            filein = makeSlashes(filein);
            string outfname = filein.Substring(filein.LastIndexOf("/") + 1);
            if (!outflr.EndsWith("/"))
                outflr += "/";
            outfname = outflr + outfname;
            File.Copy(filein, outfname, true);
            Console.WriteLine("Copied " + filein + " to " + outflr);
        }
        #endregion

        #region RLE
        static void DecompressRLE(string filein, string outflr)
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

            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Files larger than 2GB cannot be RLE-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0, curr_size = 0;
            int i, rl;
            byte flag, b;
            bool compressed;

            if (br.ReadByte() != RLE_TAG)
                throw new InvalidDataException(String.Format("File {0:s} is not a valid RLE file", filein));
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} and will not be decompressed.", filein, MAX_OUTSIZE));

            if (showAlways)
                Console.WriteLine("Decompressing {0:s}. (outsize: 0x{1:x})", filein, decomp_size);

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


            #region save
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
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            /*while (File.Exists(outflr + outfname + ext))
                outfname += "_";/**/

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.Create));
            for (i = 0; i < outdata.Length; i++)
                bw.Write(outdata[i]);
            //bw.Write(outdata);
            bw.Flush();
            bw.Close();

            #endregion

            Console.WriteLine("RLE decompressed " + filein);
        }
        #endregion

        #region Huffman
        static void DecompressHuffman(String filename, String outflr)
        {
            /*
                   Data Header (32bit)
                       Bit0-3   Data size in bit units (normally 4 or 8)
                       Bit4-7   Compressed type (must be 2 for Huffman)
                       Bit8-31  24bit size of decompressed data in bytes
                   Tree Size (8bit)
                       Bit0-7   Size of Tree Table/2-1 (ie. Offset to Compressed Bitstream)
                   Tree Table (list of 8bit nodes, starting with the root node)
                     Root Node and Non-Data-Child Nodes are:
                       Bit0-5   Offset to next child node,
                                Next child node0 is at (CurrentAddr AND NOT 1)+Offset*2+2
                                Next child node1 is at (CurrentAddr AND NOT 1)+Offset*2+2+1
                       Bit6     Node1 End Flag (1=Next child node is data)
                       Bit7     Node0 End Flag (1=Next child node is data)
                     Data nodes are (when End Flag was set in parent node):
                       Bit0-7   Data (upper bits should be zero if Data Size is less than 8)
                   Compressed Bitstream (stored in units of 32bits)
                       Bit0-31  Node Bits (Bit31=First Bit)  (0=Node0, 1=Node1)
            */

            BinaryReader br = new BinaryReader(File.OpenRead(filename));

            byte firstByte = br.ReadByte();

            int dataSize = firstByte & 0x0F;

            if ((firstByte & 0xF0) != HUFF_TAG)
                throw new InvalidDataException(String.Format("Invalid huffman comressed file; invalid tag {0:x}", firstByte));

            //Console.WriteLine("Data size: {0:x}", dataSize);
            if (dataSize != 8 && dataSize != 4)
                throw new InvalidDataException(String.Format("Unhandled dataSize {0:x}", dataSize));

            int decomp_size = 0;
            for (int i = 0; i < 3; i++)
            {
                decomp_size |= br.ReadByte() << (i * 8);
            }
            //Console.WriteLine("Decompressed size: {0:x}", decomp_size);

            byte treeSize = br.ReadByte();
            HuffTreeNode.maxInpos = 4 + (treeSize + 1) * 2;

            //Console.WriteLine("Tree Size: {0:x}", treeSize);

            HuffTreeNode rootNode = new HuffTreeNode();
            rootNode.parseData(br);

            //Console.WriteLine("Tree: {0:s}", rootNode.ToString());

            br.BaseStream.Position = 4 + (treeSize + 1) * 2; // go to start of coded bitstream.
            // read all data
            uint[] indata = new uint[(br.BaseStream.Length - br.BaseStream.Position) / 4];
            for (int i = 0; i < indata.Length; i++)
                indata[i] = br.ReadUInt32();
            
            //Console.WriteLine(indata[0]);
            //Console.WriteLine(uint_to_bits(indata[0]));

            long curr_size = 0;
            decomp_size *= dataSize == 8 ? 1 : 2;
            byte[] outdata = new byte[decomp_size];

            int idx = -1;
            string codestr = "";
            LinkedList<byte> code = new LinkedList<byte>();
            int value;
            while (curr_size < decomp_size)
            {
                try
                {
                    codestr += uint_to_bits(indata[++idx]);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new IndexOutOfRangeException("not enough data.", e);
                }
                while (codestr.Length > 0)
                {
                    code.AddFirst(byte.Parse(codestr[0] + ""));
                    codestr = codestr.Remove(0, 1);
                    if (rootNode.getValue(code.Last, out value))
                    {
                        try
                        {
                            outdata[curr_size++] = (byte)value;
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            if (code.First.Value != 0)
                                throw ex;
                        }
                        code.Clear();
                    }
                }
            }
            if (codestr.Length > 0 || idx < indata.Length-1)
            {
                while (idx < indata.Length-1)
                    codestr += uint_to_bits(indata[++idx]);
                codestr = codestr.Replace("0", "");
                if (codestr.Length > 0)
                    Console.WriteLine("too much data; str={0:s}, idx={1:g}/{2:g}", codestr, idx, indata.Length);
            }

            byte[] realout;
            if (dataSize == 4)
            {
                realout = new byte[decomp_size / 2];
                for (int i = 0; i < decomp_size / 2; i++)
                {
                    if ((outdata[i * 2] & 0xF0) > 0
                        || (outdata[i * 2 + 1] & 0xF0) > 0)
                        throw new Exception("first 4 bits of data should be 0 if dataSize = 4");
                    realout[i] = (byte)((outdata[i * 2] << 4) | outdata[i * 2 + 1]);
                }
            }
            else
            {
                realout = outdata;
            }

            #region save
            string ext = "";
            for (int i = 0; i < 4; i++)
                if (char.IsLetterOrDigit((char)realout[i]))
                    ext += (char)realout[i];
                else
                    break;
            if (ext.Length == 0)
                ext = "dat";
            ext = "." + ext;
            filename = filename.Replace("\\", "/");
            outflr = outflr.Replace("\\", "/");
            string outfname = filename.Substring(filename.LastIndexOf("/") + 1);
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            while (File.Exists(outflr + outfname + ext))
                outfname += "_";

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.CreateNew));
            bw.Write(realout);
            bw.Flush();
            bw.Close();

            #endregion

            Console.WriteLine("Huffman decompressed {0:s}", filename);
            //Console.ReadLine();
            /**/
        }
        #endregion

        #region None
        private static void DecompressNone(string filein, string outflr)
        {
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Filer larger than 2GB cannot be NONE-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0;
            int i;

            if (br.ReadByte() != NONE_TAG)
                throw new InvalidDataException(String.Format("File {0:s} is not a valid NONE file, it does not have the NONE-tag as first byte", filein));
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size != fstr.Length - 0x04)
                throw new InvalidDataException("File {0:s} is not a valid NONE file, the decompression size shold be the file size - 4");

            #region save
            string ext = "";
            char c;
            for (i = 0; i < 4; i++)
                if (char.IsLetterOrDigit(c = (char)br.ReadByte()))
                    ext += c;
                else
                    break;
            if (ext.Length == 0)
                ext = "dat";
            ext = "." + ext;
            br.BaseStream.Position -= i == 4 ? 4 : i + 1;

            filein = filein.Replace("\\", "/");
            outflr = outflr.Replace("\\", "/");
            string outfname = filein.Substring(filein.LastIndexOf("/") + 1);
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            while (File.Exists(outflr + outfname + ext))
                outfname += "_";

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.CreateNew));

            bw.Write(br.ReadBytes((int)decomp_size));

            bw.Flush();
            bw.Close();

            #endregion

            Console.WriteLine("NONE-decompressed {0:s}", filein);
        }
        #endregion

        #region tag 0x10 LZ77
        static void DecompressLZ77(string filein, string outflr)
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
                throw new Exception("Filer larger than 2GB cannot be LZ-0x10-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0, curr_size = 0;
            int flags, i, j, disp, n;
            bool flag;
            byte b;
            long cdest;

            if (br.ReadByte() != LZ10_TAG)
                throw new InvalidDataException(String.Format("File {0:s} is not a valid LZ-0x10 file", filein));
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
                        catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                        n = b >> 4;
                        disp = (b & 0x0F) << 8;
                        try { disp |= br.ReadByte(); }
                        catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
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
                        catch (EndOfStreamException) { break;}// throw new Exception("Incomplete data"); }
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

            #region save
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
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            while (File.Exists(outflr + outfname + ext))
                outfname += "_";

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.CreateNew));
            bw.Write(outdata);
            bw.Flush();
            bw.Close();

            #endregion

            Console.WriteLine("LZ-0x10 Decompressed " + filein);

        }
        #endregion

        #region tag 0x11 LZSS
        static void Decompress11LZS(string filein, string outflr)
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
                        If Indicator is 1: A(B CD E)(F GH)
                            Bit 0-3     (LEN - 0x111) MSBs
                            Bit 4-7     Indicator; unused
                            Bit 8-15    (LEN- 0x111) 'middle'-SBs
                            Bit 16-19   Disp MSBs
                            Bit 20-23   (LEN - 0x111) LSBs
                            Bit 24-31   Disp LSBs
                        If Indicator is 0:
                            Bit 0-3     (LEN - 0x11) MSBs
                            Bit 4-7     Indicator; unused
                            Bit 8-11    Disp MSBs
                            Bit 12-15   (LEN - 0x11) LSBs
                            Bit 16-23   Disp LSBs
             */
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Filer larger than 2GB cannot be LZ-0x11-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            int decomp_size = 0, curr_size = 0;
            int i, j, disp, len;
            bool flag;
            byte b1, bt, b2, b3, flags;
            int cdest;

            int threshold = 1;

            if (br.ReadByte() != LZ11_TAG)
                throw new InvalidDataException(String.Format("File {0:s} is not a valid LZ-0x11 file", filein));
            for (i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed. (1)", filein, MAX_OUTSIZE, decomp_size));
            else if (decomp_size == 0)
                for (i = 0; i < 4; i++)
                    decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE << 8)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed. (2)", filein, MAX_OUTSIZE, decomp_size));

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
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            while (File.Exists(outflr + outfname + ext))
                outfname += "_";/**/

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.Create));
            bw.Write(outdata);

            bw.Flush();
            bw.Close();

            #endregion

            Console.WriteLine("LZ-0x11 Decompressed " + filein);

        }
        #endregion

        #region tag 0x40 LZ
        static void DecompressLZ40(string filein, string outflr)
        {
            // no NDSTEK-like specification for this one; I seem to not be able to get those right.
            /*
             * byte tag; // 0x40
             * byte[3] decompressedSize;
             * the rest is the data;
             * 
             * for each chunk:
             *      - first byte determines which blocks are compressed
             *          - block i is compressed iff:
             *              - the i'th MSB is the last 1-bit in the byte
             *              - OR the i'th MSB is a 0-bit, not directly followed by other 0-bits.
             *          - note that there will never be more than one 0-bit before any 1-bit in this byte
             *          (look at the corresponding code, it may clarify this a bit more)
             *      - then come 8 blocks:
             *          - a non-compressed block is simply one single byte
             *          - a compressed block can have 3 sizes:
             *              - A0 CD EF
             *                  -> Length = EF + 0x10, Disp = CDA
             *              - A1 CD EF GH
             *                  -> Length = GHEF + 0x110, Disp = CDA
             *              - AB CD  (B > 1)
             *                  -> Length = B, Disp = CDA
             *              Copy <Length> bytes from Dest-<Disp> to Dest (with <Dest> similar to the NDSTEK specs)
             */


            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Filer larger than 2GB cannot be LZSS-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            int decomp_size = 0, curr_size = 0;

            if (br.ReadByte() != LZ40_TAG)
                throw new InvalidDataException(String.Format("File {0:s} is not a valid LZSS-11 file", filein));
            for (int i = 0; i < 3; i++)
                decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed. (1)", filein, MAX_OUTSIZE, decomp_size));
            else if (decomp_size == 0)
                for (int i = 0; i < 4; i++)
                    decomp_size += br.ReadByte() << (i * 8);
            if (decomp_size > MAX_OUTSIZE << 8)
                throw new Exception(String.Format("{0:s} will be larger than 0x{1:x} (0x{2:x}) and will not be decompressed. (2)", filein, MAX_OUTSIZE, decomp_size));

            if (showAlways)
                Console.WriteLine("Decompressing {0:s}. (outsize: 0x{1:x})", filein, decomp_size);


            byte[] outdata = new byte[decomp_size];

            while (curr_size < decomp_size)
            {
                int flag;
                try { flag = br.ReadByte(); }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("Not enough data");
                    break;
                }
                int flagB = flag;
                bool[] compFlags = new bool[8];
                bool[] fbits = new bool[11];
                fbits[0] = true;
                fbits[9] = false;
                fbits[10] = false;

                // determine which blocks are compressed
                int b = 0;
                while (flag > 0)
                {
                    bool bit = (flag & 0x80) > 0;
                    flag = (flag & 0x7F) << 1;
                    compFlags[b++] = (flag == 0) || !bit;
                }

                /*
                Console.WriteLine("Flag: 0x{0:X2}", flagB);
                Console.Write("-> (  ");
                for (int i = 0; i < 8; i++)
                    Console.Write(compFlags[i] ? "1," : "0,");
                Console.WriteLine(")");/**/

                for (int i = 0; i < 8 && curr_size < decomp_size; i++)
                {
                    if (compFlags[i])
                    {
                        ushort compressed = br.ReadUInt16();
                        // ABCD (or CD AB if read byte-by-byte)
                        // -> D is length
                        // -> ABC is disp
                        int len = compressed & 0x000F;
                        int disp = compressed >> 4;

                        // if D == 0, actual format is:
                        // CD AB EF
                        // -> DEF is length - 0x10
                        // -> ABC is disp

                        // if D == 1, actual format is:
                        // CD AB EF GH
                        // -> GHEF is length - 0x110
                        // -> ABC is disp
                        if (len == 0)
                            len = br.ReadByte() + 0x10;
                        else if (len == 1)
                            len = br.ReadUInt16() + 0x110;

                        if (disp > curr_size)
                            throw new Exception("Cannot go back more than already written "
                                + "(compressed block=0x" + compressed.ToString("X4") + ")\n"
                                + "INPOS = 0x" + (br.BaseStream.Position - 2).ToString("X4"));

                        for (int j = 0; j < len; j++)
                        {
                            outdata[curr_size + j] = outdata[curr_size - disp + j];
                        }
                        curr_size += len;
                    }
                    else
                    {
                        outdata[curr_size++] = br.ReadByte();
                    }
                }
            }

            try
            {
                byte b;
                while ((b = br.ReadByte()) == 0
                    || b == 0x80) { }
                // if we read a non-zero up to the end of the file, print that there is still some data
                // (0x40 compression seems to add 80 00 00 sometimes, so also ignore 0x80-bytes)
                Console.WriteLine("Too much data in file; current INPOS = {0:x}", br.BaseStream.Position - 1);
            }
            catch (EndOfStreamException) { }

            #region save
            string ext = "";
            for (int i = 0; i < 4; i++)
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
            if (outfname.Contains("."))
                outfname = outfname.Substring(0, outfname.LastIndexOf('.'));

            if (!outflr.EndsWith("/"))
                outflr += "/";
            while (File.Exists(outflr + outfname + ext))
                outfname += "_";/**/

            BinaryWriter bw = new BinaryWriter(new FileStream(outflr + outfname + ext, FileMode.Create));
            bw.Write(outdata);

            bw.Flush();
            bw.Close();
            #endregion

            Console.WriteLine("LZ-0x40-decompressed " + filein);
        }
        #endregion

        #region LZ Overlay
        private static void DecompressLZOverlay(string filein, string outflr)
        {
            // Overlay LZ compression is basically just LZ-0x10 compression.
            // however the order if reading is reversed: the compression starts at the end of the file.
            // Assuming we start reading at the end towards the beginning, the format is:
            /*
             * u32 extraSize; // decompressed data size = file length (including header) + this value
             * u8 headerSize;
             * u24 compressedLength; // can be less than file size (w/o header). If so, the rest of the file is uncompressed.
             * u8[headerSize-8] padding; // 0xFF-s
             * 
             * 0x10-like-compressed data follows (without the usual 4-byte header).
             * The only difference is that 2 should be added to the DISP value in compressed blocks
             * to get the proper value.
             * the u32 and u24 are read most significant byte first.
             * if extraSize is 0, there is no headerSize, decompressedLength or padding.
             * the data starts immediately, and is uncompressed.
             * 
             * arm9.bin has 3 extra u32 values at the 'start' (ie: end of the file),
             * which may be ignored. (and are ignored here) These 12 bytes also should not
             * be included in the computation of the output size.
             */

            // save the input file in a buffer, since we need to read backwards.
            // reverse the array once we're done reading
            byte[] inbuffer = new byte[0];
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filein)))
            {
                if (filein.EndsWith("arm9.bin"))
                {
                    // arm9.bin has 0xC extra bytes we don't need at the end.
                    // Without those the format is the same as with overlay files.
                    inbuffer = new byte[reader.BaseStream.Length - 0xC];
                }
                else
                    inbuffer = new byte[reader.BaseStream.Length];
                reader.Read(inbuffer, 0, inbuffer.Length);
            }
            Array.Reverse(inbuffer);

            // decompress the input. this results in an output buffer that is reversed,
            // so reverse that after decompression as well.
            byte[] outbuffer = new byte[0];
            using (BinaryReader reader = new BinaryReader(new MemoryStream(inbuffer)))
            {
                int extraSize = (reader.ReadByte() << 24)
                                | (reader.ReadByte() << 16)
                                | (reader.ReadByte() << 8)
                                | (reader.ReadByte());

                if (extraSize == 0)
                {
                    outbuffer = new byte[inbuffer.Length - 4];
                    // if the extra size if 0, there is no overlay compression.
                    reader.Read(outbuffer, 0, outbuffer.Length);
                }
                else
                {
                    byte headerLength = reader.ReadByte();
                    int compressedSize = (reader.ReadByte() << 16)
                                        | (reader.ReadByte() << 8)
                                        | reader.ReadByte();
                    // skip the padding
                    reader.BaseStream.Position = headerLength;

                    outbuffer = new byte[inbuffer.Length + extraSize];

                    // decompress the compressed part
                    #region LZ-0x10-like decompression
                    int curr_size = 0;
                    int decomp_size = compressedSize + extraSize;
                    int inpos = 0;
                    byte b;
                    int n, disp, j, cdest;
                    while (inpos < compressedSize && curr_size < decomp_size)
                    {
                        byte flags = reader.ReadByte();
                        for (int i = 0; i < 8; i++)
                        {
                            bool flag = (flags & (0x80 >> i)) > 0;
                            if (flag)
                            {
                                disp = 0;
                                try { b = reader.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                                n = b >> 4;
                                disp = (b & 0x0F) << 8;
                                try { disp |= reader.ReadByte(); }
                                catch (EndOfStreamException) { throw new Exception("Incomplete data"); }
                                n += 3;
                                cdest = curr_size;

                                inpos += 2;

                                disp += 3;

                                if (disp > curr_size)
                                {
                                    //throw new Exception("Cannot go back more than already written");
                                    Console.WriteLine("DISP is too large (0x{0:X}, curr_size=0x{1:X}, length=0x{2:X}); using 1 instead.", disp, curr_size, n);
                                    //disp %= curr_size;
                                    // HACK. this seems to produce valid files, but isn't the most elegant solution.
                                    // although this _could_ be the actual way to use a disp of 2 in this format,
                                    // as otherwise the minimum would be 3 (and 0 is undefined, and 1 is less useful).
                                    disp = 2;
                                }
                                for (j = 0; j < n; j++)
                                    outbuffer[curr_size++] = outbuffer[cdest - disp + j];

                                if (curr_size > decomp_size || inpos >= compressedSize)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                try { b = reader.ReadByte(); }
                                catch (EndOfStreamException) { break; }
                                try { outbuffer[curr_size++] = b; }
                                catch (IndexOutOfRangeException) { if (b == 0) break; }

                                inpos += 1;

                                if (curr_size > decomp_size || inpos >= compressedSize)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    #endregion

                    // if there is any uncompressed part, copy that to the buffer as well
                    int decompressedLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                    //Console.WriteLine("outlen-curr_size:0x{0:X}", outbuffer.Length - curr_size);
                    //Console.WriteLine("reader.len-reader.pos:0x{0:X}", decompressedLength);
                    if (decompressedLength > 0)
                    {
                        reader.Read(outbuffer, outbuffer.Length - decompressedLength, decompressedLength);
                    }
                }
            }
            Array.Reverse(outbuffer);

            // write the output to a file. Replace the .bin extension with .ovl.
            string infname = Path.GetFileName(filein);
            string outfname = infname.Substring(0, infname.Length - 3) + "ovl";
            string outfile = Path.Combine(outflr, outfname);

            using (BinaryWriter writer = new BinaryWriter(File.Create(outfile)))
            {
                writer.Write(outbuffer);
            }

            Console.WriteLine("LZ-Overlay compressed " + filein);
        }
        #endregion

        #region helper methods
        private static string byte_to_bits(byte b)
        {
            string o = "";
            for (int i = 0; i < 8; i++)
                o = (((b & (1 << i)) >> i) & 1) + o;
            return o;
        }
        private static string uint_to_bits(uint u)
        {
            string o = "";
            for (int i = 3; i >-1; i--)
                o += byte_to_bits((byte)((u & (0xFF << (i * 8))) >> (i * 8)));
            return o;
        }

        private static byte peekByte(BinaryReader br)
        {
            byte b = br.ReadByte();
            br.BaseStream.Position--;
            return b;
        }

        private static string makeSlashes(string path)
        {
            StringBuilder sbin = new StringBuilder(path),
                          sbout = new StringBuilder();
            char c;
            while (sbin.Length > 0)
            {
                c = sbin[0];
                sbin.Remove(0, 1);
                if (c == '\\')
                    sbout.Append('/');
                else
                    sbout.Append(c);
            }
            return sbout.ToString();
        }
        #endregion

    }

    class HuffTreeNode
    {
        internal static int maxInpos = 0;
        internal HuffTreeNode node0, node1;
        internal int data = -1; // [-1,0xFF]
        /// <summary>
        /// To get a value, provide the last node of a list of bytes &lt; 2. 
        /// the list will be read from back to front.
        /// </summary>
        internal bool getValue(LinkedListNode<byte> code, out int value)
        {
            value = data;
            if (code == null)
                return node0 == null && node1 == null && data >= 0;

            if(code.Value > 1)
                throw new Exception(String.Format("the list should be a list of bytes < 2. got:{0:g}", code.Value));
            
            byte c = code.Value;
            bool retVal;
            HuffTreeNode n = c == 0 ? node0 : node1;
            retVal = n != null && n.getValue(code.Previous, out value);
            return retVal;
        }

        internal int this[string code]
        {
            get
            {
                LinkedList<byte> c = new LinkedList<byte>();
                foreach (char ch in code)
                    c.AddFirst((byte)ch);
                int val = 1;
                return this.getValue(c.Last, out val) ? val : -1;
            }
        }

        internal void parseData(BinaryReader br)
        {
            /*
             * Tree Table (list of 8bit nodes, starting with the root node)
                     Root Node and Non-Data-Child Nodes are:
                       Bit0-5   Offset to next child node,
                                Next child node0 is at (CurrentAddr AND NOT 1)+Offset*2+2
                                Next child node1 is at (CurrentAddr AND NOT 1)+Offset*2+2+1
                       Bit6     Node1 End Flag (1=Next child node is data)
                       Bit7     Node0 End Flag (1=Next child node is data)
                     Data nodes are (when End Flag was set in parent node):
                       Bit0-7   Data (upper bits should be zero if Data Size is less than 8)
             */
            this.node0 = new HuffTreeNode();
            this.node1 = new HuffTreeNode();
            long currPos = br.BaseStream.Position;
            byte b = br.ReadByte();
            long offset = b & 0x3F;
            bool end0 = (b & 0x80) > 0, end1 = (b & 0x40) > 0;
            // parse data for node0
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end0)
                    node0.data = br.ReadByte();
                else
                    node0.parseData(br);
            }
            // parse data for node1
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2 + 1;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end1)
                    node1.data = br.ReadByte();
                else
                    node1.parseData(br);
            }
            // reset position
            br.BaseStream.Position = currPos;
        }

        public override string ToString()
        {
            if (data < 0)
                return "<" + node0.ToString() + ", " + node1.ToString() + ">";
            else
                return String.Format("[{0:x}]", data);
        }

        internal int Depth
        {
            get
            {
                if (data < 0)
                    return 0;
                else
                    return 1 + Math.Max(node0.Depth, node1.Depth);
            }
        }
    }
}
