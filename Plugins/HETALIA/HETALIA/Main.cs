// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>21/04/2012 1:55:14</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using PluginInterface;

namespace HETALIA
{
    public class Main : IGamePlugin 
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost; ;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "THTJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (id == 0x01)
                return Format.Pack;
            if (ext == "MAP\0" || ext == "IMY\0")
                return Format.FullImage;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, string file, int id)
        {
            return null;
        }
        public sFolder Unpack(string file, int id)
        {
            if (id == 0x01)
                return HETALIA.Pack.DATA.Unpack(file);

            return new sFolder();
        }

        public void Read(string file, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            if (file.EndsWith(".IMY"))
            {
                new IMY(pluginHost, file, id);
                return new PluginInterface.Images.ImageControl(pluginHost, false);
            }
            else if (file.EndsWith(".MAP"))
            {
                new MAP(pluginHost, file, id);
                return new PluginInterface.Images.ImageControl(pluginHost, true);
            }

            return new System.Windows.Forms.Control();
        }

    }
}
