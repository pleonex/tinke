using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Comun
{
    public partial class iWav : UserControl
    {
        System.Media.SoundPlayer snd;
        long pos;

        public iWav(string archivo)
        {
            InitializeComponent();

            FileStream fs = new FileStream(archivo, FileMode.Open);
            snd = new System.Media.SoundPlayer(fs);
            snd.Load();
            fs.Close();
            fs.Dispose();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            snd.Play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            snd.Stop();
        }
    }
}
