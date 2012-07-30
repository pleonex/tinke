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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Tinke.Tools
{
    public static class Helper
    {
        public static byte[] Get_Bytes(int offset, int length, string path)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = offset;

            byte[] bytes = br.ReadBytes(length);
            br.Close();

            return bytes;
        }

        public static XElement GetTranslation(string treeS)
        {
            XElement tree = null;
            try
            {
                XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
                string idioma = xml.Element("Options").Element("Language").Value;
                xml = null;

                foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "langs"))
                {
                    if (!langFile.EndsWith(".xml"))
                        continue;

                    xml = XElement.Load(langFile);
                    if (xml.Attribute("name").Value == idioma)
                        break;
                }

                tree = xml.Element(treeS);
            }
            catch { throw new Exception("There was an error in the XML file of language."); }

            return tree;
        }
        public static String GetTranslation(string tree, string code)
        {
            String message = "";

            try
            {
                XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
                string idioma = xml.Element("Options").Element("Language").Value;
                xml = null;

                foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "langs"))
                {
                    if (!langFile.EndsWith(".xml"))
                        continue;

                    xml = XElement.Load(langFile);
                    if (xml.Attribute("name").Value == idioma)
                        break;
                }
                
                message = xml.Element(tree).Element(code).Value;
            }
            catch { throw new Exception("There was an error in the XML language file."); }

            return message;
        }
        public static String Get_LangXML()
        {
            XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            string lang = xml.Element("Options").Element("Language").Value;

            foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "langs"))
            {
                if (!langFile.EndsWith(".xml"))
                    continue;

                xml = XElement.Load(langFile);
                if (xml.Attribute("name").Value == lang)
                    return langFile;
            }
            return "";
        }

    }
}
