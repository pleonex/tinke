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
using PluginInterface;

namespace SF_FEATHER
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "CS4J")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (fileName.EndsWith(".pac") && (magic[2] == 0x01 && magic[3] == 0x00))
                return Format.Pack;
            if (ext == "CG4 " || ext == "CG8 ")
                return Format.FullImage;
            else if (ext == "SC4 " || ext == "SC8 ")
                return Format.Map;
            else if (ext == "PSI3")
                return Format.Script;

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, string file, int id)
        {
            string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                System.IO.Path.GetRandomFileName();
            PAC.Pack(file, fileOut, ref unpacked);

            return fileOut;
        }
        public sFolder Unpack(string file, int id)
        {
            return PAC.Unpack(file);
        }

        public void Read(string file, int id)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
            string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "CG4 ")
                CGx.CG4_Read(file, pluginHost);
            else if (ext == "CG8 ")
                CGx.CG8_Read(file, pluginHost);
            else if (ext == "SC4 ")
                SCx.SC4_Read(file, pluginHost);
            else if (ext == "SC8 ")
                SCx.SC8_Read(file, pluginHost);
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            Read(file, id);

            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
            string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "CG4 " || ext == "CG8 ")
                return new ImageControl(pluginHost, false);
            else if (ext == "SC4 " || ext == "SC8 ")
                return new ImageControl(pluginHost, true);

        return new System.Windows.Forms.Control();
        }

    }
}
