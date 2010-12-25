using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Tinke
{
    public static class Tipos
    {
        // Al añadir nuevo formato modificar funciones FormatFile y DoAction
        // Al añadir nuevo juego modificar funcion FormatFileGame y DoActionGame
        // Al añadir nuevo formato a un juego modificar funcion DoActionGame
        // Al modificar cualquiera comprobar método See_File en la clase Acciones y modificar según convenga.
        // Si el formato nuevo es comprimido, añadir a la función EstaComprimido
        public static string[] soportados = new string[] 
        { "RLCN", "NCLR", "TXT", "NARC", "ARC", "NCGR", "RGCN", "NSCR", "RCSN", "PCM" };
        public static string[] juegos = new string[] { "A5FP", "A5FE", "YLTS" };
        public enum Role
        {
            Paleta,
            Imagen,
            Screen,
            ImagenPaleta,
            ImagenCompleta,
            ImagenAnimada,
            Texto,
            Video,
            Sonido,
            Comprimido_NARC,
            Comprimido_LZ77,
            Comprimido_Huffman,
            Comprimido_PCM,
            Desconocido
        }
        public enum Pais
        {
            EUR,
            USA,
            JPN,
            NOE,
            ITA,
            SPA,
            HOL,
            KOR,
            EUU,
            Unknown,
        }

        public static int ImageFormatFile(Role name)
        {
            switch (name)
            {
                case Role.Paleta:
                    return 2;
                case Role.ImagenCompleta:
                    return 10;
                case Role.ImagenAnimada:
                    return 8;
                case Role.Texto:
                    return 4;
                case Role.Comprimido_Huffman:
                case Role.Comprimido_LZ77:
                    return 5;
                case Role.Comprimido_PCM :
                case Role.Comprimido_NARC:
                    return 6;
                case Role.Imagen:
                    return 3;
                case Role.Screen:
                    return 9;
                default:
                    return 1;
            }
        }

        public static bool EstaComprimido(Role rol)
        {
            if (rol != Role.Comprimido_LZ77 && rol != Role.Comprimido_NARC && rol != Role.Comprimido_Huffman
                && rol != Role.Comprimido_PCM)
                return false;
            else
                return true;
        }
        public static bool IsSupportedFile(string name)
        {
            if (FormatFile(name) == Role.Desconocido)
                return false;
            else
                return true;
        }
        public static string Extension(string name)
        {
            name = name.ToUpper();
            string ext = "";
            for (int i = 0; i < soportados.Length; i++)
            {
                if (name.EndsWith(soportados[i]))
                {
                    ext = soportados[i];
                    break;
                }
            }
            return ext;
        }
        public static Role FormatFile(string name)
        {
            switch (Extension(name))
            {
                case "RLCN":
                case "NCLR":
                    return Role.Paleta;
                case "RGCN":
                case "NCGR":
                    return Role.Imagen;
                case "TXT":
                    return Role.Texto;
                case "PCM":
                    return Role.Comprimido_PCM;
                case "ARC":
                case "NARC":
                    return Role.Comprimido_NARC;
                case "RCSN":
                case "NSCR":
                    return Role.Screen;
                default:
                    return Role.Desconocido;
            }
        }
        public static Object DoAction(string file, Role formato, string extension)
        {
            switch (formato)
            {
                case Role.Paleta:
                    switch (extension)
                    {
                        case "RLCN":
                        case "NCLR":
                            return Imagen.Paleta.NCLR.Leer(file);
                    }
                    break;
                case Role.Imagen:
                    switch (extension)
                    {
                        case "RGCN":
                        case "NCGR":
                            return Imagen.Tile.NCGR.Leer(file);
                    }
                    break;
                case Role.Texto:
                    switch (extension)
                    {
                        case "TXT":
                            return Texto.TXT.Leer(file);
                    }
                    break;
                case Role.Screen:
                    switch (extension)
                    {
                        case "RCSN":
                        case "NSCR":
                            return Imagen.Screen.NSCR.Leer(file);
                    }
                    break;
            }
            throw new Exception("Excepción en DoAction: Formato no reconocido"); // Nunca puede darse el caso
        }

        public static bool IsSupportedGame(string name)
        {
            name = name.ToUpper();

            for (int i = 0; i < juegos.Length; i++)
                if (name == juegos[i])
                    return true;

            return false;
        }
        public static Pais GameCountry(string gameCode)
        {
            switch (gameCode[3])
            {
                case 'P':
                    return Pais.EUR;
                case 'E':
                    return Pais.USA;
                case 'J':
                    return Pais.JPN;
                case 'D':
                case 'F':
                    return Pais.NOE;
                case 'I':
                    return Pais.ITA;
                case 'S':
                    return Pais.SPA;
                case 'H':
                    return Pais.HOL;
                case 'K':
                    return Pais.KOR;
                case 'X':
                    return Pais.EUU;
                default:
                    return Pais.Unknown;
            }
        }
        public static Role FormatFileGame(string gameCode, int id, string name)
        {
            switch (gameCode)
            {
                case "A5FE":
                case "A5FP":
                case "YLTS":    // LAYTON2 parece tener el mismo tipo de archivos.
                    return Juegos.Layton1.FormatoArchivos(id, GameCountry(gameCode), name, gameCode);
                default:
                    return Role.Desconocido;
            }
        }
        public static Object DoActionGame(string gameCode, string file, Role formato, int id, string name, Acciones accion)
        {
            name = name.ToUpper();

            switch (gameCode)
            {
                case "YLTS":
                    #region LAYTON2 SPA
                    if (id >= 0x0037 && id <= 0x0408 && name.Contains("ARC"))
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fp = Juegos.Layton1.Ani.Obtener_Final(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fp;
                    }
                    else if (id >= 0x0409 && id <= 0x0808 && name.Contains("ARC"))
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fp = Juegos.Layton1.Bg.Obtener_Imagen(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fp;
                    }
                    #endregion
                    break;
                case "A5FP":
                    #region LAYTON1 EUR
                    if (id >= 0x0001 && id <= 0x04E7 && name.Contains("ARC"))
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fp = Juegos.Layton1.Ani.Obtener_Final(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fp;
                    }
                    else if (id >= 0x04E8 && id <= 0x0B72)
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fp = Juegos.Layton1.Bg.Obtener_Imagen(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fp;
                    }
                    break;
                    #endregion //LAYTON1 EUR
                case "A5FE":
                    #region LAYTON1 USA
                    if (id >= 0x0001 && id <= 0x02CB && name.EndsWith("ARC"))
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fe = Juegos.Layton1.Ani.Obtener_Final(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fe;
                    }
                    else if (id >= 0x02CD && id <= 0x0765)  // bg
                    {
                        Compresion.Basico.Decompress2(file, file + ".un");
                        System.IO.File.Delete(file);
                        Object a5fe = Juegos.Layton1.Bg.Obtener_Imagen(file + ".un");
                        System.IO.File.Delete(file + ".un");
                        return a5fe;
                    }
                    #region TXT
                    else if (name.EndsWith("DEBUG"))    // No funciona bien... ya lo arreglaré..xD
                    {
                        string fileLay = Application.StartupPath + "\\temp2.dat";
                        Nitro.Estructuras.File layton = accion.Search_File(0x013D);

                        BinaryReader br = new BinaryReader(File.OpenRead(accion.ROMFile));
                        br.BaseStream.Position = layton.offset;

                        BinaryWriter bw = new BinaryWriter(new FileStream(fileLay, FileMode.Create));
                        bw.Write(br.ReadBytes((int)layton.size));
                        bw.Flush();
                        bw.Close();

                        Compresion.Basico.Decompress2(fileLay, fileLay + ".un");
                        File.Delete(fileLay);

                        string fileFondo = Application.StartupPath + "\\temp3.dat";
                        Nitro.Estructuras.File fondo = accion.Search_File(0x009D);
                        br.BaseStream.Position = fondo.offset;

                        bw = new BinaryWriter(new FileStream(fileFondo, FileMode.Create));
                        bw.Write(br.ReadBytes((int)fondo.size));
                        bw.Flush();
                        bw.Close();
                        bw.Dispose();
                        br.Close();
                        br.Dispose();

                        Compresion.Basico.Decompress2(fileFondo, fileFondo + ".un");
                        File.Delete(fileFondo);

                        Object a5fe = Juegos.Layton1.Txt.Obtener_Animacion(file, fileLay + ".un", fileFondo + ".un");
                        File.Delete(fileLay + ".un");
                        File.Delete(fileFondo + ".un");

                        return a5fe;
                    }
                    #endregion

                    break;
                    #endregion //LAYTON1 USA
            }
            return new String('\0', 1); // Para que se distinga bien de lo demás xD
        }

    }
}

