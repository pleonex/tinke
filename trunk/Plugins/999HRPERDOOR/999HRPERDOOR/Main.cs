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
using Ekona;

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

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (file.id >= 0x13EF && file.id <= 0x1500)
                return Format.FullImage;
            if (ext == "AT6P")
                return Format.Compressed;
            //if (file.name.EndsWith(".at6p"))
            //    return Format.FullImage;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            if (file.id >= 0x13EF && file.id <= 0x1500)
                new SIR0_Sprite(pluginHost, file.path, file.id, file.name);
            //if (file.name.EndsWith(".at6p"))
            //    new SIR0_Image(pluginHost, file.path, file.id, file.name);
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            Read(file);

            if (file.id >= 0x13EF && file.id <= 0x1500)
                return new Ekona.Images.SpriteControl(pluginHost);
            if (file.name.EndsWith(".at6p"))
                return new Ekona.Images.ImageControl(pluginHost, false);

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file)
        {
            string tempFile = pluginHost.Get_TempFile();

			Stream strin = File.OpenRead(file.path);
            MemoryStream strout = AT6P.Decode(strin);

            FileStream strfile = File.OpenWrite(tempFile);
			strout.WriteTo(strfile);
			strfile.Flush();

            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            sFile newFile = new sFile();
            newFile.name = file.name;
            newFile.offset = 0;
            newFile.path = tempFile;
            newFile.size = (uint)strfile.Length;
            unpack.files.Add(newFile);

			strin.Dispose();
			strout.Dispose();
			strfile.Dispose();

            return unpack;
        }
    }
}
