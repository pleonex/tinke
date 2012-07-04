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
using System.IO;
using Ekona;

namespace PSL
{

	public static class PAC
	{
		public static sFolder Unpack(string file)
		{
			BinaryReader br = new BinaryReader(File.OpenRead(file));
			sFolder unpacked = new sFolder();
			unpacked.files = new System.Collections.Generic.List<sFile>();
			
			uint type = br.ReadUInt32();
			uint unknown = br.ReadUInt32();		// Maybe type of pack file
			uint num_files = br.ReadUInt32();
			
			uint offset = br.ReadUInt32();
			for (int i = 0; i < num_files; i++)
			{
				sFile newFile = new sFile();
				newFile.name = "File" + i.ToString();
				newFile.offset = offset;
				newFile.path = file;
				
				// Get the size using the next offset
				if (i + 1 != num_files)
				{
					offset = br.ReadUInt32();
					newFile.size = offset - newFile.offset;
				}
				else
					newFile.size = (uint)(br.BaseStream.Length - newFile.offset);
				
				// Get the type of file
				br.BaseStream.Position = 0x2c + i * 4;
				uint file_type = br.ReadUInt32();
				br.BaseStream.Position = 0x0C + (i + 2) * 4;
				
				if (file_type == 0)
					newFile.name += ".bmd0";
				else if (file_type == 1)
					newFile.name += ".btx0";
				else if (file_type == 2)
					newFile.name += ".btp0";
				else if (file_type == 7)
					newFile.name += ".bta0";
				else
					newFile.name += ".bin";
				
				unpacked.files.Add(newFile);
			}
			
			br.Close();
			return unpacked;
		}
	}
}
