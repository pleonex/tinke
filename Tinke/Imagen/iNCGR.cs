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
    public partial class iNCGR : UserControl
    {
        NCLR paleta;
        NCGR tile;
        int startTile;

        public iNCGR()
        {
            InitializeComponent();
        }
        public iNCGR(NCGR tile, NCLR paleta)
        {
            InitializeComponent();

            this.paleta = paleta;
            this.tile = tile;
            pic.Image = Imagen_NCGR.Crear_Imagen(tile, paleta, 0);
            this.numericWidth.Value = pic.Image.Width;
            this.numericHeight.Value = pic.Image.Height;
            this.comboDepth.Text = (tile.rahc.depth == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp");
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);

            Info();
        }

        void numericStart_ValueChanged(object sender, EventArgs e)
        {
            startTile = (int)numericStart.Value;
            pic.Image = Imagen_NCGR.Crear_Imagen(tile, paleta, startTile);
        }

        private void iNCGR_SizeChanged(object sender, EventArgs e)
        {
            pic.Location = new Point(0, 5);
            groupProp.Location = new Point(this.Width - groupProp.Width, 5);
            groupProp.Height = this.Height - btnSave.Height - 10;
            listInfo.Height = groupProp.Height - 78;
            label1.Location = new Point(label1.Location.X, listInfo.Height + 54);
            label2.Location = new Point(label2.Location.X, listInfo.Height + 54);
            label3.Location = new Point(label3.Location.X, listInfo.Height + 28);
            label4.Location = new Point(label4.Location.X, listInfo.Height + 28);
            comboDepth.Location = new Point(comboDepth.Location.X, listInfo.Height + 26);
            numericStart.Location = new Point(numericStart.Location.X, listInfo.Height + 26);
            numericHeight.Location = new Point(numericHeight.Location.X, listInfo.Height + 52);
            numericWidth.Location = new Point(numericWidth.Location.X, listInfo.Height + 52);
            btnSave.Location = new Point(this.Width - btnSave.Width, groupProp.Height + 5);
        }

        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            Actualizar_Imagen();
        }
        private void Actualizar_Imagen()
        {
            tile.rahc.nTilesX = (ushort)(numericWidth.Value / 8);
            tile.rahc.nTilesY = (ushort)(numericHeight.Value / 8);
            tile.rahc.depth = (comboDepth.Text == "4 bpp" ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            pic.Image = Imagen_NCGR.Crear_Imagen(tile, paleta, startTile);
        }
        private void Info()
        {
            listInfo.Items[0].SubItems.Add("0x" + String.Format("{0:X}", tile.cabecera.constant));
            listInfo.Items[1].SubItems.Add(tile.cabecera.nSection.ToString());
            listInfo.Items[2].SubItems.Add(new String(tile.rahc.id));
            listInfo.Items[3].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.size_section));
            listInfo.Items[4].SubItems.Add(tile.rahc.nTilesY.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesY) + ')');
            listInfo.Items[5].SubItems.Add(tile.rahc.nTilesX.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesX) + ')');
            listInfo.Items[6].SubItems.Add(Enum.GetName(tile.rahc.depth.GetType(), tile.rahc.depth));
            listInfo.Items[7].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown1));
            listInfo.Items[8].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.padding));
            listInfo.Items[9].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.size_tiledata));
            listInfo.Items[10].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown3));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = "bmp";
            o.Filter = "Imagen BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                pic.Image.Save(o.FileName);
            o.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tile.rahc.depth = (comboDepth.Text == "4 bpp" ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit);
            // TODO: Crear método en clase Convertir para convertir de 4bpp a 8bpp y viceversa
            Actualizar_Imagen();
        }
    }
}
