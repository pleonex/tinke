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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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

namespace Tinke
{
    public partial class iNCER : UserControl
    {
        NCER ncer;
        NCGR tile;
        NCLR paleta;

        public iNCER()
        {
            InitializeComponent();
            LeerIdioma();
        }
        public iNCER(NCER ncer, NCGR tile, NCLR paleta)
        {
            InitializeComponent();
            LeerIdioma();
            this.ncer = ncer;
            this.tile = tile;
            this.paleta = paleta;

            ShowInfo();

            for (ushort i = 0; i < ncer.cebk.nBanks; i++)
                comboCelda.Items.Add(ncer.labl.names[i]);
            comboCelda.SelectedIndex = 0;

            ActualizarImagen();
        }

        private void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("NCER");

            label1.Text = xml.Element("S01").Value;
            btnTodos.Text = xml.Element("S02").Value;
            btnSave.Text = xml.Element("S03").Value;
            groupBox1.Text = xml.Element("S04").Value;
            columnCampo.Text = xml.Element("S05").Value;
            columnValor.Text = xml.Element("S06").Value;
            listProp.Items[0].Text = xml.Element("S07").Value;
            listProp.Items[1].Text = xml.Element("S08").Value;
            listProp.Items[2].Text = xml.Element("S09").Value;
            listProp.Items[3].Text = xml.Element("S0A").Value;
            listProp.Items[4].Text = xml.Element("S0B").Value;
            listProp.Items[5].Text = xml.Element("S0C").Value;
            listProp.Items[6].Text = xml.Element("S0D").Value;
            listProp.Items[7].Text = xml.Element("S0E").Value;
            checkEntorno.Text = xml.Element("S0F").Value;
            checkCelda.Text = xml.Element("S10").Value;
            checkImagen.Text = xml.Element("S11").Value;
            checkTransparencia.Text = xml.Element("S12").Value;
            checkNumber.Text = xml.Element("S13").Value;
            label2.Text = xml.Element("S15").Value;
            lblZoom.Text = xml.Element("S16").Value;
            btnBgd.Text = xml.Element("S17").Value;
            btnBgdTrans.Text = xml.Element("S18").Value;
        }

        private void ShowInfo()
        {
            listProp.Items[0].SubItems.Add(new String(ncer.cebk.id));
            listProp.Items[1].SubItems.Add(ncer.cebk.nBanks.ToString());
            listProp.Items[2].SubItems.Add(ncer.cebk.tBank.ToString());
            listProp.Items[3].SubItems.Add("0x" + String.Format("{0:X}", ncer.cebk.constant));
            listProp.Items[4].SubItems.Add(ncer.cebk.block_size.ToString());
            listProp.Items[5].SubItems.Add("0x" + String.Format("{0:X}", ncer.cebk.unknown1));
            listProp.Items[6].SubItems.Add("0x" + String.Format("{0:X}", ncer.cebk.unknown2));
            listProp.Items[7].SubItems.Add("0x" + String.Format("{0:X}", ncer.uext.unknown));
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
            o.DefaultExt = ".png";
            o.Filter = "Imagen Portable Network Graphics (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                ActualizarImagen().Save(o.FileName);
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
                pic.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[i], ncer.cebk.block_size, tile, paleta, 
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

            ven.Text = Tools.Helper.ObtenerTraduccion("NCER","S14");
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.AutoScroll = true;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.ShowIcon = false;
            ven.MaximizeBox = false;
            ven.MaximumSize = new System.Drawing.Size(1024, 700);
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
            ventana.Text = Tools.Helper.ObtenerTraduccion("NCER", "S14");
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
            // Devolvemos la imagen a su estado inicial
            imgBox.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[comboCelda.SelectedIndex], ncer.cebk.block_size,
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked,
                checkImagen.Checked);

            float scale = trackZoom.Value / 100f;
            int wSize = (int)(imgBox.Image.Width * scale);
            int hSize = (int)(imgBox.Image.Height * scale);

            Bitmap imagen = new Bitmap(wSize, hSize);
            Graphics graficos = Graphics.FromImage(imagen);
            graficos.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graficos.DrawImage(imgBox.Image, wSize / 2 - wSize + 128, wSize / 2 - hSize + 128, wSize, hSize);
            imgBox.Image = imagen;

            return imagen;
        }
        private Image ActualizarFullImagen()
        {
            // Devolvemos la imagen a su estado inicial
            Image original = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[comboCelda.SelectedIndex], ncer.cebk.block_size,
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked,
                checkImagen.Checked, 512, 512);

            float scale = trackZoom.Value / 100f;
            int wSize = (int)(original.Width * scale);
            int hSize = (int)(original.Height * scale);

            Bitmap imagen = new Bitmap(wSize, hSize);
            Graphics graficos = Graphics.FromImage(imagen);
            graficos.DrawImage(original, 0, 0, wSize, hSize);

            return imagen;
        }

    }
}
