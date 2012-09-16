// ----------------------------------------------------------------------
// <copyright file="WMAP.cs" company="none">

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
// <date>01/09/2012 20:16:49</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
{
    public static class WMAP
    {
        static uint HEADER = 0x50414d57;    // "WMAP"

        public static sFolder Unpack(string fileIn, string name)
        {
            name = Path.GetFileNameWithoutExtension(name);

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            if (br.ReadUInt32() != HEADER)
            {
                br.Close();
                br = null;
                throw new FormatException("Invalid header!");
            }

            uint num_files = br.ReadUInt32();
            br.ReadUInt32();    // Unknown 1
            br.ReadUInt32();    // Unknown 2

            for (int i = 0; i < num_files; i++)
            {
                sFile cfile = new sFile();
                cfile.name = name + '_' + i.ToString() + ".bin";
                cfile.offset = br.ReadUInt32();
                cfile.size = br.ReadUInt32();
                cfile.path = fileIn;
                unpack.files.Add(cfile);
            }

            br.Close();
            br = null;
            return unpack;
        }
    }
}
