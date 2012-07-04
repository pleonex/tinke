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

namespace Pack
{
    public class Utility
    {
        string utilityFile;
        IPluginHost pluginHost;

        public Utility(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        #region Unpack methods
        public sFolder Unpack(string archivo)
        {
            ARC arc = new ARC();
            utilityFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "utility_" + Path.GetRandomFileName();
            File.Copy(archivo, utilityFile, true);
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(archivo));

            uint fntOffset = br.ReadUInt32();
            uint fntSize = br.ReadUInt32();
            uint fatOffset = br.ReadUInt32();
            uint fatSize = br.ReadUInt32();

            // FAT (File Allocation TaBle)
            br.BaseStream.Position = fatOffset;

            arc.btaf.nFiles = fatSize / 0x08;
            arc.btaf.entries = new BTAF_Entry[arc.btaf.nFiles];
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                arc.btaf.entries[i].start_offset = br.ReadUInt32();
                arc.btaf.entries[i].end_offset = br.ReadUInt32();
            }

            // FNT (File Name TaBle)
            br.BaseStream.Position = fntOffset;
            arc.btnf.entries = new List<BTNF_MainEntry>();
     
            #region Get root directory
            do
            {
                BTNF_MainEntry main = new BTNF_MainEntry();
                main.offset = br.ReadUInt32();
                main.first_pos = br.ReadUInt16();
                main.parent = br.ReadUInt16();
                uint idFile = main.first_pos;

                long currOffset = br.BaseStream.Position;
                br.BaseStream.Position = main.offset + fntOffset;
                int id = br.ReadByte();

                while (id != 0x0)   // End of subtable
                {
                    if ((id & 0x80) == 0)  // File
                    {
                        sFile currFile = new sFile();
                        currFile.id = (ushort)idFile;
                        idFile++;
                        currFile.name = new String(br.ReadChars(id));
                        
                        // Add the fat data
                        currFile.path = utilityFile;
                        currFile.offset = arc.btaf.entries[currFile.id].start_offset;
                        currFile.size = (arc.btaf.entries[currFile.id].end_offset - currFile.offset);

                        if (!(main.files is List<sFile>))
                            main.files = new List<sFile>();
                        main.files.Add(currFile);
                    }
                    else                // Directory
                    {
                        sFolder currFolder = new sFolder();
                        currFolder.name = new String(br.ReadChars(id - 0x80));
                        currFolder.id = br.ReadUInt16();
                        if (!(main.folders is List<sFolder>))
                            main.folders = new List<sFolder>();
                        main.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                }
                arc.btnf.entries.Add(main);
                br.BaseStream.Position = currOffset;

            } while (fntOffset + arc.btnf.entries[0].offset != br.BaseStream.Position);

            sFolder root = Create_TreeFolders(arc.btnf.entries, 0xF000, "root");
            #endregion

            br.Close();
            return root;
        }
        public sFolder Create_TreeFolders(List<BTNF_MainEntry> entries, int idFolder, string nameFolder)
        {
            sFolder currFolder = new sFolder();

            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = entries[idFolder & 0xFFF].files;

            if (entries[idFolder & 0xFFF].folders is List<sFolder>) // If there is folders inside.
            {
                currFolder.folders = new List<sFolder>();

                foreach (sFolder subFolder in entries[idFolder & 0xFFF].folders)
                    currFolder.folders.Add(Create_TreeFolders(entries, subFolder.id, subFolder.name));
            }

            return currFolder;
        }
        #endregion

        public String Pack(string fileIn, ref sFolder unpacked)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newUtility_" + Path.GetRandomFileName();

            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));      // Old pack file
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));    // New pack file
            List<byte> buffer = new List<byte>();   // Buffer with file data

            // By the moment, as we can not add files, the FNT section won't change, only repointing

            uint fntOffset = br.ReadUInt32();
            uint fntSize = br.ReadUInt32();
            uint fatOffset = br.ReadUInt32();
            uint fatSize = br.ReadUInt32();

            bw.Write(fntOffset);  
            bw.Write(fntSize);  
            bw.Write(fatOffset); 
            bw.Write(fatSize); 

            // Write FNT section
            br.BaseStream.Position = fntOffset;
            bw.Write(br.ReadBytes((int)fntSize));
            bw.Write((ulong)0x00);  // Padding
            bw.Flush();
            br.Close();

            // Write FAT section
            uint offset = fatOffset + fatSize + 0x10;
            for (int i = 0; i < fatSize / 8; i++)
            {
                bw.Write(offset);
                sFile currFile = Get_File(i + unpacked.id, offset, fileOut, unpacked);
                offset += currFile.size;
                bw.Write(offset);

                // Write the file to the buffer
                br = new BinaryReader(File.OpenRead(currFile.path));
                br.BaseStream.Position = currFile.offset;
                buffer.AddRange(br.ReadBytes((int)currFile.size));
                br.Close();

                // Padding
                if (offset % 4 != 0)
                {
                    for (int r = 0; r < 4 - (offset % 4); r++)
                        buffer.Add(0xFF);

                    offset += 4 - (offset % 4);
                }
            }
            bw.Write((ulong)0x00);
            bw.Write((ulong)0x00);

            // Write files
            bw.Write(buffer.ToArray());
            bw.Flush();
            bw.Close();

            buffer.Clear();

            return fileOut;
        }
        private sFile Get_File(int id, uint newOffset, string path, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        sFile original = currFolder.files[i];
                        sFile newFile = currFolder.files[i];
                        newFile.offset = newOffset;
                        newFile.path = path;
                        currFolder.files[i] = newFile;

                        return original;
                    }
                }
            }


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = Get_File(id, newOffset, path, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();

        }
    }
}
