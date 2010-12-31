using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class None
    {
        const int N = 4096, F = 18;
        const byte THRESHOLD = 2;
        const int NIL = N;

        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        #region None
        public static void DecompressNone(string filein, string outflr)
        {
            FileStream fstr = new FileStream(filein, FileMode.Open);
            if (fstr.Length > int.MaxValue)
                throw new Exception("Filer larger than 2GB cannot be NONE-compressed files.");
            BinaryReader br = new BinaryReader(fstr);

            long decomp_size = 0;
            int i;

            if (br.ReadByte() != NONE_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != NONE_TAG)
                    throw new InvalidDataException(String.Format("File {0:s} is not a valid NONE file, it does not have the NONE-tag as first byte", filein));
            }
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

    }
}
