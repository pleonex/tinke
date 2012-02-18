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

namespace RUNEFACTORY3
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
            if (gameCode == "BRFE" || gameCode == "BRFJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (id == 0x14)
                return Format.Pack;
            else if (ext == "TEXT")
                return Format.Text;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, string file, int id)
        {
            if (id == 0x14)
            {
                String fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar + "newPack_" + System.IO.Path.GetFileName(file);
                if (System.IO.File.Exists(fileOut))
                    System.IO.File.Delete(fileOut);

                Archive.Pack(fileOut, ref unpacked);
                return fileOut;
            }

            return null;
        }
        public sFolder Unpack(string file, int id)
        {
            if (id == 0x14)
                return Archive.Unpack(file);

            return new sFolder();
        }

        public void Read(string file, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }

    }
}
