using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class Basico
    {
        const int N = 4096, F = 18;
        const byte THRESHOLD = 2;
        const int NIL = N;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        #region method: DecompressFolder
        public static void DecompressFolder(string inflr, string outflr)
        {
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
        public static void Decompress(string filein, string fileout)
        {
            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Files larger than " + int.MaxValue.ToString() + " cannot be decompressed by this program.");
            BinaryReader br = new BinaryReader(fstr);
            
            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ77_TAG >> 4:
                        if (tag == LZ77_TAG)
                            LZ77.DecompressLZ77(filein, fileout);
                        else if (tag == LZSS_TAG)
                            LZSS.Decompress11LZS(filein, fileout);
                        break;
                    case RLE_TAG >> 4: RLE.DecompressRLE(filein, fileout); break;
                    case HUFF_TAG >> 4: Huffman.DecompressHuffman(filein, fileout); break;
                    default: Decompress2(filein, fileout); break;
                }
            }
            catch (InvalidDataException)
            {
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not properly decompress {0:s};", filein);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        private static void Decompress2(string filein, string fileout)
        {
            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Files larger than " + int.MaxValue.ToString() + " cannot be decompressed by this program.");
            
            BinaryReader br = new BinaryReader(fstr);
            br.BaseStream.Seek(0x4, SeekOrigin.Begin); // A veces la etiqueta está en la posición 0x4
            byte tag = br.ReadByte();
            br.Close();
            try
            {
                switch (tag >> 4)
                {
                    case LZ77_TAG >> 4:
                        if (tag == LZ77_TAG)
                            LZ77.DecompressLZ77(filein, fileout);
                        else if (tag == LZSS_TAG)
                            LZSS.Decompress11LZS(filein, fileout);
                        break;
                    case RLE_TAG >> 4: RLE.DecompressRLE(filein, fileout); break;
                    case NONE_TAG >> 4: None.DecompressNone(filein, fileout); break;
                    case HUFF_TAG >> 4: Huffman.DecompressHuffman(filein, fileout); break;
                }
            }
            catch (InvalidDataException)
            {
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not properly decompress {0:s};", filein);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        #endregion
    }
}
