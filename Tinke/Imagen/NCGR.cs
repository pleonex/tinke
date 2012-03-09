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
        //public static NCGR BitmapToTile(string bitmap, TileOrder tileOrder)
        //{
        //    NCGR tile = new NCGR();
        //    BinaryReader br = new BinaryReader(File.OpenRead(bitmap));
        //    if (new String(br.ReadChars(2)) != "BM")
        //        throw new NotSupportedException(Tools.Helper.GetTranslation("NCGR", "S23"));

        //    tile.header.id = "RGCN".ToCharArray();
        //    tile.header.endianess = 0xFEFF;
        //    tile.header.constant = 0x0001;
        //    tile.header.header_size = 0x10;
        //    tile.header.nSection = 0x01;

        //    br.BaseStream.Position = 0x0A;
        //    uint offsetImagen = br.ReadUInt32();

        //    br.BaseStream.Position += 0x04;
        //    uint ancho = br.ReadUInt32();
        //    uint alto = br.ReadUInt32();
        //    tile.rahc.nTilesX = (ushort)(ancho);
        //    tile.rahc.nTilesY = (ushort)(alto);
        //    if (tileOrder == TileOrder.Horizontal)
        //    {
        //        tile.rahc.nTilesX /= 8;
        //        tile.rahc.nTilesY /= 8;
        //    }
        //    tile.rahc.nTiles = (ushort)(tile.rahc.nTilesX * tile.rahc.nTilesY);

        //    br.BaseStream.Position += 0x02;
        //    uint bpp = br.ReadUInt16();
        //    if (bpp == 0x04)
        //        tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
        //    else if (bpp == 0x08)
        //        tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
        //    else
        //        throw new NotSupportedException(String.Format(Tools.Helper.GetTranslation("NCGR", "S24"), bpp.ToString()));

        //    uint compresion = br.ReadUInt32();
        //    uint tamañoImagen = br.ReadUInt32();

        //    tile.rahc.tileData.tiles = new byte[1][];
        //    switch (tile.rahc.depth)
        //    {
        //        case System.Windows.Forms.ColorDepth.Depth4Bit:
        //            #region 4 BPP
        //            tile.rahc.tileData.tiles[0] = new byte[ancho * alto * 2];
        //            tile.rahc.tileData.nPalette = new byte[ancho * alto * 2];

        //            int divisor = (int)ancho / 2;
        //            if (ancho % 4 != 0)
        //            {
        //                int res;
        //                Math.DivRem((int)ancho / 2, 4, out res);
        //                divisor = (int)ancho / 2 + (4 - res);
        //            }
        //            br.BaseStream.Position = offsetImagen;

        //            for (int h = (int)alto - 1; h >= 0; h--)
        //            {
        //                for (int w = 0; w < ancho; w += 2)
        //                {
        //                    string hex = String.Format("{0:X}", br.ReadByte());
        //                    if (hex.Length == 1)
        //                        hex = '0' + hex;

        //                    tile.rahc.tileData.tiles[0][w + h * ancho] = Convert.ToByte(hex[0].ToString(), 16);
        //                    tile.rahc.tileData.nPalette[w + h * ancho] = 0;
        //                    if (w + 1 == (int)ancho)
        //                        continue;
        //                    tile.rahc.tileData.tiles[0][w + 1 + h * ancho] = Convert.ToByte(hex[1].ToString(), 16);
        //                    tile.rahc.tileData.nPalette[w + 1 + h * ancho] = 0;
        //                }
        //                br.ReadBytes((int)(divisor - ((float)ancho / 2)));
        //            }
        //            #endregion
        //            break;
        //        case System.Windows.Forms.ColorDepth.Depth8Bit:
        //            #region 8 BPP
        //            tile.rahc.tileData.tiles[0] = new byte[ancho * alto];
        //            tile.rahc.tileData.nPalette = new byte[ancho * alto];

        //            divisor = (int)ancho;
        //            if (ancho % 4 != 0)
        //            {
        //                int res;
        //                Math.DivRem((int)ancho, 4, out res);
        //                divisor = (int)ancho + (4 - res);
        //            }
        //            br.BaseStream.Position = offsetImagen;
        //            for (int h = (int)alto - 1; h >= 0; h--)
        //            {
        //                for (int w = 0; w < ancho; w++)
        //                {
        //                    tile.rahc.tileData.tiles[0][w + h * ancho] = br.ReadByte();
        //                    tile.rahc.tileData.nPalette[w + h * ancho] = 0;
        //                }
        //                br.ReadBytes(divisor - (int)ancho);
        //            }
        //            #endregion
        //            break;
        //    }
        //    if (tileOrder == TileOrder.Horizontal)
        //        tile.rahc.tileData.tiles = Convertir.BytesToTiles_NoChanged(tile.rahc.tileData.tiles[0], 
        //                                   tile.rahc.nTilesX, tile.rahc.nTilesY);

        //    tile.rahc.id = "RAHC".ToCharArray();
        //    tile.rahc.size_tiledata = (uint)tile.rahc.tileData.nPalette.Length;
        //    tile.rahc.tiledFlag = (uint)(tileOrder == TileOrder.NoTiled ? 0x01 : 0x00);
        //    tile.rahc.unknown1 = 0x00;
        //    tile.rahc.unknown2 = 0x00;
        //    tile.rahc.unknown3 = 0x0018;
        //    tile.rahc.size_section = tile.rahc.size_tiledata + 0x20;
        //    tile.header.file_size = tile.rahc.size_section + tile.header.header_size;
        //    tile.order = tileOrder;

        //    br.Close();
        //    return tile;
        //}

        public static Byte[][] MergeImage(Byte[][] originalTile, Byte[][] newTiles, int startTile)
        {
            List<Byte[]> data = new List<byte[]>();


            for (int i = 0; i < startTile; i++)
            {
                if (i >= originalTile.Length)
                {
                    Byte[] nullTile = new byte[64];
                    for (int t = 0; t < 64; t++)
                        nullTile[t] = 0;
                    data.Add(nullTile);
                    continue;
                }
                data.Add(originalTile[i]);
            }

            data.AddRange(newTiles);

            for (int i = startTile + newTiles.Length; i < originalTile.Length; i++)
                data.Add(originalTile[i]);

            return data.ToArray();
        }

        //public static Bitmap Get_Image(NCGR tile, NCLR paleta, int zoom = 1)
        //{
        //    if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
        //    {
        //        if (tile.order == TileOrder.NoTiled)
        //            tile.rahc.nTilesX = 0x40;
        //        else
        //            tile.rahc.nTilesX = 0x08;
        //    }
        //    if (tile.rahc.nTilesY == 0xFFFF)
        //    {
        //        if (tile.order == TileOrder.NoTiled)
        //            if (tile.rahc.nTiles >= 0x40) // Por si es menor, para que el tamaño no sea 0
        //                tile.rahc.nTilesY = (ushort)((tile.rahc.nTiles / 0x40) * 0x40);
        //            else
        //                tile.rahc.nTilesY = 0x40;
        //        else
        //            if (tile.rahc.nTiles >= 0x40)
        //                tile.rahc.nTilesY = (ushort)(tile.rahc.nTiles / 0x08);
        //            else
        //                tile.rahc.nTilesY = 0x08;
        //    }

        //    switch (tile.order)
        //    {
        //        case TileOrder.NoTiled:
        //            return No_Tile(tile, paleta, 0, tile.rahc.nTilesX, tile.rahc.nTilesY, zoom);
        //        case TileOrder.Horizontal:
        //            return Horizontal(tile, paleta, 0, tile.rahc.nTilesX, tile.rahc.nTilesY, zoom);
        //        case TileOrder.Vertical:
        //            throw new NotImplementedException();
        //        default:
        //            return new Bitmap(0, 0);
        //    }
        //}
        //public static Bitmap Get_Image(NCGR tile, NCLR paleta, int startTile, int zoom = 1)
        //{
        //    if (tile.rahc.nTilesX == 0xFFFF)        // En caso de que no venga la información hacemos la imagen de 256x256
        //    {
        //        if (tile.order == TileOrder.NoTiled)
        //                tile.rahc.nTilesX = 0x40;
        //        else
        //            tile.rahc.nTilesX = 0x08;
        //    }
        //    if (tile.rahc.nTilesY == 0xFFFF)
        //    {
        //        if (tile.order == TileOrder.NoTiled)
        //            if (tile.rahc.nTiles >= 0x40) // Por si es menor, para que el tamaño no sea 0
        //                tile.rahc.nTilesY = (ushort)((tile.rahc.nTiles / 0x40) * 0x40);
        //            else
        //                tile.rahc.nTilesY = 0x40;
        //        else
        //            if (tile.rahc.nTiles >= 0x40)
        //                tile.rahc.nTilesY = (ushort)(tile.rahc.nTiles / 0x08);
        //            else
        //                tile.rahc.nTilesY = 0x08;
        //    }

        //    switch (tile.order)
        //    {
        //        case TileOrder.NoTiled:
        //            return No_Tile(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY, zoom);
        //        case TileOrder.Horizontal:
        //            return Horizontal(tile, paleta, startTile, tile.rahc.nTilesX, tile.rahc.nTilesY, zoom);
        //        case TileOrder.Vertical:
        //            throw new NotImplementedException();
        //        default:
        //            return new Bitmap(1, 1);
        //    }
        //}
        //public static Bitmap Get_Image(NCGR tile, NCLR paleta, int startTile, int tilesX, int tilesY, int zoom = 1)
        //{
        //    switch (tile.order)
        //    {
        //        case TileOrder.NoTiled:
        //            return No_Tile(tile, paleta, startTile, tilesX, tilesY, zoom);
        //        case TileOrder.Horizontal:
        //            return Horizontal(tile, paleta, startTile, tilesX, tilesY, zoom);
        //        case TileOrder.Vertical:
        //            throw new NotImplementedException();
        //        default:
        //            return new Bitmap(0, 0);
        //    }
        //}

        //private static Bitmap No_Tile(NTFT tile, Color[][] palette, int salto, int width, int height, int zoom = 1)
        //{
        //    if (zoom <= 0)
        //        zoom = 1;

        //    Bitmap image = new Bitmap(width * zoom, height * zoom);


        //    for (int h = 0; h < height; h++)
        //    {
        //        for (int w = 0; w < width; w++)
        //        {
        //            for (int hzoom = 0; hzoom < zoom; hzoom++)
        //            {
        //                for (int wzoom = 0; wzoom < zoom; wzoom++)
        //                {
        //                    try
        //                    {
        //                        if (tile.tiles[0].Length == 0)
        //                            goto Fin;

        //                        int num_pal;
        //                        if (tile.nPalette.Length > (w + h * width + salto))
        //                            num_pal = tile.nPalette[w + h * width + salto];
        //                        else
        //                            num_pal = 0;

        //                        if (num_pal >= palette.Length)
        //                            num_pal = 0;

        //                        Color color;
        //                        if (palette[num_pal].Length <= tile.tiles[0][w + h * width + salto])
        //                            color = Color.Transparent; // Debug
        //                            //goto fin;
        //                        else
        //                            color = palette[num_pal][tile.tiles[0][w + h * width + salto]];

        //                        image.SetPixel(
        //                            w * zoom + wzoom,
        //                            h * zoom + hzoom,
        //                            color);
        //                    }
        //                    catch { goto Fin; }
        //                }
        //            }
        //        }
        //    }
        //Fin:
        //    return image;

        //}
        //private static Bitmap No_Tile(NCGR tile, NCLR palette, int salto, int width, int height, int zoom = 1)
        //{
        //    if (zoom <= 0)
        //        zoom = 1;

        //    Bitmap image = new Bitmap(width * zoom, height * zoom);


        //        for (int h = 0; h < height; h++)
        //        {
        //            for (int w = 0; w < width; w++)
        //            {
        //                for (int hzoom = 0; hzoom < zoom; hzoom++)
        //                {
        //                    for (int wzoom = 0; wzoom < zoom; wzoom++)
        //                    {
        //                        try
        //                        {
        //                            if (tile.rahc.tileData.tiles[0].Length == 0)
        //                                goto Fin;
        //                            image.SetPixel(
        //                                w * zoom + wzoom,
        //                                h * zoom + hzoom,
        //                                palette.pltt.palettes[tile.rahc.tileData.nPalette[0]].colors[
        //                                    tile.rahc.tileData.tiles[0][w + h * width + salto]
        //                                    ]);
        //                        }
        //                        catch { goto Fin; }
        //                    }
        //                }
        //            }
        //        }
        //    Fin:
        //        return image;

        //}
        //private static Bitmap Horizontal(NCGR tile, NCLR paleta, int startTile, int tilesX, int tilesY, int zoom = 1)
        //{
        //    if (zoom <= 0)
        //        zoom = 1;
        //    Bitmap imagen = new Bitmap((tilesX * 8) * zoom, (tilesY * 8) * zoom);

        //    tile.rahc.tileData.tiles = Convertir.BytesToTiles(Convertir.TilesToBytes(tile.rahc.tileData.tiles, startTile));
        //    startTile = 0;

        //    for (int ht = 0; ht < tilesY; ht++)
        //    {
        //        for (int wt = 0; wt < tilesX; wt++)
        //        {
        //            for (int h = 0; h < 8; h++)
        //            {
        //                for (int w = 0; w < 8; w++)
        //                {
        //                    for (int hzoom = 0; hzoom < zoom; hzoom++)
        //                    {
        //                        for (int wzoom = 0; wzoom < zoom; wzoom++)
        //                        {
        //                            try
        //                            {
        //                                if (tile.rahc.tileData.tiles[wt + ht * tilesX].Length == 0)
        //                                    goto Fin;
        //                                imagen.SetPixel(
        //                                    (w + wt * 8) * zoom + wzoom,
        //                                    (h + ht * 8) * zoom + hzoom,
        //                                    paleta.pltt.palettes[tile.rahc.tileData.nPalette[startTile]].colors[
        //                                        tile.rahc.tileData.tiles[startTile][w + h * 8]
        //                                        ]);
        //                            }
        //                            catch { goto Fin; }
        //                        }
        //                    }
        //                }
        //            }
        //            startTile++;
        //        }
        //    }
        //Fin:
        //    return imagen;
        //}
        //private static Bitmap Horizontal(NTFT tile, Color[][] palette, int startTile, int tilesX, int tilesY, int zoom = 1)
        //{
        //    if (zoom <= 0)
        //        zoom = 1;
        //    Bitmap image = new Bitmap((tilesX * 8) * zoom, (tilesY * 8) * zoom);

        //    tile.tiles = Convertir.BytesToTiles(Convertir.TilesToBytes(tile.tiles, startTile));
        //    startTile = 0;

        //    for (int ht = 0; ht < tilesY; ht++)
        //    {
        //        for (int wt = 0; wt < tilesX; wt++)
        //        {
        //            for (int h = 0; h < 8; h++)
        //            {
        //                for (int w = 0; w < 8; w++)
        //                {
        //                    for (int hzoom = 0; hzoom < zoom; hzoom++)
        //                    {
        //                        for (int wzoom = 0; wzoom < zoom; wzoom++)
        //                        {
        //                            try
        //                            {
        //                                if (tile.tiles[wt + ht * tilesX].Length == 0)
        //                                    goto Fin;
        //                                image.SetPixel(
        //                                    (w + wt * 8) * zoom + wzoom,
        //                                    (h + ht * 8) * zoom + hzoom,
        //                                    palette[tile.nPalette[startTile]][
        //                                        tile.tiles[startTile][w + h * 8]
        //                                        ]);
        //                            }
        //                            catch { goto Fin; }
        //                        }
        //                    }
        //                }
        //            }
        //            startTile++;
        //        }
        //    }
        //Fin:
        //    return image;
        //}

    }
}
