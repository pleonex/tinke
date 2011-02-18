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
        }
        public iNCER(NCER ncer, NCGR tile, NCLR paleta)
        {
            InitializeComponent();
            this.ncer = ncer;
            this.tile = tile;
            this.paleta = paleta;

            ShowInfo();

            for (ushort i = 0; i < ncer.cebk.nBanks; i++)
                comboCelda.Items.Add(ncer.labl.names[i]);
            comboCelda.SelectedIndex = 0;

            imgBox.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[0],
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked);

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
            imgBox.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[comboCelda.SelectedIndex],
                tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked);
        }
        private void check_CheckedChanged(object sender, EventArgs e)
        {
            imgBox.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[comboCelda.SelectedIndex],
               tile, paleta, checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.AutoUpgradeEnabled = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Imagen Portable Network Graphics (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                imgBox.Image.Save(o.FileName);
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
                pic.Image = Imagen_NCER.Obtener_Imagen(ncer.cebk.banks[i], tile, paleta, 
                    checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked);
                Label lbl = new Label();
                lbl.Text = "Imagen " + (i + 1).ToString();
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

            ven.Text = "Imagenes";
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.AutoScroll = true;
            ven.MaximumSize = new Size(1024, 750);
            ven.ShowIcon = false;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.MaximizeBox = false;
            ven.Show();
        }

    }
}
