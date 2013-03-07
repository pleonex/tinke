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
using Ekona;
using Ekona.Images;

namespace Images
{
    public partial class AnimationControl : UserControl
    {
        IPluginHost pluginHost;
        PaletteBase palette;
        ImageBase image;
        SpriteBase sprite;
        NANR ani;

        Bitmap[] bitAni;
        bool isAni;         // If there are animations
        int imgShow;

        public AnimationControl()
        {
            InitializeComponent();
            Read_Language();
        }
        public AnimationControl(Bitmap[] anis, int interval)
        {
            InitializeComponent();
            Read_Language();

            //groupBox1.Hide();
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

            bitAni = anis;
            aniBox.Dock = DockStyle.Fill;
            aniBox.Image = bitAni[0];
            tempo.Interval = interval;
            tempo.Enabled = true;
            tempo.Start();
        }
        public AnimationControl(IPluginHost pluginHost, NANR ani)
        {
            InitializeComponent();
            Read_Language();

            this.pluginHost = pluginHost;
            this.palette = pluginHost.Get_Palette();
            this.image = pluginHost.Get_Image();
            this.sprite = pluginHost.Get_Sprite();
            this.ani = ani;
            isAni = true;

            if (ani.Struct.abnk.nBanks == 0)
            {
                MessageBox.Show("No animations.");
                isAni = false;
                tempo.Enabled = false;
                comboAni.Enabled = false;
                btnNext.Enabled = false;
                btnPlay.Enabled = false;
                btnPrevious.Enabled = false;
                btnSave.Enabled = false;
                btnStop.Enabled = false;
                txtTime.Enabled = false;
                checkCeldas.Enabled = false;
                checkEntorno.Enabled = false;
                checkImage.Enabled = false;
                checkNumeros.Enabled = false;
                checkTransparencia.Enabled = false;
            }


            for (int i = 0; i < ani.Names.Length; i++)
                comboAni.Items.Add(ani.Names[i]);
            if (isAni)
                comboAni.SelectedIndex = 0;

            ShowInfo();
            Get_Ani();

            tempo.Stop();
            tempo.Interval = Convert.ToInt32(txtTime.Text);

            if (isAni)
                aniBox.Image = bitAni[0];
        }

        private void Read_Language()
        {
            Ekona.Helper.Translation.TranslateControls(this.Controls, "AnimationControl");
            Ekona.Helper.Translation.TranslateControls(this.groupBox2.Controls, "AnimationControl");
        }

        private void ShowInfo()
        {
            //listProp.Items[1].SubItems.Add(ani.Struct.abnk.nBanks.ToString());
            //listProp.Items[2].SubItems.Add(ani.Struct.abnk.tFrames.ToString());
            //listProp.Items[3].SubItems.Add("0x" + String.Format("{0:X}", ani.Struct.abnk.constant));
            //listProp.Items[4].SubItems.Add("0x" + String.Format("{0:X}", ani.Struct.abnk.padding));
            ShowInfo(0);
        }
        private void ShowInfo(int bnk)
        {
            //listProp.Items[6].SubItems[1].Text = bnk.ToString();
            //listProp.Items[7].SubItems[1].Text = ani.Struct.abnk.anis[bnk].nFrames.ToString();
            //listProp.Items[8].SubItems[1].Text = ani.Struct.abnk.anis[bnk].dataType.ToString();
            //listProp.Items[9].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.Struct.abnk.anis[bnk].unknown1);
            //listProp.Items[10].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.Struct.abnk.anis[bnk].unknown2);
            //listProp.Items[11].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.Struct.abnk.anis[bnk].unknown3);
            ShowInfo(0, 0);
        }
        private void ShowInfo(int bnk, int frame)
        {
            //listProp.Items[13].SubItems[1].Text = frame.ToString();
            //listProp.Items[14].SubItems[1].Text = ani.Struct.abnk.anis[bnk].frames[frame].unknown1.ToString();
            //listProp.Items[15].SubItems[1].Text = "0x" + String.Format("{0:X}", ani.Struct.abnk.anis[bnk].frames[frame].constant);
            //listProp.Items[17].SubItems[1].Text = ani.Struct.abnk.anis[bnk].frames[frame].data.nCell.ToString();
        }

        private void Get_Ani()
        {
            if (!isAni)
                return;

            int id = comboAni.SelectedIndex;
            imgShow = 0;
            bitAni = new Bitmap[ani.Struct.abnk.anis[id].nFrames];
            for (int i = 0; i < ani.Struct.abnk.anis[id].nFrames; i++)
            {
                bitAni[i] = (Bitmap)sprite.Get_Image(image, palette, ani.Struct.abnk.anis[id].frames[i].data.nCell, 512, 256,
                    checkEntorno.Checked, checkCeldas.Checked, checkNumeros.Checked, checkTransparencia.Checked, checkImage.Checked);
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
                Ekona.Images.Formats.APNG.Create(bitAni, o.FileName, Convert.ToInt32(txtTime.Text) / 10, 0x00);
        }

        private void aniBox_DoubleClick(object sender, EventArgs e)
        {
            Form ventana = new Form();
            ventana.FormBorderStyle = FormBorderStyle.FixedSingle;
            ventana.Size = new System.Drawing.Size(512, 512);
            //ventana.Text = Tools.Helper.GetTranslation("NANR", "S1D");
            ventana.MaximizeBox = false;
            ventana.ShowIcon = false;
            ventana.BackColor = SystemColors.GradientInactiveCaption;

            int id = comboAni.SelectedIndex;
            Bitmap[] animations = new Bitmap[ani.Struct.abnk.anis[id].nFrames];
            for (int i = 0; i < ani.Struct.abnk.anis[id].nFrames; i++)
            {
                animations[i] = (Bitmap)sprite.Get_Image(image, palette, ani.Struct.abnk.anis[id].frames[i].data.nCell, 512, 256,
                    checkEntorno.Checked, checkCeldas.Checked, checkNumeros.Checked, checkTransparencia.Checked, checkImage.Checked);
            }

            AnimationControl control = new AnimationControl(animations, Convert.ToInt32(txtTime.Text));
            control.Dock = DockStyle.Fill;
            ventana.Controls.Add(control);
            ventana.Show();
        }
    }
}
