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
using Ekona;

namespace TXT
{
    public class TXT : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(sFile file, byte[] magic)
        {
            file.name = file.name.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if ((file.name.EndsWith("LZ.TXT") || file.name.EndsWith("LZ.XML")) && magic[0] == 0x10)
                return Format.Unknown;

            if (file.name.EndsWith(".TXT") || file.name.EndsWith(".XML") || file.name.EndsWith(".DTD")
                || file.name.EndsWith(".INI") || file.name.EndsWith(".H") || file.name.EndsWith(".XSADL")
                || file.name.EndsWith(".BAT") || file.name.EndsWith(".SARC") || file.name.EndsWith(".SBDL")
                || file.name.EndsWith(".C") || file.name.EndsWith("MAKEFILE") || file.name.EndsWith(".BSF")
                || file.name.EndsWith(".LUA") || file.name.EndsWith(".CSV") || file.name.EndsWith(".SMAP")
                || file.name.EndsWith("BUILDTIME") || file.name.EndsWith(".LUA~") || file.name.EndsWith(".INI.TEMPLATE")
                || file.name.EndsWith("LUA.BAK") || file.name.EndsWith(".NAIX") || file.name.EndsWith(".NBSD")
                || file.name.EndsWith(".HTML") || file.name.EndsWith(".CSS") || file.name.EndsWith(".JS"))
                return Format.Text;

            if (file.name.EndsWith(".SADL") && ext.ToUpper() != "SADL")
                return Format.Text;

            if (ext == "MESG")
                return Format.Text;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            if (new String(Encoding.ASCII.GetChars(br.ReadBytes(4))) == "MESG")
            {
                br.Close();
                return new bmg(pluginHost, file.path).ShowInfo();
            }
            else
                br.BaseStream.Position = 0x00;

            byte[] txt = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();

            return new iTXT(txt, pluginHost, file.id);
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
