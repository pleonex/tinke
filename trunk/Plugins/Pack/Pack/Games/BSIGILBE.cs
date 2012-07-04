// ----------------------------------------------------------------------
// <copyright file="BSIGILBE.cs" company="none">

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
// <date>11/05/2012 2:28:22</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Pack.Games
{
    public class BSIGILBE : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "CBXE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (magic[0] == 0x08)
                return Format.Compressed;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            return null;
        }
        public sFolder Unpack(sFile file)
        {
            return Decrypt(file.path, file.name);
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        public sFolder Decrypt(string fileIn, string name)
        {
            byte[] dataIn = File.ReadAllBytes(fileIn);
            if (dataIn[0] != 0x08)
            {
                Console.WriteLine("Unvalid file type: " + dataIn[0].ToString("x"));
                Console.ReadKey(true);
                return new sFolder();
            }

            byte h = dataIn[1];
            byte h1 = (byte)(h & 0x1F);
            int dec_size = 1 << h1;

            int upd_bit = (int)((uint)h >> 5);
            int min_it = 1 << upd_bit;
            min_it--;

            // Get the first control code
            uint control = dataIn[2];
            control += (uint)(dataIn[3] << 8);
            control += (uint)(dataIn[4] << 16);
            control += (uint)(dataIn[5] << 24);
            uint tcontrol = control;

            int bits_mask = 0;
            int mask = 0;
            int max_byte_read = 1;
            int unread_bits = 0x20;

            byte[] buffer = new byte[dec_size];
            int pos_buf = 0;
            int pos_dat = 6;

            #region First region
            while (pos_buf < dec_size)
            {
                tcontrol = control >> 1;

                if ((control & 1) == 0)
                {
                    buffer[pos_buf++] = (byte)tcontrol;

                    control = tcontrol >> 8;
                    unread_bits -= 9;

                    if (pos_buf >= max_byte_read)
                    {
                        max_byte_read <<= 1;
                        bits_mask++;
                        mask <<= 1;
                        mask++;
                    }
                }
                else
                {
                    int index = (int)(tcontrol & mask);

                    control = tcontrol >> bits_mask;
                    int it = (int)(min_it & (tcontrol >> bits_mask));
                    it++;
                    control >>= upd_bit;

                    for (int i = 0; i < it; i++)
                    {
                        if (pos_buf == buffer.Length)
                            Array.Resize(ref buffer, buffer.Length + 1);
                        buffer[pos_buf++] = buffer[index++];
                    }

                    int bits_read = bits_mask + upd_bit;
                    bits_read++;
                    unread_bits -= bits_read;

                    while (pos_buf >= max_byte_read)
                    {
                        max_byte_read <<= 1;
                        bits_mask++;
                        mask <<= 1;
                        mask++;
                    }
                }

                // Update control            
                while (unread_bits <= 0x18)
                {
                    byte bc = dataIn[pos_dat++];
                    control |= (uint)(bc << unread_bits);
                    unread_bits += 8;
                }
            }
            #endregion
            #region Second region
            bits_mask = h1 + upd_bit + 1;
            while (pos_dat < dataIn.Length)
            {
                tcontrol = control >> 1;

                if ((control & 1) == 0)
                {
                    if (pos_buf == buffer.Length)
                        Array.Resize(ref buffer, buffer.Length + 1);
                    buffer[pos_buf++] = (byte)tcontrol;
                    control = tcontrol >> 8;
                    unread_bits -= 9;
                }
                else
                {
                    int index = dec_size - 1;
                    index = (int)(tcontrol & index);
                    index += pos_buf;
                    index -= dec_size;

                    uint bit_c = tcontrol >> h1;
                    int it = (int)(min_it & (tcontrol >> h1));
                    it++;
                    control = bit_c >> upd_bit;

                    for (int i = 0; i < it; i++)
                    {
                        if (pos_buf == buffer.Length)
                            Array.Resize(ref buffer, buffer.Length + 1);
                        buffer[pos_buf++] = buffer[index++];
                    }
                    unread_bits -= bits_mask;
                }

                // Update control
                do
                {
                    if (pos_dat >= dataIn.Length)
                        break;
                    byte bc = dataIn[pos_dat++];
                    control |= (uint)(bc << unread_bits);
                    unread_bits += 8;
                } while (unread_bits <= 0x18);
            }
            #endregion

            string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar +
                Path.GetRandomFileName() + Path.GetFileNameWithoutExtension(fileIn) + ".dec";
            File.WriteAllBytes(tempFile, buffer);

            sFile dec = new sFile();
            dec.name = name + ".dec";
            dec.offset = 0;
            dec.size = (uint)buffer.Length;
            dec.path = tempFile;
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();
            unpack.files.Add(dec);

            return unpack;
        }
    }
}
