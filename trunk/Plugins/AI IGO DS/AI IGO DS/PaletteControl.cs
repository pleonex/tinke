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
    public partial class PaletteControl : UserControl
    {
        NCLR paleta;
        IPluginHost pluginHost;

        public PaletteControl()
        {
            InitializeComponent();
        }
        public PaletteControl(IPluginHost pluginHost)
        {
            InitializeComponent();
            this.paleta = pluginHost.Get_NCLR();
            this.pluginHost = pluginHost;

            picPaleta.Image = pluginHost.Bitmaps_NCLR(paleta)[0];
        }
    }
}
