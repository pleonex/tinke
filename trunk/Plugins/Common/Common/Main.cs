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
using PluginInterface;
using System.Windows.Forms;

namespace Common
{
    public class Main : IPlugin 
    {
        IPluginHost pluginHost;

        public Format Get_Format(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".TGA") || nombre.EndsWith(".JPG") || nombre.EndsWith(".PNG") || nombre.EndsWith(".BMP"))
                return Format.FullImage;
            else if (nombre.EndsWith(".WAV") || ext == "RIFF")
                return Format.Sound;
            
            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public void Read(string archivo, int id)
        {
        }
        public Control Show_Info(string archivo, int id)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(archivo));
            string ext = "";
            try { ext = new String(br.ReadChars(4)); }
            catch { }
            br.Close();

            if (archivo.ToUpper().EndsWith(".TGA"))
                return new TGA(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".JPG"))
                return new JPG(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".PNG"))
                return new PNG(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".WAV") || ext == "RIFF")
                return new WAV(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".BMP"))
                return new BMP(pluginHost, archivo).Show_Info();

            return new Control();
        }

        public string Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
