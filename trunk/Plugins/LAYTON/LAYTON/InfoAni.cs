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

namespace LAYTON
{
    public partial class InfoAni : UserControl
    {
        Bitmap[] imagenes;
        int imgShow = 0;
        IPluginHost pluginHost;
        Ani.Todo info;

        public InfoAni()
        {
            InitializeComponent();
        }
        public InfoAni(Ani.Todo info, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            LeerIdioma();

            this.info = info;
            infoPicture1.Idioma = pluginHost.Get_Language();
            infoPicture1.Informacion = info;
            imagenes = new Bitmap[info.imgs];
            for (int i = 0; i < info.imgs; i++)
            {
                comboBox1.Items.Add(info.imagenes[i].name);
                imagenes[i] = (Bitmap)info.imagenes[i].bitmap.Clone();
            }

            comboBox1.SelectedIndex = 0;
            pictureBox1.Image = imagenes[0];
        }
        
        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("InfoAni");

                label1.Text = xml.Element("S01").Value;
                checkBox1.Text = xml.Element("S02").Value;
                btnSave.Text = xml.Element("S03").Value;
                btnSaveAni.Text = xml.Element("S04").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            pictureBox1.Image = imagenes[comboBox1.SelectedIndex];
            infoPicture1.Imagen_Seleccionada = comboBox1.SelectedIndex;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }
        private void btnSaveAni_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Animation PNG (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                pluginHost.Create_APNG(o.FileName, imagenes, Convert.ToInt32(maskedTextBox1.Text), 0);
                
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            imgShow--;
            if (imgShow < 0)
                imgShow = imagenes.Length - 1;

            pictureBox1.Image = imagenes[imgShow];
            comboBox1.SelectedIndex = imgShow;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            imgShow++;
            if (imgShow >= imagenes.Length)
                imgShow = 0;

            pictureBox1.Image = imagenes[imgShow];
            comboBox1.SelectedIndex = imgShow;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            imgShow++;
            if (imgShow >= imagenes.Length)
                imgShow = 0;

            pictureBox1.Image = imagenes[imgShow];
            comboBox1.SelectedIndex = imgShow;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text != "" && maskedTextBox1.Text != "0")
                timer1.Interval = Convert.ToInt32(maskedTextBox1.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                for (int i = 0; i < imagenes.Length; i++)
                    imagenes[i].MakeTransparent();
            else
                for (int i = 0; i < imagenes.Length; i++)
                    imagenes[i] = info.imagenes[i].bitmap;

            pictureBox1.Image = imagenes[imgShow];
        }
    }
}
