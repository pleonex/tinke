//
//  Binary2NitroFile.cs
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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Models3D.Structures
{
    public class Binary2NitroFile : IConverter<Stream, NitroFile>
    {
        private ReadOnlyDictionary<string, Type> supportedBlocks;

        public Binary2NitroFile(Dictionary<string, Type> supportedBlocks)
        {
            this.supportedBlocks = new ReadOnlyDictionary<string, Type>(supportedBlocks);
        }

        public void Convert(Stream binary, NitroFile nitroFile)
        {
            long basePosition = binary.Position;

            var headerConverter = new Binary2NitroHeader(nitroFile.HasHeaderOffsets);
            var header = headerConverter.Convert(binary);
            nitroFile.Header = header;

            nitroFile.Blocks = new NitroFile.BlockCollection();
            for (int i = 0; i < header.Blocks; i++)
            {
                // If there are offsets in the header, use them!
                if (nitroFile.HasHeaderOffsets)
                    binary.Position = basePosition + header.BlocksOffset[i];

                // Sanity check for position.
                if (binary.Position >= binary.Length) {
                    Console.WriteLine("##ERROR?## Missing {0} blocks", header.Blocks - i);
                    return;
                }

                var reader = new BinaryReader(binary);
                long blockPosition = binary.Position;

                // Get block name to infer the type
                string blockName = ReadMagicStamp(reader, nitroFile.HasHeaderOffsets);

                // Get the block size to skip later to next block
                int blockSize = reader.ReadInt32();

                if (!supportedBlocks.ContainsKey(blockName))
                    throw new FormatException("Unknown block: " + blockName);

                // Search and convert
                binary.Position = blockPosition;
                var sectionType = supportedBlocks[blockName];

                var section = (NitroBlock)Activator.CreateInstance(sectionType, nitroFile);
                FileFormat.ConvertFrom<Stream>(binary, sectionType, section);
                nitroFile.Blocks.Add(section);

                binary.Position = blockPosition + blockSize;
            }
        }

        private static string ReadMagicStamp(BinaryReader reader, bool reverse)
        {
            var stamp = reader.ReadChars(4);
            return reverse ? new string(stamp) : new string(stamp.Reverse().ToArray());
        }
    }
}

