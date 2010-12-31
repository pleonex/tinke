using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    public partial class InfoAni : UserControl
    {
        Bitmap[] imagenes;

        public InfoAni(Ani.Todo info)
        {
            InitializeComponent();

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
    }
}
