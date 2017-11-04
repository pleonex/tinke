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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fonts
{
    public partial class CharControl : UserControl
    {
        Byte[] tiles;
        Color[] palette;
        int depth;

        int charCode;
        sNFTR.HDWC.Info tileInfo;
        int width;
        int height;
        int rotateMode;

        bool mouseDown;

        public CharControl()
        {
            InitializeComponent();
        }
        public CharControl(String lang,
            int charCode, sNFTR.HDWC.Info tileInfo, Byte[] tiles, int depth, int width, int height, int rotateMode,
            Color[] palette)
        {
            InitializeComponent();
            ReadLanguage(lang);

            this.charCode = charCode;
            this.tileInfo = tileInfo;
            this.tiles = tiles;
            this.depth = depth;
            this.width = width;
            this.height = height;
            this.rotateMode = rotateMode;
            this.palette = palette;

            txtCharCode.Text = String.Format("0x{0:X}", charCode);
            numericStart.Value = tileInfo.pixel_start;
            numericLength.Value = tileInfo.pixel_length;
            numericWidth.Value = tileInfo.pixel_width;

            Draw_Char();

            picPaletteColour.BackColor = palette[0];
            trackPalette.Maximum = Convert.ToByte(new String('1', depth), 2);
            trackPalette.Value = trackPalette.Maximum;
            picPaletteColour.BackColor = palette[trackPalette.Value];
        }
        private void ReadLanguage(string lang)
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "FontLang.xml");
                xml = xml.Element(lang).Element("CharControl");

                label1.Text = xml.Element("S00").Value;
                label3.Text = xml.Element("S01").Value;
                label2.Text = xml.Element("S02").Value;
                label4.Text = xml.Element("S03").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }
        public void Draw_Char()
        {
            Bitmap image = NFTR.Get_Char(tiles, depth, width, height, rotateMode, palette, 10);
            // Draw the grid
            Graphics graphic = Graphics.FromImage(image);
            for (int h = 0; h < height * 10; h += 10)
                graphic.DrawLine(Pens.Red, 0, h, width * 10, h);
            for (int w = 0; w < width * 10; w += 10)
                graphic.DrawLine(Pens.Red, w, 0, w, height * 10);

            picFont.Image = image;
        }

        public int CharCode
        {
            get { return charCode; }
        }
        public int CharWidth
        {
            get { return width; }
        }
        public int CharHeight
        {
            get { return height; }
        }
        public int RotateMode
        {
            get { return rotateMode; }
        }
        public int Depth
        {
            get { return depth; }
        }
        public sNFTR.HDWC.Info TileInfo
        {
            get { return tileInfo; }
        }
        public Byte[] Tiles
        {
            get { return tiles; }
        }

        private void trackPalette_Scroll(object sender, EventArgs e)
        {
            picPaletteColour.BackColor = palette[trackPalette.Value];
        }
        private void numericWidth_ValueChanged(object sender, EventArgs e)
        {
            tileInfo.pixel_width = (byte)numericWidth.Value;
        }
        private void numericLength_ValueChanged(object sender, EventArgs e)
        {
            tileInfo.pixel_length = (byte)numericLength.Value;
        }
        private void numericStart_ValueChanged(object sender, EventArgs e)
        {
            tileInfo.pixel_start = (sbyte)numericStart.Value;
        }

        private void picFont_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;

            Bitmap image = (Bitmap)picFont.Image;
            Point location_pixel = new Point(e.X / 10, e.Y / 10);
            for (int d = 0; d < depth; d++)
                tiles[(location_pixel.X + location_pixel.Y * width) * depth + d] = (byte)((trackPalette.Value >> d) & 1);

            Draw_Char();
        }
        private void picFont_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown || e.X + 3 >= picFont.Width || e.Y + 3 >= picFont.Height || e.X < 0 || e.Y < 0)
                return;
            

            Bitmap image = (Bitmap)picFont.Image;
            Point location_pixel = new Point(e.X / 10, e.Y / 10);
            for (int d = depth - 1, i = 0; d >= 0; d--, i++)
                tiles[(location_pixel.X + location_pixel.Y * width) * depth + i] = (byte)((trackPalette.Value >> d) & 1);

            Draw_Char();

        }
        private void picFont_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
