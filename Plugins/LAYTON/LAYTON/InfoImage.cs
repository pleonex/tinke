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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    public partial class InfoImage : UserControl
    {
        byte[][] datos;         // Array de datos de la array de partes.
        InfoParte[] infoPartes;
        TabPage[] partes;
        string idioma;
        string pestaña;

        public InfoImage()
        {
            InitializeComponent();
        }
        public InfoImage(string idioma)
        {
            InitializeComponent();
            this.idioma = idioma;

            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(idioma).Element("InfoImage");

                lblImgs.Text = xml.Element("S01").Value;
                lblTamanoImg.Text = xml.Element("S02").Value;
                lblAncho.Text = xml.Element("S03").Value;
                lblAlto.Text = xml.Element("S04").Value;
                lblNPartes.Text = xml.Element("S05").Value;
                lblName.Text = xml.Element("S06").Value;
                groupBox1.Text = xml.Element("S07").Value;
                pestaña = xml.Element("S08").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        #region Propiedades
        public Ani.Image Informacion
        {
            set
            {
                Imagenes = value.imgs;
                Tamano_Imagen = value.length;
                Ancho = value.width;
                Alto = value.height;
                NPartes = value.imgs;
                Partes = value.segmentos;
            }
            get
            {
                Ani.Image temp = new Ani.Image();
                temp.imgs = (ushort)Imagenes;
                temp.length = Tamano_Imagen;
                temp.width = (ushort)Ancho;
                temp.height = (ushort)Alto;
                temp.segmentos = Partes;
                return temp;
            }
        }
        public string Nombre
        {
            set { txtName.Text = value; }
            get { return txtName.Text; }
        }
        public uint Imagenes
        {
            set { txtImgs.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtImgs.Text); }
        }
        public uint Tamano_Imagen
        {
            set { txtTamanoImg.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtTamanoImg.Text); }
        }
        public uint Ancho
        {
            set { txtAncho.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtAncho.Text); }
        }
        public uint Alto
        {
            set { txtAlto.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtAlto.Text); }
        }
        public uint NPartes
        {
            set
            {
                txtNPartes.Text = value.ToString();
                infoPartes = new InfoParte[value];
                partes = new TabPage[value];

                for (int i = 0; i < value; i++)
                {
                    infoPartes[i] = new InfoParte(idioma);
                    partes[i] = new TabPage();
                    partes[i].Text = pestaña + ' ' + i.ToString();
                    partes[i].Controls.Add(infoPartes[i]);
                }

                while (tabPartes.Controls.Count != 0)
                {
                    tabPartes.Controls.RemoveAt(0);
                }
                tabPartes.Controls.AddRange(partes);
            }
            get { return Convert.ToUInt32(txtNPartes.Text); }
        }
        public Ani.Parte[] Partes
        {
            set
            {
                datos = new byte[infoPartes.Length][];

                for (int i = 0; i < infoPartes.Length; i++)
                {
                    infoPartes[i].Informacion = value[i];
                    datos[i] = value[i].datos;
                }
            }
            get
            {
                Ani.Parte[] temp = new Ani.Parte[infoPartes.Length];

                for (int i = 0; i < infoPartes.Length; i++)
                {
                    temp[i] = infoPartes[i].Informacion;
                    temp[i].datos = datos[i];
                }

                return temp;
            }
        }
        #endregion
    }
}
