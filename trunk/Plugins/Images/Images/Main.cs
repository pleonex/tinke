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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace Images
{
	/// <summary>
	/// Archivo de entrada al módulo.
	/// </summary>
	public class Main : IPlugin
	{
		IPluginHost pluginHost;
		
		public void Initialize(IPluginHost pluginHost)
		{
			this.pluginHost = pluginHost;
		}
		
		public Format Get_Format(string nombre, byte[] magic)
		{
            nombre = nombre.ToUpper();
            string ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".NBFP"))
                return Format.Palette;
            else if (nombre.EndsWith(".NBFC"))
                return Format.Tile;
            else if (nombre.EndsWith(".NBFS") || nombre.EndsWith(".MAP"))
                return Format.Map;
            else if (nombre.EndsWith(".NTFT") && ext != "CMPR" && ext != "BLDT" || nombre.EndsWith(".RAW"))
                return Format.Tile;
            else if (nombre.EndsWith(".NTFP") && ext != "BLDT")
                return Format.Palette;
            else if (nombre.EndsWith(".PLT") || nombre.EndsWith(".PAL") || nombre.EndsWith(".PLTT"))
                return Format.Palette;
            else if (nombre.EndsWith(".CHAR") || nombre.EndsWith(".CHR") && ext != "NARC" && ext != "CRAN")
                return Format.Tile;
			
			return Format.Unknown;
		}
		
		public Control Show_Info(string archivo, int id)
		{
            if (archivo.ToUpper().EndsWith(".NBFP"))
            {
                new nbfp(pluginHost, archivo, id).Leer();
                
                PaletteControl control = new PaletteControl(pluginHost);
                return control;
            }
            if (archivo.ToUpper().EndsWith(".NBFC") || archivo.ToUpper().EndsWith(".RAW"))
			{
				new nbfc(pluginHost, archivo, id).Leer();

				if (pluginHost.Get_NCLR().header.file_size != 0x00)
				{
                    ImageControl control = new ImageControl(pluginHost, false);
                    return control;
				}
			}

			if (archivo.ToUpper().EndsWith(".NBFS") || archivo.ToUpper().EndsWith(".MAP"))
			{
				new nbfs(pluginHost, archivo, id).Leer();
				
				if (pluginHost.Get_NCLR().header.file_size != 0x00 && pluginHost.Get_NCGR().header.file_size != 0x00)
				{
					NCGR tile = pluginHost.Get_NCGR();
					tile.rahc.tileData = pluginHost.Transform_NSCR(pluginHost.Get_NSCR(), tile.rahc.tileData);
					
					ImageControl control = new ImageControl(pluginHost, true);
					return control;
				}
			}

            if (archivo.ToUpper().EndsWith(".NTFT"))
            {
                new ntft(pluginHost, archivo, id).Leer();

                if (pluginHost.Get_NCLR().header.file_size != 0x00)
                {
                    ImageControl control = new ImageControl(pluginHost, false);
                    return control;
                }
            }
            if (archivo.ToUpper().EndsWith(".NTFP"))
            {
                new ntfp(pluginHost, archivo, id).Leer();

                PaletteControl control = new PaletteControl(pluginHost);
                return control;
            }

            if (archivo.ToUpper().EndsWith(".CHAR") || archivo.ToUpper().EndsWith(".CHR"))
            {
                new CHAR(pluginHost, archivo, id).Leer();

                if (pluginHost.Get_NCLR().header.file_size != 0x00)
                {
                    ImageControl control = new ImageControl(pluginHost, false);
                    return control;
                }
            }
            if (archivo.ToUpper().EndsWith(".PLT") || archivo.ToUpper().EndsWith(".PAL") || archivo.ToUpper().EndsWith(".PLTT"))
            {
                new PLT(pluginHost, archivo, id).Leer();

                PaletteControl control = new PaletteControl(pluginHost);
                return control;
            }
			
			return new Control();
		}		
		public void Read(string archivo, int id)
		{
			if (archivo.ToUpper().EndsWith(".NBFP"))
				new nbfp(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".NBFC") || archivo.ToUpper().EndsWith(".RAW"))
				new nbfc(pluginHost, archivo, id).Leer();
			if (archivo.ToUpper().EndsWith(".NBFS") ||archivo.ToUpper().EndsWith(".MAP"))
				new nbfs(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".NTFP"))
                new ntfp(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".NTFT"))
                new ntft(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".PLT") || archivo.ToUpper().EndsWith(".PAL") || archivo.ToUpper().EndsWith(".PLTT"))
                new PLT(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".CHAR") || archivo.ToUpper().EndsWith(".CHR"))
                new CHAR(pluginHost, archivo, id).Leer();
		}

        public String Pack(sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
	}
}