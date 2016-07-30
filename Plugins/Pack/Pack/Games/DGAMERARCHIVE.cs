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
 * By: pleoNeX, ccawley2011
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class DGamerArchive : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;
        ARC narc;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }
        public bool IsCompatible()
        {
            return true;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {

            if (file.name.ToUpper().EndsWith(".FUN"))
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
        public sFolder Unpack(sFile file)
        {
            ARC arc = new ARC();
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(file.path));

            uint fnt_offset = br.ReadUInt32();
            arc.btnf.section_size = br.ReadUInt32();
            uint fat_offset = br.ReadUInt32();
            uint fat_length = br.ReadUInt32();

            // BTAF (File Allocation TaBle)
            br.BaseStream.Position = fat_offset;
            arc.btaf.nFiles = fat_length/8;
            arc.btaf.entries = new BTAF_Entry[arc.btaf.nFiles];
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                arc.btaf.entries[i].start_offset = br.ReadUInt32();
                arc.btaf.entries[i].end_offset = br.ReadUInt32();
            }

            // BTNF (File Name TaBle)
            br.BaseStream.Position = fnt_offset;
            arc.btnf.entries = new List<BTNF_MainEntry>();

            long mainTables_offset = br.BaseStream.Position;
            uint gmif_offset = 0;

            br.BaseStream.Position += 6;
            ushort num_mains = br.ReadUInt16();
            br.BaseStream.Position -= 8;

            for (int m = 0; m < num_mains; m++)
            {
                BTNF_MainEntry main = new BTNF_MainEntry();
                main.offset = br.ReadUInt32();
                main.first_pos = br.ReadUInt16();
                main.parent = br.ReadUInt16();
                uint idFile = main.first_pos;

                long posmain = br.BaseStream.Position;
                br.BaseStream.Position = main.offset + mainTables_offset;

                int id = br.ReadByte();
                while (id != 0x0)   // Indicate the end of the subtable
                {
                    if ((id & 0x80) == 0)  // File
                    {
                        sFile currFile = new sFile();
                        currFile.id = (ushort)idFile++;
                        currFile.name = new String(br.ReadChars(id));

                        // FAT data
                        currFile.path = file.path;
                        currFile.offset = arc.btaf.entries[currFile.id].start_offset + gmif_offset;
                        currFile.size = (arc.btaf.entries[currFile.id].end_offset - arc.btaf.entries[currFile.id].start_offset);

                        if (!(main.files is List<sFile>))
                            main.files = new List<sFile>();
                        main.files.Add(currFile);
                    }
                    else  // Directory
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
                br.BaseStream.Position = posmain;
            }

            sFolder root = Create_TreeFolders(arc.btnf.entries, 0xF000, "root");

            br.Close();
            narc = arc;
            return root;
        }
        private sFolder Create_TreeFolders(List<BTNF_MainEntry> entries, int idFolder, string nameFolder)
        {
            sFolder currFolder = new sFolder();

            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = entries[idFolder & 0xFFF].files;

            if (entries[idFolder & 0xFFF].folders is List<sFolder>) // If there are folders
            {
                currFolder.folders = new List<sFolder>();

                foreach (sFolder subFolder in entries[idFolder & 0xFFF].folders)
                    currFolder.folders.Add(Create_TreeFolders(entries, subFolder.id, subFolder.name));
            }

            return currFolder;
        }
        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            throw new NotImplementedException();
        }
    }
    public struct ARC
    {
        public BTAF btaf;
        public BTNF btnf;
    }
    public struct BTAF
    {
        public UInt32 nFiles;
        public BTAF_Entry[] entries;
    }
    public struct BTAF_Entry
    {
        // Ambas son relativas a la sección GMIF
        public UInt32 start_offset;
        public UInt32 end_offset;
    }
    public struct BTNF
    {
        public UInt32 section_size;
        public List<BTNF_MainEntry> entries;
    }
    public struct BTNF_MainEntry
    {
        public UInt32 offset;       // Relativo a la primera entrada
        public UInt32 first_pos;    // ID del primer archivo.
        public UInt32 parent;       // En el caso de root, número de carpetas;
        public List<sFile> files;
        public List<sFolder> folders;
    }
}
