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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace GYAKUKEN
{
    /// <summary>
    /// Class for pack files in Gyakuten kenji 2
    /// </summary>
    public static class PACK
    {
        public static sFolder Unpack(IPluginHost pluginHost, sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            // Read offset table
            uint currOffset = 0x00;
            ushort size = 0x00;
            for (int i = 0; ; i++)
            {
                currOffset = br.ReadUInt32();
                size = br.ReadUInt16(); // Size of the file, if it's compressed, size of the decompressed file
                br.ReadUInt16(); // Compress flag
                if (size == 0x00)
                    break;

                sFile currFile = new sFile();
                currFile.name = file.name + '_' + i.ToString() + ".dbin";
                currFile.offset = currOffset;
                currFile.path = file.path;

                currOffset = br.ReadUInt32();
                currFile.size = currOffset - currFile.offset;
                br.BaseStream.Position -= 4;

                unpack.files.Add(currFile);
            }

            br.Close();
            return unpack;
        }
        public static sFolder Unpack2(IPluginHost pluginHost, sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            // Read offset table
            uint num_files = br.ReadUInt32() / 4;
            br.BaseStream.Position -= 4;
            for (int i = 0; i < num_files; i++)
            {
                sFile currFile = new sFile();
                currFile.name = file.name + i.ToString();
                if (i == 0)
                    currFile.name += ".ncer";
                else if (i == 1)
                    currFile.name += ".nanr";
                else if (i == 2)
                    currFile.name += ".ncgr";
                currFile.offset = br.ReadUInt32();
                currFile.path = file.path;

                if (i + 1 == num_files) // Last file
                    currFile.size = (uint)br.BaseStream.Length - currFile.offset;
                else
                    currFile.size = br.ReadUInt32() - currFile.offset;
                br.BaseStream.Position -= 4;

                unpack.files.Add(currFile);
            }

            br.Close();
            return unpack;
        }
    }
}
