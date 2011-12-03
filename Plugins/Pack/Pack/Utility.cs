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

        #region Pack methods
        public String Pack(string fileIn, ref sFolder unpacked)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newUtility_" + Path.GetRandomFileName();



            return fileOut;
        }
        public Stream Write_FAT(ref sFolder unpacked)
        {
            MemoryStream ms = new MemoryStream();

            return ms;
        }
        #endregion
    }
}
