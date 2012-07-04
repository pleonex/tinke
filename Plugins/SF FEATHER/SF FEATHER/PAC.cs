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
using Ekona;

namespace SF_FEATHER
{
    public static class PAC
    {

        public static sFolder Unpack(string file, string name)
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
                newFile.name = name + '_' + i.ToString();
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

        public static void Pack(string file_original, string fileOut, ref sFolder unpacked)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file_original));
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            List<byte> buffer = new List<byte>();


            ushort num_element = br.ReadUInt16();
            bw.Write(num_element);
            bw.Write(br.ReadUInt16());  // Unknown
            bw.Write(br.ReadUInt32());  // Type of element

            uint offset = (uint)num_element * 8 + 8;
            uint size;
            int f = 0;  // Pointer to the unpacked.files array

            // Write the final padding of the FAT section
            if (offset % 0x10 != 0)
            {
                for (int r = 0; r < 0x10 - (offset % 0x10); r++)
                    buffer.Add(0x00);

                offset += 0x10 - (offset % 0x10);
            }


            for (int i = 0; i < num_element; i++)
            {
                uint older_offset = br.ReadUInt32();
                size = br.ReadUInt32();

                // If it's a null file
                if (size == 0)
                {
                    bw.Write(older_offset);
                    bw.Write(size);
                    continue;
                }

                // Get a normalized size
                size = unpacked.files[f].size;
                if (size % 0x10 != 0)
                    size += 0x10 - (size % 0x10);

                // Write the FAT section
                bw.Write((uint)(offset / 0x10));
                bw.Write((uint)(size / 0x10));

                // Write file
                BinaryReader fileRead = new BinaryReader(File.OpenRead(unpacked.files[f].path));
                fileRead.BaseStream.Position = unpacked.files[f].offset;
                buffer.AddRange(fileRead.ReadBytes((int)unpacked.files[f].size));                
                fileRead.Close();

                // Write the padding
                for (int r = 0; r < (size - unpacked.files[f].size); r++)
                    buffer.Add(0x00);

                // Set the new offset
                sFile newFile = unpacked.files[f];
                newFile.offset = offset;
                newFile.path = fileOut;
                unpacked.files[f] = newFile;

                // Set new offset
                offset += size;
                f++;
            }
            bw.Flush();

            bw.Write(buffer.ToArray());     

            br.Close();
            bw.Flush();
            bw.Close();
        }
    }
}
