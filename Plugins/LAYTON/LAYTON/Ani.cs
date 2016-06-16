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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace LAYTON
{
    using System.Drawing.Imaging;

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
                return Format.Animation;
            else
                return Format.Unknown;
        }

        public void Leer()
        {
        }

        public Control Show_Info(int id)
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
                control = new InfoAni(Obtener_ARC(), pluginHost, id);
            else if (archivo.EndsWith(".arjnn"))
                control = new InfoAni(Obtener_ARJ(), pluginHost, id);
            File.Delete(archivo);
            return control;
        }

        public static void SaveToFile(Todo info, string filePath)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(filePath));
            bw.BaseStream.SetLength(0);
            bw.Write(info.imgs);
            bw.Write((info.tipo == ColorDepth.Depth4Bit) ? (ushort)3 : (ushort)4);
            if (info.type == 1) bw.Write((uint)info.paleta.colores.Length);

            // Write images (frames)
            for (int i = 0; i < info.imgs; i++)
            {
                bw.Write(info.imagenes[i].width);
                bw.Write(info.imagenes[i].height);
                bw.Write(info.imagenes[i].imgs);
                bw.Write((ushort)0);
                // Write parts (cells)
                for (int j = 0; j < info.imagenes[i].imgs; j++)
                {
                    info.imagenes[i].segmentos[j].offSet = (ulong)bw.BaseStream.Position;
                    if (info.type == 1)
                    {
                        bw.Write(info.imagenes[i].segmentos[j].glbX);
                        bw.Write(info.imagenes[i].segmentos[j].glbY);
                    }

                    bw.Write(info.imagenes[i].segmentos[j].posX);
                    bw.Write(info.imagenes[i].segmentos[j].posY);
                    bw.Write((ushort)Math.Log(info.imagenes[i].segmentos[j].width / 8, 2));
                    bw.Write((ushort)Math.Log(info.imagenes[i].segmentos[j].height / 8, 2));
                    bw.Write(info.imagenes[i].segmentos[j].datos);
                }
            }

            // Write palette
            info.paleta.offset = (ulong)bw.BaseStream.Position;
            if (info.type == 0) bw.Write((uint)info.paleta.colores.Length);
            bw.Write(info.paleta.datos);

            // Write animations names
            bw.Write(new byte[0x1E]);
            bw.Write((uint)info.anims.LongLength);
            for (uint i = 0; i < info.anims.LongLength; i++)
            {
                bw.Write(Encoding.ASCII.GetBytes(info.anims[i].name));
                bw.Write(new byte[0x1E - info.anims[i].name.Length]);
            }

            // Write animations info
            for (uint i = 0; i < info.anims.LongLength; i++)
            {
                bw.Write(info.anims[i].framesCount);
                for (uint j = 0; j < info.anims[i].framesCount; j++) bw.Write(info.anims[i].framesIds[j]);
                for (uint j = 0; j < info.anims[i].framesCount; j++) bw.Write(info.anims[i].framesUnk[j]);
                for (uint j = 0; j < info.anims[i].framesCount; j++) bw.Write(info.anims[i].imagesIndexes[j]);
            }

            bw.Write(info.variables);
            bw.Flush();
            bw.Close();
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
                parte.datos = /*Ekona.Helper.BitsConverter.BytesToBit4*/(rdr.ReadBytes((int)parte.length));
            }

            rdr.Close();

            return parte;
        }
        /// <summary>
        /// Transforma la estructura Imagen en un Bitmap.
        /// </summary>
        /// <param name="imagen">Estructura imagen.</param>
        /// <param name="paleta">Estructura paleta</param>
        /// <returns>Bitmap</returns>
        public static Bitmap Transformar_Imagen(Image imagen, Paleta paleta)
        {
            Size original = Tamano_Original(imagen);
            Bitmap final = new Bitmap(original.Width, original.Height);
            
            for (int i = 0; i < imagen.imgs; i++)
            {
                byte[] datos = (imagen.tipo == ColorDepth.Depth4Bit)
                                   ? Ekona.Helper.BitsConverter.BytesToBit4(imagen.segmentos[i].datos)
                                   : imagen.segmentos[i].datos;
                for (int h = 0; h < imagen.segmentos[i].height; h++)
                {
                    for (int w = 0; w < imagen.segmentos[i].width; w++)
                    {
                        // NOTA: en caso de error porque el color no lo contenga la paleta poner color negro.
                        final.SetPixel(w + imagen.segmentos[i].posX, h + imagen.segmentos[i].posY,
                            paleta.colores[datos[w + h * imagen.segmentos[i].width]]);

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
        public static Size Tamano_Original(Image imagen)
        {
            int width = imagen.width;
            int height = imagen.height;

            for (int i = 0; i < imagen.imgs; i++)
            {
                width = Math.Max(imagen.segmentos[i].posX + imagen.segmentos[i].width, width);
                height = Math.Max(imagen.segmentos[i].posY + imagen.segmentos[i].height, height);
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
                     resultados[i].PixelFormat);
            }

            return resultados;
        }
        public Todo Obtener_ARC()
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
            uint numAnims = rdr.ReadUInt32();
            Animation[] anims = new Animation[numAnims];
            for (uint i = 0; i < numAnims; i++)
            {
                anims[i].name = new String(rdr.ReadChars(0x1E));
                anims[i].name = anims[i].name.Substring(0, anims[i].name.IndexOf('\0'));
            }

            // Read animations scripts
            for (uint i = 0; i < numAnims; i++)
            {
                anims[i].framesCount = rdr.ReadUInt32();
                anims[i].framesIds = new uint[anims[i].framesCount];
                anims[i].framesUnk = new uint[anims[i].framesCount];
                anims[i].imagesIndexes = new uint[anims[i].framesCount];
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].framesIds[j] = rdr.ReadUInt32();
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].framesUnk[j] = rdr.ReadUInt32();
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].imagesIndexes[j] = rdr.ReadUInt32();
            }

            // Read variables
            byte[] vars = rdr.ReadBytes((int)(rdr.BaseStream.Length - rdr.BaseStream.Position));
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
                if (imagenes[i].height < imagenes[i].bitmap.Width || imagenes[i].width < imagenes[i].bitmap.Height)
                    imagenes[i].bitmap = imagenes[i].bitmap.Clone(new Rectangle(0, 0, imagenes[i].width, imagenes[i].height),
                        imagenes[i].bitmap.PixelFormat);
            }


            final.imagenes = imagenes;
            final.paleta = paleta;
            final.anims = anims;
            final.variables = vars;
            final.type = 0; // arc

            return final;
        }

        #endregion

        #region Estructuras
        public struct Paleta
        {
            public ulong offset;
            public uint length;
            public byte[] datos;
            public Color[] colores;

            public void Update(Color[] colors)
            {
                this.colores = colors;
                this.datos = Actions.ColorToBGR555(colors);
                this.length = (uint)this.datos.Length;
            }
        }

        public struct Image
        {
            public Parte[] segmentos;
            public uint length;
            public ushort width;
            public ushort height;
            public ushort imgs;
            public ColorDepth tipo;
            public Bitmap bitmap;

            public bool Import(Bitmap bmp, bool tiled, ref Color[] swapPalette)
            {
                Color[] pal;
                ColorFormat f = (tipo == ColorDepth.Depth4Bit) ? ColorFormat.colors16 : ColorFormat.colors256;
                try
                {
                    byte[] data;
                    Actions.Indexed_Image(bmp, f, out data, out pal);
                    if (swapPalette == null) swapPalette = pal;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }

                if (this.width == bmp.Width && this.height == bmp.Height)
                {
                    for (int i = 0; i < imgs; i++)
                    {
                        Rectangle rect = new Rectangle(
                            segmentos[i].posX,
                            segmentos[i].posY,
                            Math.Min(segmentos[i].width, width - segmentos[i].posX),
                            Math.Min(segmentos[i].height, height - segmentos[i].posY));
                        Bitmap cell = bmp.Clone(rect, PixelFormat.Format32bppArgb);
                        
                        if (rect.Width < segmentos[i].width || rect.Height < segmentos[i].height)
                        {
                            var newcell = new Bitmap(segmentos[i].width, segmentos[i].height, cell.PixelFormat);
                            for (int cx = 0; cx < cell.Width; cx++)
                                for (int cy = 0; cy < cell.Height; cy++)
                                    newcell.SetPixel(cx, cy, cell.GetPixel(cx, cy));

                            cell = newcell;
                        }

                        Actions.Indexed_Image(cell, f, out segmentos[i].datos, out pal);
                        Actions.Swap_Palette(ref segmentos[i].datos, swapPalette, pal, f, decimal.MaxValue);
                        if (tiled) segmentos[i].datos = Actions.HorizontalToLineal(segmentos[i].datos, segmentos[i].width, segmentos[i].height, (int)tipo, 8);
                    }
                }
                else
                {
                    this.length = 0;
                    this.width = (ushort)bmp.Width;
                    this.height = (ushort)bmp.Height;
                    ushort cellCountHorizontal = (ushort)Math.Ceiling(this.width / 64.0);
                    ushort cellCountVertical = (ushort)Math.Ceiling(this.height / 64.0);
                    this.imgs = (ushort)(cellCountHorizontal * cellCountVertical);
                    this.segmentos = new Parte[imgs];
                    for (int x = 0; x < cellCountHorizontal; x++)
                    {
                        for (int y = 0; y < cellCountVertical; y++)
                        {
                            int i = (y * cellCountHorizontal + x);
                            this.segmentos[i].posX = (ushort)(x * 64);
                            this.segmentos[i].posY = (ushort)(y * 64);
                            this.segmentos[i].width = (x < cellCountHorizontal - 1 || this.width % 64 == 0) ? (ushort)64 : (ushort)(width % 64);
                            this.segmentos[i].height = (y < cellCountVertical - 1 || this.height % 64 == 0) ? (ushort)64 : (ushort)(height % 64);

                            Rectangle rect = new Rectangle(segmentos[i].posX, segmentos[i].posY, segmentos[i].width, segmentos[i].height);
                            Bitmap cell = bmp.Clone(rect, PixelFormat.Format32bppArgb);

                            if (this.segmentos[i].width < 64 || this.segmentos[i].height < 64)
                            {
                                if (this.segmentos[i].width < 64)
                                {
                                    this.segmentos[i].width = (ushort)(1 << (int)Math.Ceiling(Math.Log(this.segmentos[i].width, 2)));
                                    this.segmentos[i].width = Math.Max(this.segmentos[i].width, (ushort)8);
                                }

                                if (this.segmentos[i].height < 64)
                                {
                                    this.segmentos[i].height = (ushort)(1 << (int)Math.Ceiling(Math.Log(this.segmentos[i].height, 2)));
                                    this.segmentos[i].height = Math.Max(this.segmentos[i].height, (ushort)8);
                                }

                                var newcell = new Bitmap(segmentos[i].width, segmentos[i].height, cell.PixelFormat);
                                for (int cx = 0; cx < cell.Width; cx++)
                                    for (int cy = 0; cy < cell.Height; cy++)
                                        newcell.SetPixel(cx, cy, cell.GetPixel(cx, cy));

                                cell = newcell;
                            }

                            Actions.Indexed_Image(cell, f, out segmentos[i].datos, out pal);
                            Actions.Swap_Palette(ref segmentos[i].datos, swapPalette, pal, f, decimal.MaxValue);
                            if (tiled) segmentos[i].datos = Actions.HorizontalToLineal(segmentos[i].datos, segmentos[i].width, segmentos[i].height, (int)tipo, 8);

                            this.segmentos[i].length = (ushort)segmentos[i].datos.Length;
                            this.length += this.segmentos[i].length;
                        }
                    }
                }

                this.bitmap = (tiled)
                    ? Transformar_ImagenARJ(this, new Paleta() { colores = swapPalette })
                    : Transformar_Imagen(this, new Paleta() { colores = swapPalette });
                this.bitmap = this.bitmap.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format32bppArgb);
                return true;
            }
        }
        public struct Parte
        {
            public ulong offSet;
            public uint length;
            public ushort width;
            public ushort height;
            public ushort posX;
            public ushort posY;
            public ushort glbX;
            public ushort glbY;
            public byte[] datos;
        }

        public struct Animation
        {
            public string name;
            public uint framesCount;
            public uint[] framesIds;
            public uint[] framesUnk;
            public uint[] imagesIndexes;
        }

        public struct Todo
        {
            public Paleta paleta;
            public Image[] imagenes;
            public ColorDepth tipo;
            public ushort imgs;
            public Animation[] anims;
            public byte[] variables;
            public byte type; // 0 - arc; 1 - arj
        }
        #endregion // Estructuras

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
            uint numAnims = rdr.ReadUInt32();
            Animation[] anims = new Animation[numAnims];
            for (uint i = 0; i < numAnims; i++)
            {
                anims[i].name = new String(rdr.ReadChars(0x1E));
                anims[i].name = anims[i].name.Substring(0, anims[i].name.IndexOf('\0'));
            }

            // Read animations scripts
            for (uint i = 0; i < numAnims; i++)
            {
                anims[i].framesCount = rdr.ReadUInt32();
                anims[i].framesIds = new uint[anims[i].framesCount];
                anims[i].framesUnk = new uint[anims[i].framesCount];
                anims[i].imagesIndexes = new uint[anims[i].framesCount];
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].framesIds[j] = rdr.ReadUInt32();
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].framesUnk[j] = rdr.ReadUInt32();
                for (int j = 0; j < anims[i].framesCount; j++) anims[i].imagesIndexes[j] = rdr.ReadUInt32();
            }

            // Read variables
            byte[] vars = rdr.ReadBytes((int)(rdr.BaseStream.Length - rdr.BaseStream.Position));
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
                        imagenes[i].bitmap.PixelFormat);
            }


            final.imagenes = imagenes;
            final.paleta = paleta;
            final.anims = anims;
            final.variables = vars;
            final.type = 1; // arj

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
            parte.glbX = rdr.ReadUInt16();
            parte.glbY = rdr.ReadUInt16();
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
                parte.datos = /*Ekona.Helper.BitsConverter.BytesToBit4*/(rdr.ReadBytes((int)parte.length));
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
        public static Bitmap Transformar_ImagenARJ(Image imagen, Paleta paleta)
        {
            Size original = Tamano_Original(imagen);
            Bitmap final = new Bitmap(original.Width, original.Height);
            
            int dato = 0;
            for (int i = 0; i < imagen.imgs; i++)
            {
                byte[] datos = (imagen.tipo == ColorDepth.Depth4Bit)
                                   ? Ekona.Helper.BitsConverter.BytesToBit4(imagen.segmentos[i].datos)
                                   : imagen.segmentos[i].datos;
                for (int ht = 0; ht < imagen.segmentos[i].height / 8; ht++)
                {
                    for (int wt = 0; wt < imagen.segmentos[i].width / 8; wt++)
                    {
                        for (int h = 0; h < 8; h++)
                        {
                            for (int w = 0; w < 8; w++)
                            {
                                final.SetPixel(w + wt * 8 + imagen.segmentos[i].posX, h + ht * 8 + imagen.segmentos[i].posY,
                                    paleta.colores[datos[dato]]);
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
