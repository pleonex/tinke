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
using Ekona.Images;

namespace Ekona
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

        string[] PluginList();
        Object Call_Plugin(string[] param, int id, int action);

        void Set_Files(sFolder folder);
        sFolder Get_Files();
        sFolder Get_DecompressedFiles(int id);

        String Search_File(int id); // Search file by id
        sFile Search_File(short id);
        sFolder Search_File(string name);
        Byte[] Get_Bytes(string path, int offset, int length);

        sFolder Search_Folder(int id);

        string Get_Language();
        string Get_LangXML();

        string Get_LanguageFolder();

        string Get_TempFile();
        string Get_TempFolder();
        void Set_TempFolder(string newPath);
        void Restore_TempFolder();

        void Decompress(string file);
        void Decompress(byte[] data);
        void Compress(string filein, string fileout, FormatCompress format);

        /// <summary>
        /// Change the content of a file
        /// </summary>
        /// <param name="id">The id of the file to change</param>
        /// <param name="newFile">The path where the new file is</param>
        void ChangeFile(int id, string newFile);
    }
}
