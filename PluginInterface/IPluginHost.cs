using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginInterface
{
    public interface IPluginHost
    {
        NCLR Get_NCLR();
        NCGR Get_NCGR();
        NSCR Get_NSCR();
        NCER Get_NCER();
        NANR Get_NANR();

        void Set_NCLR(NCLR nclr);
        void Set_NCGR(NCGR ncgr);
        void Set_NSCR(NSCR nscr);
        void Set_NCER(NCER ncer);
        void Set_NANR(NANR nanr);

        // Para descomprimir archivos
        void Set_Files(Archivo[] archivos);
        Archivo[] Get_Files();
        string Get_TempFolder();
    }
}
