// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

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

// <author>Daviex94 and pleonex</author>
// <email>david.iuffri94@hotmail.it</email>
// <date>17/07/2012 13:39:00:</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Ekona;
using Ekona.Images;

namespace TIMEHOLLOW
{
    public static class Compression
    {
        public static byte[] Decompress(byte[] encrypted, uint dcmpSize)
        {
            uint r0, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12;
            byte[] SP1 = new byte[0x100];   // Temp data 1 (var_128)
            byte[] SP2 = new byte[0x100];   // Temp data 2 (var_228)
            byte[] SP3 = new byte[0x100];   // Temp data 3 (var_2A8)
            uint LR;

            byte[] decrypted = new byte[dcmpSize];

            // Initialize
            r0 = 0;
            r1 = (uint)encrypted.Length;
            r2 = 0;

            // Start of code
            r3 = r0;                                     //MOV     R3, R0 - Pointer Start File

            // Loop start here
            for (; ; )
            {
                r4 = r0 - r3;                                //SUB     R4, R0, R3 - Calculate Size of File decrypted
                if (r4 == r1)                                //CMP     R4, R1 - Compare Size Decrypted with Size of File
                {
                    return decrypted;
                }

                r7 = encrypted[r0++];                        //LDRB    R7, [R0],#1 - Take first byte from encrypted data
                r6 = 0;                                      //ADD     R6, SP, #0x2A8+var_128 -> Index of SP1 to 0

                // Clear SP1 array
                r5 = 0;                                      //MOV     R5, #0
                do
                {
                    r4 = r5 + 1;                             //ADD     R4, R5, #1 - Add to R4, R5 + 1
                    r4 = r4 << 16;                           //MOV     R4, R4,LSL#16
                    SP1[r6++] = (byte)r5;                    //STRB    R5, [R6],#1
                    r5 = (uint)((int)r4 >> 16);              //MOV     R5, R4,ASR#16
                }
                while (r5 < 256);                            //CMP     R5, #0x100

                r6 = 0;                                      //MOV     R6, #0
                LR = r6;                                     //MOV     LR, R6 = 0
                r5 = 0;                                      //ADD     R5, SP, #0x2A8+var_128 -> Index to 0 for SP1
                r4 = 0;                                      //ADD     R4, SP, #0x2A8+var_228 -> Index to 0 for SP2
                r12 = r6;                                    //MOV     R12, R6

                do
                {
                    if (r7 > 0x7F)                               //CMP     R7, #0x7F
                    {
                        r7 -= 0x7F;                                   //SUB     R7, R7, #0x7F
                        r6 += r7;                                    //ADD     R6, R6, R7
                        r6 <<= 16;                               //MOV     R6, R6,LSL#16
                        r7 = LR;                                     //MOV     R7, LR
                        r6 = (uint)((int)r6 >> 16);                  //MOV     R6, R6,ASR#16
                    }

                    if (r6 == 0x100)                               //CMP     R6, #0x100
                        break;

                    r10 = r12;                                   //MOV     R10, R12

                    if (r7 < 0)                                  //CMP     R7, #0
                    {
                        if (r6 != 0x100)
                            r7 = encrypted[r0++];
                        continue;
                    }

                    r8 = r5 + r6;                                //ADD     R8, R5, R6 -> Index for SP1
                    r9 = r4 + r6;                                //ADD     R9, R4, R6 -> Index for SP2

                    do
                    {
                        r11 = encrypted[r0++];                       //LDRB    R11, [R0],#1
                        r10++;                                    //ADD     R10, R10, #1
                        r10 <<= 16;                             //MOV     R10, R10,LSL#16
                        SP1[r8] = (byte)r11;

                        if (r6 != r11)                               //CMP     R6, R11
                        {
                            r11 = encrypted[r0++];                   //LDRNEB  R11, [R0],#1
                            SP2[r9] = (byte)r11;                   //STRNEB  R11, [R9]
                        }
                        r6++;                                        //ADD     R6, R6, #1
                        r6 <<= 16;                               //MOV     R6, R6,LSL#16

                        r10 = (uint)((int)r10 >> 16);            //MOV     R10, R10,ASR#16
                        r8++;                                        //ADD     R8, R8, #1  
                        r9++;                                        //ADD     R9, R9, #1
                        r6 = (uint)((int)r6 >> 16);                  //MOV     R6, R6,ASR#16

                    } while (r7 >= r10 ); //CMP     R7, R10,ASR#16


                    // End of loop
                    if (r6 != 0x100)
                        r7 = encrypted[r0++];
                } while (r6 != 0x100);

                r4 = encrypted[r0];
                r5 = encrypted[r0 + 1];
                r0 += 2;

                r4 = r4 << 24;
                r4 = r5 + (uint)((int)r4 >> 16);
                r4 = r4 << 16;
                r6 = (uint)((int)r4 >> 16);
                r7 = 0;

                r5 = 0;                         // Index for SP3
                r4 = 0;                         // Index for SP1
                r12 = 0;                        // Index for SP2

                for (; ; )
                {
                    if (r7 == 0)
                    {
                        r8 = r6 - 1;
                        r8 = r8 << 16;

                        if (r6 == 0)
                        {
                            r6 = (uint)((int)r8 >> 16);
                            break;
                        }
                        r6 = (uint)((int)r8 >> 16);
                        r9 = encrypted[r0++];
                    }
                    else
                    {
                        r7 = r7 - 1;
                        r8 = r7 << 16;
                        r7 = (uint)((int)r8 >> 16);
                        r9 = SP3[r5 + r7];
                    }

                    LR = SP1[r4 + r9];
                    if (r9 == LR)
                    {
                        decrypted[r2++] = (byte)r9;
                        continue;
                    }

                    r8 = r7 + 1;
                    r8 = r8 << 16;
                    r10 = SP2[r12 + r9];
                    r9 = (uint)((int)r8 >> 16);
                    r9 = r9 + 1;
                    SP3[r5 + r7] = (byte)r10;
                    r7 = r9 << 16;
                    SP3[r5 + (uint)((int)r8 >> 16)] = (byte)LR;
                    r7 = (uint)((int)r7 >> 16);
                }
                
            }
        }
    }
}