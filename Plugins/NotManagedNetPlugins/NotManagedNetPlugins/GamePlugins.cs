using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using PluginInterface;

namespace NotManagedNetPlugins
{
    public class GamePlugins : IGamePlugin
    {
        #region Plugins to load
        [DllImport("LufiaCurseSinistrals.dll")]
        public unsafe static extern bool XIsCompatible(char* gameCode);
        [DllImport("LufiaCurseSinistrals.dll")]
        public unsafe static extern char* XGetFormat(char* filePath, int id);
        [DllImport("LufiaCurseSinistrals.dll")]
        public unsafe static extern char** XDecompress(char* file1, char* file2, char* file3, int id, int* num_files);
        #endregion

        IPluginHost pluginHost;
        string gameCode;
        int pluginLoaded;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public unsafe bool EsCompatible()
        {
            fixed (char* gameCodeP = gameCode)
            {
                if (File.Exists("LufiaCurseSinistrals.dll"))
                {
                    if (XIsCompatible(gameCodeP))
                    {
                        pluginLoaded = 0;
                        return true;
                    }
                }
            }

            return false;
        }

        public unsafe Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            fixed (char* filePath = nombre)
            {
                switch (pluginLoaded)
                {
                    case 0:
                        return StringToFormat(new String(XGetFormat(filePath, id)));
                }
            }

            return Formato.Desconocido;
        }
        
        public unsafe void Leer(string archivo, int id)
        {
            fixed (char* filePath = archivo.ToCharArray())
            {
                switch (pluginLoaded)
                {
                    case 0:
                        if (id == 0x15)
                        {
                            fixed (char* file2 = pluginHost.Search_File(0x16), file3 = pluginHost.Search_File(0x17))
                            {
                                int num;
                                char** decompressedFiles = XDecompress(filePath, file2, file3, id, &num);
                                Carpeta decompressedFolder = Get_DecompressedFiles(decompressedFiles, num);
                                pluginHost.Set_Files(decompressedFolder);
                            }
                        }
                        break;
                }
            }
        }
        public unsafe System.Windows.Forms.Control Show_Info(string archivo, int id)
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
        public unsafe Carpeta Get_DecompressedFiles(char** filesP, int num)
        {
            String[] files = new string[num];
            Carpeta decompressed = new Carpeta();
            decompressed.files = new List<Archivo>();
            for (int i = 0; i < num; i++)
            {
                files[i] = new String(filesP[i]);
                Archivo newFile = new Archivo();
                newFile.name = Path.GetFileName(files[i]);
                newFile.offset = 0x00;
                newFile.path = files[i];
                newFile.size = (uint)new FileInfo(files[i]).Length;
                decompressed.files.Add(newFile);
            }

            return decompressed;
        }
    }
}
