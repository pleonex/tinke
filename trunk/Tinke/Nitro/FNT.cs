// ----------------------------------------------------------------------
// <copyright file="FNT.cs" company="none">

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
// <date>28/04/2012 14:26:14</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Tinke.Nitro
{
    /// <summary>
    /// File Name Table.
    /// </summary>
    public static class FNT
    {
        /// <summary>
        /// Devuelve el sistema de archivos internos de la ROM
        /// </summary>
        /// <param name="file">Archivo ROM</param>
        /// <param name="offset">Offset donde comienza la FNT</param>
        /// <returns></returns>
        public static sFolder LeerFNT(string file, UInt32 offset)
        {
            sFolder root = new sFolder();
            List<Estructuras.MainFNT> mains = new List<Estructuras.MainFNT>();

            BinaryReader br = new BinaryReader(File.OpenRead(file)); 
            br.BaseStream.Position = offset;
            
            long offsetSubTable = br.ReadUInt32();  // Offset donde comienzan las SubTable y terminan las MainTables.
            br.BaseStream.Position  = offset;       // Volvemos al principio de la primera MainTable
           
            while (br.BaseStream.Position < offset + offsetSubTable)
            {
                Estructuras.MainFNT main = new Estructuras.MainFNT();
                main.offset = br.ReadUInt32();
                main.idFirstFile = br.ReadUInt16();
                main.idParentFolder = br.ReadUInt16();
                
                long currOffset = br.BaseStream.Position;           // Posición guardada donde empieza la siguienta maintable
                br.BaseStream.Position = offset + main.offset;      // SubTable correspondiente
                
                // SubTable
                byte id = br.ReadByte();                            // Byte que identifica si es carpeta o archivo.
                ushort idFile = main.idFirstFile;

                while (id != 0x0)   // Indicador de fin de la SubTable
                {
                    if (id < 0x80)  // Archivo
                    {
                        sFile currFile = new sFile();

                        if (!(main.subTable.files is List<sFile>))
                            main.subTable.files = new List<sFile>();

                        int lengthName = id;
                        currFile.name = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(lengthName)));
                        currFile.id = idFile; idFile++;
                        
                        main.subTable.files.Add(currFile);
                    }
                    if (id > 0x80)  // Directorio
                    {
                        sFolder currFolder = new sFolder();

                        if (!(main.subTable.folders is List<sFolder>))
                           main.subTable.folders = new List<sFolder>();

                        int lengthName = id - 0x80;
                        currFolder.name = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(lengthName)));
                        currFolder.id = br.ReadUInt16();

                        main.subTable.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                } 

                mains.Add(main);
                br.BaseStream.Position = currOffset;
            } 

            root = Jerarquizar_Carpetas(mains, 0, "root");
            root.id = 0xF000;

            br.Close();

            return root;
        }
        public static sFolder ReadFNT(string romFile, uint fntOffset, Estructuras.sFAT[] fat, Acciones accion)
        {
            sFolder root = new sFolder();
            root.files = new List<sFile>();
            List<Estructuras.MainFNT> mains = new List<Estructuras.MainFNT>();

            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = fntOffset;

            br.BaseStream.Position += 6;
            ushort number_directories = br.ReadUInt16();  // Get the total number of directories (mainTables)
            br.BaseStream.Position = fntOffset;

            for (int i = 0; i < number_directories; i++)
            {
                Estructuras.MainFNT main = new Estructuras.MainFNT();
                main.offset = br.ReadUInt32();
                main.idFirstFile = br.ReadUInt16();
                main.idParentFolder = br.ReadUInt16();

                if (i != 0)
                {
                    if (br.BaseStream.Position > fntOffset + mains[0].offset)
                    {                                      //  Error, in some cases the number of directories is wrong
                        number_directories--;              // Found in FF Four Heroes of Light, Tetris Party deluxe
                        i--;
                        continue;
                    }
                }

                long currOffset = br.BaseStream.Position;           // Posición guardada donde empieza la siguienta maintable
                br.BaseStream.Position = fntOffset + main.offset;      // SubTable correspondiente

                // SubTable
                byte id = br.ReadByte();                            // Byte que identifica si es carpeta o archivo.
                ushort idFile = main.idFirstFile;

                while (id != 0x0)   // Indicador de fin de la SubTable
                {
                    if (id < 0x80)  // File
                    {
                        sFile currFile = new sFile();

                        if (!(main.subTable.files is List<sFile>))
                            main.subTable.files = new List<sFile>();

                        int lengthName = id;
                        currFile.name = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(lengthName)));
                        currFile.id = idFile; idFile++;

                        // FAT part
                        currFile.offset = fat[currFile.id].offset;
                        currFile.size = fat[currFile.id].size;
                        currFile.path = romFile;

                        // Temporaly, for plugins (Get_Format):
                        root.files.Add(currFile);
                        accion.Root = root;

                        // Get the format
                        long pos = br.BaseStream.Position;
                        br.BaseStream.Position = currFile.offset;
                        currFile.format = accion.Get_Format(br.BaseStream, currFile.name, currFile.id, currFile.size);
                        br.BaseStream.Position = pos;

                        main.subTable.files.Add(currFile);
                    }
                    if (id > 0x80)  // Directorio
                    {
                        sFolder currFolder = new sFolder();

                        if (!(main.subTable.folders is List<sFolder>))
                            main.subTable.folders = new List<sFolder>();

                        int lengthName = id - 0x80;
                        currFolder.name = new String(Encoding.GetEncoding("shift_jis").GetChars(br.ReadBytes(lengthName)));
                        currFolder.id = br.ReadUInt16();

                        main.subTable.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                }

                mains.Add(main);
                br.BaseStream.Position = currOffset;
            }

            // Clear previous values
            root = new sFolder();
            accion.Root = new sFolder();

            root = Jerarquizar_Carpetas(mains, 0, "root");
            root.id = number_directories;

            br.Close();

            return root;
        }

        public static sFolder Jerarquizar_Carpetas(List<Estructuras.MainFNT> tables, int idFolder, string nameFolder)
        {
            sFolder currFolder = new sFolder();
            
            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = tables[idFolder & 0xFFF].subTable.files;

            if (tables[idFolder & 0xFFF].subTable.folders is List<sFolder>) // Si tiene carpetas dentro.
           {
                currFolder.folders = new List<sFolder>();

                foreach (sFolder subFolder in tables[idFolder & 0xFFF].subTable.folders)
                    currFolder.folders.Add(Jerarquizar_Carpetas(tables, subFolder.id, subFolder.name));
           }

            return currFolder;
        }

        private static void Obtener_Mains(sFolder currFolder, List<Estructuras.MainFNT> mains, int nTotalMains, ushort parent)
        {
            // Añadimos la carpeta actual al sistema
            Estructuras.MainFNT main = new Estructuras.MainFNT();
            main.offset = (uint)(nTotalMains * 0x08); // 0x08 == Tamaño de un Main sin SubTable
            main.idFirstFile = (ushort)Obtener_FirstID(currFolder);
            main.idParentFolder = parent;
            main.subTable = currFolder;
            mains.Add(main);

            // Seguimos buscando más carpetas
            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Obtener_Mains(subFolder, mains, nTotalMains, currFolder.id);
        }
        private static int Obtener_FirstID(sFolder currFolder)
        {
            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    int id = Obtener_FirstID(currFolder.folders[i]);
                    if (id != -1)
                        return id;
                }
            }

            if (currFolder.files is List<sFile>)
                return currFolder.files[0].id;

            return -1;
        }
    }
}
