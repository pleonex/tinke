using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace Compresion
{
    public static class Basico
    {
        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        public static void Decompress(string filein, string fileout)
        {
            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception(ObtenerTraduccion("Compression").Element("S00").Value);
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
                Console.WriteLine(ObtenerTraduccion("Compression").Element("S15").Value, filein);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        private static void Decompress2(string filein, string fileout)
        {
            FileStream fstr = File.OpenRead(filein);
            if (fstr.Length > int.MaxValue)
                throw new Exception(ObtenerTraduccion("Compression").Element("S00").Value);
            
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
                Console.WriteLine(ObtenerTraduccion("Compression").Element("S15").Value, filein);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        internal static XElement ObtenerTraduccion(string arbol)
        {
            XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            string idioma = xml.Element("Options").Element("Language").Value;
            xml = null;

            foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "langs"))
            {
                if (!langFile.EndsWith(".xml"))
                    continue;

                xml = XElement.Load(langFile);
                if (xml.Attribute("name").Value == idioma)
                    break;
            }

            return xml.Element(arbol);
        }

    }
}
