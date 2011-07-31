using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NSCR
    {
        public static NSCR Leer(string file, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NSCR nscr = new NSCR();
            nscr.id = (uint)id;

            // Lee cabecera genérica
            nscr.cabecera.id = br.ReadChars(4);
            nscr.cabecera.endianess = br.ReadUInt16();
            if (nscr.cabecera.endianess == 0xFFFE)
                nscr.cabecera.id.Reverse<char>();
            nscr.cabecera.constant = br.ReadUInt16();
            nscr.cabecera.file_size = br.ReadUInt32();
            nscr.cabecera.header_size = br.ReadUInt16();
            nscr.cabecera.nSection = br.ReadUInt16();

            // Lee primera y única sección:
            nscr.section.id = br.ReadChars(4);
            nscr.section.section_size = br.ReadUInt32();
            nscr.section.width = br.ReadUInt16();
            nscr.section.height = br.ReadUInt16();
            nscr.section.padding = br.ReadUInt32();
            /*if (nscr.section.padding == 0x01)
            {
                ushort height = nscr.section.height;
                nscr.section.height = nscr.section.width;
                nscr.section.width = height;
            }*/
            nscr.section.data_size = br.ReadUInt32();
            nscr.section.mapData = new NTFS[nscr.section.data_size / 2];

            for (int i = 0; i < (nscr.section.data_size / 2); i++)
            {
                string bits = Tools.Helper.BytesToBits(br.ReadBytes(2));

                nscr.section.mapData[i] = new NTFS();
                nscr.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                nscr.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                nscr.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                nscr.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();
            return nscr;
        }
        public static NSCR Leer_Basico(string archivo, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            uint file_size = (uint)new FileInfo(archivo).Length;

            // Su formato es NTFS raw, sin información, nos la inventamos por tanto
            NSCR nscr = new NSCR();
            nscr.id = (uint)id;

            // Lee cabecera genérica
            nscr.cabecera.id = "NSCR".ToCharArray();
            nscr.cabecera.endianess = 0xFEFF;
            nscr.cabecera.constant = 0x0100;
            nscr.cabecera.file_size = file_size;
            nscr.cabecera.header_size = 0x10;
            nscr.cabecera.nSection = 1;

            // Lee primera y única sección:
            nscr.section.id = "NSCR".ToCharArray();
            nscr.section.section_size = file_size;
            nscr.section.width = 0x0100;
            nscr.section.height = 0x00C0;
            nscr.section.padding = 0x00000000;
            nscr.section.data_size = file_size;
            nscr.section.mapData = new NTFS[file_size / 2];

            for (int i = 0; i < (file_size / 2); i++)
            {
                string bits = Tools.Helper.BytesToBits(br.ReadBytes(2));

                nscr.section.mapData[i] = new NTFS();
                nscr.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                nscr.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                nscr.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                nscr.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();
            return nscr;
        }

        public static NTFT Modificar_Tile(NSCR nscr, NTFT tiles)
        {
            NTFT ntft = new NTFT();
            List<Byte[]> bytes = new List<byte[]>();
            List<Byte> nPltt = new List<Byte>();
            int j = 0;
            
            for (int i = 0; i < nscr.section.mapData.Length; i++)
            {
                byte[] currTile;
                if (nscr.section.mapData[i].nTile == j)
                {
                    if (j == tiles.tiles.Length)
                        throw new Exception(Tools.Helper.ObtenerTraduccion("Messages", "S06"));

                    currTile = tiles.tiles[j];
                    j++;
                }
                else
                {
                    if (nscr.section.mapData[i].nTile >= tiles.tiles.Length)
                        throw new Exception(Tools.Helper.ObtenerTraduccion("Messages", "S06"));

                    currTile = tiles.tiles[nscr.section.mapData[i].nTile];
                }

                if (nscr.section.mapData[i].xFlip == 1)
                    currTile = XFlip(currTile);
                if (nscr.section.mapData[i].yFlip == 1)
                    currTile = YFlip(currTile);
                bytes.Add(currTile);
                nPltt.Add(nscr.section.mapData[i].nPalette);
            }
            ntft.nPaleta = nPltt.ToArray();
            ntft.tiles = bytes.ToArray();
            return ntft;
        }
        public static Byte[] XFlip(Byte[] tile)
        {
            byte[] newTile = new byte[tile.Length];

            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 4; w++)
                {
                    newTile[w + h * 8] = tile[(7 - w) + h * 8];
                    newTile[(7 - w) + h * 8] = tile[w + h * 8];
                }
            }
            return newTile;
        }
        public static Byte[] YFlip(Byte[] tile)
        {
            byte[] newTile = new byte[tile.Length];

            for (int h = 0; h < 4; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    newTile[w + h * 8] = tile[w + (7 - h) * 8];
                    newTile[w + (7 - h) * 8] = tile[w + h * 8];
                }
            }
            return newTile;
        }

    }
}