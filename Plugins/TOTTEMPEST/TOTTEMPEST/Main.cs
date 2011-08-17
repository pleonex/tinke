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
using System.Windows.Forms;
using PluginInterface;

namespace TOTTEMPEST
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool EsCompatible()
        {
            if (gameCode == "ALEJ")
                return true;

            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".ANA"))
                return Formato.Imagen;
            else if (nombre.EndsWith(".APA"))
                return Formato.Paleta;
            else if (nombre.EndsWith(".ASC"))
                return Formato.Map;
            else if (nombre.EndsWith(".NBM"))
                return Formato.ImagenCompleta;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".APA"))
                APA.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ANA"))
                ANA.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ASC"))
                ASC.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".NBM"))
                NBM.Read(archivo, id, pluginHost);
        }
        public Control Show_Info(string archivo, int id)
        {
            Leer(archivo, id);

            if (archivo.ToUpper().EndsWith(".ANA"))
                return new ImageControl(pluginHost, false);
            else if (archivo.ToUpper().EndsWith(".ASC"))
                return new ImageControl(pluginHost, true);
            else if (archivo.ToUpper().EndsWith(".APA"))
                return new PaletteControl(pluginHost);
            else if (archivo.ToUpper().EndsWith(".NBM"))
                return new ImageControl(pluginHost, false);

            return new System.Windows.Forms.Control();
        }
    }
}
