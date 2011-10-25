﻿/*
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
using System.Linq;
using System.Text;
using System.Drawing;

namespace PluginInterface
{
    public interface IPluginHost
    {
        NCLR Get_NCLR();
        NCGR Get_NCGR();
        NSCR Get_NSCR();
        NCER Get_NCER();
        NANR Get_NANR();
        Object Get_Object();

        void Set_NCLR(NCLR nclr);
        void Set_NCGR(NCGR ncgr);
        void Set_NSCR(NSCR nscr);
        void Set_NCER(NCER ncer);
        void Set_NANR(NANR nanr);
        void Set_Object(Object objects);

        
        Color[] BGR555(byte[] data);
        Byte[] ColorToBGR555(Color[] color);
        Byte[] BytesTo4BitsRev(byte[] data);
        String BytesToBits(byte[] datos);
        Byte[] Bit4ToBit8(byte[] bits4);
        Byte[] Bit8ToBit4(byte[] bits8);
        Byte[] TilesToBytes(byte[][] tiles);
        Byte[][] BytesToTiles(byte[] bytes);
        Byte[][] BytesToTiles_NoChanged(byte[] bytes, int tilesX, int tilesY);
        TTLP Palette_4bppTo8bpp(TTLP palette);
        TTLP Palette_8bppTo4bpp(TTLP palette);
        void Change_Color(ref byte[][] tiles, int oldIndex, int newIndex);

        Bitmap[] Bitmaps_NCLR(NCLR nclr);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY);
        NTFT Transform_NSCR(NSCR nscr, NTFT ntft, int startOffset = 0);
        Size Size_NCER(byte byte1, byte byte2);
        Bitmap Bitmap_NCER(Bank bank, uint blockSize, NCGR ncgr, NCLR nclr, bool guides, bool cell,
            bool numbers, bool transparency, bool image);
        Bitmap Bitmap_NCER(Bank bank, uint blockSize, NCGR tile, NCLR paleta,
            bool guides, bool cell, bool numbers, bool transparency, bool image, int maxWidth, int maxHeight);
        /// <summary>
        /// Save an animation in a APNG file (Firefox supported)
        /// </summary>
        /// <param name="salida">The path of the output file</param>
        /// <param name="frames">All frames (path of files or bitmaps)</param>
        /// <param name="delay">The delay between frames (delay/1000)</param>
        /// <param name="loops">The number of  loops (if 0 = infinite)</param>
        void Create_APNG(string outFile, Bitmap[] frames, int delay, int loops);
        void Create_APNG(string outFile, String[] frames, int delay, int loops);

        // Decompressed files methods
        void Set_Files(sFolder files);
        sFolder Get_Files();
        sFolder Get_DecompressedFiles(int id); // Get all the files and folder that have been decompressed (to compress them)
        String Search_File(int id); // Search file by id
        sFile Search_File(short id);

        string Get_Language();
        string Get_TempFolder();

        void Decompress(string file);
        void Decompress(byte[] data);
        void Compress(string filein, string fileout, FormatCompress format);

        /// <summary>
        /// Change the content of a file
        /// </summary>
        /// <param name="id">The id of the file to change</param>
        /// <param name="newFile">The path where the new file is</param>
        void ChangeFile(int id, string newFile);

        NCLR BitmapToPalette(string bitmap);
        NCGR BitmapToTile(string bitmap, TileOrder tileOrder);
        NSCR Create_BasicMap(int nTiles, int width, int height);
    }
}