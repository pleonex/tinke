// ----------------------------------------------------------------------
// <copyright file="TMAPcontrol.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>02/09/2012 3:53:19</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;

namespace NINOKUNI
{
    public partial class TMAPcontrol : UserControl
    {
        TMAP tmap;
        IPluginHost pluginHost;

        public TMAPcontrol()
        {
            InitializeComponent();
        }
        public TMAPcontrol(sFile cfile, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            tmap = new TMAP(cfile, pluginHost);
            numLayer_ValueChanged(null, null);
        }
        ~TMAPcontrol()
        {
            tmap = null;
        }

        private void numLayer_ValueChanged(object sender, EventArgs e)
        {
            picMap.Image = tmap.Get_Map((int)numLayer.Value);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".png";
            o.FileName = tmap.FileName + ".png";
            o.Filter = "Portable Net Graphic (*.png)|*.png";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            picMap.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);

            o.Dispose();
            o = null;
        }

        private void numPicMode_ValueChanged(object sender, EventArgs e)
        {
            picMap.SizeMode = (PictureBoxSizeMode)numPicMode.Value;
        }
    }
}
