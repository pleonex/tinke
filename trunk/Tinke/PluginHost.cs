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
using PluginInterface.Images;

namespace Tinke
{
    public class PluginHost : IPluginHost
    {
        ImageBase image;
        PaletteBase palette;
        MapBase map;
        SpriteBase sprite;
        Object objects;

        sFolder extraidos;
        string tempFolder;
        string _tempFolder; // Original

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
                    _tempFolder = (string)tempFolder.Clone();
                    break;
                }
            }
        }
        public void Dispose()
        {
            try { System.IO.Directory.Delete(tempFolder, true); }
            catch { MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S22")); }
        }

        public Object Get_Object() { return objects; }

        public ImageBase Get_Image() { return image; }
        public PaletteBase Get_Palette() { return palette; }
        public MapBase Get_Map() { return map; }
        public SpriteBase Get_Sprite() { return sprite; }

        public void Set_Image(ImageBase image) { this.image = image; }
        public void Set_Palette(PaletteBase palette) { this.palette = palette; }
        public void Set_Map(MapBase map) { this.map = map; }
        public void Set_Sprite(SpriteBase sprite) { this.sprite = sprite; }

        public void Set_Object(Object objects) { this.objects = objects; }

        public Color[][] Palette_4bppTo8bpp(Color[][] palette) { return Convertir.Palette_4bppTo8bpp(palette); }
        public Color[][] Palette_8bppTo4bpp(Color[][] palette) { return Convertir.Palette_8bppTo4bpp(palette); }

        public Byte[] Bit4ToBit8(byte[] bits4) { return Convertir.Bit4ToBit8(bits4); }
        public Byte[] Bit8ToBit4(byte[] bits8) { return Convertir.Bit8ToBit4(bits8); }
        public byte[] BytesToBits(byte[] bytes) { return Tools.Helper.BytesToBits(bytes); }
        public byte[] BitsToBytes(byte[] bits) { return Tools.Helper.BitsToBytes(bits); }

        public Bitmap Bitmaps_NCLR(Color[] colors) { return Actions.Get_Image(colors); }

        public NTFT Transform_NSCR(NTFS[] map, ref NTFT ntft, int startInfo = 0)
        {
            byte[] num_palette;
            ntft.tiles = Actions.Apply_Map(map, ntft.tiles, out num_palette, 8, startInfo);
            ntft.nPalette = num_palette;

            return ntft;
        }

        public Bitmap Bitmap_NTFT(NTFT tiles, Color[][] palette, TileForm tileOrder, int startTile, int tilesX, int tilesY, int zoom = 1)
        {
            return null; // return Imagen_NCGR.Get_Image(tiles, palette, tileOrder, startTile, tilesX, tilesY, zoom);
        }
        public byte[][] MergeImage(byte[][] originalTile, byte[][] newTiles, int startTile)
        {
            return Imagen_NCGR.MergeImage(originalTile, newTiles, startTile);
        }

        public Size Get_OAMSize(byte byte1, byte byte2) { return Actions.Get_OAMSize(byte1, byte2); }

        public string[] PluginList()
        {
            return event_PluginList();
        }
        public event Func<string[]> event_PluginList;
        public Object Call_Plugin(string[] param, int id, int action)
        {
            return event_CallPlugin(param, id, action);
        }
        public event Func<string[], int, int, object> event_CallPlugin;

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
        public void Set_TempFolder(string newPath)
        {
            tempFolder = newPath;
        }
        public void Restore_TempFolder()
        {
            tempFolder = (string)_tempFolder.Clone();
        }

        public string Get_LangXML() { return Tools.Helper.Get_LangXML(); }
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

        //public NCLR BitmapToPalette(string bitmap, int paletteIndex = 0) { return Imagen_NCLR.BitmapToPalette(bitmap, paletteIndex); }
        //public NCGR BitmapToTile(string bitmap, TileForm tileOrder) { return Imagen_NCGR.BitmapToTile(bitmap, tileOrder); }
        //public NSCR Create_BasicMap(int nTiles, int width, int height) { return Imagen_NSCR.Create_BasicMap(nTiles, width, height); }

        public void Create_APNG(string fileout, Bitmap[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }
        public void Create_APNG(string fileout, String[] frames, int delay, int loops) { Tools.APNG.Crear_APNG(frames, fileout, delay, loops); }
    }
}
