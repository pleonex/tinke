//
//  Format.cs
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
using System.Linq;
using System.Reflection;

namespace Models3D
{
    public abstract class FileFormat
    {

        public static dynamic ConvertFrom<TSrc>(TSrc source, Type dstType)
        {
            var newFormat = Activator.CreateInstance(dstType);
            ConvertFrom<TSrc>(source, dstType, newFormat);
            return newFormat;
        }

        public static void ConvertFrom<TSrc>(TSrc source, Type dstType, dynamic destination)
        {
            Convert(typeof(TSrc), source, dstType, destination);
        }

        public static TDst ConvertTo<TDst>(Type srcType, dynamic source)
            where TDst : new()
        {
            var newFormat = new TDst();
            ConvertTo<TDst>(srcType, source, newFormat);
            return newFormat;
        }

        public static void ConvertTo<TDst>(Type srcType, dynamic source, TDst destination)
        {
            Convert(srcType, source, typeof(TDst), destination);
        }

        public static void Convert<TSrc, TDst>(TSrc source, TDst destination)
        {
            Convert(typeof(TSrc), source, typeof(TDst), destination);
        }

        public static void Convert(Type srcType, dynamic src, Type dstType, dynamic dst)
        {
            var converterType = Assembly.GetExecutingAssembly().GetTypes().Single(type =>
                type.IsClass &&
                type.GetInterfaces().Contains(typeof(IConverter<,>)) &&
                type.GenericTypeArguments[0] == srcType &&
                type.GenericTypeArguments[1] == dstType);

            dynamic converter = Activator.CreateInstance(converterType);
            converter.Convert(src, dst);
        }

        public T ConvertTo<T>()
            where T : new()
        {
            var newFormat = new T();
            ConvertTo<T>(newFormat);
            return newFormat;
        }

        public void ConvertTo<T>(T newFormat)
        {
            FileFormat.ConvertTo<T>(this.GetType(), this, newFormat);
        }

        public T ConvertTo<T>(IConverter converter)
            where T : new()
        {
            T newFormat = new T();
            ConvertTo<T>(newFormat, converter);
            return newFormat;
        }

        public void ConvertTo<T>(T newFormat, dynamic converter)
        {
            if (!converter.GetType().GetInterfaces().Contains(typeof(IConverter<,>)))
                throw new ArgumentException("Invalid converter");

            if (converter.GetType().GenericTypeArguments[0] != this.GetType())
                throw new ArgumentException("The converter cannot convert from this type");

            if (converter.GetType().GenericTypeArguments[1] != typeof(T))
                throw new ArgumentException("The converter cannot convert to that type");

            converter.Convert(this, newFormat);
        }
    }
}

