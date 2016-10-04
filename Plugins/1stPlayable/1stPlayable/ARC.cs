/*
 * Copyright (C) 2016
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
 * Plugin by: ccawley2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace _1stPlayable
{
    public static class ARC
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost, string gameCode)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = br.ReadUInt32();
            for (int i = 0; i < num_files; i++)
            {
                uint unknown = br.ReadUInt32();
                uint startOffset = br.ReadUInt32();
                int comSize = br.ReadInt32();
                if (gameCode == "TB6X") {
                    uint fileSize = br.ReadUInt32();
                }
                long currPos = br.BaseStream.Position;

                sFile newFile = new sFile();
                newFile.name = "0x"+unknown.ToString("X");
                newFile.offset = startOffset;
                newFile.path = file;
                newFile.size = (uint)Math.Abs(comSize);

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }

}
