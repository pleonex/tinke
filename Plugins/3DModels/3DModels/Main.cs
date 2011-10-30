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
using System.IO;
using PluginInterface;

namespace _3DModels
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

            if (ext == "BTX0")
                return Format.Texture;
            else if (ext == "BMD0")
                return Format.Texture;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "BTX0")
                return new TextureControl(pluginHost, BTX0.Read(archivo, id, pluginHost));
            else if (ext == "BMD0")
            {
                sBMD0 bmd = BMD0.Read(archivo, id, pluginHost);

                if (bmd.header.numSect == 2)
                    return new TextureControl(pluginHost, bmd.texture, bmd.header.offset[1], bmd.filePath);
                else
                    System.Windows.Forms.MessageBox.Show("There is not texture section.");
            }
            
            return new System.Windows.Forms.Control();
        }

        public String Pack(sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
