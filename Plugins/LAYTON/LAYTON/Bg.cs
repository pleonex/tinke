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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
{
    public class Bg
    {
        IPluginHost pluginHost;
        string gameCode;
        string archivo;

        public Bg(IPluginHost pluginHost, string gameCode, string archivo)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
            this.archivo = archivo;
        }

        public Format Get_Formato(string nombre)
        {
            if (nombre.EndsWith(".ARC") || nombre.EndsWith(".BGX") || nombre.EndsWith(".ARB"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Leer()
        {
        }
        public Control Show_Info()
        {
            // Los archivos tienen compresión LZ77, descomprimimos primero.
            if (archivo.ToUpper().EndsWith(".ARC"))
            {
                string temp = archivo + "nn";
                Byte[] compressFile = new Byte[(new FileInfo(archivo).Length) - 4];
                Array.Copy(File.ReadAllBytes(archivo), 4, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(temp, compressFile);

                pluginHost.Decompress(temp);
                archivo = pluginHost.Get_Files().files[0].path;
                File.Delete(temp);
            }

            InfoBG control = new InfoBG(pluginHost, Obtener_Background());
            File.Delete(archivo);

            return control;
        }

        public Bitmap Obtener_Background()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            // Paleta, NCLR sin cabecera
            NCLR paleta = new NCLR();
            paleta.pltt.nColors = br.ReadUInt32();
            paleta.pltt.palettes = new NTFP[1];
            paleta.pltt.palettes[0].colors =
                pluginHost.BGR555ToColor(br.ReadBytes((int)paleta.pltt.nColors * 2));

            // Tile, sin cabecera
            NCGR tile = new NCGR();
            tile.rahc.depth = ColorDepth.Depth8Bit;
            tile.rahc.nTiles = (ushort)br.ReadUInt32();
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            for (int i = 0; i < tile.rahc.nTiles; i++)
                tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
            tile.order = TileOrder.Horizontal;

            // Tile Map Info
            NSCR map = new NSCR();
            map.section.width = (ushort)(br.ReadUInt16() * 8);
            map.section.height = (ushort)(br.ReadUInt16() * 8);
            tile.rahc.nTilesX = (ushort)(map.section.width / 8);
            tile.rahc.nTilesY = (ushort)(map.section.height / 8);
            map.section.mapData = new NTFS[map.section.width * map.section.height / 64];
            for (int i = 0; i < map.section.width * map.section.height / 64; i++)
            {
                ushort parameters = br.ReadUInt16();

                map.section.mapData[i] = new NTFS();
                map.section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                map.section.mapData[i].xFlip = (byte)((parameters >> 10) & 1);
                map.section.mapData[i].yFlip = (byte)((parameters >> 11) & 1);
                map.section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
            }

            br.Close();

            tile.rahc.tileData = pluginHost.Transform_NSCR(map, tile.rahc.tileData);
            return pluginHost.Bitmap_NCGR(tile, paleta);
        }
    }
}
