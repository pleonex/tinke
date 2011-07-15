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
 * Fecha: 11/07/2011
 * 
 */

using System;

namespace SDAT
{
    internal class Help
    {

        internal static Char[] BytesToChars(Byte[] bytes)
        {
            Char[] ch = new char[bytes.Length];

            for (int i = 0; i < bytes.Length; ++i)
            {
                ch[i] = Convert.ToChar(bytes[i]);
            }

            return ch;
        }

        internal static Byte[] CharsToBytes(Char[] chars)
        {
            Byte[] bytes = new byte[chars.Length];

            for (int i = 0; i < bytes.Length; ++i)
            {
                bytes[i] = Convert.ToByte(chars[i]);
            }

            return bytes;
        }

        internal static sbyte[] BytesToSbytes(byte[] bytes)
        {
            sbyte[] sbytes = new sbyte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                sbytes[i] = unchecked((sbyte)(bytes[i] ^ 0x80));
            }

            return sbytes;
        }
    }
}
