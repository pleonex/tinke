// ----------------------------------------------------------------------
// <copyright file="Project.cs" company="none">
// Copyright (C) 2013
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
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
//
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>07/03/2013 21:26:08</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Teniprimgaku
{
    public static class Project
    {
        public static sFolder Unpack(sFile tableFile, string dataFile)
        {
            FileStream fs = new FileStream(tableFile.path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint numFiles = br.ReadUInt32();
            string baseName = Path.GetFileNameWithoutExtension(tableFile.name);
            
            for (int i = 0; i < numFiles; i++)
            {
                sFile file = new sFile();
                file.name = baseName + "_" + i.ToString() + ".bin";
                file.path = dataFile;
                file.offset = br.ReadUInt32();
                file.size = br.ReadUInt32();
                br.ReadUInt32();    // Decoded size

                unpacked.files.Add(file);
            }

            br = null;
            fs.Close();
            fs.Dispose();
            fs = null;

            return unpacked;
        }
    }
}
