using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tinke.Texto
{
    public static class TXT
    {
        public static string Leer(string file)
        {
            string txt = "";
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            while (br.BaseStream.Position != br.BaseStream.Length - 1)
            {
                byte c = br.ReadByte();

                if (c == 0x0A)
                    txt += '\r';

                txt += Char.ConvertFromUtf32(c);
            }
            br.Close();
            br.Dispose();

            return txt;
        }
    }
}
