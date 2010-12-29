using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace Tinke
{
    public class PluginHost : IPluginHost
    {
        NCLR paleta;
        NCGR tile;
        NSCR screen;
        NCER celda;
        NANR animacion;
        int lastID;

        Archivo[] extraidos;
        string tempFolder;

        public PluginHost()
        {
            // Se crea una carpeta temporal donde almacenar los archivos de salida como los descomprimidos.
            string[] subFolders = System.IO.Directory.GetDirectories(Application.StartupPath);
            for (int n = 0; ; n++)
            {
                if (!subFolders.Contains<string>(Application.StartupPath + "\\Temp" + n))
                {
                    tempFolder = Application.StartupPath + "\\Temp" + n;
                    System.IO.Directory.CreateDirectory(tempFolder);
                    break;
                }
            }

        }
        ~PluginHost()
        {
            System.IO.Directory.Delete(tempFolder, true);
        }

        public NCLR Get_NCLR() { return paleta; }
        public NCGR Get_NCGR() { return tile; }
        public NSCR Get_NSCR() { return screen; }
        public NCER Get_NCER() { return celda; }
        public NANR Get_NANR() { return animacion; }

        public void Set_NCLR(NCLR nclr) { paleta = nclr; }
        public void Set_NCGR(NCGR ncgr) { tile = ncgr; }
        public void Set_NSCR(NSCR nscr) { screen = nscr; }
        public void Set_NCER(NCER ncer) { celda = ncer; }
        public void Set_NANR(NANR nanr) { animacion = nanr; }

        public void Set_Files(Archivo[] archivos)
        {
            extraidos = archivos;
        }
        public Archivo[] Get_Files()
        {
            Archivo[] devuelta = extraidos;
            extraidos = null;
            return devuelta;
        }
        public string Get_TempFolder()
        {
            return tempFolder;
        }
    }
}
