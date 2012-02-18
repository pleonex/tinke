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

namespace INAZUMA11
{
    public static class PAC
    {
        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            bool images = true;
            string parent_name = Path.GetFileNameWithoutExtension(file).Substring(12);
            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = parent_name + " - " + i.ToString();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = file;

                if ((num_files == 3 || num_files == 2 || num_files == 4) && images)
                {
                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = newFile.offset;
                    string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
                    br.BaseStream.Position = currPos;

                    if (ext == "BMD0" || ext == "BCA0" || ext == "CTB\0" || ext == "BMA0" || ext == "BTP0" || ext == "BTA0")
                        images = false;
                    if (newFile.size == 0 && i != 1)
                        images = false;
                    if (i == 0 && num_files == 3 && newFile.size > 0x200)
                        images = false;
                }

                unpacked.files.Add(newFile);
            }

            uint fat_size = br.ReadUInt32();

            br.Close();

            // Set extension
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = unpacked.files[i];
                if (num_files == 2 && images)
                {
                    if (i == 1) newFile.name += ".nbfp";
                    if (i == 0) newFile.name += ".nbfc";
                }
                else if ((num_files == 3 ||num_files == 4) && images)
                {
                    if (i == 0) newFile.name += ".nbfp";
                    if (i == 1) newFile.name += ".nbfs";
                    if (i == 2) newFile.name += ".nbfc";
                    if (i == 3) newFile.name += ".bin";
                }
                else
                    newFile.name += ".bin";
                unpacked.files[i] = newFile;
            }

            return unpacked;
        }
    }

    public static class PKB
    {
        public static sFolder Unpack(string pkb, string pkh)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(8)));
            br.Close();

            if (type == "PackNum ")
                return Unpack_PKH1(pkb, pkh);
            else
                return Unpack_PKH2(pkb, pkh);
        }

        public static sFolder Unpack_PKH1(string pkb, string pkh)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            br.BaseStream.Position = 0x10;  // Skip the header name
            uint file_size = br.ReadUInt32();
            ushort unknown = br.ReadUInt16();
            uint num_files = br.ReadUInt16();
            uint unknown2 = br.ReadUInt32();
            uint unknown3 = br.ReadUInt32();

            br.BaseStream.Position += 0x10; // 0x00

            for (int i = 0; i < num_files; i++)
            {
                br.ReadUInt32();    // Unknown, ID¿?

                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".pac_";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = pkb;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public static sFolder Unpack_PKH2(string pkb, string pkh)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            int num_files = (int)br.BaseStream.Length / 0x10;

            for (int i = 0; i < num_files; i++)
            {
                br.ReadUInt32();    // Unknown - ID¿?

                sFile newFile = new sFile();
                newFile.name = "File " + i.ToString() + ".pac_";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = pkb;

                br.ReadUInt32();    // First four bytes that indicates the type of compression and the final size

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }

    public static class SFP
    {
        public static sFolder Unpack(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();    // 0x00
            uint unknown2 = br.ReadUInt32();    // 0x05
            uint unknown3 = br.ReadUInt32();    // 0x20
            uint sect_size = br.ReadUInt32();

            br.BaseStream.Position = 0x20;
            uint num_files = (br.ReadUInt32() - 0x20) / 0x10;
            br.BaseStream.Position = 0x20;

            for (int i = 0; i < num_files; i++)
            {
                uint name_offset = br.ReadUInt32();

                sFile newFile = new sFile();
                newFile.size = br.ReadUInt32();
                newFile.offset = br.ReadUInt32() * 0x20 + sect_size;
                newFile.path = file;
                newFile.name = "";

                br.ReadUInt32();    // 0x00 padding

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = name_offset;
                byte c;
                for (; ; )
                {
                    c = br.ReadByte();
                    if (c == 0)
                        break;
                    newFile.name += (char)c;
                }
                br.BaseStream.Position = currPos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public static sFolder Unpack(string spd, string spl)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(spl));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();    // 0x00
            uint unknown2 = br.ReadUInt32();    // 0x05
            uint unknown3 = br.ReadUInt32();    // 0x20 (header size or value to mul)
            uint sect_size = br.ReadUInt32();

            br.BaseStream.Position = 0x20;
            uint num_files = (br.ReadUInt32() - 0x20) / 0x10;
            br.BaseStream.Position = 0x20;

            for (int i = 0; i < num_files; i++)
            {
                uint name_offset = br.ReadUInt32();

                sFile newFile = new sFile();
                newFile.size = br.ReadUInt32();
                newFile.offset = br.ReadUInt32() * 0x20;
                newFile.path = spd;
                newFile.name = "";

                br.ReadUInt32();    // 0x00 padding

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = name_offset;
                byte c;
                for (; ; )
                {
                    c = br.ReadByte();
                    if (c == 0)
                        break;
                    newFile.name += (char)c;
                }
                br.BaseStream.Position = currPos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

    }
}
