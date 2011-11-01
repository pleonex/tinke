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
            nscr.header.id = br.ReadChars(4);
            nscr.header.endianess = br.ReadUInt16();
            if (nscr.header.endianess == 0xFFFE)
                nscr.header.id.Reverse<char>();
            nscr.header.constant = br.ReadUInt16();
            nscr.header.file_size = br.ReadUInt32();
            nscr.header.header_size = br.ReadUInt16();
            nscr.header.nSection = br.ReadUInt16();

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
            nscr.header.id = "UNKN".ToCharArray();
            nscr.header.endianess = 0xFEFF;
            nscr.header.constant = 0x0100;
            nscr.header.file_size = file_size;
            nscr.header.header_size = 0x10;
            nscr.header.nSection = 1;

            // Lee primera y única sección:
            nscr.section.id = "UNKN".ToCharArray();
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
        
        public static NSCR Create_BasicMap(int nTiles, int width, int height, int startTile = 0, byte palette = 0)
        {
            NSCR map = new NSCR();

            // Common header
            map.header.id = "RCSN".ToCharArray();
            map.header.endianess = 0xFEFF;
            map.header.constant = 0x0100;
            map.header.header_size = 0x10;
            map.header.nSection = 1;

            // Lee primera y única sección:
            map.section.id = "NRCS".ToCharArray();
            map.section.width = (ushort)width;
            map.section.height = (ushort)height;
            map.section.padding = 0x00000000;
            map.section.data_size = (uint)nTiles * 2;
            map.section.mapData = new NTFS[nTiles];
            for (int i = 0; i < nTiles; i++)
            {
                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nPalette = palette;
                map.section.mapData[i].yFlip = 0;
                map.section.mapData[i].xFlip = 0;
                map.section.mapData[i].nTile = (ushort)(i + startTile);
            }
            map.section.section_size = map.section.data_size + 0x14;
            map.header.file_size = map.section.section_size + map.header.header_size;

            return map;
        }
        public static NSCR Create_BasicMap(int width, int height, int startFillTile, int fillTile, int startTile = 0, byte palette = 0)
        {
            NSCR map = new NSCR();
            int nTiles = width * height / 64;

            // Common header
            map.header.id = "RCSN".ToCharArray();
            map.header.endianess = 0xFEFF;
            map.header.constant = 0x0100;
            map.header.header_size = 0x10;
            map.header.nSection = 1;

            // Lee primera y única sección:
            map.section.id = "NRCS".ToCharArray();
            map.section.width = (ushort)width;
            map.section.height = (ushort)height;
            map.section.padding = 0x00000000;
            map.section.data_size = (uint)nTiles * 2;
            map.section.mapData = new NTFS[nTiles];
            for (int i = 0; i < nTiles; i++)
            {
                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nPalette = palette;
                map.section.mapData[i].yFlip = 0;
                map.section.mapData[i].xFlip = 0;
                if (i >= startFillTile)
                    map.section.mapData[i].nTile = (ushort)fillTile;
                else
                    map.section.mapData[i].nTile = (ushort)(i + startTile);
            }
            map.section.section_size = map.section.data_size + 0x14;
            map.header.file_size = map.section.section_size + map.header.header_size;

            return map;
        }
        public static void Write(NSCR map, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Common header
            bw.Write(map.header.id);
            bw.Write(map.header.endianess);
            bw.Write(map.header.constant);
            bw.Write(map.header.file_size);
            bw.Write(map.header.header_size);
            bw.Write(map.header.nSection);
            // SCRN section
            bw.Write(map.section.id);
            bw.Write(map.section.section_size);
            bw.Write(map.section.width);
            bw.Write(map.section.height);
            bw.Write(map.section.padding);
            bw.Write(map.section.data_size);
            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                int npalette = map.section.mapData[i].nPalette << 12;
                int yFlip = map.section.mapData[i].yFlip << 11;
                int xFlip = map.section.mapData[i].xFlip << 10;
                int data = npalette + yFlip + xFlip + map.section.mapData[i].nTile;
                bw.Write((ushort)data);
            }

            bw.Flush();
            bw.Close();
        }

        public static NTFT Modificar_Tile(NSCR nscr, NTFT tiles, int startInfo = 0)
        {
            NTFT ntft = new NTFT();
            List<Byte[]> bytes = new List<byte[]>();
            List<Byte> nPltt = new List<Byte>();
            
            for (int i = startInfo; i < nscr.section.mapData.Length; i++)
            {
                Byte[] currTile;

                if (nscr.section.mapData[i].nTile >= tiles.tiles.Length)
                {
                    nscr.section.mapData[i].nTile = 00;
                    //throw new Exception(Tools.Helper.ObtenerTraduccion("Messages", "S06"));
                }

                currTile = tiles.tiles[nscr.section.mapData[i].nTile];

                if (nscr.section.mapData[i].xFlip == 1)
                    currTile = XFlip(currTile);
                if (nscr.section.mapData[i].yFlip == 1)
                    currTile = YFlip(currTile);
                bytes.Add(currTile);
                nPltt.Add(nscr.section.mapData[i].nPalette);
            }
            ntft.nPalette = nPltt.ToArray();
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