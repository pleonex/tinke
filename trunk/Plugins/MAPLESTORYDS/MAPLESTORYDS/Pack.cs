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

namespace MAPLESTORYDS
{
    public static class PACK
    {
        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder folder = new sFolder();
            folder.id = 0xFFFF;

            #region Read nxarc file
            // Header
            NXAR nxarc = new NXAR();
            nxarc.header.type = br.ReadChars(4);
            nxarc.header.num_folders = br.ReadUInt16();
            nxarc.header.num_files = br.ReadUInt16();
            nxarc.header.fatOffset = br.ReadUInt32();
            nxarc.header.filesOffset = br.ReadUInt32();
            nxarc.header.folderTableSize = br.ReadUInt32();
            nxarc.header.unknown = br.ReadUInt32();
            nxarc.header.padding = br.ReadUInt64();

            // Folder table
            nxarc.folders = new sFolder[nxarc.header.num_folders];
            for (int i = 0; i < nxarc.header.num_folders; i++)
            {
                nxarc.folders[i] = new sFolder();
                nxarc.folders[i].files = new List<sFile>(br.ReadUInt16());
                nxarc.folders[i].id = br.ReadUInt16();  // ID of the first file
            }
            for (int i = 0; i < nxarc.header.num_folders; i++)
            {
                char c = '\x0';
                for (; ; )
                {
                    c = (char)br.ReadByte();
                    if (c == '\x0')
                        break;
                    nxarc.folders[i].name += c;
                }

            }
            
            // File offset table
            br.BaseStream.Position = nxarc.header.fatOffset;
            nxarc.files = new sFile[nxarc.header.num_files];
            for (int i = 0; i < nxarc.header.num_files; i++)
            {
                nxarc.files[i] = new sFile();
                nxarc.files[i].name = name + '_' + i.ToString() + ".bin";
                nxarc.files[i].path = file;
                nxarc.files[i].offset = br.ReadUInt32();
                nxarc.files[i].offset += nxarc.header.filesOffset;
                nxarc.files[i].size = br.ReadUInt32();
                nxarc.files[i].id = (ushort)i;
            }
            br.Close();
            #endregion

            // Work with the file
            folder.folders = new List<sFolder>();
            for (int i = 0; i < nxarc.header.num_folders; i++)
            {
                folder = ReorderFolder(folder, nxarc.folders[i], nxarc.folders[i].name);
            }
            for (int i = 0; i < nxarc.header.num_files; i++)
            {
                folder = AddFile(nxarc.files[i], folder);
            }

            return folder;
        }

        private static sFolder ReorderFolder(sFolder rootFolder, sFolder newFolder, string folderPath)
        {
            if (folderPath.Contains('/')) // Necessary to make folders
            {
                string parentFolder = folderPath.Substring(0, folderPath.IndexOf('/'));
                for (int i = 0; i < rootFolder.folders.Count; i++)
                {
                    if (rootFolder.folders[i].name == parentFolder)
                    {
                        folderPath = folderPath.Substring(folderPath.IndexOf('/') + 1);
                        rootFolder.folders[i] = ReorderFolder(rootFolder.folders[i], newFolder, folderPath);
                        return rootFolder;
                    }
                }
            }
            else
            {
                newFolder.name = folderPath;
                if (!(rootFolder.folders is List<sFolder>))
                    rootFolder.folders = new List<sFolder>();
                rootFolder.folders.Add(newFolder);

                return rootFolder;
            }

            return new sFolder();
        } 
        private static sFolder AddFile(sFile newFile, sFolder currfolder)
        {
            if (currfolder.files is List<sFile>)
            {
                if (newFile.id >= currfolder.id && newFile.id < (currfolder.id + currfolder.files.Capacity))
                {
                    if (!(currfolder.files is List<sFile>))
                        currfolder.files = new List<sFile>();
                    currfolder.files.Add(newFile);
                    return currfolder;
                }
            }

            if (currfolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currfolder.folders.Count; i++)
                {
                    sFolder returnFolder = AddFile(newFile, currfolder.folders[i]);
                    if (returnFolder.name is string)
                    {
                        currfolder.folders[i] = returnFolder;
                        return currfolder;
                    }
                }
            }

            return new sFolder();
        }

    }

    struct NXAR
    {
        public Header header;
        public sFolder[] folders;
        public sFile[] files;

        public struct Header
        {
            public char[] type;
            public ushort num_folders;
            public ushort num_files;
            public uint filesOffset;
            public uint fatOffset;
            public uint folderTableSize;
            public uint unknown;
            public ulong padding;
        }
    }
}
