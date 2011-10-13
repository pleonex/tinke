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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
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
            if (gameCode == "A5FP" || gameCode == "A5FE" || gameCode == "YLTS")
                return true;
            else
                return false;
        }
        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            switch (gameCode)
            {
                case "A5FE":
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x409 & id <= 0x808)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
            }

            if (nombre.ToUpper().EndsWith(".TXT"))
                return Format.Text;
            if (nombre.ToUpper().EndsWith(".PLZ"))
                return Format.Compressed;
            if (nombre.EndsWith(".PCM") && BitConverter.ToInt32(magic, 0) == 0x00000010)
                return Format.Compressed;

            
            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }
        public Control Show_Info(string archivo, int id)
        {
            switch (gameCode)
            {
                case "A5FE":
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x409 && id <= 0x808)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
            }

            if (archivo.ToUpper().EndsWith(".TXT"))
                return new Text(pluginHost, gameCode, archivo).Show_Info(id);
            else if (archivo.ToUpper().EndsWith(".PLZ"))
                return new Control();

            return new Control();
        }

        public sFolder Unpack(string file, int id)
        {
            if (file.ToUpper().EndsWith(".PLZ"))
                return PCK2.Read(file, id, pluginHost);
            else if (file.ToUpper().EndsWith(".PCM"))
                return PCM.Unpack(file, pluginHost);

            return new sFolder();
        }
        public String Pack(sFolder unpacked, string file, int id)
        {
            return null;
        }
    }
}
