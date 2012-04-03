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
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using PluginInterface;
using PluginInterface.Images;

namespace LAYTON
{
    public class Ani
    {
        IPluginHost pluginHost;
        string gameCode;
        string archivo;

        public Ani(IPluginHost pluginHost, string gameCode, string archivo)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
            this.archivo = archivo;
        }
        public Format Get_Formato(string nombre)
        {
            if (nombre.EndsWith(".ARC") || nombre.EndsWith(".ARJ"))
                return Format.FullImage;
            else
                return Format.Unknown;
        }

        public void Leer()
        {
        }

        public Control Show_Info()
        {
            // Los archivos tienen compresión LZ77, descomprimimos primero.
            string temp = archivo + "nn";
            Byte[] compressFile = new Byte[(new FileInfo(archivo).Length) - 4];
            Array.Copy(File.ReadAllBytes(archivo), 4, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(temp, compressFile);

            pluginHost.Decompress(temp);
            archivo = pluginHost.Get_Files().files[0].path;
            File.Delete(temp);

            
            InfoAni control = new InfoAni();
            if (archivo.EndsWith(".arcnn"))
                control = new InfoAni(Obtener_Todo(), pluginHost);
            else if (archivo.EndsWith(".arjnn"))
                control = new InfoAni(Obtener_ARJ(), pluginHost);
            File.Delete(archivo);
            return control;
        }

        #region Logística ARC

        /// <summary>
        /// Obtiene la paleta a partir de un archivo.
        /// </summary>
        /// <param name="filePath">Ruta del archivo donde se encuentra</param>
        /// <param name="posicion">Posición donde se encuentra el inicio de la paleta.</param>
        /// <returns>Paleta</returns>
        public Paleta Obtener_Paleta(long posicion)
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));

            Paleta paleta = new Paleta();

            /*
             * Estas paletas están compuestas de la siguiente forma (0x relativo al inicio):
             * -0x0-0x3 Número de colores contenido en la paleta. Cada color son 2 bytes.
             * -0x4 inicio de la paleta.
            */

            rdr.BaseStream.Position = posicion;                                  // Posición inicial de paleta
            paleta.length = (rdr.ReadUInt32() * 2);                              // Longitud de paleta
            paleta.offset = (ulong)rdr.BaseStream.Position;
            paleta.datos = rdr.ReadBytes((int)paleta.length);                    // Paleta en binario
            paleta.colores = new Color[paleta.length];                           // Declaramos el tamaño
            paleta.colores = Actions.BGR555ToColor(paleta.datos);               // Paleta en colores

            rdr.Close();

            return paleta;
        }

        /// <summary>
        /// Obtiene una imagen del archivo.
        /// </summary>
        /// <param name="filePath">Archivo</param>
        /// <param name="posicion">Posición inicial</param>
        /// <returns></returns>
        public Image Obtener_Imagen(long posicion, ColorDepth tipo)
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            rdr.BaseStream.Position = posicion;

            Image imagen = new Image();

            imagen.tipo = tipo;
            imagen.width = rdr.ReadUInt16();
            imagen.height = rdr.ReadUInt16();
            imagen.imgs = rdr.ReadUInt16();
            imagen.segmentos = new Parte[imagen.imgs];
            imagen.length = (uint)imagen.width * imagen.height;
            rdr.BaseStream.Seek(2, SeekOrigin.Current);

            for (int i = 0; i < imagen.imgs; i++)
            {
                imagen.segmentos[i] = Obtener_Parte(rdr.BaseStream.Position, imagen.tipo);
                rdr.BaseStream.Seek(imagen.segmentos[i].length + 8, SeekOrigin.Current);
            }

            rdr.Close();

            return imagen;
        }
        /// <summary>
        /// Obtiene una parte del archivo.
        /// </summary>
        /// <param name="filePath">Archivo</param>
        /// <param name="posicion">Posición de inicio de la parte</param>
        /// <returns>Parte de imagen</returns>
        public Parte Obtener_Parte(long posicion, ColorDepth tipo)
        {
            Parte parte = new Parte();

            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            rdr.BaseStream.Position = posicion;

            parte.offSet = (ulong)posicion;
            parte.posX = (ushort)rdr.ReadUInt16();
            parte.posY = (ushort)rdr.ReadUInt16();
            parte.width = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());
            parte.height = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());

            if (tipo == ColorDepth.Depth8Bit)
            {
                parte.length = (uint)parte.width * parte.height;
                parte.datos = rdr.ReadBytes((int)parte.length);
            }
            else
            {
                parte.length = (uint)(parte.width * parte.height) / 2;
                parte.datos = pluginHost.Bit8ToBit4(rdr.ReadBytes((int)parte.length));
            }

            rdr.Close();

            return parte;
        }
        /// Transforma la estructura Imagen en un Bitmap.
        /// </summary>
        /// <param name="imagen">Estructura imagen.</param>
        /// <param name="paleta">Estructura paleta</param>
        /// <returns>Bitmap</returns>
        public Bitmap Transformar_Imagen(Image imagen, Paleta paleta)
        {
            Size original = Tamano_Original(imagen);
            Bitmap final = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < imagen.imgs; i++)
            {
                for (int h = 0; h < imagen.segmentos[i].height; h++)
                {
                    for (int w = 0; w < imagen.segmentos[i].width; w++)
                    {
                        // NOTA: en caso de error porque el color no lo contenga la paleta poner color negro.
                        final.SetPixel(w + imagen.segmentos[i].posX, h + imagen.segmentos[i].posY,
                            paleta.colores[imagen.segmentos[i].datos[w + h * imagen.segmentos[i].width]]);

                    }
                }
            }

            return final;
        }

        /// <summary>
        /// Devuelve el tamaño original de la imagen sin los recortes
        /// </summary>
        /// <param name="imagen">Imagen</param>
        /// <returns>Tamaño original</returns>
        public Size Tamano_Original(Image imagen)
        {
            int width = 0;
            int height = 0;

            for (int i = 0; i < imagen.imgs; i++)
            {
                if (imagen.segmentos[i].posY == 0)
                    width += imagen.segmentos[i].width;
                if (imagen.segmentos[i].posX == 0)
                    height += imagen.segmentos[i].height;
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Obtiene todas las imágenes contenidas en un archivo.
        /// </summary>
        /// <param name="filePath">Archivo</param>
        /// <returns>Array de bitmap con las imágenes</returns>
        public Bitmap[] Obtener_Final()
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));

            Image[] imagenes = new Image[rdr.ReadUInt16()];
            Bitmap[] resultados = new Bitmap[imagenes.Length];
            ColorDepth tipo = rdr.ReadUInt16() == 3 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;

            for (int i = 0; i < imagenes.Length; i++)
            {
                imagenes[i] = Obtener_Imagen(rdr.BaseStream.Position, tipo);
                rdr.BaseStream.Position = (long)imagenes[i].segmentos[imagenes[i].imgs - 1].offSet +
                    imagenes[i].segmentos[imagenes[i].imgs - 1].length + 8;
            }

            Paleta paleta = Obtener_Paleta(rdr.BaseStream.Position);

            rdr.Close();

            for (int i = 0; i < imagenes.Length; i++)
            {
                resultados[i] = Transformar_Imagen(imagenes[i], paleta);
                resultados[i] = resultados[i].Clone(new Rectangle(0, 0, imagenes[i].width, imagenes[i].height),
                     System.Drawing.Imaging.PixelFormat.DontCare);
            }

            return resultados;
        }
        public Todo Obtener_Todo()
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            Todo final = new Todo();

            final.imgs = rdr.ReadUInt16();
            Image[] imagenes = new Image[final.imgs];
            final.tipo = rdr.ReadUInt16() == 3 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;

            for (int i = 0; i < imagenes.Length; i++)
            {
                imagenes[i] = Obtener_Imagen(rdr.BaseStream.Position, final.tipo);
                rdr.BaseStream.Position = (long)imagenes[i].segmentos[imagenes[i].imgs - 1].offSet +
                    imagenes[i].segmentos[imagenes[i].imgs - 1].length + 8;
            }

            Paleta paleta = Obtener_Paleta(rdr.BaseStream.Position);

            // Obtenemos los nombres de las imágenes.
            rdr.BaseStream.Position = (long)paleta.offset + paleta.length + 0x1E;
            uint numNombres = rdr.ReadUInt32() - 1;
            rdr.BaseStream.Seek(0x13 + 0xB, SeekOrigin.Current);
            for (uint i = 0; i < numNombres & i < imagenes.Length; i++)
            {
                imagenes[i].name = new String(rdr.ReadChars(0x13));
                imagenes[i].name = imagenes[i].name.Substring(0, imagenes[i].name.IndexOf('\0'));

                rdr.BaseStream.Seek(0xB, SeekOrigin.Current);
            }

            rdr.Close();

            for (int i = 0; i < imagenes.Length; i++)
            {
                try { imagenes[i].bitmap = Transformar_Imagen(imagenes[i], paleta); }
                catch // En caso de error puede deberse a que la imagen tenga mal las posiciones x e y, intentamos arreglarlo
                {
                    Console.WriteLine("Error al intentar crear la animación. Intentando arreglarla.");
                    for (int j = 0; j < imagenes[i].imgs; j++)
                        if (j > 0)
                            if (imagenes[i].segmentos[j].posX > imagenes[i].segmentos[j - 1].posX)
                                imagenes[i].segmentos[j].posY = imagenes[i].segmentos[j - 1].posY;
                    
                    imagenes[i].bitmap = Transformar_Imagen(imagenes[i], paleta);
                }
                if (imagenes[i].height < imagenes[i].bitmap.Height || imagenes[i].width < imagenes[i].bitmap.Height)
                    imagenes[i].bitmap = imagenes[i].bitmap.Clone(new Rectangle(0, 0, imagenes[i].width, imagenes[i].height),
                        System.Drawing.Imaging.PixelFormat.DontCare);

                if (imagenes[i].name == "" | imagenes[i].name == null)
                    imagenes[i].name = "Sin Nombre " + i.ToString();
            }


            final.imagenes = imagenes;
            final.paleta = paleta;

            return final;
        }

        #region Estructuras
        public struct Paleta
        {
            public ulong offset;
            public uint length;
            public byte[] datos;
            public Color[] colores;
        }

        public struct Image
        {
            public string name;
            public Parte[] segmentos;
            public uint length;
            public ushort width;
            public ushort height;
            public ushort imgs;
            public ColorDepth tipo;
            public Bitmap bitmap;
        }
        public struct Parte
        {
            public ulong offSet;
            public uint length;
            public ushort width;
            public ushort height;
            public ushort posX;
            public ushort posY;
            public byte[] datos;
        }

        public struct Todo
        {
            public Paleta paleta;
            public Image[] imagenes;
            public ColorDepth tipo;
            public ushort imgs;
        }
        #endregion // Estructuras
        #endregion
        #region Logística ARJ
        public Todo Obtener_ARJ()
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            Todo final = new Todo();

            final.imgs = rdr.ReadUInt16();
            Image[] imagenes = new Image[final.imgs];
            final.tipo = rdr.ReadUInt16() == 3 ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;
            uint paleta_size = rdr.ReadUInt32();

            for (int i = 0; i < imagenes.Length; i++)
            {
                imagenes[i] = Obtener_ImagenARJ(rdr.BaseStream.Position, final.tipo);
                rdr.BaseStream.Position = (long)imagenes[i].segmentos[imagenes[i].imgs - 1].offSet +
                    imagenes[i].segmentos[imagenes[i].imgs - 1].length + 12;
            }

            Paleta paleta = Obtener_PaletaARJ(rdr.BaseStream.Position, paleta_size);

            // Obtenemos los nombres de las imágenes.
            rdr.BaseStream.Position = (long)paleta.offset + paleta.length + 0x1E;
            uint numNombres = rdr.ReadUInt32() - 1;
            rdr.BaseStream.Seek(0x13 + 0xB, SeekOrigin.Current);
            for (uint i = 0; i < numNombres & i < imagenes.Length; i++)
            {
                imagenes[i].name = new String(rdr.ReadChars(0x13));
                imagenes[i].name = imagenes[i].name.Substring(0, imagenes[i].name.IndexOf('\0'));

                rdr.BaseStream.Seek(0xB, SeekOrigin.Current);
            }

            rdr.Close();

            for (int i = 0; i < imagenes.Length; i++)
            {
                try { imagenes[i].bitmap = Transformar_ImagenARJ(imagenes[i], paleta); }
                catch // En caso de error puede deberse a que la imagen tenga mal las posiciones x e y, intentamos arreglarlo
                {
                    Console.WriteLine("Error al intentar crear la animación. Intentando arreglarla.");
                    for (int j = 0; j < imagenes[i].imgs; j++)
                        if (j > 0)
                            if (imagenes[i].segmentos[j].posX > imagenes[i].segmentos[j - 1].posX)
                                imagenes[i].segmentos[j].posY = imagenes[i].segmentos[j - 1].posY;

                    imagenes[i].bitmap = Transformar_ImagenARJ(imagenes[i], paleta);
                }
                if (imagenes[i].height < imagenes[i].bitmap.Height || imagenes[i].width < imagenes[i].bitmap.Height)
                    imagenes[i].bitmap = imagenes[i].bitmap.Clone(new Rectangle(0, 0, imagenes[i].width, imagenes[i].height),
                        System.Drawing.Imaging.PixelFormat.DontCare);

                if (imagenes[i].name == "" | imagenes[i].name == null)
                    imagenes[i].name = "Sin Nombre " + i.ToString();
            }


            final.imagenes = imagenes;
            final.paleta = paleta;

            return final;
        }
        public Image Obtener_ImagenARJ(long posicion, ColorDepth tipo)
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            rdr.BaseStream.Position = posicion;

            Image imagen = new Image();

            imagen.tipo = tipo;
            imagen.width = rdr.ReadUInt16();
            imagen.height = rdr.ReadUInt16();
            imagen.imgs = rdr.ReadUInt16();
            imagen.segmentos = new Parte[imagen.imgs];
            imagen.length = (uint)imagen.width * imagen.height;
            rdr.BaseStream.Seek(2, SeekOrigin.Current);

            for (int i = 0; i < imagen.imgs; i++)
            {
                imagen.segmentos[i] = Obtener_ParteARJ(rdr.BaseStream.Position, imagen.tipo);
                rdr.BaseStream.Seek(imagen.segmentos[i].length + 12, SeekOrigin.Current);
            }

            rdr.Close();

            return imagen;
        }
        public Parte Obtener_ParteARJ(long posicion, ColorDepth tipo)
        {
            Parte parte = new Parte();

            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));
            rdr.BaseStream.Position = posicion;

            parte.offSet = (ulong)posicion;
            ushort width = rdr.ReadUInt16();
            ushort height = rdr.ReadUInt16();
            parte.posX = (ushort)rdr.ReadUInt16();
            parte.posY = (ushort)rdr.ReadUInt16();
            parte.width = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());
            parte.height = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());

            if (tipo == ColorDepth.Depth8Bit)
            {
                parte.length = (uint)parte.width * parte.height;
                parte.datos = rdr.ReadBytes((int)parte.length);
            }
            else
            {
                parte.length = (uint)(parte.width * parte.height) / 2;
                parte.datos = pluginHost.Bit8ToBit4(rdr.ReadBytes((int)parte.length));
            }

            rdr.Close();

            return parte;
        }
        public Paleta Obtener_PaletaARJ(long posicion, uint size)
        {
            BinaryReader rdr = new BinaryReader(File.OpenRead(archivo));

            Paleta paleta = new Paleta();

            /*
             * Estas paletas están compuestas de la siguiente forma (0x relativo al inicio):
             * -0x0-0x3 Número de colores contenido en la paleta. Cada color son 2 bytes.
             * -0x4 inicio de la paleta.
            */

            rdr.BaseStream.Position = posicion;                                  // Posición inicial de paleta
            paleta.length = size * 2;                                            // Longitud de paleta
            paleta.offset = (ulong)rdr.BaseStream.Position;
            paleta.datos = rdr.ReadBytes((int)paleta.length);                    // Paleta en binario
            paleta.colores = new Color[paleta.length];                           // Declaramos el tamaño
            paleta.colores = Actions.BGR555ToColor(paleta.datos);                // Paleta en colores

            rdr.Close();

            return paleta;
        }
        public Bitmap Transformar_ImagenARJ(Image imagen, Paleta paleta)
        {
            Size original = Tamano_Original(imagen);
            Bitmap final = new Bitmap(original.Width, original.Height);
            
            int dato = 0;
            for (int i = 0; i < imagen.imgs; i++)
            {
                for (int ht = 0; ht < imagen.segmentos[i].height / 8; ht++)
                {
                    for (int wt = 0; wt < imagen.segmentos[i].width / 8; wt++)
                    {
                        for (int h = 0; h < 8; h++)
                        {
                            for (int w = 0; w < 8; w++)
                            {
                                final.SetPixel(w + wt * 8 + imagen.segmentos[i].posX, h + ht * 8 + imagen.segmentos[i].posY,
                                    paleta.colores[imagen.segmentos[i].datos[dato]]);
                                dato++;
                            }
                        }
                    }
                }
                dato = 0;
            }

            return final;
        }

        #endregion
    }
}
