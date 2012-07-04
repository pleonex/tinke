// ----------------------------------------------------------------------
// <copyright file="SelectFile.cs" company="none">

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
// <date>28/04/2012 14:27:46</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;

namespace Tinke.Dialog
{
    public partial class SelectFile : Form
    {
       sFile[] files;

        public SelectFile()
        {
            InitializeComponent();
        }
        public SelectFile(sFile[] files)
        {
            InitializeComponent();

            this.files = files;

            for (int i = 0; i < files.Length; i++)
            {
                String text = "0x" + files[i].id.ToString("x") + " - ";
                text += (String)files[i].tag + '/' + files[i].name;
                listFiles.Items.Add(text);
            }
        }

        public sFile SelectedFile
        {
            get { return files[listFiles.SelectedIndex]; }
        }
    }
}
