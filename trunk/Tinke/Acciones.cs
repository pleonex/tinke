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
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using PluginInterface;

namespace Tinke
{
    /// <summary>
    /// Métodos adicionales para la clase Sistema
    /// </summary>
    public class Acciones
    {
        const byte LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20;

        string file;
        string gameCode;
        sFolder root;
        int idSelect;
        int lastFileId;
        int lastFolderId;
        bool isNewRom;

        IList<IPlugin> formatList;
        IGamePlugin gamePlugin;
        PluginHost pluginHost;

        public Acciones(string file, string name)
        {
            this.file = file;
            gameCode = name.Replace("\0", "");

            formatList = new List<IPlugin>();
            pluginHost = new PluginHost();
            pluginHost.DescompressEvent += new Action<string>(pluginHost_DescomprimirEvent);
            pluginHost.ChangeFile_Event += new Action<int, string>(pluginHost_ChangeFile_Event);
            pluginHost.event_GetDecompressedFiles += new Func<int, sFolder>(pluginHost_event_GetDecompressedFiles);
            pluginHost.event_SearchFile += new Func<int, String>(Save_File);
            pluginHost.event_SearchFile2 += new Func<int, sFile>(Search_File);
            Cargar_Plugins();
        }
        public void Dispose()
        {
            pluginHost.Dispose();
        }

        public void Cargar_Plugins()
        {
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins"))
                return;

            foreach (string fileName in Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins", "*.dll"))
            {
                try
                {

                    if (fileName.EndsWith("PluginInterface.dll"))
                        continue;


                    Assembly assembly = Assembly.LoadFile(fileName);
                    foreach (Type pluginType in assembly.GetTypes())
                    {
                        if (!pluginType.IsPublic || pluginType.IsAbstract || pluginType.IsInterface)
                            continue;

                        Type concreteType = pluginType.GetInterface(typeof(IPlugin).FullName, true);
                        if (concreteType != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(assembly.GetType(pluginType.ToString()));
                            plugin.Initialize(pluginHost);
                            formatList.Add(plugin);

                            break;
                        } // end if
                        else
                        {
                            concreteType = pluginType.GetInterface(typeof(IGamePlugin).FullName, true);
                            if (concreteType != null)
                            {
                                IGamePlugin plugin = (IGamePlugin)Activator.CreateInstance(assembly.GetType(pluginType.ToString()));
                                plugin.Initialize(pluginHost, gameCode);
                                if (plugin.IsCompatible())
                                    gamePlugin = plugin;

                                break;
                            } // end if
                        } //end else
                    } //end foreach
                }
                catch (Exception e)
                {
                    if (e is BadImageFormatException) // The DLL is written in other language
                        continue;

                    MessageBox.Show(String.Format(Tools.Helper.GetTranslation("Messages", "S20"), fileName, e.Message));
                    Console.WriteLine(String.Format(Tools.Helper.GetTranslation("Messages", "S20"), fileName, e.Message));
                    continue;
                }

            } // end foreach

        }
        public void Liberar_Plugins()
        { // IT DOESN'T WORK
            formatList.Clear();
            gamePlugin = null;
        }

        public sFolder Root
        {
            get { return root; }
            set
            {
                root = value;
                Set_LastFileID(root);
                lastFileId++;

                Set_LastFolderID(root);
                lastFolderId++;
            }
        }
        public int IDSelect
        {
            get { return idSelect; }
            set { idSelect = value; }
        }
        public String ROMFile
        {
            get { return file; }
        }
        public int LastFileID
        {
            get { return lastFileId; }
            set { lastFileId = value; }
        }
        public int LastFolderID
        {
            get { return lastFolderId; }
            set { lastFolderId = value; }
        }
        public bool IsNewRom
        {
            get { return isNewRom; }
            set { isNewRom = value; }
        }
        public String TempFolder
        {
            get { return pluginHost.Get_TempFolder(); }
        }

        public int ImageFormatFile(Format name)
        {
            switch (name)
            {
                case Format.Font:
                    return 16;
                case Format.Palette:
                    return 2;
                case Format.Tile:
                    return 3;
                case Format.Map:
                    return 9;
                case Format.Cell:
                    return 8;
                case Format.Animation:
                    return 15;
                case Format.FullImage:
                    return 10;
                case Format.Text:
                    return 4;
                case Format.Compressed:
                    return 5;
                case Format.Sound:
                    return 14;
                case Format.Video:
                    return 13;
                case Format.System:
                    return 20;
                case Format.Script:
                    return 17;
                case Format.Texture:
                    return 21;
                case Format.Model3D:
                    return 22;
                case Format.Pack:
                    return 6;
                case Format.Unknown:
                default:
                    return 1;
            }
        }
        public String Get_RelativePath(int id, string relativePath, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        return relativePath;
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (id == currFolder.folders[i].id)
                        return relativePath;

                    relativePath += '/' + currFolder.folders[i].name;
                    String path = Get_RelativePath(id, relativePath, currFolder.folders[i]);

                    if (path != "")
                        return path;

                    relativePath = relativePath.Remove(relativePath.LastIndexOf('/'));
                }
            }

