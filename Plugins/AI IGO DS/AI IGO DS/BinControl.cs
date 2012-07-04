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
using System.Xml.Linq;
using System.Windows.Forms;
using Ekona;

namespace AI_IGO_DS
{
    public partial class BinControl : UserControl
    {
        IPluginHost pluginHost;
        BIN bin;
        bool stop;

        public BinControl()
        {
            InitializeComponent();
        }
        public BinControl(IPluginHost pluginHost, BIN bin)
        {
            InitializeComponent();
            this.bin = bin;
            this.pluginHost = pluginHost;

            ReadLanguage();

            stop = true;
            numericImage.Maximum = bin.NumImages - 1;
            numericWidth.Value = bin.Get_Size(0).Width;
            numericHeight.Value = bin.Get_Size(0).Height;
            stop = false;

            picBox.Image = bin.Get_Image(0);
        }

        private void ReadLanguage()
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "Plugins" +
                    System.IO.Path.DirectorySeparatorChar + "AI IGO DSLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("BinControl");

                label1.Text = xml.Element("S00").Value;
                label3.Text = xml.Element("S01").Value;
                label2.Text = xml.Element("S02").Value;
                checkTransparency.Text = xml.Element("S03").Value;
                btnBgd.Text = xml.Element("S04").Value;
                btnBgdRem.Text = xml.Element("S05").Value;
                label4.Text = xml.Element("S06").Value;
                btnSave.Text = xml.Element("S07").Value;
                label5.Text = xml.Element("S08").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = "bmp";
            o.OverwritePrompt = true;
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            if (o.ShowDialog() == DialogResult.OK)
                picBox.Image.Save(o.FileName);
            o.Dispose();

        }

        private void numericImage_ValueChanged(object sender, EventArgs e)
        {
            stop = true;
            numericWidth.Value = bin.Get_Size((int)numericImage.Value).Width;
            numericHeight.Value = bin.Get_Size((int)numericImage.Value).Height;
            stop = false;

            Update_Image();
        }
        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            if (numericWidth.Value != 0 && numericHeight.Value != 0)
                bin.Set_Size(new Size((int)numericWidth.Value, (int)numericHeight.Value),
                             (int)numericImage.Value);

            Update_Image();
        }
        private void Update_Image()
        {
            picBox.Image = bin.Get_Image((int)numericImage.Value);

            if (checkTransparency.Checked)
            {
                Bitmap imaget = (Bitmap)picBox.Image;
                imaget.MakeTransparent(bin.Get_Palette[0]);
                picBox.Image = imaget;
            }
        }

        private void picBox_DoubleClick(object sender, EventArgs e)
        {
            Form ven = new Form();
            PictureBox pcBox = new PictureBox();
            pcBox.Image = picBox.Image;
            pcBox.SizeMode = PictureBoxSizeMode.AutoSize;

            ven.Controls.Add(pcBox);
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.Text = "Image";
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
            Update_Image();
            float scale = trackZoom.Value / 100f;
            Bitmap imagen = new Bitmap((int)(picBox.Image.Width * scale), (int)(picBox.Image.Height * scale));
            Graphics graficos = Graphics.FromImage(imagen);
            graficos.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graficos.DrawImage(picBox.Image, 0, 0, picBox.Image.Width * scale, picBox.Image.Height * scale);
            picBox.Image = imagen;
        }

        private void checkTransparency_CheckedChanged(object sender, EventArgs e)
        {
            Update_Image();
        }

        private void btnBgdRem_Click(object sender, EventArgs e)
        {
            picBox.BackColor = System.Drawing.Color.Transparent;
        }

        private void btnBgd_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;
            o.FullOpen = true;
            if (o.ShowDialog() == DialogResult.OK)
                picBox.BackColor = o.Color;
        }


    }
}
