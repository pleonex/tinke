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
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Tinke
{
    public static class Convertir
    {
        #region Paleta
        /// <summary>
        /// A partir de un array de bytes devuelve un array de colores.
        /// </summary>
        /// <param name="bytes">Bytes para convertir</param>
        /// <returns>Colores de la paleta.</returns>
        public static Color[] BGR555(byte[] bytes)
        {
            Color[] paleta = new Color[bytes.Length / 2];

            for (int i = 0; i < bytes.Length / 2; i++)
            {
                paleta[i] = BGR555(bytes[i * 2], bytes[i * 2 + 1]);
            }
            return paleta;
        }
        /// <summary>
        /// Convierte dos bytes en un color.
        /// </summary>
        /// <param name="byte1">Primer byte</param>
        /// <param name="byte2">Segundo byte</param>
        /// <returns>Color convertido</returns>
        public static Color BGR555(byte byte1, byte byte2)
        {
            int r, b; double g;

            r = (byte1 % 0x20) * 0x8;
            g = (byte1 / 0x20 + ((byte2 % 0x4) * 7.96875)) * 0x8;
            b = byte2 / 0x4 * 0x8;

            return System.Drawing.Color.FromArgb(r, (int)g, b);
        }
        #endregion

        #region Tiles
        /// <summary>
        /// Convierte una array de Tiles en bytes
        /// </summary>
        /// <param name="tiles">Tiles para convertir</param>
        /// <returns>Array de bytes</returns>
        public static byte[] TilesToBytes(byte[][] tiles)
        {
            List<byte> resul = new List<byte>();

            for (int i = 0; i < tiles.Length; i++)
                for (int j = 0; j < 64; j++)
                    resul.Add(tiles[i][j]);

            return resul.ToArray();
        }
        /// <summary>
        /// Convierte una array de bytes en otra de tiles
        /// </summary>
        /// <param name="bytes">Bytes para convertir</param>
        /// <returns>Array de tiles</returns>
        public static byte[][] BytesToTiles(byte[] bytes)
        {
            List<byte[]> resul = new List<byte[]>();
            List<byte> temp = new List<byte>();

            for (int i = 0; i < bytes.Length / 64; i++)
            {
                for (int j = 0; j < 64; j++)
                    temp.Add(bytes[j + i * 64]);

                resul.Add(temp.ToArray());
                temp.Clear();
            }

            return resul.ToArray();
        }
        #endregion

        /// <summary>
        /// Convierte una array de bytes en formato 4-bit a otro en formato 8-bit
        /// </summary>
        /// <param name="bits4">Datos de entrada en formato 4-bit (valor máximo 15 (0xF))</param>
        /// <returns>Devuelve una array de bytes en 8-bit</returns>
        public static Byte[] Bit4ToBit8(byte[] bits4)
        {
            List<byte> bits8 = new List<byte>();

            for (int i = 0; i < bits4.Length; i += 2)
            {
                string nByte = String.Format("{0:X}", bits4[i]);
                nByte += String.Format("{0:X}", bits4[i + 1]);

                bits8.Add((byte)Convert.ToInt32(nByte, 16));
            }

            return bits8.ToArray();
        }
        /// <summary>
        /// Convierte una array de bytes en formato 8-bit a otro en formato 4-bit
        /// </summary>
        /// <param name="bits8">Datos de entrada en formato 8-bit</param>
        /// <returns>Devuelve una array de bytes en 4-bit</returns>
        public static Byte[] Bit8ToBit4(byte[] bits8)
        {
            List<byte> bits4 = new List<byte>();

            for (int i = 0; i < bits8.Length; i++)
            {
                string nByte = String.Format("{0:X}", bits8[i]);
                if (nByte.Length == 1)
                    nByte = '0' + nByte;
                bits4.Add((byte)Convert.ToInt32(nByte[0].ToString(), 16));
                bits4.Add((byte)Convert.ToInt32(nByte[1].ToString(), 16));
            }

            return bits4.ToArray();
        }

        /// <summary>
        /// Modifica algunas propiedades de un archivo gif
        /// </summary>
        /// <param name="gif">Ruta donde se encuentra el archivo a modificar</param>
        /// <param name="delay">1/100 segundos entre frames</param>
        /// <param name="loops">Número de repeticiones. 0 para infinito, -1 para ninguna</param>
        public static void ModificarGif(string gif, int delay, int loops)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(gif));
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            // Cabecera por defecto
            bw.Write(br.ReadBytes(0xD));
            // Añadimos campo de animación
            if (loops >= 0)
            {
                List<byte> bAni = new List<byte>();
                byte[] rawData = new Byte[] {
	                0x21, 0xFF, 0x0B, 0x4E, 0x45, 0x54, 0x53, 0x43, 0x41, 0x50, 0x45, 0x32,
	                0x2E, 0x30, 0x03, 0x01 };
                bAni.AddRange(rawData);
                bAni.Add(Convert.ToByte(loops & 0xff));
                bAni.Add(Convert.ToByte((loops >> 8) & 0xff));
                bAni.Add(0x00); // Terminator
                bw.Write(bAni.ToArray());
            }
            // Buscamos cada campo de cada frame y le cambiamos el delay
            byte first, second;
            first = br.ReadByte();
            bw.Write(first);

            while (br.BaseStream.Position + 1 != br.BaseStream.Length)
            {
                if (first == 0x21)
                {
                    second = br.ReadByte();
                    bw.Write(second);
                    if (second == 0xF9)
                    {
                        bw.Write(br.ReadBytes(2));
                        bw.Write(Convert.ToByte(delay & 0xff));
                        bw.Write(Convert.ToByte((delay >> 8) & 0xff));
                        br.ReadBytes(2); // Cambiamos esos dos bytes que son el valor del delay
                    }
                    else
                    {
                        first = second;
                        continue;
                    }
                }

                first = br.ReadByte();
                bw.Write(first);
            }

            br.Close();
            br.Dispose();
            File.Delete(gif);
            FileStream fs = new FileStream(gif, FileMode.Create);
            fs.Write(ms.ToArray(), 0, (int)ms.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            ms.Close();
            ms.Dispose();
            bw.Close();
            bw.Dispose();
        }
        /// <summary>
        /// Crea un archivos animado gif apartir de varios archivos bitmap
        /// </summary>
        /// <param name="fout">Ruta de salida del archivo</param>
        /// <param name="frames">Cada uno de los frames</param>
        /// <param name="delay">1/100 seg entre frame</param>
        /// <param name="loops">Número de repeticiones. 0 para infinito, -1 para ninguna</param>
        public static void CrearGif(string fout, Bitmap[] frames, int delay, int loops)
        {
            // ¡¡No funciona con mono!!
            GifBitmapEncoder encoder = new GifBitmapEncoder();

            for (int i = 0; i < frames.Length; i++)
            {
                MemoryStream ms = new MemoryStream();
                frames[i].Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                BitmapFrame bf = BitmapFrame.Create(ms);
                encoder.Frames.Add(bf);
            }
            FileStream fs = new FileStream(fout, FileMode.Create);
            encoder.Save(fs);
            fs.Close();
            fs.Dispose();

            ModificarGif(fout, delay, loops);
        }
    }
}
