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
using PluginInterface;

namespace AI_IGO_DS
{
    public static class ANCG
    {
        public static void Leer(string archivo, IPluginHost pluginHost)
        {
            NCGR imagen = new NCGR();
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            imagen.order = TileOrder.Horizontal;
            // Cabecera genérica
            imagen.header.id = br.ReadChars(4);
            imagen.header.endianess = 0xFFFE;
            imagen.header.constant = 0x0100;
            imagen.header.file_size = (uint)br.BaseStream.Length;
            imagen.header.header_size = 0x08;
            imagen.header.nSection = 1;
            // Tile data
            imagen.rahc.id = imagen.header.id;
            imagen.rahc.size_tiledata = br.ReadUInt32();
            imagen.rahc.size_section = imagen.rahc.size_tiledata;
            imagen.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            imagen.rahc.nTiles = (ushort)(imagen.rahc.size_tiledata / 32);
            imagen.rahc.nTilesX = 8;
            imagen.rahc.nTilesY = (ushort)(imagen.rahc.nTiles / 8);
            imagen.rahc.tiledFlag = 0x00;

            imagen.rahc.tileData.tiles = new byte[imagen.rahc.nTiles][];
            imagen.rahc.tileData.nPalette = new byte[imagen.rahc.nTiles];
            for (int i = 0; i < imagen.rahc.nTiles; i++)
            {
                imagen.rahc.tileData.tiles[i] = pluginHost.BytesTo4BitsRev(br.ReadBytes(32));
                imagen.rahc.tileData.nPalette[i] = 0;
            }


            pluginHost.Set_NCGR(imagen);
            br.Close();
        }
    }
}
