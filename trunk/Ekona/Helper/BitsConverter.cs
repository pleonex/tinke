// ----------------------------------------------------------------------
// <copyright file="BitConverter.cs" company="none">

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
// <date>24/06/2012 14:28:44</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekona.Helper
{
    public static class BitsConverter
    {
        // From Byte
        public static Byte[] ByteToBits(Byte data)
        {
            List<Byte> bits = new List<byte>();

            for (int j = 7; j >= 0; j--)
                bits.Add((byte)((data >> j) & 1));

            return bits.ToArray();
        }
        public static Byte[] ByteToBit2(Byte data)
        {
            Byte[] bit2 = new byte[4];

            bit2[0] = (byte)(data & 0x3);
            bit2[1] = (byte)((data >> 2) & 0x3);
            bit2[2] = (byte)((data >> 4) & 0x3);
            bit2[3] = (byte)((data >> 6) & 0x3);

            return bit2;
        }
        public static Byte[] ByteToBit4(Byte data)
        {
            Byte[] bit4 = new Byte[2];

            bit4[0] = (byte)(data & 0x0F);
            bit4[1] = (byte)((data & 0xF0) >> 4);

            return bit4;
        }
        public static Byte[] BytesToBit4(Byte[] data)
        {
            byte[] bit4 = new byte[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte[] b4 = ByteToBit4(data[i]);
                bit4[i * 2] = b4[0];
                bit4[i * 2 + 1] = b4[1];
            }
            return bit4;
        }
        public static String BytesToHexString(Byte[] bytes)
        {
            string result = "";

            for (int i = 0; i < bytes.Length; i++)
                result += String.Format("{0:X}", bytes[i]);

            return result;
        }

        // To Byte
        public static Byte[] BitsToBytes(Byte[] bits)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes.Add(newByte);
            }

            return bytes.ToArray();
        }
        public static Byte Bit4ToByte(Byte[] data)
        {
            return (byte)(data[0] + (data[1] << 4));
        }
        public static Byte Bit4ToByte(Byte b1, Byte b2)
        {
            return (byte)(b1 + (b2 << 4));
        }
        public static Byte[] Bits4ToByte(Byte[] data)
        {
            byte[] b = new byte[data.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
                b[i / 2] = Bit4ToByte(data[i], data[i + 1]);

            return b;
        }
        public static Byte[] StringToBytes(String text, int num_bytes)
        {
            string hexText = text.Replace("-", "");
            hexText = hexText.PadRight(num_bytes * 2, '0');

            List<Byte> hex = new List<byte>();
            for (int i = 0; i < hexText.Length; i += 2)
                hex.Add(Convert.ToByte(hexText.Substring(i, 2), 16));

            return hex.ToArray();
        }

    }
}
