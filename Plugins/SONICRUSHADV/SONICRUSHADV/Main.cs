/*
 * Copyright (C) 2016
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
 * Plugin by: ccawley2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace SONICRUSHADV
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool IsCompatible()
        {
            if (gameCode == "ASCP" || gameCode == "ASCJ" || gameCode == "ASCE" ||
                gameCode == "A3YP" || gameCode == "A3YJ" || gameCode == "A3YE" ||
                gameCode == "BXSP" || gameCode == "BXSJ" || gameCode == "BXSE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string type = new String(Encoding.ASCII.GetChars(magic));
            if (type == "BB" + Convert.ToChar(0) + Convert.ToChar(0))
                return Format.Pack;
            if (type == "BAC" + Convert.ToChar(0) || type == "BAC" + Convert.ToChar(0xA))
                return Format.FullImage;

            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();
            if (type == "BAC" + Convert.ToChar(0) || type == "BAC" + Convert.ToChar(0xA))
                return new BAC(file.path, file.id, pluginHost, file.name).Get_Control();

            return new Control();
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();
            if (type == "BB" + Convert.ToChar(0) + Convert.ToChar(0))
            {
                String packFile = pluginHost.Get_TempFile();
                if (File.Exists(packFile))
                    File.Delete(packFile);

                BB.Pack(packFile, ref unpacked);
                return packFile;
            }

            return null;
        }
        public sFolder Unpack(sFile file) 
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
            br.Close();
            if (type == "BB" + Convert.ToChar(0) + Convert.ToChar(0))
                return BB.Unpack(file.path, pluginHost);

            return new sFolder();
        }
    }
}
