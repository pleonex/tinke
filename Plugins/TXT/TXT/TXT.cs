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
        public Format Get_Format(string name, byte[] magic, int id)
        {
            name = name.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if ((name.EndsWith("LZ.TXT") || name.EndsWith("LZ.XML")) && magic[0] == 0x10)
                return Format.Unknown;

            if (name.EndsWith(".TXT") || name.EndsWith(".SADL") || name.EndsWith(".XML")
                || name.EndsWith(".INI") || name.EndsWith(".H") || name.EndsWith(".XSADL")
                || name.EndsWith(".BAT") || name.EndsWith(".SARC") || name.EndsWith(".SBDL")
                || name.EndsWith(".C") || name.EndsWith("MAKEFILE") || name.EndsWith(".BSF")
                || name.EndsWith(".LUA") || name.EndsWith(".CSV") || name.EndsWith(".SMAP")
                || name.EndsWith("BUILDTIME") || name.EndsWith(".LUA~") || name.EndsWith(".INI.TEMPLATE")
                || name.EndsWith("LUA.BAK") || name.EndsWith(".NAIX") || name.EndsWith(".NBSD"))
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
