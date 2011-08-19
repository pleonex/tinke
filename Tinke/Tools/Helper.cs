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
 * Programador: pleoNeX
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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
        public static byte[] BytesTo4BitsRev(byte[] bytes)
        {
            byte[] bits = new Byte[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bits[i * 2 + 1] = (byte)((bytes[i] & 0xF0) >> 4);
                bits[i * 2] = (byte)(bytes[i] & 0x0F);
            }

            return bits;
        }
        /// <summary>
        /// Convierte un byte en dos bits
        /// </summary>
        /// <param name="Byte">Byte para convertir</param>
        /// <returns>2 bytes que corresponden a los bits.</returns>
        public static byte[] ByteTo4Bits(byte Byte)
        {
            byte[] bits = new byte[2];

            bits[0] = (byte)(Byte & 0x0F);
            bits[1] = (byte)((Byte & 0xF0) >> 4);

            return bits;
        }
        public static byte[] Bits4ToBits8Rev(byte[] bytes)
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

        public static string BytesToBits(byte[] bytes)
        {
            string bits = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                bits = Convert.ToString(bytes[i], 2) + bits;
                for (int j = 0; j < (8 * (i + 1)); j++)
                    if (bits.Length == j)
                        bits = '0' + bits;
            }
            return bits;

        }

        public static XElement ObtenerTraduccion(string arbol)
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

                tree = xml.Element(arbol);
            }
            catch { throw new Exception("There was an error in the XML file of language."); }

            return tree;
        }
        public static String ObtenerTraduccion(string arbol, string codigo)
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
                
                message = xml.Element(arbol).Element(codigo).Value;
            }
            catch { throw new Exception("There was an error in the XML language file."); }

            return message;
        }

    }
}
