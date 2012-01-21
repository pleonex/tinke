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
        NSCR map_old;
        NCER celda;
        NANR animacion;

        ImageBase image;
        PaletteBase palette;
        MapBase map;
        Object objects;

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
            catch { MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S22")); }
        }

        public NCLR Get_NCLR() { return paleta; }
        public NCGR Get_NCGR() { return tile; }
        public NSCR Get_NSCR() { return map_old; }
        public NCER Get_NCER() { return celda; }
        public NANR Get_NANR() { return animacion; }
        public Object Get_Object() { return objects; }

        public ImageBase Get_Image() { return image; }
        public PaletteBase Get_Palette() { return palette; }
        public MapBase Get_Map() { return map; }

        public void Set_Image(ImageBase image) { this.image = image; }
        public void Set_Palette(PaletteBase palette) { this.palette = palette; }
        public void Set_Map(MapBase map) { this.map = map; }

        public void Set_NCLR(NCLR nclr) { paleta = nclr; }
        public void Set_NCGR(NCGR ncgr) { tile = ncgr; }
        public void Set_NSCR(NSCR nscr) { map_old = nscr; }
        public void Set_NCER(NCER ncer) { celda = ncer; }
        public void Set_NANR(NANR nanr) { animacion = nanr; }
        public void Set_Object(Object objects) { this.objects = objects; }

        public Color[] BGR555ToColor(byte[] datos) { return Convertir.BGR555(datos); }
        public Byte[] ColorToBGR555(Color[] color) { return Convertir.ColorToBGR555(color); }
        public NTFS MapInfo(ushort value) { return Convertir.MapInfo(value); }
        public ushort MapInfo(NTFS map) { return Convertir.MapInfo(map); }
        public Byte[] TilesToBytes(byte[][] tiles, int startByte = 0) { return Convertir.TilesToBytes(tiles, startByte); }
        public Byte[][] BytesToTiles(byte[] bytes) { return Convertir.BytesToTiles(bytes); }
        public Byte[][] BytesToTiles_NoChanged(byte[] bytes, int tilesX, int tilesY) { return Convertir.BytesToTiles_NoChanged(bytes, tilesX, tilesY); }
        public TTLP Palette_4bppTo8bpp(TTLP palette) { return Convertir.Palette_4bppTo8bpp(palette); }
        public TTLP Palette_8bppTo4bpp(TTLP palette) { return Convertir.Palette_8bppTo4bpp(palette); }
        public Color[][] Palette_4bppTo8bpp(Color[][] palette) { return Convertir.Palette_4bppTo8bpp(palette); }
        public Color[][] Palette_8bppTo4bpp(Color[][] palette) { return Convertir.Palette_8bppTo4bpp(palette); }
        public void Change_Color(ref byte[][] tiles, int oldIndex, int newIndex) { Convertir.Change_Color(ref tiles, oldIndex, newIndex); }

        public Byte[] Bit4ToBit8(byte[] bits4) { return Convertir.Bit4ToBit8(bits4); }
        public Byte[] Bit8ToBit4(byte[] bits8) { return Convertir.Bit8ToBit4(bits8); }
        public byte[] BytesToBits(byte[] bytes) { return Tools.Helper.BytesToBits(bytes); }
        public byte[] BitsToBytes(byte[] bits) { return Tools.Helper.BitsToBytes(bits); }

        public Bitmap Bitmaps_NCLR(Color[] colors) { return Imagen_NCLR.Show(colors); }
        public Bitmap[] Bitmaps_NCLR(NCLR nclr) { return Imagen_NCLR.Show(nclr); }
        public int Remove_DuplicatedColors(ref NTFP palette, ref byte[][] tiles)
        {
            return Convertir.Remove_DuplicatedColors(ref palette, ref tiles);
        }
        public int Remove_DuplicatedColors(ref Color[] palette, ref byte[][] tiles)
        {
            return Convertir.Remove_DuplicatedColors(ref palette, ref tiles);
        }
        public int Remove_NotUsedColors(ref NTFP palette, ref byte[][] tiles)
        {
            return Convertir.Remove_NotUsedColors(ref palette, ref tiles);
        }
        public int Remove_NotUsedColors(ref Color[] palette, ref byte[][] tiles)
        {
            return Convertir.Remove_NotUsedColors(ref palette, ref tiles);
        }
        public void Replace_Color(ref byte[][] tiles, int oldIndex, int newIndex)
        {
            Convertir.Replace_Color(ref tiles, oldIndex, newIndex);
        }

        public NTFT Transform_NSCR(NSCR nscr, NTFT ntft, int startInfo = 0) { return Imagen_NSCR.Transform_Tile(nscr, ntft, startInfo); }
        public byte[] XFlip(byte[] tile)
        {
            return Imagen_NSCR.XFlip(tile);
        }
        public byte[] YFlip(byte[] tile)
        {
            return Imagen_NSCR.YFlip(tile);
        }

        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int zoom = 1)
        {
            return Imagen_NCGR.Get_Image(ncgr, nclr, zoom);
        }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int zoom = 1) 
        { 
            return Imagen_NCGR.Get_Image(ncgr, nclr, startTile, zoom);
        }
        public Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY, int zoom = 1)
        { 
            return Imagen_NCGR.Get_Image(ncgr, nclr, startTile, tilesX, tilesY, zoom);
        }
        public Bitmap Bitmap_NTFT(NTFT tiles, Color[][] palette, TileOrder tileOrder, int startTile, int tilesX, int tilesY, int zoom = 1)
        {
            return Imagen_NCGR.Get_Image(tiles, palette, tileOrder, startTile, tilesX, tilesY, zoom);
        }
        public byte[][] MergeImage(byte[][] originalTile, byte[][] newTiles, int startTile)
        {
            return Imagen_NCGR.MergeImage(originalTile, newTiles, startTile);
        }

        public Size Size_NCER(byte byte1, byte byte2) { return Imagen_NCER.Calculate_Size(byte1, byte2); }
        public Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR ncgr, NCLR nclr, bool entorno, bool celda, bool numero, bool transparencia, bool imagen, int zoom = 1) 
        {
            return Imagen_NCER.Get_Image(banco, blockSize, ncgr, nclr, entorno, celda, numero, transparencia, imagen, zoom);
        }
        public Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR tile, NCLR paleta, bool entorno, bool celda, bool numero, bool transparencia,
            bool image, int maxWidth, int maxHeight, int zoom = 1)
        {
            return Imagen_NCER.Get_Image(banco, blockSize, tile, paleta, entorno, celda, numero, transparencia, image, maxWidth, maxHeight, zoom);
        }

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
        public Byte[] Get_Bytes(int id, int offset, int length)
        {
            return Tools.Helper.Get_Bytes(offset, length, Search_File((short)id));
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

        public NCLR BitmapToPalette(string bitmap, int paletteIndex = 0) { return Imagen_NCLR.BitmapToPalette(bitmap, paletteIndex); }
        public NCGR BitmapToTile(string bitmap, TileOrder tileOrder) { return Imagen_NCGR.BitmapToTile(bitmap, tileOrder); }
        public NSCR Create_BasicMap(int nTiles, int width, int height) { return Imagen_NSCR.Create_BasicMap(nTiles, width, height); }

        public void Create_APNG(string fileout, Bitmap[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }
        public void Create_APNG(string fileout, String[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }

        public NCLR Read_WinPal(string fileIn, ColorDepth depth) { return Imagen_NCLR.Read_WinPal(fileIn, depth); }
        public Color[][] Read_WinPal2(string fileIn, ColorDepth depth) { return Imagen_NCLR.Read_WinPal2(fileIn, depth); }
        public void Write_WinPal(string fileOut, NCLR palette) { Imagen_NCLR.Write_WinPal(fileOut, palette); }
        public void Write_WinPal(string fileOut, Color[][] palette) { Imagen_NCLR.Write_WinPal(fileOut, palette); }
    }
}
