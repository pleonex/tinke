// ----------------------------------------------------------------------
// <copyright file="Packs.cs" company="none">

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
// <date>27/04/2012 23:54:44</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace INAZUMA11
{
    public static class PAC
    {
        public static sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            bool images = true;
            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = file.name + " - " + i.ToString();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = file.path;

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
                else if ((num_files == 3 || num_files == 4) && images)
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
        public static void Pack(ref sFolder unpacked, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write((uint)unpacked.files.Count);

            uint header_length = 8 + 8 * (uint)unpacked.files.Count;
            uint offset = header_length;
            List<byte> buffer = new List<byte>();
            BinaryReader br;

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                sFile currfile = unpacked.files[i];

                // Get the file data
                br = new BinaryReader(File.OpenRead(currfile.path));
                br.BaseStream.Position = currfile.offset;
                buffer.AddRange(br.ReadBytes((int)currfile.size));
                br.Close();

                // Write the fat data
                bw.Write(offset);
                bw.Write(currfile.size);

                // Update file structure
                currfile.offset = offset;
                currfile.path = fileout;
                unpacked.files[i] = currfile;

                offset += currfile.size;
            }

            bw.Write(offset - header_length);
            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();
        }
    }

    public static class PKB
    {
        public static sFolder Unpack(sFile pkb, sFile pkh)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(8)));
            br.Close();

            if (type == "PackNum ")
                return Unpack_PKH1(pkb, pkh);
            else
                return Unpack_PKH2(pkb, pkh);
        }
        public static sFolder Unpack_PKH1(sFile pkb, sFile pkh)
        {
            // Fixed problem with some files thanks to ouioui2003
            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            br.BaseStream.Position = 0x10;  // Skip the header name
            ushort file_size = br.ReadUInt16();
            ushort type = br.ReadUInt16();
            ushort unknown = br.ReadUInt16();
            uint num_files = br.ReadUInt16();
            uint unknown2 = br.ReadUInt32();
            uint block_length = br.ReadUInt32();

            br.BaseStream.Position += 0x10; // 0x00

            if (type == 0)
            {
                for (int i = 0; i < num_files; i++)
                {
                    br.ReadUInt32();    // Unknown, ID¿?

                    sFile newFile = new sFile();
                    newFile.name = pkb.name + '_' + i.ToString() + ".pac_";
                    newFile.offset = br.ReadUInt32();
                    newFile.size = br.ReadUInt32();
                    newFile.path = pkb.path;

                    unpacked.files.Add(newFile);
                }
            }
            else if (type == 3 || type == 2)
            {
                for (int i = 0; i < num_files; i++)
                {
                    br.ReadUInt32();    // Unknown, ID¿?

                    sFile newFile = new sFile();
                    newFile.name = pkb.name + '_' + i.ToString() + ".pac_";
                    newFile.offset = (uint)i * block_length;
                    newFile.size = block_length;
                    newFile.path = pkb.path;

                    unpacked.files.Add(newFile);
                }
            }

            br.Close();
            return unpacked;
        }
        public static sFolder Unpack_PKH2(sFile pkb, sFile pkh)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            int num_files = (int)br.BaseStream.Length / 0x10;

            for (int i = 0; i < num_files; i++)
            {
                br.ReadUInt32();    // Unknown - ID¿?

                sFile newFile = new sFile();
                newFile.name = pkb.name + '_' + i.ToString() + ".pac_";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = pkb.path;

                br.ReadUInt32();    // First four bytes that indicates the type of compression and the final size

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static void Pack(ref sFolder unpacked, sFile pkh, string fileOut, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(8)));
            br.Close();

            if (type == "PackNum ")
                Pack_PKH1(ref unpacked, pkh, fileOut, pluginHost);
            else
                Pack_PKH2(ref unpacked, pkh, fileOut, pluginHost);
        }
        public static void Pack_PKH1(ref sFolder unpacked, sFile pkh, string fileOut, IPluginHost pluginHost)
        {
            string pkh_out = pluginHost.Get_TempFile();

            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            BinaryWriter bwPkb = new BinaryWriter(File.OpenWrite(fileOut));
            BinaryWriter bwPkh = new BinaryWriter(File.OpenWrite(pkh_out));

            // Write header
            bwPkh.Write(Encoding.ASCII.GetChars(br.ReadBytes(16))); // Header
            bwPkh.Write(br.ReadUInt16());                           //File_Size
            ushort type = br.ReadUInt16();
            bwPkh.Write(type);                                      //Type
            bwPkh.Write(br.ReadUInt16());                           //Unknown
            ushort num_files = br.ReadUInt16();
            bwPkh.Write(num_files);                                 //Num_files
            bwPkh.Write(br.ReadUInt32());                           //Unknown

            if (type == 0)
                bwPkh.Write(br.ReadUInt32());                       //Block_Length
            else if (type == 2 || type == 3)
            {
                bwPkh.Write(unpacked.files[0].size);
                br.BaseStream.Position += 4;
            }

            bwPkh.Write(br.ReadBytes(0x10));                        // Unknown, usually 0x00 but not always

            uint offset = 0;
            for (int i = 0; i < num_files; i++)
            {
                BinaryReader br_file = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br_file.BaseStream.Position = unpacked.files[i].offset;
                bwPkb.Write(br_file.ReadBytes((int)unpacked.files[i].size));  //Write File in new PKB
                while (bwPkb.BaseStream.Position % 0x10 != 0)       // Padding
                    bwPkb.Write((byte)0xFF);
                br_file.Close();
                bwPkb.Flush();

                if (type == 0)
                {
                    bwPkh.Write(br.ReadUInt32());           // Unknown, ID¿?
                    bwPkh.Write(offset);                    // Offset
                    bwPkh.Write(unpacked.files[i].size);    //Size File

                    sFile cfile = unpacked.files[i];
                    cfile.offset = offset;
                    cfile.path = fileOut;
                    unpacked.files[i] = cfile;

                    br.BaseStream.Position += 8;
                    offset += unpacked.files[i].size;
                    if (offset % 0x10 != 0)
                        offset += 0x10 - (offset % 0x10);
                }
                else if (type == 2)
                    bwPkh.Write(br.ReadUInt64());
                else if (type == 3)
                    bwPkh.Write(br.ReadUInt32());
            }

            while (bwPkh.BaseStream.Position % 0x10 != 0)
                bwPkh.Write((byte)0xFF);

            bwPkh.Flush();

            bwPkb.Close();
            bwPkh.Close();

            br.Close();

            pluginHost.ChangeFile(pkh.id, pkh_out);
        }
        public static void Pack_PKH2(ref sFolder unpacked, sFile pkh, string fileOut, IPluginHost pluginHost)
        {
            string fileOut2 = pluginHost.Get_TempFile();

            BinaryReader br = new BinaryReader(File.OpenRead(pkh.path));
            BinaryWriter bwPkb = new BinaryWriter(File.OpenWrite(fileOut));
            BinaryWriter bwPkh = new BinaryWriter(File.OpenWrite(fileOut2));

            int num_files = (int)br.BaseStream.Length / 0x10;

            // Get the PKH info
            uint[,] info = new uint[num_files, 5];
            for (int i = 0; i < num_files; i++)
            {
                info[i, 2] = br.ReadUInt32();   // ID
                info[i, 0] = br.ReadUInt32();   // old offset, used to get the file order
                info[i, 1] = br.ReadUInt32();   // old size
                br.BaseStream.Position += 4;    // Skip LZSS header
            }
            br.Close();
            br = null;

            File_Order2(ref info);  // Get the file order

            // Create PKB file, we write it in order
            uint offset = 0;
            for (int i = 0; i < num_files; i++)
            {
                // Get the index of the next file to write
                int index = 0;
                for (; index < num_files; index++)
                    if (info[index, 4] == i)
                        break;
                if (info[index, 4] != i)
                    Console.WriteLine("Error searching index file!");

                sFile cfile = unpacked.files[index];

                // Write PKB file
                BinaryReader br_file = new BinaryReader(File.OpenRead(cfile.path));
                br_file.BaseStream.Position = cfile.offset;

                bwPkb.Write(br_file.ReadBytes((int)cfile.size));        //Write File in new PKB
                while (bwPkb.BaseStream.Position % 0x10 != 0)           // Padding
                    bwPkb.Write((byte)0xFF);
                bwPkb.Flush();

                // Get LZSS header
                br_file.BaseStream.Position = cfile.offset;
                info[index, 3] = br_file.ReadUInt32();
                if ((info[index, 3] & 0xFF) != 0x10 || ((info[index, 3] >> 8) == 0))         // File no compressed, it can be a PAC file
                    info[index, 3] = 0;

                br_file.Close();
                br_file = null;

                // Set new values
                cfile.offset = offset;
                cfile.path = fileOut;
                offset += cfile.size;
                if (offset % 0x10 != 0)                 // Padding offset
                    offset += 0x10 - (offset % 0x10);

                unpacked.files[index] = cfile;
            }

            for (int i = 0; i < num_files; i++)
            {
                // Write PKH file
                bwPkh.Write(info[i, 2]);           // Unknown ID
                bwPkh.Write(unpacked.files[i].offset);
                bwPkh.Write(unpacked.files[i].size);
                bwPkh.Write(info[i, 3]);
            }

            // The file is padded
            while (bwPkh.BaseStream.Position % 0x10 != 0)
                bwPkh.Write((byte)0xFF);

            // Flush and close files
            bwPkh.Flush();

            bwPkh.Close();
            bwPkb.Close();

            pluginHost.ChangeFile(pkh.id, fileOut2);
        }

        // Get the order of the files
        private static void File_Order2(ref uint[,] info)
        {
            // It gets the order to write files by searching the next offset
            uint offset = 0;

            // Get a index per file
            for (uint i = 0; i < info.GetLength(0); i++)
            {
                // Search the next offset
                for (uint index = 0; index < info.Length; index++)
                {
                    if (info[index, 0] == offset)   // Find next entry
                    {
                        info[index, 4] = i;         // Set the index
                        offset += info[index, 1];   // Add the size

                        // Offset padded
                        if (offset % 0x10 != 0)
                            offset += 0x10 - (offset % 0x10);
                        break;
                    }
                }
            }

        }
    }

    public static class SFP
    {
        public static sFolder Unpack(string sfp_info, string sfp_data)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(sfp_info));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] header = br.ReadChars(4);
            uint unknown1 = br.ReadUInt32();    // Always 0x00
            uint unknown2 = br.ReadUInt32();    // Always 0x05
            uint unknown3 = br.ReadUInt32();    // Always 0x20

            uint sect_size = br.ReadUInt32();

            br.BaseStream.Position = 0x20;
            uint num_files = (br.ReadUInt32() - 0x20) / 0x10;  // The game really does it!
            br.BaseStream.Position = 0x28;
            if (br.ReadUInt32() != 0x20)
            {
                Console.WriteLine("First offset isn't 0x20!");
                Console.ReadKey();
            }
            br.BaseStream.Position = 0x20;

            for (int i = 0; i < num_files; i++)
            {
                uint name_offset = br.ReadUInt32();

                sFile newFile = new sFile();
                newFile.size = br.ReadUInt32();
                newFile.offset = br.ReadUInt32() * 0x20;
                if (sfp_info == sfp_data)
                    newFile.offset += sect_size;
                newFile.path = sfp_data;
                newFile.name = "";

                br.ReadUInt32();    // 0x00 padding

                if (name_offset == 0x00)
                {
                    newFile.name = "No name.bin";
                    unpacked.files.Add(newFile);
                    continue;
                }

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
        public static void Pack(ref sFolder unpacked, string fileout, string fileout2, bool separated)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            BinaryReader br = null;

            // INFO section
            bw.Write(0x00504653);   // SFP\0
            bw.Write(0x00);         // Always 0x00
            bw.Write(0x05);         // Always 0x05
            bw.Write(0x20);         // Always 0x20

            List<byte> names = new List<byte>();
            List<byte> fat = new List<byte>();
            List<byte> buffer = new List<byte>();

            uint name_offset = (uint)(0x20 + unpacked.files.Count * 0x10);
            uint file_offset = 0x400;    // First file will be always there

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                sFile cfile = unpacked.files[i];

                if (cfile.size == 0x00)
                {
                    fat.AddRange(new byte[0x10]);
                    continue;
                }

                // Store the name
                names.AddRange(Encoding.GetEncoding("shift_jis").GetBytes(cfile.name));
                names.Add(0x00);    // Null terminator

                // Store and update name offset
                fat.AddRange(BitConverter.GetBytes(name_offset));
                int name_size = Encoding.GetEncoding("shift_jis").GetByteCount(cfile.name) + 1;
                name_offset += (uint)name_size;

                fat.AddRange(BitConverter.GetBytes(cfile.size));
                fat.AddRange(BitConverter.GetBytes((uint)(file_offset / 0x20)));
                fat.AddRange(BitConverter.GetBytes((uint)0x00));

                // Store file data and update offset
                br = new BinaryReader(File.OpenRead(cfile.path));
                br.BaseStream.Position = cfile.offset;
                buffer.AddRange(br.ReadBytes((int)cfile.size));
                while (buffer.Count % 0x20 != 0)
                    buffer.Add(0x00);

                br.Close();
                br = null;

                file_offset += cfile.size;
                if (file_offset % 0x20 != 0)
                    file_offset += 0x20 - (file_offset % 0x20);

                // Update file info
                cfile.offset = file_offset;
                cfile.path = fileout2;

                unpacked.files[i] = cfile;
            }
            if (!separated)
                while ((0x20 + fat.Count + names.Count) % 0x20 != 0)
                    names.Add(0x00);

            // Write the rest of the section
            uint sect_size = (uint)(0x20 + fat.Count + names.Count);
            bw.Write(sect_size);
            bw.Write(new byte[0x0C]);
            bw.Write(fat.ToArray());
            bw.Write(names.ToArray());

            // DATA section
            if (separated)
            {
                bw.Flush();
                bw.Close();
                bw = new BinaryWriter(File.OpenWrite(fileout2));
            }
            bw.Write(0x00504653);   // SFP\0
            bw.Write(0x00);         // Always 0x00
            bw.Write(0x05);         // Always 0x05
            bw.Write(0x20);         // Always 0x20

            sect_size = (uint)(0x400 + buffer.Count);
            bw.Write(sect_size);
            bw.Write(new byte[0x3EC]);
            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();

            buffer = null;
            names = null;
            fat = null;
            bw = null;
        }
    }
}
