/*
 * Copyright (C) 2011  CUE, pleoNeX
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
 * By: CUE, pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace LAYTON
{
    /// <summary>
    /// Pack files from LAYTON4 game (London Life). Credits to CUE
    /// </summary>
    public static class DARC
    {

        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint number_files = br.ReadUInt32();

            for (int i = 0; i < number_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".denc";
                newFile.offset = br.ReadUInt32() + (uint)br.BaseStream.Position;    // Relative offset
                newFile.path = file;

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = newFile.offset - 4;
                newFile.size = br.ReadUInt32();
                br.BaseStream.Position = currPos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }

    /// <summary>
    /// Compressed files inside of DARC. Credits again to CUE :)
    /// </summary>
    public static class DENC
    {

        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder decode = new sFolder();
            decode.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint decoded_length = br.ReadUInt32();
            bool coded = (new String(br.ReadChars(4)) == "LZSS" ? true : false);
            uint coded_length = br.ReadUInt32();

            if (!coded)
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileNameWithoutExtension(file);
                newFile.offset = (uint)br.BaseStream.Position;
                newFile.size = decoded_length;
                newFile.path = file;

                newFile.name += '.' + new String(br.ReadChars(4));

                decode.files.Add(newFile);
            }
            else
            {
                // ENCODING UNKNOWN. It is not the same as normal LZSS.
            }

            br.Close();
            return decode;
        }
    }
}
