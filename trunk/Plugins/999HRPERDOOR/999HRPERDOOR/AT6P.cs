/*
 * Copyright (C) 2013 CUE 
 * Decrypt method translated to C# by pleoNeX
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
 */
using System;
using System.IO;

namespace _999HRPERDOOR
{
    // Use only for research porpouse
    public static class AT6P
    {
        public static MemoryStream Decode(Stream encoded)
        {
			MemoryStream decoded = null;
			BinaryReader br = new BinaryReader(encoded);

            // Header check (AT6P)
            if (br.ReadUInt32() != 0x50365441) {
                throw new InvalidDataException("Wrong file header");
            }

			// Get decoded size
			encoded.Position = 0x10;
			int decSize = (int)(br.ReadUInt32() & 0x00FFFFFF);
			decoded     = new MemoryStream(decSize);

			encoded.Position = 0x14;
			decoded.Position = 0;

			if (decSize <= 4)
				return decoded;

			int prev = -1;
			int code = encoded.ReadByte();
			decoded.WriteByte((byte)code);
			encoded.Position++;

			uint nbits = 0;
			uint flags = 0;

			// Decode until fill buffer
			while (decoded.Position < decSize) {

				// Fill the flag
				while (nbits < 17) {
					if (encoded.Position < encoded.Length)
						flags |= (uint)(encoded.ReadByte() << (int)nbits);
					nbits += 8;
				}

				int nbit;
				for (nbit = 0; nbit <= 8; nbit++)
					if ((flags & (1 << nbit)) != 0)
						break;
				if (nbit > 8) {
					throw new Exception("ERROR: Invalid control mask");
				}

				uint n = (uint)(1 << nbit) - 1;
				n += (flags >> (nbit + 1)) & n;

				if (n == 1) {
					if (prev == -1) {
						throw new Exception("ERROR: Unexpected control mask found");
					}
					decoded.WriteByte((byte)prev);

					int t = prev;
					prev = code;
					code = t;
				} else {
					if (n != 0)
						prev = code;
					code = (int)((code + (n >> 1) * (1 - 2 * (n & 1))) & 0xFF);
					decoded.WriteByte((byte)code);
				}

				flags >>= 2 * nbit + 1;
				nbits  -= (uint)(2 * nbit + 1);
			}

			decoded.Flush();
            return decoded;
        }
    }
}
