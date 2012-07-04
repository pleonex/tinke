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
using Ekona;

namespace TXT
{
    public partial class iTXT : UserControl
    {
        IPluginHost pluginHost;
        int id;
        byte[] text;

        public iTXT(byte[] text, IPluginHost pluginHost, int id)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.id = id;
            this.text = text;

            if ((text[0] == 0xFE && text[1] == 0xFF) || (text[0] == 0xFF && text[1] == 0xFE))
            {
                txtBox.Text = Descodificar(Encoding.Unicode);
                comboEncod.SelectedIndex = 1;
            }
            else if (BitConverter.ToUInt16(text, 0) == 0xBBEF)
            {
                txtBox.Text = Descodificar(Encoding.UTF8);
                comboEncod.SelectedIndex = 4;
            }
            else
            {
                txtBox.Text = Descodificar(Encoding.UTF7);
                comboEncod.SelectedIndex = 0;
            }

            txtBox.Text = txtBox.Text.Replace("\n", "\r\n");
            txtBox.Text = txtBox.Text.Replace("\0", "\\0");

            ReadLanguage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string tempFile = Path.GetTempFileName();

            string textSave = txtBox.Text;
            textSave = textSave.Replace("\\0", "\0");
            text = Encoding.GetEncoding(comboEncod.Text).GetBytes(textSave);
            File.WriteAllBytes(tempFile, text);
            pluginHost.ChangeFile(id, tempFile);
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "TXTLang.xml");
                xml = xml.Element(pluginHost.Get_Language());
                xml = xml.Element("TXT");

                btnSave.Text = xml.Element("S00").Value;
                label1.Text = xml.Element("S01").Value;
                checkWordWrap.Text = xml.Element("S02").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        private String Descodificar(Encoding encoding)
        {
            String texto = new String(encoding.GetChars(text));
            texto = texto.Replace("\n", "\r\n");
            texto = texto.Replace("\0", "\\0");

            return texto;
        }
        private void comboEncod_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBox.Text = Descodificar(Encoding.GetEncoding(comboEncod.Text));
        }

        private void checkWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            txtBox.WordWrap = checkWordWrap.Checked;
        }
    }
}
