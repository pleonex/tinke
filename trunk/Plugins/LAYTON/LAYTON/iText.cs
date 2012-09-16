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
using System.IO;
using System.Windows.Forms;
using Ekona;

namespace LAYTON
{
    public partial class iText : UserControl
    {
        IPluginHost pluginHost;
        int id;

        public iText(IPluginHost pluginHost, string text, int id)
        {
            InitializeComponent();
            this.pluginHost = pluginHost; ;
            this.id = id;
            LeerIdioma();

            text = Convertir_Especiales(text, true);
            txtBox.Text = text;
        }
        private void LeerIdioma()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "LaytonLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("Text");
                btnSave.Text = xml.Element("S00").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private string Convertir_Especiales(string txtin, bool convertir)
        {
            if (convertir)
            {
                txtin = txtin.Replace("\x0A", "\r\n");
                txtin = txtin.Replace("<`a>", "à");
                txtin = txtin.Replace("<'a>", "á");
                txtin = txtin.Replace("<:a>", "ä");
                txtin = txtin.Replace("<^a>", "â");
                txtin = txtin.Replace("<`e>", "è");
                txtin = txtin.Replace("<'e>", "é");
                txtin = txtin.Replace("<^e>", "ê");
                txtin = txtin.Replace("<:e>", "ë");
                txtin = txtin.Replace("<`i>", "ì");
                txtin = txtin.Replace("<'i>", "í");
                txtin = txtin.Replace("<^i>", "î");
                txtin = txtin.Replace("<:i>", "ï");
                txtin = txtin.Replace("<`o>", "ò");
                txtin = txtin.Replace("<'o>", "ó");
                txtin = txtin.Replace("<^o>", "ô");
                txtin = txtin.Replace("<:o>", "ö");
                txtin = txtin.Replace("<oe>", "œ");
                txtin = txtin.Replace("<`u>", "ù");
                txtin = txtin.Replace("<'u>", "ú");
                txtin = txtin.Replace("<^u>", "û");
                txtin = txtin.Replace("<:u>", "ü");
                txtin = txtin.Replace("<,c>", "ç");
                txtin = txtin.Replace("<~n>", "ñ");
                txtin = txtin.Replace("<-n>", "ñ");
                //txtin = txtin.Replace("<ss>", ""); Desconocida la equivalencia
                txtin = txtin.Replace("<`A>", "À");
                txtin = txtin.Replace("<'A>", "Á");
                txtin = txtin.Replace("<~A>", "Ã");
                txtin = txtin.Replace("<-A>", "Ã");
                txtin = txtin.Replace("<^A>", "Â");
                txtin = txtin.Replace("<:A>", "Ä");
                txtin = txtin.Replace("<`E>", "È");
                txtin = txtin.Replace("<'E>", "É");
                txtin = txtin.Replace("<^E>", "Ê");
                txtin = txtin.Replace("<:E>", "Ë");
                txtin = txtin.Replace("<`I>", "Ì");
                txtin = txtin.Replace("<'I>", "Í");
                txtin = txtin.Replace("<^I>", "Î");
                txtin = txtin.Replace("<:I>", "Ï");
                txtin = txtin.Replace("<`O>", "Ò");
                txtin = txtin.Replace("<'O>", "Ó");
                txtin = txtin.Replace("<^O>", "Ô");
                txtin = txtin.Replace("<:O>", "Ö");
                txtin = txtin.Replace("<OE>", "Œ");
                txtin = txtin.Replace("<`U>", "Ù");
                txtin = txtin.Replace("<'U>", "Ú");
                txtin = txtin.Replace("<^U>", "Û");
                txtin = txtin.Replace("<:U>", "Ü");
                txtin = txtin.Replace("<,C>", "Ç");
                txtin = txtin.Replace("<~N>", "Ñ");
                txtin = txtin.Replace("<-N>", "Ñ");
                txtin = txtin.Replace("<^!>", "¡");
                txtin = txtin.Replace("<^?>", "¿");
                //txtin = txtin.Replace("<a>", ""); Desconocida la equivalencia
                //txtin = txtin.Replace("<0>", ""); Desconocida la equivalencia
            }
            else
            {
                txtin = txtin.Replace("\r\n", "\x0A");
                txtin = txtin.Replace("à", "<`a>");
                txtin = txtin.Replace("á", "<'a>");
                txtin = txtin.Replace("ä", "<:a>");
                txtin = txtin.Replace("è", "<`e>");
                txtin = txtin.Replace("é", "<'e>");
                txtin = txtin.Replace("ê", "<^e>");
                txtin = txtin.Replace("ë", "<:e>");
                txtin = txtin.Replace("ì", "<`i>");
                txtin = txtin.Replace("í", "<'i>");
                txtin = txtin.Replace("î", "<^i>");
                txtin = txtin.Replace("ï", "<:i>");
                txtin = txtin.Replace("ò", "<`o>");
                txtin = txtin.Replace("ó", "<'o>");
                txtin = txtin.Replace("ô", "<^o>");
                txtin = txtin.Replace("ö", "<:o>");
                txtin = txtin.Replace("œ", "<oe>");
                txtin = txtin.Replace("ù", "<`u>");
                txtin = txtin.Replace("ú", "<'u>");
                txtin = txtin.Replace("û", "<^u>");
                txtin = txtin.Replace("ü", "<:u>");
                txtin = txtin.Replace("ç", "<,c>");
                txtin = txtin.Replace("ñ", "<~n>");
                //txtin = txtin.Replace("", "<ss>"); Desconocida la equivalencia
                txtin = txtin.Replace("À", "<`A>");
                txtin = txtin.Replace("Á", "<'A>");
                txtin = txtin.Replace("Ã", "<~A>");
                txtin = txtin.Replace("Â", "<^A>");
                txtin = txtin.Replace("Ä", "<:A>");
                txtin = txtin.Replace("È", "<`E>");
                txtin = txtin.Replace("É", "<'E>");
                txtin = txtin.Replace("Ê", "<^E>");
                txtin = txtin.Replace("Ë", "<:E>");
                txtin = txtin.Replace("Ì", "<`I>");
                txtin = txtin.Replace("Í", "<'I>");
                txtin = txtin.Replace("Î", "<^I>");
                txtin = txtin.Replace("Ï", "<:I>");
                txtin = txtin.Replace("Ò", "<`O>");
                txtin = txtin.Replace("Ó", "<'O>");
                txtin = txtin.Replace("Ô", "<^O>");
                txtin = txtin.Replace("Ö", "<:O>");
                txtin = txtin.Replace("Œ", "<OE>");
                txtin = txtin.Replace("Ù", "<`U>");
                txtin = txtin.Replace("Ú", "<'U>");
                txtin = txtin.Replace("Û", "<^U>");
                txtin = txtin.Replace("Ü", "<:U>");
                txtin = txtin.Replace("Ç", "<,C>");
                txtin = txtin.Replace("Ñ", "<~N>");
                txtin = txtin.Replace("¡", "<^!>");
                txtin = txtin.Replace("¿", "<^¿>");
                //txtin = txtin.Replace("", "<a>"); Desconocida la equivalencia
                //txtin = txtin.Replace("", "<0>"); Desconocida la equivalencia
            }
            return txtin;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string text = ""; // Makes the compiler happy (siempre deseé poner eso :) )
            text = Convertir_Especiales(txtBox.Text,false);

            string tempFile = pluginHost.Get_TempFile();
            File.WriteAllText(tempFile, text);
            pluginHost.ChangeFile(id, tempFile);
        }
    }
}
