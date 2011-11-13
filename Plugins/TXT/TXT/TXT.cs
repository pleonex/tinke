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
using System.IO;
using PluginInterface;

namespace TXT
{
    public class TXT : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if ((nombre.EndsWith("LZ.TXT") || nombre.EndsWith("LZ.XML")) && magic[0] == 0x10)
                return Format.Unknown;

            if (nombre.EndsWith(".TXT") || nombre.EndsWith(".SADL") || nombre.EndsWith(".XML")
                || nombre.EndsWith(".INI") || nombre.EndsWith(".H") || nombre.EndsWith(".XSADL")
                || nombre.EndsWith(".BAT") || nombre.EndsWith(".SARC") || nombre.EndsWith(".SBDL")
                || nombre.EndsWith(".C") || nombre.EndsWith("MAKEFILE") || nombre.EndsWith(".BSF")
                || nombre.EndsWith(".LUA") || nombre.EndsWith(".CSV") || nombre.EndsWith(".SMAP")
                || nombre.EndsWith("BUILDTIME") || nombre.EndsWith(".LUA~") || nombre.EndsWith(".INI.TEMPLATE")
                || nombre.EndsWith("LUA.BAK"))
                return Format.Text;
            else if (ext == "MESG")
                return Format.Text;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }
        public Control Show_Info(string archivo, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            if (new String(Encoding.ASCII.GetChars(br.ReadBytes(4))) == "MESG")
            {
                br.Close();
                return new bmg(pluginHost, archivo).ShowInfo();
            }
            else
                br.BaseStream.Position = 0x00;

            byte[] txt = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();

            return new iTXT(txt, pluginHost, id);
        }

        public String Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
