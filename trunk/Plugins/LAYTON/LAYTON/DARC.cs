/*
 * Copyright (C) 2011  CUE, pleoNeX
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
 * By: CUE, pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace LAYTON
{
    /// <summary>
    /// Pack files from LAYTON4 game (London Life). Credits to CUE
    /// </summary>
    public static class DARC
    {

        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint number_files = br.ReadUInt32();

            for (int i = 0; i < number_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString() + ".denc";
                newFile.offset = br.ReadUInt32() + (uint)br.BaseStream.Position;    // Relative offset
                newFile.path = file;

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = newFile.offset - 4;
                newFile.size = br.ReadUInt32();
                br.BaseStream.Position = currPos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static void Pack(string fileOut, ref sFolder unpacked)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(new char[] { 'D', 'A', 'R', 'C' });
            bw.Write((uint)unpacked.files.Count);

            uint offset = (uint)unpacked.files.Count * 4 + 8;
            uint[] offset_files = new uint[unpacked.files.Count];
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                offset_files[i] = (uint)(offset - bw.BaseStream.Position);
                if (offset_files[i] % 4 != 0)
                    offset_files[i] += (4 - (offset_files[i] % 4));
                bw.Write(offset_files[i]);
                offset_files[i] += (uint)bw.BaseStream.Position;

                offset += unpacked.files[i].size + 4;
            }

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                bw.Write(unpacked.files[i].size);

                BinaryReader br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;
                bw.Write(br.ReadBytes((int)unpacked.files[i].size));
                br.Close();

                int rem = (int)bw.BaseStream.Position % 4;
                if (rem != 0)
                    for (; rem < 4; rem++)
                        bw.Write((byte)0x00);
                bw.Flush();

                sFile newFile = unpacked.files[i];
                newFile.offset = offset_files[i];
                newFile.path = fileOut;
                unpacked.files[i] = newFile;
            }

            bw.Flush();
            bw.Close();
        }
    }

    /// <summary>
    /// Compressed files inside of DARC. Credits again to CUE :)
    /// </summary>
    public static class DENC
    {

        public static sFolder Unpack(string file, string name, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder decode = new sFolder();
            decode.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint decoded_length = br.ReadUInt32();
            bool coded = (new String(br.ReadChars(4)) == "LZSS" ? true : false);
            uint coded_length = br.ReadUInt32();

            if (!coded)
            {
                sFile newFile = new sFile();
                newFile.name = name;
                newFile.offset = (uint)br.BaseStream.Position;
                newFile.size = decoded_length;
                newFile.path = file;

                newFile.name += '.' + new String(br.ReadChars(4));

                decode.files.Add(newFile);
            }
            else
            {
                sFile newFile = new sFile();
                newFile.name = name;
                newFile.offset = 0;
                newFile.size = decoded_length;
                newFile.path = pluginHost.Get_TempFile();

                Decode_LZSS(br.ReadBytes((int)coded_length), newFile.path, (int)decoded_length);

                br.Close();
                br = new BinaryReader(File.OpenRead(newFile.path));
                newFile.name += '.' + new String(br.ReadChars(4));

                decode.files.Add(newFile);
            }

            br.Close();
            return decode;
        }

        public static void Pack(string fileOut, sFolder unpacked)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(new char[] { 'D', 'E', 'N', 'C' });
            bw.Write(unpacked.files[0].size);
            bw.Write(new char[] { 'N', 'U', 'L', 'L' });
            bw.Write(unpacked.files[0].size);

            BinaryReader br = new BinaryReader(File.OpenRead(unpacked.files[0].path));
            br.BaseStream.Position = unpacked.files[0].offset;
            bw.Write(br.ReadBytes((int)unpacked.files[0].size));
            br.Close();

            bw.Flush();
            bw.Close();
        }

        public static void Decode_LZSS(byte[] encoded, string fileOut, int decode_length)
        {
            Byte[] buffer = new byte[decode_length];
            int pos_buf = 0;
            int pos_enc = 0;

            while (pos_buf < decode_length)
            {
                int length = 0;
                if ((encoded[pos_enc] & 1) == 0)    // No coded, just copy
                {
                    length = encoded[pos_enc++] >> 1;
                    for (int i = 0; i < length; i++)
                        buffer[pos_buf++] = encoded[pos_enc++];
                }
                else                                // Coded, go back and copy
                {
                    ushort value = BitConverter.ToUInt16(encoded, pos_enc);
                    pos_enc += 2;

                    length = (value >> 0xC) + 2;
                    int cod_pos = pos_buf - ((value & 0xFFF) >> 1);

                    for (int i = 0; i < length; i++)
                        buffer[pos_buf++] = buffer[cod_pos++];
                }
            }

            File.WriteAllBytes(fileOut, buffer);
        }
    }
}
