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

namespace Ekona.Images
{
    public partial class PaletteControl : UserControl
    {
        IPluginHost pluginHost;
        PaletteBase palette;
        string[] translation;

        public PaletteControl()
        {
            InitializeComponent();
        }
        public PaletteControl(IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.palette = pluginHost.Get_Palette();
            btnImport.Enabled = palette.CanEdit;

            ReadLanguage();
            Update_Info();
        }
        public PaletteControl(IPluginHost pluginHost, PaletteBase palette)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.palette = palette;
            btnImport.Enabled = palette.CanEdit;

            ReadLanguage();
            Update_Info();
        }

        private void Update_Info()
        {
            picPalette.Image = palette.Get_Image(0);

            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = translation[0] + (palette.NumberOfPalettes - 1).ToString();
            numericStartByte.Maximum = palette.Original.Length - 1;
            comboDepth.SelectedIndex = (palette.Depth == ColorFormat.colors16 ? 0 : 1);


            if (palette.Depth == ColorFormat.colors16)
                numFillColors.Value = 16;
            else
                numFillColors.Value = 256;

            checkDuplicated.Checked = palette.Has_DuplicatedColors(0);
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = XElement.Load(pluginHost.Get_LangXML());
                xml = xml.Element("Ekona");
                xml = xml.Element("PaletteControl");

                label1.Text = xml.Element("S01").Value;
                btnShow.Text = xml.Element("S02").Value;
                btnExport.Text = xml.Element("S03").Value;
                btnImport.Text = xml.Element("S04").Value;
                label2.Text = xml.Element("S05").Value;
                label4.Text = xml.Element("S06").Value;
                btnUseThis.Text = xml.Element("S0A").Value;
                checkHex.Text = xml.Element("S0B").Value;
                label5.Text = xml.Element("S0C").Value;
                btnFillColors.Text = xml.Element("S0D").Value;

                translation = new string[3];
                translation[0] = xml.Element("S07").Value;
                translation[1] = xml.Element("S09").Value;
                translation[2] = xml.Element("S08").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void numericPalette_ValueChanged(object sender, EventArgs e)
        {
            picPalette.Image = palette.Get_Image((int)numericPalette.Value);
            checkDuplicated.Checked = palette.Has_DuplicatedColors((int)numericPalette.Value);
        }
        private void numericStartByte_ValueChanged(object sender, EventArgs e)
        {
            palette.StartByte = (int)numericStartByte.Value;
            picPalette.Image = palette.Get_Image((int)numericPalette.Value);
            
            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = translation[0] + (palette.NumberOfPalettes - 1).ToString();
            checkDuplicated.Checked = palette.Has_DuplicatedColors((int)numericPalette.Value);
        }
        private void comboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            palette.Depth = (comboDepth.SelectedIndex == 0 ? ColorFormat.colors16 : ColorFormat.colors256);
            picPalette.Image = palette.Get_Image((int)numericPalette.Value);

            numericPalette.Value = 0;
            numericPalette.Maximum = palette.NumberOfPalettes - 1;
            label3.Text = translation[0] + (palette.NumberOfPalettes - 1).ToString();

            
            if (palette.Depth == ColorFormat.colors16)
                numFillColors.Value = 16;
            else
                numFillColors.Value = 256;
            checkDuplicated.Checked = palette.Has_DuplicatedColors((int)numericPalette.Value);
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
            Form win = new Form();
            int xMax = 6 * 170;
            int x = 0;
            int y = 15;

            for (int i = 0; i < palette.NumberOfPalettes; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(160, 160);
                pic.Location = new Point(x, y);
                pic.BorderStyle = BorderStyle.FixedSingle;
                pic.Image = palette.Get_Image(i);
                Label lbl = new Label();
                lbl.Text = translation[2] + (i + 1).ToString();
                lbl.Location = new Point(x, y - 15);

                win.Controls.Add(pic);
                win.Controls.Add(lbl);

                x += 170;
                if (x >= xMax)
                {
                    x = 0;
                    y += 185;
                }
            }

            win.Text = translation[1];
            win.BackColor = SystemColors.GradientInactiveCaption;
            win.MaximumSize = new Size(1024, 760);
            win.ShowIcon = false;
            win.AutoSize = true;
            win.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            win.MaximizeBox = false;
            win.Show();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".pal";
            o.Filter = "Windows Palette for Gimp 2.8 (*.pal)|*.pal|" +
                        "Windows Palette (*.pal)|*.pal|" +
                        "Portable Network Graphics (*.png)|*.png|" +
                        "Adobe COlor (*.aco)|*.aco";
            o.OverwritePrompt = true;
            o.FileName = palette.FileName;

            if (o.ShowDialog() != DialogResult.OK)
                return;

            if (o.FilterIndex == 3)
                picPalette.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
            else if (o.FilterIndex == 1 || o.FilterIndex == 2)
            {
                Formats.PaletteWin palwin = new Formats.PaletteWin(palette.Palette[(int)numericPalette.Value]);
                if (o.FilterIndex == 1) palwin.Gimp_Error = true;
                palwin.Write(o.FileName);
            }
            else if (o.FilterIndex == 4)
            {
                Formats.ACO palaco = new Formats.ACO(palette.Palette[(int)numericPalette.Value]);
                palaco.Write(o.FileName);
            }

            o.Dispose();
            o = null;
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
			OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.Filter = "All supported formats|*.pal;*.aco;*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.ico;*.icon|" +
                "Windows Palette (*.pal)|*.pal|" +
                "Adobe COlor (*.aco)|*.aco|" +
                "Palette from image|*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.ico;*.icon";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            string ext = Path.GetExtension(o.FileName).ToLower();
			if (string.IsNullOrEmpty(ext) || ext.Length == 0) {
				MessageBox.Show("File without extension... Aborting");
				return;
			}

			if (ext.Contains("."))
				ext = ext.Substring(ext.LastIndexOf(".") + 1);
			Console.WriteLine("File extension:" + ext);
            PaletteBase newpal;

            if (ext == "pal")
                newpal = new Formats.PaletteWin(o.FileName);
            else if (ext == "aco")
                newpal = new Formats.ACO(o.FileName);
            else
            {
                byte[] tiles;
                Color[] newcol;
                Actions.Indexed_Image((Bitmap)Image.FromFile(o.FileName), palette.Depth, out tiles, out newcol);
                newpal = new RawPalette(newcol, palette.CanEdit, palette.Depth);
            }
            
            if (newpal != null)
                palette.Set_Palette(newpal);

            // Write the file
            Write_File();

            o.Dispose();
            o = null;
        }
        private void Write_File()
        {
            if (palette.ID > 0)
            {
                try
                {
                    String fileOut = pluginHost.Get_TempFile();
                    palette.Write(fileOut);
                    pluginHost.ChangeFile(palette.ID, fileOut);
                }
                catch (Exception ex) { MessageBox.Show("Error writing new palette:\n" + ex.Message); };
            }
        }

        private void checkHex_CheckedChanged(object sender, EventArgs e)
        {
            numericStartByte.Hexadecimal = checkHex.Checked;
        }

        private void btnUseThis_Click(object sender, EventArgs e)
        {
            pluginHost.Set_Palette(palette);
        }

        private void btnFillColors_Click(object sender, EventArgs e)
        {
            palette.FillColors((int)numFillColors.Value, (int)numericPalette.Value);
            Write_File();
            picPalette.Image = palette.Get_Image((int)numericPalette.Value);
            checkDuplicated.Checked = palette.Has_DuplicatedColors((int)numericPalette.Value);
        }
    }
}
