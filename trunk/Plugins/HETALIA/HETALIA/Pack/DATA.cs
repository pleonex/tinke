// ----------------------------------------------------------------------
// <copyright file="DATA.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>21/04/2012 2:06:58</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using Ekona;

namespace HETALIA.Pack
{
    public static class DATA
    {
        public static sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            br.BaseStream.Position = 0x10;  // Header

            const int num_files = 0x1246;
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File_" + i.ToString() + '.';
                newFile.size = br.ReadUInt32();
                newFile.offset = br.ReadUInt32();
                newFile.path = file.path;

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = newFile.offset;
                string ext = new String(br.ReadChars(3));
                if (ext != "IMY" && ext != "MAP")
                    ext = "BIN";
                newFile.name += ext;
                br.BaseStream.Position = currPos;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
    }
}
