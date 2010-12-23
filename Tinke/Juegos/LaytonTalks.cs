using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke.Juegos
{
    public partial class LaytonTalks : UserControl
    {
        string[] textos;
        Bitmap[] layton;
        int actual;

        public LaytonTalks(string[] txts, Bitmap[] layton, Bitmap fondo)
        {
            InitializeComponent();

            label1.Parent = pictureBox2;
            label1.Dock = DockStyle.Fill;

            pictureBox2.Image = fondo;
            textos = txts;
            this.layton = layton;
            actual = 0;
            pictureBox1.Image = layton[0];
            label1.Text = "\n" + textos[0];
            timer1.Interval = TextToTime(textos[0]) * 100;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox2.Location = new Point(0, pictureBox1.Height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            actual++;
            if (actual >= textos.Length) actual = 0;
            label1.Text = "\n" +  (textos[actual][0] == '@' ? textos[actual].Remove(0, 1) : textos[actual]);
            
            if (textos[actual][0] == '@')
                pictureBox1.Image = layton[actual];
        }

        private int TextToTime(string texto)
        {
            int palabras = texto.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;

            return palabras * 4;
        }

    }
}
