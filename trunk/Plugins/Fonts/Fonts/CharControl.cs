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

        public CharControl()
        {
            InitializeComponent();
        }
        public CharControl(String lang, 
            int charCode, sNFTR.HDWC.Info tileInfo, Byte[] tiles, int depth, int width, int height, int rotateMode)
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

            txtCharCode.Text = String.Format("0x{0:X}", charCode);
            numericStart.Value = tileInfo.pixel_start;
            numericLength.Value = tileInfo.pixel_length;
            numericWidth.Value = tileInfo.pixel_width;

            Draw_Char();

            palette = new Color[(int)Math.Pow(2, depth)];
            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = 255 - (i * (255 / (palette.Length - 1)));
                palette[i] = Color.FromArgb(colorIndex, 0, 0, 0);
            }
            palette = palette.Reverse().ToArray();

            picPaletteColour.BackColor = palette[0];
            trackPalette.Maximum = palette.Length - 1;
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
            Bitmap image = NFTR.Get_Char(tiles, depth, width, height, rotateMode, 10);
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

        private void picFont_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap image = (Bitmap)picFont.Image;
            Point location_pixel = new Point(e.X / 10, e.Y / 10);
            for (int d = 0; d < depth; d++)
                tiles[(location_pixel.X + location_pixel.Y * width) * depth + d] = (byte)((trackPalette.Value >> d) & 1);

            Draw_Char();
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
            tileInfo.pixel_start = (byte)numericStart.Value;
        }
    }
}
