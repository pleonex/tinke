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
        public static NCGR Leer(string file)
        {
            NCGR ncgr = new NCGR();
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            // Lee cabecera genérica
            ncgr.cabecera.id = br.ReadChars(4);
            ncgr.cabecera.endianess = br.ReadUInt16();
            if (ncgr.cabecera.endianess == 0xFFFE)
                ncgr.cabecera.id.Reverse<char>();
            ncgr.cabecera.constant = br.ReadUInt16();
            ncgr.cabecera.file_size = br.ReadUInt32();
            ncgr.cabecera.header_size = br.ReadUInt16();
            ncgr.cabecera.nSection = br.ReadUInt16();
            ncgr.orden = Orden_Tiles.Horizontal; // Este formato es siempre tiled

            // Lee primera sección CHAR (CHARacter data)
            ncgr.rahc.id = br.ReadChars(4);
            ncgr.rahc.size_section = br.ReadUInt32();
            ncgr.rahc.nTilesY = br.ReadUInt16();
            ncgr.rahc.nTilesX = br.ReadUInt16();
            ncgr.rahc.depth = (br.ReadUInt32() == 0x3 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            ncgr.rahc.unknown1 = br.ReadUInt16();
            ncgr.rahc.unknown2 = br.ReadUInt16();
            ncgr.rahc.padding = br.ReadUInt32();
            ncgr.rahc.size_tiledata = (ncgr.rahc.depth == ColorDepth.Depth8Bit ? br.ReadUInt32() : br.ReadUInt32() * 2);
            ncgr.rahc.unknown3 = br.ReadUInt32();

            if (ncgr.rahc.size_tiledata != 0)
                ncgr.rahc.nTiles = (ushort)(ncgr.rahc.size_tiledata / 64);
            else
                ncgr.rahc.nTiles = (ushort)(ncgr.rahc.nTilesX * ncgr.rahc.nTilesY);
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                if (ncgr.rahc.depth == ColorDepth.Depth4Bit)
                    ncgr.rahc.tileData.tiles[i] = Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32));
                else
                    ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            if (ncgr.cabecera.nSection == 1 || br.BaseStream.Position == br.BaseStream.Length)   // En caso de que no haya más secciones
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
        public static NCGR Leer_Basico(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            uint file_size = (uint)new FileInfo(file).Length;

            // Creamos un archivo NCGR genérico.
            NCGR ncgr = new NCGR();
            ncgr.cabecera.endianess = 0xFEFF;
            ncgr.cabecera.id = "NCGR".ToCharArray();
            ncgr.cabecera.nSection = 1;
            ncgr.cabecera.constant = 0x0100;
            ncgr.cabecera.file_size = file_size;
            // El archivo es NTFT raw, sin ninguna información.
            ncgr.orden = Orden_Tiles.Horizontal;
            ncgr.rahc.nTiles = (ushort)(file_size / 64);
            ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            ncgr.rahc.nTilesX = 0x0020;
            ncgr.rahc.nTilesY = 0x0018;
            ncgr.rahc.padding = 0x00000000;
            ncgr.rahc.size_section = file_size;
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            br.Close();
            br.Dispose();
            return ncgr;
        }
        
        public static Bitmap Crear_Imagen(NCGR tile, NCLR paleta)
        {
            if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20;

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
                tile.rahc.nTilesX = 0x20;
            if (tile.rahc.nTilesY == 0xFFFF)
                tile.rahc.nTilesY = 0x20;

            switch (tile.orden)
            {
                case Orden_Tiles.No_Tiles:
                    return No_Tile(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Horizontal:
                    return Horizontal(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY);
                case Orden_Tiles.Vertical:
                    throw new NotImplementedException();
                default:
                    return new Bitmap(0, 0);
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
                            if (tile.rahc.tileData.tiles[w + h * width].Length == 0)
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
