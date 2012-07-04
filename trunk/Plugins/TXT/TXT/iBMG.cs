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
using Ekona;

namespace TXT
{
    public partial class iBMG : UserControl
    {
        IPluginHost pluginHost;
        sBMG bmg;
        string[] msg;
        string[] traducciones;

        public iBMG(IPluginHost pluginHost, sBMG bmg)
        {
            this.pluginHost = pluginHost;
            InitializeComponent();
            ReadLanguage();

            this.bmg = bmg;
            Informacion();
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + 
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "TXTLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("BMG");

                columnHeader1.Text = xml.Element("S01").Value;
                columnHeader2.Text = xml.Element("S02").Value;
                listProp.Items[0].Text = xml.Element("S03").Value;
                listProp.Items[1].Text = xml.Element("S04").Value;
                listProp.Items[2].Text = xml.Element("S05").Value;
                listProp.Items[3].Text = xml.Element("S06").Value;
                listProp.Items[4].Text = xml.Element("S07").Value;
                listProp.Items[5].Text = xml.Element("S08").Value;
                label1.Text = xml.Element("S09").Value;
                traducciones = new String[2];
                traducciones[0] = xml.Element("S0A").Value;
                traducciones[1] = xml.Element("S0B").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        private void Informacion()
        {
            listProp.Items[0].SubItems.Add(bmg.uft16 ? traducciones[0] : traducciones[1]);
            listProp.Items[2].SubItems.Add(bmg.inf1.nMsg.ToString());
            listProp.Items[3].SubItems.Add(bmg.inf1.offsetLength.ToString());
            listProp.Items[4].SubItems.Add(bmg.inf1.unknown1.ToString());
            listProp.Items[5].SubItems.Add(bmg.inf1.unknown2.ToString());

            msg = bmg.dat1.msgs;
            numericMsg.Maximum = bmg.inf1.nMsg;
            txtMsg.Text = msg[0];
        }

        private void numericMsg_ValueChanged(object sender, EventArgs e)
        {
            txtMsg.Text = msg[(int)numericMsg.Value - 1];
        }
    }
}
