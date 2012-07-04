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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona.Helper;

namespace Tinke.Dialog
{
    public partial class CallPlugin : Form
    {
        public CallPlugin()
        {
            InitializeComponent();
        }
        public CallPlugin(string[] list)
        {
            InitializeComponent();

            comboPlugin.Items.AddRange(list);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public String Extension
        {
            get 
            {
                return txtExt.Text;
            }
            set 
            {
                txtExt.Text = value;
                txtHeaderHex.Text = BitConverter.ToString(Encoding.ASCII.GetBytes(txtHeader.Text.ToCharArray()));
            }
        }
        public String Header
        {
            get 
            {
                if (txtHeader.TextLength != 4)
                    txtHeader.Text = txtHeader.Text.PadRight(4, ' ');
                return txtHeader.Text;
            }
            set { txtHeader.Text = value; }
        }
        public String Plugin
        {
            get { return comboPlugin.Text; }
        }
        public int Action
        {
            get { return comboAction.SelectedIndex; }
        }
        public ushort ID
        {
            get { return (ushort)numericID.Value; }
            set { numericID.Value = value; }
        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            if (txtHeader.Focused)
                txtHeaderHex.Text = BitConverter.ToString(Encoding.ASCII.GetBytes(txtHeader.Text.ToCharArray()));
        }
        private void txtHeaderHex_TextChanged(object sender, EventArgs e)
        {
            if (txtHeaderHex.Focused)
                txtHeader.Text = new String(Encoding.ASCII.GetChars(BitsConverter.StringToBytes(txtHeaderHex.Text, 4)));
        }
    }
}
