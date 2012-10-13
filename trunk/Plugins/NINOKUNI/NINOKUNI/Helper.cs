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

        static string xmlconf = Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "Ninokuni.xml";
        static string fontPath = Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "Ninokuni" + Path.DirectorySeparatorChar;

        static Dictionary<char, char> replaces;    // Chars to replace from Latin to SJIS
        static Dictionary<char, char> replacesBack;// Chars to replace from SJIS to Latin

        // Text max size and max lines: 0º Subtitles
        static int[,] MAX_SIZE = new int[1, 2] { { 244, 2 } };
        static Dictionary<char, int> charSize_10;    // Width of each char for common text
        static Dictionary<char, int> charSize_12;     // Width of each char for subtitles

        public static void Initialize()
        {
            Read_Replace();
            charSize_10 = Read_CharWidth(fontPath + "font10.xml");
            charSize_12 = Read_CharWidth(fontPath + "font12.xml");
        }

        public static void Read_Replace()
        {
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
        public static Dictionary<char, int> Read_CharWidth(string xmlFont)
        {
            Dictionary<char, int> charSize = new Dictionary<char, int>();

            try
            {
                XDocument doc = XDocument.Load(xmlFont);
                XElement root = doc.Element("CharMap");

                foreach (XElement e in root.Elements("CharInfo"))
                {
                    char c = e.Attribute("Char").Value[0];
                    if (replacesBack.ContainsKey(c))
                        c = replacesBack[c];
                    int w = Convert.ToInt32(e.Attribute("Width").Value);

                    charSize.Add(c, w);
                }

                root = null;
                doc = null;
            }
            catch { charSize = new Dictionary<char, int>(); }

            return charSize;
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

            char[] chars = text.ToCharArray();
            Replace_Quotes(ref chars);
            text = new string(chars);

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
        public static string Reformat(string text, int nIndent, bool nr)
        {
            if (nIndent < 2)
                return "";
            string s = new string(' ', nIndent);

            if (text.Contains("\n"))
            {
                if (nr)
                {
                    text = text.Remove(0, nIndent + 2);
                    text = text.Remove(text.Length - nIndent);
                }
                else
                {
                    text = text.Remove(0, nIndent + 1);
                    text = text.Remove(text.Length - nIndent + 1);
                }
                text = text.Replace("\n" + s, "\n");
            }
            text = text.Replace('【', '<');
            text = text.Replace('】', '>');

            return text;
        }
        public static void Replace_Quotes(ref char[] chars)
        {
            // Change quotes
            char[] quotes = { '“', '”' };
            bool start_quote = false;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\"' && !start_quote)
                {
                    start_quote = true;
                    chars[i] = quotes[0];
                }
                else if (chars[i] == '\"' && start_quote)
                {
                    start_quote = false;
                    chars[i] = quotes[1];
                }
            }
        }


        public static bool Check(string text, int type)
        {
            if (type < 0 || type > 0)
                return false;

            Dictionary<char, int> charSize = new Dictionary<char, int>();
            if (type == 0)
                charSize = charSize_12;

            int total_width = 0;
            bool furigana = false;

            // Check text width
            foreach (char c in text.ToCharArray())
            {
                // Skip furigana chars
                if (c == '【' || c == '{' || c == '<')
                {
                    furigana = true;
                    continue;
                }
                if (c == '】' || c == '}' || c == '>')
                {
                    furigana = false;
                    continue;
                }
                if (furigana)
                    continue;

                // Reset counter and check if is validate
                if (c == '\n')
                {
                    if (total_width > MAX_SIZE[type, 0])
                        return false;
                    total_width = 0;
                    continue;
                }

                // DEBUG: If there are jap chars, it's not a translated text so skip it (I do it for unrevised texts)
                if (Encoding.GetEncoding("shift_jis").GetByteCount(new char[] { c }) > 1)
                    break;

                if (charSize.ContainsKey(c))
                    total_width += charSize[c];
                else if (type == 0)     // Max width for Font12
                    total_width += 11;
            }

            if (total_width > MAX_SIZE[type, 0])
                return false;

            // Check number of lines
            int lines = 1;
            for (int n = 0; n < text.Length; n++)
                if (text[n] == '\n' && n + 1 != text.Length)
                    lines++;

            if (lines > MAX_SIZE[type, 1])
                return false;


            return true;
        }

        public static string Read_String(byte[] data)
        {
            string text = new String(Encoding.GetEncoding("shift_jis").GetChars(data));
            text = Helper.SJISToLatin(text.Replace("\0", ""));
            return text;
        }
        public static byte[] Write_String(string text, uint size)
        {
            byte[] d, t;

            d = Encoding.GetEncoding("shift_jis").GetBytes(Helper.LatinToSJIS(text));
            t = new byte[size];
            if (d.Length > size)
                System.Windows.Forms.MessageBox.Show("Invalid text. It's so big\n" + text);
            else
                Array.Copy(d, t, d.Length);

            d = null;
            return t;
        }
    }
}
