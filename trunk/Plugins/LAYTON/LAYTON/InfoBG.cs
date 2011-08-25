using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
{
    public partial class InfoBG : UserControl
    {
        IPluginHost pluginHost;

        public InfoBG(IPluginHost pluginHost,Bitmap imagen)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            LeerIdioma();

            pic.Image = imagen;
        }
        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + "\\Plugins\\LaytonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("InfoAni");
                btnSave.Text = xml.Element("S03").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;

            if (o.ShowDialog() == DialogResult.OK)
                pic.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
