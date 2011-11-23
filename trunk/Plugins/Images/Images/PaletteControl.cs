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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using PluginInterface;

namespace Images
{
    public partial class PaletteControl : UserControl
    {
        IPluginHost pluginHost;
        PaletteBase palette;

        public PaletteControl()
        {
            InitializeComponent();
        }
        public PaletteControl(IPluginHost pluginHost, PaletteBase palette)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.palette = palette;
            btnImport.Enabled = palette.CanEdit;

            ReadLanguage();

            picPalette.Image = palette.Get_PaletteImage(0);

            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = "of " + (palette.NumberOfPalettes - 1).ToString();
            numericStartByte.Maximum = palette.NumberOfColors * palette.NumberOfPalettes * 2;
            comboDepth.SelectedIndex = (palette.Depth == ColorDepth.Depth4Bit ? 0 : 1);
        }

        private void ReadLanguage()
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "ImagesLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("PaletteControl");
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void numericPalette_ValueChanged(object sender, EventArgs e)
        {
            picPalette.Image = palette.Get_PaletteImage((int)numericPalette.Value);
        }
        private void numericStartByte_ValueChanged(object sender, EventArgs e)
        {
            palette.StartByte = (int)numericStartByte.Value;
            picPalette.Image = palette.Get_PaletteImage((int)numericPalette.Value);
            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = "of " + (palette.NumberOfPalettes - 1).ToString();
        }
        private void comboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            palette.Depth = (comboDepth.SelectedIndex == 0 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            picPalette.Image = palette.Get_PaletteImage((int)numericPalette.Value);
            numericPalette.Value = 0;
            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = "of " + (palette.NumberOfPalettes - 1).ToString();
        }

        private void picPalette_MouseClick(object sender, MouseEventArgs e)
        {
            if (picPalette.Image is Image)
            {
                Color color = ((Bitmap)picPalette.Image).GetPixel(e.X, e.Y);
                lblRGB.Text = "RGB: " + color.R + ", " + color.G + ", " + color.B;
            }

        }
        private void btnShow_Click(object sender, EventArgs e)
        {
            Form ven = new Form();
            int xMax = 6 * 170;
            int x = 0;
            int y = 15;

            for (int i = 0; i < palette.NumberOfPalettes; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(160, 160);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = palette.Get_PaletteImage(i);
                Label lbl = new Label();
                lbl.Text = "Palette " + (i + 1).ToString();
                lbl.Location = new Point(x, y - 15);

                ven.Controls.Add(pic);
                ven.Controls.Add(lbl);

                x += 170;
                if (x >= xMax)
                {
                    x = 0;
                    y += 185;
                }
            }

            ven.Text = "Palette viewer";
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.MaximumSize = new Size(1024, 760);
            ven.ShowIcon = false;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.MaximizeBox = false;
            ven.Show();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Portable Network Graphics (*.png)|*.png|" +
                "Windows Palette (*.pal)|*.pal";
            o.OverwritePrompt = true;

            if (o.ShowDialog() != DialogResult.OK)
                return;

            if (o.FilterIndex == 1)
                picPalette.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
            else if (o.FilterIndex == 2)
                pluginHost.Write_WinPal(o.FileName, palette.Palette);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            //String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + palette.FileName;
            //palette.WritePalette(fileOut);
            //pluginHost.ChangeFile(palette.ID, fileOut);
        }

    }
}
