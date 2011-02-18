using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
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

        Carpeta extraidos;
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
            try { System.IO.Directory.Delete(tempFolder, true); }
            catch { MessageBox.Show("No se pudo eliminar la carpeta temporal."); }
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

        public Color[] BGR555(byte[] datos) { return Convertir.BGR555(datos); }
        public Byte[] BytesTo4BitsRev(byte[] datos) { return Tools.Helper.BytesTo4BitsRev(datos); }
        public String BytesToBits(byte[] datos) { return Tools.Helper.BytesToBits(datos); }

        public NTFT Transformar_NSCR(NSCR nscr, NTFT ntft) { return Imagen_NSCR.Modificar_Tile(nscr, ntft); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile, tilesX, tilesY); }
        public Size Tamaño_NCER(byte byte1, byte byte2) { return Imagen_NCER.Obtener_Tamaño(byte1, byte2); }
        public Bitmap Bitmap_NCER(Bank banco, NCGR ncgr, NCLR nclr, bool entorno, bool celda, bool numero, bool transparencia) 
            { return Imagen_NCER.Obtener_Imagen(banco, ncgr, nclr, entorno, celda, numero, transparencia); }

        public void Set_Files(Carpeta archivos)
        {
            extraidos = archivos;
        }
        public Carpeta Get_Files()
        {
            Carpeta devuelta = extraidos;
            extraidos = new Carpeta();
            return devuelta;
        }
        public string Get_TempFolder()
        {
            return tempFolder;
        }

        public event Action<string, byte> DescomprimirEvent;
        public void Descomprimir(string archivo)
        {
            DescomprimirEvent(archivo, 0x00);
        }
        public void Descomprimir(byte[] datos)
        {
        	string temp = System.IO.Path.GetTempFileName();
        	System.IO.File.WriteAllBytes(temp, datos);
        	DescomprimirEvent(temp, 0x00);
        	System.IO.File.Delete(temp);
        }
        public void Descomprimir(byte[] datos, byte tag)
        {
        	string temp = System.IO.Path.GetTempFileName();
        	System.IO.File.WriteAllBytes(temp, datos);
        	DescomprimirEvent(temp, tag);
        	System.IO.File.Delete(temp);
        }
    }
}
