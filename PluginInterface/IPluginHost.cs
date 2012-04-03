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
using System.Linq;
using System.Text;
using System.Drawing;
using PluginInterface.Images;

namespace PluginInterface
{
    public interface IPluginHost
    {
        Object Get_Object();

        ImageBase Get_Image();
        PaletteBase Get_Palette();
        MapBase Get_Map();
        SpriteBase Get_Sprite();

        void Set_Object(Object objects);

        void Set_Image(ImageBase image);
        void Set_Palette(PaletteBase palette);
        void Set_Map(MapBase map);
        void Set_Sprite(SpriteBase sprite);
        
        Byte[][] MergeImage(Byte[][] originalTile, Byte[][] newTiles, int startTile);
        Color[][] Palette_4bppTo8bpp(Color[][] palette);
        Color[][] Palette_8bppTo4bpp(Color[][] palette);

        Bitmap Bitmaps_NCLR(Color[] colors);
        Bitmap Bitmap_NTFT(NTFT tiles, Color[][] palette, TileForm tileOrder, int startTile, int tilesX, int tilesY, int zoom = 1);

        Size Get_OAMSize(byte byte1, byte byte2);

        Byte[] BytesToBits(byte[] bytes);
        Byte[] BitsToBytes(byte[] bits);
        Byte[] Bit4ToBit8(byte[] bits4);
        Byte[] Bit8ToBit4(byte[] bits8);

        void Set_Files(sFolder folder);
        sFolder Get_Files();
        sFolder Get_DecompressedFiles(int id);

        String Search_File(int id); // Search file by id
        sFile Search_File(short id);
        Byte[] Get_Bytes(int id, int offset, int length);

        string Get_Language();
        string Get_LangXML();
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

        //NCLR BitmapToPalette(string bitmap, int paletteIndex = 0);
        //NCGR BitmapToTile(string bitmap, TileOrder tileOrder);
        //NSCR Create_BasicMap(int nTiles, int width, int height);

        /// <summary>
        /// Save an animation in a APNG file (Firefox supported)
        /// </summary>
        /// <param name="salida">The path of the output file</param>
        /// <param name="frames">All frames (path of files or bitmaps)</param>
        /// <param name="delay">The delay between frames (delay/1000)</param>
        /// <param name="loops">The number of  loops (if 0 = infinite)</param>
        void Create_APNG(string outFile, Bitmap[] frames, int delay, int loops);
        void Create_APNG(string outFile, String[] frames, int delay, int loops);

        Color[][] Read_WinPal2(string file, System.Windows.Forms.ColorDepth depth);
        void Write_WinPal(string fileOut, Color[] palette);
    }
}
