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
            return new ImageControl(pluginHost, Imagen_Bin());
        }

        public Bitmap Imagen_Bin()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            NCGR tile = new NCGR();
            NCLR paleta = new NCLR();
            NSCR map = new NSCR();

            uint header = br.ReadUInt32();

            paleta.pltt.length = br.ReadUInt32();
            tile.rahc.size_tiledata = br.ReadUInt32();
            map.section.data_size = br.ReadUInt32();
            tile.order = TileOrder.Horizontal;

            // Si el tamaño de la cabecera es 0x18 entonces hay información de ancho y largo, sino la imagen es imcompatible por el momento
            if (header == 0x18)
            {
                map.section.width = (ushort)br.ReadUInt32();
                map.section.height = (ushort)br.ReadUInt32();
            }
            else
            {
                //Console.WriteLine("Archivo .bin no reconocido, cabecera diferente a 0x18");
                //throw new Exception("Archivo incompatible");
                map.section.width = 0x0200;
                map.section.height = 0x00C0;
                map.section.id = "NO MAP".ToCharArray();
            }

            tile.rahc.nTilesX = (ushort)(map.section.width / 8);
            tile.rahc.nTilesY = (ushort)(map.section.height / 8);
            paleta.pltt.depth = (paleta.pltt.length < 512 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            tile.rahc.nTiles = (ushort)(paleta.pltt.depth == ColorDepth.Depth4Bit ?
                tile.rahc.size_tiledata / 32 :
                tile.rahc.size_tiledata / 64);

            // Comienzo de la paleta
            if (paleta.pltt.depth == ColorDepth.Depth4Bit)
                paleta.pltt.nColors = 0x10;
            else
                paleta.pltt.nColors = 0x0100;
            paleta.pltt.palettes = new NTFP[(paleta.pltt.length / 2) / paleta.pltt.nColors];
            for (int i = 0; i < paleta.pltt.palettes.Length; i++)
                paleta.pltt.palettes[i].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)(paleta.pltt.nColors * 2)));

            // Lectura de tiles
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            for (int i = 0; i < tile.rahc.nTiles; i++)
                if (paleta.pltt.depth == ColorDepth.Depth4Bit)
                    tile.rahc.tileData.tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(32));
                else
                    tile.rahc.tileData.tiles[i] = br.ReadBytes(64);

            // Lectura del map
            //if (new String(map.section.id) == "NO MAP")
            //    goto Fin;

            map.section.mapData = new NTFS[tile.rahc.nTilesX * tile.rahc.nTilesY];
            for (int i = 0; i < (map.section.data_size / 2); i++)
            {
                if (new String(map.section.id) == "NO MAP")
                {
                    map.section.mapData[i] = new NTFS();
                    map.section.mapData[i].nPalette = 0;
                    map.section.mapData[i].yFlip = 0;
                    map.section.mapData[i].xFlip = 0;
                    map.section.mapData[i].nTile = (ushort)br.ReadByte(); ;
                }
                else
                {
                    ushort parameters = br.ReadUInt16();

                    map.section.mapData[i] = new NTFS();
                    map.section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                    map.section.mapData[i].xFlip = (byte)((parameters >> 10) & 1);
                    map.section.mapData[i].yFlip = (byte)((parameters >> 11) & 1);
                    map.section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
                }
            }
            tile.rahc.tileData = pluginHost.Transform_NSCR(map, tile.rahc.tileData);

            br.Close();
            return pluginHost.Bitmap_NCGR(tile, paleta);
        }

    }
}
