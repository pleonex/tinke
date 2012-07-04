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
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace EDGEWORTH
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool IsCompatible()
        {
            if (gameCode == "C32P" || gameCode == "C32J" || gameCode == "C32E")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper() == "ROMFILE.BIN")
                return Format.Pack;

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
            return new Control();
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.name.ToUpper().EndsWith("ROMFILE.BIN"))
            {
                String packFile = pluginHost.Get_TempFile();
                if (File.Exists(packFile))
                    File.Delete(packFile);

                PACK.Pack(packFile, ref unpacked);
                return packFile;
            }

            return null;
        }
        public sFolder Unpack(sFile file) 
        {
            if (file.name.ToUpper().EndsWith("ROMFILE.BIN"))
            {
                System.Threading.Thread waiting = new System.Threading.Thread(ThreadWait);
                String lang = "";
                try
                {
                    System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                        "Plugins" + System.IO.Path.DirectorySeparatorChar + "EDGEWORTHLang.xml");
                    lang = xml.Element(pluginHost.Get_Language()).Element("S02").Value;
                }
                catch { throw new NotSupportedException("There was an error reading the language file"); }
                waiting.Start(lang);

                sFolder desc = PACK.Unpack(file.path, pluginHost);

                waiting.Abort();
                return desc;
            }

            return new sFolder();
        }

        private void ThreadWait(object name)
        {
            Espera wait = new Espera((string)name);
            wait.ShowDialog();
        }
    }
}
