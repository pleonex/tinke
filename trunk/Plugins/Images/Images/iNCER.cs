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
using PluginInterface;

namespace Images
{
    public partial class iNCER : UserControl
    {
        NCER ncer;
        NCGR tile;
        NCLR paleta;

        IPluginHost pluginHost;
        bool selectColor;

        public iNCER()
        {
            InitializeComponent();
            LeerIdioma();
        }
        public iNCER(NCER ncer, NCGR tile, NCLR paleta, IPluginHost pluginHost)
        {
            InitializeComponent();
            LeerIdioma();
            this.ncer = ncer;
            this.tile = tile;
            this.paleta = paleta;
            this.pluginHost = pluginHost;


            for (ushort i = 0; i < ncer.cebk.nBanks; i++)
                comboCelda.Items.Add(ncer.labl.names[i]);
            comboCelda.SelectedIndex = 0;

            ActualizarImagen();

            if (new String(paleta.header.id) != "NCLR" && new String(paleta.header.id) != "RLCN") // Not NCLR file
                btnSetTrans.Enabled = false;
        }

        private void LeerIdioma()
        {
            try
            {
                //System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("NCER");

                //label1.Text = xml.Element("S01").Value;
                //btnTodos.Text = xml.Element("S02").Value;
                //btnSave.Text = xml.Element("S03").Value;
                //checkEntorno.Text = xml.Element("S0F").Value;
                //checkCelda.Text = xml.Element("S10").Value;
                //checkImagen.Text = xml.Element("S11").Value;
                //checkTransparencia.Text = xml.Element("S12").Value;
                //checkNumber.Text = xml.Element("S13").Value;
                //label2.Text = xml.Element("S15").Value;
                //lblZoom.Text = xml.Element("S16").Value;
                //btnBgd.Text = xml.Element("S17").Value;
                //btnBgdTrans.Text = xml.Element("S18").Value;
                //btnImport.Text = xml.Element("S24").Value;
                //btnSetTrans.Text = xml.Element("S25").Value;
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }

        private void comboCelda_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarImagen();
        }
        private void check_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarImagen();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
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
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 2)
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 3)
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    ActualizarFullImagen().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
        }

        private void btnTodos_Click(object sender, EventArgs e)
        {
            Form ven = new Form();
            int xMax = 4 * 260;
            int x = 0;
            int y = 15;

            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(256, 256);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = pluginHost.Bitmap_NCER(ncer.cebk.banks[i], ncer.cebk.block_size, tile, paleta,
                    checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked,
                    checkTransparencia.Checked, checkImagen.Checked);
                Label lbl = new Label();
                lbl.Text = ncer.labl.names[i];
                lbl.Location = new Point(x, y - 15);

                ven.Controls.Add(pic);
                ven.Controls.Add(lbl);

                x += 260;
                if (x >= xMax)
                {
                    x = 0;
                    y += 275;
                }
            }

            //ven.Text = Tools.Helper.GetTranslation("NCER", "S14");
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.AutoScroll = true;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.ShowIcon = false;
            ven.MaximizeBox = false;
            ven.MaximumSize = new System.Drawing.Size(1024, 700);
            ven.Location = new Point(20, 20);
            ven.Show();
        }

        private void imgBox_DoubleClick(object sender, EventArgs e)
        {
            Form ventana = new Form();
            PictureBox pic = new PictureBox();

            pic.Location = new Point(0, 0);
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.BackColor = pictureBgd.BackColor;
            ventana.AutoSize = true;
            ventana.BackColor = SystemColors.GradientInactiveCaption;
            ventana.AutoScroll = true;
            ventana.MaximumSize = new Size(1024, 700);
            ventana.ShowIcon = false;
            //ventana.Text = Tools.Helper.GetTranslation("NCER", "S14");
            ventana.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ventana.MaximizeBox = false;

            pic.Image = ActualizarFullImagen();

            ventana.Controls.Add(pic);
            ventana.Show();
        }

        private void btnBgdTrans_Click(object sender, EventArgs e)
        {
            btnBgdTrans.Enabled = false;

            pictureBgd.BackColor = Color.Transparent;
            imgBox.BackColor = Color.Transparent;
        }
        private void btnBgd_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;

            if (o.ShowDialog() == DialogResult.OK)
            {
                pictureBgd.BackColor = o.Color;
                imgBox.BackColor = o.Color;
                btnBgdTrans.Enabled = true;
            }
        }

        private void trackZoom_Scroll(object sender, EventArgs e)
        {
            ActualizarImagen();
        }

        private Image ActualizarImagen()
        {
            imgBox.Image = pluginHost.Bitmap_NCER(ncer.cebk.banks[comboCelda.SelectedIndex], ncer.cebk.block_size,
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked,
                checkImagen.Checked, trackZoom.Value);

            return imgBox.Image;
        }
        private Image ActualizarFullImagen()
        {
            // Devolvemos la imagen a su estado inicial
            Image original = pluginHost.Bitmap_NCER(ncer.cebk.banks[comboCelda.SelectedIndex], ncer.cebk.block_size,
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked,
                checkImagen.Checked, 512, 512, trackZoom.Value);

            return original;
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
            }
        }
        private void btnSetTrans_Click(object sender, EventArgs e)
        {
        }
        private void imgBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectColor && imgBox.Image is Image)
            {
                Color color = ((Bitmap)imgBox.Image).GetPixel(e.X, e.Y);
                Change_TransparencyColor(color);
            }
        }
        private void Change_TransparencyColor(Color color)
        {
        }
        private void Add_TransparencyColor()
        {
        }

    }
}
