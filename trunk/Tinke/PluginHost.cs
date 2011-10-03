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
        NSCR map;
        NCER celda;
        NANR animacion;

        sFolder extraidos;
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
        public void Dispose()
        {
            try { System.IO.Directory.Delete(tempFolder, true); }
            catch { MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S22")); }
        }

        public NCLR Get_NCLR() { return paleta; }
        public NCGR Get_NCGR() { return tile; }
        public NSCR Get_NSCR() { return map; }
        public NCER Get_NCER() { return celda; }
        public NANR Get_NANR() { return animacion; }

        public void Set_NCLR(NCLR nclr) { paleta = nclr; }
        public void Set_NCGR(NCGR ncgr) { tile = ncgr; }
        public void Set_NSCR(NSCR nscr) { map = nscr; }
        public void Set_NCER(NCER ncer) { celda = ncer; }
        public void Set_NANR(NANR nanr) { animacion = nanr; }

        public Color[] BGR555(byte[] datos) { return Convertir.BGR555(datos); }
        public Byte[] ColorToBGR555(Color[] color) { return Convertir.ColorToBGR555(color); }
        public Byte[] BytesTo4BitsRev(byte[] datos) { return Tools.Helper.BytesTo4BitsRev(datos); }
        public String BytesToBits(byte[] datos) { return Tools.Helper.BytesToBits(datos); }
        public Byte[] Bit4ToBit8(byte[] bits4) { return Convertir.Bit4ToBit8(bits4); }
        public Byte[] Bit8ToBit4(byte[] bits8) { return Convertir.Bit8ToBit4(bits8); }
        public Byte[] TilesToBytes(byte[][] tiles) { return Convertir.TilesToBytes(tiles); }
        public Byte[][] BytesToTiles(byte[] bytes) { return Convertir.BytesToTiles(bytes); }
        public Byte[][] BytesToTiles_NoChanged(byte[] bytes, int tilesX, int tilesY) { return Convertir.BytesToTiles_NoChanged(bytes, tilesX, tilesY); }
        public TTLP Palette_4bppTo8bpp(TTLP palette) { return Convertir.Palette_4bppTo8bpp(palette); }
        public TTLP Palette_8bppTo4bpp(TTLP palette) { return Convertir.Palette_8bppTo4bpp(palette); }
        public void Change_Color(ref byte[][] tiles, int oldIndex, int newIndex) { Convertir.Change_Color(ref tiles, oldIndex, newIndex); }

        public Bitmap[] Bitmaps_NCLR(string archivo) { return Imagen_NCLR.Mostrar(archivo); }
        public Bitmap[] Bitmaps_NCLR(NCLR nclr) { return Imagen_NCLR.Mostrar(nclr); }
        public NTFT Transform_NSCR(NSCR nscr, NTFT ntft, int startOffset = 0) { return Imagen_NSCR.Modificar_Tile(nscr, ntft, startOffset); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile); }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY) { return Imagen_NCGR.Crear_Imagen(ncgr, nclr, startTile, tilesX, tilesY); }
        public Size Size_NCER(byte byte1, byte byte2) { return Imagen_NCER.Obtener_Tamaño(byte1, byte2); }
        public Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR ncgr, NCLR nclr, bool entorno, bool celda, bool numero, bool transparencia, bool imagen) 
            { return Imagen_NCER.Obtener_Imagen(banco, blockSize, ncgr, nclr, entorno, celda, numero, transparencia, imagen); }
        public Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR tile, NCLR paleta, bool entorno, bool celda, bool numero, bool transparencia,
            bool image, int maxWidth, int maxHeight) { return Imagen_NCER.Obtener_Imagen(banco, blockSize, tile, paleta, entorno, celda, numero, transparencia, image, maxWidth, maxHeight); }

        public void Create_APNG(string fileout, Bitmap[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }
        public void Create_APNG(string fileout, String[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }

        public void Set_Files(sFolder archivos)
        {
            extraidos = archivos;
        }
        public sFolder Get_Files()
        {
            sFolder devuelta = extraidos;
            extraidos = new sFolder();
            return devuelta;
        }
        public event Func<int, sFolder> event_GetDecompressedFiles;
        public sFolder Get_DecompressedFiles(int id) { return event_GetDecompressedFiles(id); }
        public event Func<int, String> event_SearchFile;
        public String Search_File(int id) { return event_SearchFile(id); }
        public event Func<int, sFile> event_SearchFile2;
        public sFile Search_File(short id) { return event_SearchFile2(id); }

        public string Get_TempFolder()
        {
            return tempFolder;
        }
        public string Get_Language()
        {
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "Tinke.xml");
            return xml.Element("Options").Element("Language").Value;
        }

        public event Action<string> DescompressEvent;
        public void Decompress(string archivo)
        {
            DescompressEvent(archivo);
        }
        public void Decompress(byte[] data)
        {
        	string temp = System.IO.Path.GetTempFileName();
        	System.IO.File.WriteAllBytes(temp, data);
        	DescompressEvent(temp);
        	System.IO.File.Delete(temp);
        }
        public void Compress(string filein, string fileout, FormatCompress format) { DSDecmp.Main.Compress(filein, fileout, format); }

        public event Action<int, string> ChangeFile_Event;
        public void ChangeFile(int id, string newFile) { ChangeFile_Event(id, newFile); }

        public NCLR BitmapToPalette(string bitmap) { return Imagen_NCLR.BitmapToPalette(bitmap); }
        public NCGR BitmapToTile(string bitmap, TileOrder tileOrder) { return Imagen_NCGR.BitmapToTile(bitmap, tileOrder); }
        public NSCR Create_BasicMap(int nTiles, int width, int height) { return Imagen_NSCR.Create_BasicMap(nTiles, width, height); }

    }
}
