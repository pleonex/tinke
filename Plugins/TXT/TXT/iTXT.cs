/*
 * Copyright (C) 2011-2016  pleoNeX
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
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona;
using System.Xml.Linq;
using System.Drawing;

namespace TXT
{
    public partial class iTXT : UserControl
    {
        private IPluginHost pluginHost;
        private int id;
        private byte[] text;

        public iTXT(byte[] text, IPluginHost pluginHost, int id)
        {
            InitializeComponent();

            // TextBox parameter
            this.txtBox.Dock = DockStyle.Top;
            this.txtBox.Font = new Font(FontFamily.GenericMonospace, 11);
            this.txtBox.HideSelection = false;
            this.txtBox.Multiline = true;
            this.txtBox.WordWrap = this.checkWordWrap.Checked;
            this.txtBox.ScrollBars = ScrollBars.Both;
            this.txtBox.Size = new Size(510, 466);

            this.pluginHost = pluginHost;
            this.id = id;
            this.text = text;

            // TODO: Compare preamble with supported encodings.
            if ((text[0] == 0xFE && text[1] == 0xFF) || (text[0] == 0xFF && text[1] == 0xFE))
                comboEncod.SelectedIndex = 2;
            else if (BitConverter.ToUInt16(text, 0) == 0xBBEF)
                comboEncod.SelectedIndex = 4;
            else
                comboEncod.SelectedIndex = 1;

            ReadLanguage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string tempFile = Path.GetTempFileName();

            string textSave = txtBox.Text;
            textSave = textSave.Replace("\\0", "\0");   // FIXME: This could cause issues.
            text = Encoding.GetEncoding(comboEncod.Text).GetBytes(textSave);

            File.WriteAllBytes(tempFile, text);
            pluginHost.ChangeFile(id, tempFile);
        }

        private void ReadLanguage()
        {
            try {
                var pluginPath = Path.Combine(Application.StartupPath, "Plugins");
                var xml = XElement.Load(Path.Combine(pluginPath, "TXTLang.xml"));
                xml = xml.Element(pluginHost.Get_Language());
                xml = xml.Element("TXT");

                btnSave.Text = xml.Element("S00").Value;
                label1.Text  = xml.Element("S01").Value;
                checkWordWrap.Text = xml.Element("S02").Value;
            } catch (Exception ex) { 
                throw new Exception("Exception reading the XML language file.", ex);
            } 
        }

        private string Decode(Encoding encoding)
        {
            string decodedText = encoding.GetString(text);

            // The textbox component excepts Windows EOL.
            if (!decodedText.Contains("\r\n"))
                decodedText = decodedText.Replace("\n", "\r\n");

            // Replace this to view binary text. This could cause issues.
            decodedText = decodedText.Replace("\0", "\\0");

            return decodedText;
        }

        private void comboEncod_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBox.Text = Decode(Encoding.GetEncoding(comboEncod.Text));
            this.txtBox.Select(0, 0);
        }

        private void checkWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            txtBox.WordWrap = checkWordWrap.Checked;
        }
    }
}
