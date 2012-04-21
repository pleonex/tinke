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

namespace _999HRPERDOOR
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "BSKE")
                return true;

            return false;
        }

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (id >= 0x13EF && id <= 0x1500)
                return Format.FullImage;
            if (ext == "AT6P")
                return Format.Compressed;
            if (nombre.EndsWith(".at6p"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
            if (id >= 0x13EF && id <= 0x1500)
                new SIR0_Sprite(pluginHost, archivo, id);
            if (archivo.EndsWith(".at6p"))
                new SIR0_Image(pluginHost, archivo, id);
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            Read(archivo, id);

            if (id >= 0x13EF && id <= 0x1500)
                return new PluginInterface.Images.SpriteControl(pluginHost);
            if (archivo.EndsWith(".at6p"))
                return new PluginInterface.Images.ImageControl(pluginHost, false);

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id)
        {
            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(file).Substring(12) + ".at6p";

            byte[] data = File.ReadAllBytes(file);
            byte[] decrypted = AT6P.Decrypt(data);
            File.WriteAllBytes(tempFile, decrypted);

            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            sFile newFile = new sFile();
            newFile.name = Path.GetFileName(tempFile);
            newFile.offset = 0;
            newFile.path = tempFile;
            newFile.size = (uint)decrypted.Length;
            unpack.files.Add(newFile);

            return unpack;
        }
    }
}
