using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace KIRBY_DRO
{
    public partial class ImageControl : UserControl
    {
        IPluginHost pluginHost;

        public ImageControl()
        {
            InitializeComponent();
        }
        public ImageControl(IPluginHost pluginHost, Bitmap image)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            picBox.Image = image;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                picBox.Image.Save(o.FileName);
            o.Dispose();
        }
    }
}
