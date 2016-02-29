//
//  ResourcesManager.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
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
using System.IO;
using System.Reflection;
using System.Drawing;
using System;

namespace Ekona
{
    /// <summary>
    /// Resources manager for assemblies.
    /// </summary>
    public static class ResourcesManager
    {
        /// <summary>
        /// Get a resource stream from the calling assembly.
        /// </summary>
        /// <returns>The data resource stream.</returns>
        /// <param name="name">Name of the resource.</param>
        public static Stream GetStream(string name)
        {
            return GetStream(name, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets a resource image from the calling assembly.
        /// </summary>
        /// <returns>The resource image.</returns>
        /// <param name="name">Name of the resource.</param>
        public static Image GetImage(string name)
        {
            return Image.FromStream(GetStream(name, Assembly.GetCallingAssembly()));
        }

        /// <summary>
        /// Gets a resource icon from the calling assembly.
        /// </summary>
        /// <returns>The resource icon.</returns>
        /// <param name="name">Name of the resource.</param>
        public static Icon GetIcon(string name)
        {
            return new Icon(GetStream(name, Assembly.GetCallingAssembly()));
        }

        private static Stream GetStream(string name, Assembly assembly)
        {
            var prefix = GetPrefix(assembly);
            return assembly.GetManifestResourceStream(prefix + name);
        }

        private static string GetPrefix(Assembly assembly)
        {
            // Search for the AssemblyTitle attribute.
            // This is usually defined under Properties/AssemblyInfo.cs file.
            string assemblyTitle = null;
            foreach (var attr in assembly.CustomAttributes) {
                if (attr.AttributeType.IsAssignableFrom(typeof(AssemblyTitleAttribute)))
                    assemblyTitle = (string)attr.ConstructorArguments[0].Value;
            }

            // If the assembly does not contain the attribute throw the exception.
            if (assemblyTitle == null)
                throw new FormatException("ERROR: Cannot get AssemblyTitle attribute.");
            
            return assemblyTitle + ".Resources.";
        }
    }
}