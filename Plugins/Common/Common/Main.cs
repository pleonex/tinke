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
using Ekona;
using System.Windows.Forms;

namespace Common
{
    public class Main : IPlugin 
    {
        IPluginHost pluginHost;

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (file.name.ToUpper().EndsWith(".TGA") || file.name.ToUpper().EndsWith(".GIF") || file.name.ToUpper().EndsWith(".JPG") || file.name.ToUpper().EndsWith(".PNG"))
                return Format.FullImage;
            else if (file.name.ToUpper().EndsWith(".BMP") && magic[0] == 'B' && magic[1] == 'M')
                return Format.FullImage;
            else if (file.name.ToUpper().EndsWith(".WAV") || ext == "RIFF")
                return Format.Sound;
            
            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public void Read(sFile file) { }
        public Control Show_Info(sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string ext = "";
            try { ext = new String(br.ReadChars(4)); }
            catch { }
            br.Close();

            if (file.name.ToUpper().EndsWith(".TGA"))
                return new TGA(pluginHost, file.path).Show_Info();
            else if (file.name.ToUpper().EndsWith(".JPG"))
                return new JPG(pluginHost, file.path).Show_Info();
            else if (file.name.ToUpper().EndsWith(".GIF"))
                return new GIF(pluginHost, file.path).Show_Info();
            else if (file.name.ToUpper().EndsWith(".PNG"))
                return new PNG(pluginHost, file.path).Show_Info();
            else if (file.name.ToUpper().EndsWith(".WAV") || ext == "RIFF")
                return new WAV(pluginHost, file.path).Show_Info();
            else if (file.name.ToUpper().EndsWith(".BMP"))
                return new BMP(pluginHost, file.path).Show_Info();

            return new Control();
        }

        public string Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
