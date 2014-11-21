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
// <date>29/04/2012 13:41:18</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
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
            if (gameCode == "B2KJ")
            {
                Helper.pluginHost = pluginHost;
                Helper.Initialize();
                return true;
            }

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            String ext = new String(Encoding.ASCII.GetChars(magic));
            uint exti = 0;
            if (magic.Length == 4)
                exti = BitConverter.ToUInt32(magic, 0);

            if (ext == "NPCK" || ext == "KPCN")
                return Format.Pack;
            else if (exti == 0x001C080A)
                return Format.Text;
            else if (magic[0] == 0x42 && magic[1] == 0x4D && file.name.ToUpper().StartsWith("EDDN")) // BMP image
                return Format.FullImage;
            else if (exti == 0x001F080A || exti == 0x0019090A)
                return Format.Script;
            else if (file.id == 0xFF2)
                return Format.Text;
            else if (file.id == 0xFF4)
                return Format.Text;
            else if (file.id == 0xF76)
                return Format.Text;
            else if (file.name.EndsWith(".txt"))
                return Format.Text;     // Subtitles
            else if (ext == "TMAP")
                return Format.FullImage;
            else if (ext == "spdl")
                return Format.Pack;
            else if (file.name == "arm9.bin")
                return Format.System;
            else if (Text.TextControl.IsSupported(file.id))
                return Format.Text;
            else if (ext == "WMAP")
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            //// BMP images
            //string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + file.name + ".png";
            //System.Drawing.Image img = System.Drawing.Image.FromFile(file.path);
            //img.Save(fileOut, System.Drawing.Imaging.ImageFormat.Png);
            //img.Dispose();
            //img = null;
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            if (file.name.ToUpper().EndsWith(".SQ"))
                return new SQcontrol(pluginHost, file.path, file.id);
            else if (Path.GetFileName(file.path).Contains("eddn"))
                return new BMPControl(file.path, file.id, pluginHost);
            else if (file.id == 0xFF2)
                return new ScenarioText(pluginHost, file.path, file.id);
            else if (file.id == 0xFF4)
                return new SystemText(file.path, file.id, pluginHost);
            else if (file.id == 0xF76)
                return new MQuestText(pluginHost, file.path, file.id);
            else if (file.name.EndsWith(".txt"))
                return new SubtitleControl(pluginHost, file.path, file.id);
            else if (file.name == "arm9.bin")
                return new MainWin(pluginHost);
            else if (file.name.EndsWith(".tmap"))
                return new TMAPcontrol(file, pluginHost);

            if (Text.TextControl.IsSupported(file.id))
                return new Text.TextControl(pluginHost, file);

            return new System.Windows.Forms.Control();
        }

        public String Pack(ref sFolder unpacked, sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new String(br.ReadChars(4));
            br.Close();

			if (ext == "NPCK") {
				string fileOut = pluginHost.Get_TempFile();
                
				NPCK.Pack(fileOut, ref unpacked, pluginHost);
				return fileOut;
			} else if (ext == "KPCN") {
				string fileOut = pluginHost.Get_TempFile();

				KPCN.Pack(fileOut, file.path, ref unpacked, pluginHost);
				return fileOut;
			} else if (ext == "spdl") {
				string fileOut = pluginHost.Get_TempFile();

				SPDL.Pack(fileOut, file.path, ref unpacked, pluginHost);
				return fileOut;
			}

            return null;
        }
        public sFolder Unpack(sFile file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "NPCK")
                return NPCK.Unpack(file.path, file.name);
            else if (ext == "KPCN")
                return KPCN.Unpack(file.path, file.name);
            else if (ext == "spdl")
                return SPDL.Unpack(file);
            else if (ext == "WMAP")
                return WMAP.Unpack(file.path, file.name);

            return new sFolder();
        }
    }
}
