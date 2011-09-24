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
 *   by pleoNeX
 * 
 */ 
using System.Windows.Forms;

namespace PluginInterface
{
    public interface IPlugin
    {
        void Initialize(IPluginHost pluginHost);
        Format Get_Format(string fileName, byte[] magic);

        Control Show_Info(string file, int id);     // When the user click button "View"
        void Read(string file, int id);     // When the user double click a file or in pack/compressed file when the button Decompress is clicked
    }
}
