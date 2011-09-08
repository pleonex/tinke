using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PluginInterface
{
    public struct Archivo
        {
            public UInt32 offset;           // Offset inside of a pack file
            public string packFile;
            public UInt32 size;             // Tamaño definido por la resta del offset inicial y el offset final
            public string name;             // Nombre dado en la FNT
            public UInt16 id;               // ID único de cada archivo.
            public string path;             // En caso de haber sido descomprimido y estar fuera del sistema de archivos.
            public Formato formato;         // Formato del archivo  
            public Object tag;              // Para asignar cualquier valor
    }
    public struct Carpeta
    {
        public List<Archivo> files;        // Lista de archivos que contiene el directorio
        public List<Carpeta> folders;      // Lista de carpetas que contiene el directorio
        public string name;                // Nombre de la carpeta
        public UInt16 id;                  // ID de la carpeta. Comienza en 0xF000 que es root
        public Object tag;                 // Para asignar cualquier valor
    }

    public struct Header    // Generic Header
    {
        public char[] id;                   // RCSN = 0x5243534E
        public UInt16 endianess;            // 0xFFFE -> indica little endian
        public UInt16 constant;             // Siempre es 0x0100
        public UInt32 file_size;            // Tamaño total del archivo
        public UInt16 header_size;          // Siempre es 0x10
        public UInt16 nSection;             // Número de secciones
    }
    #region NCLR
    public struct NCLR
    {
        public Header cabecera;
        public TTLP pltt;
        public Object other;
        public UInt32 id;
    }
    public struct TTLP
    {
        public char[] ID;
        public UInt32 tamaño;       // Incluye cabecera
        public ColorDepth profundidad;
        public UInt32 unknown1;    // ¿¿padding??
        public UInt32 tamañoPaletas;
        public UInt32 nColores;     // Suele ser 0x10
        public NTFP[] paletas;
    }

    public struct NTFP              // Nintendo Tile Format Palette
    {
        public Color[] colores;
    }
    #endregion
    #region NCGR
    public struct NCGR
    {
        public Header cabecera;
        public RAHC rahc;
        public SOPC sopc;
        public Orden_Tiles orden;
        public Object other;
        public UInt32 id;
    }
    public struct RAHC
    {
        public char[] id;               // Siempre RAHC = 0x52414843
        public UInt32 size_section;
        public UInt16 nTilesY;
        public UInt16 nTilesX;
        public ColorDepth depth;
        public UInt16 unknown1;
        public UInt16 unknown2;
        public UInt32 tiledFlag;
        public UInt32 size_tiledata;
        public UInt32 unknown3;         // Constante siempre 0x18 (24)
        public NTFT tileData;

        public UInt32 nTiles;       // Campo propio para operaciones más fáciles, resultado de nTilesX * nTilesY ó size_Tiledata / 64
    }
    public struct SOPC
    {
        public char[] id;
        public UInt32 size_section;
        public UInt32 unknown1;
        public UInt16 charSize;
        public UInt16 nChar;
    }

    public struct NTFT              // Nintendo Tile Format Tile
    {
        public byte[][] tiles;
        public byte[] nPaleta;
    }
    #endregion
    #region NSCR
    public struct NSCR
    {
        public Header cabecera;
        public NSCR_Section section;        // Sección NSCR
        public Object other;
        public UInt32 id;
    }
    public struct NSCR_Section
    {
        public char[] id;                   // NRCS = 0x4E524353
        public UInt32 section_size;         // Tamaño del archivo total
        public UInt16 width;                // Ancho de la imagen
        public UInt16 height;               // Alto de la imagen
        public UInt32 padding;              // Siempre 0x0
        public UInt32 data_size;            //
        public NTFS[] mapData;
    }
    public struct NTFS              // Nintedo Tile Format Screen
    {
        public byte nPalette;        // Junto con los cuatro siguientes forman dos bytes de la siguiente forma (en bits):
        public byte xFlip;           // PPPP Y X NNNNNNNNNN
        public byte yFlip;
        public ushort nTile;
    }
    #endregion
    #region NCER
    public struct NCER       // CEll Resource
    {
        public Header header;
        public CEBK cebk;
        public LABL labl;
        public UEXT uext;
        public Object other;
        public UInt32 id;
    }
    public struct CEBK
    {
        public char[] id;
        public UInt32 section_size;
        public UInt16 nBanks;
        public UInt16 tBank;            // Formato de bank, 0 ó 1
        public UInt32 constant;
        public UInt32 block_size;
        public UInt32 unknown1;
        public UInt64 unknown2;         // ¿¿ padding ??
        public Bank[] banks;
    }
    public struct Bank
    {
        public UInt16 nCells;
        public UInt16 unknown1;
        public UInt32 cell_offset;
        public Cell[] cells;
    }
    public struct Cell
    {
        public UInt16 width;
        public UInt16 height;
        public Int32 xOffset;
        public Int32 yOffset;
        public UInt32 tileOffset;
        public byte nPalette;
        public byte priority;
        public bool xFlip;
        public bool yFlip;
        public ushort num_cell;
    }
    #endregion // CER
    #region NANR
    public struct NANR
    {
        public Header cabecera;
        public ABNK abnk;
        public LABL labl;
        public UEXT uext;
        public Object other;
        public UInt32 id;
    }
    public struct ABNK
    {
        public char[] id;
        public uint length;
        public ushort nBanks;
        public ushort tFrames;
        public uint constant;
        public uint offset1;
        public uint offset2;
        public ulong padding;
        public Animation[] anis;
    }
    public struct Animation
    {
        public uint nFrames;
        public ushort dataType;
        public ushort unknown1;
        public ushort unknown2;
        public ushort unknown3;
        public uint offset_frame;
        public Frame[] frames;
    }
    public struct Frame
    {
        public uint offset_data;
        public ushort unknown1;
        public ushort constant;
        public Frame_Data data;
    }
    public struct Frame_Data
    {
        public ushort nCell;
        // DataType 1
        public ushort[] transform; // See http://nocash.emubase.de/gbatek.htm#lcdiobgrotationscaling
        public short xDisplacement;
        public short yDisplacement;
        //DataType 2 (the Displacement above)
        public ushort constant; // 0xBEEF
    }
    #endregion
    public struct LABL
    {
        public char[] id;
        public UInt32 section_size;
        public UInt32[] offset;
        public string[] names;
    }
    public struct UEXT
    {
        public char[] id;
        public UInt32 section_size;
        public UInt32 unknown;
    }

    public enum Formato
    {
        Paleta,
        Imagen,
        Map,
        Celdas,
        Animación,
        ImagenCompleta,
        Texto,
        Video,
        Sonido,
        Fuentes,
        Comprimido,
        Desconocido,
        Sistema,
        Script
    }
    public enum FormatCompress // From DSDecmp
    {
        LZOVL, // keep this as the first one, as only the end of a file may be LZ-ovl-compressed (and overlay files are oftenly double-compressed)
        LZ10,
        LZ11,
        HUFF4,
        HUFF8,
        RLE,
        HUFF,
        NDS,
        GBA,
        Invalid
    }
    public enum Orden_Tiles
    {
        No_Tiles,
        Horizontal,
        Vertical
    }
}