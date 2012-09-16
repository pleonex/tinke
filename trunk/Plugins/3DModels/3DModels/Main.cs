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
using System.Text;
using System.Drawing;
using System.IO;
using Ekona;

namespace _3DModels
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;
        sBTX0 btx;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "BTX0")
                return Format.Texture;
            else if (ext == "BMD0")
                return Format.Model3D;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "BTX0")
            {
                sBTX0 btx = BTX0.Read(file.path, file.id, pluginHost);

                Bitmap[] tex = new Bitmap[btx.texture.texInfo.num_objs];
                for (int i = 0; i < btx.texture.texInfo.num_objs; i++)
                {
                    string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar +
                        file.name + '_' + btx.texture.texInfo.names[i] + ".png";
                    if (File.Exists(fileOut))
                        fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() +
                            '_' + btx.texture.texInfo.names[i] + ".png";

                    tex[i] = BTX0.GetTexture(pluginHost, btx, i);
                    tex[i].Save(fileOut);
                }
                pluginHost.Set_Object(tex);
            }
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "BTX0")
            {
                btx = BTX0.Read(file.path, file.id, pluginHost);
                return new TextureControl(pluginHost, btx);
            }
            else if (ext == "BMD0")
            {
                sBMD0 bmd = BMD0.Read(file.path, file.id, pluginHost);

                if (bmd.header.numSect == 2)
                    return new ModelControl(pluginHost, bmd);
                else if (btx.texture.texInfo.num_objs != 0)
                    return new ModelControl(pluginHost, bmd, btx);
                else
                    System.Windows.Forms.MessageBox.Show("There aren't textures.");
            }
            
            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
