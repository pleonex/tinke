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
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Ekona;

namespace Tinke
{
    /// <summary>
    /// Métodos adicionales para la clase Sistema
    /// </summary>
    public class Acciones
    {
        string file;
        string gameCode;
        sFolder root;
        ushort[] sortedIds;
        int idSelect;
        int lastFileId;
        int lastFolderId;
        bool isNewRom;

        List<IPlugin> formatList;
        List<IGamePlugin> gamePlugin;
        PluginHost pluginHost;

        public Acciones(string file, string name)
        {
            this.file = file;
            gameCode = name.Replace("\0", "");

            formatList = new List<IPlugin>();
            gamePlugin = new List<IGamePlugin>();
            pluginHost = new PluginHost();
            pluginHost.DescompressEvent += new Action<string>(pluginHost_DescomprimirEvent);
            pluginHost.ChangeFile_Event += new Action<int, string>(pluginHost_ChangeFile_Event);
            pluginHost.event_GetDecompressedFiles += new Func<int, sFolder>(pluginHost_event_GetDecompressedFiles);
            pluginHost.event_SearchFile += new Func<int, String>(Save_File);
            pluginHost.event_SearchFile2 += new Func<int, sFile>(SearchSave_File);
            pluginHost.event_PluginList += new Func<string[]>(Get_PluginsList);
            pluginHost.event_CallPlugin += new Func<string[],int,int,object>(Call_Plugin);
            pluginHost.event_SearchFolder += new Func<int, sFolder>(Search_Folder);
            pluginHost.event_SearchFileN += new Func<string, sFolder>(Search_FileName);
            Load_Plugins();
        }
        public void Dispose()
        {
            pluginHost.Dispose();
        }

