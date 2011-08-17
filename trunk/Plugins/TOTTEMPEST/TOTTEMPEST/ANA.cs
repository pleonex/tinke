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

namespace TOTTEMPEST
{
    public static class ANA
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            uint file_size = (uint)new FileInfo(file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCGR ncgr = new NCGR();

            // Common header
            ncgr.id = (uint)id;
            ncgr.cabecera.id = "ANA ".ToCharArray();
            ncgr.cabecera.nSection = 1;
            ncgr.cabecera.constant = 0x0100;
            ncgr.cabecera.file_size = file_size;
            
            // RAHC section
            ncgr.orden = Orden_Tiles.Horizontal;
            ncgr.rahc.nTiles = (ushort)br.ReadUInt16();
            ncgr.rahc.depth = (br.ReadUInt16() == 0x01 ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit);
            if (file_size - 4 == ncgr.rahc.nTiles * 0x40)
                ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;

            ncgr.rahc.nTilesX = 0x08;
            if (ncgr.rahc.nTiles == 0x10)   // All the images with 0x10 tiles are 32x32
                ncgr.rahc.nTilesX = 0x04;
            ncgr.rahc.nTilesY = (ushort)(ncgr.rahc.nTiles / ncgr.rahc.nTilesX);
            if (ncgr.rahc.nTilesY == 0x00)
                ncgr.rahc.nTilesY = 0x01;

            ncgr.rahc.tiledFlag = 0x00000000;
            ncgr.rahc.size_section = file_size;

            // Tile data
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                if (ncgr.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    ncgr.rahc.tileData.tiles[i] = pluginHost.BytesTo4BitsRev(br.ReadBytes(32));
                else
                    ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            br.Close();
            pluginHost.Set_NCGR(ncgr);

            // If the image is 8bpp, convert to 8bpp the palette (4bpp per default)
            if (ncgr.rahc.depth == System.Windows.Forms.ColorDepth.Depth8Bit &&
                pluginHost.Get_NCLR().cabecera.file_size != 0x00)
            {
                NCLR paleta = pluginHost.Get_NCLR();
                paleta.pltt = pluginHost.Palette_4bppTo8bpp(paleta.pltt);
                pluginHost.Set_NCLR(paleta);
            }
        }
        public static void Write(NCGR tile, string fileout, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
        
            bw.Write(tile.rahc.nTiles);
            bw.Write((ushort)(tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit ? 0x00 : 0x01));
            // Write the tile data
            for (int i = 0; i < tile.rahc.tileData.tiles.Length; i++)
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    bw.Write(pluginHost.Bit4ToBit8(tile.rahc.tileData.tiles[i]));
                else
                    bw.Write(tile.rahc.tileData.tiles[i]);

            bw.Flush();
            bw.Close();
        }
    }
}
