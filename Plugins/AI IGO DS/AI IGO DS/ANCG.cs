using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace AI_IGO_DS
{
    public static class ANCG
    {
        public static void Leer(string archivo, IPluginHost pluginHost)
        {
            NCGR imagen = new NCGR();
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            imagen.orden = Orden_Tiles.Horizontal;
            // Cabecera genérica
            imagen.cabecera.id = br.ReadChars(4);
            imagen.cabecera.endianess = 0xFFFE;
            imagen.cabecera.constant = 0x0100;
            imagen.cabecera.file_size = (uint)br.BaseStream.Length;
            imagen.cabecera.header_size = 0x08;
            imagen.cabecera.nSection = 1;
            // Tile data
            imagen.rahc.id = imagen.cabecera.id;
            imagen.rahc.size_tiledata = br.ReadUInt32();
            imagen.rahc.size_section = imagen.rahc.size_tiledata;
            imagen.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            imagen.rahc.nTiles = (ushort)(imagen.rahc.size_tiledata / 32);
            imagen.rahc.nTilesX = 8;
            imagen.rahc.nTilesY = (ushort)(imagen.rahc.nTiles / 8);
            imagen.rahc.tiledFlag = 0x00;

            imagen.rahc.tileData.tiles = new byte[imagen.rahc.nTiles][];
            imagen.rahc.tileData.nPaleta = new byte[imagen.rahc.nTiles];
            for (int i = 0; i < imagen.rahc.nTiles; i++)
            {
                imagen.rahc.tileData.tiles[i] = pluginHost.BytesTo4BitsRev(br.ReadBytes(32));
                imagen.rahc.tileData.nPaleta[i] = 0;
            }


            pluginHost.Set_NCGR(imagen);
            br.Close();
        }
    }
}
