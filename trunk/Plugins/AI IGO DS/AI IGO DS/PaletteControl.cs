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
using PluginInterface;

namespace AI_IGO_DS
{
    public partial class PaletteControl : UserControl
    {
        NCLR paleta;
        IPluginHost pluginHost;

        public PaletteControl()
        {
            InitializeComponent();
        }
        public PaletteControl(IPluginHost pluginHost)
        {
            InitializeComponent();
            this.paleta = pluginHost.Get_NCLR();
            this.pluginHost = pluginHost;

            picPaleta.Image = pluginHost.Bitmaps_NCLR(paleta)[0];
        }
    }
}
