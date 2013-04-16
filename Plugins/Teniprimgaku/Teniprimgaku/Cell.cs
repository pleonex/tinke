// ----------------------------------------------------------------------
// <copyright file="Cell.cs" company="none">
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
// <date>25/03/2013 2:13:36</date>
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
        0x00 - 4 - Magic stamp "CELL"
        0x04 - 4 - Number of banks
        0x08 - 4 - Palette offset
        0x0C - 4 - Palette + Image length
        0x10 - 4 - Unknown offset (animation section?)

	        For each bank 0x18 bytes
	        0x00 - 2 - Unknown
	        0x02 - 1 - Number of OAMs
            0x03 - 1 - Unknown (used to be 0x00 but also 0x10)
	        0x04 - 2 - Unknown Y coordinate (sprite coordinate?)
	        0x06 - 2 - Unknown X coordinate (sprite coordinate?)
	        0x08 - 2 - Sprite width
	        0x0A - 2 - Sprite height
	        0x0C - 4 - OAM offset
	        0x10 - 4 - Unknown (always 0?) (palette base address?)
	        0x14 - 4 - Image base address

	        OAM structure 0x10 bytes
	        0x00 - 2 - Obj0
	        0x02 - 2 - Obj1
	        0x04 - 2 - OAM Y coordinate
	        0x06 - 2 - OAM X coordinate
	        0x08 - 2 - Unknown (always 0?)
	        0x0A - 2 - Tile offset (* 0x20)
	        0x0C - 4 - Unknown (always 0?) 
	        Final y position = "OAM Y coord" + "yPos" from Obj0 - (OAM height * 1.525)
	        Final x position = "OAM X coord" + "xPos" from Obj1 - (OAM width * 1.525)
     */

    public class Cell
    {
        public const uint MagicStamp = 0x4C4C4543; // "CELL"
        private const int HeaderSize = 0x14;
        private const int BankHeaderSize = 0x18;

        public Cell(sFile file)
        {
            this.Id = file.id;
            this.Name = file.name;
            this.Read(file.path);
        }

        public ushort Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public GameSprite Sprite
        {
            get;
            set;
        }

        private void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            if (br.ReadUInt32() != MagicStamp)
            {
                throw new InvalidDataException();
            }

            uint numSprites = br.ReadUInt32();
            uint dataOffset = br.ReadUInt32();
            int dataLength = br.ReadInt32();
            uint unkOffset = br.ReadUInt32();

            Bank[] sprites = new Bank[numSprites];
            for (int i = 0; i < numSprites; i++)
            {
                br.BaseStream.Position = HeaderSize + i * BankHeaderSize;
                sprites[i] = new Bank();

                ushort unk1 = br.ReadUInt16();
                byte numOams = br.ReadByte();
                byte unk2 = br.ReadByte();
                short yBasePos = br.ReadInt16();
                short xBasePos = br.ReadInt16();
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();
                uint oamOffset = br.ReadUInt32();
                uint unk7 = br.ReadUInt32();
                uint baseOffset = br.ReadUInt32();

                sprites[i].oams = new OAM[numOams];
                br.BaseStream.Position = oamOffset;
                for (int j = 0; j < numOams; j++)
                {
                    ushort obj0 = br.ReadUInt16();
                    ushort obj1 = br.ReadUInt16();
                    short yPos = br.ReadInt16();
                    short xPos = br.ReadInt16();
                    ushort unkOam3 = br.ReadUInt16();
                    ushort tileOffset = br.ReadUInt16();
                    uint unkOam4 = br.ReadUInt32();

                    sprites[i].oams[j] = Actions.OAMInfo(obj0, obj1, 0);
                    sprites[i].oams[j].obj0.yOffset = yPos + sprites[i].oams[j].obj0.yOffset - (int)(sprites[i].oams[j].height * 1.525);
                    sprites[i].oams[j].obj1.xOffset = xPos + sprites[i].oams[j].obj1.xOffset - (int)(sprites[i].oams[j].width * 1.525);
                    sprites[i].oams[j].obj2.tileOffset = tileOffset;
                    sprites[i].oams[j].num_cell = (ushort)j;
                }
            }

            this.Sprite = new GameSprite();
            this.Sprite.Sprite = new RawSprite(sprites, 0x20);

            ColorFormat depth = ColorFormat.colors16;
            if (sprites.Length > 0 && sprites[0].oams.Length > 0)
            {
                depth = (sprites[0].oams[0].obj0.depth == 0) ? ColorFormat.colors16 : ColorFormat.colors256;
            }
            int palSize = (depth == ColorFormat.colors16) ? 0x20 : 0x200;
                
            // Get the palette
            br.BaseStream.Position = dataOffset;
            this.Sprite.Palette = new RawPalette(
                Actions.BGR555ToColor(br.ReadBytes(palSize)),
                false,
                depth,
                this.Name);

            // Get the image data
            br.BaseStream.Position = dataOffset;
            byte[] imageData = br.ReadBytes(dataLength);
            this.Sprite.Image = new RawImage(
                imageData,
                TileForm.Lineal,
                depth,
                0x100,
                imageData.Length / 0x100 * ((depth == ColorFormat.colors16) ? 2 : 1),
                false,
                this.Name);

            br.Close();
            br = null;
        }
    }
}
