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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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

        IList<IPlugin> formatList;
        IGamePlugin gamePlugin;
        PluginHost pluginHost;

        public Acciones(string file, string name)
        {
            this.file = file;
            gameCode = name.Replace("\0", "");

            formatList = new List<IPlugin>();
            pluginHost = new PluginHost();
            pluginHost.DescomprimirEvent += new Action<string, byte>(pluginHost_DescomprimirEvent);
            Cargar_Plugins();
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
                    MessageBox.Show("Fallo al cargar el plugin: " + fileName + "\nEstá obsoleto.");
                    Console.WriteLine("Fallo al cargar el plugin: " + fileName);
                    Console.WriteLine(e.Message);
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
            set { root = value; Set_LastFileID(root); lastFileId++; Set_LastFolderID(root); lastFolderId++; }
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
                case Formato.Screen:
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
                    return 6;
                case Formato.Sonido:
                    return 14;
                case Formato.Video:
                    return 13;
                case Formato.Desconocido:
                default:
                    return 1;
            }
        }


        public void Set_Data()
        {
            // Guardamos el archivo fuera del sistema de ROM
            Archivo selectFile = Select_File();
            string tempFile;
            BinaryReader br;

            if (selectFile.offset != 0x0)
            {
                tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.name;
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;

                BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
                bw.Write(br.ReadBytes((int)selectFile.size));
                bw.Flush();
                bw.Close();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                FileInfo info = new FileInfo(selectFile.path);
                File.Copy(selectFile.path, info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name, true);
                tempFile = info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name;
                br = new BinaryReader(File.OpenRead(tempFile));
            }

            byte[] ext = br.ReadBytes(4);

            br.Close();

            #region Búsqueda y llamada a plugin
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
                        plugin.Leer(tempFile);
                        File.Delete(tempFile);
                        return;
                    }
                }

                if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                    MessageBox.Show("Archivo comprimido", "Datos del archivo",  MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
            }
            #endregion

            #region Formatos comunes
            try
            {
                selectFile.name = selectFile.name.ToUpper();
                if (selectFile.name.EndsWith(".NCLR") || new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                {
                    pluginHost.Set_NCLR(Imagen_NCLR.Leer(tempFile));
                    File.Delete(tempFile);
                }
                if (selectFile.name.EndsWith(".NCGR") || new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    pluginHost.Set_NCGR(Imagen_NCGR.Leer(tempFile));
                    File.Delete(tempFile);
                }
                else if (selectFile.name.EndsWith(".NSCR") || new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Leer(tempFile));
                    File.Delete(tempFile);
                }
                else if (selectFile.name.EndsWith(".NCER") || new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Leer(tempFile));
                    File.Delete(tempFile);
                }
                else if (selectFile.name.EndsWith(".NANR") || new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(tempFile));
                    File.Delete(tempFile);
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
                case Formato.Screen:
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
        	#region Guardamos el archivo fuera del sistema de ROM
            Archivo selectFile = Select_File();
            string tempFile;
            BinaryReader br;

            if (selectFile.offset != 0x0)
            {
                tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.name;
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;

                BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
                bw.Write(br.ReadBytes((int)selectFile.size));
                bw.Flush();
                bw.Close();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                FileInfo info = new FileInfo(selectFile.path);
                File.Copy(selectFile.path, info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name, true);
                tempFile = info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name;
                br = new BinaryReader(File.OpenRead(tempFile));
            }

            byte[] ext = br.ReadBytes(4);

            br.Close();
			#endregion
			
			#region Formatos comunes
            try
            {
                if (formato == Formato.Paleta)
                {
                    NCLR paleta = Imagen_NCLR.Leer_Basico(tempFile);
                    pluginHost.Set_NCLR(paleta);
                    File.Delete(tempFile);
                    
                    return new iNCLR(paleta);
                }               
                else if (formato == Formato.Imagen)
                {
                    NCGR tile = Imagen_NCGR.Leer_Basico(tempFile);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR());
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                        return new Control();
                }
                else if (formato == Formato.Screen)
                {
                    NSCR nscr = Imagen_NSCR.Leer_Basico(tempFile);
                    pluginHost.Set_NSCR(nscr);
                    File.Delete(tempFile);
                    
                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 || pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        NCGR tile = pluginHost.Get_NCGR();
                        tile.rahc.tileData = Imagen_NSCR.Modificar_Tile(pluginHost.Get_NSCR(), tile.rahc.tileData);
                        tile.rahc.nTilesX = (ushort)(pluginHost.Get_NSCR().section.width / 8);
                        tile.rahc.nTilesY = (ushort)(pluginHost.Get_NSCR().section.height / 8);

                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR());
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    return new Control();
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
        public void Add_Files(Carpeta files)
        {
            files.name = Select_File().name;
            root = FileToFolder(Select_File().id, root);
            files = Add_ID(files); 
            Add_Files(files, (ushort)(lastFolderId - 1), root);
        }
        public Carpeta Add_Files(Carpeta files, ushort idFolder, Carpeta currFolder)
        {
            if (currFolder.folders is List<Carpeta>)   // Si tiene subdirectorios, buscamos en cada uno de ellos
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (currFolder.folders[i].id == idFolder)
                    {
                        currFolder.folders[i] = files;
                        return currFolder.folders[i];
                    }

                    Carpeta folder = Add_Files(files, idFolder, currFolder.folders[i]);
                    if (folder.name is string)  // Comprobamos que se haya devuelto un directorio, en cuyo caso es el buscado que lo devolvemos
                        return folder;
                }
            }

            return new Carpeta();
        }
        public Carpeta FileToFolder(int id, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        Carpeta newFolder = new Carpeta();
                        newFolder.name = currFolder.files[i].name;
                        newFolder.id = (ushort)lastFolderId;
                        lastFolderId++;
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Carpeta>))
                            currFolder.folders = new List<Carpeta>();
                        currFolder.folders.Add(newFolder);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Carpeta folder = FileToFolder(id, subFolder);
                    if (folder.name is string)
                    {
                        currFolder.folders.Remove(subFolder);
                        currFolder.folders.Add(folder);
                        currFolder.folders.Sort(Comparacion_Directorios);
                        return currFolder;
                    }
                }
            }

            return new Carpeta();
        }
        private static int Comparacion_Directorios(Carpeta f1, Carpeta f2)
        {
            return String.Compare(f1.name, f2.name);
        }
        public void Change_File(int id, Archivo fileChanged, Carpeta currFolder)
        {            
            if (currFolder.files is List<Archivo>)
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        currFolder.files[i] = fileChanged;


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Change_File(id, fileChanged, subFolder);
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
        public Carpeta Select_Folder()
        {
            return Recursivo_Carpeta(idSelect, root);
        }
        private Archivo Recursivo_Archivo(int id, Carpeta currFolder)
        {
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
        private Carpeta Recursivo_Carpeta(int id, Carpeta currFolder)
        {
            if (currFolder.id == id)
                return currFolder;

            if (currFolder.folders is List<Carpeta>)   // Si tiene subdirectorios, buscamos en cada uno de ellos
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    if (subFolder.id == id)     // Si lo hemos encontrado lo devolvemos, en caso contrario, seguimos buscando
                        return subFolder;

                    Carpeta folder = Recursivo_Carpeta(id, subFolder);
                    if (folder.name is string)  // Comprobamos que se haya devuelto un directorio, en cuyo caso es el buscado que lo devolvemos
                        return folder;
                }
            }

            return new Carpeta();
        }

        public Byte[] Get_MagicID(int id)
        {
            Archivo currFile = Search_File(id);
            BinaryReader br;
            if (currFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = currFile.offset;
            }
            else    // En caso de que el archivo haya sido extraído y no esté en la ROM
                br = new BinaryReader(File.OpenRead(currFile.path));

            if (br.BaseStream.Length == 0x00)
                return null;

            byte[] ext = null;
            try { ext = br.ReadBytes(4); }
            catch { }

            br.Close();

            return ext;
        }
        public String Get_MagicIDS(int id)
        {
            Archivo currFile = Search_File(id);
            BinaryReader br;
            if (currFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = currFile.offset;
            }
            else    // En caso de que el archivo haya sido extraído y no esté en la ROM
                br = new BinaryReader(File.OpenRead(currFile.path));

            if (br.BaseStream.Length == 0x00)
                return "";

            byte[] ext = null;
            try { ext = br.ReadBytes(4); }
            catch { }

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
            byte[] ext = Get_MagicID(id);

            #region Búsqueda y llamada de plugin
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
            if (currFile.name.EndsWith(".NCLR") || new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                return Formato.Paleta;
            else if (currFile.name.EndsWith(".NCGR") || new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                return Formato.Imagen;
            else if (currFile.name.EndsWith(".NSCR") || new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                return Formato.Screen;
            else if (currFile.name.EndsWith(".NCER") || new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                return Formato.Celdas;
            else if (currFile.name.EndsWith(".NANR") || new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                return Formato.Animación;

            if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                return Formato.Comprimido;

            return Formato.Desconocido;
        }

        public Carpeta Extract()
        {
            // Guardamos el archivo para descomprimir fuera del sistema de ROM
            Archivo selectFile = Select_File();
            string tempFile; 
            BinaryReader br;
            byte[] ext;

            if (selectFile.offset != 0x0)
            {
                tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + selectFile.name;
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;

                BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
                bw.Write(br.ReadBytes((int)selectFile.size));
                bw.Flush();
                bw.Close();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                FileInfo info = new FileInfo(selectFile.path);
                File.Copy(selectFile.path, info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name, true);
                tempFile = info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name;
                br = new BinaryReader(File.OpenRead(tempFile));             
            }

            ext = br.ReadBytes(4);

            br.Close();

            #region Búsqueda y llamada a plugin
            try
            {
                if (gamePlugin is IGamePlugin)
                {
                    if (gamePlugin.Get_Formato(selectFile.name, ext, idSelect) != Formato.Desconocido)
                    {
                        gamePlugin.Leer(tempFile, idSelect);
                        goto Continuar;
                    }
                }
                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Formato(selectFile.name, ext) != Formato.Desconocido)
                    {
                        plugin.Leer(tempFile);
                        goto Continuar;
                    }
                }

                if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                {
                    FileInfo info = new FileInfo(tempFile);
                    Compresion.Basico.Decompress(tempFile, info.DirectoryName + Path.DirectorySeparatorChar + "un_" + info.Name);
                    Carpeta carpeta = new Carpeta();
                    Archivo file = new Archivo();
                    file.name = selectFile.name;
                    file.path = info.DirectoryName + Path.DirectorySeparatorChar + "un_" + info.Name;
                    file.size = (uint)new FileInfo(info.DirectoryName + Path.DirectorySeparatorChar + "un_" + info.Name).Length;
                    carpeta.files = new List<Archivo>();
                    carpeta.files.Add(file);
                    pluginHost.Set_Files(carpeta);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
                return new Carpeta();
            }
            #endregion
        Continuar:

            // Se añaden los archivos descomprimidos al árbol de archivos.
            File.Delete(tempFile);
            Carpeta desc = pluginHost.Get_Files();
            Add_Files(desc);
            return desc;
        }
        /// <summary>
        /// Evento llamado desde los plugins en el cual se descomprime un archivo a través de otros plugins.
        /// </summary>
        /// <param name="archivo">Archivo a descomprimir</param>
        /// <returns>Ruta de la carpeta donde se encuentran los archivos descomprimidos</returns>
        void pluginHost_DescomprimirEvent(string arg, byte tag)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(arg));
            byte[] ext = br.ReadBytes(4);
            br.Close();

            try
            {
                foreach (IPlugin plugin in formatList)
                {
                    if (plugin.Get_Formato(new FileInfo(arg).Name, ext) != Formato.Desconocido)
                    {
                        plugin.Leer(arg);
                        goto Continuar;
                    }
                }
                // Si no hay plugins disponibles, se descomprime con el método normal
                FileInfo info = new FileInfo(arg);
                Directory.CreateDirectory(info.DirectoryName + Path.DirectorySeparatorChar + "un");
                switch (tag) {
                	case LZ77_TAG:
                		Compresion.LZ77.DecompressLZ77(arg, info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name);
                		break;
                	case LZSS_TAG:
                		Compresion.LZSS.Decompress11LZS(arg, info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name);
                		break;
                	case HUFF_TAG:
                		Compresion.Huffman.DecompressHuffman(arg, info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name);
                		break;
                	case RLE_TAG :
                		Compresion.RLE.DecompressRLE(arg, info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name);
                		break;
                	default:
                		Compresion.Basico.Decompress(arg, info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name);
                		break;
                }
                Archivo file = new Archivo();
                file.name = new FileInfo(arg).Name;
                file.path = info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name;
                file.size = (uint)new FileInfo(info.DirectoryName + Path.DirectorySeparatorChar + "un" + Path.DirectorySeparatorChar + info.Name).Length;
                Carpeta carpeta = new Carpeta();
                carpeta.files = new List<Archivo>();
                carpeta.files.Add(file);
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

        public Control See_File()
        {
            // Guardamos el archivo fuera del sistema de ROM
            Archivo selectFile = Select_File();
            string tempFile;
            BinaryReader br;

            if (selectFile.offset != 0x0)
            {
                tempFile = pluginHost.Get_TempFolder() + '\\' + selectFile.name;
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;

                BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
                bw.Write(br.ReadBytes((int)selectFile.size));
                bw.Flush();
                bw.Close();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                FileInfo info = new FileInfo(selectFile.path);
                File.Copy(selectFile.path, info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name, true);
                tempFile = info.DirectoryName + Path.DirectorySeparatorChar + "temp_" + info.Name;
                br = new BinaryReader(File.OpenRead(tempFile));
            }

            byte[] ext = br.ReadBytes(4);

            br.Close();

            #region Búsqueda y llamada a plugin
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
                        Control resultado = plugin.Show_Info(tempFile);
                        File.Delete(tempFile);
                        return resultado;
                    }
                }

                if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                {
                    File.Delete(tempFile);
                    MessageBox.Show("Por el momento compresiones normales no devuelven información.");
                    return new Control();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
                try { File.Delete(tempFile); }
                catch { }
                return new Control();
            }
            #endregion

            #region Formatos comunes
            try
            {
                selectFile.name = selectFile.name.ToUpper();
                if (selectFile.name.EndsWith(".NCLR") || new String(Encoding.ASCII.GetChars(ext)) == "NCLR" || new String(Encoding.ASCII.GetChars(ext)) == "RLCN")
                {
                    NCLR nclr = Imagen_NCLR.Leer(tempFile);
                    pluginHost.Set_NCLR(nclr);
                    File.Delete(tempFile);

                    iNCLR control = new iNCLR(nclr);
                    control.Dock = DockStyle.Fill;
                    return control; ;
                }
                if (selectFile.name.EndsWith(".NCGR") || new String(Encoding.ASCII.GetChars(ext)) == "NCGR" || new String(Encoding.ASCII.GetChars(ext)) == "RGCN")
                {
                    NCGR tile = Imagen_NCGR.Leer(tempFile);
                    pluginHost.Set_NCGR(tile);
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR());
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    else
                        return new Control();
                }
                else if (selectFile.name.EndsWith(".NSCR") || new String(Encoding.ASCII.GetChars(ext)) == "NSCR" || new String(Encoding.ASCII.GetChars(ext)) == "RCSN")
                {
                    pluginHost.Set_NSCR(Imagen_NSCR.Leer(tempFile));
                    File.Delete(tempFile);
                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 || pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        NCGR tile = pluginHost.Get_NCGR();
                        tile.rahc.tileData = Imagen_NSCR.Modificar_Tile(pluginHost.Get_NSCR(), tile.rahc.tileData);
                        tile.rahc.nTilesX = (ushort)(pluginHost.Get_NSCR().section.width / 8);
                        tile.rahc.nTilesY = (ushort)(pluginHost.Get_NSCR().section.height / 8);

                        iNCGR control = new iNCGR(tile, pluginHost.Get_NCLR());
                        control.Dock = DockStyle.Fill;
                        return control;
                    }
                    return new Control();
                }
                else if (selectFile.name.EndsWith(".NCER") || new String(Encoding.ASCII.GetChars(ext)) == "NCER" || new String(Encoding.ASCII.GetChars(ext)) == "RECN")
                {
                    pluginHost.Set_NCER(Imagen_NCER.Leer(tempFile));
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNCER control = new iNCER(pluginHost.Get_NCER(), pluginHost.Get_NCGR(), pluginHost.Get_NCLR());
                        control.Dock = DockStyle.Fill;

                        return control;
                    }

                    return new Control();
                }
                else if (selectFile.name.EndsWith(".NANR") || new String(Encoding.ASCII.GetChars(ext)) == "NANR" || new String(Encoding.ASCII.GetChars(ext)) == "RNAN")
                {
                    pluginHost.Set_NANR(Imagen_NANR.Leer(tempFile));
                    File.Delete(tempFile);

                    if (pluginHost.Get_NCER().header.file_size != 0x00 && pluginHost.Get_NCGR().cabecera.file_size != 0x00 &&
                        pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                    {
                        iNANR control = new iNANR(pluginHost.Get_NCLR(), pluginHost.Get_NCGR(), pluginHost.Get_NCER(), pluginHost.Get_NANR());
                        control.Dock = DockStyle.Fill;

                        return control;
                    }
                    return new Control();
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

    }

}
