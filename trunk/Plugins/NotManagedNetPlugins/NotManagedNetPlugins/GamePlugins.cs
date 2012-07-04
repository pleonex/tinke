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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ekona;

namespace NotManagedNetPlugins
{
    public class GamePlugins : IGamePlugin
    {
        #region Plugins to load
        [DllImport("LufiaCurseSinistrals.dll")]
        public static extern bool XIsCompatible(string GameCode);
        [DllImport("LufiaCurseSinistrals.dll", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern System.IntPtr XGetFormat(string filePath, int id);
        [DllImport("LufiaCurseSinistrals.dll", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern bool XDecompress(string file1, string file2, string file3, string id, int* num_files);
        #endregion

        IPluginHost pluginHost;
        string gameCode;
        int pluginLoaded;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public bool IsCompatible()
        {
            if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar +
                "LufiaCurseSinistrals.dll"))
            {
                if (XIsCompatible(gameCode))
                {
                    pluginLoaded = 0;
                    return true;
                }
            }

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            switch (pluginLoaded)
            {
                case 0:
                    IntPtr p = XGetFormat(file.name, file.id);
                    string c = Marshal.PtrToStringAnsi(p);
                    return Helper.StringToFormat(c);
            }

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        public unsafe sFolder Unpack(sFile file)
        {
            switch (pluginLoaded)
            {
                case 0:
                    if (file.id == 0x15)
                    {
                        int num = 0;
                        bool b = XDecompress(file.path, pluginHost.Search_File(0x16), pluginHost.Search_File(0x17), file.id.ToString(), &num);

                        String txtfile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "tinke_file_list.txt";
                        sFolder decompressedFolder = Helper.Get_DecompressedFiles(txtfile, num, pluginHost);

                        return decompressedFolder;
                    }
                    break;
            }

            return new sFolder();
        }
        public String Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
    }
}
