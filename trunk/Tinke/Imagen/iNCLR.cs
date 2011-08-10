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
    public partial class iNCLR : UserControl
    {
        NCLR paleta;
        Bitmap[] paletas;
        IPluginHost pluginHost;

        Byte[] data;
        ColorDepth oldDepth;

        public iNCLR()
        {
            InitializeComponent();
            LeerIdioma();
        }
        public iNCLR(NCLR paleta, IPluginHost pluginHost)
        {
            InitializeComponent();
            LeerIdioma();

            this.paleta = paleta;
            this.pluginHost = pluginHost;
            ShowInfo();

            paletas = Imagen_NCLR.Mostrar(paleta);
            paletaBox.Image = paletas[0];
            nPaleta.Maximum = paletas.Length;
            nPaleta.Minimum = 1;
            nPaleta.Value = 1;

            data = Convertir.ColorToBGR555(paleta.pltt.paletas[0].colores);
            oldDepth = paleta.pltt.profundidad;
        }

        private void LeerIdioma()
        {
            System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("NCLR");

            label1.Text = xml.Element("S01").Value;
            groupProp.Text = xml.Element("S02").Value;
            columnName.Text = xml.Element("S03").Value;
            columnValor.Text = xml.Element("S04").Value;
            listProp.Items[0].Text = xml.Element("S05").Value;
            listProp.Items[1].Text = xml.Element("S06").Value;
            listProp.Items[2].Text = xml.Element("S07").Value;
            listProp.Items[3].Text = xml.Element("S08").Value;
            listProp.Items[4].Text = xml.Element("S09").Value;
            btnSave.Text = xml.Element("S0A").Value;
            btnShow.Text = xml.Element("S0B").Value;
            groupModificar.Text = xml.Element("S11").Value;
            label2.Text = xml.Element("S12").Value;
            btnImport.Text = xml.Element("S13").Value;
            btnConverter.Text = xml.Element("S14").Value;
        }

        private void ShowInfo()
        {
            listProp.Items[0].SubItems.Add(paleta.pltt.paletas.Length.ToString());
            listProp.Items[1].SubItems.Add(paleta.pltt.profundidad == ColorDepth.Depth4Bit ?
                "4-bit" : "8-bit");
            listProp.Items[2].SubItems.Add("0x" + String.Format("{0:X}", paleta.pltt.unknown1));
            listProp.Items[3].SubItems.Add(paleta.pltt.nColores.ToString());
            listProp.Items[4].SubItems.Add(paleta.pltt.tamañoPaletas.ToString());
        }

        private void nPaleta_ValueChanged(object sender, EventArgs e)
        {
            paletaBox.Image = paletas[(int)nPaleta.Value - 1];
            data = Convertir.ColorToBGR555(paleta.pltt.paletas[(int)nPaleta.Value - 1].colores);
            numericStartByte.Value = 0;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            string trad = Tools.Helper.ObtenerTraduccion("NCLR").Element("S0C").Value;
            Form ven = new Form();
            int xMax = 6 * 170;
            int x = 0;
            int y = 15;

            for (int i = 0; i < paletas.Length; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(160, 160);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = paletas[i];
                Label lbl = new Label();
                lbl.Text = trad + ' ' + (i + 1).ToString();
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

            ven.Text = trad;
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
            o.Filter = "Portable Network Graphics (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                paletaBox.Image.Save(o.FileName);
        }

        private void paletaBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (paletaBox.Image is Image)
            {
                Color color = ((Bitmap)paletaBox.Image).GetPixel(e.X, e.Y);
                lblRGB.Text = "RGB: " + color.R + ", " + color.G + ", " + color.B;
            }
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
                NCLR newPalette = Imagen_NCLR.BitmapToPalette(o.FileName);
                newPalette.id = paleta.id;
                paleta = newPalette;

                pluginHost.Set_NCLR(paleta);
                String paletteFile = System.IO.Path.GetTempFileName();
                Imagen_NCLR.Escribir(paleta, paletteFile);
                pluginHost.ChangeFile((int)paleta.id, paletteFile);

                ShowInfo();
                paletas = Imagen_NCLR.Mostrar(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = paletas.Length;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;
                data = Convertir.ColorToBGR555(paleta.pltt.paletas[0].colores);
                oldDepth = paleta.pltt.profundidad;
            }
        }

        private void numericStartByte_ValueChanged(object sender, EventArgs e)
        {
            Byte[] temp = new Byte[data.Length - (int)numericStartByte.Value];
            Array.Copy(data, (int)numericStartByte.Value, temp, 0, temp.Length);

            paleta.pltt.paletas[(int)nPaleta.Value - 1].colores = Convertir.BGR555(temp);
            pluginHost.Set_NCLR(paleta);

            ShowInfo();
            paletas = Imagen_NCLR.Mostrar(paleta);
            paletaBox.Image = paletas[0];
            nPaleta.Maximum = paleta.pltt.paletas.Length;
            nPaleta.Minimum = 1;
            nPaleta.Value = 1;
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            numericStartByte.Value = 0;

            if (oldDepth == ColorDepth.Depth4Bit) // Convert to 8bpp
            {
                paleta.pltt = Convertir.Palette_4bppTo8bpp(paleta.pltt);
                pluginHost.Set_NCLR(paleta);

                ShowInfo();
                paletas = Imagen_NCLR.Mostrar(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = 1;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;

                data = Convertir.ColorToBGR555(paleta.pltt.paletas[0].colores);
                oldDepth = ColorDepth.Depth8Bit;
            }
            else  // Convert to 4bpp
            {
                paleta.pltt = Convertir.Palette_8bppTo4bpp(paleta.pltt);
                pluginHost.Set_NCLR(paleta);

                ShowInfo();
                paletas = Imagen_NCLR.Mostrar(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = paleta.pltt.paletas.Length;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;

                data = Convertir.ColorToBGR555(paleta.pltt.paletas[0].colores);
                oldDepth = ColorDepth.Depth4Bit;
            }

        }
    }
}
