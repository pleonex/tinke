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
            Cargar_Plugins();
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

        public void Set_Data(Nitro.Estructuras.File fileData)
        {
            /*Formato tipo = Get_Formato(fileData.id);

            string tempFile = "";

            if (fileData.offset != 0x0)
            {
                tempFile = Application.StartupPath + "\\temp.dat";
                BinaryReader br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = fileData.offset;
                BinaryWriter bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\temp.dat", FileMode.Create));
                bw.Write(br.ReadBytes((int)fileData.size));
                bw.Flush();
                bw.Close();
                bw.Dispose();
                br.Close();
                br.Dispose();
            }
            else
                tempFile = fileData.path;

            Object obtenido;
            if (Tipos.IsSupportedGame(gameCode))
            {
                obtenido = Tipos.DoActionGame(gameCode, tempFile, tipo, idSelect, fileData.name, this);
                if (obtenido is String)
                    obtenido = Tipos.DoAction(tempFile, tipo, Extension());
            }
            else
                obtenido = Tipos.DoAction(tempFile, tipo, Extension());

            if (obtenido is Imagen.Paleta.Estructuras.NCLR)
                paleta = (Imagen.Paleta.Estructuras.NCLR)obtenido;
            else if (obtenido is Imagen.Tile.Estructuras.NCGR)
                tile = (Imagen.Tile.Estructuras.NCGR)obtenido;
            else if (obtenido is Imagen.Screen.Estructuras.NSCR)
                screen = (Imagen.Screen.Estructuras.NSCR)obtenido;*/
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

            char[] ext;
            try { ext = br.ReadChars(4); }
            catch
            {
                Console.WriteLine("Error al intentar obtener el formato del archivo");
                return Formato.Desconocido;
            }

            br.Close();
            br.Dispose();

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

            char[] ext = null;
            try { ext = br.ReadChars(4); }
            catch { } 

            br.Close();
            br.Dispose();

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
            return Formato.Desconocido;
        }

        public void Extract()
        {
            // Guardamos el archivo para descomprimir fuera del sistema de ROM
            string tempFile = pluginHost.Get_TempFolder() + "\\temp.dat";
            Nitro.Estructuras.File selectFile = Select_File();
            BinaryReader br;
            if (selectFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;
            }
            else
                br = new BinaryReader(File.OpenRead(selectFile.path));

            BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
            bw.Write(br.ReadBytes((int)selectFile.size));
            bw.Flush();
            bw.Close();
            bw.Dispose();
            br.Close();
            br.Dispose();

            // Determinado el tipo de compresión y descomprimimos
            Formato tipo = Get_Formato();

            try
            {
                // TODO: Buscar el plugin correspondiente y llamarlo para que descomprima.
                // los archivos se quedan en pluginHost.Get_Files();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.Message);
                return;
            }
             List<Nitro.Estructuras.File> files = new List<Nitro.Estructuras.File>(); // Todavia no implementado
            // TODO: Convertir mediante un foreach del tipo PluginInterface.Archivo[] a Nitro.Estructuras.File[]
            // Se añaden los archivos descomprimidos al árbol de archivos.
            Add_Files(files);

            File.Delete(tempFile);
        }

        public Control See_File()
        {
            return new Control();
            /*Tipos.Role tipo = Get_Formato();

            Nitro.Estructuras.File fileSelect = Select_File();
            string tempFile = "";

            if (fileSelect.offset != 0x0)
            {
                tempFile = Application.StartupPath + "\\temp.dat";
                BinaryReader br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = fileSelect.offset;
                BinaryWriter bw = new BinaryWriter(new FileStream(Application.StartupPath + "\\temp.dat", FileMode.Create));
                bw.Write(br.ReadBytes((int)fileSelect.size));
                bw.Flush();
                bw.Close();
                bw.Dispose();
                br.Close();
                br.Dispose();
            }
            else
                tempFile = fileSelect.path;

            Object obtenido;

            if (Tipos.IsSupportedGame(gameCode))
            {
                obtenido = Tipos.DoActionGame(gameCode, tempFile, tipo, idSelect, fileSelect.name, this);
                if (obtenido is String)
                    obtenido = Tipos.DoAction(tempFile, tipo, Extension());
            }
            else
            {
                obtenido = Tipos.DoAction(tempFile, tipo, Extension());
            }

            if (obtenido is Bitmap[])
                return Obtenido_Imagenes((Bitmap[])obtenido);
            else if (obtenido is Bitmap)
                return Obtenido_Imagen((Bitmap)obtenido);
            else if (obtenido is Imagen.Paleta.Estructuras.NCLR)
            {
                paleta = (Imagen.Paleta.Estructuras.NCLR)obtenido;
                return Obtenido_Paleta();
            }
            else if (obtenido is Imagen.Tile.Estructuras.NCGR)
            {
                tile = (Imagen.Tile.Estructuras.NCGR)obtenido;
                return Obtenido_Tile();
            }
            else if (obtenido is Imagen.Screen.Estructuras.NSCR)
            {
                screen = (Imagen.Screen.Estructuras.NSCR)obtenido;
                return Obtenido_Tile();
            }
            else if (obtenido is Imagen.CER)
            {
                cell = (Imagen.CER)obtenido;
                if (new String(tile.id) == "RGCN" && new String(paleta.ID) == "RLCN")
                {
                    Bitmap[] imagenes = new Bitmap[cell.cebk.nBanks];
                    for (int i = 0; i < cell.cebk.nBanks; i++)
                        imagenes[i] = Imagen.NCER.Obtener_Imagen(cell.cebk.banks[i], tile, paleta);
                    return Obtenido_Imagenes(imagenes);
                }
            }
            else if (obtenido is String)
                return Obtenido_Texto((String)obtenido);
            else if (obtenido is Panel)
                return (Panel)obtenido;

            return new Control();   // Nunca debería darse este caso*/
        }

        /*#region Devolver GUI del archivo
        private TabControl Obtenido_Imagenes(Bitmap[] imagenes)
        {
            TabControl tab = new TabControl();
            TabPage[] pages = new TabPage[imagenes.Length];

            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = new TabPage("Imagen " + i.ToString());

                PictureBox pic = new PictureBox();
                pic.Image = imagenes[i];
                pic.SizeMode = PictureBoxSizeMode.AutoSize;
                pages[i].Controls.Add(pic);
                pic.MouseDoubleClick += new MouseEventHandler(pic_MouseDoubleClick);
            }

            tab.TabPages.AddRange(pages);
            tab.Dock = DockStyle.Fill;
            return tab;
        }
        private PictureBox Obtenido_Imagen(Bitmap imagen)
        {
            PictureBox pic = new PictureBox();
            pic.Image = imagen;
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.MouseDoubleClick += new MouseEventHandler(pic_MouseDoubleClick);

            return pic;
        }
        void pic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            if (o.ShowDialog() == DialogResult.OK)
                ((PictureBox)sender).Image.Save(o.FileName);
        }
        private PictureBox Obtenido_Paleta()
        {
            PictureBox pic = new PictureBox();
            pic.Image = Imagen.Paleta.NCLR.Mostrar(paleta);
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.MouseDoubleClick += new MouseEventHandler(pic_MouseDoubleClick);

            return pic;
        }
        private Control Obtenido_Tile()
        {
            if (paleta.constante == 0x0100) // Comprobación de que hay guardada una paleta
            {
                Imagen.Tile.iNCGR ventana;
                if (new String(screen.id) == "RCSN") // Comprobación de que hay una información de imagen guardada
                {
                    Imagen.Tile.Estructuras.NCGR newTile = tile;
                    newTile.rahc.tileData = Imagen.Screen.NSCR.Modificar_Tile(screen, tile.rahc.tileData);
                    newTile.rahc.nTilesX = (ushort)(screen.section.width / 8);
                    newTile.rahc.nTilesY = (ushort)(screen.section.height / 8);
                    ventana = new Imagen.Tile.iNCGR(newTile, paleta);
                }
                else
                    ventana = new Imagen.Tile.iNCGR(tile, paleta);
                ventana.Dock = DockStyle.Fill;
                return ventana;
            }
            else
                return new PictureBox();
        }
        private TextBox Obtenido_Texto(string texto)
        {
            TextBox txt = new TextBox();
            txt.Multiline = true;
            txt.Width = 300;
            txt.Dock = DockStyle.Fill;
            txt.ScrollBars = ScrollBars.Both;
            txt.Text = texto;
            txt.Select(0, 0);
            txt.ReadOnly = true;

            return txt;
        }
        #endregion // Devolver GUI del archivo*/

        private void Cargar_Plugins()
        {
            foreach (string fileName in Directory.GetFiles("Plugins", "*.dll"))
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
            } // end foreach
        }

    }

}
