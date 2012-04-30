// ----------------------------------------------------------------------
// <copyright file="MapOptions.cs" company="none">

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
// <date>28/04/2012 14:27:14</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke.Dialog
{
    public partial class MapOptions : Form
    {

        public MapOptions()
        {
            InitializeComponent();
        }
        public MapOptions(int width, int height)
        {
            InitializeComponent();
            ReadLanguage();

            numericWidth.Value = width;
            numericHeight.Value = height;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("NSCR");

                this.Text = xml.Element("S00").Value;
                label1.Text = xml.Element("S01").Value;
                label2.Text = xml.Element("S02").Value;
                checkFillTile.Text = xml.Element("S03").Value;
                label3.Text = xml.Element("S04").Value;
                label4.Text = xml.Element("S05").Value;
                btnOk.Text = xml.Element("S06").Value;
                label5.Text = xml.Element("S07").Value;
                label6.Text = xml.Element("S08").Value;
                checkSubImage.Text = xml.Element("S09").Value;
                groupSubImages.Text = xml.Element("S09").Value;
                label7.Text = xml.Element("S0A").Value;
                label8.Text = xml.Element("S0B").Value;
                btnCancel.Text = xml.Element("S0C").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        public int ImagenWidth
        {
            get { return (int)numericWidth.Value; }
        }
        public int ImageHeight
        {
            get { return (int)numericHeight.Value; }
        }
        public bool FillTiles
        {
            get { return checkFillTile.Checked; }
        }
        public int StartFillTiles
        {
            get { return (int)numericStartTile.Value; }
        }
        public int FillTilesWith
        {
            get { return (int)numericFillTile.Value; }
        }
        public bool SubImages
        {
            get { return checkSubImage.Checked; }
        }
        public int SubImagesStart
        {
            get { return (int)numericSubStart.Value; }
        }
        public int SubPalette
        {
            get { return (int)numericSubPalette.Value; }
            set { numericSubPalette.Value = value; }
        }

        private void checkFillTile_CheckedChanged(object sender, EventArgs e)
        {
            groupFill.Enabled = checkFillTile.Checked;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void numericMaxSize_ValueChanged(object sender, EventArgs e)
        {
            numericStartTile.Value = (numericMaxHeight.Value * numericMaxWidth.Value) / 64;
        }
        private void numericStartTile_ValueChanged(object sender, EventArgs e)
        {
            numericMaxHeight.Value = (numericStartTile.Value * 64) / numericWidth.Value;
        }

        private void checkSubImage_CheckedChanged(object sender, EventArgs e)
        {
            groupSubImages.Enabled = checkSubImage.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
