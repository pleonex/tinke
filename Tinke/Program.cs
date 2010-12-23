using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Tinke
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (args.Length != 2)
                Application.Run(new Form1());
            else if (args.Length == 2)
                Application.Run(new Form1(args[0], Convert.ToInt32(args[1])));
            
        }

        static void Info()
        {
            Console.WriteLine("¡Bienvenido a Tinke!, programa dedicado a la Scene o ingeniería inversa para la Consola Nintendo DS / DSL / DSi");
            Console.WriteLine("Este programa es totalmente gratuito y OpenSource, el código viene con el programa");
            Console.WriteLine("Información de uso del programa y sus herramientas:\n\n");
            Console.WriteLine("\tNarcTool 0.3 .NET:");
            Console.WriteLine("\t\tDesempaquetar archivos NARC: tinke narctool u Archivo.narc CarpetaDestino");
            Console.WriteLine("\tLZ77 0.1 .NET:");
            Console.WriteLine("\t\tDescomprimir archivos de compresión LZ77: tinke lz77 u archivo destino");
        }
    }
}
