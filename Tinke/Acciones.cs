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
        Carpeta root;
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
            pluginHost.DescomprimirEvent += new Action<string>(pluginHost_DescomprimirEvent);
            pluginHost.ChangeFile_Event += new Action<int, string>(pluginHost_ChangeFile_Event);
            pluginHost.event_GetDecompressedFiles += new Func<int, Carpeta>(pluginHost_event_GetDecompressedFiles);
            pluginHost.event_SearchFile += new Func<int, String>(Save_File);
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
                            plugin.Inicializar(pluginHost);
                            formatList.Add(plugin);

                            break;
                        } // end if
                        else
                        {
                            concreteType = pluginType.GetInterface(typeof(IGamePlugin).FullName, true);
                            if (concreteType != null)
                            {
                                IGamePlugin plugin = (IGamePlugin)Activator.CreateInstance(assembly.GetType(pluginType.ToString()));
                                plugin.Inicializar(pluginHost, gameCode);
                                if (plugin.EsCompatible())
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

                    MessageBox.Show(String.Format(Tools.Helper.ObtenerTraduccion("Messages", "S20"), fileName, e.Message));
                    Console.WriteLine(String.Format(Tools.Helper.ObtenerTraduccion("Messages", "S20"), fileName, e.Message));
                    continue;
                }

            } // end foreach

        }
        public void Liberar_Plugins()
        {
            formatList.Clear();
            gamePlugin = null;
        }

        public Carpeta Root
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

        public int ImageFormatFile(Formato name)
        {
            switch (name)
            {
                case Formato.Fuentes:
                    return 16;
                case Formato.Paleta:
                    return 2;
                case Formato.Imagen:
                    return 3;
                case Formato.Map:
                    return 9;
                case Formato.Celdas:
                    return 8;
                case Formato.Animación:
                    return 15;
                case Formato.ImagenCompleta:
                    return 10;
                case Formato.Texto:
                    return 4;
                case Formato.Comprimido:
                    return 5;
                case Formato.Sonido:
                    return 14;
                case Formato.Video:
                    return 13;
                case Formato.Sistema:
                    return 20;
                case Formato.Script:
                    return 17;
                case Formato.Texture:
                    return 21;
                case Formato.Model3D:
                    return 22;
                case Formato.Pack:
                    return 6;
                case Formato.Desconocido:
                default:
                    return 1;
            }
        }
        public String Get_RelativePath(int id, string relativePath, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        return relativePath;
            }

            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    relativePath += '/' + currFolder.folders[i].name;
                    String path = Get_RelativePath(id, relativePath, currFolder.folders[i]);

                    if (path != "")
                        return path;

                    relativePath = relativePath.Remove(relativePath.LastIndexOf('/'));
                }
            }

            return "";
        }

        public void Set_LastFileID(Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.id > lastFileId)
                        lastFileId = archivo.id;

            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Set_LastFileID(subFolder);
        }
        public void Set_LastFolderID(Carpeta currFolder)
        {
            if (currFolder.id > lastFolderId)
                lastFolderId = currFolder.id;

            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
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
        public void Delete_PicturesSaved(Formato formato)
        {
            switch (formato)
            {
                case Formato.Paleta:
                    pluginHost.Set_NCLR(new NCLR());
                    break;
                case Formato.Imagen:
                    pluginHost.Set_NCGR(new NCGR());
                    break;
                case Formato.Map:
                    pluginHost.Set_NSCR(new NSCR());
                    break;
                case Formato.Celdas:
                    pluginHost.Set_NCER(new NCER());
                    break;
                case Formato.Animación:
                    break;
            }
        }
        public Control Set_PicturesSaved(Formato formato)
        {
            Archivo selectFile = Select_File();

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
                if (formato == Formato.Paleta)
                {
                    NCLR paleta = Imagen_NCLR.Leer_Basico(tempFile, idSelect);
                    pluginHost.Set_NCLR(paleta);
                    File.Delete(tempFile);

                    iNCLR control = new iNCLR(paleta, pluginHost);
                    control.btnImport.Enabled = false;
                    return control;
                }
                else if (formato == Formato.Imagen)
                {
                    NCGR tile = Imagen_NCGR.Leer_Basico(tempFile, idSelect);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        control.btnImport.Enabled = false;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (formato == Formato.Map)
                {
                    NSCR nscr = Imagen_NSCR.Leer_Basico(tempFile, idSelect);
                    pluginHost.Set_NSCR(nscr);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 || pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), nscr, pluginHost);
                        control.btnImport.Enabled = false;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1C"));
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

        public void Add_Files(ref Carpeta files, int id)
        {
            Archivo file = Search_File(id); // Compress or Pack file
            files.name = file.name;
            files.id = file.id;
            // Extra info about the compress info in a Folder variable
            files.tag = String.Format("{0:X}", file.size).PadLeft(8, '0') +
                String.Format("{0:X}", file.offset).PadLeft(8, '0') + file.path;

            files = Add_ID(files);  // Set the ID of each file and folder
            Set_Formato(ref files); // Set the format of each file
            root = FileToFolder(root, files); // Convert the pack or compressed file to a folder
        }
        private Carpeta Add_ID(Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    Archivo currFile = currFolder.files[i];
                    currFile.id = (ushort)lastFileId;
                    currFolder.files.RemoveAt(i);
                    currFolder.files.Insert(i, currFile);
                    lastFileId++;
                }
            }

            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Carpeta newFolder = Add_ID(currFolder.folders[i]);
                    newFolder.name = currFolder.folders[i].name;
                    newFolder.id = (ushort)lastFolderId;
                    lastFolderId++;
                    currFolder.folders.RemoveAt(i);
                    currFolder.folders.Insert(i, newFolder);
                }
            }

            return currFolder;
        }
        private void Set_Formato(ref Carpeta carpeta)
        {
            if (carpeta.files is List<Archivo>)
            {
                for (int i = 0; i < carpeta.files.Count; i++)
                {
                    Archivo newFile = carpeta.files[i];
                    newFile.formato = Get_Formato(newFile, -1);
                    carpeta.files[i] = newFile;
                }
            }


            if (carpeta.folders is List<Carpeta>)
            {
                for (int i = 0; i < carpeta.folders.Count; i++)
                {
                    Carpeta currFolder = carpeta.folders[i];
                    Set_Formato(ref currFolder);
                    carpeta.folders[i] = currFolder;
                }
            }
        }
        public Carpeta FileToFolder(Carpeta currFolder, Carpeta decompressedFile)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == decompressedFile.id)
                    {
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Carpeta>))
                            currFolder.folders = new List<Carpeta>();
                        currFolder.folders.Add(decompressedFile);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Carpeta folder = FileToFolder(currFolder.folders[i], decompressedFile);
                    if (folder.name is string)
                    {
                        currFolder.folders[i] = folder;
                        return currFolder;
                    }
                }
            }

            return new Carpeta();
        }

        public Carpeta Add_Folder(Carpeta folder, ushort idParentFolder, Carpeta currFolder)
        {
            if (currFolder.id == idParentFolder)
            {
                if (!(currFolder.folders is List<Carpeta>))
                    currFolder.folders = new List<Carpeta>();
                currFolder.folders.Add(folder);

                return currFolder;
            }

            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Carpeta subfolder = Add_Folder(folder, idParentFolder, currFolder.folders[i]);
                    if (subfolder.name is string)
                    {
                        currFolder.folders[i] = subfolder;
                        return currFolder;
                    }
                }
            }

            return new Carpeta();
        }
        public Carpeta Add_Files(Archivo[] files, ushort idFolder, Carpeta currFolder)
        {
            if (currFolder.id == idFolder)
            {
                currFolder.files.AddRange(files);
                return currFolder;
            }

            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Carpeta folder = Add_Files(files, idFolder, currFolder.folders[i]);
                    if (folder.name is string) // Folder found
                    {
                        currFolder.folders[i] = folder;
                        return currFolder;
                    }
                }
            }

            return new Carpeta();
        }
        public Carpeta Recursive_GetExternalDirectories(string folderPath, Carpeta currFolder)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                Archivo newFile = new Archivo();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;
                newFile.id = (ushort)lastFileId++;

                if (!(currFolder.files is List<Archivo>))
                    currFolder.files = new List<Archivo>();
                currFolder.files.Add(newFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                Carpeta newFolder = new Carpeta();
                newFolder.name = new DirectoryInfo(folder).Name;
                newFolder.id = (ushort)lastFolderId++;
                newFolder = Recursive_GetExternalDirectories(folder, newFolder);

                if (!(currFolder.folders is List<Carpeta>))
                    currFolder.folders = new List<Carpeta>();
                currFolder.folders.Add(newFolder);
            }

            return currFolder;
        }
        private void Recursivo_RemoveNullFiles(Carpeta carpeta)
        {
            if (carpeta.files is List<Archivo>)
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

            if (carpeta.folders is List<Carpeta>)
                foreach (Carpeta subCarpeta in carpeta.folders)
                    Recursivo_RemoveNullFiles(subCarpeta);
        }
        public void Remove_File(int id, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        currFolder.files.RemoveAt(i);


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Remove_File(id, subFolder);
        }

        public Carpeta Change_Folder(Carpeta folder, ushort idFolder, Carpeta currFolder)
        {
            if (currFolder.id == idFolder)
            {
                currFolder = folder;
                return currFolder;
            }

            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Carpeta subFolder = Change_Folder(folder, idFolder, currFolder.folders[i]);
                    if (subFolder.name is string) // Folder found
                    {
                        currFolder.folders[i] = subFolder;
                        return currFolder;
                    }
                }
            }

            return new Carpeta();
        }
        public void Change_File(int id, Archivo fileChanged, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
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


            if (currFolder.folders is List<Carpeta>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (currFolder.folders[i].id == id) // It's a decompressed file
                    {
                        Carpeta newFolder = currFolder.folders[i];
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
            Archivo newFile = new Archivo();
            Archivo oldFile = Search_File(id);
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
            newFile.formato = oldFile.formato;
            newFile.size = (uint)new FileInfo(newFilePath).Length;
            newFile.tag = oldFile.tag;
            if ((String)newFile.tag == "Descomprimido") // Set the tag for the folder
                newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') +
                    String.Format("{0:X}", newFile.offset).PadLeft(8, '0') + newFile.path;

            Change_File(id, newFile, root);
        }

        public Archivo Select_File()
        {
            return Recursivo_Archivo(idSelect, root);
        }
        public Archivo Search_File(int id)
        {
            return Recursivo_Archivo(id, root);
        }
        public Carpeta Search_File(string name)
        {
            Carpeta carpeta = new Carpeta();
            carpeta.files = new List<Archivo>();
            Recursivo_Archivo(name, root, carpeta);
            return carpeta;
        }
        public Carpeta Search_FileName(string name)
        {
            Carpeta carpeta = new Carpeta();
            carpeta.files = new List<Archivo>();
            Recursive_FileName(name, root, carpeta);
            return carpeta;
        }
        public Carpeta Search_FileName(string name, Carpeta folder)
        {
            Carpeta carpeta = new Carpeta();
            carpeta.files = new List<Archivo>();
            Recursive_FileName(name, folder, carpeta);
            return carpeta;
        }
        public Carpeta Search_File(Formato formato)
        {
            Carpeta carpeta = new Carpeta();
            carpeta.files = new List<Archivo>();
            carpeta.folders = new List<Carpeta>();
            Recursivo_Archivo(formato, root, carpeta);
            return carpeta;
        }
        public Carpeta Search_FileLength(int length)
        {
            Carpeta carpeta = new Carpeta();
            carpeta.files = new List<Archivo>();
            Recursivo_Archivo(length, root, carpeta);
            return carpeta;
        }
        public Carpeta Search_Folder(string name)
        {
            return Recursivo_Carpeta(name, root);
        }
        public Carpeta Select_Folder()
        {
            return Recursivo_Carpeta(idSelect, root);
        }
        public Carpeta Search_Folder(int id)
        {
            return Recursivo_Carpeta(id, root);
        }

        private Archivo Recursivo_Archivo(int id, Carpeta currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                Archivo folderFile = new Archivo();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);
                folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8, 8), 16);
                folderFile.path = ((string)currFolder.tag).Substring(16);
                folderFile.formato = Get_Formato(folderFile, folderFile.id);
                folderFile.tag = "Descomprimido"; // Tag para indicar que ya ha sido procesado

                return folderFile;
            }

            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Archivo currFile = Recursivo_Archivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new Archivo();
        }
        private Archivo Recursivo_Archivo(Formato formato, string name, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.formato == formato && archivo.name.Contains('.')) // Previene de archivos sin extensión (ie: file1)
                        if (archivo.name.Remove(archivo.name.LastIndexOf('.')) == name)
                            return archivo;


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Archivo currFile = Recursivo_Archivo(formato, name, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new Archivo();
        }
        private void Recursivo_Archivo(int length, Carpeta currFolder, Carpeta carpeta)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.size == length)
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Recursivo_Archivo(length, subFolder, carpeta);

        }
        private void Recursivo_Archivo(string name, Carpeta currFolder, Carpeta carpeta)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.name.Contains(name))
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Recursivo_Archivo(name, subFolder, carpeta);

        }
        private void Recursive_FileName(string name, Carpeta currFolder, Carpeta carpeta)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.name == name)
                        carpeta.files.Add(archivo);


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Recursive_FileName(name, subFolder, carpeta);

        }
        private void Recursivo_Archivo(Formato formato, Carpeta currFolder, Carpeta carpeta)
        {
            if (currFolder.files is List<Archivo>)
            {
                foreach (Archivo archivo in currFolder.files)
                {
                    if (archivo.formato == formato)
                    {

                        if (formato == Formato.Imagen || formato == Formato.Celdas ||
                            formato == Formato.Animación || formato == Formato.Map)
                        {
                            if (!archivo.name.Contains('.')) // Archivos de nombre desconocido
                                continue;

                            #region Búsqueda de compañeros

                            string name = archivo.name.Remove(archivo.name.LastIndexOf('.'));
                            Carpeta fm = new Carpeta();
                            fm.name = name;
                            fm.files = new List<Archivo>();
                            Archivo pal, til, cel;

                            switch (formato)
                            {
                                case Formato.Animación:
                                    pal = Recursivo_Archivo(Formato.Paleta, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Formato.Imagen, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);
                                    cel = Recursivo_Archivo(Formato.Celdas, name, root);
                                    if (!(cel.name is String))
                                        goto No_Valido;
                                    fm.files.Add(cel);

                                    fm.files.Add(archivo);
                                    break;

                                case Formato.Celdas:
                                    pal = Recursivo_Archivo(Formato.Paleta, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Formato.Imagen, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Formato.Map:
                                    pal = Recursivo_Archivo(Formato.Paleta, name, root);
                                    if (!(pal.name is String))
                                        goto No_Valido;
                                    fm.files.Add(pal);
                                    til = Recursivo_Archivo(Formato.Imagen, name, root);
                                    if (!(til.name is String))
                                        goto No_Valido;
                                    fm.files.Add(til);

                                    fm.files.Add(archivo);
                                    break;

                                case Formato.Imagen:
                                    pal = Recursivo_Archivo(Formato.Paleta, name, root);
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


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Recursivo_Archivo(formato, subFolder, carpeta);

        }
        private Carpeta Recursivo_Carpeta(int id, Carpeta currFolder)
        {
            if (currFolder.id == id)
                return currFolder;

            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    if (subFolder.id == id)
                        return subFolder;

                    Carpeta folder = Recursivo_Carpeta(id, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new Carpeta();
        }
        private Carpeta Recursivo_Carpeta(string name, Carpeta currFolder)
        {
            // Not recommend method to use, can be more than one directory with the same folder
            if (currFolder.name == name)
                return currFolder;

            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    if (subFolder.name == name)
                        return subFolder;

                    Carpeta folder = Recursivo_Carpeta(name, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new Carpeta();
        }

        public Byte[] Get_MagicID(int id)
        {
            Archivo currFile = Search_File(id);
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
        public Byte[] Get_MagicID(Archivo currFile)
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
            Archivo currFile = Search_File(id);
            if (currFile.size < 0x04)
                return "";

            BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
            br.BaseStream.Position = currFile.offset;

            if (br.BaseStream.Length < 0x04)
                return "";

            byte[] ext = br.ReadBytes(4);
            br.Close();

            string fin = new String(Encoding.ASCII.GetChars(ext));
            for (int i = 0; i < 4; i++)             // En caso de no ser extensión
                if (!Char.IsLetterOrDigit(fin[i]))
                    return "";

            return fin;
        }

        public Formato Get_Formato()
        {
            return Get_Formato(idSelect);
        }
        public Formato Get_Formato(int id)
        {
            Formato tipo = Formato.Desconocido;
            Archivo currFile = Search_File(id);
            if (currFile.size == 0x00)
                return Formato.Desconocido;

            byte[] ext = Get_MagicID(currFile);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Formato(currFile.name, ext, id);
                if (tipo != Formato.Desconocido)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Formato(currFile.name, ext);
                    if (tipo != Formato.Desconocido)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Formato.Desconocido;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Formato.Paleta;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Formato.Imagen;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Formato.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Formato.Celdas;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Formato.Animación;
            else if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN")
                return Formato.Sistema;

            FileStream fs = File.OpenRead(currFile.path);
            fs.Position = currFile.offset;
            if (DSDecmp.Main.Get_Format(fs, (currFile.name == "ARM9.BIN" ? true : false)) != FormatCompress.Invalid)
            {
                fs.Close();
                fs.Dispose();
                return Formato.Comprimido;
            }
            fs.Close();
            fs.Dispose();

            return Formato.Desconocido;
        }
        public Formato Get_Formato(Archivo currFile, int id)
        {
            if (currFile.size == 0x00)
                return Formato.Desconocido;

            Formato tipo = Formato.Desconocido;
            byte[] ext = Get_MagicID(currFile);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Formato(currFile.name, ext, id);
                if (tipo != Formato.Desconocido)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Formato(currFile.name, ext);
                    if (tipo != Formato.Desconocido)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Formato.Desconocido;
            }
            #endregion

            currFile.name = currFile.name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Formato.Paleta;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Formato.Imagen;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Formato.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Formato.Celdas;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Formato.Animación;
            else if (currFile.name == "FNT.BIN" || currFile.name == "FAT.BIN" || currFile.name.StartsWith("OVERLAY9_") || currFile.name.StartsWith("OVERLAY7_") ||
                currFile.name == "ARM9.BIN" || currFile.name == "ARM7.BIN" || currFile.name == "Y9.BIN" || currFile.name == "Y7.BIN")
                return Formato.Sistema;

            FileStream fs = new FileStream(currFile.path, FileMode.Open);
            fs.Position = currFile.offset;
            if (DSDecmp.Main.Get_Format(fs, (currFile.name == "ARM9.BIN" ? true : false)) != FormatCompress.Invalid)
            {
                fs.Close();
                fs.Dispose();
                return Formato.Comprimido;
            }
            fs.Close();
            fs.Dispose();

            return Formato.Desconocido;
        }
        public Formato Get_Formato(string file)
        {
            if (new FileInfo(file).Length == 0x00)
                return Formato.Desconocido;

            Formato tipo = Formato.Desconocido;
            string name = new FileInfo(file).Name;
            byte[] ext = Get_MagicID(file);

            #region Calling to plugins
            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Formato(name, ext, -1);
                if (tipo != Formato.Desconocido)
                    return tipo;

                foreach (IPlugin formato in formatList)
                {
                    tipo = formato.Get_Formato(name, ext);
                    if (tipo != Formato.Desconocido)
                        return tipo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Formato.Desconocido;
            }
            #endregion

            name = name.ToUpper();
            if (new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Formato.Paleta;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Formato.Imagen;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Formato.Map;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Formato.Celdas;
            else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Formato.Animación;
            else if (name == "FNT.BIN" || name == "FAT.BIN" || name.StartsWith("OVERLAY9_") || name.StartsWith("OVERLAY7_") ||
                name == "ARM9.BIN" || name == "ARM7.BIN" || name == "Y9.BIN" || name == "Y7.BIN")
                return Formato.Sistema;

            if (DSDecmp.Main.Get_Format(file) != FormatCompress.Invalid)
                return Formato.Comprimido;

            return Formato.Desconocido;
        }

        public Carpeta Extract()
        {
            return Extract(idSelect);
        }
        public Carpeta Extract(int id)
        {
            Archivo selectedFile = Search_File(id);

            // Save the file
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + id + selectedFile.name;
            BinaryReader br = new BinaryReader(File.OpenRead(selectedFile.path));
            br.BaseStream.Position = selectedFile.offset;
            File.WriteAllBytes(tempFile, br.ReadBytes((int)selectedFile.size));

            br.BaseStream.Position = selectedFile.offset;
            byte[] ext = br.ReadBytes(4);
            br.Close();

            #region Calling to plugins
            try
            {
                Formato f;
                if (gamePlugin is IGamePlugin)
                {
                    f = gamePlugin.Get_Formato(selectedFile.name, ext, idSelect);
                    if (f == Formato.Comprimido || f == Formato.Pack)
                    {
                        gamePlugin.Leer(tempFile, idSelect);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Formato(selectedFile.name, ext);
                    if (f == Formato.Comprimido || f == Formato.Pack)
                    {
                        plugin.Leer(tempFile, id);
                        goto Continuar;
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(tempFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    FileInfo info = new FileInfo(tempFile);
                    String uncompFile = info.DirectoryName + Path.DirectorySeparatorChar + "de_" + info.Name;
                    DSDecmp.Main.Decompress(tempFile, uncompFile, compressFormat);
                    if (!File.Exists(uncompFile))
                        throw new Exception(Tools.Helper.ObtenerTraduccion("Sistema", "S36"));

                    Archivo file = new Archivo();
                    file.name = selectedFile.name;
                    file.offset = 0x00;
                    file.path = uncompFile;
                    file.size = (uint)new FileInfo(uncompFile).Length;

                    Carpeta carpeta = new Carpeta();
                    carpeta.files = new List<Archivo>();
                    carpeta.files.Add(file);
                    pluginHost.Set_Files(carpeta);
                }
                else
                    return new Carpeta();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Carpeta();
            }
            #endregion
        Continuar:

            File.Delete(tempFile);
            Carpeta desc = pluginHost.Get_Files();

            Add_Files(ref desc, id);
            return desc;
        }
        public Carpeta Extract(String compressedFile, int id)
        {
            byte[] ext = Get_MagicID(compressedFile);
            String name = new FileInfo(compressedFile).Name;

            #region Calling to plugins
            try
            {
                Formato f;
                if (gamePlugin is IGamePlugin)
                {
                    f = gamePlugin.Get_Formato(name, ext, idSelect);
                    if (f == Formato.Comprimido || f == Formato.Pack)
                    {
                        gamePlugin.Leer(compressedFile, idSelect);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Formato(name, ext);
                    if (f == Formato.Comprimido || f == Formato.Pack)
                    {
                        plugin.Leer(compressedFile, id);
                        goto Continuar;
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(compressedFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    FileInfo info = new FileInfo(compressedFile);
                    String uncompFile = info.DirectoryName + Path.DirectorySeparatorChar + "de_" + id + info.Name;

                    DSDecmp.Main.Decompress(compressedFile, uncompFile, compressFormat);
                    if (!File.Exists(uncompFile))
                        throw new Exception(Tools.Helper.ObtenerTraduccion("Sistema", "S36"));

                    Archivo file = new Archivo();
                    file.name = name;
                    file.offset = 0x00;
                    file.path = uncompFile;
                    file.size = (uint)new FileInfo(uncompFile).Length;

                    Carpeta carpeta = new Carpeta();
                    carpeta.files = new List<Archivo>();
                    carpeta.files.Add(file);
                    pluginHost.Set_Files(carpeta);
                }
                else
                    return new Carpeta();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Carpeta();
            }
            #endregion

        Continuar:
            Carpeta desc = pluginHost.Get_Files();
            Add_Files(ref desc, id);    // Añadimos los archivos descomprimidos al árbol de archivos
            return desc;
        }
        void pluginHost_DescomprimirEvent(string arg)
        {
            byte[] ext = Get_MagicID(arg);

            try
            {
                Formato f;
                foreach (IPlugin plugin in formatList)
                {
                    f = plugin.Get_Formato(new FileInfo(arg).Name, ext);
                    if (f == Formato.Comprimido || f == Formato.Pack)
                    {
                        plugin.Leer(arg, -1);
                        goto Continuar;
                    }
                }

                Carpeta carpeta = new Carpeta();
                FileInfo info = new FileInfo(arg);

                String dec_file = info.DirectoryName + Path.DirectorySeparatorChar + "de" + Path.DirectorySeparatorChar + info.Name;
                Directory.CreateDirectory(info.DirectoryName + Path.DirectorySeparatorChar + "de");

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(arg);
                if (compressFormat != FormatCompress.Invalid)
                    DSDecmp.Main.Decompress(arg, dec_file);

                if (File.Exists(dec_file))
                {
                    Archivo file = new Archivo();
                    file.name = new FileInfo(arg).Name;
                    file.offset = 0x00;
                    file.path = dec_file;
                    file.size = (uint)new FileInfo(dec_file).Length;

                    carpeta.files = new List<Archivo>();
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
        Carpeta pluginHost_event_GetDecompressedFiles(int id)
        {
            Carpeta compresFile = Search_Folder(id);

            Carpeta decompressedFiles = new Carpeta();
            decompressedFiles.files = new List<Archivo>();
            decompressedFiles.folders = new List<Carpeta>();
            decompressedFiles.id = 0xF000; // Null value

            Get_DecompressedFiles(compresFile, decompressedFiles);
            Get_LowestID(decompressedFiles, ref decompressedFiles.id);

            return decompressedFiles;
        }
        void Get_DecompressedFiles(Carpeta currFolder, Carpeta decompressedFiles)
        {
            if (currFolder.files is List<Archivo>)
            {
                foreach (Archivo archivo in currFolder.files)
                {
                    decompressedFiles.files.Add(archivo);
                }
            }


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Carpeta currDecompressed = new Carpeta();
                    currDecompressed.files = new List<Archivo>();
                    currDecompressed.folders = new List<Carpeta>();
                    currDecompressed.id = subFolder.id;
                    currDecompressed.name = subFolder.name;

                    if ((String)subFolder.tag != "") // Decompressed file
                    {
                        Archivo file = Search_File(subFolder.id);
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
        void Get_LowestID(Carpeta currFolder, ref ushort id)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.id < id)
                        id = archivo.id;


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Get_LowestID(subFolder, ref id);
        }

        public void Save_File(int id, string outFile)
        {
            Archivo selectFile = Search_File(id);

            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)selectFile.size));
            br.Close();
        }
        public String Save_File(int id)
        {
            Archivo selectFile = Search_File(id);
            String outFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + id + selectFile.name;

            BinaryReader br = new BinaryReader(File.OpenRead(selectFile.path));
            br.BaseStream.Position = selectFile.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)selectFile.size));
            br.Close();

            return outFile;
        }
        public void Save_File(Archivo currfile, string outFile)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(currfile.path));
            br.BaseStream.Position = currfile.offset;
            File.WriteAllBytes(outFile, br.ReadBytes((int)currfile.size));
            br.Close();
        }


        public Control See_File()
        {
            Archivo selectFile = Select_File();

            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.id + selectFile.name; ;
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
                    if (gamePlugin.Get_Formato(selectFile.name, ext, idSelect) != Formato.Desconocido)
                    {
                        Control resultado = gamePlugin.Show_Info(tempFile, idSelect);
                        File.Delete(tempFile);
                        return resultado;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Formato(selectFile.name, ext) != Formato.Desconocido)
                    {
                        Control resultado = plugin.Show_Info(tempFile, idSelect);
                        File.Delete(tempFile);
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
                    File.Delete(tempFile);

                    iNCLR control = new iNCLR(nclr, pluginHost);
                    return control; ;
                }
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    NCGR tile = Imagen_NCGR.Leer(tempFile, idSelect);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost.Get_NSCR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Leer(tempFile, idSelect));
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCER control = new iNCER(pluginHost.Get_NCER(), pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost);
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCER().header.file_size != 0x00 && pluginHost.Get_NCGR().cabecera.file_size != 0x00 &&
                        pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNANR control = new iNANR(pluginHost.Get_NCLR(), pluginHost.Get_NCGR(), pluginHost.Get_NCER(), pluginHost.Get_NANR());
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1E"));
                        return new Control();
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(tempFile);
                if (compressFormat != FormatCompress.Invalid)
                {
                    Control resultado = new DSDecmp.CompressionControl(idSelect, compressFormat, pluginHost);
                    File.Delete(tempFile);
                    return resultado;
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
                    if (gamePlugin.Get_Formato(name, ext, idSelect) != Formato.Desconocido)
                    {
                        Control resultado = gamePlugin.Show_Info(archivo, idSelect);
                        File.Delete(archivo);
                        return resultado;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Formato(name, ext) != Formato.Desconocido)
                    {
                        Control resultado = plugin.Show_Info(archivo, idSelect);
                        File.Delete(archivo);
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
                    File.Delete(archivo);

                    iNCLR control = new iNCLR(nclr, pluginHost);
                    control.Dock = DockStyle.Fill;
                    return control; ;
                }
                if (new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    NCGR tile = Imagen_NCGR.Leer(archivo, idSelect);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(archivo);

                    if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR(), pluginHost);
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1B"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Leer(archivo, idSelect));
                    File.Delete(archivo);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost.Get_NSCR(), pluginHost);
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Leer(archivo, idSelect));
                    File.Delete(archivo);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCER control = new iNCER(pluginHost.Get_NCER(), pluginHost.Get_NCGR(), pluginHost.Get_NCLR(), pluginHost);
                        control.Dock = DockStyle.Fill;

                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1C"));
                        return new Control();
                    }
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(archivo, idSelect));
                    File.Delete(archivo);

                    if (pluginHost.Get_NCER().header.file_size != 0x00 && pluginHost.Get_NCGR().cabecera.file_size != 0x00 &&
                        pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNANR control = new iNANR(pluginHost.Get_NCLR(), pluginHost.Get_NCGR(), pluginHost.Get_NCER(), pluginHost.Get_NANR());
                        control.Dock = DockStyle.Fill;

                        return control;
                    }
                    else
                    {
                        MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1E"));
                        return new Control();
                    }
                }

                FormatCompress compressFormat = DSDecmp.Main.Get_Format(archivo);
                if (compressFormat != FormatCompress.Invalid)
                {
                    Control resultado = new DSDecmp.CompressionControl(idSelect, compressFormat, pluginHost);
                    File.Delete(archivo);
                    return resultado;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            try { File.Delete(archivo); }
            catch { }
            return new Control();
        }
        public void Set_Data()
        {
            Archivo selectFile = Select_File();

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
                    if (gamePlugin.Get_Formato(selectFile.name, ext, idSelect) != Formato.Desconocido)
                    {
                        gamePlugin.Leer(tempFile, idSelect);
                        File.Delete(tempFile);
                        return;
                    }
                }

                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Formato(selectFile.name, ext) != Formato.Desconocido)
                    {
                        plugin.Leer(tempFile, idSelect);
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
                    pluginHost.Set_NCGR(Imagen_NCGR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Leer(tempFile, idSelect));
                    File.Delete(tempFile);
                    return;
                }
                else if (new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Leer(tempFile, idSelect));
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
                    MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S1A"), "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            try { File.Delete(tempFile); }
            catch { }
        }
    }

}
