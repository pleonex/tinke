/*
 * Copyright (C) 2011
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
 * Programador: rafael1193, pleonex
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDAT
{
    /// <summary>
    /// Operations with the AD-PCM compression
    /// </summary>
    public static class Compression_ADPCM
    {
        /// <summary>
        /// Decompress an array data of bytes using AD-PCM compression
        /// </summary>
        /// <param name="data">Data to be decompress</param>
        /// <returns>Data decompressed</returns>
        public static byte[] Decompress(byte[] data)
        {
            List<byte> resul = new List<byte>();

            #region Preinitialized variables
            int index = 0;
            int stepsize = 7;
            #endregion

            data = Bit8ToBit4(data);

            int difference, newSample = 0;
            for (int i = 0; i < data.Length; i++)
            {
                difference = 0;

                if ((data[i] & 4) != 0)
                    difference += stepsize;
                if ((data[i] & 2) != 0)
                    difference += stepsize >> 1;
                if ((data[i] & 1) != 0)
                    difference += stepsize >> 2;
                difference += stepsize >> 3;

                if ((data[i] & 8) != 0)
                    difference = -difference;
                newSample += difference;

                if (newSample > 32767)
                    newSample = 32767;
                else if (newSample < -32768)
                    newSample = -32768;

                resul.AddRange(BitConverter.GetBytes((short)newSample));

                index += indexTable[data[i]];
                if (index < 0)
                    index = 0;
                else if (index > 88)
                    index = 88;
                stepsize = stepsizeTable[index];
            }

            return resul.ToArray();
        }
        public static byte[] Decompress(byte[] datos, int sample, int stepindex)
        {
            if (datos.Length < 4)
                return datos;

            List<byte> resul = new List<byte>();

            #region Preinitialized variables
            int index = stepindex;
            if (index < 0)
                index = 0;
            else if (index > 88)
                index = 88;
            int stepsize = stepsizeTable[index];
            #endregion

            byte[] data = new byte[datos.Length - 4];
            Array.Copy(datos, 4, data, 0, data.Length);
            data = Bit8ToBit4(data);

            int difference, newSample = sample;
            for (int i = 0; i < data.Length; i++)
            {
                difference = 0;

                if ((data[i] & 4) != 0)
                    difference += stepsize;
                if ((data[i] & 2) != 0)
                    difference += stepsize >> 1;
                if ((data[i] & 1) != 0)
                    difference += stepsize >> 2;
                difference += stepsize >> 3;

                if ((data[i] & 8) != 0)
                    difference = -difference;
                newSample += difference;

                if (newSample > 32767)
                    newSample = 32767;
                else if (newSample < -32768)
                    newSample = -32768;

                resul.AddRange(BitConverter.GetBytes((short)newSample));

                index += indexTable[data[i]];
                if (index < 0)
                    index = 0;
                else if (index > 88)
                    index = 88;
                stepsize = stepsizeTable[index];
            }

            return resul.ToArray();
        }

        public static byte[] Compress(byte[] data)
        {
            List<Byte> result = new List<byte>();

            #region Preinitialized variables
            int predictedSample = 0;
            int index = 0;
            int stepsize = stepsizeTable[index];
            #endregion

            int different, newSample, mask, tempStepsize;
            for (int i = 0; i < data.Length; i += 2)
            {
                short originalSample = BitConverter.ToInt16(data, i);
                different = originalSample - predictedSample; // find difference from predicted sample

                if (different >= 0) // Set sign bit and find absolute value of difference
                {
                    newSample = 0;  // Set sign bit (newSample[3]) to 0
                }
                else
                {
                    newSample = 8;  // Set sign bit(newSample[3]) to one
                    different = -different;     // Absolute value of negative difference
                }

                mask = 4;       // Used to set bits in newSample
                tempStepsize = stepsize;        // Store quantizer stepsize for later use
                for (int j = 0; j < 3; j++)     // Quantize difference down to four bits
                {
                    if (different >= tempStepsize)      // newSample[2:0] = 4 * (difference / stepsize)
                    {
                        newSample |= mask;      // perfom division...
                        different -= tempStepsize;      // ...through repeated subtraction
                    }
                    tempStepsize >>= 1;     // adjust comparator for next iteration
                    mask >>= 1;     // adjust bit-set mask for next iteration
                }

                result.Add((byte)newSample);    // Store 4-bit newSample

                // Compute new sample estimate predictedSample
                different = 0;      // Calculate difference = (newSample + 1/2) * stepsize / 4
                if ((newSample & 4) != 0)      // perform multiplication through repetitive addition
                    different += stepsize;
                if ((newSample & 2) != 0)
                    different += stepsize >> 1;
                if ((newSample & 1) != 0)
                    different += stepsize >> 2;
                different += stepsize >> 3;      // (newSample + 1/2) * stepsize / 4 = newSample * stepsize / 4 + stepsize / 8

                if ((newSample & 8) != 0)       // account for sign bit
                    different = -different;     // adjust predicted sample based on calculated difference
                predictedSample += different; 

                if (predictedSample > 32767)        // check for overflow
                    predictedSample = 32767;
                else if (predictedSample < -32768)
                    predictedSample = -32768;

                // compute new stepsize
                index += indexTable[newSample];     // adjust index into stepsize lookup table using newSample
                if (index < 0)      // check for index overflow
                    index = 0;
                else if (index > 88)
                    index = 88;
                stepsize = stepsizeTable[index];        // find new quantizer stepsize
            }

            return Bit4ToBit8(result.ToArray());
        }
        public static byte[][] CompressBlock(byte[] data, int blockSize)
        {
            List<Byte[]> result = new List<byte[]>();
            List<Byte> block = new List<byte>();

            #region Preinitialized variables
            int predictedSample = 0;
            int index = 0;
            int stepsize = stepsizeTable[index];
            #endregion

            int different, newSample, mask, tempStepsize;
            for (int i = 0; i < data.Length; i += 2)
            {
                if (i % blockSize == 0)
                {
                    if (i != 0)
                    {
                        Byte[] blockData = Bit4ToBit8(block.GetRange(4, block.Count - 4).ToArray());
                        List<byte> newBlock = new List<byte>();
                        newBlock.AddRange(block.GetRange(0, 4));
                        newBlock.AddRange(blockData);
                        result.Add(newBlock.ToArray());
                    }
                    block = new List<byte>();
                    block.AddRange(BitConverter.GetBytes((short)predictedSample));
                    block.AddRange(BitConverter.GetBytes((short)index));
                }

                short originalSample = BitConverter.ToInt16(data, i);
                different = originalSample - predictedSample; // find difference from predicted sample

                if (different >= 0) // Set sign bit and find absolute value of difference
                {
                    newSample = 0;  // Set sign bit (newSample[3]) to 0
                }
                else
                {
                    newSample = 8;  // Set sign bit(newSample[3]) to one
                    different = -different;     // Absolute value of negative difference
                }

                mask = 4;       // Used to set bits in newSample
                tempStepsize = stepsize;        // Store quantizer stepsize for later use
                for (int j = 0; j < 3; j++)     // Quantize difference down to four bits
                {
                    if (different >= tempStepsize)      // newSample[2:0] = 4 * (difference / stepsize)
                    {
                        newSample |= mask;      // perfom division...
                        different -= tempStepsize;      // ...through repeated subtraction
                    }
                    tempStepsize >>= 1;     // adjust comparator for next iteration
                    mask >>= 1;     // adjust bit-set mask for next iteration
                }

                block.Add((byte)newSample);    // Store 4-bit newSample

                // Compute new sample estimate predictedSample
                different = 0;      // Calculate difference = (newSample + 1/2) * stepsize / 4
                if ((newSample & 4) != 0)      // perform multiplication through repetitive addition
                    different += stepsize;
                if ((newSample & 2) != 0)
                    different += stepsize >> 1;
                if ((newSample & 1) != 0)
                    different += stepsize >> 2;
                different += stepsize >> 3;      // (newSample + 1/2) * stepsize / 4 = newSample * stepsize / 4 + stepsize / 8

                if ((newSample & 8) != 0)       // account for sign bit
                    different = -different;     // adjust predicted sample based on calculated difference
                predictedSample += different;

                if (predictedSample > 32767)        // check for overflow
                    predictedSample = 32767;
                else if (predictedSample < -32768)
                    predictedSample = -32768;

                // compute new stepsize
                index += indexTable[newSample];     // adjust index into stepsize lookup table using newSample
                if (index < 0)      // check for index overflow
                    index = 0;
                else if (index > 88)
                    index = 88;
                stepsize = stepsizeTable[index];        // find new quantizer stepsize
            }

            if (block.Count > 4)
            {
                Byte[] blockData = Bit4ToBit8(block.GetRange(4, block.Count - 4).ToArray());
                List<byte> newBlock = new List<byte>();
                newBlock.AddRange(block.GetRange(0, 4));
                newBlock.AddRange(blockData);
                result.Add(newBlock.ToArray());
            }

            return result.ToArray();
        }

        private static byte[] Bit8ToBit4(byte[] data)
        {
            List<byte> bit4 = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                bit4.Add((byte)(data[i] & 0x0F));
                bit4.Add((byte)((data[i] & 0xF0) >> 4));
            }

            return bit4.ToArray();
        }
        private static byte[] Bit4ToBit8(byte[] bytes)
        {
            List<Byte> bit8 = new List<byte>();

            for (int i = 0; i < bytes.Length; i += 2)
            {
                byte byte1 = bytes[i];
                byte byte2 = 0;
                if (i + 1 < bytes.Length)
                    byte2 = (byte)(bytes[i + 1] << 4);
                bit8.Add((byte)(byte1 + byte2));
            }

            return bit8.ToArray();
        }


        private static int[] indexTable = new int[16] { -1, -1, -1, -1, 2, 4, 6, 8,
                                             -1, -1, -1, -1, 2, 4, 6, 8 };

        private static int[] stepsizeTable = new int[89] { 7, 8, 9, 10, 11, 12, 13, 14,
                                                16, 17, 19, 21, 23, 25, 28,
                                                31, 34, 37, 41, 45, 50, 55,
                                                60, 66, 73, 80, 88, 97, 107,
                                                118, 130, 143, 157, 173, 190, 209,
                                                230, 253, 279, 307, 337, 371, 408,
                                                449, 494, 544, 598, 658, 724, 796,
                                                876, 963, 1060, 1166, 1282, 1411, 1552,
                                                1707, 1878, 2066, 2272, 2499, 2749, 3024, 3327, 3660, 4026,
                                                4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630,
                                                9493, 10442, 11487, 12635, 13899, 15289, 16818,
                                                18500, 20350, 22385, 24623, 27086, 29794, 32767 };
    }
}
