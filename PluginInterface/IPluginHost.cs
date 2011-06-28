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
        String BytesToBits(byte[] datos);
        Byte[] Bit4ToBit8(byte[] bits4);
        Byte[] Bit8ToBit4(byte[] bits8);
        Byte[] TilesToBytes(byte[][] tiles);
        Byte[][] BytesToTiles(byte[] bytes);

        Bitmap[] Bitmaps_NCLR(string archivo);
        Bitmap[] Bitmaps_NCLR(NCLR nclr);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile);
        Bitmap Bitmap_NCGR(NCGR ncgr, NCLR nclr, int startTile, int tilesX, int tilesY);
        NTFT Transformar_NSCR(NSCR nscr, NTFT ntft);
        Size Tamaño_NCER(byte byte1, byte byte2);
        Bitmap Bitmap_NCER(Bank banco, uint blockSize, NCGR ncgr, NCLR nclr, bool entorno, bool celda,
            bool numero, bool transparencia, bool imagen);
        /// <summary>
        /// Save an animation in a APNG file (Firefox supported)
        /// </summary>
        /// <param name="salida">The path of the output file</param>
        /// <param name="frames">All frames (path of files or bitmaps)</param>
        /// <param name="delay">The delay between frames (delay/1000)</param>
        /// <param name="loops">The number of  loops (if 0 = infinite)</param>
        void Crear_APNG(string salida, Bitmap[] frames, int delay, int loops);
        void Crear_APNG(string salida, String[] frames, int delay, int loops);

        // Para descomprimir archivos
        void Set_Files(Carpeta archivos);
        Carpeta Get_Files();

        string Get_Language();
        string Get_TempFolder();

        event Action<string, byte>  DescomprimirEvent;
        void Descomprimir(string archivo);
        void Descomprimir(byte[] datos);
        void Descomprimir(byte[] datos, byte tag);
    }
}
