using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace AI_IGO_DS
{
    public static class R00
    {
        public static Control Leer(string archivo, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            // Todos los offset del archivo han de ser multiplicados por 4
            // Cabecera
            uint paletaOffset = br.ReadUInt32() * 4;
            uint tileOffset = br.ReadUInt32() * 4;
            uint mapOffset = br.ReadUInt32() * 4;
            // Paleta
            br.BaseStream.Position = paletaOffset;
            uint pCabeceraSize = br.ReadUInt32() * 4;
            uint pSize = br.ReadUInt32() * 4;
            NCLR paleta = new NCLR();
            paleta.pltt.tamaño = pSize - 0x08;
            paleta.pltt.tamañoPaletas = pSize - 0x08;
            paleta.pltt.nColores = (pSize - 0x08) / 2;
            paleta.pltt.profundidad = ColorDepth.Depth8Bit;
            paleta.pltt.paletas = new NTFP[1];
            paleta.pltt.paletas[0].colores = pluginHost.BGR555(br.ReadBytes((int)paleta.pltt.tamañoPaletas));
            // Tile data
            br.BaseStream.Position = tileOffset;
            uint tCabeceraSize = br.ReadUInt32() * 4;
            uint tSize = br.ReadUInt32() * 4;
            NCGR tile = new NCGR();
            tile.orden = Orden_Tiles.Horizontal;
            tile.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            tile.rahc.nTilesX = (ushort)(0x20);
            tile.rahc.nTilesY = (ushort)(0x18);
            tile.rahc.nTiles = (ushort)(tile.rahc.nTilesX * tile.rahc.nTilesY);

            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPaleta = new byte[tile.rahc.nTiles];
            for (int i = 0; i < tile.rahc.nTiles; i++)
            {
                tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
                tile.rahc.tileData.nPaleta[i] = 0;
            }
            // Map, innecesario y estropea la imagen
            br.BaseStream.Position = mapOffset;
            uint mCabeceraSize = br.ReadUInt32() * 4;
            uint mSize = br.ReadUInt32() * 4;
            NSCR map = new NSCR();
            map.section.width = br.ReadUInt16();
            map.section.height = br.ReadUInt16();

            map.section.mapData = new NTFS[mSize / 2];
            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                string bits = pluginHost.BytesToBits(br.ReadBytes(2));

                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                map.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                map.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                map.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();

            return new BinControl(pluginHost, paleta, new NCGR[] { tile });
        }
    }
}
