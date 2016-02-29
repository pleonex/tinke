//
//  NitroHeader.cs
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

namespace Models3D.Structures
{
    public struct NitroHeader
    {
        public string MagicStamp { get; set; }
        public ushort ByteOrderMask { get { return 0xFEFF; } }
        public ushort Version { get; set; }
        public uint   FileSize { get; set; }
        public ushort DataOffset { get; set; }
        public ushort Blocks { get; set; }
        public uint[] BlocksOffset { get; set; }
    }
}

