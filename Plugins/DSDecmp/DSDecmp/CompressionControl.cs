// ----------------------------------------------------------------------
// <copyright file="CompressionControl.cs" company="none">

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
// <date>28/04/2012 14:29:37</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ekona;

namespace DSDecmp
{
    public partial class CompressionControl : UserControl
    {
        int id; // ID of the file to change (the compressed file)
        IPluginHost pluginHost;
        FormatCompress format;

        string nullString;

        public CompressionControl()
        {
            InitializeComponent();
        }
        public CompressionControl(int id, FormatCompress format, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.id = id;
            this.format = format;
            ReadLanguage();

            txtBoxOlderCompress.Text = Enum.GetName(typeof(FormatCompress), format);
            comboFormat.Text = txtBoxOlderCompress.Text;
            txtBoxNewCompress.Text = nullString;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = Main.Get_Traduction();

                label1.Text = xml.Element("S15").Value;
                label2.Text = xml.Element("S12").Value;
                label3.Text = xml.Element("S13").Value;
                nullString = xml.Element("S14").Value;
                checkLookAhead.Text = xml.Element("S16").Value;
                btnCompress.Text = xml.Element("S17").Value;
                btnSearchCompression.Text = xml.Element("S18").Value;
            }
            catch
            {
                throw new Exception("There was an error in the XML file of language.");
            }
        }

        private void comboFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            format = (FormatCompress)Enum.Parse(typeof(FormatCompress), comboFormat.Text);

            if (format == FormatCompress.LZ10 || format == FormatCompress.LZ11 || format == FormatCompress.LZOVL)
                checkLookAhead.Enabled = true;
            else
                checkLookAhead.Enabled = false;
        }
        private void checkLookAhead_CheckedChanged(object sender, EventArgs e)
        {
            switch (format)
            {
                case FormatCompress.LZOVL:
                    Formats.LZOvl.LookAhead = checkLookAhead.Checked;
                    break;
                case FormatCompress.LZ10:
                    Formats.Nitro.LZ10.LookAhead = checkLookAhead.Checked;
                    break;
                case FormatCompress.LZ11:
                    Formats.Nitro.LZ11.LookAhead = checkLookAhead.Checked;
                    break;
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            sFile decompressedFile = pluginHost.Get_DecompressedFiles(id).files[0];
            String filein = decompressedFile.path;
            String fileout = System.IO.Path.GetTempFileName();

            try
            {
                Main.Compress(filein, fileout, format);
                pluginHost.ChangeFile(id, fileout);
                txtBoxNewCompress.Text = Enum.GetName(typeof(FormatCompress), format);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
                txtBoxNewCompress.Text = nullString;
            }
        }
        private void btnSearchCompression_Click(object sender, EventArgs e)
        {
            sFile decompressedFile = pluginHost.Get_DecompressedFiles(id).files[0];
            String filein = decompressedFile.path;
            String fileout = System.IO.Path.GetTempFileName();

            try
            {
                Main.Compress(filein, fileout, FormatCompress.NDS);
                pluginHost.ChangeFile(id, fileout);
                txtBoxNewCompress.Text = Enum.GetName(typeof(FormatCompress), format);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
                txtBoxNewCompress.Text = nullString;
            }
        }
    }
}
