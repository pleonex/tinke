using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace AI_IGO_DS
{
    public partial class ImageControl : UserControl
    {
        NCGR tiles;
        NCLR paleta;
        NSCR map;
        IPluginHost pluginHost;
        bool isMap;

        public ImageControl()
        {
            InitializeComponent();
        }
        public ImageControl(IPluginHost pluginHost, bool isMap)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            paleta = pluginHost.Get_NCLR();
            tiles = pluginHost.Get_NCGR();
            this.isMap = isMap;

            if (isMap)
            {
                map = pluginHost.Get_NSCR();
                tiles.rahc.tileData = pluginHost.Transformar_NSCR(map, tiles.rahc.tileData);
                picImage.Image = pluginHost.Bitmap_NCGR(tiles, paleta);
            }
            else
                picImage.Image = pluginHost.Bitmap_NCGR(tiles, paleta);

            numericH.Value = tiles.rahc.nTilesY * 8;
            numericW.Value = tiles.rahc.nTilesX * 8;
        }

        private void numeric_ValueChanged(object sender, EventArgs e)
        {
            if (numericW.Value == 0x00 || numericH.Value == 0x00)
                return;

            int nBloques = (int)(tiles.rahc.size_tiledata * 2 / numericH.Value / numericW.Value);
            tiles.rahc.nTilesX = (ushort)(numericW.Value / 8 * nBloques);
            tiles.rahc.nTilesY = (ushort)(numericH.Value / 8);

            Bitmap bitIma = new Bitmap((int)numericW.Value * nBloques, (int)numericH.Value);
            Graphics graphic = Graphics.FromImage(bitIma);

            for (int i = 0; i < nBloques; i++)
            {
                graphic.DrawImage(pluginHost.Bitmap_NCGR(tiles, paleta, (int)(i * numericW.Value * numericH.Value * 64 + (numericTile.Value / 64)),
                    (int)(numericW.Value / 8), (int)(numericH.Value / 8)),
                    (int)(i * numericW.Value), 0);
            }
            picImage.Image = bitIma;
        }
    }
}
