using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace AI_IGO_DS
{
    public static class ATEX
    {
        public static void Leer(string archivo, IPluginHost pluginHost)
        {
            NCGR imagen = new NCGR();
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            imagen.order = TileOrder.NoTiled;
            // Cabecera genérica
            imagen.header.id = br.ReadChars(4);
            imagen.header.endianess = 0xFFFE;
            imagen.header.constant = 0x0100;
            imagen.header.file_size = (uint)br.BaseStream.Length;
            imagen.header.header_size = 0x08;
            imagen.header.nSection = 1;
            // Tile data
            imagen.rahc.id = imagen.header.id;
            imagen.rahc.size_tiledata = br.ReadUInt32();
            imagen.rahc.size_section = imagen.rahc.size_tiledata;
            imagen.rahc.nTiles = (ushort)(imagen.rahc.size_tiledata / 32);
            imagen.rahc.nTilesX = br.ReadUInt16();
            imagen.rahc.nTilesY = br.ReadUInt16();
            imagen.rahc.tiledFlag = 0x00;
            imagen.rahc.depth = (br.ReadUInt16() == 0x04) ? System.Windows.Forms.ColorDepth.Depth4Bit : System.Windows.Forms.ColorDepth.Depth8Bit;

            imagen.rahc.tileData.tiles = new byte[1][];
            imagen.rahc.tileData.nPalette = new byte[imagen.rahc.nTiles];
            for (int i = 0; i < imagen.rahc.nTiles; i++)
            {
                imagen.rahc.tileData.nPalette[i] = 0;
            }
            imagen.rahc.tileData.tiles[0] = pluginHost.BytesTo4BitsRev(br.ReadBytes((int)imagen.rahc.size_tiledata));


            pluginHost.Set_NCGR(imagen);
            br.Close();

        }
    }
}
