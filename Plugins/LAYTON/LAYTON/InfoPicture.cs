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
using System.IO;

namespace LAYTON
{
    public partial class InfoPicture : UserControl
    {
        #region Variables
        InfoImage[] infoImag;
        TabPage[] tabImag;
        Bitmap[] imagenes;
        string idioma;
        string pestaña;        
        #endregion

        // Constructor
        public InfoPicture()
        {
            InitializeComponent();
            idioma = "Español";
        }
        public void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(idioma).Element("InfoPicture");

                groupImage.Text = xml.Element("S01").Value;
                lblTipo.Text = xml.Element("S02").Value;
                lblNImgs.Text = xml.Element("S03").Value;
                groupBox1.Text = xml.Element("S04").Value;
                pestaña = xml.Element("S05").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        public string Idioma
        {
            set { idioma = value; LeerIdioma(); }
        }
        public string Tipo
        {
            set { txtTipo.Text = value; }
            get { return txtTipo.Text; }
        }
        public ushort Numero_Imagen
        {
            set 
            { 
                txtNImgs.Text = value.ToString();
                infoImag = new InfoImage[value];
                tabImag = new TabPage[value];

                for (int i = 0; i < value; i++)
                {
                    infoImag[i] = new InfoImage(idioma);
                    tabImag[i] = new TabPage();
                    tabImag[i].Text = pestaña + ' ' + i.ToString();
                    tabImag[i].Controls.Add(infoImag[i]);
                }

                while (tabImags.Controls.Count != 0) // Elimina datos de archivos anteriores
                {
                    tabImags.Controls.RemoveAt(0);
                }
                tabImags.Controls.AddRange(tabImag);
            }
            get { return Convert.ToUInt16(txtNImgs.Text); }
        }
        public Ani.Todo Informacion
        {
            set
            {
                Numero_Imagen = value.imgs;
                Tipo = value.imagenes[0].tipo == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp";
                for (int i = 0; i < value.imgs; i++)
                    infoImag[i].Informacion = value.imagenes[i];
            }
        }
        public int Imagen_Seleccionada
        {
            set
            {
                if (tabImags.SelectedIndex == value)
                    pictureBox1.Image = imagenes[value];
                else tabImags.SelectedIndex = value;
            }
            get
            {
                return tabImags.SelectedIndex;
            }
        }

        public Bitmap[] Imagenes
        {
            set
            {
                this.imagenes = value;
            }
        }

        public void UpdateSelected(Ani.Image image)
        {
            int i = tabImags.SelectedIndex;
            infoImag[i] = new InfoImage(idioma);
            tabImag[i].Text = pestaña + ' ' + i.ToString();
            tabImag[i].Controls.Add(infoImag[i]);
        }

        private void tabImags_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabImags.SelectedIndex >= 0) pictureBox1.Image = imagenes[tabImags.SelectedIndex];
        }
    }
}
