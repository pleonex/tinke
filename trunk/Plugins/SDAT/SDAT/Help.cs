/*
 * Copyright (C) 2011  rafael1193
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
 * Programador: rafael1193
 * 
 * Fecha: 29/06/2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDAT
{
    public static class Help
    {
        internal static Char[] ConvertirBytesAChars(Byte[] bytes)
        {
            Char[] ch = new char[bytes.Length];

            for (int i = 0; i < bytes.Length; ++i)
            {
                ch[i] = Convert.ToChar(bytes[i]);
            }

            return ch;
        }

        internal static Byte[] ConvertirCharsABytes(Char[] chars)
        {
            Byte[] bytes = new byte[chars.Length];

            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = Convert.ToByte(chars[i]);
            }

            return bytes;
        }

    }
}
