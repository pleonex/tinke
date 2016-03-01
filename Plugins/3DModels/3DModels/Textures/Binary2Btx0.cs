//
//  Binary2Btx0.cs
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
using System;
using System.IO;
using Models3D.Structures;
using System.Collections.Generic;

namespace Models3D.Textures
{
    public class Binary2Btx0 : IConverter<Stream, Btx0>, IConverter<Btx0, Stream>
    {
        public void Convert(Stream binary, Btx0 btx0)
        {
            var nitroConverter = new Binary2NitroFile(
                new Dictionary<string, Type> { { Tex0.BlockName, typeof(Tex0) } });
            nitroConverter.Convert(binary, btx0);
        }

        public void Convert(Btx0 btx0, Stream binary)
        {
            
        }
    }
}

