// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

// Copyright (C) 2019
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

// <author>Priverop</author>
// <contact>https://github.com/priverop/</contact>
// <date>25/02/2019</date>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Ekona;

namespace JUS
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "DSIG")
              return Format.FullImage;
            else if (ext == "ALAR")
                return Format.Pack;
            else if (ext == "DSCP")
                return Format.Compressed;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }

        public sFolder Unpack(sFile file)
        {
          System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
          string type = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
          br.Close();

          if (type == "ALAR")
              return new ALAR(pluginHost).Unpack(file);
          else if (type == "DSCP")
              return new ALAR(pluginHost).Unpack(file);

          return new sFolder();
        }

        public void Read(sFile file)
        {
        }

        public System.Windows.Forms.Control Show_Info(sFile file)
        {

            if (file.name.EndsWith(".dig", StringComparison.CurrentCulture))
            {
              new DIG(pluginHost, file.path, file. id);
              return new Ekona.Images.ImageControl(pluginHost, false);
            }

            return new System.Windows.Forms.Control();
        }

    }
}
