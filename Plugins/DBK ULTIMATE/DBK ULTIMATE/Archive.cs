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

namespace DBK_ULTIMATE
{
    public static class Archive
    {
        public static sFolder Unpack_archiveDBK(IPluginHost pluginHost, string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            unpacked.folders = new List<sFolder>();

            // Read file
            char[] type = br.ReadChars(4); // Type of file "DSA "
            uint unknown = br.ReadUInt32();
            uint fileTableSize = br.ReadUInt32();
            uint num_folder = br.ReadUInt32();

            // Read folder
            for (int i = 0; i < num_folder; i++)
            {
                sFolder currFolder = new sFolder();
                currFolder.tag = br.ReadUInt32(); // First file ID
                currFolder.files = new List<sFile>((int)br.ReadUInt32());

                uint name_offset = br.ReadUInt32();
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = name_offset;
                char c = '\0';
                do
                {
                    c = (char)br.ReadByte();
                    currFolder.name += c;
                } while (c != '\0');
                currFolder.name = currFolder.name.Substring(1).Replace("\0", "");
                br.BaseStream.Position = currPos;

                unpacked = ReorderFolder(unpacked, currFolder, currFolder.name);
            }

            #region Read files
            for (int f = 0; f < unpacked.folders.Count; f++)
            {
                if (unpacked.folders[f].files is List<sFile>)   // If the folder contains files
                {
                    for (int i = 0; i < unpacked.folders[f].files.Capacity; i++)    // Read all files, the id start at 0
                    {
                        sFile newFile = new sFile();
                        newFile.path = file;

                        uint name_length = br.ReadUInt32();
                        uint name_offset = 0x00;
                        if (name_length != 0)
                            br.ReadUInt32();   // Not used
                        else
                            name_offset = br.ReadUInt32();
                        uint decompressed_size = br.ReadUInt32();   // Not used
                        newFile.size = br.ReadUInt32();
                        newFile.offset = br.ReadUInt32();

                        long currPos = br.BaseStream.Position;
                        if (name_length != 0)
                        {
                            br.BaseStream.Position = newFile.offset;
                            newFile.name = new String(
                                Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes((int)name_length)));
                            newFile.offset += name_length;
                        }
                        else
                        {
                            br.BaseStream.Position = name_offset;
                            char c = '\0';
                            do
                            {
                                c = (char)br.ReadByte();
                                newFile.name += c;
                            } while (c != '\0');

                        }
                        newFile.name = newFile.name.Replace("\0", "");
                        br.BaseStream.Position = currPos;

                        unpacked.folders[f].files.Add(newFile);
                    }
                }
                else // I know that this only work with this file (no more than two folders) but I haven't got so much time... :)
                {
                    for (int f2 = 0; f2 < unpacked.folders[f].folders.Count; f2++)
                    {
                        if (unpacked.folders[f].folders[f2].files is List<sFile>)   // If the folder contains files
                        {
                            for (int i = 0; i < unpacked.folders[f].folders[f2].files.Capacity; i++)    // Read all files, the id start at 0
                            {
                                sFile newFile = new sFile();
                                newFile.path = file;

                                uint name_length = br.ReadUInt32();
                                uint name_offset = 0x00;
                                if (name_length != 0)
                                    br.ReadUInt32();   // Not used
                                else
                                    name_offset = br.ReadUInt32();
                                uint decompressed_size = br.ReadUInt32();   // Not used
                                newFile.size = br.ReadUInt32();
                                newFile.offset = br.ReadUInt32();

                                long currPos = br.BaseStream.Position;
                                if (name_length != 0)
                                {
                                    br.BaseStream.Position = newFile.offset;
                                    newFile.name = new String(
                                        Encoding.GetEncoding("shift-jis").GetChars(br.ReadBytes((int)name_length)));
                                    newFile.offset += name_length;
                                }
                                else
                                {
                                    br.BaseStream.Position = name_offset;
                                    char c = '\0';
                                    do
                                    {
                                        c = (char)br.ReadByte();
                                        newFile.name += c;
                                    } while (c != '\0');

                                }
                                newFile.name = newFile.name.Replace("\0", "");
                                br.BaseStream.Position = currPos;

                                unpacked.folders[f].folders[f2].files.Add(newFile);
                            }
                        }
                    }
                }
            }
            #endregion

            br.Close();

            return unpacked;
        }

        private static sFolder ReorderFolder(sFolder rootFolder, sFolder currFolder, string folderPath)
        {
            if (folderPath.Contains('/')) // Necessary to make folders
            {
                string currName = folderPath.Substring(0, folderPath.IndexOf('/'));
                sFolder newFolder = SearchFolder(currName, rootFolder);

                folderPath = folderPath.Substring(folderPath.IndexOf('/') + 1);
                newFolder = ReorderFolder(newFolder, currFolder, folderPath);

                if (!(newFolder.name is string))
                {
                    newFolder.name = currName;
                    if (!(rootFolder.folders is List<sFolder>))
                        rootFolder.folders = new List<sFolder>();
                    rootFolder.folders.Add(newFolder);
                }
                return rootFolder;
            }
            else
            {
                currFolder.name = folderPath;
                if (!(rootFolder.folders is List<sFolder>))
                    rootFolder.folders = new List<sFolder>();
                rootFolder.folders.Add(currFolder);

                return rootFolder;
            }
            
        }
        private static sFolder SearchFolder(string name, sFolder currfolder)
        {
            if (currfolder.name == name)
                return currfolder;

            if (currfolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currfolder.folders.Count; i++)
                {
                    sFolder returnFolder = SearchFolder(name, currfolder.folders[i]);
                    if (returnFolder.name is string)
                        return returnFolder;
                }
            }

            return new sFolder();
        }
    }
}
