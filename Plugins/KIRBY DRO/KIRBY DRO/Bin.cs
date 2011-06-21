using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace KIRBY_DRO
{
    class Bin
    {
        IPluginHost pluginHost;
        string archivo;

        public Bin(string archivo, IPluginHost pluginHost)
        {
            this.archivo = archivo;
            this.pluginHost = pluginHost;
        }

        public Control Show_Info()
        {
            PictureBox pics = new PictureBox();
            pics.SizeMode = PictureBoxSizeMode.AutoSize;
            pics.Image = Imagen_Bin();

            return pics;
        }

        public Bitmap Imagen_Bin()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            NCGR tile = new NCGR();
            NCLR paleta = new NCLR();
            NSCR screen = new NSCR();

            uint header = br.ReadUInt32();

            paleta.pltt.tamaño = br.ReadUInt32();
            tile.rahc.size_tiledata = br.ReadUInt32();
            screen.section.data_size = br.ReadUInt32();
            tile.orden = Orden_Tiles.Horizontal;

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
            paleta.pltt.profundidad = (paleta.pltt.tamaño < 512 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            tile.rahc.nTiles = (ushort)(paleta.pltt.profundidad == ColorDepth.Depth4Bit ?
                tile.rahc.size_tiledata / 32 :
                tile.rahc.size_tiledata / 64);

            // Comienzo de la paleta
            paleta.pltt.nColores = 0x10;
            paleta.pltt.paletas = new NTFP[(paleta.pltt.tamaño / 2) / paleta.pltt.nColores];
            for (int i = 0; i < paleta.pltt.paletas.Length; i++)
                paleta.pltt.paletas[i].colores = pluginHost.BGR555(br.ReadBytes((int)(paleta.pltt.nColores * 2)));

            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            for (int i = 0; i < tile.rahc.nTiles; i++)
                if (paleta.pltt.profundidad == ColorDepth.Depth4Bit)
                    tile.rahc.tileData.tiles[i] = pluginHost.BytesTo4BitsRev(br.ReadBytes(32));
                else
                    tile.rahc.tileData.tiles[i] = br.ReadBytes(64);

            screen.section.screenData = new NTFS[tile.rahc.nTilesX * tile.rahc.nTilesY];
            for (int i = 0; i < (screen.section.data_size / 2); i++)
            {
                string bits = pluginHost.BytesToBits(br.ReadBytes(2));

                screen.section.screenData[i] = new NTFS();
                screen.section.screenData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                screen.section.screenData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                screen.section.screenData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                screen.section.screenData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }
            br.Close();

            tile.rahc.tileData = pluginHost.Transformar_NSCR(screen, tile.rahc.tileData);
            return pluginHost.Bitmap_NCGR(tile, paleta);
        }

    }
}
