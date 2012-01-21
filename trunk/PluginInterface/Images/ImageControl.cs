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

namespace PluginInterface
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
        public ImageControl(IPluginHost pluginHost, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            stop = true;
            isMap = false;
            this.image = image;
            this.palette = palette;
            this.pluginHost = pluginHost;
            btnImport.Enabled = image.CanEdit;

            pic.Image = image.Get_Image(palette.Palette);

            if (image.Depth == ColorDepth.Depth32Bit)   // 1bpp
                comboDepth.Text = "1 bpp";
            else if (image.Depth == ColorDepth.Depth4Bit)
                comboDepth.Text = "4 bpp";
            else if (image.Depth == ColorDepth.Depth8Bit)
                comboDepth.Text = "8 bpp";

            switch (image.TileForm)
            {
                case TileOrder.NoTiled:
                    comboBox1.SelectedIndex = 0;
                    numericHeight.Minimum = 1;
                    numericWidth.Minimum = 1;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case TileOrder.Horizontal:
                    comboBox1.SelectedIndex = 1;
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    break;
            }
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;

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
         
            if (image.Depth == ColorDepth.Depth32Bit)   // 1bpp
                comboDepth.Text = "1 bpp";
            else if (image.Depth == ColorDepth.Depth4Bit)
                comboDepth.Text = "4 bpp";
            else if (image.Depth == ColorDepth.Depth8Bit)
                comboDepth.Text = "8 bpp";
 
            this.comboBox1.SelectedIndex = 1;
            this.comboBox1.Enabled = false;

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

            image.Height = (int)numericHeight.Value;
            image.Width = (int)numericWidth.Value;

            Update_Image();
        }
        private void comboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            if (comboDepth.Text == "1 bpp")
                image.Depth = ColorDepth.Depth32Bit;    // 1 bpp
            else if (comboDepth.Text == "4 bpp")
                image.Depth = ColorDepth.Depth4Bit;
            else if (comboDepth.Text == "8 bpp")
                image.Depth = ColorDepth.Depth8Bit;

            Update_Image();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    image.TileForm = TileOrder.NoTiled;
                    numericHeight.Minimum = 1;
                    numericWidth.Minimum = 1;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case 1:
                    image.TileForm = TileOrder.Horizontal;
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    break;
            }

            pic.Image = image.Get_Image(palette.Palette);
        }

        private void Update_Image()
        {
            if (!isMap)
                pic.Image = image.Get_Image(palette.Palette);
            else
                pic.Image = map.Get_Image(image, palette);
        }

        private void ReadLanguage()
        {
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
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.Multiselect = false;
            if (o.ShowDialog() == DialogResult.OK)
            {
                // TODO: write new palette file

                NCGR newTile = pluginHost.BitmapToTile(o.FileName, (comboBox1.SelectedIndex == 0 ? TileOrder.NoTiled : TileOrder.Horizontal));
                String tileFile = System.IO.Path.GetTempFileName() + '.' + image.FileName;
                // TODO: set new tile data

                image.Write_Tiles(tileFile);
                pluginHost.ChangeFile(image.ID, tileFile);

                if (isMap)
                {
                    //NSCR newMap;
                    //if (image.TileForm == TileOrder.Horizontal)
                    //    newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX * 8, tile.rahc.nTilesY * 8);
                    //else
                    //    newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX, tile.rahc.nTilesY);
                    //newMap.id = map.id;
                    //newMap.header.id = map.header.id;
                    //String mapFile = System.IO.Path.GetTempFileName() + new String(map.header.id);
                    //map = newMap;

                    //pluginHost.Set_NSCR(map);
                    //Write_Map(mapFile, map, pluginHost);
                    //pluginHost.ChangeFile((int)map.id, mapFile);
                }

                if (image.TileForm == TileOrder.NoTiled)
                {
                    numericWidth.Value = image.Width;
                    numericHeight.Value = image.Height;
                    numericHeight.Minimum = 1;
                    numericWidth.Minimum = 1;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    numericWidth.Value = image.Width * 8;
                    numericHeight.Value = image.Height * 8;
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    comboBox1.SelectedIndex = 1;
                }
            }
        }
        public static void Write_Tiles(string fileout, NCGR tiles, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            for (int i = 0; i < tiles.rahc.tileData.tiles.Length; i++)
                if (tiles.rahc.depth == ColorDepth.Depth4Bit)
                    bw.Write(pluginHost.Bit4ToBit8(tiles.rahc.tileData.tiles[i]));
                else
                    bw.Write(tiles.rahc.tileData.tiles[i]);

            bw.Flush();
            bw.Close();
        }
        public static void Write_Map(string fileout, NSCR map, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            for (int i = 0; i < map.section.mapData.Length; i++)
            {
                int npalette = map.section.mapData[i].nPalette << 12;
                int yFlip = map.section.mapData[i].yFlip << 11;
                int xFlip = map.section.mapData[i].xFlip << 10;
                int data = npalette + yFlip + xFlip + map.section.mapData[i].nTile;
                bw.Write((ushort)data);
            }

            bw.Flush();
            bw.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            o.Dispose();
        }
        private void pic_DoubleClick(object sender, EventArgs e)
        {
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
                imageT.MakeTransparent(palette.Palette[image.TilesPalette[0]][0]);
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
    }
}
