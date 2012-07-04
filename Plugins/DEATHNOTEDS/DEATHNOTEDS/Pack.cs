// ----------------------------------------------------------------------
// <copyright file="Pack.cs" company="none">

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
// <date>28/04/2012 14:56:02</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace DEATHNOTEDS
{
    public static class Packs
    {
        public static sFolder Unpack_data(sFile file)
        {
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            uint num_files = br.ReadUInt32();
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileNameWithoutExtension(file.name) + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32() * 4;
                newFile.size = br.ReadUInt32();
                newFile.path = file.path;
                
                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }
}
