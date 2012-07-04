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

namespace Ekona
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

    public struct NTFS              // Nintedo Tile Format Screen
    {
        public byte nPalette;        // The parameters (two bytes) is PPPP Y X NNNNNNNNNN
        public byte xFlip;
        public byte yFlip;
        public ushort nTile;
    }
    public struct NTFT              // Nintendo Tile Format Tile
    {
        public byte[] tiles;
        public byte[] nPalette;     // Number of the palette that this tile uses
    }

    public struct NitroHeader    // Generic Header in Nitro formats
    {
        public char[] id;
        public UInt16 endianess;            // 0xFFFE -> little endian
        public UInt16 constant;             // Always 0x0100
        public UInt32 file_size;
        public UInt16 header_size;          // Always 0x10
        public UInt16 nSection;             // Number of sections
    }
}