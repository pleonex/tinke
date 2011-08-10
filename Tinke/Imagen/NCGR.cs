using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NCGR
    {
        public static NCGR Leer(string file, int id)
        {
            NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;

            BinaryReader br = new BinaryReader(File.OpenRead(file));

            // Read the common header
            ncgr.cabecera.id = br.ReadChars(4);
            ncgr.cabecera.endianess = br.ReadUInt16();
            if (ncgr.cabecera.endianess == 0xFFFE)
                ncgr.cabecera.id.Reverse<char>();
            ncgr.cabecera.constant = br.ReadUInt16();
            ncgr.cabecera.file_size = br.ReadUInt32();
            ncgr.cabecera.header_size = br.ReadUInt16();
            ncgr.cabecera.nSection = br.ReadUInt16();

            // Read the first section: CHAR (CHARacter data)
            ncgr.rahc.id = br.ReadChars(4);
            ncgr.rahc.size_section = br.ReadUInt32();
            ncgr.rahc.nTilesY = br.ReadUInt16();
            ncgr.rahc.nTilesX = br.ReadUInt16();
            ncgr.rahc.depth = (br.ReadUInt32() == 0x3 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            ncgr.rahc.unknown1 = br.ReadUInt16();
            ncgr.rahc.unknown2 = br.ReadUInt16();
            ncgr.rahc.tiledFlag = br.ReadUInt32();
            if ((ncgr.rahc.tiledFlag & 0xFF) == 0x0)
                ncgr.orden = Orden_Tiles.Horizontal;
            else
            {
                ncgr.orden = Orden_Tiles.No_Tiles;
                if (ncgr.rahc.nTilesX != 0xFFFF)
                {
                    ncgr.rahc.nTilesX *= 8;
                    ncgr.rahc.nTilesY *= 8;
                }
            }
            ncgr.rahc.size_tiledata = (ncgr.rahc.depth == ColorDepth.Depth8Bit ? br.ReadUInt32() : br.ReadUInt32() * 2);
            ncgr.rahc.unknown3 = br.ReadUInt32();

            if (ncgr.rahc.size_tiledata != 0)
                ncgr.rahc.nTiles = (ushort)(ncgr.rahc.size_tiledata / 64);
            else
                ncgr.rahc.nTiles = (ushort)(ncgr.rahc.nTilesX * ncgr.rahc.nTilesY);
            if (ncgr.orden == Orden_Tiles.Horizontal)
                ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];
            else
                ncgr.rahc.tileData.tiles = new byte[1][];
            List<byte> noTile = new List<byte>();
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                if (ncgr.orden == Orden_Tiles.Horizontal)
                {
                    if (ncgr.rahc.depth == ColorDepth.Depth4Bit)
                        ncgr.rahc.tileData.tiles[i] = Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32));
                    else
                        ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
                }
                else
                {
                    if (ncgr.rahc.depth == ColorDepth.Depth4Bit)
                        noTile.AddRange(Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32)));
                    else
                        noTile.AddRange(br.ReadBytes(64));
                }
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }
            if (ncgr.orden == Orden_Tiles.No_Tiles)
                ncgr.rahc.tileData.tiles[0] = noTile.ToArray();

            if (ncgr.cabecera.nSection == 1 || br.BaseStream.Position == br.BaseStream.Length)   // If there isn't SOPC section
            {
                br.Close();
                return ncgr;
            }
            
            // Read the second section: SOPC
            ncgr.sopc.id = br.ReadChars(4);
            ncgr.sopc.size_section = br.ReadUInt32();
            ncgr.sopc.unknown1 = br.ReadUInt32();
            ncgr.sopc.charSize = br.ReadUInt16();
            ncgr.sopc.nChar = br.ReadUInt16();

            br.Close();
            return ncgr;
        }
        public static NCGR Leer_Basico(string file, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            uint file_size = (uint)new FileInfo(file).Length;

            // Creamos un archivo NCGR genérico.
            NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;
            ncgr.cabecera.endianess = 0xFEFF;
            ncgr.cabecera.id = "NCGR".ToCharArray();
            ncgr.cabecera.nSection = 1;
            ncgr.cabecera.constant = 0x0100;
            ncgr.cabecera.file_size = file_size;
            // El archivo es NTFT raw, sin ninguna información.
            ncgr.orden = Orden_Tiles.Horizontal;
            ncgr.rahc.nTiles = (ushort)(file_size / 32);
            ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            ncgr.rahc.nTilesX = 0x0020;
            ncgr.rahc.nTilesY = 0x0018;
            ncgr.rahc.tiledFlag = 0x00000000;
            ncgr.rahc.size_section = file_size;
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                ncgr.rahc.tileData.tiles[i] = Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32));
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            br.Close();
            return ncgr;
        }

        public static void Write(NCGR tile, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Common header
            bw.Write(tile.cabecera.id);
            bw.Write(tile.cabecera.endianess);
            bw.Write(tile.cabecera.constant);
            bw.Write(tile.cabecera.file_size);
            bw.Write(tile.cabecera.header_size);
            bw.Write(tile.cabecera.nSection);
            // RAHC section
            bw.Write(tile.rahc.id);
            bw.Write(tile.rahc.size_section);
            bw.Write(tile.rahc.nTilesY);
            bw.Write(tile.rahc.nTilesX);
            bw.Write((uint)(tile.rahc.depth == ColorDepth.Depth4Bit ? 0x03 : 0x04));
            bw.Write(tile.rahc.unknown1);
            bw.Write(tile.rahc.unknown2);
            bw.Write(tile.rahc.tiledFlag);
            bw.Write((uint)(tile.rahc.depth == ColorDepth.Depth4Bit ? tile.rahc.size_tiledata * 2 : tile.rahc.size_tiledata));
            bw.Write(tile.rahc.unknown3);
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
                if (tile.rahc.depth == ColorDepth.Depth4Bit)
                    bw.Write(Convertir.Bit4ToBit8(tile.rahc.tileData.tiles[i]));
                else
                    bw.Write(tile.rahc.tileData.tiles[i]);
            // SOPC section
            if (tile.cabecera.nSection == 2)
            {
                bw.Write(tile.sopc.id);
                bw.Write(tile.sopc.size_section);
                bw.Write(tile.sopc.unknown1);
                bw.Write(tile.sopc.charSize);
                bw.Write(tile.sopc.nChar);
            }

            bw.Flush();
            bw.Close();
        }
        public static NCGR BitmapToTile(string bitmap, Orden_Tiles tileOrder)
        {
            NCGR tile = new NCGR();
            BinaryReader br = new BinaryReader(File.OpenRead(bitmap));
            if (new String(br.ReadChars(2)) != "BM")
                throw new NotSupportedException("Archivo no soportado, no es BITMAP");

            tile.cabecera.id = "RGCN".ToCharArray();
            tile.cabecera.endianess = 0xFEFF;
            tile.cabecera.constant = 0x0001;
            tile.cabecera.header_size = 0x10;
            tile.cabecera.nSection = 0x01;

            br.BaseStream.Position = 0x0A;
            uint offsetImagen = br.ReadUInt32();

            br.BaseStream.Position += 0x04;
            uint ancho = br.ReadUInt32();
            uint alto = br.ReadUInt32();
            tile.rahc.nTilesX = (ushort)(ancho);
            tile.rahc.nTilesY = (ushort)(alto);
            if (tileOrder == Orden_Tiles.Horizontal)
            {
                tile.rahc.nTilesX /= 8;
                tile.rahc.nTilesY /= 8;
            }
            tile.rahc.nTiles = (ushort)(tile.rahc.nTilesX * tile.rahc.nTilesY);

            br.BaseStream.Position += 0x02;
            uint bpp = br.ReadUInt16();
            if (bpp == 0x04)
                tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            else if (bpp == 0x08)
                tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            else
                throw new NotSupportedException("Archivo BMP no soportado, profundidad " + bpp.ToString());

            uint compresion = br.ReadUInt32();
            uint tamañoImagen = br.ReadUInt32();

            tile.rahc.tileData.tiles = new byte[1][];
            switch (tile.rahc.depth)
            {
                case System.Windows.Forms.ColorDepth.Depth4Bit:
                    #region 4 BPP
                    tile.rahc.tileData.tiles[0] = new byte[ancho * alto * 2];
                    tile.rahc.tileData.nPaleta = new byte[ancho * alto * 2];

                    int divisor = (int)ancho / 2;
                    if (ancho % 4 != 0)
                    {
                        int res;
                        Math.DivRem((int)ancho / 2, 4, out res);
                        divisor = (int)ancho / 2 + (4 - res);
                    }
                    br.BaseStream.Position = offsetImagen;

                    for (int h = (int)alto - 1; h >= 0; h--)
                    {
                        for (int w = 0; w < ancho; w += 2)
                        {
                            string hex = String.Format("{0:X}", br.ReadByte());
                            if (hex.Length == 1)
                                hex = '0' + hex;

                            tile.rahc.tileData.tiles[0][w + h * ancho] = Convert.ToByte(hex[0].ToString(), 16);
                            tile.rahc.tileData.nPaleta[w + h * ancho] = 0;
                            if (w + 1 == (int)ancho)
                                continue;
                            tile.rahc.tileData.tiles[0][w + 1 + h * ancho] = Convert.ToByte(hex[1].ToString(), 16);
                            tile.rahc.tileData.nPaleta[w + 1 + h * ancho] = 0;
                        }
                        br.ReadBytes((int)(divisor - ((float)ancho / 2)));
                    }
                    #endregion
                    break;
                case System.Windows.Forms.ColorDepth.Depth8Bit:
                    #region 8 BPP
                    tile.rahc.tileData.tiles[0] = new byte[ancho * alto];
                    tile.rahc.tileData.nPaleta = new byte[ancho * alto];

                    divisor = (int)ancho;
                    if (ancho % 4 != 0)
                    {
                        int res;
                        Math.DivRem((int)ancho, 4, out res);
                        divisor = (int)ancho + (4 - res);
                    }
                    br.BaseStream.Position = offsetImagen;
                    for (int h = (int)alto - 1; h >= 0; h--)
                    {
                        for (int w = 0; w < ancho; w++)
                        {
                            tile.rahc.tileData.tiles[0][w + h * ancho] = br.ReadByte();
                            tile.rahc.tileData.nPaleta[w + h * ancho] = 0;
                        }
                        br.ReadBytes(divisor - (int)ancho);
                    }
                    #endregion
                    break;
            }
            if (tileOrder == Orden_Tiles.Horizontal)
                tile.rahc.tileData.tiles = Convertir.BytesToTiles_NoChanged(tile.rahc.tileData.tiles[0], 
                                           tile.rahc.nTilesX, tile.rahc.nTilesY);

            tile.rahc.id = "RAHC".ToCharArray();
            tile.rahc.size_tiledata = (uint)tile.rahc.tileData.nPaleta.Length;
            tile.rahc.tiledFlag = (uint)(tileOrder == Orden_Tiles.No_Tiles ? 0x01 : 0x00);
            tile.rahc.unknown1 = 0x00;
            tile.rahc.unknown2 = 0x00;
            tile.rahc.unknown3 = 0x0018;
            tile.rahc.size_section = tile.rahc.size_tiledata + 0x20;
            tile.cabecera.file_size = tile.rahc.size_section + tile.cabecera.header_size;
            tile.orden = tileOrder;

            return tile;
        }


        public static Bitmap Crear_Imagen(NCGR tile, NCLR paleta)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
            {
                if (tile.orden == Orden_Tiles.No_Tiles)
                    tile.rahc.nTilesX = 0x40;
                else
                    tile.rahc.nTilesX = 0x08;
            }
            if (tile.rahc.nTilesY == 0xFFFF)
            {
                if (tile.orden == Orden_Tiles.No_Tiles)
                    if (tile.rahc.nTiles >= 0x40) // Por si es menor, para que el tamaño no sea 0
                        tile.rahc.nTilesY = (ushort)((tile.rahc.nTiles / 0x40) * 0x40);
                    else
                        tile.rahc.nTilesY = 0x40;
                else
                    if (tile.rahc.nTiles >= 0x40)
                        tile.rahc.nTilesY = (ushort)(tile.rahc.nTiles / 0x08);
                    else
                        tile.rahc.nTilesY = 0x08;
            }

            switch (tile.orden)
            {
                case Orden_Tiles.No_Tiles:
                    return No_Tile(tile, paleta, 0, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Horizontal:
                    return Horizontal(tile, paleta, 0, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Vertical:
                    throw new NotImplementedException();
                default:
                    return new Bitmap(0, 0);
            }
        }
        public static Bitmap Crear_Imagen(NCGR tile, NCLR paleta, int startTile)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
            {
                if (tile.orden == Orden_Tiles.No_Tiles)
                        tile.rahc.nTilesX = 0x40;
                else
                    tile.rahc.nTilesX = 0x08;
            }
            if (tile.rahc.nTilesY == 0xFFFF)
            {
                if (tile.orden == Orden_Tiles.No_Tiles)
                    if (tile.rahc.nTiles >= 0x40) // Por si es menor, para que el tamaño no sea 0
                        tile.rahc.nTilesY = (ushort)((tile.rahc.nTiles / 0x40) * 0x40);
                    else
                        tile.rahc.nTilesY = 0x40;
                else
                    if (tile.rahc.nTiles >= 0x40)
                        tile.rahc.nTilesY = (ushort)(tile.rahc.nTiles / 0x08);
                    else
                        tile.rahc.nTilesY = 0x08;
            }

            switch (tile.orden)
            {
                case Orden_Tiles.No_Tiles:
                    return No_Tile(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Horizontal:
                    return Horizontal(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Vertical:
                    throw new NotImplementedException();
                default:
                    return new Bitmap(1, 1);
            }
        }
        public static Bitmap Crear_Imagen(NCGR tile, NCLR paleta, int startTile, int tilesX, int tilesY)
        {
            switch (tile.orden)
            {
                case Orden_Tiles.No_Tiles:
                    return No_Tile(tile, paleta, startTile, tilesX, tilesY);
                case Orden_Tiles.Horizontal:
                    return Horizontal(tile, paleta, startTile, tilesX, tilesY);
                case Orden_Tiles.Vertical:
                    throw new NotImplementedException();
                default:
                    return new Bitmap(0, 0);
            }
        }


        private static Bitmap No_Tile(NCGR tile, NCLR paleta, int salto, int width, int height)
        {
            Bitmap imagen = new Bitmap(width, height);


                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        try
                        {
                            if (tile.rahc.tileData.tiles[0].Length == 0)
                                goto Fin;
                            imagen.SetPixel(
                                w,
                                h,
                                paleta.pltt.paletas[tile.rahc.tileData.nPaleta[0]].colores[
                                    tile.rahc.tileData.tiles[0][w + h * width + salto]
                                    ]);
                        }
                        catch { goto Fin; }
                    }
                }
            Fin:
                return imagen;

        }
        private static Bitmap Horizontal(NCGR tile, NCLR paleta, int startTile, int tilesX, int tilesY)
        {
            Bitmap imagen = new Bitmap(tilesX * 8, tilesY * 8);

            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            try
                            {
                                if (tile.rahc.tileData.tiles[wt + ht * tilesX].Length == 0)
                                    goto Fin;
                                imagen.SetPixel(
                                    w + wt * 8,
                                    h + ht * 8,
                                    paleta.pltt.paletas[tile.rahc.tileData.nPaleta[startTile]].colores[
                                        tile.rahc.tileData.tiles[startTile][w + h * 8]
                                        ]);
                            }
                            catch { goto Fin; }

                        }
                    }
                    startTile++;
                }
            }
        Fin:
            return imagen;
        }
    }
}
