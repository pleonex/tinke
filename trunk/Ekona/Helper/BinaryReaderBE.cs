// ----------------------------------------------------------------------
// <copyright file="BinaryReaderBE.cs" company="none">

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
// <date>28/04/2012 20:22:31</date>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.IO;

namespace Ekona.Helper
{
    // Not finished
    public class BinaryReaderBE : BinaryReader
    {
        public BinaryReaderBE(string file) : base(File.OpenRead(file))
        { }

        public override ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2).Reverse().ToArray(), 0);
        }

    }
}
