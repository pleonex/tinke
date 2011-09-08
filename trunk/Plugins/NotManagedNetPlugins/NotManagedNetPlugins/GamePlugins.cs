using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginInterface;

namespace NotManagedNetPlugins
{
    public class GamePlugins : IGamePlugin
    {
        #region Plugins to load
        [DllImport("LufiaCurseSinistrals.dll")]
        public static extern bool XIsCompatible(string GameCode);
        [DllImport("LufiaCurseSinistrals.dll", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern System.IntPtr XGetFormat(string filePath, int id);
        [DllImport("LufiaCurseSinistrals.dll", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool XDecompress(string file1, string file2, string file3, string id, int* num_files);
        #endregion

        IPluginHost pluginHost;
        string gameCode;
        int pluginLoaded;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public bool EsCompatible()
        {
            if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar +
                "LufiaCurseSinistrals.dll"))
            {
                if (XIsCompatible(gameCode))
                {
                    pluginLoaded = 0;
                    return true;
                }
            }

            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            switch (pluginLoaded)
            {
                case 0:
                    IntPtr p = XGetFormat(nombre, id);
                    string c = Marshal.PtrToStringAnsi(p);
                    return StringToFormat(c);
            }

            return Formato.Desconocido;
        }

        public unsafe void Leer(string archivo, int id)
        {
            switch (pluginLoaded)
            {
                case 0:
                    if (id == 0x15)
                    {
                        int num = 0;
                        DateTime t1 = DateTime.Now;
                        bool b = XDecompress(archivo, pluginHost.Search_File(0x16), pluginHost.Search_File(0x17), id.ToString(), &num);

                        DateTime t2 = DateTime.Now;
                        String txtfile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "tinke_file_list.txt";
                        Carpeta decompressedFolder = Get_DecompressedFiles(txtfile, num);
                        DateTime t3 = DateTime.Now;

                        pluginHost.Set_Files(decompressedFolder);

                        Console.WriteLine("Unpacking files {0}", (t2 - t1).ToString());
                        Console.WriteLine("Reading new files {0}", (t3 - t2).ToString());
                        Console.WriteLine("Total time {0}", (t3 - t1).ToString());
                    }
                    break;
            }
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            return new System.Windows.Forms.Control();
        }

        public Formato StringToFormat(string format)
        {
            switch (format)
            {
                case "Palette":
                    return Formato.Paleta;
                case "Tile":
                    return Formato.Imagen;
                case "Map":
                    return Formato.Map;
                case "Cell":
                    return Formato.Celdas;
                case "Animation":
                    return Formato.Animación;
                case "FullImage":
                    return Formato.ImagenCompleta;
                case "Text":
                    return Formato.Texto;
                case "Video":
                    return Formato.Video;
                case "Sound":
                    return Formato.Sonido;
                case "Font":
                    return Formato.Fuentes;
                case "Compress":
                    return Formato.Comprimido;
                case "Script":
                    return Formato.Script;
                case "System":
                    return Formato.Sistema;
                case "Unknown":
                    return Formato.Desconocido;
                default:
                    return Formato.Desconocido;
            }
        }
        public Carpeta Get_DecompressedFiles(string txtFile, int num)
        {
            String[] files = File.ReadAllLines(txtFile);
            Carpeta decompressed = new Carpeta();
            decompressed.files = new List<Archivo>();

            String currFolder = files[0].Substring((pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar).Length);
            currFolder = currFolder.Substring(0, currFolder.IndexOf(Path.DirectorySeparatorChar));
            String relativePath = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + currFolder;
            currFolder = "";

            decompressed = Recursive_GetDirectories(relativePath, decompressed);

            return decompressed;
        }

        /// <summary>
        /// Get a "Carpeta" variable with all files and folders from the main folder path.
        /// </summary>
        /// <param name="folderPath">Folder to read</param>
        /// <param name="currFolder">Empty folder</param>
        /// <returns></returns>
        public Carpeta Recursive_GetDirectories(string folderPath, Carpeta currFolder)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                Archivo newFile = new Archivo();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<Archivo>))
                    currFolder.files = new List<Archivo>();
                currFolder.files.Add(newFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                Carpeta newFolder = new Carpeta();
                newFolder.name = new DirectoryInfo(folder).Name;
                newFolder = Recursive_GetDirectories(folder, newFolder);

                if (!(currFolder.folders is List<Carpeta>))
                    currFolder.folders = new List<Carpeta>();
                currFolder.folders.Add(newFolder);
            }

            return currFolder;
        }
        /// <summary>
        /// Include a file in a "Carpeta" variable.
        /// </summary>
        /// <param name="file">Path of the file to include</param>
        /// <param name="currFolder">Root "Carpeta" variable to include it</param>
        /// <param name="pathFolder">Empty string</param>
        /// <param name="relativePath">Path of the first folder where all files and folders are.</param>
        /// <returns></returns>
        public Carpeta Recursive_GetDirectories(string file, Carpeta currFolder, string pathFolder, string relativePath)
        {
            String directoryPath = new FileInfo(file).DirectoryName;
            if (directoryPath.Replace(relativePath, "") == pathFolder)
            {
                Archivo newFile = new Archivo();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<Archivo>))
                    currFolder.files = new List<Archivo>();
                currFolder.files.Add(newFile);
                return currFolder;
            }
            else
            {
                Carpeta newFolder = new Carpeta(); ;
                if (currFolder.folders is List<Carpeta>)
                {
                    string folderName = file.Replace(relativePath + pathFolder, "");
                    folderName = folderName.Substring(1, folderName.Substring(2).IndexOf(Path.DirectorySeparatorChar) + 1);

                    int i;
                    for (i = 0; i < currFolder.folders.Count; i++)
                    {
                        if (currFolder.folders[i].name == folderName)
                        {
                            newFolder = currFolder.folders[i];
                            break;
                        }
                    }

                    if (!(newFolder.name is String))
                        goto Create_Folder;

                    pathFolder += Path.DirectorySeparatorChar + newFolder.name;
                    newFolder = Recursive_GetDirectories(file, newFolder, pathFolder, relativePath);
                    currFolder.folders[i] = newFolder;

                    return currFolder;
                }

            Create_Folder:
                newFolder.name = file.Replace(relativePath + pathFolder, "");
                newFolder.name = newFolder.name.Substring(1, newFolder.name.Substring(2).IndexOf(Path.DirectorySeparatorChar) + 1);
                if (!(currFolder.folders is List<Carpeta>))
                    currFolder.folders = new List<Carpeta>();

                pathFolder += Path.DirectorySeparatorChar + newFolder.name;
                newFolder = Recursive_GetDirectories(file, newFolder, pathFolder, relativePath);
                currFolder.folders.Add(newFolder);
                return currFolder;
            }
        }
    }
}
