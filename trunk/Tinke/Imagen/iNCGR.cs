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
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace Tinke
{
    public partial class iNCGR : UserControl
    {
        NCLR paleta;
        NCGR tile;
        NSCR map;
        IPluginHost pluginHost;

        string oldDepth;


        int oldTiles;
        bool stopUpdating;
        bool isMap;

        bool selectColor;

        public iNCGR()
        {
            InitializeComponent();
            ReadLanguage();
        }
        public iNCGR(NCGR tile, NCLR paleta, IPluginHost pluginHost)
        {
            InitializeComponent();
            ReadLanguage();

            this.isMap = false;
            this.paleta = paleta;
            this.tile = tile;
            this.pluginHost = pluginHost;

            if (!(this.tile.other is int))
                this.tile.other = 0;

            pic.Image = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)this.tile.other);
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;
            this.comboDepth.Text = (tile.rahc.depth == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp");
            oldDepth = comboDepth.Text;
            switch (tile.order)
            {
                case TileOrder.NoTiled:
                    oldTiles = 0;
                    comboBox1.SelectedIndex = 0;
                    break;
                case TileOrder.Horizontal:
                    oldTiles = 1;
                    comboBox1.SelectedIndex = 1;
                    break;
            }


            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            Info();

            if (new String(paleta.header.id) != "NCLR" && new String(paleta.header.id) != "RLCN")
                btnSetTrans.Enabled = false;
        }
        public iNCGR(NCGR tile, NCLR paleta, NSCR map, IPluginHost pluginHost)
        {
            InitializeComponent();
            ReadLanguage();

            this.isMap = true;
            this.paleta = paleta;
            this.tile = tile;
            this.map = map;
            this.pluginHost = pluginHost;


            if (!(this.tile.other is int))
                this.tile.other = 0;
            if (!(this.map.other is int))
                this.map.other = 0;

            this.numericWidth.Value = map.section.width;
            this.numericHeight.Value = map.section.height;

            this.comboDepth.Text = (tile.rahc.depth == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp");
            oldDepth = comboDepth.Text;

            switch (tile.order)
            {
                case TileOrder.NoTiled:
                    oldTiles = 0;
                    comboBox1.SelectedIndex = 0;
                    break;
                case TileOrder.Horizontal:
                    oldTiles = 1;
                    comboBox1.SelectedIndex = 1;
                    break;
            }

            this.comboDepth.SelectedIndexChanged += new EventHandler(comboDepth_SelectedIndexChanged);
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            Info();

            if (new String(paleta.header.id) != "NCLR" && new String(paleta.header.id) != "RLCN")
                btnSetTrans.Enabled = false;

            UpdateImage();
        }

        private void numericStart_ValueChanged(object sender, EventArgs e)
        {
            if (isMap)
            {
                map.other = (int)numericStart.Value;
                pluginHost.Set_NSCR(map);
            }
            else
            {
                tile.other = (int)numericStart.Value;
                pluginHost.Set_NCGR(tile);
            }

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
                byte[] temp = Convertir.Bit8ToBit4(Convertir.TilesToBytes(tile.rahc.tileData.tiles));
                if (tile.order != TileOrder.NoTiled)
                {
                    tile.rahc.tileData.tiles = pluginHost.BytesToTiles(temp);
                    tile.rahc.tileData.nPalette = new byte[tile.rahc.tileData.tiles.Length];
                }
                else
                {
                    tile.rahc.tileData.tiles = new Byte[1][];
                    tile.rahc.tileData.tiles[0] = temp;
                    tile.rahc.tileData.nPalette = new Byte[tile.rahc.tileData.tiles[0].Length];
                }
            }
            else
            {
                byte[] temp = Convertir.Bit4ToBit8(Convertir.TilesToBytes(tile.rahc.tileData.tiles));
                if (tile.order != TileOrder.NoTiled)
                {
                    tile.rahc.tileData.tiles = pluginHost.BytesToTiles(temp);
                    tile.rahc.tileData.nPalette = new byte[tile.rahc.tileData.tiles.Length];
                }
                else
                {
                    tile.rahc.tileData.tiles = new Byte[1][];
                    tile.rahc.tileData.tiles[0] = temp;
                    tile.rahc.tileData.nPalette = new byte[tile.rahc.tileData.tiles[0].Length];
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
                    tile.order = TileOrder.NoTiled;
                    tile.rahc.tileData.tiles[0] = Convertir.TilesToBytes(tile.rahc.tileData.tiles);
                    numericHeight.Minimum = 0;
                    numericWidth.Minimum = 0;
                    numericWidth.Increment = 1;
                    numericHeight.Increment = 1;
                    break;
                case 1:
                    tile.order = TileOrder.Horizontal;
                    tile.rahc.tileData.tiles = Convertir.BytesToTiles(tile.rahc.tileData.tiles[0]);
                    numericHeight.Minimum = 8;
                    numericWidth.Minimum = 8;
                    numericWidth.Increment = 8;
                    numericHeight.Increment = 8;
                    break;
                case 2:
                    tile.order = TileOrder.Vertical;
                    break;
            }
            oldTiles = comboBox1.SelectedIndex;

            pluginHost.Set_NCGR(tile);
            UpdateImage();
        }
        private Image UpdateImage()
        {
            if (stopUpdating)
                return null;

            Image image;

            if (tile.order != TileOrder.NoTiled)
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
                newTile.rahc.tileData.tiles = Convertir.BytesToTiles(Convertir.TilesToBytes(newTile.rahc.tileData.tiles, (int)tile.other));
                newTile.rahc.tileData = pluginHost.Transform_NSCR(map, newTile.rahc.tileData, (int)map.other);
                image = Imagen_NCGR.Crear_Imagen(newTile, paleta, 0, trackZoom.Value);
            }
            else
                image = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tile.other, trackZoom.Value);
                
            pic.Image = image;
            return image;
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("NCGR");

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
                checkTransparency.Text = xml.Element("S1C").Value;
                lblZoom.Text = xml.Element("S1E").Value;
                btnBgd.Text = xml.Element("S1F").Value;
                btnBgdTrans.Text = xml.Element("S20").Value;
                btnImport.Text = xml.Element("S21").Value;
                btnSetTrans.Text = xml.Element("S22").Value;
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }
        private void Info()
        {
            listInfo.Items[0].SubItems.Add("0x" + String.Format("{0:X}", tile.header.constant));
            listInfo.Items[1].SubItems.Add(tile.header.nSection.ToString());
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
            o.Filter = "Supported images |*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.ico;*.icon|" +
                       "BitMaP (*.bmp)|*.bmp|" +
                       "Portable Network Graphic (*.png)|*.png|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.Multiselect = false;
            if (o.ShowDialog() == DialogResult.OK)
            {
                string filePath = o.FileName;

                #region Convert to BMP format
                Image tempImage = Image.FromFile(o.FileName);
                if (!(tempImage.PixelFormat == PixelFormat.Format8bppIndexed ||tempImage.PixelFormat == PixelFormat.Format4bppIndexed) ||
                    tempImage.RawFormat != ImageFormat.Bmp)
                {
                    tempImage.Dispose();
                    // Convert the image to bmp format
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        // Convert to BMP
                        Image.FromFile(filePath).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        Bitmap bmp = (Bitmap)Image.FromStream(ms);

                        // Convert to indexed colors (palette+tiles)
                        if (tile.rahc.depth == ColorDepth.Depth4Bit)
                        {
                            bmp = bmp.Clone(
                                new Rectangle(new Point(0, 0), bmp.Size),
                                System.Drawing.Imaging.PixelFormat.Format4bppIndexed);
                        }
                        else
                        {
                            bmp = bmp.Clone(
                                new Rectangle(new Point(0, 0), bmp.Size),
                                System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                        }

                        // Save the new file
                        filePath = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar + "bmp_" +
                            System.IO.Path.GetFileName(filePath);
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
                tempImage.Dispose();
                #endregion

                // Ask for map options
                int width, heigth, num_palette = 0, startTile = 0;
                Dialog.MapOptions mapOptions = new Dialog.MapOptions();
                if (isMap)
                {
                    mapOptions.SubPalette = map.section.mapData[0].nPalette;
                    Image currentImg = Image.FromFile(filePath);
                    width = currentImg.Width;
                    heigth = currentImg.Height;
                    currentImg.Dispose();

                    mapOptions = new Dialog.MapOptions(width, heigth);
                    mapOptions.ShowDialog();
                    startTile = mapOptions.SubImagesStart;
                    num_palette = mapOptions.SubPalette;
                }


                #region Save the new palette
                if (new String(paleta.header.id) != "NCLR" && new String(paleta.header.id) != "RLCN")
                    goto Set_Tiles;

                NCLR newPalette = Imagen_NCLR.BitmapToPalette(filePath, num_palette);

                // Save the other palettes
                if (mapOptions.SubImages)
                {
                    List<NTFP> pals = new List<NTFP>();
                    for (int i = 0; i < paleta.pltt.palettes.Length; i++)
                    {
                        if (i == num_palette)
                        {
                            pals.Add(newPalette.pltt.palettes[num_palette]);
                            continue;
                        }
                        pals.Add(paleta.pltt.palettes[i]);
                    }
                    newPalette.pltt.palettes = pals.ToArray();
                    newPalette.pltt.paletteLength = (uint)pals.Count * newPalette.pltt.nColors * 2;
                    newPalette.pltt.length = newPalette.pltt.paletteLength + 0x18;
                    newPalette.header.file_size = newPalette.pltt.length + newPalette.header.header_size;
                }
                newPalette.id = paleta.id;
                paleta = newPalette;

                pluginHost.Set_NCLR(paleta);
                String paletteFile = System.IO.Path.GetTempFileName();
                Imagen_NCLR.Escribir(paleta, paletteFile);
                pluginHost.ChangeFile((int)paleta.id, paletteFile);
                #endregion
                #region Save the new tiles
            Set_Tiles:

                NCGR newTile = Imagen_NCGR.BitmapToTile(filePath, (comboBox1.SelectedIndex == 0 ? TileOrder.NoTiled : TileOrder.Horizontal));
                newTile.id = tile.id;

                // Save the other images
                if (mapOptions.SubImages)
                {
                    newTile.rahc.tileData.tiles = Imagen_NCGR.MergeImage(tile.rahc.tileData.tiles, newTile.rahc.tileData.tiles, startTile);
                    newTile.rahc.nTiles = (uint)newTile.rahc.tileData.tiles.Length;
                    newTile.rahc.nTilesY = (ushort)(newTile.rahc.nTiles / newTile.rahc.nTilesX);
                    newTile.rahc.size_tiledata = (uint)newTile.rahc.tileData.tiles.Length * 64;
                    newTile.rahc.size_section = newTile.rahc.size_tiledata + 0x20;
                    newTile.header.file_size = newTile.rahc.size_section + newTile.header.header_size;
                }

                tile = newTile;
                tile.other = 0;

                pluginHost.Set_NCGR(tile);
                String tileFile = System.IO.Path.GetTempFileName();
                Imagen_NCGR.Write(tile, tileFile);
                pluginHost.ChangeFile((int)tile.id, tileFile);
                #endregion
                #region Save the new map
                if (isMap)
                {
                    if (new String(map.header.id) != "NSCR" && new String(map.header.id) != "RCSN")
                        goto End;

                    NSCR newMap;

                    width = mapOptions.ImagenWidth;
                    heigth = mapOptions.ImageHeight;

                    if (mapOptions.FillTiles)
                        newMap = Imagen_NSCR.Create_BasicMap(width, heigth, mapOptions.StartFillTiles, mapOptions.FillTilesWith, startTile, (byte)num_palette);
                    else
                        newMap = Imagen_NSCR.Create_BasicMap((int)tile.rahc.nTiles, width, heigth, startTile, (byte)num_palette);

                    newMap.id = map.id;
                    newMap.section.padding = map.section.padding;
                    map = newMap;
                    map.other = 0;

                    pluginHost.Set_NSCR(map);
                    String mapFile = System.IO.Path.GetTempFileName();
                    Imagen_NSCR.Write(map, mapFile);
                    pluginHost.ChangeFile((int)map.id, mapFile);
                }
                #endregion

                #region Refresh image and information
            End:
                stopUpdating = true;
                if (tile.order == TileOrder.NoTiled)
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
                    if (isMap)
                    {
                        numericWidth.Value = map.section.width;
                        numericHeight.Value = map.section.height;
                    }
                    else
                    {
                        numericWidth.Value = tile.rahc.nTilesX * 8;
                        numericHeight.Value = tile.rahc.nTilesY * 8;
                    }
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
                #endregion
            }
        }
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
                Bitmap image = (Bitmap)UpdateImage();
                if (o.FilterIndex == 1)
                    image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 2)
                    UpdateImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 3)
                    UpdateImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    UpdateImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    UpdateImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    UpdateImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
            o.Dispose();
        }
        private void pic_DoubleClick(object sender, EventArgs e)
        {
            Form ven = new Form();
            PictureBox pcBox = new PictureBox();
            pcBox.Image = pic.Image;
            pcBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pcBox.BackColor = pictureBgd.BackColor;

            ven.Controls.Add(pcBox);
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.Text = Tools.Helper.ObtenerTraduccion("NCGR", "S19");
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
            UpdateImage();
        }
        private void checkTransparency_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTransparency.Checked)
            {
                Bitmap imagen = (Bitmap)pic.Image;
                imagen.MakeTransparent(paleta.pltt.palettes[tile.rahc.tileData.nPalette[0]].colors[0]);
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

        private void btnSetTrans_Click(object sender, EventArgs e)
        {
            Dialog.SelectModeColor dialog = new Dialog.SelectModeColor();
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            if (dialog.Option == 2)
            {
                ColorDialog o = new ColorDialog();
                o.AllowFullOpen = true;
                o.AnyColor = true;
                o.FullOpen = true;
                if (o.ShowDialog() == DialogResult.OK)
                    Change_TransparencyColor(o.Color);
                o.Dispose();
            }
            else if (dialog.Option == 1)
                selectColor = true;
            else if (dialog.Option == 3)
                Add_TransparencyColor();

        }
        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectColor && pic.Image is Image)
            {
                Color color = ((Bitmap)pic.Image).GetPixel(e.X, e.Y);
                Change_TransparencyColor(color);
            }
        }
        private void Change_TransparencyColor(Color color)
        {
            int colorIndex = 0;
            for (int i = 0; i < paleta.pltt.palettes[0].colors.Length; i++)
            {
                if (paleta.pltt.palettes[0].colors[i] == color)
                {
                    paleta.pltt.palettes[0].colors[i] = paleta.pltt.palettes[0].colors[0];
                    paleta.pltt.palettes[0].colors[0] = color;
                    colorIndex = i;
                    break;
                }
            }

            pluginHost.Set_NCLR(paleta);
            String paletteFile = System.IO.Path.GetTempFileName();
            Imagen_NCLR.Escribir(paleta, paletteFile);
            pluginHost.ChangeFile((int)paleta.id, paletteFile);

            Convertir.Change_Color(ref tile.rahc.tileData.tiles, colorIndex, 0);
            pluginHost.Set_NCGR(tile);
            String tileFile = System.IO.Path.GetTempFileName();
            Imagen_NCGR.Write(tile, tileFile);
            pluginHost.ChangeFile((int)tile.id, tileFile);

            UpdateImage();
            checkTransparency.Checked = true;
        }
        private void Add_TransparencyColor()
        {
            // Search for unused or duplicated colors to change them with transparency color
            // Search for duplicated colors
            int result = Convertir.Remove_NotDuplicatedColors(ref paleta.pltt.palettes[0], ref tile.rahc.tileData.tiles);
            if (result == -1)
            {
                // Try another way: search for not used colors
                result = Convertir.Remove_NotUsedColors(ref paleta.pltt.palettes[0], ref tile.rahc.tileData.tiles);
                if (result == -1)
                {
                    MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S24"));
                    return;  // Nothing found.
                }
            }

            // Now, the palette must have at least one transparency color, we put it in first place.
            paleta.pltt.palettes[0].colors[result] = paleta.pltt.palettes[0].colors[0];
            paleta.pltt.palettes[0].colors[0] = Color.FromArgb(248, 0, 248);
            Convertir.Change_Color(ref tile.rahc.tileData.tiles, result, 0);

            // Save new palette file
            pluginHost.Set_NCLR(paleta);
            String paletteFile = System.IO.Path.GetTempFileName();
            Imagen_NCLR.Escribir(paleta, paletteFile);
            pluginHost.ChangeFile((int)paleta.id, paletteFile);

            // Save new tile file
            pluginHost.Set_NCGR(tile);
            String tileFile = System.IO.Path.GetTempFileName();
            Imagen_NCGR.Write(tile, tileFile);
            pluginHost.ChangeFile((int)tile.id, tileFile);

            // Refresh de image
            UpdateImage();
            checkTransparency.Checked = true;
        }
    }
}
