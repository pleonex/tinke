// ----------------------------------------------------------------------
// <copyright file="NCGR.cs" company="none">

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
// <date>28/04/2012 14:26:51</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NCGR
    {
        public static Byte[][] MergeImage(Byte[][] originalTile, Byte[][] newTiles, int startTile)
        {
            List<Byte[]> data = new List<byte[]>();


            for (int i = 0; i < startTile; i++)
            {
                if (i >= originalTile.Length)
                {
                    Byte[] nullTile = new byte[64];
                    for (int t = 0; t < 64; t++)
                        nullTile[t] = 0;
                    data.Add(nullTile);
                    continue;
                }
                data.Add(originalTile[i]);
            }

            data.AddRange(newTiles);

            for (int i = startTile + newTiles.Length; i < originalTile.Length; i++)
                data.Add(originalTile[i]);

            return data.ToArray();
        }
    }
}
