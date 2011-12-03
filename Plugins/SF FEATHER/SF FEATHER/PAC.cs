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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace SF_FEATHER
{
    public static class PAC
    {

        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            ushort num_element = br.ReadUInt16();
            ushort unknown = br.ReadUInt16();
            uint type_file = br.ReadUInt32();

            for (int i = 0; i < num_element; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString();
                newFile.offset = br.ReadUInt32() * 0x10;
                newFile.size = br.ReadUInt32() * 0x10;
                newFile.path = file;

                // Extension check
                if (newFile.size != 0x00)
                {
                    bool compressed = false;

                    // Check if this file is pac, it searches the extension
                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = newFile.offset;
                    byte cInd = br.ReadByte();
                    uint cSize = br.ReadUInt32();
                    if ((cInd == 0x11 || cInd == 0x10) && cSize < 0x2000000)
                        compressed = true;

                    // Search the indicator of the pac file
                    if (compressed)
                        br.BaseStream.Position = newFile.offset + 9;
                    else
                        br.BaseStream.Position = newFile.offset + 4;
                    uint currType = br.ReadUInt32();

                    if (currType == 0x04)
                        newFile.name += ".pac";
                    else
                    {
                        // Search for the header extension
                        if (compressed)
                            br.BaseStream.Position = newFile.offset + 5;
                        else
                            br.BaseStream.Position = newFile.offset;
                           
                        currType = br.ReadUInt32();
                        char[] ext = Encoding.ASCII.GetChars(BitConverter.GetBytes(currType));
                        String extS = ".";
                        for (int s = 0; s < 4; s++)
                            if (Char.IsLetterOrDigit(ext[s]) || ext[s] == 0x20)
                                extS += ext[s];

                        if (extS != "." && extS.Length == 5)
                            newFile.name += extS;
                        else
                            newFile.name += ".bin";
                    }

                    br.BaseStream.Position = currPos;
                }
                else
                    continue;

                unpacked.files.Add(newFile);
            }            

            br.Close();
            return unpacked;
        }
    }
}
