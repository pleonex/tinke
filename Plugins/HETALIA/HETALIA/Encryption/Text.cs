// ----------------------------------------------------------------------
// <copyright file="EncryptText.cs" company="none">

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
// <date>21/04/2012 2:03:49</date>
// -----------------------------------------------------------------------
using System;

namespace HETALIA.Encryption
{
    public class Text
    {
        public static byte[] Decrypt(byte[] data)
        {
            int header = BitConverter.ToInt32(data, 0);
            byte check = (byte)((header >> 24) & 0xFF);
            if (check != 0xDA)
            {
                Console.WriteLine("File not encrypted!");
                return new byte[0];
            }

            const uint KEY = 0xDADADADA;
            int size = (int)(header ^ KEY);
            if (size == 0)
            {
                Console.WriteLine("Size 0?");
                return new byte[0];
            }

            int pos = 0;
            byte[] buffer = new byte[size];
            while (pos < size)
            {
                byte b = data[pos];
                b = (byte)(b ^ 0xDA);
                buffer[pos++] = b;
            }

            return buffer;
        }
    }
}