        #region Plugins
        public void Load_Plugins()
        {
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins"))
                return;

            foreach (string fileName in Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins", "*.dll"))
            {
                try
                {

                    if (fileName.EndsWith("Ekona.dll"))
                        continue;


                    Assembly assembly = Assembly.LoadFile(fileName);
                    foreach (Type pluginType in assembly.GetTypes())
                    {
                        if (!pluginType.IsPublic || pluginType.IsAbstract || pluginType.IsInterface)
                            continue;

                        Type concreteType = null;
                        // WORKAROUND: for bug in mono.
                        foreach (Type inter in pluginType.GetInterfaces())
                            if (inter.FullName == typeof(IPlugin).FullName)
                                concreteType = inter;
                        if (concreteType != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(assembly.GetType(pluginType.ToString()));
                            plugin.Initialize(pluginHost);
                            formatList.Add(plugin);
                        } // end if
                        else
                        {
                            // WORKAROUND: for bug in mono.
                            foreach (Type inter in pluginType.GetInterfaces())
                                if (inter.FullName == typeof(IGamePlugin).FullName)
                                    concreteType = inter;
                            if (concreteType != null)
                            {
                                IGamePlugin plugin = (IGamePlugin)Activator.CreateInstance(assembly.GetType(pluginType.ToString()));
                                plugin.Initialize(pluginHost, gameCode);
                                if (plugin.IsCompatible())
                                {
                                    gamePlugin.Add(plugin);
                                    Console.WriteLine("Game plugin loaded: " + assembly.FullName);
                                }
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
        public String[] Get_PluginsList()
        {
            List<String> list = new List<String>();

            for (int i = 0; i < formatList.Count; i++)
                list.Add(formatList[i].ToString());
            if (gamePlugin is IGamePlugin)
                list.Add(gamePlugin.ToString());

            return list.ToArray();
        }
        public Object Call_Plugin(sFile file, string pname, string ext, ushort id, string header, int action)
        {
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Save_File(file, tempFile);

            sFile pfile = new sFile();
            pfile.path = tempFile;
            pfile.size = file.size;
            pfile.offset = 0;
            pfile.name = Path.GetFileNameWithoutExtension(file.name) + '.' + ext;
            pfile.id = (ushort)id;
            pfile.format = Format.Unknown;

            return Call_Plugin(pfile, Encoding.ASCII.GetBytes(header), pname, action);
        }
        public Object Call_Plugin(string file, string pname, string fname, int id, string header, int action)
        {
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + fname;
            File.Copy(file, tempFile, true);

            sFile pfile = new sFile();
            pfile.path = tempFile;
            pfile.offset = 0;
            pfile.name = fname;
            pfile.id = (ushort)id;

            return Call_Plugin(pfile, Encoding.ASCII.GetBytes(header), pname, action);
        }
        public Object Call_Plugin(string[] param, int id, int action)
        {
            return Call_Plugin(param[0], param[1], param[2], id, param[3], action);
        }
        public Object Call_Plugin(sFile file, byte[] magic, string pname, int action)
        {
            foreach (IGamePlugin plugin in gamePlugin)
            {
                if (plugin.ToString() != pname)
                    continue;

                try
                {
                    switch (action)
                    {
                        case 0:
                            plugin.Read(file);
                            return null;

                        case 1:
                            return plugin.Show_Info(file);

                        case 2:
                            sFolder unpacked = plugin.Unpack(file);
                            Add_Files(ref unpacked, file.id);
                            return unpacked;

                        case 3:
                            sFolder unpack = pluginHost_event_GetDecompressedFiles(file.id);
                            String packFile = plugin.Pack(ref unpack, file);

                            if (!(packFile is String) || packFile == "")
                            {
                                MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S23"));
                                Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
                                throw new NotSupportedException();
                            }

                            pluginHost_ChangeFile_Event(file.id, packFile);
                            Change_Files(unpack);
                            return null;

                        case 4:
                            return plugin.Get_Format(file, magic);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S25"));
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            foreach (IPlugin plugin in formatList)
            {
                if (plugin.ToString() != pname)
                    continue;

                try
                {
                    switch (action)
                    {
                        case 0:
                            plugin.Read(file);
                            return null;

                        case 1:
                            return plugin.Show_Info(file);

                        case 2: // Unpack
                            sFolder unpacked = plugin.Unpack(file);
                            Add_Files(ref unpacked, file.id);
                            return unpacked;

                        case 3: // Pack
                            sFolder unpack = pluginHost_event_GetDecompressedFiles(file.id);
                            String packFile = plugin.Pack(ref unpack, file);

                            if (!(packFile is String) || packFile == "")
                            {
                                MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S23"));
                                Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
                                throw new NotSupportedException();
                            }

                            pluginHost_ChangeFile_Event(file.id, packFile);
                            Change_Files(unpack);
                            return null;

                        case 4: // Get format
                            return plugin.Get_Format(file, magic);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S25"));
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            Console.WriteLine("Plugin not found");
            return null;
        }

        public string Get_TempFolder()
        {
            return pluginHost.Get_TempFolder();
        }
        public void Set_TempFolder(string newPath)
        {
            pluginHost.Set_TempFolder(newPath);
        }
        public void Restore_TempFolder()
        {
            pluginHost.Restore_TempFolder();
        }
        #endregion

        #region Properties
        public sFolder Root
        {
            get { return root; }
            set { root = value; }
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
        public ushort[] SortedIDs
        {
            get { return sortedIds; }
            set { sortedIds = value; }
        }
        #endregion

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

        #region Add, remove and change files methods
        public void Add_Files(ref sFolder files, int id)
        {
            sFile file = Search_File(id); // Compress or Pack file
            files.name = file.name;
            files.id = file.id;
            // Extra info about the compress info in a Folder variable
            files.tag = String.Format("{0:X}", file.size).PadLeft(8, '0') +
                String.Format("{0:X}", file.offset).PadLeft(8, '0') + file.path;

            files = Add_ID(files);  // Set the ID of each file and folder
            Set_Format(ref files); // Set the format of each file
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
        private void Set_Format(ref sFolder folder)
        {
            if (folder.files is List<sFile>)
            {
                for (int i = 0; i < folder.files.Count; i++)
                {
                    sFile newFile = folder.files[i];
                    newFile.format = Get_Format(newFile);
                    folder.files[i] = newFile;
                }
            }


            if (folder.folders is List<sFolder>)
            {
                for (int i = 0; i < folder.folders.Count; i++)
                {
                    sFolder currFolder = folder.folders[i];
                    Set_Format(ref currFolder);
                    folder.folders[i] = currFolder;
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
        private void Recursive_RemoveNullFiles(sFolder folder)
        {
            if (folder.files is List<sFile>)
            {
                for (int i = 0; i < folder.files.Count; i++)
                {
                    if (folder.files[i].size == 0x00)
                    {
                        folder.files.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (folder.folders is List<sFolder>)
                foreach (sFolder subCarpeta in folder.folders)
                    Recursive_RemoveNullFiles(subCarpeta);
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
        public void Change_File(int id, string newFilePath)
        {
            pluginHost_ChangeFile_Event(id, newFilePath);
        }
        void pluginHost_ChangeFile_Event(int id, string newFilePath)
        {
            sFile newFile = new sFile();
            sFile oldFile = Search_File(id);
            newFile.name = oldFile.name;
            newFile.id = (ushort)id;
            newFile.offset = 0x00;
            if (ROMFile == "" && (oldFile.offset == 0x00 && new FileInfo(oldFile.path).Length == oldFile.size))
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
        #endregion

        #region Search methods
        public sFile Selected_File()
        {
            return Recursive_File(idSelect, root);
        }
        public sFile Search_File(int id)
        {
            return Recursive_File(id, root);
        }
        public sFile SearchSave_File(int id)
        {
            sFile cfile = Recursive_File(id, root);
            cfile.path = Save_File(cfile);
            cfile.offset = 0;
            return cfile;
        }
        public sFolder Search_File(string name)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            Recursive_File(name, root, carpeta);
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
        public sFolder Search_File(Format format)
        {
            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            carpeta.folders = new List<sFolder>();
            Recursive_File(format, root, carpeta);
            return carpeta;
        }
        public sFolder Search_FileLength(int length)
        {
            sFolder folder = new sFolder();
            folder.files = new List<sFile>();
            Recursive_File(length, root, folder);
            return folder;
        }
        public sFolder Search_File(byte[] header)
        {
            sFolder folder = new sFolder();
            folder.files = new List<sFile>();
            Recursive_File(header, root, folder);
            return folder;
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
            return Recursive_Folder(name, root);
        }
        public sFolder Selected_Folder()
        {
            return Recursive_Folder(idSelect, root);
        }
        public sFolder Search_Folder(int id)
        {
            return Recursive_Folder(id, root);
        }
        #endregion

        #region Recursive methods
        private sFile Recursive_File(int id, sFolder currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                sFile folderFile = new sFile();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);
                folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8, 8), 16);
                folderFile.path = ((string)currFolder.tag).Substring(16);
                folderFile.format = Get_Format(folderFile);
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
                    sFile currFile = Recursive_File(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }
        private sFile Recursive_File(Format format, string name, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.format == format && archivo.name.Contains('.')) // Previene de archivos sin extensión (ie: file1)
                        if (archivo.name.Remove(archivo.name.LastIndexOf('.')) == name)
                            return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = Recursive_File(format, name, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }
        private void Recursive_File(int length, sFolder currFolder, sFolder folder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.size == length)
                        folder.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_File(length, subFolder, folder);

        }
        private void Recursive_FileOffset(int offset, sFolder currFolder, sFolder folder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.offset <= offset && archivo.offset + archivo.size >= offset)
                        folder.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_FileOffset(offset, subFolder, folder);

        }
        private void Recursive_File(string name, sFolder currFolder, sFolder folder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.name.ToLower().Contains(name.ToLower()))
                        folder.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_File(name, subFolder, folder);

        }
        private void Recursive_FileName(string name, sFolder currFolder, sFolder folder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.name == name)
                        folder.files.Add(archivo);


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_FileName(name, subFolder, folder);

        }
        private void Recursive_File(Format formato, sFolder currFolder, sFolder folder)
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
                                    pal = Recursive_File(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursive_File(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);
                                    cel = Recursive_File(Format.Cell, name, root);
                                    if (!(cel.name is String))
                                        goto No_Valido;
                                    fm.files.Add(cel);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Cell:
                                    pal = Recursive_File(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursive_File(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Map:
                                    pal = Recursive_File(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursive_File(Format.Tile, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Format.Tile:
                                    pal = Recursive_File(Format.Palette, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);

                                    fm.files.Add(archivo);
                                    break;
                            }

                            if (fm.files.Count > 0)
                                folder.folders.Add(fm);

                        No_Valido: // No se han encontrado todos los archivos necesario y no se añade
                            continue;
                            #endregion
                        }
                        else
                            folder.files.Add(archivo);

                    }
                }
            }


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_File(formato, subFolder, folder);

        }
        private void Recursive_File(byte[] header, sFolder currFolder, sFolder folder)
        {
            if (currFolder.files is List<sFile>)
            {
                foreach (sFile currFile in currFolder.files)
                {
                    byte[] ext = Get_MagicID(currFile, header.Length);
                    if (ext.Length != header.Length)
                        continue;

                    bool equal = true;
                    for (int i = 0; i < ext.Length; i++)
                    {
                        if (ext[i] != header[i])
                        {
                            equal = false;
                            break;
                        }
                    }

                    if (equal)
                        folder.files.Add(currFile);
                }
            }

            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Recursive_File(header, subFolder, folder);
        }
        private sFolder Recursive_Folder(int id, sFolder currFolder)
        {
            if (currFolder.id == id)
                return currFolder;

            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    if (subFolder.id == id)
                        return subFolder;

                    sFolder folder = Recursive_Folder(id, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new sFolder();
        }
        private sFolder Recursive_Folder(string name, sFolder currFolder)
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

                    sFolder folder = Recursive_Folder(name, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new sFolder();
        }
        #endregion

        #region Get header
        public Byte[] Get_MagicID(int id)
        {
            sFile currFile = Search_File(id);

            byte[] ext;
            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (currFile.size < 0x04)
                ext = br.ReadBytes((int)currFile.size);
            else
                ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public Byte[] Get_MagicID(sFile currFile)
        {
            byte[] ext;

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (currFile.size < 0x04)
                ext = br.ReadBytes((int)currFile.size);
            else
                ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public Byte[] Get_MagicID(sFile currFile, int length)
        {
            byte[] ext;

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (currFile.size < length)
                ext = br.ReadBytes((int)currFile.size);
            else
                ext = br.ReadBytes(length);
            br.Close();

            return ext;
        }
        public Byte[] Get_MagicID(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Byte[] ext;

            if (br.BaseStream.Length < 0x04)
                ext = br.ReadBytes((int)br.BaseStream.Length);
            else
                ext = br.ReadBytes(4);
            br.Close();

            return ext;
        }
        public String Get_MagicIDS(sFile currFile)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            byte[] ext;
            if (currFile.size < 0x04)
                ext = br.ReadBytes((int)currFile.size);
            else
                ext = br.ReadBytes(4);
            br.Close();

            string fin = new String(Encoding.ASCII.GetChars(ext));

            for (int i = 0; i < fin.Length; i++)
                if ((byte)fin[i] < 0x20 || (byte)fin[i] > 0x7D || fin[i] == '?')   // ASCII chars
                    return "";

            return fin;
        }
        public String Get_MagicIDS(Stream stream, uint size)
        {
            Byte[] buffer = new byte[4];
            if (size < 0x04)
            {
                buffer = new byte[size];
                stream.Read(buffer, 0, (int)size);
            }
            else
                stream.Read(buffer, 0, 4);

            string ext = new String(Encoding.ASCII.GetChars(buffer));

            for (int i = 0; i < ext.Length; i++)
                if ((byte)ext[i] < 0x20 || (byte)ext[i] > 0x7D || ext[i] == '?')   // ASCII chars
                    return "";

            return ext;
        }
        #endregion

        #region Get format
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
                foreach (IGamePlugin format in gamePlugin)
                {
                    tipo = format.Get_Format(currFile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }

                foreach (IPlugin format in formatList)
                {
                    tipo = format.Get_Format(currFile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return Format.Unknown;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN" || currFile.name == "BANNER.BIN" ||
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
        public Format Get_Format(sFile currFile)
        {
            if (currFile.size == 0x00)
                return Format.Unknown;

            Format tipo = Format.Unknown;
            byte[] ext = Get_MagicID(currFile);

            #region Calling to plugins
            try
            {
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    tipo = plugin.Get_Format(currFile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(currFile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return Format.Unknown;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (currFile.name.EndsWith(".SRL") || currFile.name.EndsWith(".NDS"))
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

            if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN" || currFile.name == "BANNER.BIN")
                return Format.System;


            return Format.Unknown;
        }
        public Format Get_Format(string file)
        {
            sFile cfile = new sFile();
            cfile.name = Path.GetFileName(file);
            cfile.path = file;
            cfile.offset = 0;
            cfile.size = (uint)new FileInfo(file).Length;
            if (cfile.size == 0x00)
                return Format.Unknown;

            Format tipo = Format.Unknown;
            byte[] ext = Get_MagicID(file);

            #region Calling to plugins
            try
            {
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    tipo = plugin.Get_Format(cfile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(cfile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return Format.Unknown;
            }
            #endregion

            cfile.name = cfile.name.ToUpper();
            if (cfile.name == "FNT.BIN" || cfile.name == "FAT.BIN" || cfile.name.StartsWith("OVERLAY9_") || cfile.name.StartsWith("OVERLAY7_") ||
                cfile.name == "ARM9.BIN" || cfile.name == "ARM7.BIN" || cfile.name == "Y9.BIN" || cfile.name == "Y7.BIN" || cfile.name.EndsWith(".SRL") ||
                cfile.name.EndsWith(".NDS") || cfile.name == "BANNER.BIN")
                return Format.System;

            if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
            {
                string tempFile2 = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + "c_" + cfile.name;
                Byte[] compressFile = new Byte[new FileInfo(file).Length - 4];
                Array.Copy(File.ReadAllBytes(file), 4, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile2, compressFile);
                file = tempFile2;
            }
            if (DSDecmp.Main.Get_Format(file) != FormatCompress.Invalid)
                return Format.Compressed;

            return Format.Unknown;
        }
        public Format Get_Format(Stream stream, string name, int id, uint size)
        {
            if (size == 0x00)
                return Format.Unknown;

            sFile cfile = new sFile();
            cfile.name = name;
            cfile.id = (ushort)id;
            cfile.size = size;
            cfile.offset = (uint)stream.Position;
            cfile.path = file;                      // When it's called it pass the stream of the rom file.

            Format tipo = Format.Unknown;
            // Extension
            Byte[] ext = new byte[4];
            if (size < 0x04)
            {
                ext = new byte[size];
                stream.Read(ext, 0, (int)size);
                stream.Position -= size;
            }
            else
            {
                stream.Read(ext, 0, 4);
                stream.Position -= 4;
            }

            #region Calling to plugins
            try
            {
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    tipo = plugin.Get_Format(cfile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Format(cfile, ext);
                    if (tipo != Format.Unknown)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return Format.Unknown;
            }
            #endregion

            name = name.ToUpper();
            if (name.EndsWith(".SRL") || name.EndsWith(".NDS"))
                return Format.System;

            FileStream fs = (FileStream)stream;
            if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
                fs.Seek(4, SeekOrigin.Current);
            if (DSDecmp.Main.Get_Format(fs, (name == "ARM9.BIN" ? true : false)) != FormatCompress.Invalid)
                return Format.Compressed;

            if (name == "FNT.BIN" || name == "FAT.BIN" || name.StartsWith("OVERLAY9_") || name.StartsWith("OVERLAY7_") ||
                name == "ARM9.BIN" || name == "ARM7.BIN" || name == "Y9.BIN" || name == "Y7.BIN" || name == "BANNER.BIN")
                return Format.System;

            return Format.Unknown;
        }
        #endregion

        #region Pack / Unpack methods
        public sFolder Unpack()
        {
            return Unpack(idSelect);
        }
        public sFolder Unpack(int id)
        {
            sFile selectedFile = Search_File(id);

            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectedFile.name;
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            BinaryReader br = new BinaryReader(File.OpenRead(selectedFile.path));
            br.BaseStream.Position = selectedFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectedFile.size));

            br.BaseStream.Position = selectedFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            sFile cfile = selectedFile;
            cfile.path = tempFile;
            cfile.offset = 0;

            sFolder desc = new sFolder();
            try
            {
                Format f;
                #region Calling to plugins
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(cfile);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(cfile);
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
                Console.WriteLine(e.StackTrace);
                return new sFolder();
            }
        Continuar:

            Add_Files(ref desc, id);
            return desc;
        }
        public sFolder Unpack(String compressedFile, int id)
        {
            sFile cfile = new sFile();
            cfile.id = (ushort)id;
            cfile.name = Path.GetFileName(compressedFile);
            cfile.offset = 0;
            cfile.path = compressedFile;
            cfile.size = (uint)new FileInfo(compressedFile).Length;

            byte[] ext = Get_MagicID(compressedFile);

            sFolder desc = new sFolder();
            #region Calling to plugins
            try
            {
                Format f;
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(cfile);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        desc = plugin.Unpack(cfile);
                        goto Continuar;
                    }
                }

                if (new String(Encoding.ASCII.GetChars(ext)) == "LZ77") // LZ77
                {
                    string tempFile2 = Path.GetTempPath() + Path.DirectorySeparatorChar + "c_" + id + cfile.name;
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
                    newFile.name = cfile.name;
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
            sFile cfile = new sFile();
            cfile.id = 0;
            cfile.name = Path.GetFileName(arg);
            cfile.offset = 0;
            cfile.path = arg;
            cfile.size = (uint)new FileInfo(arg).Length;

            byte[] ext = Get_MagicID(arg);

            try
            {
                Format f;
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        pluginHost.Set_Files(plugin.Unpack(cfile));
                        return;
                    }
                }

                sFolder carpeta = new sFolder();
                FileInfo info = new FileInfo(arg);

                String dec_file = info.DirectoryName + Path.DirectorySeparatorChar + "de" + Path.DirectorySeparatorChar + info.Name;
                Directory.CreateDirectory(info.DirectoryName + Path.DirectorySeparatorChar + "de");

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(arg);
                if (compressFormat != FormatCompress.Invalid)
                    DSDecmp.Main.Decompress(arg, dec_file, compressFormat);

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

                    if ((String)subFolder.tag != "" && subFolder.tag is String) // Decompressed file
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

            sFile cfile = selectedFile;
            cfile.offset = 0;
            cfile.path = original_packfile;

            String packFile;
            sFolder unpacked = pluginHost_event_GetDecompressedFiles(id);

            #region Calling to plugins
            try
            {
                Format f;
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        packFile = plugin.Pack(ref unpacked, cfile);
                        goto Continue;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Format(cfile, ext);
                    if (f == Format.Compressed || f == Format.Pack)
                    {
                        packFile = plugin.Pack(ref unpacked, cfile);
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
        #endregion

        public Control See_File()
        {
            sFile selectFile = Selected_File();
            if (selectFile.name == "rom.nds")
                goto NDS;

            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + selectFile.name;
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectFile.size));

            sFile cfile = selectFile;
            cfile.offset = 0;
            cfile.path = tempFile;

            br.BaseStream.Position = selectFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            GC.Collect();

            #region Calling to plugins
            try
            {
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    if (plugin.Get_Format(cfile, ext) != Format.Unknown)
                    {
                        Control resultado = plugin.Show_Info(cfile);
                        return resultado;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Format(cfile, ext) != Format.Unknown)
                    {
                        Control resultado = plugin.Show_Info(cfile);
                        return resultado;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                try { File.Delete(tempFile); }
                catch { }
                return new Control();
            }
            #endregion

            #region Common formats
            try
            {
                FormatCompress compressFormat = DSDecmp.Main.Get_Format(tempFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    Control resultado = new DSDecmp.CompressionControl(idSelect, compressFormat, pluginHost);
                    return resultado;
                }

                if (selectFile.name.ToUpper().EndsWith(".SRL") || selectFile.name.ToUpper().EndsWith(".NDS"))
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

        public void Read_File()
        {
            Read_File(Selected_File());
        }
        public void Read_File(sFile currfile)
        {
            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + currfile.name;
            BinaryReader br = new BinaryReader(File.OpenRead(currfile.path));
            br.BaseStream.Position = currfile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)currfile.size));

            sFile cfile = currfile;
            cfile.path = tempFile;
            cfile.offset = 0;

            br.BaseStream.Position = currfile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            #region Calling to plugins
            try
            {
                foreach (IGamePlugin plugin in gamePlugin)
                {
                    if (plugin.Get_Format(cfile, ext) != Format.Unknown)
                    {
                        plugin.Read(cfile);
                        File.Delete(tempFile);
                        GC.Collect();
                        return;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Format(cfile, ext) != Format.Unknown)
                    {
                        plugin.Read(cfile);
                        File.Delete(tempFile);
                        GC.Collect();
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

            try { File.Delete(tempFile); }
            catch { }
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S25"));
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
        public String Save_File(sFile file)
        {
            String outFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + file.name;
            if (File.Exists(outFile))
                File.Delete(outFile);

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = file.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)file.size));
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
    }

}
