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
using PluginInterface;

namespace Tinke
{
    public partial class iNANR : UserControl
    {
        NCLR paleta;
        NCGR tiles;
        NCER celdas;
        NANR ani;
        Bitmap[] bitAni;
        int imgShow;

        public iNANR()
        {
            InitializeComponent();
            LeerIdioma();
        }
        public iNANR(Bitmap[] animaciones, int intervalo)
        {
            // Constructor para que sólo se vea una animación en bucle en el picturebox.
            InitializeComponent();
            LeerIdioma();

            groupBox1.Hide();
            groupBox2.Hide();
            btnNext.Hide();
            btnPlay.Hide();
            btnPrevious.Hide();
            btnSave.Hide();
            btnStop.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            comboAni.Hide();
            txtTime.Hide();
            lblFullImage.Hide();

            bitAni = animaciones;
            aniBox.Dock = DockStyle.Fill;
            aniBox.Image = bitAni[0];
            tempo.Interval = intervalo;
            tempo.Enabled = true;
            tempo.Start();
        }
        public iNANR(NCLR paleta, NCGR tiles, NCER celdas, NANR ani)
        {
            InitializeComponent();
            LeerIdioma();

            this.paleta = paleta;
            this.tiles = tiles;
            this.celdas = celdas;
            this.ani = ani;

            for (int i = 0; i < ani.abnk.nBanks; i++)
                comboAni.Items.Add(ani.labl.names[i]);
            comboAni.SelectedIndex = 0;

            ShowInfo();
            Get_Ani();

            tempo.Stop();
            tempo.Interval = Convert.ToInt32(txtTime.Text);
            aniBox.Image = bitAni[0];
        }

        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("NANR");

                label2.Text = xml.Element("S01").Value;
                label1.Text = xml.Element("S02").Value;
                groupBox2.Text = xml.Element("S03").Value;
                checkEntorno.Text = xml.Element("S04").Value;
                checkCeldas.Text = xml.Element("S05").Value;
                checkImage.Text = xml.Element("S06").Value;
                checkTransparencia.Text = xml.Element("S07").Value;
                checkNumeros.Text = xml.Element("S08").Value;
                groupBox1.Text = xml.Element("S09").Value;
                columnCampo.Text = xml.Element("S0A").Value;
                columnValor.Text = xml.Element("S0B").Value;
                listProp.Items[0].Text = xml.Element("S0C").Value;
                listProp.Items[1].Text = xml.Element("S0D").Value;
                listProp.Items[2].Text = xml.Element("S0E").Value;
                listProp.Items[3].Text = xml.Element("S0F").Value;
                listProp.Items[4].Text = xml.Element("S10").Value;
                listProp.Items[5].Text = xml.Element("S0C").Value;
                listProp.Items[5].SubItems[1].Text = xml.Element("S11").Value;
                listProp.Items[6].Text = xml.Element("S12").Value;
                listProp.Items[7].Text = xml.Element("S13").Value;
                listProp.Items[8].Text = xml.Element("S14").Value;
                listProp.Items[9].Text = xml.Element("S15").Value;
                listProp.Items[10].Text = xml.Element("S16").Value;
                listProp.Items[11].Text = xml.Element("S17").Value;
                listProp.Items[12].Text = xml.Element("S0C").Value;
                listProp.Items[12].SubItems[1].Text = xml.Element("S18").Value;
                listProp.Items[13].Text = xml.Element("S19").Value;
                listProp.Items[14].Text = xml.Element("S15").Value;
                listProp.Items[15].Text = xml.Element("S0F").Value;
                listProp.Items[16].Text = xml.Element("S0C").Value;
                listProp.Items[16].SubItems[1].Text = xml.Element("S1A").Value;
                listProp.Items[17].Text = xml.Element("S1B").Value;
                label3.Text = xml.Element("S11").Value;
                btnSave.Text = xml.Element("S1C").Value;
                lblFullImage.Text = xml.Element("S1E").Value;
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }

