// ----------------------------------------------------------------------
// <copyright file="TMAP.cs" company="none">

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
// <date>08/06/2012 2:08:13</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
{
    public static class TMAP
    {

        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint unk1 = br.ReadUInt32();
            uint unk2 = br.ReadUInt32();
            uint unk3 = br.ReadUInt32();

            uint numfiles = (br.ReadUInt32() - 0x10) / 8;
            br.BaseStream.Position = 0x10;

            for (int i = 0; i < numfiles; i++)
            {
                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = file;

                if (newFile.offset == 0 || newFile.size == 0)
                    continue;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
    }
}
