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
using System.Windows.Forms;
using Ekona;

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
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + 
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
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
