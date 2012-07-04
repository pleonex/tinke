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
using System.IO;

namespace Tinke
{
    partial class Autores : Form
    {
        public Autores()
        {
            InitializeComponent();

            LeerIdioma();
            ReadPlugins();
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
            System.Diagnostics.Process.Start("http://llref.emutalk.net/docs");
        }
        private void linkGBATEK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://nocash.emubase.de/gbatek.htm");
        }
        private void linkfamfamfam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://famfamfam.com/");
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://hexbox.sourceforge.net/");
        }

        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("Autores");

                this.Text = xml.Element("S01").Value + ' ' + AssemblyTitle;

                label1.Text = "Tinke  " + xml.Element("S02").Value + ' ' + AssemblyVersion;
                label2.Text = xml.Element("S03").Value;
                label4.Text = xml.Element("S04").Value;
                lblTrad.Text = xml.Element("S0C").Value;
                label5.Text = xml.Element("S06").Value;
                label6.Text = xml.Element("S05").Value;
                lblDescription.Text = xml.Element("S07").Value;
                lblDSDecmp2.Text = xml.Element("S08").Value + " LZ77 (0x10), LZSS (0x11)," +
                                    "\nLZSS (0x40), Huffman (0x20), RLE (0x30), 'overlays'." +
                                    "\nBy: barubary";
                lblGBATEK.Text = xml.Element("S09").Value;
                lblLowLines.Text = xml.Element("S0A").Value;
                lblfamfamfam.Text = xml.Element("S0B").Value;
                label7.Text = xml.Element("S0D").Value;
                label9.Text = String.Format(xml.Element("S0E").Value, "Bernhard Elbl");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }
        private void ReadPlugins()
        {
            if (!Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins"))
                return;

            foreach (string fileName in Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins", "*.dll"))
            {
                try
                {

                    if (fileName.EndsWith("Ekona.dll"))
                        continue;

                    
                    Assembly assembly = Assembly.LoadFile(fileName);
                    object[] attributes =  assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if (attributes.Length == 0)
                        continue;

                    listPlugin.Items.Add(Path.GetFileNameWithoutExtension(fileName) + "  -->  " +
                        ((AssemblyCopyrightAttribute)attributes[0]).Copyright);
                       
                }
                catch { continue; }
            }

        }



    }
}
