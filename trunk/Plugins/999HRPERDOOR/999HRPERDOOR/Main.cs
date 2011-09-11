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
using System.Text;
using PluginInterface;

namespace _999HRPERDOOR
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
            if (gameCode == "BSKE")
                return true;

            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            if (id >= 0x13EF && id <= 0x1500)
                return Formato.ImagenCompleta;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
            if (id >= 0x13EF && id <= 0x1500)
                SIR0.Read(archivo, id, pluginHost);

        }

        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            Leer(archivo, id);

            if (id >= 0x13EF && id <= 0x1500)
                return new CellControl(pluginHost);

            return new System.Windows.Forms.Control();
        }
    }
}
