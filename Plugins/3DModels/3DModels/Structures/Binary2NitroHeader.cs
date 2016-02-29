//
//  Binary2NitroHeader.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Linq;
using System.IO;

namespace Models3D.Structures
{
    public class Binary2NitroHeader : IConverter<Stream, NitroHeader>
    {
        public Binary2NitroHeader(bool hasOffsets)
        {
            HasOffsets = hasOffsets;
        }

        public bool HasOffsets { get; private set; }

        public NitroHeader Convert(Stream binary)
        {
            var header = new NitroHeader();
            Convert(binary, header);
            return header;
        }

        public void Convert(Stream binary, NitroHeader header)
        {
            var reader = new BinaryReader(binary);
            header.MagicStamp = ReadMagicStamp(reader);

            ushort bom = reader.ReadUInt16();
            if (bom != header.ByteOrderMask)
                throw new InvalidDataException("The data is not little endian.");

            header.Version    = reader.ReadUInt16();
            header.FileSize   = reader.ReadUInt32();
            header.DataOffset = reader.ReadUInt16();
            header.Blocks     = reader.ReadUInt16();

            if (HasOffsets) {
                header.BlocksOffset = new uint[header.Blocks];
                for (int i = 0; i < header.Blocks; i++)
                    header.BlocksOffset[i] = reader.ReadUInt32();
            }
        }

        private string ReadMagicStamp(BinaryReader reader)
        {
            var stamp = reader.ReadChars(4);
            return HasOffsets ? new string(stamp) : new string(stamp.Reverse().ToArray());
        }
    }
}

