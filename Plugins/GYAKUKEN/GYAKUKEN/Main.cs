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
using PluginInterface;

namespace GYAKUKEN
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
            if (gameCode == "BXOJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            uint offset = BitConverter.ToUInt32(magic, 0);
 
            if (fileName.ToUpper().EndsWith(".BIN"))
                return Format.Pack;
            else if (fileName.ToUpper().EndsWith(".DBIN") && IsPack2(fileName) && offset == 0x0C)
                return Format.Pack;

            return Format.Unknown;
        }
        private bool IsPack2(string name)
        {
            List<string> pack2 = new List<string>();
            pack2.Add("48bustup.bin");
            pack2.Add("50cutobj.bin");
            pack2.Add("52idcom.bin");
            pack2.Add("54logic_keyword.bin");
            pack2.Add("60mapchar.bin");
            pack2.Add("87cutobj_local.bin");
            pack2.Add("88idlocal.bin");
            pack2.Add("89logic_keyword_local.bin");

            foreach (string folder in pack2)
                if (name.StartsWith(folder))
                    return true;

            return false;
        }

        public void Read(string file, int id)
        {
            if (gameCode == "BXOJ")
            {
                if (file.ToUpper().EndsWith(".BIN"))
                    Pack.Unpack(pluginHost, file);
                else if (file.ToUpper().EndsWith(".DBIN"))
                    Pack.Unpack2(pluginHost, file);
            }
        }

        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }
    }
}
