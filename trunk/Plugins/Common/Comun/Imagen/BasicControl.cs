using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Comun
{
    public partial class BasicControl : UserControl
    {
        String image;

        public BasicControl()
        {
            InitializeComponent();
        }
        public BasicControl(Bitmap imagen)
        {
            InitializeComponent();

            picBox.Image = imagen;
        }
        public BasicControl(String imagen)
        {
            InitializeComponent();

            image = Path.GetTempFileName();
            File.Copy(imagen, image, true);
            picBox.ImageLocation = image;
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
