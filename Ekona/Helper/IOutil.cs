// ----------------------------------------------------------------------
// <copyright file="IOutil.cs" company="none">

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
// <date>04/07/2012 12:55:15</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ekona.Helper
{
    public static class IOutil
    {
        public static void Append(ref BinaryWriter bw, string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Append(ref bw, ref br);

            br.Close();
            br = null;
        }
        public static void Append(ref BinaryWriter bw, ref BinaryReader br)
        {
            const int block_size = 0x80000; // 512 KB
            int size = (int)br.BaseStream.Length;

            while (br.BaseStream.Position + block_size < size)
            {
                bw.Write(br.ReadBytes(block_size));
                bw.Flush();
            }

            int rest = size - (int)br.BaseStream.Position;
            bw.Write(br.ReadBytes(rest));
            bw.Flush();
        }

    }
}
