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

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public bool IsCompatible()
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

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            switch (pluginLoaded)
            {
                case 0:
                    IntPtr p = XGetFormat(nombre, id);
                    string c = Marshal.PtrToStringAnsi(p);
                    return StringToFormat(c);
            }

            return Format.Unknown;
        }

        public unsafe void Read(string archivo, int id)
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
                        sFolder decompressedFolder = Get_DecompressedFiles(txtfile, num);
                        DateTime t3 = DateTime.Now;

                        pluginHost.Set_Files(decompressedFolder);
                    }
                    break;
            }
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            return new System.Windows.Forms.Control();
        }

        public Format StringToFormat(string format)
        {
            switch (format)
            {
                case "Palette":
                    return Format.Palette;
                case "Tile":
                    return Format.Tile;
                case "Map":
                    return Format.Map;
                case "Cell":
                    return Format.Cell;
                case "Animation":
                    return Format.Animation;
                case "FullImage":
                    return Format.FullImage;
                case "Text":
                    return Format.Text;
                case "Video":
                    return Format.Video;
                case "Sound":
                    return Format.Sound;
                case "Font":
                    return Format.Font;
                case "Compress":
                    return Format.Compressed;
                case "Script":
                    return Format.Script;
                case "System":
                    return Format.System;
                case "Unknown":
                    return Format.Unknown;
                default:
                    return Format.Unknown;
            }
        }
        public sFolder Get_DecompressedFiles(string txtFile, int num)
        {
            String[] files = File.ReadAllLines(txtFile);
            sFolder decompressed = new sFolder();
            decompressed.files = new List<sFile>();

            String currFolder = files[0].Substring((pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar).Length);
            currFolder = currFolder.Substring(0, currFolder.IndexOf(Path.DirectorySeparatorChar));
            String relativePath = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + currFolder;
            currFolder = "";

            decompressed = Recursive_GetDirectories(relativePath, decompressed);

            return decompressed;
        }

        /// <summary>
        /// Get a "sFolder" variable with all files and folders from the main folder path.
        /// </summary>
        /// <param name="folderPath">Folder to read</param>
        /// <param name="currFolder">Empty folder</param>
        /// <returns></returns>
        public sFolder Recursive_GetDirectories(string folderPath, sFolder currFolder)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<sFile>))
                    currFolder.files = new List<sFile>();
                currFolder.files.Add(newFile);
            }

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                sFolder newFolder = new sFolder();
                newFolder.name = new DirectoryInfo(folder).Name;
                newFolder = Recursive_GetDirectories(folder, newFolder);

                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();
                currFolder.folders.Add(newFolder);
            }

            return currFolder;
        }
        /// <summary>
        /// Include a file in a "sFolder" variable.
        /// </summary>
        /// <param name="file">Path of the file to include</param>
        /// <param name="currFolder">Root "sFolder" variable to include it</param>
        /// <param name="pathFolder">Empty string</param>
        /// <param name="relativePath">Path of the first folder where all files and folders are.</param>
        /// <returns></returns>
        public sFolder Recursive_GetDirectories(string file, sFolder currFolder, string pathFolder, string relativePath)
        {
            String directoryPath = new FileInfo(file).DirectoryName;
            if (directoryPath.Replace(relativePath, "") == pathFolder)
            {
                sFile newFile = new sFile();
                newFile.name = Path.GetFileName(file);
                newFile.offset = 0x00;
                newFile.path = file;
                newFile.size = (uint)new FileInfo(file).Length;

                if (!(currFolder.files is List<sFile>))
                    currFolder.files = new List<sFile>();
                currFolder.files.Add(newFile);
                return currFolder;
            }
            else
            {
                sFolder newFolder = new sFolder(); ;
                if (currFolder.folders is List<sFolder>)
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
                if (!(currFolder.folders is List<sFolder>))
                    currFolder.folders = new List<sFolder>();

                pathFolder += Path.DirectorySeparatorChar + newFolder.name;
                newFolder = Recursive_GetDirectories(file, newFolder, pathFolder, relativePath);
                currFolder.folders.Add(newFolder);
                return currFolder;
            }
        }
    }
}
