using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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

        
        Color[] BGR555(byte[] datos);
        Byte[] BytesTo4BitsRev(byte[] datos);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int tilesX, int tilesY);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY);
        NTFT Transformar_NSCR(NSCR nscr, NTFT ntft);
        Size Tamaño_NCER(byte byte1, byte byte2);
        Bitmap Bitmap_NCER(Bank celda, NCGR ncgr, NCLR nclr);

        // Para descomprimir archivos
        void Set_Files(Carpeta archivos);
        Carpeta Get_Files();

        string Get_TempFolder();

        event Func<String, String> DescomprimirEvent;
        string Descomprimir(string archivo);
    }
}
