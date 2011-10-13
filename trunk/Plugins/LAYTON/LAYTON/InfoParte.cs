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

namespace LAYTON
{
    public partial class InfoParte : UserControl
    {
        public InfoParte()
        {
            InitializeComponent();
        }
        public InfoParte(string idioma)
        {
            InitializeComponent();

            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(idioma).Element("InfoParte");

                lblPos.Text = xml.Element("S01").Value;
                lblTamanoP.Text = xml.Element("S02").Value;
                lblAnchoP.Text = xml.Element("S03").Value;
                lblAltoP.Text = xml.Element("S04").Value;
                lblPosX.Text = xml.Element("S05").Value;
                lblPosY.Text = xml.Element("S06").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        public Ani.Parte Informacion
        {
            set
            {
                txtAltoP.Text = value.height.ToString();
                txtAnchoP.Text = value.width.ToString();
                txtPos.Text = value.offSet.ToString();
                txtPosX.Text = value.posX.ToString();
                txtPosY.Text = value.posY.ToString();
                txtTamanoP.Text = value.length.ToString();
            }
            get
            {
                Ani.Parte info = new Ani.Parte();

                info.height = Convert.ToUInt16(txtAltoP.Text);
                info.width = Convert.ToUInt16(txtAnchoP.Text);
                info.offSet = Convert.ToUInt64(txtPos.Text);
                info.posX = Convert.ToUInt16(txtPosX.Text);
                info.posY = Convert.ToUInt16(txtPosY.Text);

                return info;
            }
        }
    }
}
