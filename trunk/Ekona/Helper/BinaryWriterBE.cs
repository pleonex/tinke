// ----------------------------------------------------------------------
// <copyright file="BinaryWriterBE.cs" company="none">

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
// <date>28/04/2012 20:18:58</date>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.IO;

namespace Ekona.Helper
{
    // Not finished
    public class BinaryWriterBE : BinaryWriter
    {

        public BinaryWriterBE(string file) : base(File.OpenWrite(file))
        { }

        public override void Write(ushort value)
        {
            byte[] v = BitConverter.GetBytes(value);
            v = v.Reverse().ToArray();
            Write(v);
        }
    }
}
