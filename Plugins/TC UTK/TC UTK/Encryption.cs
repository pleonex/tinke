// ----------------------------------------------------------------------
// <copyright file="Encryption.cs" company="none">

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
// <date>21/04/2012 11:20:12</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;

namespace TC_UTK
{
    class Encryption
    {
        List<byte> final;
        byte[] data;
        int pos_dat;
        int enc_size;

        byte[] buffer;
        int pos_buf;

        ushort decoded;
        int pos_dec;
        ushort control;

        public Encryption(byte[] dataIn)
        {
            final = new List<byte>();
            data = dataIn;
            pos_dat = 0;
            enc_size = data.Length;

            buffer = new byte[0x1000];
            pos_buf = 0xFEE;

            decoded = 0;
            pos_dec = 0;
            control = 0;
        }

        public byte[] Decrypt()
        {
            do
            {
                control >>= 1;  // Update control code

                // It's like a loop because it adds 0xFF at the
                // end of the control byte
                if ((control & 0x100) == 0)
                {
                    // Load a new control code
                    if (enc_size == 0)
                        break;

                    control = data[pos_dat++];
                    enc_size--;
                    control |= 0xFF00;
                }

                // Read control code
                if ((control & 1) == 1)
                    Flag1();
                else
                    Flag0();

            } while (enc_size > 0);

            return final.ToArray();
        }

        /// <summary>
        /// Get x bytes from the buffer
        /// </summary>
        private void Flag0()
        {
            // Read two bytes
            if (enc_size == 0)
                return;
            byte b1 = data[pos_dat++];
            enc_size--;

            if (enc_size == 0)
                return;
            byte b2 = data[pos_dat++];
            enc_size--;

            // Index of the buffer
            ushort index0 = (ushort)(b2 & 0xF0);
            index0 <<= 4;
            index0 |= b1;   // index = b1 | ((b2 & 0xF0) << 4)

            // X bytes to read
            ushort loop_end = (ushort)(b2 & 0x0F);
            loop_end += 2;

            for (ushort i = 0; i <= loop_end; i++)
            {
                // Update index
                int index = index0 + i;
                index &= 0xFFF;

                // Get a byte from the buffer
                byte value = buffer[index];
                decoded += (ushort)(value << (pos_dec++ * 8));

                // If the decoded value is completed write it
                if (pos_dec == 2)
                {
                    final.AddRange(BitConverter.GetBytes((decoded)));
                    pos_dec = 0;
                    decoded = 0;
                }

                // Update the buffer
                buffer[pos_buf++] = value;
                pos_buf &= 0xFFF;
            }
        }

        /// <summary>
        /// Read a byte
        /// </summary>
        private void Flag1()
        {
            if (enc_size == 0)
                return;

            byte b1 = data[pos_dat++];
            enc_size--;

            // Get the decoded byte
            decoded += (ushort)(b1 << (pos_dec++ * 8));

            // Write then
            if (pos_dec == 2)
            {
                final.AddRange(BitConverter.GetBytes((decoded)));
                pos_dec = 0;
                decoded = 0;
            }

            // Update the buffer
            buffer[pos_buf++] = b1;
            pos_buf &= 0xFFF;
        }

    }
}
