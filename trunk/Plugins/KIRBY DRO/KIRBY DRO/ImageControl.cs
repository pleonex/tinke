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
            ReadLanguage();

            picBox.Image = image;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "KIRBY DROLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("ImageControl");

                btnSave.Text = xml.Element("S00").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
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
