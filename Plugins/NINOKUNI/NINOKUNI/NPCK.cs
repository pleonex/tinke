// ----------------------------------------------------------------------
// <copyright file="NPCK.cs" company="none">

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
// <date>29/04/2012 13:38:54</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
{
    public static class NPCK
    {
        public static sFolder Unpack(string file, string name)
        {
            name = Path.GetFileNameWithoutExtension(name);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint header_size = br.ReadUInt32();
            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();

                if (offset == 0x00 || size == 0x00)
                    continue;

                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString() + Get_Extension(num_files, i);
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = file;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
        public static void Pack(string file, ref sFolder unpacked, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(file));
            uint offset = (uint)0x09 * 8 + 0xC;

            bw.Write(new char[] { 'N', 'P', 'C', 'K' });
            bw.Write(offset);
            bw.Write((uint)0x09);

            List<byte> buffer = new List<byte>();
            BinaryReader br;
            for (int i = 0; i < 9; i++)
            {

                int num = Search_File(i, unpacked);

                if (num != -1)
                {
                    sFile cfile = unpacked.files[num];

                    // Get file data
                    br = new BinaryReader(File.OpenRead(cfile.path));
                    br.BaseStream.Position = cfile.offset;
                    buffer.AddRange(br.ReadBytes((int)cfile.size));
                    br.Close();

                    // Update file info
                    cfile.offset = offset;
                    cfile.path = file;

                    unpacked.files[num] = cfile;

                    // Write FAT
                    bw.Write(offset);
                    bw.Write(cfile.size);
                    offset += cfile.size;
                }
                else
                {
                    bw.Write((uint)0x00);   // Null offset
                    bw.Write((uint)0x00);   // Null size
                }
            }

            bw.Write(buffer.ToArray());

            bw.Flush();
            bw.Close();
        }

        public static int Search_File(int num, sFolder folder)
        {
            for (int i = 0; i < folder.files.Count; i++)
            {
                string name = folder.files[i].name;
                name = name.Substring(name.LastIndexOf('.') - 1, 1);
                if (Convert.ToInt32(name) == num)
                    return i;
            }

            return -1;
        }

        public static string Get_Extension(uint packType, int num_file)
        {
            switch (packType)
            {
                case 9: // N2D (Nitro 2D), files with 2D images
                    switch (num_file)
                    {
                        case 0: return ".nclr";
                        case 1: return ".ncgr";
                        case 2: return ".ncgr";
                        case 3: return ".ncer";
                        case 5: return ".nanr";
                        case 6: return ".nscr";
                        default: return ".bin";
                    }

                case 6: // N3D (Nitro 3D), files with 3D models and animations
                    switch (num_file)
                    {
                        case 0: return ".bmd0";
                        case 1: return ".bca0";
                        case 2: return ".bva0";
                        case 3: return ".bma0";
                        case 4: return ".bta0";
                        case 5: return ".btp0";
                        default: return ".bin";
                    }

                case 2: // NPD (Nitro Pulse Digitial??), files with sounds
                    switch (num_file)
                    {
                        case 0: return ".sedl";
                        case 1: return ".swdl";
                        default: return ".bin";
                    }

                default:
                    return ".bin";
            }
        }
    }

    /* In N2D files there must be 9 offset in this order:
    * 
    * 0 - Palette       (nclr)
    * 1 - 1º Tiles      (ncgr)
    * 2 - 2º Tiles      (ncgr)
    * 3 - 1º Cell       (ncer)
    * 4 - Nothing
    * 5 - 1º Animation  (nanr)
    * 6 - 1º Map        (nscr)
    * 7 - Nothing
    * 8 - Nothing
    */

    /* In NPD files there must be 2 offset in this order:
     * 
     * 0 - sedl         (sedl)
     * 1 - swdl         (swdl)
     */

    /* In N3D files there must be 6 offset in this order:
     * 
     * 0 - BMD0
     * 1 - BCA0
     * 2 - BVA0
     * 3 - BMA0
     * 4 - BTA0
     * 5 - BTP0
     */
}
