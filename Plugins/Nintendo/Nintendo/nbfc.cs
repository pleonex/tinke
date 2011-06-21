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
 * Programa utilizado: SharpDevelop
 * Fecha: 16/02/2011
 * 
 */
using System;
using System.IO;
using PluginInterface;

namespace Nintendo
{
	/// <summary>
	/// Description of nbfc.
	/// </summary>
	public class nbfc
	{
		IPluginHost pluginsHost;
		string archivo;
		
		public nbfc(IPluginHost pluginHost, string archivo)
		{
			this.pluginsHost = pluginHost;
			this.archivo = archivo;
		}
		
		public void Leer()
		{
			BinaryReader br = new BinaryReader(File.OpenRead(archivo));
			uint file_size = (uint)new FileInfo(archivo).Length;

			// Creamos un archivo NCGR genérico.
			NCGR ncgr = new NCGR();
			ncgr.cabecera.id = "NCGR".ToCharArray();
			ncgr.cabecera.nSection = 1;
			ncgr.cabecera.constant = 0x0100;
			ncgr.cabecera.file_size = file_size;
			// El archivo es NTFT raw, sin ninguna información.
			ncgr.orden = Orden_Tiles.Horizontal;
			ncgr.rahc.nTiles = (ushort)(file_size / 64);
			ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
			ncgr.rahc.nTilesX = 0x0020;
			ncgr.rahc.nTilesY = 0x0018;
			ncgr.rahc.padding = 0x00000000;
			ncgr.rahc.size_section = file_size;
			ncgr.rahc.tileData = new NTFT();
			ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
			ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];
			
			for (int i = 0; i < ncgr.rahc.nTiles; i++)
			{
				ncgr.rahc.tileData.tiles[i] = br.ReadBytes(64);
				ncgr.rahc.tileData.nPaleta[i] = 0;
			}
			
			br.Close();
			pluginsHost.Set_NCGR(ncgr);
		}
		
	}
}
