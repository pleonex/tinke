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
            if (gameCode == "A5FP" || gameCode == "A5FE" || gameCode == "YLTS" || gameCode == "BLFE" ||
                gameCode == "YLTE" || gameCode == "YLTP" || gameCode == "C2AJ")
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
                        return Format.FullImage;
                    else if (id == 0x0766)
                        return Format.System;   // The same as id 0xB73 in A5FP
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return Format.FullImage;
                    else if (id == 0x0B73)
                        return Format.System;   // Dummy, it was text to test the puzzle system, nothing interesing and not used
                    break;

                // Professor Layton and the Diabolical Box
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x409 & id <= 0x808)
                        return Format.FullImage;
                    break;
                case "YLTE":
                    if (id >= 0x37 && id <= 0x412)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x413 && id <= 0x818)
                        return Format.FullImage;
                    break;
                case "YLTP":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x409 && id <= 0x808)
                        return Format.FullImage;
                    break;
                
                case "BLFE":
                    if (nombre.EndsWith(".DARC"))
                        return Format.Pack;
                    else if (nombre.EndsWith(".DENC"))
                        return Format.Compressed;
                    else if (nombre.EndsWith(".ARCHIVE"))
                        return Format.Pack;
                    break;

                case "C2AJ":
                    if (id >= 0x35 && id <= 0xEF)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0xF0 && id <= 0x193)
                        return Format.FullImage;
                    break;
            }

            if (gameCode != "C2AJ" && nombre.ToUpper().EndsWith(".TXT"))
                return Format.Text;
            if (nombre.ToUpper().EndsWith(".PLZ"))
                return Format.Pack;
            if (nombre.EndsWith(".PCM") && BitConverter.ToInt32(magic, 0) == 0x00000010)
                return Format.Pack;
            if (nombre.EndsWith(".GDS"))
                return Format.Script;
            if (nombre.EndsWith("_DATABAS_LE.BIN"))
                return Format.System;
            
            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }
        public Control Show_Info(string file, int id)
        {
            switch (gameCode)
            {
                case "A5FE":
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0x02CD && id <= 0x0765)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0x04E8 && id <= 0x0B72)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;

                // Professor Layton and the Diabolical Box
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0x409 && id <= 0x808)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;
                case "YLTE":
                    if (id >= 0x37 && id <= 0x412)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0x413 && id <= 0x818)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;
                case "YLTP":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0x409 && id <= 0x808)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;

                case "C2AJ":
                    if (id >= 0x35 && id <= 0xEF)
                        return new Ani(pluginHost, gameCode, file).Show_Info();
                    else if (id >= 0xF0 && id <= 0x193)
                    {
                        Bg bg = new Bg(pluginHost, file, id);
                        return bg.Get_Control();
                    }
                    break;
            }

            if (file.ToUpper().EndsWith(".TXT"))
                return new Text(pluginHost, gameCode, file).Show_Info(id);
            else if (file.ToUpper().EndsWith(".PLZ"))
                return new Control();
            else if (file.ToUpper().EndsWith(".GDS"))
                return new ScriptControl(GDS.Read(file));         

            return new Control();
        }

        public sFolder Unpack(string file, int id)
        {
            if (file.ToUpper().EndsWith(".PLZ"))
                return PCK2.Read(file, id, pluginHost);
            else if (file.ToUpper().EndsWith(".PCM"))
                return PCM.Unpack(file, pluginHost);
            else if (file.ToUpper().EndsWith(".DARC"))
                return DARC.Unpack(file);
            else if (file.ToUpper().EndsWith(".DENC"))
                return DENC.Unpack(file, pluginHost);
            else if (file.ToUpper().EndsWith(".ARCHIVE"))
                return LAYTON4.Unpack_ARCHIVE(file);

            return new sFolder();
        }
        public String Pack(ref sFolder unpacked, string file, int id)
        {
            if (gameCode == "BLFE")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                    System.IO.Path.GetRandomFileName();

                if (file.ToUpper().EndsWith(".DENC"))
                {
                    fileOut += ".denc";
                    DENC.Pack(fileOut, unpacked);
                    return fileOut;
                }
                else if (file.ToUpper().EndsWith(".DARC"))
                {
                    fileOut += ".darc";
                    DARC.Pack(fileOut, ref unpacked);
                    return fileOut;
                }
            }
            return null;
        }
    }
}
