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

namespace Nintendo
{
	/// <summary>
	/// Archivo de entrada al módulo.
	/// </summary>
	public class Main : IPlugin
	{
		IPluginHost pluginHost;
		
		public void Inicializar(IPluginHost pluginHost)
		{
			this.pluginHost = pluginHost;
		}
		
		public Formato Get_Formato(string nombre, byte[] magic)
		{
            nombre = nombre.ToUpper();
            string ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".NBFP"))
                return Formato.Paleta;
            else if (nombre.EndsWith(".NBFC"))
                return Formato.Imagen;
            else if (nombre.EndsWith(".NBFS"))
                return Formato.Screen;
            else if (ext == "MESG" && nombre.EndsWith(".BMG"))
                return Formato.Texto;
            else if (nombre.EndsWith(".NTFT") && ext != "CMPR" && ext != "BLDT")
                return Formato.Imagen;
            else if (nombre.EndsWith(".NTFP") && ext != "BLDT")
                return Formato.Paleta;
            else if (nombre.EndsWith(".PLT"))
                return Formato.Paleta;
            else if (nombre.EndsWith(".CHAR") || nombre.EndsWith(".CHR"))
                return Formato.Imagen;
			
			return Formato.Desconocido;
		}
		
		public Control Show_Info(string archivo, int id)
		{
            if (archivo.ToUpper().EndsWith(".NBFP"))
            {
                new nbfp(pluginHost, archivo, id).Leer();
                
                iNCLR control = new iNCLR(pluginHost, pluginHost.Get_NCLR());
                return control;
            }
			if (archivo.ToUpper().EndsWith(".NBFC"))
			{
				new nbfc(pluginHost, archivo, id).Leer();

				if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
				{
                    iNCGR control = new iNCGR(pluginHost, pluginHost.Get_NCGR(), pluginHost.Get_NCLR());
                    return control;
				}
			}

			if (archivo.ToUpper().EndsWith(".NBFS"))
			{
				new nbfs(pluginHost, archivo, id).Leer();
				
				if (pluginHost.Get_NCLR().cabecera.file_size != 0x00 && pluginHost.Get_NCGR().cabecera.file_size != 0x00)
				{
					NCGR tile = pluginHost.Get_NCGR();
					tile.rahc.tileData = pluginHost.Transformar_NSCR(pluginHost.Get_NSCR(), tile.rahc.tileData);
					
					iNCGR control = new iNCGR(pluginHost, tile, pluginHost.Get_NCLR());
					return control;
				}
			}
            if (archivo.ToUpper().EndsWith(".BMG"))
                return new bmg(pluginHost, archivo).ShowInfo();

            if (archivo.ToUpper().EndsWith(".NTFT"))
            {
                new ntft(pluginHost, archivo, id).Leer();

                if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                {
                    iNCGR control = new iNCGR(pluginHost, pluginHost.Get_NCGR(), pluginHost.Get_NCLR());
                    return control;
                }
            }
            if (archivo.ToUpper().EndsWith(".NTFP"))
            {
                new ntfp(pluginHost, archivo, id).Leer();

                iNCLR control = new iNCLR(pluginHost, pluginHost.Get_NCLR());
                return control;
            }

            if (archivo.ToUpper().EndsWith(".CHAR") || archivo.ToUpper().EndsWith(".CHR"))
            {
                new CHAR(pluginHost, archivo, id).Leer();

                if (pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                {
                    iNCGR control = new iNCGR(pluginHost, pluginHost.Get_NCGR(), pluginHost.Get_NCLR());
                    return control;
                }
            }
            if (archivo.ToUpper().EndsWith(".PLT"))
            {
                new PLT(pluginHost, archivo, id).Leer();

                iNCLR control = new iNCLR(pluginHost, pluginHost.Get_NCLR());
                return control;
            }
			
			return new Control();
		}
		
		public void Leer(string archivo, int id)
		{
			if (archivo.ToUpper().EndsWith(".NBFP"))
				new nbfp(pluginHost, archivo, id).Leer();
			if (archivo.ToUpper().EndsWith(".NBFC"))
				new nbfc(pluginHost, archivo, id).Leer();
			if (archivo.ToUpper().EndsWith(".NBFS"))
				new nbfs(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".BMG"))
                Console.WriteLine("Este archivo no contiene información para guardar.");
            if (archivo.ToUpper().EndsWith(".NTFP"))
                new ntfp(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".NTFT"))
                new ntft(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".PLT"))
                new PLT(pluginHost, archivo, id).Leer();
            if (archivo.ToUpper().EndsWith(".CHAR") || archivo.ToUpper().EndsWith(".CHR"))
                new CHAR(pluginHost, archivo, id).Leer();
		}
	}
}