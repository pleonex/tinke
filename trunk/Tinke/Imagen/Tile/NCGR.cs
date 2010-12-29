using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Tinke.Imagen.Tile
{
    public static class NCGR
    {
        public static Estructuras.NCGR Leer(string file)
        {
            Estructuras.NCGR ncgr = new Estructuras.NCGR();
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            // Lee cabecera genérica
            ncgr.id = br.ReadChars(4);
            ncgr.id_endian = br.ReadUInt16();
            if (ncgr.id_endian == 0xFFFE)
                ncgr.id.Reverse<char>();
            ncgr.constant = br.ReadUInt16();
            ncgr.size_file = br.ReadUInt32();
            ncgr.size_header = br.ReadUInt16();
            ncgr.nSections = br.ReadUInt16();

            // Lee primera sección CHAR (CHARacter data)
            ncgr.rahc.id = br.ReadChars(4);
            ncgr.rahc.size_section = br.ReadUInt32();
            ncgr.rahc.nTilesY = br.ReadUInt16();
            ncgr.rahc.nTilesX = br.ReadUInt16();
            ncgr.rahc.depth = (br.ReadUInt32() == 0x3 ? Tiles_Form.bpp4 : Tiles_Form.bpp8);
            ncgr.rahc.unknown1 = br.ReadUInt16();
            ncgr.rahc.unknown2 = br.ReadUInt16();
            ncgr.rahc.padding = br.ReadUInt32();
            ncgr.rahc.size_tiledata = (ncgr.rahc.depth == Tiles_Form.bpp8 ? br.ReadUInt32() : br.ReadUInt32() * 2);
            ncgr.rahc.unknown3 = br.ReadUInt32();

            ncgr.rahc.nTiles = (ushort)(ncgr.rahc.size_tiledata / 64);
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                if (ncgr.rahc.depth == Tiles_Form.bpp4)
                    ncgr.rahc.tileData.tiles[i] = Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32));
                else
                    ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            if (ncgr.nSections == 1)   // En caso de que no haya más secciones
            {
                br.Close();
                br.Dispose();
                return ncgr;
            }
            
            // Lee la segunda sección SOPC
            ncgr.sopc.id = br.ReadChars(4);
            ncgr.sopc.size_section = br.ReadUInt32();
            ncgr.sopc.unknown1 = br.ReadUInt32();
            ncgr.sopc.nTilesX = br.ReadUInt16();
            ncgr.sopc.nTilesY = br.ReadUInt16();

            br.Close();
            br.Dispose();
            return ncgr;
        }
        public static Estructuras.NCGR Leer_SinCabecera(string file, long offset)
        {
            Estructuras.NCGR ncgr = new Estructuras.NCGR();
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            // Lee primera sección CHAR (CHARacter data)
            ncgr.rahc.depth = Tiles_Form.bpp8;
            ncgr.rahc.size_tiledata = br.ReadUInt32() ;

            ncgr.rahc.nTiles = (ushort)(ncgr.rahc.size_tiledata);
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
                    ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);

                br.Close();
                br.Dispose();
                return ncgr;
        }

        public static Bitmap Crear_Imagen(Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20;

            Bitmap imagen = new Bitmap(tile.rahc.nTilesX * 8, tile.rahc.nTilesY * 8);


            for (int ht = 0; ht < tile.rahc.nTilesY; ht++)
            {
                for (int wt = 0; wt < tile.rahc.nTilesX; wt++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            try
                            {
                                if (tile.rahc.tileData.tiles[wt + ht * tile.rahc.nTilesX].Length == 0)
                                    goto Fin;
                                imagen.SetPixel(
                                    w + wt * 8,
                                    h + ht * 8,
                                    paleta.pltt.paletas[tile.rahc.tileData.nPaleta[wt + ht * tile.rahc.nTilesX]].colores[
                                        tile.rahc.tileData.tiles[wt + ht * tile.rahc.nTilesX][w + h * 8]
                                        ]);
                            }
                            catch { goto Fin; }
                        }
                    }
                }
            }
            Fin:
            return imagen;

        }
        public static Bitmap Crear_Imagen(Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta, int tilesX, int tilesY)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20; 
            
            Bitmap imagen = new Bitmap(tilesX * 8, tilesY * 8);

            for (int ht = 0; ht < tilesY; ht++)
            {
                for (int wt = 0; wt < tilesX; wt++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            if (tile.rahc.tileData.tiles[wt + ht * tilesX].Length == 0)
                                goto Fin;
                            imagen.SetPixel(
                                w + wt * 8,
                                h + ht * 8,
                                paleta.pltt.paletas[tile.rahc.tileData.nPaleta[wt + ht * tilesX]].colores[
                                    tile.rahc.tileData.tiles[wt + ht * tilesX][w + h * 8]
                                    ]);

                        }
                    }
                }
            }
        Fin:
            return imagen;

        }
        public static Bitmap Crear_Imagen(Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta, int startTile)
        {
            //if (tile.rahc.nTilesX == 0xFFFF || tile.rahc.nTilesY == 0xFFFF && tile.rahc.unknown1 != 0)
            //    return Crear_Imagen(tile, paleta, startTile, tile.rahc.unknown1, tile.rahc.unknown2);
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20; 

            Bitmap imagen = new Bitmap(tile.rahc.nTilesX * 8, tile.rahc.nTilesY * 8);


            for (int ht = 0; ht < tile.rahc.nTilesY; ht++)
            {
                for (int wt = 0; wt < tile.rahc.nTilesX; wt++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            try
                            {
                                if (tile.rahc.tileData.tiles[startTile].Length == 0)
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
        public static Bitmap Crear_Imagen(Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta, int startTile, uint gridX, uint gridY)
        {
            Bitmap imagen = new Bitmap((int)gridX * 4, (int)(tile.rahc.nTiles * 64 / (gridX * 4)));


            for (int ht = 0; ht < (tile.rahc.nTiles / (gridX / 2)); ht++)
            {
                for (int wt = 0; wt < (gridX / 2); wt++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            try
                            {
                                if (tile.rahc.tileData.tiles[startTile].Length == 0)
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
        public static Bitmap Crear_Imagen(Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta, int startTile, int tilesX, int tilesY)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20;

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
