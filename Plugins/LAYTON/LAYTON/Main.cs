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
using Ekona;

namespace LAYTON
{
    using System.Globalization;
    using System.IO;

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
            var testedGames = new[] { "A5FP", "A5FE",
                "YLTS", "YLTE", "YLTP", "YLTH", "YLTJ",
                "BLFE", "C2AJ"};
            return testedGames.Contains(gameCode);
        }
        public Format Get_Format(sFile file, byte[] magic)
        {
            file.name = file.name.ToUpper();
            ushort nazeId = 0;
            switch (gameCode)
            {
                case "A5FE":    // Layton 1 - USA
                    if (file.id >= 0x0001 && file.id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if (file.id >= 0x02CD && file.id <= 0x0765)
                        return Format.FullImage;
                    else if (file.id == 0x0766)
                        return Format.System;   // The same as id 0xB73 in A5FP
                    break;
                
                case "A5FP":    // Layton 1 - EUR
                    if (file.id >= 0x0001 && file.id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if (file.id >= 0x04E8 && file.id <= 0x0B72)
                        return Format.FullImage;
                    else if (file.id == 0x0B73)
                        return Format.System;   // Dummy, it was text to test the puzzle system, nothing interesing and not used
                    else if (file.name.EndsWith(".TXT"))
                        return Format.Text;
                    break;

                // Professor Layton and the Diabolical Box
                case "YLTS":
                case "YLTP":
                case "YLTH":
                    if (file.id >= 0x37 && file.id <= 0x408)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if (file.id >= 0x409 & file.id <= 0x808)
                        return Format.FullImage;
                    else if (Path.GetExtension(file.name) == ".DAT" && file.name[0] == 'N'
                             && ushort.TryParse(
                                 Path.GetFileNameWithoutExtension(file.name).Remove(0, 1),
                                 NumberStyles.Integer,
                                 CultureInfo.CurrentUICulture,
                                 out nazeId)) return Format.Text;
                    break;
                case "YLTE":
                    if (file.id >= 0x37 && file.id <= 0x412)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if (file.id >= 0x413 && file.id <= 0x818)
                        return Format.FullImage;
                    else if (Path.GetExtension(file.name) == ".DAT" && file.name[0] == 'N'
                             && ushort.TryParse(
                                 Path.GetFileNameWithoutExtension(file.name).Remove(0, 1),
                                 NumberStyles.Integer,
                                 CultureInfo.CurrentUICulture,
                                 out nazeId)) return Format.Text;
                    break;
                
                // Layton 4 US (London life files)
                case "BLFE":
                    if (file.name.EndsWith(".DARC"))
                        return Format.Pack;
                    else if (file.name.EndsWith(".DENC"))
                        return Format.Compressed;
                    else if (file.name.EndsWith(".ARCHIVE"))
                        return Format.Pack;
                    break;

                case "C2AJ":
                    if (file.id >= 0x35 && file.id <= 0xEF)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if (file.id >= 0xF0 && file.id <= 0x193)
                        return Format.FullImage;
                    break;
                case "YLTJ":
                    if ((file.id >= 0x3CA && file.id <= 0x47F) || (file.id >= 0x740 && file.id <= 0xB14))
                        return new Ani(pluginHost, gameCode, "").Get_Formato(file.name);
                    else if ((file.id >= 0x480 && file.id <= 0x523) || (file.id >= 0xB15 && file.id <= 0xF3D))
                        return Format.FullImage;
                    break;
            }

            if (file.name.ToUpper().EndsWith(".PLZ"))
                return Format.Pack;
            if (file.name.EndsWith(".PCM") && BitConverter.ToInt32(magic, 0) == 0x00000010)
                return Format.Pack;
            if (file.name.EndsWith(".GDS"))
                return Format.Script;
            if (file.name.EndsWith("_DATABAS_LE.BIN"))
                return Format.System;
            
            return Format.Unknown;
        }

        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            switch (gameCode)
            {
                case "A5FE":
                    if (file.id >= 0x0001 && file.id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if (file.id >= 0x02CD && file.id <= 0x0765)
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;
                case "A5FP":
                    if (file.id >= 0x0001 && file.id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if (file.id >= 0x04E8 && file.id <= 0x0B72)
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;

                // Professor Layton and the Diabolical Box
                case "YLTS":
                case "YLTP":
                case "YLTH":
                    if (file.id >= 0x37 && file.id <= 0x408)
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if (file.id >= 0x409 && file.id <= 0x808)
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;
                case "YLTE":
                    if (file.id >= 0x37 && file.id <= 0x412)
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if (file.id >= 0x413 && file.id <= 0x818)
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;

                case "C2AJ":
                    if (file.id >= 0x35 && file.id <= 0xEF)
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if (file.id >= 0xF0 && file.id <= 0x193)
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;
                case "YLTJ":
                    if ((file.id >= 0x3CA && file.id <= 0x47F) || (file.id >= 0x740 && file.id <= 0xB14))
                        return new Ani(pluginHost, gameCode, file.path).Show_Info(file.id);
                    else if ((file.id >= 0x480 && file.id <= 0x523) || (file.id >= 0xB15 && file.id <= 0xF3D))
                    {
                        Bg bg = new Bg(pluginHost, file.path, file.id, file.name);
                        return bg.Get_Control();
                    }
                    break;
            }

            if (file.name.ToUpper().EndsWith(".TXT"))
                return new Text(pluginHost, gameCode, file.path).Show_Info(file.id);
            else if (file.name.ToUpper().EndsWith(".PLZ"))
                return new Control();
            else if (file.name.ToUpper().EndsWith(".GDS"))
                return new ScriptControl(GDS.Read(file.path));
            else if (file.format == Format.Text && file.name.ToUpper().EndsWith(".DAT"))
                return new NazoTextControl(this.pluginHost, file.path, file.id);

            return new Control();
        }

        public sFolder Unpack(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".PLZ"))
                return PCK2.Read(file.path, file.id, pluginHost);
            else if (file.name.ToUpper().EndsWith(".PCM"))
                return PCM.Unpack(file.path);
            else if (file.name.ToUpper().EndsWith(".DARC"))
                return DARC.Unpack(file.path, file.name);
            else if (file.name.ToUpper().EndsWith(".DENC"))
                return DENC.Unpack(file.path, file.name, pluginHost);
            else if (file.name.ToUpper().EndsWith(".ARCHIVE"))
                return LAYTON4.Unpack_ARCHIVE(file.path, file.name);

            return new sFolder();
        }
        public String Pack(ref sFolder unpacked, sFile file)
        {
            if (file.name.EndsWith(".pcm"))
            {
                string fileOut = pluginHost.Get_TempFile();
                PCM.Pack(fileOut, unpacked);
                return fileOut;
            }

            if (file.name.EndsWith(".plz"))
            {
                string fileOut = this.pluginHost.Get_TempFile();
                PCK2.Pack(fileOut, unpacked.files);
                string compressed = this.pluginHost.Get_TempFile();
                this.pluginHost.Compress(fileOut, compressed, FormatCompress.LZ10);
                File.Delete(fileOut);
                return compressed;
            }

            if (gameCode == "BLFE")
            {
                if (file.name.ToUpper().EndsWith(".DENC"))
                {
                    string fileOut = pluginHost.Get_TempFile() + ".denc";
                    DENC.Pack(fileOut, unpacked);
                    return fileOut;
                }
                else if (file.name.ToUpper().EndsWith(".DARC"))
                {
                    string fileOut = pluginHost.Get_TempFile() + ".darc";
                    DARC.Pack(fileOut, ref unpacked);
                    return fileOut;
                }
            }
            return null;
        }
    }
}
