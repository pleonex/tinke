using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Tinke.Juegos
{
    /// <summary>
    /// Métodos y funciones para el juego "Profesor Layton y la Misteriosa Villa"
    /// </summary>
    public static class Layton1
    {
        /// <summary>
        /// Métodos para la carpeta Ani
        /// </summary>
        public static class Ani
        {
            /// <summary>
            /// Obtiene la paleta a partir de un archivo.
            /// </summary>
            /// <param name="filePath">Ruta del archivo donde se encuentra</param>
            /// <param name="posicion">Posición donde se encuentra el inicio de la paleta.</param>
            /// <returns>Paleta</returns>
            public static Paleta Obtener_Paleta(string filePath, long posicion)
            {
                FileStream file = File.OpenRead(filePath);
                BinaryReader rdr = new BinaryReader(file);

                Paleta paleta;

                /*
                 * Estas paletas están compuestas de la siguiente forma (0x relativo al inicio):
                 * -0x0-0x3 byte que al duplicarlo (x2) nos devuelve el tamaño de la paleta.
                 * -0x4 inicio de la paleta.
                */

                rdr.BaseStream.Position = posicion;                                  // Posición inicial de paleta
                paleta.length = (rdr.ReadUInt32() * 2);                             // Longitud de paleta
                paleta.offset = (ulong)rdr.BaseStream.Position;
                paleta.datos = rdr.ReadBytes((int)paleta.length);                    // Paleta en binario
                paleta.colores = new Color[paleta.length];                           // Declaramos el tamaño
                paleta.colores = Imagen.Paleta.Convertidor.BGR555(paleta.datos);     // Paleta en colores

                rdr.Close();
                rdr.Dispose();
                file.Close();
                file.Dispose();

                return paleta;
            }
            /// <summary>
            /// Muestra la paleta en un PictureBox
            /// </summary>
            /// <param name="paleta">Paleta para mostrar.</param>
            /// <param name="pictureBox">PictureBox donde se va a mostrar.</param>
            public static void Mostrar_Paleta(Paleta paleta, ref System.Windows.Forms.PictureBox pictureBox)
            {
                Bitmap imagen = new Bitmap(160, 160);
                bool fin = false;

                for (int i = 0; i < 16 & !fin; i++)
                {
                    for (int j = 0; j < 16 & !fin; j++)
                    {
                        for (int k = 0; k < 10 & !fin; k++)
                        {
                            for (int q = 0; q < 10; q++)
                            {
                                try { imagen.SetPixel(j * 10 + q, i * 10 + k, paleta.colores[j + 16 * i]); }
                                catch { fin = true; imagen.SetPixel(j * 10, i * 10 + q, Color.White); }
                            }
                        }
                    }
                }

                pictureBox.Image = imagen;
            }

            /// <summary>
            /// Obtiene una imagen del archivo.
            /// </summary>
            /// <param name="filePath">Archivo</param>
            /// <param name="posicion">Posición inicial</param>
            /// <returns></returns>
            public static Image Obtener_Imagen(string filePath, long posicion, Imagen.Tiles_Form tipo)
            {
                FileStream file = File.OpenRead(filePath);
                BinaryReader rdr = new BinaryReader(file);

                Image imagen = new Image();

                rdr.BaseStream.Position = posicion;

                imagen.tipo = tipo;
                imagen.width = rdr.ReadUInt16();
                imagen.height = rdr.ReadUInt16();
                imagen.imgs = rdr.ReadUInt16();
                imagen.segmentos = new Parte[imagen.imgs];
                imagen.length = (uint)imagen.width * imagen.height;
                rdr.BaseStream.Seek(2, SeekOrigin.Current);

                for (int i = 0; i < imagen.imgs; i++)
                {
                    imagen.segmentos[i] = Obtener_Parte(filePath, rdr.BaseStream.Position, imagen.tipo);
                    rdr.BaseStream.Seek(imagen.segmentos[i].length + 8, SeekOrigin.Current);
                }

                rdr.Close();
                rdr.Dispose();
                file.Close();
                file.Dispose();

                return imagen;
            }
            /// <summary>
            /// Obtiene una parte del archivo.
            /// </summary>
            /// <param name="filePath">Archivo</param>
            /// <param name="posicion">Posición de inicio de la parte</param>
            /// <returns>Parte de imagen</returns>
            public static Parte Obtener_Parte(string filePath, long posicion, Imagen.Tiles_Form tipo)
            {
                Parte parte = new Parte();

                FileStream file = File.OpenRead(filePath);
                BinaryReader rdr = new BinaryReader(file);
                rdr.BaseStream.Position = posicion;

                parte.offSet = (ulong)posicion;
                parte.posX = (ushort)rdr.ReadUInt16();
                parte.posY = (ushort)rdr.ReadUInt16();
                parte.width = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());
                parte.height = (ushort)Math.Pow(2, 3 + rdr.ReadUInt16());
                if (tipo == Imagen.Tiles_Form.bpp8)
                {
                    parte.length = (uint)parte.width * parte.height;
                    parte.datos = rdr.ReadBytes((int)parte.length);
                }
                else
                {
                    parte.length = (uint)(parte.width * parte.height) / 2;
                    parte.datos = new Byte[2 * parte.length];
                    parte.datos = Tools.Helper.BytesTo4BitsRev(rdr.ReadBytes((int)parte.length));
                }

                rdr.Close();
                rdr.Dispose();
                file.Close();
                file.Dispose();

                return parte;
            }
            /// <summary>
            /// Muestra la estrucutura imagen en un pictueBox
            /// </summary>
            /// <param name="imagen">Estructura imagen</param>
            /// <param name="paleta">Estructura paleta</param>
            public static void Mostrar_Imagen(Image imagen, Paleta paleta, ref System.Windows.Forms.PictureBox pictureBox)
            {

                Size original = Tamano_Original(imagen);
                Bitmap final = new Bitmap(original.Width, original.Height);

                for (int i = 0; i < imagen.imgs; i++)
                    for (int h = 0; h < imagen.segmentos[i].height; h++)
                        for (int w = 0; w < imagen.segmentos[i].width; w++)
                            final.SetPixel(w + imagen.segmentos[i].posX, h + imagen.segmentos[i].posY,
                                paleta.colores[imagen.segmentos[i].datos[w + h * imagen.segmentos[i].width]]);

                pictureBox.Image = final;
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
            public static Size Tamano_Original(Image imagen)
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
            public static Bitmap[] Obtener_Final(string filePath)
            {
                FileStream file = File.OpenRead(filePath);
                BinaryReader rdr = new BinaryReader(file);

                Image[] imagenes = new Image[rdr.ReadUInt16()];
                Bitmap[] resultados = new Bitmap[imagenes.Length];
                Imagen.Tiles_Form tipo = rdr.ReadUInt16() == 3 ? Imagen.Tiles_Form.bpp4 : Imagen.Tiles_Form.bpp8;

                for (int i = 0; i < imagenes.Length; i++)
                {
                    imagenes[i] = Obtener_Imagen(filePath, rdr.BaseStream.Position, tipo);
                    rdr.BaseStream.Position = (long)imagenes[i].segmentos[imagenes[i].imgs - 1].offSet +
                        imagenes[i].segmentos[imagenes[i].imgs - 1].length + 8;
                }

                Paleta paleta = Obtener_Paleta(filePath, rdr.BaseStream.Position);

                rdr.Close();
                rdr = null;
                file.Dispose();
                file = null;

                for (int i = 0; i < imagenes.Length; i++)
                {
                    resultados[i] = Transformar_Imagen(imagenes[i], paleta);
                    resultados[i] = resultados[i].Clone(new Rectangle(0, 0, imagenes[i].width, imagenes[i].height),
                         System.Drawing.Imaging.PixelFormat.DontCare);
                }

                return resultados;
            }
            public static Todo Obtener_Todo(string filePath)
            {
                FileStream file = File.OpenRead(filePath);
                BinaryReader rdr = new BinaryReader(file);
                Todo final = new Todo();

                final.imgs = rdr.ReadUInt16();
                Image[] imagenes = new Image[final.imgs];
                final.tipo = rdr.ReadUInt16() == 3 ? Imagen.Tiles_Form.bpp4 : Imagen.Tiles_Form.bpp8;

                for (int i = 0; i < imagenes.Length; i++)
                {
                    imagenes[i] = Obtener_Imagen(filePath, rdr.BaseStream.Position, final.tipo);
                    rdr.BaseStream.Position = (long)imagenes[i].segmentos[imagenes[i].imgs - 1].offSet +
                        imagenes[i].segmentos[imagenes[i].imgs - 1].length + 8;
                }

                Paleta paleta = Obtener_Paleta(filePath, rdr.BaseStream.Position);

                // Obtenemos los nombres de las imágenes.
                rdr.BaseStream.Position = (long)paleta.offset + paleta.length + 0x1E;
                uint numNombres = rdr.ReadUInt32() - 1;
                rdr.BaseStream.Seek(0x13 + 0xB, SeekOrigin.Current);
                for (uint i = 0; i < numNombres & i < imagenes.Length; i++)
                {
                    imagenes[i].name = new String(rdr.ReadChars(0x13));

                    // Eliminamos caracteres no admitidos por windows. El nombre llega hasta el primer \0
                    imagenes[i].name = imagenes[i].name.Substring(0, imagenes[i].name.IndexOf('\0'));
                    /*imagenes[i].name = imagenes[i].name.Replace("\0", "");
                    imagenes[i].name = imagenes[i].name.Replace("*", "");
                    imagenes[i].name = imagenes[i].name.Replace("?", "");
                    imagenes[i].name = imagenes[i].name.Replace("\\", "");
                    imagenes[i].name = imagenes[i].name.Replace("/", "");
                    imagenes[i].name = imagenes[i].name.Replace(":", "");
                    imagenes[i].name = imagenes[i].name.Replace("|", "");
                    imagenes[i].name = imagenes[i].name.Replace("\"", "");
                    imagenes[i].name = imagenes[i].name.Replace("<", "");
                    imagenes[i].name = imagenes[i].name.Replace(">", "");*/

                    rdr.BaseStream.Seek(0xB, SeekOrigin.Current);
                }

                rdr.Close();
                rdr = null;
                file.Dispose();
                file = null;

                for (int i = 0; i < imagenes.Length; i++)
                {
                    imagenes[i].bitmap = Transformar_Imagen(imagenes[i], paleta);
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
                public Imagen.Tiles_Form tipo;
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
                public Imagen.Tiles_Form tipo;
                public ushort imgs;
            }
            #endregion // Estructuras
        }

        /// <summary>
        /// Métodos para los archivos de la carpeta bg
        /// </summary>
        public static class Bg
        {
            public static Background Obtener_Background(string file)
            {
                Background imagen = new Background();
                BinaryReader br = new BinaryReader(File.OpenRead(file));

                // Paleta, NCLR sin cabecera
                // TODO: Crear la función en la clase de NCLR
                imagen.paleta.pltt.nColores = br.ReadUInt32();
                imagen.paleta.pltt.paletas = new Imagen.NTFP[1];
                imagen.paleta.pltt.paletas[0].colores =
                    Imagen.Paleta.Convertidor.BGR555(br.ReadBytes((int)imagen.paleta.pltt.nColores * 2));

                // Tile, sin cabecera
                long offset = br.BaseStream.Position;
                br.Close();
                br.Dispose();
                imagen.tile = Imagen.Tile.NCGR.Leer_SinCabecera(file, offset);


                // Tile Screen Info
                offset += imagen.tile.rahc.size_tiledata * 64 + 4;

                imagen.screen = Imagen.Screen.NSCR.Leer_NoCabecera(file, offset);
                imagen.tile.rahc.tileData = Imagen.Screen.NSCR.Modificar_Tile(imagen.screen, imagen.tile.rahc.tileData);
                imagen.tile.rahc.nTilesX = imagen.screen.section.width;
                imagen.tile.rahc.nTilesY = imagen.screen.section.height;

                return imagen;
            }
            public static Bitmap Obtener_Imagen(string file)
            {
                Background bg = Obtener_Background(file);
                return Imagen.Tile.NCGR.Crear_Imagen(bg.tile, bg.paleta);
            }

            #region Estructuras
            public struct Background
            {
                public Imagen.Paleta.Estructuras.NCLR paleta;
                public Imagen.Tile.Estructuras.NCGR  tile;
                public Imagen.Screen.Estructuras.NSCR screen;
            }
            #endregion
        }

        public static class Txt
        {
            public static String Leer(string file)
            {
                String txt = "";
                BinaryReader br = new BinaryReader(File.OpenRead(file));

                while (br.BaseStream.Position != br.BaseStream.Length - 1)
                {
                    byte c = br.ReadByte();

                    if (c == 0x0A)
                        txt += '\r';

                    txt += Char.ConvertFromUtf32(c);
                }
                br.Close();
                br.Dispose();

                return txt;
            }

            public static Panel Obtener_Animacion(string fileTxt, string fileLay, string fondo)
            {
                // No funciona bien.. Hay que investigar más y rehacerlo de nuevo
                BinaryReader br = new BinaryReader(File.OpenRead(fileTxt));
                Panel panel = new System.Windows.Forms.Panel();

                string txt = Leer(fileTxt);
                TextBox txtBox = new TextBox();
                txtBox.Dock = DockStyle.Left;
                txtBox.Width = 350;
                txtBox.ReadOnly = true;
                txtBox.Multiline = true;
                txtBox.Text = txt;
                panel.Controls.Add(txtBox);

                List<Bitmap> imagenes = new List<Bitmap>();
                List<String> textos = new List<string>();
                textos.Add(""); int y = 0;
                Bitmap imgFondo = Ani.Obtener_Final(fondo)[0];

                for (int i = 0; br.BaseStream.Position != br.BaseStream.Length; i++)
                {
                    byte c = br.ReadByte();
                    if (c == 0x40)
                    {
                        y++;
                        textos.Add("");
                        br.ReadByte();
                        if (br.ReadByte() == 0x40)
                            br.ReadByte();
                        else
                            br.BaseStream.Position = br.BaseStream.Position - 1;
                    }
                    if (c == 0x0A)
                        textos[y] += "\r";
                    textos[y] += Char.ConvertFromUtf32(c);
                }
                Ani.Todo layton = Ani.Obtener_Todo(fileLay);

                for (int i = 0; i < txt.Length; i++)
                {
                    if (i + 7 < txt.Length)
                    {
                        if (txt.Substring(i, 7) == "&SetAni")
                        {
                            textos[y] += '@';

                            string script = txt.Substring(i + 1);
                            script = script.Remove(script.IndexOf("&"));
                            int numero = Convert.ToInt32(script[7]);
                            string aniName = script.Substring(9);

                            for (int j = 0; j < layton.imagenes.Length; j++)
                                if (layton.imagenes[j].name == aniName)
                                    imagenes.Add(Ani.Transformar_Imagen(layton.imagenes[j], layton.paleta));


                            i += script.Length + 2;
                        }
                    }
                }
                for (int i = 0; i < textos.Count; i++)
                    if (textos[i][0] != '@')
                        imagenes.Insert(i, null);

                LaytonTalks talks = new LaytonTalks(textos.ToArray(), imagenes.ToArray(), imgFondo);
                talks.Dock = DockStyle.Right;
                panel.Controls.Add(talks);

                panel.Dock = DockStyle.Fill;
                return panel;
            }
        }

        public static Tipos.Role FormatoArchivos(int id, Tipos.Pais version, string name, String gameCode)
        {
                name = name.ToUpper();


            switch (gameCode)
            {
                case "A5FP":
                case "A5FE":
                switch (version)
                {
                    case Tipos.Pais.SPA:
                    case Tipos.Pais.EUR:
                        if (id >= 0x0001 && id <= 0x04E7 && name.EndsWith(".ARC"))
                            return Tipos.Role.ImagenAnimada;
                        else if (id >= 0x04E8 && id <= 0x0B72)
                            return Tipos.Role.ImagenCompleta;
                            return Tipos.Role.Desconocido;
                    case Tipos.Pais.USA:
                        if (id >= 0x0001 && id <= 0x02CB && name.EndsWith(".ARC")) // ani
                            return Tipos.Role.ImagenAnimada;
                        else if (id >= 0x02CD && id <= 0x0765) // bg
                            return Tipos.Role.ImagenCompleta;
                        else if (name.EndsWith(".TXT"))
                            return Tipos.Role.Texto;
                        else
                            return Tipos.Role.Desconocido;
                    default:
                        return Tipos.Role.Desconocido;
                }
                case "YLTS":
                switch (version)
                {
                    case Tipos.Pais.SPA:
                        if (id >= 0x0037 && id <= 0x0408 && name.EndsWith(".ARC"))
                            return Tipos.Role.ImagenAnimada;
                        else if (id >= 0x0409 && id <= 0x0808 && name.Contains("ARC"))
                            return Tipos.Role.ImagenCompleta;
                            return Tipos.Role.Desconocido;
                    default :
                            return Tipos.Role.Desconocido;
                }
                default:
                return Tipos.Role.Desconocido;
            }
        }
    }
}