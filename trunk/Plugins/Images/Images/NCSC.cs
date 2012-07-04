/*
 * Copyright (C) 2011  pleoNeX
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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NCSC : MapBase
    {
        public NCSC(string file, int id, string fileName = "") : base(file, id, fileName) { }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sNCSC ncsc = new sNCSC();

            // Nitro generic header
            ncsc.generic.id = br.ReadChars(4);
            ncsc.generic.endianess = br.ReadUInt16();
            ncsc.generic.constant = br.ReadUInt16();
            ncsc.generic.file_size = br.ReadUInt32();
            ncsc.generic.header_size = br.ReadUInt16();
            ncsc.generic.nSection = br.ReadUInt16();

            // SCRN section
            ncsc.scrn.id = br.ReadChars(4);
            ncsc.scrn.size = br.ReadUInt32();
            ncsc.scrn.width = br.ReadUInt32() * 8;
            ncsc.scrn.height = br.ReadUInt32() * 8;
            ncsc.scrn.unknown1 = br.ReadUInt32();
            ncsc.scrn.unknown2 = br.ReadUInt32();

            NTFS[] map = new NTFS[(ncsc.scrn.size - 0x18) / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            // Read other sections
            for (int n = 1; n < ncsc.generic.nSection; n++)
            {
                String type = new String(br.ReadChars(4));

                switch (type)
                {
                    case "ESCR":

                        ncsc.escr.id = "ESCR".ToCharArray();
                        ncsc.escr.size = br.ReadUInt32();
                        ncsc.escr.width = br.ReadUInt32();
                        ncsc.escr.height = br.ReadUInt32();
                        ncsc.escr.unknown = br.ReadUInt32();
                        ncsc.escr.unknown2 = br.ReadUInt32();

                        ncsc.escr.unknownData = new uint[ncsc.escr.width * ncsc.escr.height];
                        for (int i = 0; i < ncsc.escr.unknownData.Length; i++)
                            ncsc.escr.unknownData[i] = br.ReadUInt32();
                        break;

                    case "CLRF":

                        ncsc.clrf.id = "CLRF".ToCharArray();
                        ncsc.clrf.size = br.ReadUInt32();
                        ncsc.clrf.width = br.ReadUInt32();
                        ncsc.clrf.height = br.ReadUInt32();
                        ncsc.clrf.unknown = br.ReadBytes((int)ncsc.clrf.size - 0x10);
                        break;

                    case "CLRC":

                        ncsc.clrc.id = "CLRC".ToCharArray();
                        ncsc.clrc.size = br.ReadUInt32();
                        ncsc.clrc.unknown = br.ReadBytes((int)ncsc.clrc.size - 0x08);
                        break;

                    case "GRID":

                        ncsc.grid.id = "GRID".ToCharArray();
                        ncsc.grid.size = br.ReadUInt32();
                        ncsc.grid.unknown = br.ReadBytes((int)ncsc.grid.size - 0x08);
                        break;

                    case "LINK":

                        ncsc.link.id = "LINK".ToCharArray();
                        ncsc.link.size = br.ReadUInt32();
                        ncsc.link.link = new string(br.ReadChars((int)ncsc.link.size - 0x08));
                        break;

                    case "CMNT":

                        ncsc.cmnt.id = "CMNT".ToCharArray();
                        ncsc.cmnt.size = br.ReadUInt32();
                        ncsc.cmnt.unknown = br.ReadBytes((int)ncsc.cmnt.size - 0x08);
                        break;
                }
            }

            br.Close();
            Set_Map(map, false, (int)ncsc.scrn.width, (int)ncsc.scrn.height);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
        }

        public struct sNCSC
        {
            public NitroHeader generic;
            public SCRN scrn;
            public ESCR escr;
            public CLRF clrf;
            public CLRC clrc;
            public GRID grid;
            public LINK link;
            public CMNT cmnt;

            public struct SCRN
            {
                public char[] id;
                public uint size;
                public uint width;
                public uint height;
                public uint unknown1;
                public uint unknown2;
            }
            public struct ESCR
            {
                public char[] id;
                public uint size;
                public uint width;
                public uint height;
                public uint unknown;
                public uint unknown2;

                public uint[] unknownData;
            }
            public struct CLRF
            {
                public char[] id;
                public uint size;
                public uint width;
                public uint height;
                public byte[] unknown;
            }
            public struct CLRC
            {
                public char[] id;
                public uint size;
                public byte[] unknown;
            }
            public struct GRID
            {
                public char[] id;
                public uint size;
                public byte[] unknown;
            }
            public struct LINK
            {
                public char[] id;
                public uint size;
                public string link;
            }
            public struct CMNT
            {
                public char[] id;
                public uint size;
                public byte[] unknown;
            }
        }
    }
}