        private void ShowInfo()
        {
            listProp.Items[1].SubItems.Add(ani.abnk.nBanks.ToString());
            listProp.Items[2].SubItems.Add(ani.abnk.tFrames.ToString());
            listProp.Items[3].SubItems.Add("0x" + String.Format("{0:X}", ani.abnk.constant));
            listProp.Items[4].SubItems.Add("0x" + String.Format("{0:X}", ani.abnk.padding));
            ShowInfo(0);
        }
        private void ShowInfo(int bnk)
        {
            listProp.Items[6].SubItems[1].Text = bnk.ToString();
            listProp.Items[7].SubItems[1].Text = ani.abnk.anis[bnk].nFrames.ToString();
            listProp.Items[8].SubItems[1].Text = ani.abnk.anis[bnk].dataType.ToString();
            listProp.Items[9].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.abnk.anis[bnk].unknown1);
            listProp.Items[10].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.abnk.anis[bnk].unknown2);
            listProp.Items[11].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.abnk.anis[bnk].unknown3);
            ShowInfo(0, 0);
        }
        private void ShowInfo(int bnk, int frame)
        {
            listProp.Items[13].SubItems[1].Text = frame.ToString();
            listProp.Items[14].SubItems[1].Text = ani.abnk.anis[bnk].frames[frame].unknown1.ToString();
            listProp.Items[15].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.abnk.anis[bnk].frames[frame].constant);
            listProp.Items[17].SubItems[1].Text = ani.abnk.anis[bnk].frames[frame].data.nCell.ToString();
        }

        private void Get_Ani()
        {
            int id = comboAni.SelectedIndex;
            imgShow = 0;
            bitAni = new Bitmap[ani.abnk.anis[id].nFrames];
            for (int i = 0; i < ani.abnk.anis[id].nFrames; i++)
            {
                bitAni[i] = Imagen_NCER.Get_Image(celdas.cebk.banks[ani.abnk.anis[id].frames[i].data.nCell], celdas.cebk.block_size,
                    tiles, paleta, checkEntorno.Checked, checkCeldas.Checked, checkNumeros.Checked, checkTransparencia.Checked,
                    checkImage.Checked);
            }
        }

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            Get_Ani();
            aniBox.Image = bitAni[imgShow];
        }
        private void comboAni_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Ani();

            aniBox.Image = bitAni[imgShow];
            ShowInfo(comboAni.SelectedIndex);
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            tempo.Stop();
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            tempo.Start();
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
        }
        private void tempo_Tick(object sender, EventArgs e)
        {
            imgShow += 1;
            if (imgShow >= bitAni.Length)
                imgShow = 0;
            aniBox.Image = bitAni[imgShow];
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            imgShow += 1;
            if (imgShow >= bitAni.Length)
                imgShow = 0;
            aniBox.Image = bitAni[imgShow];

            ShowInfo(comboAni.SelectedIndex, imgShow);
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            imgShow -= 1;
            if (imgShow < 0)
                imgShow = bitAni.Length - 1;
            aniBox.Image = bitAni[imgShow];

            ShowInfo(comboAni.SelectedIndex, imgShow);
        }
        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtTime.Text) != 0)
                tempo.Interval = Convert.ToInt32(txtTime.Text);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Animation PNG (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                Tools.APNG.Crear_APNG(bitAni, o.FileName, Convert.ToInt32(txtTime.Text) / 10, 0x00);            
        }

        private void aniBox_DoubleClick(object sender, EventArgs e)
        {
            Form ventana = new Form();
            ventana.FormBorderStyle = FormBorderStyle.FixedSingle;
            ventana.Size = new System.Drawing.Size(512, 512);
            ventana.Text = Tools.Helper.ObtenerTraduccion("NANR", "S1D");
            ventana.MaximizeBox = false;
            ventana.ShowIcon = false;
            ventana.BackColor = SystemColors.GradientInactiveCaption;

            int id = comboAni.SelectedIndex;
            Bitmap[] animaciones = new Bitmap[ani.abnk.anis[id].nFrames];
            for (int i = 0; i < ani.abnk.anis[id].nFrames; i++)
            {
                animaciones[i] = Imagen_NCER.Get_Image(celdas.cebk.banks[ani.abnk.anis[id].frames[i].data.nCell], celdas.cebk.block_size,
                    tiles, paleta, checkEntorno.Checked, checkCeldas.Checked, checkNumeros.Checked, checkTransparencia.Checked,
                    checkImage.Checked, 512, 512);
            }

            iNANR control = new iNANR(animaciones, Convert.ToInt32(txtTime.Text));
            control.Dock = DockStyle.Fill;
            ventana.Controls.Add(control);
            ventana.Show();
        }
    }
}
