/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;

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
