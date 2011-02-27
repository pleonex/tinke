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
                string hex = DecToHex(bytes[i]);

                bits[i * 2] = (byte)HexToSingleDec(hex[1]);
                bits[i * 2 + 1] = (byte)HexToSingleDec(hex[0]);
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

            string hex = DecToHex(Byte);
            bits[0] = (byte)HexToSingleDec(hex[0]);
            bits[1] = (byte)HexToSingleDec(hex[1]);

            return bits;
        }

        /// <summary>
        /// Convierte números decimales a hexadecimales.
        /// </summary>
        /// <param name="x">Número decimal.</param>
        /// <returns>Número hexadecimal en string para que no se autoconvierta a decimal.</returns>
        public static string DecToHex(int x)
        {

            if (x < 10) { return '0' + x.ToString(); }
            else if (x < 16) { return '0' + DecToSingleHex(x).ToString(); }
            else
            {
                string y = "";
                do
                {
                    y = DecToSingleHex((x / 16)) + y;
                    x = x % 16;
                } while (x > 15);

                if (x < 10) { return y + x.ToString(); }
                else { return y + DecToSingleHex(x); }
            }
        }

        /// <summary>
        /// Convierte los números decimales a la letra correspondiente en hexadecimal
        /// </summary>
        /// <param name="x">número decimal</param>
        /// <returns>Letra hexadecimal</returns>
        public static char DecToSingleHex(int x)
        {
            switch (x)
            {
                case 10:
                    return 'A';
                case 11:
                    return 'B';
                case 12:
                    return 'C';
                case 13:
                    return 'D';
                case 14:
                    return 'E';
                case 15:
                    return 'F';
                case 9:
                    return '9';
                case 8:
                    return '8';
                case 7:
                    return '7';
                case 6:
                    return '6';
                case 5:
                    return '5';
                case 4:
                    return '4';
                case 3:
                    return '3';
                case 2:
                    return '2';
                case 1:
                    return '1';
                default:
                    return '0';
            }
        }

        /// <summary>
        /// Convierte de hexadecimal a decimal
        /// </summary>
        /// <param name="x">número hexadecimal</param>
        /// <returns>número decimal</returns>
        public static int HexToSingleDec(char x)
        {
            switch (x)
            {
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case '0':
                    return 0;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Convierte un número hexadecimal en decimal
        /// </summary>
        /// <param name="hex">número hexadecimal</param>
        /// <returns>número decimal</returns>
        public static int HexToDec(string hex)
        {
            int resultado = 0;
            int[] numeros = new int[hex.Length];
            double j = 0;

            for (int i = 0; i < hex.Length; i++)
                numeros[i] = HexToSingleDec(hex[i]);

            for (int i = numeros.Length - 1; i >= 0; i--)
            {
                resultado += numeros[i] * (int)Math.Pow(2, j);
                j += 4;
            }

            return resultado;
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
            XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + "\\Tinke.xml");
            string idioma = xml.Element("Options").Element("Language").Value;
            xml = null;

            foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\langs"))
            {
                if (!langFile.EndsWith(".xml"))
                    continue;

                xml = XElement.Load(langFile);
                if (xml.Attribute("name").Value == idioma)
                    break;
            }

            return xml.Element(arbol);
        }
        public static String ObtenerTraduccion(string arbol, string codigo)
        {
            XElement xml = XElement.Load(System.Windows.Forms.Application.StartupPath + "\\Tinke.xml");
            string idioma = xml.Element("Options").Element("Language").Value;
            xml = null;

            foreach (string langFile in Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\langs"))
            {
                if (!langFile.EndsWith(".xml"))
                    continue;

                xml = XElement.Load(langFile);
                if (xml.Attribute("name").Value == idioma)
                    break;
            }
            xml = xml.Element(arbol);

            string res;
            try { res = xml.Element(codigo).Value; }
            catch 
            {
                System.Windows.Forms.MessageBox.Show("The translate language is incomplete");
                throw new NotImplementedException("The translate language is incomplete");
            }

            return res;
        }

    }
}
