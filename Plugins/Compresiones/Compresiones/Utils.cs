using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{

    public static class Utils
    {
        #region helper methods
        public static string byte_to_bits(byte b)
        {
            string o = "";
            for (int i = 0; i < 8; i++)
                o = (((b & (1 << i)) >> i) & 1) + o;
            return o;
        }
        public static string uint_to_bits(uint u)
        {
            string o = "";
            for (int i = 3; i > -1; i--)
                o += byte_to_bits((byte)((u & (0xFF << (i * 8))) >> (i * 8)));
            return o;
        }

        public static byte peekByte(BinaryReader br)
        {
            byte b = br.ReadByte();
            br.BaseStream.Position--;
            return b;
        }

        public static string makeSlashes(string path)
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
}
