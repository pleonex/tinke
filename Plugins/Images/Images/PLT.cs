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
    class PLT
    {	
        IPluginHost pluginHost;
		string archivo;
        int id;
		
		public PLT(IPluginHost pluginHost, string archivo, int id)
		{
			this.pluginHost = pluginHost;
			this.archivo = archivo;
            this.id = id;
		}
		
		public void Leer()
		{
			uint file_size = (uint)new FileInfo(archivo).Length;
			BinaryReader br = new BinaryReader(File.OpenRead(archivo));
			
			NCLR nclr = new NCLR();
            nclr.id = (uint)id;
			// Ponemos una cabecera genérica
			nclr.cabecera.id = "PLT".ToCharArray();
			nclr.cabecera.constant = 0x0100;
			nclr.cabecera.file_size = file_size;
			nclr.cabecera.header_size = 0x10;
			// El archivo es PLTT raw, es decir, exclusivamente colores
			nclr.pltt.ID = "PLTT".ToCharArray();
			nclr.pltt.tamaño = file_size;
			nclr.pltt.profundidad = (file_size > 0x20) ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit;
			nclr.pltt.unknown1 = 0x00000000;
			nclr.pltt.tamañoPaletas = file_size;
			nclr.pltt.paletas = new NTFP[1];
			// Rellenamos los colores en formato BGR555
			nclr.pltt.paletas[0].colores = pluginHost.BGR555(br.ReadBytes((int)file_size));
            nclr.pltt.nColores = file_size / 2;
			
			br.Close();
			pluginHost.Set_NCLR(nclr);
		}
	}  
}
