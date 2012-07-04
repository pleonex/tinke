// ----------------------------------------------------------------------
// <copyright file="Helper.cs" company="none">

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
// <date>25/05/2012 14:21:29</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text;
using Ekona;
using System.Windows.Forms;

namespace NINOKUNI
{
    public static class Helper
    {
        public static IPluginHost pluginHost;
        static Dictionary<char, char> replaces;    // Chars to replace from Latin to SJIS
        static Dictionary<char, char> replacesBack;// Chars to replace from SJIS to Latin

        public static void Read_Replace()
        {
            string xmlconf = Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "Ninokuni.xml";

            try
            {
                replaces = new Dictionary<char, char>();
                replacesBack = new Dictionary<char, char>();

                XDocument doc = XDocument.Load(xmlconf);
                XElement root = doc.Element("GameConfig").Element(pluginHost.Get_Language()).Element("ReplaceChars");

                foreach (XElement c in root.Elements("Char"))
                {
                    if (c.Attribute("Original").Value.Length == 0 || c.Attribute("New").Value.Length == 0)
                        continue;

                    replaces.Add(c.Attribute("Original").Value[0], c.Attribute("New").Value[0]);
                    replacesBack.Add(c.Attribute("New").Value[0], c.Attribute("Original").Value[0]);
                }
                root = null;
                doc = null;
            }
            catch
            {
                replaces = new Dictionary<char, char>();
                replacesBack = new Dictionary<char, char>();
            }
        }
        public static string SJISToLatin(string text)
        {
            foreach (char c in replacesBack.Keys)
                text = text.Replace(c, replacesBack[c]);

            return text;
        }
        public static string LatinToSJIS(string text)
        {
            foreach (char c in replaces.Keys)
                text = text.Replace(c, replaces[c]);

            return text;
        }
        public static string Format(string text, int nIndent)
        {
            if (nIndent < 2)
                return "";
            string s = new string(' ', nIndent);

            text = text.Replace('<', '【');
            text = text.Replace('>', '】');
            text = text.Replace("\n", "\n" + s);
            if (text.Contains("\n"))
                text = "\n" + s + text + "\n" + s.Substring(2);

            return text;
        }
        public static string Format(string text, int nIndent, char f1, char f2)
        {
            if (nIndent < 2)
                return "";
            string s = new string(' ', nIndent);

            text = text.Replace(f1, '【');
            text = text.Replace(f2, '】');
            text = text.Replace("\n", "\n" + s);
            if (text.Contains("\n"))
                text = "\n" + s + text + "\n" + s.Substring(2);

            return text;
        }

        public static string Reformat(string text, int nIndent)
        {
            if (nIndent < 2)
                return "";
            string s = new string(' ', nIndent);

            if (text.Contains("\n"))
            {
                text = text.Remove(0, nIndent + 2);
                text = text.Remove(text.Length - nIndent);
                text = text.Replace("\n" + s, "\n");
            }
            text = text.Replace('【', '<');
            text = text.Replace('】', '>');

            return text;
        }

    }
}
