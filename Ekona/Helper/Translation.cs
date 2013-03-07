//-----------------------------------------------------------------------
// <copyright file="Translation.cs" company="none">
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
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>07/03/2013</date>
//-----------------------------------------------------------------------
namespace Ekona.Helper
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml.Linq;

    /// <summary>
    /// Represents operations to get text in different languages.
    /// </summary>
    public static class Translation
    {
        private static string language;

        /// <summary>
        /// Initializes a new instance of the <see cref="Translation"/> class.
        /// </summary>
        static Translation()
        {
            string tinkePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            XDocument confXml = XDocument.Load(Path.Combine(tinkePath, "Tinke.xml"));

            XElement optionsXml = confXml.Element("Tinke").Element("Options");
            language = optionsXml.Element("Language").Value;
        }

        /// <summary>
        /// Get the path to the translation file of the current assembly.
        /// </summary>
        /// <returns>String with the path of the file.</returns>
        public static string GetTranslationFile()
        {
            string assemblyName = Assembly.GetCallingAssembly().ManifestModule.Name;
            assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf('.'));    // Remove extension

            return GetTranslationFile(assemblyName);
        }

        /// <summary>
        /// Get the path to the translation file of an assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to get translation.</param>
        /// <returns>String with the path of the file.</returns>
        public static string GetTranslationFile(string assemblyName)
        {
            string tinkePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //string langsPath = Path.Combine(tinkePath, "langs");
            string langsPath = Path.Combine(tinkePath, "Plugins");  // TEMP

            return Path.Combine(langsPath, assemblyName + ".xml");
        }

        /// <summary>
        /// Get the a XML element of the current assembly.
        /// </summary>
        /// <returns>XML element with of the current language.</returns>
        public static XElement GetTranslationXml()
        {
            string assemblyName = Assembly.GetCallingAssembly().ManifestModule.Name;
            assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf('.'));    // Remove extension

            return GetTranslationXml(assemblyName);
        }

        /// <summary>
        /// Get the a XML element from an assemby name.
        /// </summary>
        /// <returns>XML element with of the current language.</returns>
        public static XElement GetTranslationXml(string assemblyName)
        {
            string xmlFile = GetTranslationFile(assemblyName);
            if (!File.Exists(xmlFile))
            {
                return null;
            }

            XDocument doc = XDocument.Load(xmlFile);
            XElement element = doc.Element(assemblyName);
            if (element == null)
            {
                return null;
            }

            element = element.Element(language);

            return element;
        }

        /// <summary>
        /// Translate controls using the current assembly translation XML. It matches the control name.
        /// </summary>
        /// <param name="controls">Controls to translate.</param>
        /// <param name="xmlName">Subelement inside the XML file.</param>
        public static void TranslateControls(Control.ControlCollection controls, string xmlName)
        {
            string assemblyName = Assembly.GetCallingAssembly().ManifestModule.Name;
            assemblyName = assemblyName.Substring(0, assemblyName.LastIndexOf('.'));    // Remove extension

            Control[] controlArray = new Control[controls.Count];
            controls.CopyTo(controlArray, 0);

            TranslateControls(controlArray, xmlName, assemblyName);
        }

        /// <summary>
        /// Translate controls using a translation XML from an assembly name. It matches the control name.
        /// </summary>
        /// <param name="controls">Controls to translate.</param>
        /// <param name="xmlName">Subelement inside the XML file.</param>
        public static void TranslateControls(Control[] controls, string xmlName, string assemblyName)
        {
            XElement transXml = GetTranslationXml(assemblyName);
            if (transXml == null || transXml.Element(xmlName) == null)
            {
                return;
            }

            transXml = transXml.Element(xmlName);
            foreach (Control control in controls)
            {
                if (transXml.Element(control.Name) != null)
                {
                    control.Text = transXml.Element(control.Name).Value;
                }
            }
        }

        /// <summary>
        /// User choosen language.
        /// </summary>
        public static string Language
        {
            get { return language; }
        }
    }
}
