/*
 * Copyright (C) 2012  pleonex
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
 *
 * Creado por SharpDevelop.
 * Fecha: 16/03/2012
 *
 */
using System;
using System.Text;
using System.Collections.Generic;
using PluginInterface;

namespace PSL
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
			if (gameCode == "VPYJ")
				return true;
			
			return false;
		}
		
		public Format Get_Format(string fileName, byte[] magic, int id)
		{
			string ext = new string(Encoding.ASCII.GetChars(magic));
			uint exti = BitConverter.ToUInt32(magic, 0);
			
			if (ext == "LINK")
				return Format.Pack;
			else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
				return Format.Pack;
			
			return Format.Unknown;
		}
		
		public System.Windows.Forms.Control Show_Info(string file, int id)
		{
			return new System.Windows.Forms.Control();
		}		
		public void Read(string file, int id)
		{
		}
		
		public sFolder Unpack(string file, int id)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
			byte[] header = br.ReadBytes(4);
			string ext = new String(Encoding.ASCII.GetChars(header));
			uint exti = BitConverter.ToUInt32(header, 0);
			
			if (ext == "LINK")
				return LINK.Unpack(file);
			else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
				return PAC.Unpack(file);
			
			return new sFolder();
		}		
		public string Pack(ref sFolder unpacked, string file, int id)
		{
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
            byte[] header = br.ReadBytes(4);
            string ext = new String(Encoding.ASCII.GetChars(header));
            uint exti = BitConverter.ToUInt32(header, 0);

            if (ext == "LINK")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetRandomFileName() +
                    System.IO.Path.GetFileName(file);
                LINK.Pack(file, fileOut, ref unpacked);
                return fileOut;
            }

            return null;
		}
	}
}