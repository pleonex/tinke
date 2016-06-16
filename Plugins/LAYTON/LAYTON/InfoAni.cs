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
using Ekona;

namespace LAYTON
{
    public partial class InfoAni : UserControl
    {
        IPluginHost pluginHost;
        Ani.Todo info;
        int id;

        Bitmap[] imagenes;
        int frameIndex = 0;

        public InfoAni()
        {
            InitializeComponent();
        }

        public InfoAni(Ani.Todo info, IPluginHost pluginHost, int id)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.id = id;
            LeerIdioma();

            this.info = info;
            infoPicture1.Idioma = pluginHost.Get_Language();
            infoPicture1.Informacion = info;
            for (uint i = 0; i < info.anims.LongLength; i++)
                comboBox1.Items.Add(info.anims[i].name);

            imagenes = new Bitmap[info.imgs];
            for (uint i = 0; i < info.imgs; i++)
                imagenes[i] = (Bitmap)info.imagenes[i].bitmap.Clone();

            infoPicture1.Imagenes = imagenes;
            if (info.anims.LongLength > 1) comboBox1.SelectedIndex = 1;
            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
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
                btnImport.Text = xml.Element("S05").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            frameIndex = 0;
            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
        }

        private Bitmap GetSelectedImage()
        {
            return info.imagenes[this.infoPicture1.Imagen_Seleccionada].bitmap;
        }

        private int GetSelectedIndex()
        {
            return this.infoPicture1.Imagen_Seleccionada;
        }

        private int GetIndexByFrame(int frameIndex)
        {
            if (info.anims[comboBox1.SelectedIndex].framesCount > frameIndex)
                return (int)info.anims[comboBox1.SelectedIndex].imagesIndexes[this.frameIndex];
            else return 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp|Portable Network Graphics (*.png)|*.png";
            o.OverwritePrompt = true;
            o.FileName = this.pluginHost.Search_File((short)id).name + "_" + this.GetSelectedIndex();
            if (o.ShowDialog() == DialogResult.OK)
            {
                switch (o.FilterIndex)
                {
                    case 0:
                        GetSelectedImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    default:
                        GetSelectedImage().Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }
            }
        }

        private void btnSaveAni_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Animation PNG (*.png)|*.png";
            o.OverwritePrompt = true;
            o.FileName = info.anims[comboBox1.SelectedIndex].name;
            Bitmap[] frames = new Bitmap[info.anims[comboBox1.SelectedIndex].framesCount];
            for (int i = 0; i < frames.Length; i++) frames[i] = imagenes[GetIndexByFrame(i)];
            if (frames.Length > 0 && o.ShowDialog() == DialogResult.OK)
                Ekona.Images.Formats.APNG.Create(frames, o.FileName, Convert.ToInt32(maskedTextBox1.Text), 0);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (imagenes.Length == 0) MessageBox.Show("This Animation is Blank.");
            else
            {
                comboBox1.Enabled = false;
                btnPrevious.Enabled = false;
                btnNext.Enabled = false;
                btnPlay.Enabled = false;
                btnImport.Enabled = false;
                btnSave.Enabled = false;

                timer1.Enabled = true;
                timer1.Start();
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            comboBox1.Enabled = true;
            btnPrevious.Enabled = true;
            btnNext.Enabled = true;
            btnPlay.Enabled = true;
            btnImport.Enabled = true;
            btnSave.Enabled = true;

            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
        }
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            frameIndex--;
            if (frameIndex < 0)
                frameIndex = (int)info.anims[comboBox1.SelectedIndex].framesCount - 1;

            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            frameIndex++;
            if (frameIndex >= (int)info.anims[comboBox1.SelectedIndex].framesCount)
                frameIndex = 0;

            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            frameIndex++;
            if (frameIndex >= (int)info.anims[comboBox1.SelectedIndex].framesCount)
                frameIndex = 0;

            this.infoPicture1.Imagen_Seleccionada = GetIndexByFrame(frameIndex);
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
                    imagenes[i] = (Bitmap)info.imagenes[i].bitmap.Clone();

            if (!timer1.Enabled)
                infoPicture1.Imagen_Seleccionada = infoPicture1.Imagen_Seleccionada;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Multiselect = false;
            o.DefaultExt = ".png";
            o.Filter = "All formats (*.png, *.bmp)|*.png;*.bmp|BitMaP (*.bmp)|*.bmp|Portable Network Graphics (*.png)|*.png";
            if (o.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(o.FileName);
                
                Color[] palette = (this.info.imgs > 1) ? this.info.paleta.colores : null;
                palette = this.info.paleta.colores;
                int selIndex = GetSelectedIndex();
                if (info.imagenes[selIndex].Import(bmp, info.type == 1, ref palette))
                {
                    // Update structures
                    if (palette != this.info.paleta.colores && palette != null) this.info.paleta.Update(palette);
                    imagenes[selIndex] = (Bitmap)info.imagenes[selIndex].bitmap.Clone();
                    if (checkBox1.Checked) imagenes[selIndex].MakeTransparent();
                    infoPicture1.UpdateSelected(info.imagenes[selIndex]);
                    infoPicture1.Imagen_Seleccionada = selIndex;

                    // Write file
                    string tempFile = this.pluginHost.Get_TempFile();
                    Ani.SaveToFile(this.info, tempFile);

                    // Compress file
                    string compressedFile = this.pluginHost.Get_TempFile();
                    this.pluginHost.Compress(tempFile, compressedFile, FormatCompress.LZ10);

                    System.IO.BinaryWriter bw = new System.IO.BinaryWriter(System.IO.File.OpenWrite(tempFile));
                    bw.BaseStream.SetLength(0);
                    bw.Write((uint)2);
                    bw.Write(System.IO.File.ReadAllBytes(compressedFile));
                    bw.Close();
                    System.IO.File.Delete(compressedFile);

                    this.pluginHost.ChangeFile(id, tempFile);
                }
            }
        }
    }
}
