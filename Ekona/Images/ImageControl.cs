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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using Ekona.Images.Formats;

namespace Ekona.Images
{
    public partial class ImageControl : UserControl
    {
        IPluginHost pluginHost;
        ImageBase image;
        PaletteBase palette;
        bool isMap;
        MapBase map;

        bool stop;

        public ImageControl()
        {
            InitializeComponent();
        }
        public ImageControl(IPluginHost pluginHost, bool isMap)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.isMap = isMap;
            this.palette = pluginHost.Get_Palette();
            this.image = pluginHost.Get_Image();
            if (isMap)
                this.map = pluginHost.Get_Map();

            Update_Info();

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            Update_Image();
        }
        public ImageControl(IPluginHost pluginHost, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            isMap = false;
            this.image = image;
            this.palette = palette;
            this.pluginHost = pluginHost;

            Update_Info();

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            Update_Image();
        }
        public ImageControl(IPluginHost pluginHost, ImageBase image, PaletteBase palette, MapBase map)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.isMap = true;
            this.palette = palette;
            this.image = image;
            this.map = map;

            Update_Info();

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            Update_Image();
        }
        public ImageControl(XElement lang, ImageBase image, PaletteBase palette, MapBase map)
        {
            InitializeComponent();

            this.isMap = true;
            this.palette = palette;
            this.image = image;
            this.map = map;

            Update_Info();

            btnImport.Enabled = false;
            groupBox2.Enabled = false;
            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage(lang);
            Update_Image();
        }
        public ImageControl(XElement lang, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            isMap = false;
            this.image = image;
            this.palette = palette;

            Update_Info();

            btnImport.Enabled = false;
            groupBox2.Enabled = false;
            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage(lang);
            Update_Image();
        }

        private void ReadLanguage()
        {
            XElement xml = XElement.Load(pluginHost.Get_LangXML());
            xml = xml.Element("Ekona").Element("ImageControl");
            ReadLanguage(xml);
        }
        private void ReadLanguage(XElement xml)
        {
            try
            {
                label5.Text = xml.Element("S01").Value;
                groupProp.Text = xml.Element("S02").Value;
                label3.Text = xml.Element("S03").Value;
                checkHex.Text = xml.Element("S04").Value;
                label1.Text = xml.Element("S05").Value;
                label2.Text = xml.Element("S06").Value;
                label6.Text = xml.Element("S07").Value;
                label10.Text = xml.Element("S08").Value;
                label9.Text = xml.Element("S09").Value;
                checkTransparency.Text = xml.Element("S0A").Value;
                btnSetTrans.Text = xml.Element("S0B").Value;
                btnBgd.Text = xml.Element("S0C").Value;
                btnBgdRem.Text = xml.Element("S0D").Value;
                radioOriginalPal.Text = xml.Element("S0E").Value;
                checkMapCmp.Text = xml.Element("S0F").Value;
                btnExport.Text = xml.Element("S10").Value;
                btnImport.Text = xml.Element("S11").Value;
                comboBox1.Items[0] = xml.Element("S12").Value;
                comboBox1.Items[1] = xml.Element("S13").Value;
                groupBox2.Text = xml.Element("S16").Value;
                radioSwapPal.Text = xml.Element("S17").Value;
                radioReplacePal.Text = xml.Element("S18").Value;
                label7.Text = xml.Element("S19").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void Update_Info()
        {
            stop = true;

            switch (image.FormatColor)
            {
                case ColorFormat.A3I5: comboDepth.SelectedIndex = 4; break;
                case ColorFormat.A5I3: comboDepth.SelectedIndex = 5; break;
                case ColorFormat.colors4: comboDepth.SelectedIndex = 6; break;
                case ColorFormat.colors16: comboDepth.SelectedIndex = 0; break;
                case ColorFormat.colors256: comboDepth.SelectedIndex = 1; break;
                case ColorFormat.direct: comboDepth.SelectedIndex = 3; break;
                case ColorFormat.colors2: comboDepth.SelectedIndex = 2; break;
            }

            if (isMap)
            {
                btnImport.Enabled = (map.CanEdit && image.CanEdit && palette.CanEdit ? true : false);
                this.comboBox1.Enabled = false;

                this.numericWidth.Value = (map.Width != 0 ? map.Width : image.Width);
                this.numericHeight.Value = (map.Height != 0 ? map.Height : image.Height);

                comboBox1.SelectedIndex = 1;
                numericHeight.Minimum = image.TileSize;
                numericWidth.Minimum = image.TileSize;
                numericWidth.Increment = image.TileSize;
                numericHeight.Increment = image.TileSize;
            }
            else
            {
                btnImport.Enabled = (image.CanEdit && palette.CanEdit ? true : false);

                this.numericWidth.Value = image.Width;
                this.numericHeight.Value = image.Height;

                switch (image.FormTile)
                {
                    case TileForm.Lineal:
                        comboBox1.SelectedIndex = 0;
                        numericHeight.Minimum = 1;
                        numericWidth.Minimum = 1;
                        numericWidth.Increment = 1;
                        numericHeight.Increment = 1;
                        break;
                    case TileForm.Horizontal:
                        comboBox1.SelectedIndex = 1;
                        numericHeight.Minimum = image.TileSize;
                        numericWidth.Minimum = image.TileSize;
                        numericWidth.Increment = image.TileSize;
                        numericHeight.Increment = image.TileSize;
                        break;
                }
            }

            this.numTileSize.Value = image.TileSize;
            this.numPal.Maximum = palette.NumberOfPalettes - 1;
            this.numericStart.Maximum = image.Original.Length - 1;
            this.checkMapCmp.Enabled = isMap;

            stop = false;
        }
        private void Update_Image()
        {
            Bitmap bitmap;

            if (!isMap)
                bitmap = (Bitmap)image.Get_Image(palette);
            else
                bitmap = (Bitmap)map.Get_Image(image, palette);

            if (checkTransparency.Checked)
                bitmap.MakeTransparent(palette.Palette[(int)numPal.Value][0]);

            Clipboard.SetImage(bitmap);
            pic.Image = bitmap;

            if (bitmap.Width == 512)
                pic.BorderStyle = System.Windows.Forms.BorderStyle.None;
            else
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        private void numericStart_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            if (!isMap)
                image.StartByte = (int)numericStart.Value;
            else
                map.StartByte = (int)numericStart.Value;

            Update_Image();
        }
        private void numTileSize_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            image.TileSize = (int)numTileSize.Value;
            Update_Image();
        }
        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            if (!isMap)
            {
                image.Height = (int)numericHeight.Value;
                image.Width = (int)numericWidth.Value;
            }
            else
            {
                map.Width = (int)numericWidth.Value;
                map.Height = (int)numericHeight.Value;
            }

            Update_Image();
        }
        private void comboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            switch (comboDepth.SelectedIndex)
            {
                case 0: image.FormatColor = ColorFormat.colors16; break;
                case 1: image.FormatColor = ColorFormat.colors256; break;
                case 2: image.FormatColor = ColorFormat.colors2; break;
                case 3: image.FormatColor = ColorFormat.direct; break;
                case 4: image.FormatColor = ColorFormat.A3I5; break;
                case 5: image.FormatColor = ColorFormat.A5I3; break;
                case 6: image.FormatColor = ColorFormat.colors4; break;
                case 7: image.FormatColor = ColorFormat.A4I4; break;
                case 8: image.FormatColor = ColorFormat.BGRA32; break;
                case 9: image.FormatColor = ColorFormat.ABGR32; break;
            }

            Update_Image();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    image.FormTile = TileForm.Lineal;
                    numericHeight.Minimum = 1;
                    numericWidth.Minimum = 1;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case 1:
                    image.FormTile = TileForm.Horizontal;
                    numericHeight.Minimum = image.TileSize;
                    numericWidth.Minimum = image.TileSize;
                    numericWidth.Increment = image.TileSize;
                    numericHeight.Increment = image.TileSize;
                    break;
            }

            Update_Image();
        }
        private void numPal_ValueChanged(object sender, EventArgs e)
        {
            for (int j = 0; j < image.TilesPalette.Length; j++)
                image.TilesPalette[j] = (byte)numPal.Value;

            Update_Image();
        }
        private void checkTransparency_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTransparency.Checked)
            {
                Bitmap imageT = (Bitmap)pic.Image;
                imageT.MakeTransparent(palette.Palette[(int)numPal.Value][0]);
                pic.Image = imageT;
            }
            else
                Update_Image();
        }
        private void checkHex_CheckedChanged(object sender, EventArgs e)
        {
            numericStart.Hexadecimal = checkHex.Checked;
        }
        private void btnSetTrans_Click(object sender, EventArgs e)
        {
            int pal_index = (int)numPal.Value;

            Color[] pal = palette.Palette[pal_index];
            byte[] tiles = image.Tiles;
            int index = -1;

            if ((palette.Depth == ColorFormat.colors256 && palette.NumberOfColors == 256) ||
                (palette.Depth == ColorFormat.colors16 && palette.NumberOfColors == 16))
            {
                index = Actions.Remove_DuplicatedColors(ref pal, ref tiles);
                if (index == -1)
                {
                    index = Actions.Remove_NotUsedColors(ref pal, ref tiles);

                }
            }
            else
            {
                index = palette.NumberOfColors; // First empty place
                Color[] newPal = new Color[pal.Length + 1];
                Array.Copy(pal, newPal, pal.Length);
                pal = newPal;
            }

            if (index == -1)
            {
                MessageBox.Show("No space in the palette found");
                return;
            }

            pal[index] = Color.FromArgb(248, 0, 248);   // Usually used as transparent color
            Actions.Swap_Color(ref tiles, ref pal, index, 0, image.FormatColor);

            Color[][] new_pal = palette.Palette;
            new_pal[pal_index] = pal;

            if (image.ID > 0)
                image.Set_Tiles(tiles);
            if (palette.ID > 0)
                palette.Set_Palette(new_pal);

            Save_Files();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".png";
            o.Filter = "Portable Network Graphic (*.png)|*.png|" +
                        "BitMaP (*.bmp)|*.bmp|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.OverwritePrompt = true;
            if (isMap)
                o.FileName = map.FileName + ".png";
            else
                o.FileName = image.FileName + ".png";

            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FilterIndex == 2)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 1)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 3)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
            o.Dispose();
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.Filter = "Supported images |*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.ico;*.icon|" +
                       "BitMaP (*.bmp)|*.bmp|" +
                       "Portable Network Graphic (*.png)|*.png|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.Multiselect = false;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            Bitmap bitmap = (Bitmap)Image.FromFile(o.FileName);

            // Get tiles + palette from the current image
            byte[] tiles = new byte[0];
            Color[] pal = new Color[0];

            if (radioOriginalPal.Checked)
            {
                BMP bmp = new BMP(o.FileName);
                tiles = bmp.Tiles;
                pal = bmp.Palette.Palette[0];
            }
            else
            {
                try { Actions.Indexed_Image(bitmap, image.FormatColor, out tiles, out pal); }
                catch (Exception ex) { MessageBox.Show(ex.Message); Console.WriteLine(ex.Message); return; }
            }

            // Swap palettes if "Swap palette" is checked. Try to change the colors to the old palette
            if (radioSwapPal.Checked)
            {
                try { Actions.Swap_Palette(ref tiles, palette.Palette[(int)numPal.Value], pal, image.FormatColor); }
                catch (Exception ex) { MessageBox.Show(ex.Message); Console.WriteLine(ex.Message); return; }
            }

            // If the tile form is horizontal convert to it
            if (image.FormTile == TileForm.Horizontal || isMap)
            {
                tiles = Actions.HorizontalToLineal(tiles, bitmap.Width, bitmap.Height, image.BPP, 8);
                image.FormTile = TileForm.Horizontal;
            }

            // Create a map file // MetLob edition 19/05/2015
            if (isMap && checkMapCmp.Checked)
                map.Set_Map(Actions.Create_Map(ref tiles, image.BPP, image.TileSize, (byte)numPal.Value), map.CanEdit, bitmap.Width, bitmap.Height);
            else if (isMap)
            {
                int num_tiles = (tiles.Length * 8 / image.BPP) / (image.TileSize * image.TileSize);
                map.Set_Map(Actions.Create_BasicMap(num_tiles, 0, (byte)numPal.Value), map.CanEdit);
            }

            // Set the data
            image.Set_Tiles(tiles, bitmap.Width, bitmap.Height, image.FormatColor, image.FormTile, image.CanEdit, 8);

            if (radioReplacePal.Checked)
                palette.Set_Palette(pal, (int)numPal.Value);

            Save_Files();
            Update_Image();
            Update_Info();
        }
        private void Save_Files()
        {
            if (image.ID >= 0)
            {
                try
                {
                    string imageFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + image.FileName;
                    image.Write(imageFile, palette);
                    pluginHost.ChangeFile(image.ID, imageFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new image:\n" + e.Message); };
            }
            if (palette.ID >= 0 && radioReplacePal.Checked)
            {
                try
                {
                    string paletteFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + palette.FileName;
                    palette.Write(paletteFile);
                    pluginHost.ChangeFile(palette.ID, paletteFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new palette:\n" + e.Message); };
            }
            if (isMap && map.ID >= 0)
            {
                try
                {
                    string mapFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + map.FileName;
                    map.Write(mapFile, image, palette);
                    pluginHost.ChangeFile(map.ID, mapFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new map:\n" + e.Message); };
            }
        }

        private void pic_DoubleClick(object sender, EventArgs e)
        {
            XElement xml = XElement.Load(pluginHost.Get_LangXML());
            xml = xml.Element("Ekona").Element("ImageControl");

            Form ven = new Form();

            PictureBox pcBox = new PictureBox();
            pcBox.Image = pic.Image;
            pcBox.SizeMode = PictureBoxSizeMode.AutoSize;

            ven.Controls.Add(pcBox);
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.Text = xml.Element("S15").Value;
            ven.AutoScroll = true;
            ven.MaximumSize = new Size(1024, 700);
            ven.ShowIcon = false;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.MaximizeBox = false;
            ven.Show();
        }
        private void btnBgd_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;

            if (o.ShowDialog() == DialogResult.OK)
            {
                pic.BackColor = o.Color;
                btnBgdRem.Enabled = true;
            }
        }
        private void btnBgdTrans_Click(object sender, EventArgs e)
        {
            btnBgdRem.Enabled = false;
            pic.BackColor = Color.Transparent;
        }
    }
}
