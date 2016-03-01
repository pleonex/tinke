//
//  ColorExtension.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Drawing;

namespace Models3D.Extensions
{
    public static class ColorExtension
    {
        public static Color SumColors(this Color a, Color b, int wa, int wb)
        {
            return Color.FromArgb(
                (a.R * wa + b.R * wb) / (wa + wb),
                (a.G * wa + b.G * wb) / (wa + wb),
                (a.B * wa + b.B * wb) / (wa + wb));
        }
    }
}

