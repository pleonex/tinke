/*
 * Copyright (C) 2012  pleoNeX
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
 */
using System;

namespace _999HRPERDOOR
{
    // Use it only for research porpouse
    public static class AT6P
    {
        public static byte[] Decrypt(byte[] data)
        {
            // Header check
            if (BitConverter.ToUInt32(data, 0) != 0x50365441)
            {
                Console.WriteLine("Wrong file header");
                return new byte[0];
            }

            int dec_size = BitConverter.ToInt32(data, 0x10);

            byte[] buffer = new byte[dec_size];

            int pos_dat = 0x14;
            int pos_buf = 0x00;

            int r0 = 0;
            int r1 = 0;
            int r2 = 0;
            int r3 = 0;
            int r4 = 0;
            int r5 = 0;
            int r6 = 0;
            int r7 = 0;
            int r8 = 0;
            int r11 = -1;

            r5 = data[pos_dat];
            buffer[pos_buf++] = (byte)r5;
            pos_dat = 0x16;

            do
            {
                r3 = r7 - r6;

                if (r3 < 0x10)
                {
                    r1 = (int)(0xFFFFFFFF ^ (r11 << r3));
                    r2 = r8 & r1;
                    if (pos_dat + 1 == data.Length)
                        Array.Resize(ref data, data.Length + 1);
                    r1 = BitConverter.ToUInt16(data, pos_dat);
                    pos_dat += 2;

                    r7 = r3 + 0x10;
                    r6 = 0;
                    r8 = r2 | (r1 << r3);
                }

                if ((r8 & 0x01) != 0)
                {
                    // NOT TESTED
                    buffer[pos_buf++] = (byte)r5;
                    r6++;
                    r8 = (int)((uint)r8 >> 1);
                    continue;
                }

                if ((r8 & 0x02) != 0)
                {
                    // NOT TESTED
                    r1 = (int)((uint)r8 >> 2);
                    r1 &= 1;
                    r4 = r1 + 1;
                    r6 += 3;
                    r8 = (int)((uint)r8 >> 3);

                    if (r4 == 1)
                    {
                        // NOT TESTED
                        buffer[pos_buf] = (byte)r0;
                        r0 = r5;
                        r5 = buffer[pos_buf++];
                        continue;
                    }
                }
                else if ((r8 & 0x04) != 0)
                {
                    // NOT TESTED
                    r0 = (int)((uint)r8 >> 3);
                    r0 &= 3;
                    r4 = r0 + 3;
                    r6 += 5;
                    r8 = (int)((uint)r8 >> 5);
                }
                else if ((r8 & 0x08) != 0)
                {
                    // NOT TESTED
                    r0 = (int)((uint)r8 >> 4);
                    r0 &= 7;
                    r4 = r0 + 7;
                    r6 += 7;
                    r8 = (int)((uint)r8 >> 7);
                }
                else if ((r8 & 0x10) != 0)
                {
                    r0 = (int)((uint)r8 >> 5);
                    r0 &= 0xF;
                    r4 = r0 + 0xF;
                    r6 += 9;
                    r8 = (int)((uint)r8 >> 9);
                }
                else if ((r8 & 0x20) != 0)
                {
                    // NOT TESTED
                    r0 = (int)((uint)r8 >> 6);
                    r0 &= 0x1F;
                    r4 = r0 + 0x1F;
                    r6 += 0xB;
                    r8 = (int)((uint)r8 >> 11);
                }
                else if ((r8 & 0x40) != 0)
                {
                    r0 = (int)((uint)r8 >> 7);
                    r0 &= 0x3F;
                    r4 = r0 + 0x3F;
                    r6 += 0xD;
                    r8 = (int)((uint)r8 >> 13);
                }
                else if ((r8 & 0x80) != 0)
                {
                    // NOT TESTED
                    r0 = (int)((uint)r8 >> 8);
                    r0 &= 0x7F;
                    r4 = r0 + 0x7F;
                    r6 += 0xF;
                    r8 = (int)((uint)r8 >> 15);
                }
                else if ((r8 & 0x100) != 0)
                {
                    // NOT TESTED
                    r6 += 9;
                    r8 = (int)((uint)r8 >> 9);
                    r2 = r7 - r6;

                    if (r2 < 0x10)
                    {
                        // NOT TESTED
                        r0 = (int)(0xFFFFFFFF ^ (r11 << r2));
                        r1 = r8 & r0;
                        r0 = BitConverter.ToUInt16(data, pos_dat);
                        pos_dat += 2;
                        r7 = r2 + 0x10;
                        r6 = 0;
                        r8 = r1 | (r0 << r2);
                    }

                    r0 = r8 & 0xFF;
                    r4 = r0 + 0xFF;
                    r6 += 8;
                    r8 = (int)((uint)r8 >> 8);
                }
                else
                {
                    // NOT SUPPORTED, ERROR?
                    r4 = 0;
                    Console.ReadKey(true);
                }

                if ((r4 & 0x01) == 0)
                    r4 >>= 1;
                else
                {
                    r0 = r4 - 2;
                    r0 = (int)(0xFFFFFFFF ^ r0);
                    r0 >>= 1;
                    r4 = r0 | 0x80;
                }

                r1 = r4 + r5;
                r0 = r5;
                r5 = (byte)r1;
                buffer[pos_buf++] = (byte)r1;

            } while (pos_dat < data.Length & pos_buf < buffer.Length);

            return buffer;
        }
    }
}
