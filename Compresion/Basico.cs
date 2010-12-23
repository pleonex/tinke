using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class Basico
    {
        static int MAX_OUTSIZE = 0x200000;
        const int N = 4096, F = 18;
        const byte THRESHOLD = 2;
        const int NIL = N;
        static bool showAlways = true;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        #region method: DecompressFolder
        public static void DecompressFolder(string inflr, string outflr)
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
                    Decompress(Utils.makeSlashes(fname), outflr);
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
        public static void Decompress(string filein, string outflr)
        {
            FileStream fstr = File.OpenRead(filein);
            //if (fstr.Length > int.MaxValue)
            //    throw new Exception("Files larger than 2GB cannot be decompressed by this program.");
            BinaryReader br = new BinaryReader(fstr);
            
            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ77_TAG >> 4:
                        if (tag == LZ77_TAG)
                            LZ77.DecompressLZ77(filein, outflr, true);
                        else if (tag == LZSS_TAG)
                            LZSS.Decompress11LZS(filein, outflr, true);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case RLE_TAG >> 4: RLE.DecompressRLE(filein, outflr, true); break;
                    case NONE_TAG >> 4: Decompress2(filein, outflr); break;
                    case HUFF_TAG >> 4: Huffman.DecompressHuffman(filein, outflr, true); break;
                    default: Decompress2(filein, outflr); break;
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
                Console.ReadLine();
            }
        }
        public static void Decompress(string filein, string outflr, bool isFolder)
        {
            FileStream fstr = File.OpenRead(filein);
            //if (fstr.Length > int.MaxValue)
            //    throw new Exception("Files larger than 2GB cannot be decompressed by this program.");
            BinaryReader br = new BinaryReader(fstr);

            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ77_TAG >> 4:
                        if (tag == LZ77_TAG)
                            LZ77.DecompressLZ77(filein, outflr, isFolder);
                        else if (tag == LZSS_TAG)
                            LZSS.Decompress11LZS(filein, outflr, isFolder);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case RLE_TAG >> 4: RLE.DecompressRLE(filein, outflr, isFolder); break;
                    case NONE_TAG >> 4: Decompress2(filein, outflr); break;
                    case HUFF_TAG >> 4: Huffman.DecompressHuffman(filein, outflr, isFolder); break;
                    default: Decompress2(filein, outflr); break;
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
                Console.ReadLine();
            }
        }
        public static void Decompress2(string filein, string outflr)
        {
            if (File.Exists(outflr))
                File.Delete(outflr);

            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Files larger than 2GB cannot be decompressed by this program.");
            BinaryReader br = new BinaryReader(fstr);

            br.BaseStream.Seek(0x4, SeekOrigin.Begin);
            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ77_TAG >> 4:
                        if (tag == LZ77_TAG)
                            LZ77.DecompressLZ77(filein, outflr, false);
                        else if (tag == LZSS_TAG)
                            LZSS.Decompress11LZS(filein, outflr, false);
                        else
                            CopyFile(filein, outflr);
                        break;
                    case RLE_TAG >> 4: RLE.DecompressRLE(filein, outflr, false); break;
                    case NONE_TAG >> 4: None.DecompressNone(filein, outflr); break;
                    case HUFF_TAG >> 4: Huffman.DecompressHuffman(filein, outflr, false); break;
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
            filein = Utils.makeSlashes(filein);
            string outfname = filein.Substring(filein.LastIndexOf("/") + 1);
            if (!outflr.EndsWith("/"))
                outflr += "/";
            outfname = outflr + outfname;
            File.Copy(filein, outfname, true);
            Console.WriteLine("Copied " + filein + " to " + outflr);
        }
        #endregion

    }
}
