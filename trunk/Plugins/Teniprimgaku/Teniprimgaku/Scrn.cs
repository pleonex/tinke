// ----------------------------------------------------------------------
// <copyright file="Scrn.cs" company="none">
// Copyright (C) 2013
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
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
//
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>25/03/2013 0:53:11</date>
// -----------------------------------------------------------------------
namespace Teniprimgaku
{
    using System;
    using System.Drawing;
    using System.IO;
    using Ekona;
    using Ekona.Images;

    /*
        Header:
        0x00 - 4 - Magic stamp (SCRN)
        0x04 - 2 - Number of images
	        For each image 0x10 bytes
	        0x00 - 2 - image data offset (* 0x20)
	        0x02 - 2 - image data length (* 0x20)
	        0x04 - 2 - palette offset (* 0x20)
	        0x06 - 2 - number of palettes (4bpp or 8bpp)
	        0x08 - 2 - map offset (* 0x20)
	        0x0A - 2 - number of X tiles
	        0x0C - 2 - number of Y tiles 
	        0x0E - 2 - map type, 0-> common NTFS, 1-> 1 16bits value for tiles, another for palette
	
        Padding 0x20
     */

    public class Scrn
    {
        public const uint MagicStamp = 0x4E524353; //"SCRN"
        private const int HeaderSize = 0x10;
        private const int Padding = 0x20;

        private ushort id;
        private GameImage[] images;

        public Scrn(sFile file)
        {
            this.id = file.id;
            this.Name = file.name;
            this.Read(file.path);
        }

        public string Name
        {
            get;
            private set;
        }

        public int NumberImages
        {
            get;
            private set;
        }

        public GameImage this[int index]
        {
            get { return this.images[index]; }
            set { this.images[index] = value; }
        }

        public void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            if (br.ReadUInt32() != MagicStamp)
            {
                throw new InvalidDataException();
            }

            this.NumberImages = br.ReadUInt16();
            this.images = new GameImage[this.NumberImages];
            for (int i = 0; i < this.NumberImages; i++)
            {
                this.images[i] = new GameImage();
                br.BaseStream.Position = 6 + i * HeaderSize;

                int imageOffset = br.ReadUInt16() * Padding;
                int imageSize = br.ReadUInt16() * Padding;
                int palOffset = br.ReadUInt16() * Padding;
                int palNumber = br.ReadUInt16();
                int mapOffset = br.ReadUInt16() * Padding;
                int tilesX = br.ReadUInt16();
                int tilesY = br.ReadUInt16();
                int mapType = br.ReadUInt16();
                
                // Set map
                br.BaseStream.Position = mapOffset;
                NTFS[] map = new NTFS[tilesX * tilesY];
                bool multiPalette = false;
                for (int m = 0; m < map.Length; m++)
                {
                    if (mapType == 0)
                    {
                        map[m] = Actions.MapInfo(br.ReadUInt16());
                    }
                    else
                    {
                        ushort v1 = br.ReadUInt16();
                        ushort v2 = br.ReadUInt16();
                        map[m].nPalette = (byte)v2;
                        map[m].nTile = v1;
                    }

                    if (map[m].nPalette != 0)
                    {
                        multiPalette = true;
                    }
                }
                this.images[i].Map = new RawMap(map, tilesX * 8, tilesY * 8, false, this.Name);

                ColorFormat depth;
                int palSize;
                if (!multiPalette && palNumber == 0x10)
                {
                    depth = ColorFormat.colors256;
                    palSize = 0x200;
                    palNumber = 1;
                }
                else
                {
                    depth = ColorFormat.colors16;
                    palSize = 0x20;
                }

                // Set palette
                br.BaseStream.Position = palOffset;
                Color[][] colors = new Color[palNumber][];
                for (int p = 0; p < palNumber; p++)
                {
                    colors[p] = Actions.BGR555ToColor(br.ReadBytes(palSize));
                }
                this.images[i].Palette = new RawPalette(colors, false, depth, this.Name);

                // Set image
                br.BaseStream.Position = imageOffset;
                byte[] imgData = br.ReadBytes(imageSize);
                this.images[i].Image = new RawImage(
                    imgData,
                    TileForm.Horizontal,
                    depth,
                    0x100,
                    imgData.Length * (depth == ColorFormat.colors16 ? 2 : 1) / 0x100,
                    false,
                    this.Name);

            }

            br.Close();
            br = null;
        }
    }
}
