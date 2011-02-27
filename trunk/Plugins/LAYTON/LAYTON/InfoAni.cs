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

        public InfoAni()
        {
            InitializeComponent();
        }
        public InfoAni(Ani.Todo info, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;

            infoPicture1.Informacion = info;
            imagenes = new Bitmap[info.imgs];
            for (int i = 0; i < info.imgs; i++)
            {
                comboBox1.Items.Add(info.imagenes[i].name);
                imagenes[i] = info.imagenes[i].bitmap;
            }

            comboBox1.SelectedIndex = 0;
            pictureBox1.Image = imagenes[0];
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
            o.Filter = "Imagen BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }
        private void btnSaveAni_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".gif";
            o.Filter = "Animación gif (*.gif)|*.gif";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                pluginHost.Crear_Gif(o.FileName, imagenes, Convert.ToInt32(maskedTextBox1.Text), 0);
                
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
            timer1.Interval = Convert.ToInt32(maskedTextBox1.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < imagenes.Length; i++)
                imagenes[i].MakeTransparent();

            pictureBox1.Image = imagenes[imgShow];
        }
    }
}
