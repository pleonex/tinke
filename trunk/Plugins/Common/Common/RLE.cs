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
 * Programa utilizado: SharpDevelop
 * Fecha: 10/02/2011
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace Common
{
	/// <summary>
	/// Métodos para datos comprimidos con RLE (Run-Length Encodign)
	/// </summary>
	public static class RLE
	{
		public static byte[] Descomprimir_Pixel(byte[] datos, byte depth)
		{
			if (depth % 8 != 0)
				throw new NotImplementedException("Profundidad no soportada: " + depth.ToString());
			
			List<byte> des = new List<byte>();

			for (int pos = 0; pos < datos.Length; )
			{
				if (datos[pos] >= 0x80)
				{
					int rep = datos[pos] - 0x80 + 1;
					pos++;
					
					for (; rep > 0; rep--)
						for (int d = 0; d < (depth / 8); d++)
							des.Add(datos[pos]);
				}
				else
				{
					int rep = datos[pos] + 1;
					pos++;
					
					for (; rep > 0; rep--)
						for (int d = 0; d < (depth/8); d++, pos++)
							des.Add(datos[pos]);
				}
				pos++;
			}
			
			return des.ToArray();

		}
		public static byte[] Descomprimir_Pixel(string file, ref long pos, byte depth, ushort width, ushort height)
		{
			if (depth % 8 != 0)
				throw new NotImplementedException("Profundidad no soportada: " + depth.ToString());
			
			BinaryReader br = new BinaryReader(File.OpenRead(file));
			br.BaseStream.Position = pos;
			
			List<byte> des = new List<byte>();

			for (; des.Count < ((width * height) * (depth / 8)); ) // Paramos cuando se tengan todos los pixels
			{
				byte id = br.ReadByte();
				
				if (id >= 0x80)
				{
					int rep = id - 0x80 + 1;
					byte[] dato = br.ReadBytes(depth / 8);
					
					for (; rep > 0; rep--)
						des.AddRange(dato);
				}
				else
				{
					int rep = id + 1;
					
					for (; rep > 0; rep--)
						des.AddRange(br.ReadBytes(depth / 8));
				}
			}
			
			pos = br.BaseStream.Position;
			br.Close();
			return des.ToArray();
		}
	}
}
