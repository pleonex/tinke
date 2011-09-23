using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace Common
{
    public partial class BasicControl : UserControl
    {
        String image;
        IPluginHost pluginHost;

        public BasicControl()
        {
            InitializeComponent();
        }
        public BasicControl(Bitmap imagen, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            ReadLanguage();

            picBox.Image = imagen;
        }
        public BasicControl(String imagen, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            ReadLanguage();

            image = Path.GetTempFileName();
            File.Copy(imagen, image, true);
            picBox.ImageLocation = image;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "CommonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("BasicControl");

                btnSave.Text = xml.Element("S00").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = "png";
            o.Filter = "PNG graphic (*.png)|*.png";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                picBox.Image.Save(o.FileName);
        }
    }
}
