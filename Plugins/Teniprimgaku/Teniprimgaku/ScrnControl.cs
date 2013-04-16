// ----------------------------------------------------------------------
// <copyright file="ImageControl.cs" company="none">
// Copyright (C) 2013
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
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
//
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>25/03/2013 1:13:43</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Teniprimgaku
{
    public partial class ScrnControl : UserControl
    {
        private Scrn scrn;

        public ScrnControl()
        {
            InitializeComponent();
        }
        public ScrnControl(Scrn scrn)
        {
            InitializeComponent();

            this.scrn = scrn;
            this.lblTotal.Text = string.Format("of {0}", this.scrn.NumberImages - 1);
            this.numImgs.Maximum = this.scrn.NumberImages - 1;
            this.UpdateImage(0);

            if (this.scrn.NumberImages == 1)
            {
                this.picBox2.Hide();
                this.label3.Hide();
                this.panel1.Size = new Size(367, 489);
            }
        }

        public void UpdateImage(int index)
        {
            this.picBox.Image = scrn[index].GetImage();
            if (scrn.NumberImages > index + 1)
            {
                this.picBox2.Image = scrn[index + 1].GetImage();
            }
            else
            {
                this.picBox2.Image = null;
            }
        }

        private void numImgs_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateImage((int)this.numImgs.Value);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.AddExtension = true;
                sfd.DefaultExt = ".png";
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(this.scrn.Name) + ".png";
                sfd.Filter = "Portable Network Graphic (*.png)|*.png";
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    this.scrn[(int)this.numImgs.Value].GetImage().Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}
