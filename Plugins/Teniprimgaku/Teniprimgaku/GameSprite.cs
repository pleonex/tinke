// ----------------------------------------------------------------------
// <copyright file="GameSprite.cs" company="none">
// Copyright (C) 2013
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
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
//
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>25/03/2013 2:13:54</date>
// -----------------------------------------------------------------------
namespace Teniprimgaku
{
    using System;
    using System.Drawing;
    using Ekona.Images;

    public class GameSprite
    {
        public PaletteBase Palette
        {
            get;
            set;
        }

        public ImageBase Image
        {
            get;
            set;
        }

        public SpriteBase Sprite
        {
            get;
            set;
        }
    }
}
