// ----------------------------------------------------------------------
// <copyright file="SelectModeColor.cs" company="none">

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
// <date>28/04/2012 14:27:56</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Tinke.Dialog
{
    public partial class SelectModeColor : Form
    {
        public SelectModeColor()
        {
            InitializeComponent();
            ReadLanguage();
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.GetTranslation("Dialog");

                this.Text = xml.Element("S00").Value;
                btnOK.Text = xml.Element("S01").Value;
                radioButton1.Text = xml.Element("S02").Value;
                radioButton2.Text = xml.Element("S03").Value;
                radioButton3.Text = xml.Element("S1D").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public int Option
        {
            get
            {
                if (radioButton1.Checked)
                    return 1;
                else if (radioButton2.Checked)
                    return 2;
                else if (radioButton3.Checked)
                    return 3;
                else
                    return 0;
            }
            set
            {
                switch (value)
                {
                    case 1:
                        radioButton1.Checked = true;
                        break;
                    case 2:
                        radioButton2.Checked = true;
                        break;
                    case 3:
                        radioButton3.Checked = true;
                        break;
                }
            }
        }
    }
}
