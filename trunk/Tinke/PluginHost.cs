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
                if (!subFolders.Contains<string>(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "Temp" + n))
                {
                    tempFolder = Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "Temp" + n;
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
        public Byte[] Bit4ToBit8(byte[] bits4) { return Convertir.Bit4ToBit8(bits4); }
        public Byte[] Bit8ToBit4(byte[] bits8) { return Convertir.Bit8ToBit4(bits8); }
        public Byte[] TilesToBytes(byte[][] tiles) { return Convertir.TilesToBytes(tiles); }
        public Byte[][] BytesToTiles(byte[] bytes) { return Convertir.BytesToTiles(bytes); }

        public Bitmap[] Bitmaps_NCLR(string archivo) { return Imagen_NCLR.Mostrar(archivo); }
        public Bitmap[] Bitmaps_NCLR(NCLR nclr) { return Imagen_NCLR.Mostrar(nclr); }
        public NTFT Transformar_NSCR(NSCR nscr, NTFT ntft) { return Imagen_NSCR.Modificar_Tile(nscr, ntft); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile, tilesX, tilesY); }
        public Size Tamaño_NCER(byte byte1, byte byte2) { return Imagen_NCER.Obtener_Tamaño(byte1, byte2); }
        public Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR ncgr, NCLR nclr, bool entorno, bool celda, bool numero, bool transparencia, bool imagen) 
            { return Imagen_NCER.Obtener_Imagen(banco, blockSize, ncgr, nclr, entorno, celda, numero, transparencia, imagen); }
        public void Crear_APNG(string salida, Bitmap[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, salida, delay, loops); }
        public void Crear_APNG(string salida, String[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, salida, delay, loops); }

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
        public string Get_Language()
        {
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "Tinke.xml");
            return xml.Element("Options").Element("Language").Value;
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

        public event Action<int, string> ChangeFile_Event;
        public void ChangeFile(int id, string newFile)
        {
            ChangeFile_Event(id, newFile);
        }
    }
}
