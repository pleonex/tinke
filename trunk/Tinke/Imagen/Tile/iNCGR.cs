using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke.Imagen.Tile
{
    public partial class iNCGR : UserControl
    {
        Imagen.Paleta.Estructuras.NCLR paleta;
        Imagen.Tile.Estructuras.NCGR tile;
        int startTile;

        public iNCGR()
        {
            InitializeComponent();
        }
        public iNCGR(Imagen.Tile.Estructuras.NCGR tile, Imagen.Paleta.Estructuras.NCLR paleta)
        {
            InitializeComponent();

            this.paleta = paleta;
            this.tile = tile;
            if (tile.rahc.nTilesX != 0xFFFF)
                this.numericWidth.Value = tile.rahc.nTilesX * 8;
            else
                this.numericWidth.Value = 0x100;
            if (tile.rahc.nTilesY != 0xFFFF)
                this.numericHeight.Value = tile.rahc.nTilesY * 8;
            else
                this.numericHeight.Value = 0x100;
            this.numericWidth.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericHeight.ValueChanged += new EventHandler(numericSize_ValueChanged);
            this.numericStart.ValueChanged += new EventHandler(numericStart_ValueChanged);
            pic.Image = Imagen.Tile.NCGR.Crear_Imagen(tile, paleta, 0);
            Info();
        }

        void numericStart_ValueChanged(object sender, EventArgs e)
        {
            startTile = (int)numericStart.Value;
            pic.Image = Imagen.Tile.NCGR.Crear_Imagen(tile, paleta, startTile);
        }

        private void iNCGR_SizeChanged(object sender, EventArgs e)
        {
            pic.Location = new Point(0, 0);
            groupProp.Location = new Point(this.Width - groupProp.Width, 0);
            groupProp.Height = this.Height - btnSave.Height - 10;
            listInfo.Height = groupProp.Height - 78;
            label1.Location = new Point(label1.Location.X, listInfo.Height + 54);
            label2.Location = new Point(label2.Location.X, listInfo.Height + 54);
            label3.Location = new Point(label3.Location.X, listInfo.Height + 28);
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
            pic.Image = Imagen.Tile.NCGR.Crear_Imagen(tile, paleta, startTile);
        }
        private void Info()
        {
            listInfo.Items[0].SubItems.Add("0x" + String.Format("{0:X}", tile.constant));
            listInfo.Items[1].SubItems.Add(tile.nSections.ToString());
            listInfo.Items[2].SubItems.Add(new String(tile.rahc.id));
            listInfo.Items[3].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.size_section));
            listInfo.Items[4].SubItems.Add(tile.rahc.nTilesY.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesY) + ')');
            listInfo.Items[5].SubItems.Add(tile.rahc.nTilesX.ToString() + " (0x" + String.Format("{0:X}", tile.rahc.nTilesX) + ')');
            listInfo.Items[6].SubItems.Add(Enum.GetName(tile.rahc.depth.GetType(), tile.rahc.depth));
            listInfo.Items[7].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown1));
            listInfo.Items[8].SubItems.Add("0x" + String.Format("{0:X}", tile.rahc.unknown2));
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
    }
}
