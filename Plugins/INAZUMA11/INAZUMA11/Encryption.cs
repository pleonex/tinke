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

// <author>pleoNeX and Daviex94</author>
// <email>benito356@gmail.com</email>
// <date>21/08/2012 15:15:22</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace INAZUMA11
{
    public static class Encryption
    {
        public static sFolder Decrypt(sFile item, int blockSize, IPluginHost pluginHost)
        {
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            byte[] data = File.ReadAllBytes(item.path);
			int numBlocks = data.Length / blockSize;

            for (int i = 0; i < numBlocks; i++)
            {
                byte[] block = new byte[blockSize];
                Array.Copy(data, i * blockSize, block, 0, blockSize);
                Decrypt(ref block);
                Array.Copy(block, 0, data, i * blockSize, blockSize);
            }

            string fileout = pluginHost.Get_TempFile();
            File.WriteAllBytes(fileout, data);

            sFile newItem = new sFile();
            newItem.path = fileout;
            newItem.offset = 0;
            newItem.name = item.name;
            newItem.size = (uint)data.Length;
            unpacked.files.Add(newItem);

            return unpacked;
        }
        public static void Encrypt(string fileIn, int blockSize, string fileout)
        {
            byte[] data = File.ReadAllBytes(fileIn);
			int numBlocks = data.Length / blockSize;

            for (int i = 0; i < numBlocks; i++)
            {
                byte[] block = new byte[blockSize];
                Array.Copy(data, i * blockSize, block, 0, blockSize);
                Encrypt(ref block);
                Array.Copy(block, 0, data, i * blockSize, blockSize);
            }

            File.WriteAllBytes(fileout, data);
        }
        
        private static void Decrypt(ref byte[] data)
        {
            // First operation: XOR with 0xAD
            for (int i = 0; i < data.Length; i++)
                data[i] ^= 0xAD;

            // Second operation: Shift bits
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                int t = 0;
                for (int l = 0; l < 2; l++)
                {
                    t = b << 7;
                    t |= (int)b >> 1;
                    b = (byte)(t & 0xFF);
                }
                data[i] = b;
            }

            // Third operation: Shift bytes
            for (int i = 0; i < data.Length - 2; i += 3)
            {
                byte b1 = data[i];
                byte b2 = data[i + 2];
                data[i] = b2;
                data[i + 2] = b1;
            }


            for (int i = 0; i < data.Length - 4; i += 5)
            {
                byte b1 = data[i];
                byte b2 = data[i + 4];
                data[i] = b2;
                data[i + 4] = b1;
            }

            for (int i = 0; i < data.Length - 6; i += 7)
            {
                byte b1 = data[i];
                byte b2 = data[i + 6];
                data[i] = b2;
                data[i + 6] = b1;
            }

            for (int i = 0; i < data.Length - 1; i += 2)
            {
                byte b1 = data[i];
                byte b2 = data[i + 1];
                data[i] = b2;
                data[i + 1] = b1;
            }
        }
        private static void Encrypt(ref byte[] data)
        {
            // Reverse decrypt

            // First operation: Shift bytes
            for (int i = 0; i < data.Length - 1; i += 2)
            {
                byte b1 = data[i];
                byte b2 = data[i + 1];
                data[i] = b2;
                data[i + 1] = b1;
            }

            for (int i = 0; i < data.Length - 6; i += 7)
            {
                byte b1 = data[i];
                byte b2 = data[i + 6];
                data[i] = b2;
                data[i + 6] = b1;
            }

            for (int i = 0; i < data.Length - 4; i += 5)
            {
                byte b1 = data[i];
                byte b2 = data[i + 4];
                data[i] = b2;
                data[i + 4] = b1;
            }

            for (int i = 0; i < data.Length - 2; i += 3)
            {
                byte b1 = data[i];
                byte b2 = data[i + 2];
                data[i] = b2;
                data[i + 2] = b1;
            }

            // Second operation: Shift bits
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                int t = 0;
                for (int l = 0; l < 2; l++)
                {
                    t = b >> 7;
                    t |= b << 1;
                    b = (byte)(t & 0xFF);
                }
                data[i] = b;
            }

            // Third operation: XOR with 0xAD
            for (int i = 0; i < data.Length; i++)
                data[i] ^= 0xAD;
        }
    }

}
