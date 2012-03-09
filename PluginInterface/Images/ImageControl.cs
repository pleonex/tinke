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
//using FotochohForTinke;

namespace PluginInterface.Images
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
            stop = true;

            this.pluginHost = pluginHost;
            this.isMap = isMap;
            this.palette = pluginHost.Get_Palette();
            this.image = pluginHost.Get_Image();
            if (isMap)
            {
                this.map = pluginHost.Get_Map();
                btnImport.Enabled = map.CanEdit;
                pic.Image = map.Get_Image(image, palette);
                this.comboBox1.Enabled = false;
            }
            else
            {
                btnImport.Enabled = image.CanEdit;
                pic.Image = image.Get_Image(palette);
            }


            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;

            switch (image.ColorFormat)
            {
                case ColorFormat.A3I5:
                    comboDepth.SelectedIndex = 4;
                    break;
                case ColorFormat.A5I3:
                    comboDepth.SelectedIndex = 5;
                    break;
                case ColorFormat.colors4:
                    comboDepth.SelectedIndex = 6;
                    break;
                case ColorFormat.colors16:
                    comboDepth.SelectedIndex = 0;
                    break;
                case ColorFormat.colors256:
                    comboDepth.SelectedIndex = 1;
                    break;
                case ColorFormat.direct:
                    comboDepth.SelectedIndex = 3;
                    break;
                case ColorFormat.colors2:
                    comboDepth.SelectedIndex = 2;
                    break;
            }

            this.comboBox1.SelectedIndex = 1;
            this.numPal.Maximum = palette.NumberOfPalettes;

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            stop = false;
        }
        public ImageControl(IPluginHost pluginHost, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            stop = true;
            isMap = false;
            this.image = image;
            this.palette = palette;
            this.pluginHost = pluginHost;
            btnImport.Enabled = image.CanEdit;

            pic.Image = image.Get_Image(palette);


            switch (image.ColorFormat)
            {
                case ColorFormat.A3I5:
                    comboDepth.SelectedIndex = 4;
                    break;
                case ColorFormat.A5I3:
                    comboDepth.SelectedIndex = 5;
                    break;
                case ColorFormat.colors4:
                    comboDepth.SelectedIndex = 6;
                    break;
                case ColorFormat.colors16:
                    comboDepth.SelectedIndex = 0;
                    break;
                case ColorFormat.colors256:
                    comboDepth.SelectedIndex = 1;
                    break;
                case ColorFormat.direct:
                    comboDepth.SelectedIndex = 3;
                    break;
                case ColorFormat.colors2:
                    comboDepth.SelectedIndex = 2;
                    break;
            }

            switch (image.TileForm)
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
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    break;
            }
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;
            this.numPal.Maximum = palette.NumberOfPalettes;

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            stop = false;
        }
        public ImageControl(IPluginHost pluginHost, ImageBase image, PaletteBase palette, MapBase map)
        {
            InitializeComponent();
            stop = true;

            this.pluginHost = pluginHost;
            this.isMap = true;
            this.palette = palette;
            this.image = image;
            this.map = map;
            btnImport.Enabled = map.CanEdit;

            pic.Image = map.Get_Image(image, palette);
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;

            switch (image.ColorFormat)
            {
                case ColorFormat.A3I5:
                    comboDepth.SelectedIndex = 4;
                    break;
                case ColorFormat.A5I3:
                    comboDepth.SelectedIndex = 5;
                    break;
                case ColorFormat.colors4:
                    comboDepth.SelectedIndex = 6;
                    break;
                case ColorFormat.colors16:
                    comboDepth.SelectedIndex = 0;
                    break;
                case ColorFormat.colors256:
                    comboDepth.SelectedIndex = 1;
                    break;
                case ColorFormat.direct:
                    comboDepth.SelectedIndex = 3;
                    break;
                case ColorFormat.colors2:
                    comboDepth.SelectedIndex = 2;
                    break;
            }

            this.comboBox1.SelectedIndex = 1;
            this.comboBox1.Enabled = false;
            this.numPal.Maximum = palette.NumberOfPalettes;

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            ReadLanguage();
            stop = false;
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
                case 0: image.ColorFormat = ColorFormat.colors16; break;
                case 1: image.ColorFormat = ColorFormat.colors256; break;
                case 2: image.ColorFormat = ColorFormat.colors2; break;
                case 3: image.ColorFormat = ColorFormat.direct; break;
                case 4: image.ColorFormat = ColorFormat.A3I5; break;
                case 5: image.ColorFormat = ColorFormat.A5I3; break;
                case 6: image.ColorFormat = ColorFormat.colors4; break;
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
                    image.TileForm = TileForm.Lineal;
                    numericHeight.Minimum = 1;
                    numericWidth.Minimum = 1;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case 1:
                    image.TileForm = TileForm.Horizontal;
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
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

        private void Update_Image()
        {
            Bitmap bitmap;

            if (!isMap)
                bitmap = (Bitmap)image.Get_Image(palette);
            else
                bitmap = (Bitmap)map.Get_Image(image, palette);

            if (checkTransparency.Checked)
                bitmap.MakeTransparent(palette.Palette[(int)numPal.Value][0]);

            pic.Image = bitmap;
        }

        private void ReadLanguage()
        {
            // TODO
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "ImagesLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("ImageControl");

                label5.Text = xml.Element("S01").Value;
                groupProp.Text = xml.Element("S02").Value;
                label3.Text = xml.Element("S11").Value;
                label1.Text = xml.Element("S12").Value;
                label2.Text = xml.Element("S13").Value;
                label6.Text = xml.Element("S14").Value;
                btnSave.Text = xml.Element("S15").Value;
                comboBox1.Items[0] = xml.Element("S16").Value;
                comboBox1.Items[1] = xml.Element("S17").Value;
                checkTransparency.Text = xml.Element("S1D").Value;
                lblZoom.Text = xml.Element("S1E").Value;
                btnBgd.Text = xml.Element("S1F").Value;
                btnBgdTrans.Text = xml.Element("S20").Value;
                btnImport.Text = xml.Element("S21").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //// TODO
            //OpenFileDialog o = new OpenFileDialog();
            //o.CheckFileExists = true;
            //o.DefaultExt = "bmp";
            //o.Filter = "BitMaP (*.bmp)|*.bmp";
            //o.Multiselect = false;
            //if (o.ShowDialog() == DialogResult.OK)
            //{
            //    // TODO: write new palette file

            //    NCGR newTile = pluginHost.BitmapToTile(o.FileName, (comboBox1.SelectedIndex == 0 ? TileOrder.NoTiled : TileOrder.Horizontal));
            //    String tileFile = System.IO.Path.GetTempFileName() + '.' + image.FileName;
            //    // TODO: set new tile data

            //    image.Write(tileFile, palette);
            //    pluginHost.ChangeFile(image.ID, tileFile);

            //    if (isMap)
            //    {
            //        //NSCR newMap;
            //        //if (image.TileForm == TileOrder.Horizontal)
            //        //    newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX * 8, tile.rahc.nTilesY * 8);
            //        //else
            //        //    newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX, tile.rahc.nTilesY);
            //        //newMap.id = map.id;
            //        //newMap.header.id = map.header.id;
            //        //String mapFile = System.IO.Path.GetTempFileName() + new String(map.header.id);
            //        //map = newMap;

            //        //pluginHost.Set_NSCR(map);
            //        //Write(mapFile, map, pluginHost);
            //        //pluginHost.ChangeFile((int)map.id, mapFile);
            //    }

            //    if (image.TileForm == TileForm.Lineal)
            //    {
            //        numericWidth.Value = image.Width;
            //        numericHeight.Value = image.Height;
            //        numericHeight.Minimum = 1;
            //        numericWidth.Minimum = 1;
            //        numericWidth.Increment = 1;
            //        numericHeight.Increment = 1;
            //        comboBox1.SelectedIndex = 0;
            //    }
            //    else
            //    {
            //        numericWidth.Value = image.Width * 8;
            //        numericHeight.Value = image.Height * 8;
            //        numericHeight.Minimum = 8;
            //        numericWidth.Minimum = 8;
            //        numericWidth.Increment = 8;
            //        numericHeight.Increment = 8;
            //        comboBox1.SelectedIndex = 1;
            //    }
            //}
        }
        //public static void Write_Tiles(string fileout, NCGR tiles, IPluginHost pluginHost)
        //{
        //    // Obsolet
        //    BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

        //    for (int i = 0; i < tiles.rahc.tileData.tiles.Length; i++)
        //        if (tiles.rahc.depth == ColorDepth.Depth4Bit)
        //            bw.Write(pluginHost.Bit4ToBit8(tiles.rahc.tileData.tiles[i]));
        //        else
        //            bw.Write(tiles.rahc.tileData.tiles[i]);

        //    bw.Flush();
        //    bw.Close();
        //}
        //public static void Write_Map(string fileout, NSCR map, IPluginHost pluginHost)
        //{
        //    // Obsolet
        //    BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

        //    for (int i = 0; i < map.section.mapData.Length; i++)
        //    {
        //        int npalette = map.section.mapData[i].nPalette << 12;
        //        int yFlip = map.section.mapData[i].yFlip << 11;
        //        int xFlip = map.section.mapData[i].xFlip << 10;
        //        int data = npalette + yFlip + xFlip + map.section.mapData[i].nTile;
        //        bw.Write((ushort)data);
        //    }

        //    bw.Flush();
        //    bw.Close();
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp|" +
                       "Portable Network Graphic (*.png)|*.png|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FilterIndex == 1)
                    pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 2)
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
        private void pic_DoubleClick(object sender, EventArgs e)
        {
            // TODO: language
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                Path.DirectorySeparatorChar + "ImagesLang.xml");
            xml = xml.Element(pluginHost.Get_Language()).Element("ImageControl");

            Form ven = new Form();
            PictureBox pcBox = new PictureBox();
            pcBox.Image = pic.Image;
            pcBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pcBox.BackColor = pictureBgd.BackColor;

            ven.Controls.Add(pcBox);
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.Text = xml.Element("S19").Value;
            ven.AutoScroll = true;
            ven.MaximumSize = new Size(1024, 700);
            ven.ShowIcon = false;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.MaximizeBox = false;
            ven.Show();
        }

        private void trackZoom_Scroll(object sender, EventArgs e)
        {
            image.Zoom = trackZoom.Value;
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
        private void btnBgd_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;

            if (o.ShowDialog() == DialogResult.OK)
            {
                pictureBgd.BackColor = o.Color;
                pic.BackColor = o.Color;
                btnBgdTrans.Enabled = true;
            }
        }
        private void btnBgdTrans_Click(object sender, EventArgs e)
        {
            btnBgdTrans.Enabled = false;

            pictureBgd.BackColor = Color.Transparent;
            pic.BackColor = Color.Transparent;
        }

        private void checkHex_CheckedChanged(object sender, EventArgs e)
        {
            numericStart.Hexadecimal = checkHex.Checked;
        }

        //private void btnFotochoh_Click(object sender, EventArgs e)
        //{
        //    byte[] tiles = image.Tiles;
        //    int width = image.Width;
        //    int height = image.Height;
        //    byte[] tile_pal;    // Not used

        //    if (isMap)
        //    {
        //        tiles = Actions.Apply_Map(map.Map, tiles, out tile_pal, image.TileWidth);
        //        if (map.Width != 0)
        //            width = map.Width;
        //        if (map.Height != 0)
        //            height = map.Height;
        //    }

        //    if (image.TileForm == TileForm.Horizontal)
        //        tiles = Actions.LinealToHorizontal(tiles, image.Width / 8, image.Height / 8, image.TileWidth);

        //    FotochohForTinke.FotochohForm fotochoh = new FotochohForm();
        //    fotochoh.SetBitmap(tiles, image.Width, palette.Palette[0], (PaletteType)image.ColorFormat);
        //    fotochoh.ShowDialog();
        //}
    }
}
