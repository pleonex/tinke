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

namespace Fonts
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(string nombre, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "NFTR" || ext == "RTFN")
                return Format.Font;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(archivo));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "NFTR" || ext == "RTFN")
            {
                return new FontControl(pluginHost, NFTR.Read(archivo, id, pluginHost.Get_Language()));
            }

            return new System.Windows.Forms.Control();
        }

        public String Pack(sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
