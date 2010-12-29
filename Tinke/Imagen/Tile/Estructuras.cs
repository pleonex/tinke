using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tinke.Imagen.Tile
{
    public static class Estructuras
    {
        public struct NCGR
        {
            public char[] id;
            public UInt16 id_endian;
            public UInt16 constant;
            public UInt32 size_file;
            public UInt16 size_header;
            public UInt16 nSections;
            public RAHC rahc;
            public SOPC sopc;
        }
        public struct RAHC
        {
            public char[] id;               // Siempre RAHC = 0x52414843
            public UInt32 size_section;
            public UInt16 nTilesY;
            public UInt16 nTilesX;
            public Imagen.Tiles_Form depth;
            public UInt16 unknown1;
            public UInt16 unknown2;
            public UInt32 padding;
            public UInt32 size_tiledata;
            public UInt32 unknown3;         // Constante siempre 0x18 (24)
            public Imagen.NTFT tileData;

            public UInt16 nTiles;       // Campo propio para operaciones más fáciles, resultado de nTilesX * nTilesY ó size_Tiledata / 64
        }
        public struct SOPC
        {
            public char[] id;
            public UInt32 size_section;
            public UInt32 unknown1;
            public UInt16 nTilesX;
            public UInt16 nTilesY;
        }
    }
}
