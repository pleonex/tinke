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

        public iNCLR()
        {
            InitializeComponent();
        }
        public iNCLR(NCLR paleta)
        {
            InitializeComponent();

            this.paleta = paleta;
            ShowInfo();

            paletas = Imagen_NCLR.Mostrar(paleta);
            paletaBox.Image = paletas[0];
            nPaleta.Maximum = paletas.Length;
            nPaleta.Minimum = 1;
            nPaleta.Value = 1;
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
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
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
                lbl.Text = "Paleta " + (i + 1).ToString();
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

            ven.Text = "Paletas";
            ven.AutoScroll = true;
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
            o.AutoUpgradeEnabled = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Imagen Portable Network Graphics (*.png)|*.png";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                paletaBox.Image.Save(o.FileName);
        }
    }
}
