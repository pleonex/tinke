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
        public static string BytesToHexString(byte[] bytes)
        {
            string result = "0x";

            for (int i = 0; i < bytes.Length; i++)
                result += String.Format("{0:X}", bytes[i]);

            return result;
        }

        /// <summary>
        /// Convierte una array de bytes en bits y les cambia el orden.
        /// </summary>
        /// <param name="bytes">Array de bytes.</param>
        /// <returns>Array de bits.</returns>
        public static byte[] Bits8To4Bits(byte[] bytes)
        {
            byte[] bits = new Byte[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bits[i * 2] = (byte)(bytes[i] & 0x0F);
                bits[i * 2 + 1] = (byte)((bytes[i] & 0xF0) >> 4);
            }

            return bits;
        }
        public static byte[] Bits4ToBits8(byte[] bytes)
        {
            List<Byte> bit8 = new List<byte>();

            for (int i = 0; i < bytes.Length; i += 2)
            {
                byte byte1 = bytes[i];
                byte byte2 = (byte)(bytes[i + 1] << 4);
                bit8.Add((byte)(byte1 + byte2));
            }

            return bit8.ToArray();
        }

        public static Byte[] BytesToBits(Byte[] bytes)
        {
            List<Byte> bits = new List<byte>();

            for (int i = 0; i < bytes.Length; i++)
                for (int j = 7; j >= 0; j--)
                    bits.Add((byte)((bytes[i] >> j) & 1));

            return bits.ToArray();
        }
        public static Byte[] BitsToBytes(Byte[] bits)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes.Add(newByte);
            }

            return bytes.ToArray();
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

    }
}
