// ----------------------------------------------------------------------
// <copyright file="Filedata.cs" company="none">
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
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>07/03/2013 21:09:54</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Teniprimgaku
{
    public static class Filedata
    {
        public static sFolder Unpack_Type1(sFile fileIn)
        {
            FileStream fs = new FileStream(fileIn.path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            sFolder unpacked = new sFolder();
            unpacked.folders = new List<sFolder>();

            uint numFolders = br.ReadUInt32() / 4;
            for (int i = 0; i < numFolders; i++)
            {
                Console.WriteLine("Reading folder {0}", i);

                // Get folder address
                fs.Position = 4 + i * 4;
                uint startAddress = br.ReadUInt32() * 4;

                sFolder folder = new sFolder();
                folder.name = string.Format("Folder {0}", i);
                folder.files = new List<sFile>();

                fs.Position = startAddress;
                while (true)
                {
                    // Read file header
                    uint header = br.ReadUInt32();
                    bool encoded = (header & 0x80000000) == 1;
                    uint fileSize = header & ~0x80000000;
                    
                    if (header == 0xFFFFFFFF)
                    {
                        // End of folder
                        break;
                    }

                    if (fileSize == 0)
                    {
                        // Null file
                        continue;
                    }

                    sFile file = new sFile();
                    file.path = fileIn.path;
                    file.size = fileSize;
                    file.offset = (uint)fs.Position;
                    file.name = string.Format("File {0}.bin", folder.files.Count);

                    folder.files.Add(file);
                    fs.Position += fileSize;
                }

                if (folder.files.Count > 0)
                {
                    unpacked.folders.Add(folder);
                }
            }

            br = null;
            fs.Close();
            fs.Dispose();
            fs = null;

            return unpacked;
        }

        public static sFolder Unpack_Type2(sFile fileIn)
        {
            FileStream fs = new FileStream(fileIn.path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            sFolder unpacked = new sFolder();
            unpacked.folders = new List<sFolder>();

            uint numFolders = br.ReadUInt32() / 4;
            for (int i = 0; i < numFolders; i++)
            {
                Console.WriteLine("Reading folder {0}", i);

                // Get folder address
                fs.Position = 4 + i * 4;
                uint folderAddress = br.ReadUInt32() * 4;

                sFolder folder = GetSubfolder_Type2(fs, folderAddress, fileIn.path);
                folder.name = string.Format("Folder {0}", i);
                if (folder.files.Count > 0 || folder.folders.Count > 0)
                {
                    unpacked.folders.Add(folder);
                }
            }

            br = null;
            fs.Close();
            fs.Dispose();
            fs = null;

            return unpacked;
        }

        private static sFolder GetSubfolder_Type2(Stream str, uint folderAddress, string path)
        {
            BinaryReader br = new BinaryReader(str);

            sFolder folder = new sFolder();
            folder.files = new List<sFile>();
            folder.folders = new List<sFolder>();

            // Get number of files
            str.Position = folderAddress;
            uint numFiles = br.ReadUInt32();
            numFiles = (numFiles == 0xFFFFFFFF) ? 0 : numFiles / 4;

            for (int j = 0; j < numFiles; j++)
            {
                str.Position = folderAddress + j * 4;
                uint fileAddress = (br.ReadUInt32() - 4) + (uint)str.Position;

                // Read file header
                str.Position = fileAddress;
                uint header = br.ReadUInt32();
                bool encoded = (header & 0x80000000) == 1;
                uint fileSize = header & ~0x80000000;

                if (fileSize == 0)
                {
                    // Null file
                    continue;
                }

                uint magicStamp = br.ReadUInt32();
                str.Position += 8;
                uint magicStamp2 = br.ReadUInt32();
                if (magicStamp == 8 && (magicStamp2 == Cell.MagicStamp || magicStamp2 == Scrn.MagicStamp))
                {
                    // High chance it's a subfolder
                    sFolder subfolder = GetSubfolder_Type2(str, fileAddress + 4, path);
                    subfolder.name = string.Format("Subfolder {0}", folder.folders.Count);
                    folder.folders.Add(subfolder);
                }
                else
                {
                    // It's a file
                    sFile file = new sFile();
                    file.path = path;
                    file.size = fileSize;
                    file.offset = fileAddress + 4;
                    file.name = string.Format("File {0}.bin", j);
                    folder.files.Add(file);
                }
            }

            return folder;
        }
    }
}
