// ----------------------------------------------------------------------
// <copyright file="EncryptImage.cs" company="none">

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
// <date>21/04/2012 2:03:17</date>
// -----------------------------------------------------------------------
using System;

namespace HETALIA.Encryption
{
    public static class Image
    {
        public static Byte[] Decrypt(byte[] data)
        {
            int width = BitConverter.ToInt16(data, 8);
            int height = BitConverter.ToInt16(data, 12);
            int num_colors = BitConverter.ToInt16(data, 0xE);
            int img_pos = num_colors * 2 + 0x20;

            int dec_size = width * height + img_pos;
            int enc_size = data.Length - img_pos;

            byte[] buffer = new byte[dec_size];
            Array.Copy(data, 0, buffer, 0, img_pos);

            int pos_buf = img_pos;
            int pos_control = img_pos;
            int pos_enc = 0;

            // Get encryption data offset (encryption control size)
            pos_enc = BitConverter.ToInt16(data, pos_control);
            if (pos_enc == pos_control) // pos_enc == 0
            {
                pos_enc = BitConverter.ToInt32(data, pos_control);
                pos_control += 2;
            }
            pos_control += 2;
            pos_enc += pos_control;

            // Initialize values
            byte[] list = new byte[0x10];
            Array.Copy(BitConverter.GetBytes(0xFFFFFFFE), 0, list, 0, 4);
            Array.Copy(BitConverter.GetBytes(0 - width), 0, list, 4, 4);
            Array.Copy(BitConverter.GetBytes(0 - width - 2), 0, list, 8, 4);
            Array.Copy(BitConverter.GetBytes(0 - width + 2), 0, list, 12, 4);

            // Start of decryption
            while (pos_buf < dec_size)
            {
                // Read encryption control code
                int control = data[pos_control++];

                // Check type of encryption
                if (control < 0x10)     // Copy X bytes
                {
                    int loop = control + 1;
                    for (; loop != 0; loop--, pos_enc += 2, pos_buf += 2)
                    {
                        short value = BitConverter.ToInt16(data, pos_enc);
                        Array.Copy(BitConverter.GetBytes(value), 0, buffer, pos_buf, 2);
                    }
                }
                else if (control < 0xC0)
                {
                    int pos = -1 - control + 0x10;
                    pos <<= 1;
                    // Read past value
                    short value = BitConverter.ToInt16(data, pos_enc + pos);
                    Array.Copy(BitConverter.GetBytes(value), 0, buffer, pos_buf, 2);
                    pos_buf += 2;
                }
                else
                {
                    control -= 0xC0;

                    int pos = control >> 4;
                    pos <<= 2;
                    pos = BitConverter.ToInt32(list, pos);
                    pos += pos_buf;

                    int loop = control & 0xF;
                    loop++;

                    for (; loop != 0; loop--, pos += 2, pos_buf += 2)
                    {
                        short value = BitConverter.ToInt16(buffer, pos);
                        Array.Copy(BitConverter.GetBytes(value), 0, buffer, pos_buf, 2);
                    }
                }
            }

            return buffer;
        }
    }
}