            return "";
        }

        public void Set_LastFileID(sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id > lastFileId)
                        lastFileId = archivo.id;

            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Set_LastFileID(subFolder);
        }
        public void Set_LastFolderID(sFolder currFolder)
        {
            if (currFolder.id > lastFolderId)
                lastFolderId = currFolder.id;

            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Set_LastFolderID(subFolder);
        }
        public void Delete_PicturesSaved()
        {
            pluginHost.Set_NCLR(new NCLR());
            pluginHost.Set_NCGR(new NCGR());
            pluginHost.Set_NSCR(new NSCR());
            pluginHost.Set_NCER(new NCER());
            pluginHost.Set_NANR(new NANR());
        }
        public void Delete_PicturesSaved(Format formato)
        {
            switch (formato)
            {
                case Format.Palette:
                    pluginHost.Set_NCLR(new NCLR());
                    break;
                case Format.Tile:
                    pluginHost.Set_NCGR(new NCGR());
                    break;
                case Format.Map:
                    pluginHost.Set_NSCR(new NSCR());
                    break;
                case Format.Cell:
                    pluginHost.Set_NCER(new NCER());
                    break;
                case Format.Animation:
                    break;
            }
        }
        public Control Set_PicturesSaved(Format formato)
        {
            sFile selectFile = Select_File();

            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.id + selectFile.name;
            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectFile.size));

            br.BaseStream.Position = selectFile.offset;
            byte[] ext = br.ReadBytes(4);  // First four bytes (Type ID)
            br.Close();

            #region Common image format
            try
            {
                if (formato == Format.Palette)
                {
                    NCLR paleta = Imagen_NCLR.Leer_Basico(tempFile, idSelect);
                    pluginHost.Set_NCLR(paleta);
                    File.Delete(tempFile);

                    iNCLR control = new iNCLR(paleta, pluginHost);
                    control.btnImport.Enabled = false;
                    return control;
                }
                else if (formato == Format.Tile)
                {
                    NCGR tile = Imagen_NCGR.Read_Basic(tempFile, idSelect);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        control.btnImport.Enabled = false;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (formato == Format.Map)
                {
                    NSCR nscr = Imagen_NSCR.Read_Basic(tempFile, idSelect);
                    pluginHost.Set_NSCR(nscr);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCGR().header.file_size != 0x00 || pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), nscr, pluginHost);
                        control.btnImport.Enabled = false;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1C"));
                        return new Control();
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            try { File.Delete(tempFile); }
            catch { }

            return new Control();
        }

        public void Add_Files(ref sFolder files, int id)
        {
            sFile file = Search_File(id); // Compress or Pack file
            files.name = file.name;
            files.id = file.id;
            // Extra info about the compress info in a Folder variable
            files.tag = String.Format("{0:X}", file.size).PadLeft(8, '0') +
                String.Format("{0:X}", file.offset).PadLeft(8, '0') + file.path;

            files = Add_ID(files);  // Set the ID of each file and folder
            Set_Formato(ref files); // Set the format of each file
            root = FileToFolder(root, files); // Convert the pack or compressed file to a folder
        }
        private sFolder Add_ID(sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    sFile currFile = currFolder.files[i];
                    currFile.id = (ushort)lastFileId;
                    currFolder.files.RemoveAt(i);
                    currFolder.files.Insert(i, currFile);
                    lastFileId++;
                }
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    sFolder newFolder = Add_ID(currFolder.folders[i]);
                    newFolder.name = currFolder.folders[i].name;
                    newFolder.id = (ushort)lastFolderId;
                    lastFolderId++;
                    currFolder.folders.RemoveAt(i);
                    currFolder.folders.Insert(i, newFolder);
                }
            }

            return currFolder;
        }
        private void Set_Formato(ref sFolder carpeta)
        {
            if (carpeta.files is List<sFile>)
            {
                for (int i = 0; i < carpeta.files.Count; i++)
                {
                    sFile newFile = carpeta.files[i];
                    newFile.format = Get_Format(newFile, -1);
                    carpeta.files[i] = newFile;
                }
            }


            if (carpeta.folders is List<sFolder>)
            {
                for (int i = 0; i < carpeta.folders.Count; i++)
                {
                    sFolder currFolder = carpeta.folders[i];
                    Set_Formato(ref currFolder);
                    carpeta.folders[i] = currFolder;
                }
            }
        }
        public sFolder FileToFolder(sFolder currFolder, sFolder decompressedFile)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == decompressedFile.id)
                    {
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<sFolder>))
                            currFolder.folders = new List<sFolder>();
                        currFolder.folders.Add(decompressedFile);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    sFolder folder = FileToFolder(currFolder.folders[i], decompressedFile);
                    if (folder.name is string)
                    {
                        currFolder.folders[i] = folder;
                        return currFolder;
                    }
                }
            }

            return new sFolder();
        }

        public sFolder Add_Folder(sFolder folder, ushort idParentFolder, sFolder currFolder)
        {
            if (currFolder.id == idParentFolder)
            {
                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();
                currFolder.folders.Add(folder);

                return currFolder;
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    sFolder subfolder = Add_Folder(folder, idParentFolder, currFolder.folders[i]);
                    if (subfolder.name is string)
                    {
                        currFolder.folders[i] = subfolder;
                        return currFolder;
                    }
                }
            }

            return new sFolder();
        }
        public sFolder Add_Files(sFile[] files, ushort idFolder, sFolder currFolder)
        {
            if (currFolder.id == idFolder)
            {
                currFolder.files.AddRange(files);
                return currFolder;
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    sFolder folder = Add_Files(files, idFolder, currFolder.folders[i]);
                    if (folder.name is string) // Folder found
                    {
                        currFolder.folders[i] = folder;
                        return currFolder;
                    }
                }
            }

            return new sFolder();
        }
        public sFolder Recursive_GetExternalDirectories(string folderPath, sFolder currFolder)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;
                newFile.id = (ushort)lastFileId++;

                if (!(currFolder.files is List<sFile>))
                    currFolder.files = new List<sFile>();
                currFolder.files.Add(newFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                sFolder newFolder = new sFolder();
                newFolder.name = new DirectoryInfo(folder).Name;
                newFolder.id = (ushort)lastFolderId++;
                newFolder = Recursive_GetExternalDirectories(folder, newFolder);

                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();
                currFolder.folders.Add(newFolder);
            }

            return currFolder;
        }
        private void Recursivo_RemoveNullFiles(sFolder carpeta)
        {
            if (carpeta.files is List<sFile>)
            {
                for (int i = 0; i < carpeta.files.Count; i++)
                {
                    if (carpeta.files[i].size == 0x00)
                    {
                        carpeta.files.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (carpeta.folders is List<sFolder>)
                foreach (sFolder subCarpeta in carpeta.folders)
                    Recursivo_RemoveNullFiles(subCarpeta);
        }
        public void Remove_File(int id, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        currFolder.files.RemoveAt(i);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Remove_File(id, subFolder);
        }

        public void Change_Files(sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].tag is String)
                    {
                        // Restore the tag
                        sFile newFile = currFolder.files[i];
                        newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') +
                            String.Format("{0:X}", newFile.offset).PadLeft(8, '0') + newFile.path;
                        currFolder.files[i] = newFile;
                    }
                    Change_File(currFolder.files[i].id, currFolder.files[i], root);
                }
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Change_Files(currFolder.folders[i]);
                }
            }
        }
        public sFolder Change_Folder(sFolder folder, ushort idFolder, sFolder currFolder)
        {
            if (currFolder.id == idFolder)
            {
                currFolder = folder;
                return currFolder;
            }

            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    sFolder subFolder = Change_Folder(folder, idFolder, currFolder.folders[i]);
                    if (subFolder.name is string) // Folder found
                    {
                        currFolder.folders[i] = subFolder;
                        return currFolder;
                    }
                }
            }

            return new sFolder();
        }
        public void Change_File(int id, sFile fileChanged, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        currFolder.files[i] = fileChanged;
                        isNewRom = true;
                    }
                }
            }


            if (currFolder.folders is List<sFolder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (currFolder.folders[i].id == id) // It's a decompressed file
                    {
                        sFolder newFolder = currFolder.folders[i];
                        newFolder.tag = fileChanged.tag;
                        currFolder.folders[i] = newFolder;
                        isNewRom = true;
                    }
                    else
                        Change_File(id, fileChanged, currFolder.folders[i]);
                }
            }
        }
        void pluginHost_ChangeFile_Event(int id, string newFilePath)
        {
            sFile newFile = new sFile();
            sFile oldFile = Search_File(id);
            newFile.name = oldFile.name;
            newFile.id = (ushort)id;
            newFile.offset = 0x00;
            if (ROMFile == "")
            {
                File.Copy(newFilePath, oldFile.path, true);
                newFile.path = oldFile.path;
            }
            else
                newFile.path = newFilePath;
            newFile.format = oldFile.format;
            newFile.size = (uint)new FileInfo(newFilePath).Length;
            newFile.tag = oldFile.tag;
            if ((String)newFile.tag == "Descomprimido") // Set the tag for the folder
                newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') +
                    String.Format("{0:X}", newFile.offset).PadLeft(8, '0') + newFile.path;

            Change_File(id, newFile, root);
        }

        public sFile Select_File()
        {
            return Recursivo_Archivo(idSelect, root);
        }
        public sFile Search_File(int id)
        {
            return Recursivo_Archivo(id, root);
        }
        public sFolder Search_File(string name)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursivo_Archivo(name, root, carpeta);
            return carpeta;
        }
        public sFolder Search_FileName(string name)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursive_FileName(name, root, carpeta);
            return carpeta;
        }
        public sFolder Search_FileName(string name, sFolder folder)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursive_FileName(name, folder, carpeta);
            return carpeta;
        }
        public sFolder Search_File(Format formato)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            carpeta.folders = new List<sFolder>();
            Recursivo_Archivo(formato, root, carpeta);
            return carpeta;
        }
        public sFolder Search_FileLength(int length)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursivo_Archivo(length, root, carpeta);
            return carpeta;
        }
        public sFolder Search_FileOffset(int offset)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursive_FileOffset(offset, root, carpeta);
            return carpeta;
        }
        public sFolder Search_Folder(string name)
        {
            return Recursivo_Carpeta(name, root);
        }
        public sFolder Select_Folder()
        {
            return Recursivo_Carpeta(idSelect, root);
        }
        public sFolder Search_Folder(int id)
        {
            return Recursivo_Carpeta(id, root);
        }

        private sFile Recursivo_Archivo(int id, sFolder currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                sFile folderFile = new sFile();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);
                folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8, 8), 16);
                folderFile.path = ((string)currFolder.tag).Substring(16);
                folderFile.format = Get_Format(folderFile, folderFile.id);
                folderFile.tag = "Descomprimido"; // Tag para indicar que ya ha sido procesado

                return folderFile;
            }

            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = Recursivo_Archivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }
        private sFile Recursivo_Archivo(Format formato, string name, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.format == formato && archivo.name.Contains('.')) // Previene de archivos sin extensión (ie: file1)
                        if (archivo.name.Remove(archivo.name.LastIndexOf('.')) == name)
                            return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = Recursivo_Archivo(formato, name, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }
        private void Recursivo_Archivo(int length, sFolder currFolder, sFolder carpeta)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.size == length)
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursivo_Archivo(length, subFolder, carpeta);

        }
        private void Recursive_FileOffset(int offset, sFolder currFolder, sFolder carpeta)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.offset <= offset && archivo.offset + archivo.size >= offset)
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_FileOffset(offset, subFolder, carpeta);

        }
        private void Recursivo_Archivo(string name, sFolder currFolder, sFolder carpeta)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.name.Contains(name))
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursivo_Archivo(name, subFolder, carpeta);

        }
        private void Recursive_FileName(string name, sFolder currFolder, sFolder carpeta)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.name == name)
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_FileName(name, subFolder, carpeta);

        }
        private void Recursivo_Archivo(Format formato, sFolder currFolder, sFolder carpeta)
        {
            if (currFolder.files is List<sFile>)
            {
                foreach (sFile archivo in currFolder.files)
                {
                    if (archivo.format == formato)
                    {

                        if (formato == Format.Tile || formato == Format.Cell ||
                            formato == Format.Animation || formato == Format.Map)
                        {
                            if (!archivo.name.Contains('.')) // Archivos de nombre desconocido
                                continue;

                            #region Búsqueda de compañeros

                            string name = archivo.name.Remove(archivo.name.LastIndexOf('.'));
                            sFolder fm = new sFolder();
                            fm.name = name;
                            fm.files = new List<sFile>();
                            sFile pal, til, cel;

                            switch (formato)
                            {
                                case Format.Animation:
                                    pal = Recursivo_Archivo(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);
                                    cel = Recursivo_Archivo(Format.Cell, name, root);
                                    if (!(cel.name is String))
                                        goto No_Valido;
                                    fm.files.Add(cel);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Cell:
                                    pal = Recursivo_Archivo(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Map:
                                    pal = Recursivo_Archivo(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Tile:
                                    pal = Recursivo_Archivo(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);

                                    fm.files.Add(archivo);
                                    break;
                            }

                            if (fm.files.Count > 0)
                                carpeta.folders.Add(fm);

                        No_Valido: // No se han encontrado todos los archivos necesario y no se añade
                            continue;
                            #endregion
                        }
                        else
                            carpeta.files.Add(archivo);

                    }
                }
            }


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursivo_Archivo(formato, subFolder, carpeta);

        }
        private sFolder Recursivo_Carpeta(int id, sFolder currFolder)
        {
            if (currFolder.id == id)
                return currFolder;

            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    if (subFolder.id == id)
                        return subFolder;

                    sFolder folder = Recursivo_Carpeta(id, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new sFolder();
        }
        private sFolder Recursivo_Carpeta(string name, sFolder currFolder)
        {
            // Not recommend method to use, can be more than one directory with the same folder
            if (currFolder.name == name)
                return currFolder;

            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    if (subFolder.name == name)
                        return subFolder;

                    sFolder folder = Recursivo_Carpeta(name, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new sFolder();
        }

        public Byte[] Get_MagicID(int id)
        {
            sFile currFile = Search_File(id);
            if (currFile.size < 0x04)
                return null;

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (br.BaseStream.Length < 0x04)
                return null;

            byte[] ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public Byte[] Get_MagicID(sFile currFile)
        {
            if (currFile.size < 0x04)
                return null;

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (br.BaseStream.Length < 0x04)
                return null;

            byte[] ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public Byte[] Get_MagicID(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            if (br.BaseStream.Length < 0x04)
                return null;

            byte[] ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public String Get_MagicIDS(int id)
        {
            sFile currFile = Search_File(id);
            if (currFile.size < 0x04)
                return "";

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (br.BaseStream.Length < 0x04)
                return "";

            byte[] ext = br.ReadBytes(4);
            br.Close();
            if (ext.Length < 4)
                return "";

            string fin = new String(Encoding.ASCII.GetChars(ext));
            if (fin.Length < 4)
                return "";

            for (int i = 0; i < 4; i++)             // En caso de no ser extensión
                if (!Char.IsLetterOrDigit(fin[i]) && fin[i] != ' ')
                    return "";

            return fin;
        }

        public Format Get_Format()
        {
            return Get_Format(idSelect);
        }
        public Format Get_Format(int id)
        {
            Format tipo = Format.Unknown;
            sFile currFile = Search_File(id);
            if (currFile.size == 0x00)
                return Format.Unknown;

            byte[] ext = Get_MagicID(currFile);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Format(currFile.name, ext, id);
                if (tipo != Format.Unknown)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(currFile.name, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Format.Unknown;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Format.Palette;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Format.Tile;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Format.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Format.Cell;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Format.Animation;
            else if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN" ||
                currFile.name.EndsWith(".SRL") || currFile.name.EndsWith(".NDS"))
                return Format.System;


            FileStream fs = File.OpenRead(currFile.path);
            fs.Position = currFile.offset;
            if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
                fs.Seek(4, SeekOrigin.Current);
            if (DSDecmp.Main.Get_Format(fs, (currFile.name == "ARM9.BIN" ? true : false)) != FormatCompress.Invalid)
            {
                fs.Close();
                fs.Dispose();
                return Format.Compressed;
            }
            fs.Close();
            fs.Dispose();

            return Format.Unknown;
        }
        public Format Get_Format(sFile currFile, int id)
        {
            if (currFile.size == 0x00)
                return Format.Unknown;

            Format tipo = Format.Unknown;
            byte[] ext = Get_MagicID(currFile);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Format(currFile.name, ext, id);
                if (tipo != Format.Unknown)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(currFile.name, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Format.Unknown;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Format.Palette;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Format.Tile;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Format.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Format.Cell;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Format.Animation;
            else if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN" ||
                currFile.name.EndsWith(".SRL") || currFile.name.EndsWith(".NDS"))
                return Format.System;

            FileStream fs = File.OpenRead(currFile.path);
            fs.Position = currFile.offset;
            if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
                fs.Seek(4, SeekOrigin.Current);
            if (DSDecmp.Main.Get_Format(fs, (currFile.name == "ARM9.BIN" ? true : false)) != FormatCompress.Invalid)
            {
                fs.Close();
                fs.Dispose();
                return Format.Compressed;
            }
            fs.Close();
            fs.Dispose();

            return Format.Unknown;
        }
        public Format Get_Format(string file)
        {
            if (new FileInfo(file).Length == 0x00)
                return Format.Unknown;

            Format tipo = Format.Unknown;
            string name = new FileInfo(file).Name;
            byte[] ext = Get_MagicID(file);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Format(name, ext, -1);
                if (tipo != Format.Unknown)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(name, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Format.Unknown;
            }
            #endregion

            name = name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Format.Palette;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Format.Tile;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Format.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Format.Cell;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Format.Animation;
            else if (name == "FNT.BIN" || name == "FAT.BIN" || name.StartsWith("OVERLAY9_") || name.StartsWith("OVERLAY7_") ||
                name == "ARM9.BIN" || name == "ARM7.BIN" || name == "Y9.BIN" || name == "Y7.BIN"|| name.EndsWith(".SRL") ||
                name.EndsWith(".NDS"))
                return Format.System;

            if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
            {
                string tempFile2 = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + "c_" + name;
                Byte[] compressFile = new Byte[new FileInfo(file).Length - 4];
                Array.Copy(File.ReadAllBytes(file), 4, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile2, compressFile);
                file = tempFile2;
            }
            if (DSDecmp.Main.Get_Format(file) != FormatCompress.Invalid)
                return Format.Compressed;

            return Format.Unknown;
        }

        public sFolder Unpack()
        {
            return Unpack(idSelect);
        }
        public sFolder Unpack(int id)
        {
            sFile selectedFile = Search_File(id);

            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + id + selectedFile.name;
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            BinaryReader br = new BinaryReader(File.OpenRead(selectedFile.path));
            br.BaseStream.Position = selectedFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectedFile.size));

            br.BaseStream.Position = selectedFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            sFolder desc = new sFolder();
            try
            {
                Format f;
                #region Calling to plugins
                if (gamePlugin is IGamePlugin)
                {
                    f = gamePlugin.Get_Format(selectedFile.name, ext, idSelect);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = gamePlugin.Unpack(tempFile, idSelect);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(selectedFile.name, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(tempFile);
                        goto Continuar;
                    }
                }
                #endregion

                if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77 header
                {
                    string tempFile2 = Path.GetTempPath() + Path.DirectorySeparatorChar + "c_" + selectedFile.id + selectedFile.name;
                    Byte[] compressFile = new Byte[selectedFile.size - 4];
                    Array.Copy(File.ReadAllBytes(tempFile), 4, compressFile, 0, compressFile.Length); ;
                    File.WriteAllBytes(tempFile2, compressFile);
                    tempFile = tempFile2;
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(tempFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    FileInfo info = new FileInfo(tempFile);
                    String uncompFile = info.DirectoryName + Path.DirectorySeparatorChar + "de_" + selectedFile.id + info.Name;
                    DSDecmp.Main.Decompress(tempFile, uncompFile, compressFormat);
                    if (!File.Exists(uncompFile))
                        throw new Exception(Tools.Helper.GetTranslation("Sistema", "S36"));

                    sFile newFile = new sFile();
                    newFile.name = selectedFile.name;
                    newFile.offset = 0x00;
                    newFile.path = uncompFile;
                    newFile.size = (uint)new FileInfo(uncompFile).Length;

                    desc.files = new List<sFile>();
                    desc.files.Add(newFile);
                }
                else
                    return new sFolder();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new sFolder();
            }
        Continuar:

            Add_Files(ref desc, id);
            return desc;
        }
        public sFolder Unpack(String compressedFile, int id)
        {
            byte[] ext = Get_MagicID(compressedFile);
            String name = new FileInfo(compressedFile).Name;

            sFolder desc = new sFolder();
            #region Calling to plugins
            try
            {
                Format f;
                if (gamePlugin is IGamePlugin)
                {
                    f = gamePlugin.Get_Format(name, ext, idSelect);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = gamePlugin.Unpack(compressedFile, idSelect);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(name, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(compressedFile);
                        goto Continuar;
                    }
                }

                if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
                {
                    string tempFile2 = Path.GetTempPath() + Path.DirectorySeparatorChar + "c_" + id + name;
                    Byte[] compressFile = new Byte[new FileInfo(compressedFile).Length - 4];
                    Array.Copy(File.ReadAllBytes(compressedFile), 4, compressFile, 0, compressFile.Length); ;
                    File.WriteAllBytes(tempFile2, compressFile);
                    compressedFile = tempFile2;
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(compressedFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    FileInfo info = new FileInfo(compressedFile);
                    String uncompFile = info.DirectoryName + Path.DirectorySeparatorChar + "de_" + id + info.Name;

                    DSDecmp.Main.Decompress(compressedFile, uncompFile, compressFormat);
                    if (!File.Exists(uncompFile))
                        throw new Exception(Tools.Helper.GetTranslation("Sistema", "S36"));

                    sFile newFile = new sFile();
                    newFile.name = name;
                    newFile.offset = 0x00;
                    newFile.path = uncompFile;
                    newFile.size = (uint)new FileInfo(uncompFile).Length;

                    desc.files = new List<sFile>();
                    desc.files.Add(newFile);
                }
                else
                    return new sFolder();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new sFolder();
            }
            #endregion

        Continuar:
            Add_Files(ref desc, id);    // Añadimos los archivos descomprimidos al árbol de archivos
            return desc;
        }
        void pluginHost_DescomprimirEvent(string arg)
        {
            byte[] ext = Get_MagicID(arg);

            try
            {
                Format f;
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(new FileInfo(arg).Name, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        plugin.Read(arg, -1);
                        goto Continuar;
                    }
                }

                sFolder carpeta = new sFolder();
                FileInfo info = new FileInfo(arg);

                String dec_file = info.DirectoryName + Path.DirectorySeparatorChar + "de" + Path.DirectorySeparatorChar + info.Name;
                Directory.CreateDirectory(info.DirectoryName + Path.DirectorySeparatorChar + "de");

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(arg);
                if (compressFormat != FormatCompress.Invalid)
                    DSDecmp.Main.Decompress(arg, dec_file);

                if (File.Exists(dec_file))
                {
                    sFile file = new sFile();
                    file.name = new FileInfo(arg).Name;
                    file.offset = 0x00;
                    file.path = dec_file;
                    file.size = (uint)new FileInfo(dec_file).Length;

                    carpeta.files = new List<sFile>();
                    carpeta.files.Add(file);
                }
                pluginHost.Set_Files(carpeta);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
        Continuar:
            return;
        }

        sFolder pluginHost_event_GetDecompressedFiles(int id)
        {
            sFolder compresFile = Search_Folder(id);

            sFolder decompressedFiles = new sFolder();
            decompressedFiles.files = new List<sFile>();
            decompressedFiles.folders = new List<sFolder>();
            decompressedFiles.id = 0xF000; // Null value

            Get_DecompressedFiles(compresFile, decompressedFiles);
            Get_LowestID(decompressedFiles, ref decompressedFiles.id);

            decompressedFiles.files.Sort(SortByID);

            return decompressedFiles;
        }
        void Get_DecompressedFiles(sFolder currFolder, sFolder decompressedFiles)
        {
            if (currFolder.files is List<sFile>)
            {
                foreach (sFile archivo in currFolder.files)
                {
                    decompressedFiles.files.Add(archivo);
                }
            }


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFolder currDecompressed = new sFolder();
                    currDecompressed.files = new List<sFile>();
                    currDecompressed.folders = new List<sFolder>();
                    currDecompressed.id = subFolder.id;
                    currDecompressed.name = subFolder.name;

                    if ((String)subFolder.tag !=  "" && subFolder.tag is String) // Decompressed file
                    {
                        sFile file = Search_File(subFolder.id);
                        decompressedFiles.files.Add(file);
                    }
                    else
                    {
                        Get_DecompressedFiles(subFolder, currDecompressed);
                        decompressedFiles.folders.Add(currDecompressed);
                    }
                }
            }
        }
        private int SortByID(sFile f1, sFile f2)
        {
            return f1.id.CompareTo(f2.id);
        }
        void Get_LowestID(sFolder currFolder, ref ushort id)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id < id)
                        id = archivo.id;


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Get_LowestID(subFolder, ref id);
        }
        public void Pack()
        {
            Pack(idSelect);
        }
        public void Pack(int id)
        {
            sFile selectedFile = Search_File(id);
            String original_packfile = Save_File(id);
            Byte[] ext = Get_MagicID(id);

            String packFile;
            sFolder unpacked = pluginHost_event_GetDecompressedFiles(id);

            #region Calling to plugins
            try
            {
                Format f;
                if (gamePlugin is IGamePlugin)
                {
                    f = gamePlugin.Get_Format(selectedFile.name, ext, idSelect);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        packFile = gamePlugin.Pack(ref unpacked, original_packfile, id);
                        goto Continue;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(selectedFile.name, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        packFile = plugin.Pack(ref unpacked, original_packfile);
                        goto Continue;
                    }
                }

                #region Common compression
                String fileToCompress = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectedFile.name;

                if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77 header
                {
                    Save_File(unpacked.files[0], fileToCompress);
                    String compFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectedFile.name;
                    if (File.Exists(compFile))
                        File.Delete(compFile);

                    DSDecmp.Main.Compress(fileToCompress, compFile, FormatCompress.LZ10);
                    if (!File.Exists(compFile))
                        throw new Exception(Tools.Helper.GetTranslation("Sistema", "S43"));

                    // Write the header LZ77
                    String packFilePath = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectedFile.name;
                    BinaryReader br = new BinaryReader(File.OpenRead(compFile));
                    BinaryWriter bw = new BinaryWriter(File.OpenWrite(packFilePath));
                    bw.Write(new char[] { 'L', 'Z', '7', '7' });
                    bw.Write(br.ReadBytes((int)br.BaseStream.Length));
                    bw.Flush();
                    bw.Close();
                    br.Close();
                    File.Delete(compFile);

                    packFile = packFilePath;
                }

                FileStream fs = new FileStream(selectedFile.path, FileMode.OpenOrCreate);
                fs.Seek(selectedFile.offset, SeekOrigin.Begin);
                FormatCompress compressFormat = DSDecmp.Main.Get_Format(fs, false);
                fs.Close();
                fs.Dispose();
                if (compressFormat != FormatCompress.Invalid)
                {
                    Save_File(unpacked.files[0], fileToCompress);
                    String compFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectedFile.name;
                    if (File.Exists(compFile))
                        File.Delete(compFile);

                    DSDecmp.Main.Compress(fileToCompress, compFile, compressFormat);
                    if (!File.Exists(compFile))
                        throw new Exception(Tools.Helper.GetTranslation("Sistema", "S43"));

                    packFile = compFile;
                }
                else
                    return;
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            #endregion

        Continue:
            if (!(packFile is String) || packFile == "")
            {
                MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S23"));
                Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
                return;
            }

            pluginHost_ChangeFile_Event(id, packFile);
            
            Change_Files(unpacked);
        }

        public void Save_File(int id, string outFile)
        {
            sFile selectFile = Search_File(id);

            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)selectFile.size));
            br.Close();
        }
        public String Save_File(int id)
        {
            sFile selectFile = Search_File(id);
            String outFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectFile.name;
            if (File.Exists(outFile))
                File.Delete(outFile);

            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)selectFile.size));
            br.Close();

            return outFile;
        }
        public void Save_File(sFile currfile, string outFile)
        {
            if (File.Exists(outFile))
                File.Delete(outFile);
            if (currfile.size == new FileInfo(currfile.path).Length)
            {
                File.Copy(currfile.path, outFile);
            }
            else
            {
                BinaryReader br = new BinaryReader(File.OpenRead(currfile.path));
                br.BaseStream.Position = currfile.offset;
                File.WriteAllBytes(outFile, br.ReadBytes((int)currfile.size));
                br.Close();
            }
        }


        public Control See_File()
        {
            sFile selectFile = Select_File();
            if (selectFile.name == "rom.nds")
                goto NDS;

            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.id + selectFile.name;
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectFile.size));

            br.BaseStream.Position = selectFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                {
                    if (gamePlugin.Get_Format(selectFile.name, ext, idSelect) != Format.Unknown)
                    {
                        Control resultado = gamePlugin.Show_Info(tempFile, idSelect);
                        return resultado;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Format(selectFile.name, ext) != Format.Unknown)
                    {
                        Control resultado = plugin.Show_Info(tempFile, idSelect);
                        return resultado;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                try { File.Delete(tempFile); }
                catch { }
                return new Control();
            }
            #endregion

            #region Common formats
            try
            {
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                {
                    NCLR nclr = Imagen_NCLR.Leer(tempFile, idSelect);
                    pluginHost.Set_NCLR(nclr);

                    iNCLR control = new iNCLR(nclr, pluginHost);
                    return control; ;
                }
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    NCGR tile = Imagen_NCGR.Read(tempFile, idSelect);
                    pluginHost.Set_NCGR(tile);

                    if (pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Read(tempFile, idSelect));

                    if (pluginHost.Get_NCGR().header.file_size != 0x00 && pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost.Get_NSCR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Read(tempFile, idSelect));

                    if (pluginHost.Get_NCGR().header.file_size != 0x00 && pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCER control = new iNCER(pluginHost.Get_NCER(), pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(tempFile, idSelect));

                    if (pluginHost.Get_NCER().header.file_size != 0x00 && pluginHost.Get_NCGR().header.file_size != 0x00 &&
                        pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNANR control = new iNANR(pluginHost.Get_NCLR(), pluginHost.Get_NCGR(), pluginHost.Get_NCER(), pluginHost.Get_NANR());
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1E"));
                        return new Control();
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(tempFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    Control resultado = new DSDecmp.CompressionControl(idSelect, compressFormat, pluginHost);
                    return resultado;
                }

                if (selectFile.name.ToUpper().EndsWith(".SRL"))
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath, '\"' + tempFile + '\"');
                    return new Control();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }

        NDS:
            if (selectFile.name == "rom.nds")
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath, '\"' + ROMFile + '\"');
                return new Control();
            }
            #endregion

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
            return new Control();
        }
        public Control See_File(String archivo)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            byte[] ext = br.ReadBytes(4);
            br.Close();
            string name = Path.GetFileName(archivo);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                {
                    if (gamePlugin.Get_Format(name, ext, idSelect) != Format.Unknown)
                    {
                        Control resultado = gamePlugin.Show_Info(archivo, idSelect);
                        return resultado;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Format(name, ext) != Format.Unknown)
                    {
                        Control resultado = plugin.Show_Info(archivo, idSelect);
                        return resultado;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                try { File.Delete(archivo); }
                catch { }
                return new Control();
            }
            #endregion

            #region Common form
            try
            {
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                {
                    NCLR nclr = Imagen_NCLR.Leer(archivo, idSelect);
                    pluginHost.Set_NCLR(nclr);

                    iNCLR control = new iNCLR(nclr, pluginHost);
                    control.Dock = DockStyle.Fill;
                    return control; ;
                }
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    NCGR tile = Imagen_NCGR.Read(archivo, idSelect);
                    pluginHost.Set_NCGR(tile);

                    if (pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Read(archivo, idSelect));

                    if (pluginHost.Get_NCGR().header.file_size != 0x00 && pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost.Get_NSCR(), pluginHost);
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Read(archivo, idSelect));

                    if (pluginHost.Get_NCGR().header.file_size != 0x00 && pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNCER control = new iNCER(pluginHost.Get_NCER(), pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost);
                        control.Dock = DockStyle.Fill;

                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(archivo, idSelect));

                    if (pluginHost.Get_NCER().header.file_size != 0x00 && pluginHost.Get_NCGR().header.file_size != 0x00 &&
                        pluginHost.Get_NCLR().header.file_size != 0x00)
                    {
                        iNANR control = new iNANR(pluginHost.Get_NCLR(), pluginHost.Get_NCGR(), pluginHost.Get_NCER(), pluginHost.Get_NANR());
                        control.Dock = DockStyle.Fill;

                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1E"));
                        return new Control();
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(archivo);
                if (compressFormat != FormatCompress.Invalid)
                {
                    Control resultado = new DSDecmp.CompressionControl(idSelect, compressFormat, pluginHost);
                    return resultado;
                }

                if (Path.GetFileName(archivo).ToUpper().EndsWith(".SRL"))
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath, '\"' + archivo + '\"');
                    return new Control();
                }
                else if (Path.GetFileName(archivo).ToUpper().EndsWith(".NDS"))
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath, '\"' + archivo + '\"');
                    return new Control();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
            return new Control();
        }
        public void Read_File()
        {
            sFile selectFile = Select_File();

            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.id + selectFile.name;
            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectFile.size));

            br.BaseStream.Position = selectFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                {
                    if (gamePlugin.Get_Format(selectFile.name, ext, idSelect) != Format.Unknown)
                    {
                        gamePlugin.Read(tempFile, idSelect);
                        File.Delete(tempFile);
                        return;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Format(selectFile.name, ext) != Format.Unknown)
                    {
                        plugin.Read(tempFile, idSelect);
                        File.Delete(tempFile);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            #region Common image format
            try
            {
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                {
                    pluginHost.Set_NCLR(Imagen_NCLR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    pluginHost.Set_NCGR(Imagen_NCGR.Read(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Read(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Read(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }

                if (DSDecmp.Main.Get_Format(tempFile) != FormatCompress.Invalid)
                    MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1A"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            try { File.Delete(tempFile); }
            catch { }
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
        }
    }

}
