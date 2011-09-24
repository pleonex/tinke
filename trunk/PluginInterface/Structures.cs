/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 *   by pleoNeX
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PluginInterface
{
    public struct sFile
    {
            public UInt32 offset;           // Offset where the files inside of the file in path
            public UInt32 size;             // Length of the file
            public string name;             // File name
            public UInt16 id;               // Internal id
            public string path;             // Path where the file is
            public Format format;           // Format file 
            public Object tag;              // Extra information
    }
    public struct sFolder
    {
        public List<sFile> files;           // List of files
        public List<sFolder> folders;      // List of folders
        public string name;                // Folder name
        public UInt16 id;                  // Internal id
        public Object tag;                 // Extra information
    }

    public struct Header    // Generic Header
    {
        public char[] id;                   
        public UInt16 endianess;            // 0xFFFE -> little endian
        public UInt16 constant;             // Always 0x0100
        public UInt32 file_size;            
        public UInt16 header_size;          // Always 0x10
        public UInt16 nSection;             // Number of sections
    }
    #region NCLR
    public struct NCLR      // Nintendo CoLor Resource
    {
        public Header header;
        public TTLP pltt;
        public Object other;
        public UInt32 id;
    }
    public struct TTLP  // PaLeTTe
    {
        public char[] ID;
        public UInt32 length;       
        public ColorDepth depth;
        public UInt32 unknown1;    // padding?
        public UInt32 paletteLength;
        public UInt32 nColors;    // Number of colors
        public NTFP[] palettes;
    }
    public struct NTFP              // Nintendo Tile Format Palette
    {
        public Color[] colors;
    }
    #endregion
    #region NCGR
    public struct NCGR  // Nintendo Character Graphic Resource
    {
        public Header header;
        public RAHC rahc;
        public SOPC sopc;
        public TileOrder order;
        public Object other;
        public UInt32 id;
    }
    public struct RAHC  // CHARacter
    {
        public char[] id;               // Always RAHC = 0x52414843
        public UInt32 size_section;
        public UInt16 nTilesY;
        public UInt16 nTilesX;
        public ColorDepth depth;
        public UInt16 unknown1;
        public UInt16 unknown2;
        public UInt32 tiledFlag;
        public UInt32 size_tiledata;
        public UInt32 unknown3;         // Always 0x18 (24) (data offset?)
        public NTFT tileData;

        public UInt32 nTiles;       // Number of tiles
    }
    public struct SOPC  // Unknown section
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
        public byte[] nPalette;     // Number of the palette that this tile uses
    }
    #endregion
    #region NSCR
    public struct NSCR      // Nintendo SCreen Resource
    {
        public Header header;
        public NSCR_Section section;        // Sección NSCR
        public Object other;
        public UInt32 id;
    }
    public struct NSCR_Section
    {
        public char[] id;                   // NRCS = 0x4E524353
        public UInt32 section_size;         
        public UInt16 width;                
        public UInt16 height;               
        public UInt32 padding;              // Always 0x0
        public UInt32 data_size;          
        public NTFS[] mapData;
    }
    public struct NTFS              // Nintedo Tile Format Screen
    {
        public byte nPalette;        // The parameters (two bytes) is PPPP Y X NNNNNNNNNN
        public byte xFlip;           
        public byte yFlip;
        public ushort nTile;
    }
    #endregion
    #region NCER
    public struct NCER       // Nintendo CEll Resource
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
        public UInt16 tBank;            // type of banks, 0 ó 1
        public UInt32 constant;
        public UInt32 block_size;
        public UInt32 unknown1;
        public UInt64 unknown2;         // padding?
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
        public Header header;
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

    public enum Format
    {
        Palette,
        Tile,
        Map,
        Cell,
        Animation,
        FullImage,
        Text,
        Video,
        Sound,
        Font,
        Compressed,
        Unknown,
        System,
        Script,
        Pack,
        Model3D,
        Texture
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
    public enum TileOrder
    {
        NoTiled,
        Horizontal,
        Vertical
    }
}