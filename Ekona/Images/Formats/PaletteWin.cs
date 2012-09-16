// ----------------------------------------------------------------------
// <copyright file="PaletteWin.cs" company="none">

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
// <date>28/04/2012 19:01:44</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;

namespace Ekona.Images.Formats
{
    public class PaletteWin : PaletteBase
    {
        bool gimp_error;        // Error of Gimp, it reads the first colors at 0x1C instead of 0x18
        public PaletteWin(string file) : base()
        {
            Read(file);
        }
        public PaletteWin(Color[] colors) : base()
        {
            Set_Palette(new Color[][] { colors }, true);
        }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            br.ReadChars(4);  // RIFF
            br.ReadUInt32();
            br.ReadChars(4);  // PAL
            br.ReadChars(4);  // data
            br.ReadUInt32();  // unknown, always 0x00
            br.ReadUInt16();  // unknown, always 0x0300
            ushort nColors = br.ReadUInt16();

            Color[][] colors = new Color[1][];
            colors[0] = new Color[nColors];
            for (int j = 0; j < nColors; j++)
            {
                Color newColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                br.ReadByte(); // always 0x00
                colors[0][j] = newColor;
            }

            br.Close();
            Set_Palette(colors, true);
        }

        public override void Write(string fileOut)
        {
            if (File.Exists(fileOut))
                File.Delete(fileOut);

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });            // "RIFF"
            bw.Write((uint)(0x10 + palette[0].Length * 4));         // file_length - 8
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });            // "PAL "
            bw.Write(new char[] { 'd', 'a', 't', 'a' });            // "data"
            bw.Write((uint)palette[0].Length * 4 + 4);              // data_size = file_length - 0x14
            bw.Write((ushort)0x0300);                               // version = 00 03
            bw.Write((ushort)(palette[0].Length));                  // num_colors
            if (gimp_error) bw.Write((uint)0x00);                   // Error in Gimp 2.8
            for (int i = 0; i < palette[0].Length; i++)
            {
                bw.Write(palette[0][i].R);
                bw.Write(palette[0][i].G);
                bw.Write(palette[0][i].B);
                bw.Write((byte)0x00);
                bw.Flush();
            }

            bw.Close();
        }


        public bool Gimp_Error
        {
            get { return gimp_error; }
            set { gimp_error = value; }
        }
    }
}
