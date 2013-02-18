using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;
using System.IO;

namespace NINOKUNI
{
    public partial class BMPControl : UserControl
    {
        IPluginHost pluginHost;
        int id;

        public BMPControl()
        {
            InitializeComponent();
        }
        public BMPControl(string image, int id, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = id;

            string imagePath = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "bmp_" + Path.GetFileName(image);
            File.Copy(image, imagePath, true);
            picBox.ImageLocation = imagePath;

            //ReadLanguage();
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "NINOKUNILang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("BMPControl");

                btnSave.Text = xml.Element("S00").Value;
                btnImport.Text = xml.Element("S01").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.OverwritePrompt = true;
            o.Filter = "BitMaP image (*.bmp)|*.bmp|" +
                       "PNG image (*.png)|*.png";
            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FileName.ToUpper().EndsWith(".BMP"))
                    picBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else
                    picBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.Filter = "BitMaP image (*.bmp)|*.bmp|" +
                       "PNG image (*.png)|*.png";
            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FileName.ToUpper().EndsWith(".BMP"))
                {
                    pluginHost.ChangeFile(id, o.FileName);
                    picBox.ImageLocation = o.FileName;
                }
                else
                {
                    string tempFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "newBmp_" + Path.GetFileName(picBox.ImageLocation);
                    Image.FromFile(o.FileName).Save(tempFile, System.Drawing.Imaging.ImageFormat.Bmp);
                    pluginHost.ChangeFile(id, tempFile);
                    picBox.ImageLocation = tempFile;
                }

            }
        }
    }
}
