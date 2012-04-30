// ----------------------------------------------------------------------
// <copyright file="Espera.cs" company="none">

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
// <date>28/04/2012 14:24:51</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke
{
    public partial class Espera : Form
    {
        public Espera()
        {
            InitializeComponent();
            LeerIdioma();
        }
        public Espera(string label, bool step)
        {
            InitializeComponent();

            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Espera");
            this.Text = xml.Element("S01").Value;
            label1.Text = xml.Element(label).Value;
            
            if (step)
                progressBar1.Style = ProgressBarStyle.Continuous;

        }
        public Espera(string label, int step)
        {
            InitializeComponent();

            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Espera");
            this.Text = xml.Element("S01").Value;
            label1.Text = xml.Element(label).Value;

            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Step = step;
        }

        public void Set_ProgressValue(int porcentaje)
        {
            progressBar1.Value = porcentaje;
        }
        public void Step()
        {
            progressBar1.PerformStep();
        }

        private void LeerIdioma()
        {
                System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Espera");

                this.Text = xml.Element("S01").Value;
                label1.Text = xml.Element("S01").Value;
        }
    }
}
