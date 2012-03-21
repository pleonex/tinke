﻿/*
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
 *   by pleoNeX
 * 
 */ 
using System.Windows.Forms;

namespace PluginInterface
{
    /// <summary>
    /// IPlugin interface to support a file format
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// First method to be called. It passes the class IPluginHost
        /// </summary>
        /// <param name="pluginHost">Class with common and necessary methods</param>
        void Initialize(IPluginHost pluginHost);
        /// <summary>
        /// Get the file format
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="magic">First four bytes of the file</param>
        /// <param name="id">ID to do operation using PluginHost with the file</param>
        /// <returns>File format</returns>
        Format Get_Format(string fileName, byte[] magic, int id);

        /// <summary>
        /// This method is called when the user click on button "view", it will return a control with the file information
        /// </summary>
        /// <param name="file">File to analyze</param>
        /// <param name="id">File ID</param>
        /// <returns>Control that will be displayed</returns>
        Control Show_Info(string file, int id);
        /// <summary>
        /// This methos will be called when the user double click a file.
        /// It's used for a fast reading, 
        /// ie: double click the palette file instead of click on the button View if you want to see the next image.
        /// It should call the methods Set_NCLR, Set_NANR, Set_Objects.... in pluginHost.
        /// </summary>
        /// <param name="file">File to analyze</param>
        /// <param name="id">File ID</param>
        void Read(string file, int id);

        /// <summary>
        /// It will be called when the user click on button "Unpack".
        /// </summary>
        /// <param name="file">File path where the file to unpack is</param>
        /// <param name="id">File ID</param>
        /// <returns>sFolder variable with files to show in the tree interface</returns>
        sFolder Unpack(string file);
        /// <summary>
        /// It will be called when the user click on button "Pack"
        /// </summary>
        /// <param name="unpacked">sFolder variable with all the unpacked files to pack</param>
        /// <param name="file">Path where the original pack file is</param>
        /// <returns>Path where the new pack file is</returns>
        string Pack(ref sFolder unpacked, string file);
    }
}