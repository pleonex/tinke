using System;
using System.Drawing;
using System.Collections.Generic;

namespace Tinke.Imagen
{
    public struct Header    // Generic Header
    {
        public char[] id;
        public UInt16 endian;
        public UInt16 constant;
        public UInt32 file_size;
        public UInt16 header_size;
        public UInt16 nSections;
    }

    public struct NTFP              // Nintendo Tile Format Palette
    {
        public Color[] colores;
    }
    public struct NTFT              // Nintendo Tile Format Tile
    {
        public byte[][] tiles;
        public byte[] nPaleta;
    }
    public struct NTFS              // Nintedo Tile Format Screen
    {
        public byte nPalette;        // Junto con los cuatro siguientes forman dos bytes de la siguiente forma (en bits):
        public byte xFlip;           // PPPP X Y NNNNNNNNNN
        public byte yFlip;
        public ushort nTile;
    }

    #region NCER
    public struct CER       // CEll Resource
    {
        public Header header;
        public CEBK cebk;
        public LABL labl;
        public UEXT uext;
    }
    public struct CEBK
    {
        public char[] id;
        public UInt32 section_size;
        public UInt16 nBanks;
        public UInt16 tBank;            // Formato de bank, 0 ó 1
        public UInt32 constant;
        public UInt32 unknown1;
        public UInt32 unknown2;
        public UInt64 unknown3;         // ¿¿ padding ??
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
    }
    #endregion // CER
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

    public enum Tiles_Form
    {
        bpp8 = 0,
        bpp4 = 1
    }
    public enum Tiles_Style
    {
        Tiled,
        Horizontal,
        Vertical
    }
}