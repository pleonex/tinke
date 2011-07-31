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

namespace Images
{
	/// <summary>
	/// Description of nbfs.
	/// </summary>
	public class nbfs
	{
		IPluginHost pluginsHost;
		string archivo;
        int id;
		
		public nbfs(IPluginHost pluginHost, string archivo, int id)
		{
			this.pluginsHost = pluginHost;
			this.archivo = archivo;
            this.id = id;
		}
		
		public void Leer()
		{
			BinaryReader br = new BinaryReader(File.OpenRead(archivo));
			uint file_size = (uint)new FileInfo(archivo).Length;
			
			// Su formato es NTFS raw, sin información, nos la inventamos por tanto
            NSCR nscr = new NSCR();
            nscr.id = (uint)id;

            // Lee cabecera genérica
            nscr.cabecera.id = "NSCR".ToCharArray();
            nscr.cabecera.endianess = 0xFEFF;
            nscr.cabecera.constant = 0x0100;
            nscr.cabecera.file_size = file_size;
            nscr.cabecera.header_size = 0x10;
            nscr.cabecera.nSection = 1;

            // Lee primera y única sección:
            nscr.section.id = "NSCR".ToCharArray();
            nscr.section.section_size = file_size;
            nscr.section.width = 0x0020;
            nscr.section.height = 0x0018;
            nscr.section.padding = 0x00000000;
            nscr.section.data_size = file_size;
            nscr.section.mapData = new NTFS[file_size / 2];

            for (int i = 0; i < (file_size / 2); i++)
            {
                string bits = pluginsHost.BytesToBits(br.ReadBytes(2));

                nscr.section.mapData[i] = new NTFS();
                nscr.section.mapData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                nscr.section.mapData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                nscr.section.mapData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                nscr.section.mapData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();
            pluginsHost.Set_NSCR(nscr);
		}
	}
}
