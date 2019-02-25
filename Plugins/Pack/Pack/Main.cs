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
 * By: pleoNeX
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona;

namespace Pack
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(sFile file, byte[] magic)
        {
            string type = new String(Encoding.ASCII.GetChars(magic));

            if (type == "NARC" || type == "CRAN")
                return Format.Pack;
            else if (file.name.ToUpper().EndsWith("UTILITY.BIN") && magic[0] == 0x10)
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            return new Control();
        }

        public String Pack(ref sFolder unpacked, sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (type == "NARC" || type == "CRAN")
                return new NARC(pluginHost).Pack(file, ref unpacked);
            else if (file.name.ToUpper().EndsWith("UTILITY.BIN"))
                return new Utility(pluginHost).Pack(file.path, ref unpacked);

            return null;
        }
        public sFolder Unpack(sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (file.name.ToUpper().EndsWith("UTILITY.BIN"))
                return new Utility(pluginHost).Unpack(file.path);
            else if (type == "NARC" || type == "CRAN")
                return new NARC(pluginHost).Unpack(file);

            return new sFolder();
        }

    }
}
