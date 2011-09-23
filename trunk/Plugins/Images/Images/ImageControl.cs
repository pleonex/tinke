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
using System.IO;
using System.Xml.Linq;
using PluginInterface;

namespace Images
{
    public partial class ImageControl : UserControl
    {
        NCLR paleta;
        NCGR tile;
        NSCR map;
        IPluginHost pluginHost;
        int startTile;

        string oldDepth;
        int oldTiles;
        bool stopUpdating;
        bool isMap;

        public ImageControl()
        {
            InitializeComponent();
        }
        public ImageControl(IPluginHost pluginHost, bool isMap)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            ReadLanguage();

            stopUpdating = true;
            this.isMap = isMap;
            this.paleta = pluginHost.Get_NCLR();
            this.tile = pluginHost.Get_NCGR();

            if (isMap)
            {
                this.map = pluginHost.Get_NSCR();
                NCGR newTile = tile;
                newTile.rahc.tileData = pluginHost.Transformar_NSCR(map, tile.rahc.tileData);
                newTile.rahc.nTilesX = (ushort)(map.section.width / 8);
                newTile.rahc.nTilesY = (ushort)(map.section.height / 8);
                pic.Image = pluginHost.Bitmap_NCGR(newTile, paleta, 0);
            }
            else
                pic.Image = pluginHost.Bitmap_NCGR(tile, paleta, 0);
            
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;
            oldDepth = comboDepth.Text;
            this.comboDepth.Text = (tile.rahc.depth == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp");
            switch (tile.orden)
            {
                case Orden_Tiles.No_Tiles:
                    oldTiles = 0;
                    comboBox1.SelectedIndex = 0;
                    break;
                case Orden_Tiles.Horizontal:
                    oldTiles = 1;
                    comboBox1.SelectedIndex = 1;
                    break;
            }

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            Info();
            stopUpdating = false;
        }

