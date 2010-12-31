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
        int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20;

        string file;
        string gameCode;
        Nitro.Estructuras.Folder root;
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
            pluginHost.DescomprimirEvent += new Func<string, string>(pluginHost_DescomprimirEvent);
            Cargar_Plugins();
        }
        private void Cargar_Plugins()
        {

            foreach (string fileName in Directory.GetFiles("Plugins", "*.dll"))
            {
                try
                {

                    if (fileName.EndsWith("PluginInterface.dll"))
                        continue;

                    Assembly assembly = Assembly.LoadFile(Application.StartupPath + '\\' + fileName);
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

        public Nitro.Estructuras.Folder Root
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
                case Formato.Paleta:
                    return 2;
                case Formato.Imagen:
                    return 3;
                case Formato.Screen:
                    return 9;
                case Formato.Celdas:
                    return 8;
                case Formato.Animación:
                    return 8;
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
            Nitro.Estructuras.File selectFile = Select_File();
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
                bw.Dispose();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                tempFile = selectFile.path;
                br = new BinaryReader(File.OpenRead(tempFile));
            }

            byte[] ext = br.ReadBytes(4);

            br.Close();
            br.Dispose();

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

            File.Delete(tempFile);
        }
        public void Set_LastFileID(Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.files is List<Nitro.Estructuras.File>)
                foreach (Nitro.Estructuras.File archivo in currFolder.files)
                    if (archivo.id > lastFileId)
                        lastFileId = archivo.id;

            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                    Set_LastFileID(subFolder);

        }
        public void Set_LastFolderID(Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.id > lastFolderId)
                lastFolderId = currFolder.id;

            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
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

        public void Add_Files(List<Nitro.Estructuras.File> files)
        {
            for (int i = 0; i < files.Count; i++)
            {
                Nitro.Estructuras.File currFile = files[i];
                currFile.id = (ushort)lastFileId;
                files.RemoveAt(i);
                files.Insert(i, currFile);
                lastFileId++;
            }

            int idNewFolder = Select_File().id;
            root = FileToFolder(idNewFolder, root);
            Add_Files(files, (ushort)(lastFolderId - 1), root);
           
        }
        public Nitro.Estructuras.Folder Add_Files(List<Nitro.Estructuras.File> files, ushort idFolder, Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.folders is List<Nitro.Estructuras.Folder>)   // Si tiene subdirectorios, buscamos en cada uno de ellos
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (currFolder.folders[i].id == idFolder)
                    {
                        Nitro.Estructuras.Folder newFolder = currFolder.folders[i];
                        if (!(newFolder.files is List<Nitro.Estructuras.File>))
                            newFolder.files = new List<Nitro.Estructuras.File>();
                        newFolder.files.AddRange(files);
                        currFolder.folders[i] = newFolder;
                        return currFolder.folders[i];
                    }

                    Nitro.Estructuras.Folder folder = Add_Files(files, idFolder, currFolder.folders[i]);
                    if (folder.name is string)  // Comprobamos que se haya devuelto un directorio, en cuyo caso es el buscado que lo devolvemos
                        return folder;
                }
            }

            return new Nitro.Estructuras.Folder();
        }
        public Nitro.Estructuras.Folder FileToFolder(int id, Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.files is List<Nitro.Estructuras.File>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        Nitro.Estructuras.Folder newFolder = new Nitro.Estructuras.Folder();
                        newFolder.name = currFolder.files[i].name;
                        newFolder.id = (ushort)lastFolderId;
                        lastFolderId++;
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Nitro.Estructuras.Folder>))
                            currFolder.folders = new List<Nitro.Estructuras.Folder>();
                        currFolder.folders.Add(newFolder);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
            {
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                {
                    Nitro.Estructuras.Folder folder = FileToFolder(id, subFolder);
                    if (folder.name is string)
                    {
                        currFolder.folders.Remove(subFolder);
                        currFolder.folders.Add(folder);
                        currFolder.folders.Sort(Comparacion_Directorios);
                        return currFolder;
                    }
                }
            }

            return new Nitro.Estructuras.Folder();
        }
        private static int Comparacion_Directorios(Nitro.Estructuras.Folder f1, Nitro.Estructuras.Folder f2)
        {
            return String.Compare(f1.name, f2.name);
        }
        public void Change_File(int id, Nitro.Estructuras.File fileChanged, Nitro.Estructuras.Folder currFolder)
        {            
            if (currFolder.files is List<Nitro.Estructuras.File>)
                for (int i = 0; i < currFolder.files.Count; i++)
                    if (currFolder.files[i].id == id)
                        currFolder.files[i] = fileChanged;


            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                    Change_File(id, fileChanged, subFolder);
        }
    
        public Nitro.Estructuras.File Select_File()
        {
            return Recursivo_Archivo(idSelect, root);
        }
        public Nitro.Estructuras.File Search_File(int id)
        {
            return Recursivo_Archivo(id, root);
        }
        public Nitro.Estructuras.Folder Select_Folder()
        {
            return Recursivo_Carpeta(idSelect, root);
        }
        private Nitro.Estructuras.File Recursivo_Archivo(int id, Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.files is List<Nitro.Estructuras.File>)
                foreach (Nitro.Estructuras.File archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
            {
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                {
                    Nitro.Estructuras.File currFile = Recursivo_Archivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new Nitro.Estructuras.File();
        }
        private Nitro.Estructuras.Folder Recursivo_Carpeta(int id, Nitro.Estructuras.Folder currFolder)
        {
            if (currFolder.folders is List<Nitro.Estructuras.Folder>)   // Si tiene subdirectorios, buscamos en cada uno de ellos
            {
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                {
                    if (subFolder.id == id)     // Si lo hemos encontrado lo devolvemos, en caso contrario, seguimos buscando
                        return subFolder;

                    Nitro.Estructuras.Folder folder = Recursivo_Carpeta(id, subFolder);
                    if (folder.name is string)  // Comprobamos que se haya devuelto un directorio, en cuyo caso es el buscado que lo devolvemos
                        return folder;
                }
            }

            return new Nitro.Estructuras.Folder();
        }

        public Formato Get_Formato()
        {
            Formato tipo = Formato.Desconocido;
            Nitro.Estructuras.File currFile = Select_File();

            BinaryReader br;
            if (currFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = currFile.offset;
            }
            else    // En caso de que el archivo haya sido extraído y no esté en la ROM
                br = new BinaryReader(File.OpenRead(currFile.path));

            byte[] ext;
            try { ext = br.ReadBytes(4); }
            catch
            {
                Console.WriteLine("Error al intentar obtener el formato del archivo");
                return Formato.Desconocido;
            }

            br.Close();
            br.Dispose();

            try
            {
                if (gamePlugin is IGamePlugin)
                    tipo = gamePlugin.Get_Formato(currFile.name, ext, idSelect);
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

            if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                return Formato.Comprimido;

            return Formato.Desconocido;
        }
        public Formato Get_Formato(int id)
        {
            Formato tipo = Formato.Desconocido;
            Nitro.Estructuras.File currFile = Search_File(id);
            BinaryReader br;
            if (currFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = currFile.offset;
            }
            else    // En caso de que el archivo haya sido extraído y no esté en la ROM
                br = new BinaryReader(File.OpenRead(currFile.path));

            byte[] ext = null;
            try { ext = br.ReadBytes(4); }
            catch { } 

            br.Close();
            br.Dispose();

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

            if (ext[0] == LZ77_TAG || ext[0] == LZSS_TAG || ext[0] == RLE_TAG || ext[0] == HUFF_TAG)
                return Formato.Comprimido;

            return Formato.Desconocido;
        }

        public void Extract()
        {
            // Guardamos el archivo para descomprimir fuera del sistema de ROM
            Nitro.Estructuras.File selectFile = Select_File();
            string tempFile; 
            BinaryReader br;
            byte[] ext;

            if (selectFile.offset != 0x0)
            {
                tempFile = pluginHost.Get_TempFolder() + '\\' + selectFile.name;
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;

                BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
                bw.Write(br.ReadBytes((int)selectFile.size));
                bw.Flush();
                bw.Close();
                bw.Dispose();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                tempFile = selectFile.path;
                br = new BinaryReader(File.OpenRead(tempFile));             
            }

            ext = br.ReadBytes(4);

            br.Close();
            br.Dispose();

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
                    Compresion.Basico.Decompress(tempFile, info.DirectoryName + "\\un_" + info.Name);
                    Archivo file = new Archivo();
                    file.name = selectFile.name;
                    file.path = info.DirectoryName + "\\un_" + info.Name;
                    file.size = (uint)new FileInfo(info.DirectoryName + "\\un_" + info.Name).Length;
                    pluginHost.Set_Files(new Archivo[1] { file });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
                return;
            }
            #endregion
        Continuar:

            List<Nitro.Estructuras.File> files = new List<Nitro.Estructuras.File>();
            foreach (Archivo archivo in pluginHost.Get_Files())
            {
                Nitro.Estructuras.File newFile = new Nitro.Estructuras.File();
                newFile.name = archivo.name;
                newFile.path = archivo.path;
                newFile.size = archivo.size;
                files.Add(newFile);
            }
            // Se añaden los archivos descomprimidos al árbol de archivos.
            Add_Files(files);

            File.Delete(tempFile);
        }
        /// <summary>
        /// Evento llamado desde los plugins en el cual se descomprime un archivo a través de otros plugins.
        /// </summary>
        /// <param name="archivo">Archivo a descomprimir</param>
        /// <returns>Ruta de la carpeta donde se encuentran los archivos descomprimidos</returns>
        string pluginHost_DescomprimirEvent(string arg)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(arg));
            byte[] ext = br.ReadBytes(4);
            br.Close();
            br.Dispose();

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
                Directory.CreateDirectory(info.DirectoryName + "\\un");
                Compresion.Basico.Decompress(arg, info.DirectoryName + "\\un\\" + info.Name);
                Archivo file = new Archivo();
                file.name = new FileInfo(arg).Name;
                file.path = info.DirectoryName + "\\un\\" + info.Name;
                file.size = (uint)new FileInfo(info.DirectoryName + "\\un\\" + info.Name).Length;
                pluginHost.Set_Files(new Archivo[1] { file });

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
                return "";
            }
        Continuar:
            return new FileInfo(pluginHost.Get_Files()[0].path).DirectoryName;
        }

        public Control See_File()
        {
            // Guardamos el archivo fuera del sistema de ROM
            Nitro.Estructuras.File selectFile = Select_File();
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
                bw.Dispose();

                br.BaseStream.Position = selectFile.offset;
            }
            else
            {
                tempFile = selectFile.path;
                br = new BinaryReader(File.OpenRead(tempFile));
            }

            byte[] ext = br.ReadBytes(4);

            br.Close();
            br.Dispose();

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
                File.Delete(tempFile);
                return new Control();
            }
            #endregion

            File.Delete(tempFile);
            return new Control();
        }

    }

}
