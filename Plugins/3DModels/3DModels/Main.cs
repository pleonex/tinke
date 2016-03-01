//
//  Main.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ekona;
using Models3D.Textures;
using Models3D.Models;

namespace Models3D
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;
        sBTX0 preloadedTexture;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = Encoding.ASCII.GetString(magic);

            if (ext == "BTX0")
                return Format.Texture;
            else if (ext == "BMD0")
                return Format.Model3D;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            // If it's a texture file, preload for a 3D model without textures inside.
            if (GetExtension(file) == "BTX0")
                preloadedTexture = Btx0.Read(file.path, file.id, pluginHost);
        }

        public Control Show_Info(sFile file)
        {
            string extension = GetExtension(file);

            if (extension == "BTX0") {
                // Texture format
                preloadedTexture = Btx0.Read(file.path, file.id, pluginHost);
                return new TextureControl(pluginHost, preloadedTexture);
            } else if (extension == "BMD0") {
                // 3D model format
                sBMD0 bmd = BMD0.Read(file.path, file.id, pluginHost);

                // Check if the model has textures and otherwise use the preloaded one.
                if (bmd.header.numSect == 2)
                    return new ModelControl(pluginHost, bmd);
                else if (preloadedTexture.texture.texInfo.num_objs != 0)
                    return new ModelControl(pluginHost, bmd, preloadedTexture);
                else
                    MessageBox.Show("There are not textures.");
            }
            
            return new Control();
        }

        // No pack formats in this plugin.
        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }

        private static string GetExtension(sFile file)
        {
            string extension;
            using (var reader = new BinaryReader(File.OpenRead(file.path)))
                extension = new String(reader.ReadChars(4));
            return extension;
        }
    }
}
