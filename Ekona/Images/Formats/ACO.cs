// ----------------------------------------------------------------------
// <copyright file="ACO.cs" company="none">

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
// <date>28/04/2012 19:33:55</date>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Ekona.Images.Formats
{
    /// <summary>
    /// Adobe COlor
    /// Info from http://www.nomodes.com/aco.html thanks to "Larry Tesler"
    /// </summary>
    public class ACO : PaletteBase
    {
        public ACO(string file)
            : base()
        {
            Read(file);
        }
        public ACO(Color[] colors)
            : base()
        {
            Set_Palette(new Color[][] { colors }, true);
        }

        public override void Read(string fileIn)
        {
            Helper.BinaryReaderBE br = new Helper.BinaryReaderBE(fileIn);
            Color[] pal;

            ushort version = br.ReadUInt16();
            if (version != 0x00)
            {
                System.Windows.Forms.MessageBox.Show("Version not supported. Only 0");
                throw new FormatException("Version not supported. Only 0");
            }
            ushort num_colors = br.ReadUInt16();

            pal = new Color[num_colors];
            for (int i = 0; i < num_colors; i++)
            {
                ushort spec = br.ReadUInt16();
                if (spec != 0x00)
                {
                    System.Windows.Forms.MessageBox.Show("Color spec not supported. Only 0");
                    throw new FormatException("Color spec not supported. Only 0");
                }
                pal[i] = Color.FromArgb(br.ReadUInt16(), br.ReadUInt16(), br.ReadUInt16());
                br.ReadUInt16();    // Always 0x00
            }
            
            br.Close();
        }
        public override void Write(string fileOut)
        {
            // Only accept one palette
            Color[] pal = palette[0];

            Helper.BinaryWriterBE bw = new Helper.BinaryWriterBE(fileOut);

            bw.Write((ushort)0x00);         // Version 0
            bw.Write((ushort)pal.Length);   // Number of colors
            

            for (int i = 0; i < pal.Length; i++)
            {
                bw.Write((ushort)0x00);         // Color spec set to 0
                bw.Write((ushort)pal[i].R);     // Red component
                bw.Write((ushort)pal[i].G);     // Green component
                bw.Write((ushort)pal[i].B);     // Blue component
                bw.Write((ushort)0x00);         // Always 0x00, not used
            }

            bw.Flush();
            bw.Close();
        }
    }
}
