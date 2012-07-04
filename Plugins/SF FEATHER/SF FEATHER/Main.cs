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
using Ekona;
using Ekona.Images;

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

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (file.name.EndsWith(".pac") && (magic[2] == 0x01 && magic[3] == 0x00))
                return Format.Pack;
            if (ext == "CG4 " || ext == "CG8 ")
                return Format.FullImage;
            else if (ext == "SC4 " || ext == "SC8 ")
                return Format.Map;
            else if (ext == "CGT ")
                return Format.FullImage;
            else if (ext == "PSI3")
                return Format.Script;

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, sFile file)
        {
            string fileOut = pluginHost.Get_TempFile();
            PAC.Pack(file.path, fileOut, ref unpacked);

            return fileOut;
        }
        public sFolder Unpack(sFile file)
        {
            return PAC.Unpack(file.path, file.name);
        }

        public void Read(sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "CG4 ")
            {
                CGx cgx = new CGx(file.path, file.id, false, file.name);
                pluginHost.Set_Palette(cgx.Palette);
                pluginHost.Set_Image(cgx);
            }
            else if (ext == "CG8 ")
            {
                CGx cgx = new CGx(file.path, file.id, true, file.name);
                pluginHost.Set_Palette(cgx.Palette);
                pluginHost.Set_Image(cgx);
            }
            else if (ext == "SC4 " || ext == "SC8 ")
            {
                SCx scx = new SCx(file.path, file.id, file.name);
                pluginHost.Set_Map(scx);
            }
            else if (ext == "CGT ")
            {
                CGT cgt = new CGT(file.path, file.id, file.name);
                pluginHost.Set_Palette(cgt.Palette);
                pluginHost.Set_Image(cgt);
            }
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            Read(file);

            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();

            if (ext == "CG4 " || ext == "CG8 ")
                return new ImageControl(pluginHost, false);
            else if (ext == "SC4 " || ext == "SC8 ")
                return new ImageControl(pluginHost, true);
            else if (ext == "CGT ")
                return new ImageControl(pluginHost, false);

            return new System.Windows.Forms.Control();
        }

    }
}