        private void numericStart_ValueChanged(object sender, EventArgs e)
        {
            startTile = (int)numericStart.Value;
            UpdateImage();
        }
        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }
        private void comboDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboDepth.Text == oldDepth || stopUpdating)
                return;

            oldDepth = comboDepth.Text;
            tile.rahc.depth = (comboDepth.Text == "4 bpp" ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);

            if (comboDepth.Text == "4 bpp")
            {
                byte[] temp = pluginHost.Bit8ToBit4(pluginHost.TilesToBytes(tile.rahc.tileData.tiles));
                if (tile.orden != Orden_Tiles.No_Tiles)
                {
                    tile.rahc.tileData.tiles = pluginHost.BytesToTiles(temp);
                    tile.rahc.tileData.nPaleta = new byte[tile.rahc.tileData.tiles.Length];
                }
                else
                {
                    tile.rahc.tileData.tiles = new Byte[1][];
                    tile.rahc.tileData.tiles[0] = temp;
                    tile.rahc.tileData.nPaleta = new byte[tile.rahc.tileData.tiles[0].Length];
                }
            }
            else
            {
                byte[] temp = pluginHost.Bit4ToBit8(pluginHost.TilesToBytes(tile.rahc.tileData.tiles));
                if (tile.orden != Orden_Tiles.No_Tiles)
                {
                    tile.rahc.tileData.tiles = pluginHost.BytesToTiles(temp);
                    tile.rahc.tileData.nPaleta = new byte[tile.rahc.tileData.tiles.Length];
                }
                else
                {
                    tile.rahc.tileData.tiles = new Byte[1][];
                    tile.rahc.tileData.tiles[0] = temp;
                    tile.rahc.tileData.nPaleta = new byte[tile.rahc.tileData.tiles[0].Length];
                }
            }
            pluginHost.Set_NCGR(tile);
            UpdateImage();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (oldTiles == comboBox1.SelectedIndex)
                return;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    tile.orden = Orden_Tiles.No_Tiles;
                    tile.rahc.tileData.tiles[0] = pluginHost.TilesToBytes(tile.rahc.tileData.tiles);
                    numericHeight.Minimum = 0;
                    numericWidth.Minimum = 0;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case 1:
                    tile.orden = Orden_Tiles.Horizontal;
                    tile.rahc.tileData.tiles = pluginHost.BytesToTiles(tile.rahc.tileData.tiles[0]);
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    break;
                case 2:
                    tile.orden = Orden_Tiles.Vertical;
                    break;
            }
            oldTiles = comboBox1.SelectedIndex;

            UpdateImage();
        }
        private void UpdateImage()
        {
            if (stopUpdating)
                return;

            if (tile.orden != Orden_Tiles.No_Tiles)
            {
                tile.rahc.nTilesX = (ushort)(numericWidth.Value / 8);
                tile.rahc.nTilesY = (ushort)(numericHeight.Value / 8);
            }
            else
            {
                tile.rahc.nTilesX = (ushort)numericWidth.Value;
                tile.rahc.nTilesY = (ushort)numericHeight.Value;
            }

            if (isMap)
            {
                NCGR newTile = tile;
                newTile.rahc.tileData = pluginHost.Transformar_NSCR(map, tile.rahc.tileData);
                pic.Image = pluginHost.Bitmap_NCGR(newTile, paleta, startTile);
            }
            else
                pic.Image = pluginHost.Bitmap_NCGR(tile, paleta, startTile);
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
                columnPos.Text = xml.Element("S03").Value;
                columnCampo.Text = xml.Element("S04").Value;
                columnValor.Text = xml.Element("S05").Value;
                listInfo.Items[0].SubItems[1].Text = xml.Element("S06").Value;
                listInfo.Items[1].SubItems[1].Text = xml.Element("S07").Value;
                listInfo.Items[2].SubItems[1].Text = xml.Element("S08").Value;
                listInfo.Items[3].SubItems[1].Text = xml.Element("S09").Value;
                listInfo.Items[4].SubItems[1].Text = xml.Element("S0A").Value;
                listInfo.Items[5].SubItems[1].Text = xml.Element("S0B").Value;
                listInfo.Items[6].SubItems[1].Text = xml.Element("S0C").Value;
                listInfo.Items[7].SubItems[1].Text = xml.Element("S0D").Value;
                listInfo.Items[8].SubItems[1].Text = xml.Element("S0E").Value;
                listInfo.Items[9].SubItems[1].Text = xml.Element("S0F").Value;
                listInfo.Items[10].SubItems[1].Text = xml.Element("S10").Value;
                label3.Text = xml.Element("S11").Value;
                label1.Text = xml.Element("S12").Value;
                label2.Text = xml.Element("S13").Value;
                label6.Text = xml.Element("S14").Value;
                btnSave.Text = xml.Element("S15").Value;
                comboBox1.Items[0] = xml.Element("S16").Value;
                comboBox1.Items[1] = xml.Element("S17").Value;
                //comboBox1.Items[2] = xml.Element("S18").Value;
                checkTransparency.Text = xml.Element("S1D").Value;
                lblZoom.Text = xml.Element("S1E").Value;
                btnBgd.Text = xml.Element("S1F").Value;
                btnBgdTrans.Text = xml.Element("S20").Value;
                btnImport.Text = xml.Element("S21").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }
        private void Info()
        {
            listInfo.Items[0].SubItems.Add("0x" + String.Format("{0:X}", tile.cabecera.constant));
            listInfo.Items[1].SubItems.Add(tile.cabecera.nSection.ToString());
            listInfo.Items[2].SubItems.Add(new String(tile.rahc.id));
            listInfo.Items[3].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.size_section));
            listInfo.Items[4].SubItems.Add(tile.rahc.nTilesY.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesY) + ')');
            listInfo.Items[5].SubItems.Add(tile.rahc.nTilesX.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesX) + ')');
            listInfo.Items[6].SubItems.Add(Enum.GetName(tile.rahc.depth.GetType(), tile.rahc.depth));
            listInfo.Items[7].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown1));
            listInfo.Items[8].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.tiledFlag));
            listInfo.Items[9].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.size_tiledata));
            listInfo.Items[10].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown3));
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
                NCGR newTile = pluginHost.BitmapToTile(o.FileName, (comboBox1.SelectedIndex == 0 ? Orden_Tiles.No_Tiles : Orden_Tiles.Horizontal));
                String tileFile = System.IO.Path.GetTempFileName() + '.' + new String(tile.cabecera.id);
                newTile.id = tile.id;
                newTile.cabecera.id = tile.cabecera.id;
                tile = newTile;

                pluginHost.Set_NCGR(tile);
                Write_Tiles(tileFile, tile, pluginHost);
                pluginHost.ChangeFile((int)tile.id, tileFile);

                if (isMap)
                {
                    NSCR newMap;
                    if (tile.orden == Orden_Tiles.Horizontal)
                        newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX * 8, tile.rahc.nTilesY * 8);
                    else
                        newMap = pluginHost.Create_BasicMap((int)tile.rahc.nTiles, tile.rahc.nTilesX, tile.rahc.nTilesY);
                    newMap.id = map.id;
                    newMap.cabecera.id = map.cabecera.id;
                    String mapFile = System.IO.Path.GetTempFileName() + new String(map.cabecera.id);
                    map = newMap;

                    pluginHost.Set_NSCR(map);
                    Write_Map(mapFile, map, pluginHost);
                    pluginHost.ChangeFile((int)map.id, mapFile);
                }

                stopUpdating = true;
                if (tile.orden == Orden_Tiles.No_Tiles)
                {
                    numericWidth.Value = tile.rahc.nTilesX;
                    numericHeight.Value = tile.rahc.nTilesY;
                    numericHeight.Minimum = 0;
                    numericWidth.Minimum = 0;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    oldTiles = 0;
                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    numericWidth.Value = tile.rahc.nTilesX * 8;
                    numericHeight.Value = tile.rahc.nTilesY * 8;
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    oldTiles = 1;
                    comboBox1.SelectedIndex = 1;
                }
                comboDepth.Text = (tile.rahc.depth == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp");
                oldDepth = comboDepth.Text;
                stopUpdating = false;

                UpdateImage();
                Info();
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
            UpdateImage(); // Devolvemos la imagen original para no perder calidad

            float scale = trackZoom.Value / 100f;
            Bitmap imagen = new Bitmap((int)(pic.Image.Width * scale), (int)(pic.Image.Height * scale));
            Graphics graficos = Graphics.FromImage(imagen);
            graficos.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graficos.DrawImage(pic.Image, 0, 0, pic.Image.Width * scale, pic.Image.Height * scale);
            pic.Image = imagen;
        }
        private void checkTransparency_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTransparency.Checked)
            {
                Bitmap imagen = (Bitmap)pic.Image;
                imagen.MakeTransparent(paleta.pltt.paletas[tile.rahc.tileData.nPaleta[0]].colores[0]);
                pic.Image = imagen;
            }
            else
                UpdateImage();
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
