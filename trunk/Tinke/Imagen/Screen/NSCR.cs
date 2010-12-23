using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Tinke.Imagen.Screen
{
    public static class NSCR
    {
        public static Estructuras.NSCR Leer(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Estructuras.NSCR nscr = new Estructuras.NSCR();

            // Lee cabecera genérica
            nscr.id = br.ReadChars(4);
            nscr.endianess = br.ReadUInt16();
            if (nscr.endianess == 0xFFFE)
                nscr.id.Reverse<char>();
            nscr.constant = br.ReadUInt16();
            nscr.file_size = br.ReadUInt32();
            nscr.header_size = br.ReadUInt16();
            nscr.nSection = br.ReadUInt16();

            // Lee primera y única sección:
            nscr.section.id = br.ReadChars(4);
            nscr.section.section_size = br.ReadUInt32();
            nscr.section.width = br.ReadUInt16();
            nscr.section.height = br.ReadUInt16();
            nscr.section.padding = br.ReadUInt32();
            nscr.section.data_size = br.ReadUInt32();
            nscr.section.screenData = new Imagen.NTFS[nscr.section.data_size / 2];

            for (int i = 0; br.BaseStream.Position < nscr.file_size; i++)
            {
                string bits = Tools.Helper.BytesToBits(br.ReadBytes(2));

                nscr.section.screenData[i] = new Imagen.NTFS();
                nscr.section.screenData[i].nPalette = Convert.ToByte(bits.Substring(0, 4));
                nscr.section.screenData[i].yFlip = Convert.ToByte(bits.Substring(4, 1));
                nscr.section.screenData[i].xFlip = Convert.ToByte(bits.Substring(5, 1));
                nscr.section.screenData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Dispose();
            br.Close();
            return nscr;
        }
        public static Estructuras.NSCR Leer_NoCabecera(string file, Int64 offset)
        {
            // Se omite toda la cabecera común más:
            // ID de sección + tamaño de cabecera + tamaño de sección + padding + tamaño de los datos

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;
            Estructuras.NSCR nscr = new Estructuras.NSCR();

            // Datos por defecto
            nscr.id = "NSCR".ToCharArray();
            nscr.endianess = 0xFEFF;
            nscr.constant = 0x0100;
            nscr.header_size = 0x10;
            nscr.nSection = 1;
            nscr.section.id = "NSCR".ToCharArray();
            nscr.section.padding = 0x0;

            nscr.section.width = br.ReadUInt16();
            nscr.section.height = br.ReadUInt16();
            nscr.section.screenData = new Imagen.NTFS[nscr.section.width * nscr.section.height];

            for (int i = 0; i < (nscr.section.height * nscr.section.width); i++)
            {
                string bits = Tools.Helper.BytesToBits(br.ReadBytes(2));

                nscr.section.screenData[i] = new Imagen.NTFS();
                nscr.section.screenData[i].nPalette = Convert.ToByte(bits.Substring(0, 4));
                nscr.section.screenData[i].xFlip = Convert.ToByte(bits.Substring(4, 1));
                nscr.section.screenData[i].yFlip = Convert.ToByte(bits.Substring(5, 1));
                nscr.section.screenData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();
            br.Dispose();

            return nscr;
        }


        public static Byte[][] Modificar_Tile(Estructuras.NSCR nscr, byte[][] tileData)
        {
            List<Byte[]> bytes = new List<byte[]>();
            int j = 0;
            
            for (int i = 0; i < nscr.section.screenData.Length; i++)
            {
                byte[] currTile;
                if (nscr.section.screenData[i].nTile == j)
                {
                    currTile = tileData[j];
                    j++;
                }
                else
                    currTile = tileData[nscr.section.screenData[i].nTile];

                // TODO: no funciona bien los xFlip e yFlip
                if (nscr.section.screenData[i].xFlip == 1)
                    currTile = XFlip(currTile);
                if (nscr.section.screenData[i].yFlip == 1)
                    currTile = YFlip(currTile);

                bytes.Add(currTile);
            }

            return bytes.ToArray();
        }
        public static Byte[] XFlip(Byte[] tile)
        {
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 4; w++)
                {
                    byte color = tile[w + h * 8];
                    tile[w + h * 8] = tile[(7 - w) + h * 8];
                    tile[(7 - w) + h * 8] = color;
                }
            }
            return tile;
        }
        public static Byte[] YFlip(Byte[] tile)
        {
            for (int h = 0; h < 4; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    Byte color = tile[w + h * 8];
                    tile[w + h * 8] = tile[w + (7 - h) * 8];
                    tile[w + (7 - h) * 8] = color;
                }
            }
            return tile;
        }

    }
}