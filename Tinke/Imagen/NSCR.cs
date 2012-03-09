using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NSCR
    {        
        //public static NSCR Create_BasicMap(int nTiles, int width, int height, int startTile = 0, byte palette = 0)
        //{
        //    NSCR map = new NSCR();

        //    // Common header
        //    map.header.id = "RCSN".ToCharArray();
        //    map.header.endianess = 0xFEFF;
        //    map.header.constant = 0x0100;
        //    map.header.header_size = 0x10;
        //    map.header.nSection = 1;

        //    // Lee primera y única sección:
        //    map.section.id = "NRCS".ToCharArray();
        //    map.section.width = (ushort)width;
        //    map.section.height = (ushort)height;
        //    map.section.padding = 0x00000000;
        //    map.section.data_size = (uint)nTiles * 2;
        //    map.section.mapData = new NTFS[nTiles];
        //    for (int i = 0; i < nTiles; i++)
        //    {
        //        map.section.mapData[i] = new NTFS();
        //        map.section.mapData[i].nPalette = palette;
        //        map.section.mapData[i].yFlip = 0;
        //        map.section.mapData[i].xFlip = 0;
        //        map.section.mapData[i].nTile = (ushort)(i + startTile);
        //    }
        //    map.section.section_size = map.section.data_size + 0x14;
        //    map.header.file_size = map.section.section_size + map.header.header_size;

        //    return map;
        //}
        //public static NSCR Create_BasicMap(int width, int height, int startFillTile, int fillTile, int startTile = 0, byte palette = 0)
        //{
        //    NSCR map = new NSCR();
        //    int nTiles = width * height / 64;

        //    // Common header
        //    map.header.id = "RCSN".ToCharArray();
        //    map.header.endianess = 0xFEFF;
        //    map.header.constant = 0x0100;
        //    map.header.header_size = 0x10;
        //    map.header.nSection = 1;

        //    // Lee primera y única sección:
        //    map.section.id = "NRCS".ToCharArray();
        //    map.section.width = (ushort)width;
        //    map.section.height = (ushort)height;
        //    map.section.padding = 0x00000000;
        //    map.section.data_size = (uint)nTiles * 2;
        //    map.section.mapData = new NTFS[nTiles];
        //    for (int i = 0; i < nTiles; i++)
        //    {
        //        map.section.mapData[i] = new NTFS();
        //        map.section.mapData[i].nPalette = palette;
        //        map.section.mapData[i].yFlip = 0;
        //        map.section.mapData[i].xFlip = 0;
        //        if (i >= startFillTile)
        //            map.section.mapData[i].nTile = (ushort)fillTile;
        //        else
        //            map.section.mapData[i].nTile = (ushort)(i + startTile);
        //    }
        //    map.section.section_size = map.section.data_size + 0x14;
        //    map.header.file_size = map.section.section_size + map.header.header_size;

        //    return map;
        //}

    }
}