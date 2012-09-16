// ----------------------------------------------------------------------
// <copyright file="Overlay.cs" company="none">

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
// <date>28/04/2012 14:26:34</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Tinke.Nitro
{
    public static class Overlay
    {
        public static Estructuras.ARMOverlay[] LeerOverlays(string file, UInt32 offset, UInt32 size, bool arm9)
        {
            Estructuras.ARMOverlay[] overlays = new Estructuras.ARMOverlay[size / 0x20];
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            for (int i = 0; i < overlays.Length; i++)
            {
                overlays[i] = new Estructuras.ARMOverlay();

                overlays[i].fileID = br.ReadUInt32();
                overlays[i].RAM_Adress = br.ReadUInt32();
                overlays[i].RAM_Size = br.ReadUInt32();
                overlays[i].BSS_Size = br.ReadUInt32();
                overlays[i].stInitStart = br.ReadUInt32();
                overlays[i].stInitEnd = br.ReadUInt32();
                overlays[i].fileID = br.ReadUInt32();
                overlays[i].reserved = br.ReadUInt32();
                overlays[i].ARM9 = arm9;
            }

            return overlays;
        }

        public static sFile[] LeerOverlaysBasico(string file, UInt32 offset, UInt32 size, bool arm9)
        {
            sFile[] overlays = new sFile[size / 0x20];
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            for (int i = 0; i < overlays.Length; i++)
            {
                overlays[i] = new sFile();
                overlays[i].name = "overlay" + (arm9 ? '9' : '7') + '_' + br.ReadUInt32();
                br.ReadBytes(20);
                overlays[i].id = (ushort)br.ReadUInt32();
                br.ReadBytes(4);

            }

            return overlays;

        }
        public static sFile[] ReadBasicOverlays(string romFile, UInt32 offset, UInt32 size, bool arm9, Estructuras.sFAT[] fat)
        {
            sFile[] overlays = new sFile[size / 0x20];
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = offset;

            for (int i = 0; i < overlays.Length; i++)
            {
                overlays[i] = new sFile();
                overlays[i].name = "overlay" + (arm9 ? '9' : '7') + '_' + br.ReadUInt32();
                br.ReadBytes(20);
                overlays[i].id = (ushort)br.ReadUInt32();
                br.ReadBytes(4);
                overlays[i].offset = fat[overlays[i].id].offset;
                overlays[i].size = fat[overlays[i].id].size;
                overlays[i].path = romFile;

            }

            br.Close();
            return overlays;
        }

        public static void Write_Y9(BinaryWriter bw, BinaryReader br, sFile[] overlays)
        {
            for (int i = 0; i < overlays.Length; i++)
            {
                bw.Write(br.ReadBytes(0x1C));
                byte[] d = BitConverter.GetBytes(overlays[i].size);
                bw.Write(d[0]);
                bw.Write(d[1]);
                bw.Write(d[2]);
                br.BaseStream.Position += 3;
                bw.Write(br.ReadByte());
            }
        }

        public static void EscribirOverlays(string salida, List<sFile> overlays, string romFile)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(salida));
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));

            for (int i = 0; i < overlays.Count; i++)
            {
                if (overlays[i].path == romFile)
                {
                    br.BaseStream.Position = overlays[i].offset;
                    bw.Write(br.ReadBytes((int)overlays[i].size));
                }
                else
                {
                    BinaryReader br2 = new BinaryReader(new FileStream(overlays[i].path, FileMode.Open));
                    br2.BaseStream.Position = overlays[i].offset;
                    bw.Write(br2.ReadBytes((int)overlays[i].size));
                    br2.Close();
                }

                uint rem = (uint)bw.BaseStream.Position % 0x200;
                if (rem != 0)
                {
                    while (rem < 0x200)
                    {
                        bw.Write((byte)0xFF);
                        rem++;
                    }
                }

            }

            br.Close();
            bw.Flush();
            bw.Close();
        }
    }
}
