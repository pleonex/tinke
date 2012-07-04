// ----------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="none">

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
// <date>24/06/2012 15:01:47</date>
// -----------------------------------------------------------------------
using System.Windows.Forms;

namespace Ekona
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
        /// <param name="file">
        /// File info. It's the same variable as used internally.
        /// Normally, the path value will be the rom file.
        /// </param>
        /// <param name="magic">First four bytes of the file</param>
        /// <returns>File format</returns>
        Format Get_Format(sFile file, byte[] magic);

        /// <summary>
        /// This method is called when the user click on button "view", it will return a control with the file information
        /// </summary>
        /// <param name="file">File to analyze. It's not the same variable as used internally.</param>
        /// <returns>Control that will be displayed</returns>
        Control Show_Info(sFile file);

        /// <summary>
        /// This methos will be called when the user double click a file.
        /// It's used for a fast reading, 
        /// ie: double click the palette file instead of click on the button View if you want to see the next image.
        /// It should call the methods Set_Palette, Set_Image, Set_Sprite.... in pluginHost.
        /// </summary>
        /// <param name="file">File to analyze. It's not the same variable as used internally.</param>
        void Read(sFile file);

        /// <summary>
        /// It will be called when the user click on button "Unpack".
        /// </summary>
        /// <param name="file">File to unpack. It's not the same variable as used internally.</param>
        /// <returns>sFolder variable with files to show in the tree interface</returns>
        sFolder Unpack(sFile file);

        /// <summary>
        /// It will be called when the user click on button "Pack"
        /// </summary>
        /// <param name="unpacked">sFolder variable with all the unpacked files to pack</param>
        /// <param name="file">Original pack file. It's not the same variable as used internally.</param>
        /// <returns>Path where the new pack file is</returns>
        string Pack(ref sFolder unpacked, sFile file);
    }
}
