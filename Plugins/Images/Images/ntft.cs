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
 * Fecha: 1/07/2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Images
{
    public class ntft
    {
        IPluginHost pluginsHost;
		string archivo;
        int id;
		
		public ntft(IPluginHost pluginHost, string archivo, int id)
		{
			this.pluginsHost = pluginHost;
			this.archivo = archivo;
            this.id = id;
		}
		
		public void Leer()
		{
			BinaryReader br = new BinaryReader(File.OpenRead(archivo));
			uint file_size = (uint)new FileInfo(archivo).Length;

			// Creamos un archivo NCGR genérico.
			NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;
			ncgr.cabecera.id = "NTFT".ToCharArray();
			ncgr.cabecera.nSection = 1;
			ncgr.cabecera.constant = 0x0100;
			ncgr.cabecera.file_size = file_size;
			// El archivo es NTFT raw, sin ninguna información.
			ncgr.orden = Orden_Tiles.No_Tiles;
			ncgr.rahc.nTiles = (ushort)(0xC000);
			ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
			ncgr.rahc.nTilesX = 0x0100;
			ncgr.rahc.nTilesY = 0x00C0;
			ncgr.rahc.tiledFlag = 0x00000001;
			ncgr.rahc.size_section = file_size;
			ncgr.rahc.tileData = new NTFT();
			ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
			ncgr.rahc.tileData.tiles = new byte[1][];
            ncgr.rahc.tileData.tiles[0] = br.ReadBytes((int)ncgr.rahc.nTiles);

			for (int i = 0; i < ncgr.rahc.nTiles; i++)
			{
				ncgr.rahc.tileData.nPaleta[i] = 0;
			}
			
			br.Close();
			pluginsHost.Set_NCGR(ncgr);
		}
    }
}
