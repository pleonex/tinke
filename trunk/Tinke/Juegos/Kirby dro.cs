using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Tinke.Juegos
{
    public static class Kirby_dro
    {
        public static Bitmap Imagen_Bin(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Imagen.Tile.Estructuras.NCGR tile = new Imagen.Tile.Estructuras.NCGR();
            Imagen.Paleta.Estructuras.NCLR paleta = new Imagen.Paleta.Estructuras.NCLR();
            Imagen.Screen.Estructuras.NSCR screen = new Imagen.Screen.Estructuras.NSCR();

            uint header = br.ReadUInt32();

            paleta.pltt.tamaño = br.ReadUInt32();
            tile.rahc.size_tiledata = br.ReadUInt32();
            screen.section.data_size = br.ReadUInt32();

            // Si el tamaño de la cabecera es 0x18 entonces hay información de ancho y largo, sino la imagen es imcompatible por el momento
            if (header == 0x18)
            {
                screen.section.width = (ushort)br.ReadUInt32();
                screen.section.height = (ushort)br.ReadUInt32();
            }
            else
            {
                Console.WriteLine("Archivo .bin no reconocido, cabecera diferente a 0x18");
                throw new Exception("Archivo incompatible");
            }

            tile.rahc.nTilesX = (ushort)(screen.section.width / 8);
            tile.rahc.nTilesY = (ushort)(screen.section.height / 8);
            paleta.pltt.profundidad = (paleta.pltt.tamaño < 512 ? Imagen.Paleta.Depth.bits4 : Imagen.Paleta.Depth.bits8);
            tile.rahc.nTiles = (ushort)(paleta.pltt.profundidad == Imagen.Paleta.Depth.bits4 ? 
                tile.rahc.size_tiledata / 32 : 
                tile.rahc.size_tiledata / 64);

            // Comienzo de la paleta
            paleta.pltt.nColores = 0x10;
            paleta.pltt.paletas = new Imagen.NTFP[(paleta.pltt.tamaño / 2) / paleta.pltt.nColores];
            for (int i = 0; i < paleta.pltt.paletas.Length; i++)
                paleta.pltt.paletas[i] = Imagen.Paleta.NCLR.Paleta_NTFP(ref br, paleta.pltt.nColores);

            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            for (int i = 0; i < tile.rahc.nTiles; i++)
                if (paleta.pltt.profundidad == Imagen.Paleta.Depth.bits4)
                    tile.rahc.tileData.tiles[i] = Tools.Helper.BytesTo4BitsRev(br.ReadBytes(32));
                else
                    tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
            
            screen.section.screenData = new Imagen.NTFS[tile.rahc.nTilesX * tile.rahc.nTilesY];
            for (int i = 0; i < (screen.section.data_size / 2); i++)
            {
                string bits = Tools.Helper.BytesToBits(br.ReadBytes(2));

                screen.section.screenData[i] = new Imagen.NTFS();
                screen.section.screenData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                screen.section.screenData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                screen.section.screenData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                screen.section.screenData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }
            br.Close();
            br.Dispose();

            tile.rahc.tileData = Imagen.Screen.NSCR.Modificar_Tile(screen, tile.rahc.tileData);
            return Imagen.Tile.NCGR.Crear_Imagen(tile, paleta);
        }
    }
}
