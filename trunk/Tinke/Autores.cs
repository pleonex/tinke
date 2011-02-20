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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 20/02/2011
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Tinke
{
    partial class Autores : Form
    {
        public Autores()
        {
            InitializeComponent();
            this.Text = String.Format("Acerca de {0}", AssemblyTitle);
            
            this.label1.Text = "Tinke Versión " + AssemblyVersion;
            this.label2.Text = "Programado por:";
            this.label4.Text = String.Format("Traducción al {0} por {1}", "español", "pleoNeX");
            this.label5.Text = "Este programa se encuentra bajo los\ntérminos de la licencia GPL V3";

            lblDescription.Text = "Este programa se ha realizado gracias a la información y a las herramientas de código" +
                                    "\nabierto encontradas en las siguientes páginas.";
            lblDSDecmp2.Text = "Librería de descompresión de formatos LZ77 (0x10), LZSS (0x11)," +
                                "\nLZSS (0x40), Huffman (0x20), RLE (0x30) y 'overlays'." +
                                "\nAutor: barubary";
            lblGBATEK.Text = "Información técnica sobre estructura de las roms";
            lblLowLines.Text = "Información sobre estructura de formatos de Nintendo";
        }

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void lblDSDecmp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/dsdecmp/");
        }
        private void linkLowLines_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://llref.emutalk.net/nds_formats.htm");
        }
        private void linkGBATEK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://nocash.emubase.de/gbatek.htm");
        }
    }
}
