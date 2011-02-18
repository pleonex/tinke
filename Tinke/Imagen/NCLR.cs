using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NCLR
    {
        public static NCLR Leer(string file)
        {
            NCLR nclr = new NCLR();
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Console.WriteLine("Analizando paleta NCLR:");

            nclr.cabecera.id = br.ReadChars(4);
            nclr.cabecera.endianess = br.ReadUInt16();
            if (nclr.cabecera.endianess == 0xFFFE)
                nclr.cabecera.id.Reverse<char>();
            nclr.cabecera.constant = br.ReadUInt16();
            nclr.cabecera.file_size = br.ReadUInt32();
            nclr.cabecera.header_size = br.ReadUInt16();
            nclr.cabecera.nSection = br.ReadUInt16();
            if (nclr.cabecera.nSection < 1 || nclr.cabecera.nSection > 2)
                Console.WriteLine("\tNo hay secciones o hay de más ¿?: " + nclr.cabecera.nSection.ToString());

            nclr.pltt = Seccion_PLTT(ref br);

            br.Close();
            br.Dispose();

            return nclr;
        }
        public static TTLP Seccion_PLTT(ref BinaryReader br)
        {
            TTLP pltt = new TTLP();
            long posIni = br.BaseStream.Position;

            pltt.ID = br.ReadChars(4);
            pltt.tamaño = br.ReadUInt32();
            pltt.profundidad = (br.ReadUInt32() == 0x00000003) ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;
            pltt.unknown1 = br.ReadUInt32();
            pltt.tamañoPaletas = br.ReadUInt32();
            pltt.nColores = br.ReadUInt32();
            if (pltt.profundidad == ColorDepth.Depth8Bit)
                pltt.nColores = 0x100;
            pltt.paletas = new NTFP[(pltt.tamaño - 0x18) / (pltt.nColores * 2)];
            Console.WriteLine("\t" + pltt.paletas.Length + " paletas encontradas de " + pltt.nColores + " colores.");

            for (int i = 0; i < pltt.paletas.Length; i++)
                pltt.paletas[i] = Paleta_NTFP(ref br, pltt.nColores);

            return pltt;
        }
        public static NTFP Paleta_NTFP(ref BinaryReader br, UInt32 colores)
        {
            NTFP ntfp = new NTFP();
            
            ntfp.colores = Convertir.BGR555(br.ReadBytes((int)colores * 2));
            
            return ntfp;
        }
        
        /// <summary>
        /// Lee un archivo que incluye una paleta raw, es deicr, sólo contiene colores.
        /// La información adicional es inventada.
        /// </summary>
        /// <param name="archivo">Archivo para leer</param>
        /// <returns></returns>
        public static NCLR Leer_Basico(string archivo)
        {
            uint file_size = (uint)new FileInfo(archivo).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            NCLR nclr = new NCLR();
            // Ponemos una cabecera genérica
            nclr.cabecera.id = "NCLR".ToCharArray();
            nclr.cabecera.endianess = 0xFEFF;
            nclr.cabecera.constant = 0x0100;
            nclr.cabecera.file_size = file_size;
            nclr.cabecera.header_size = 0x10;
            // El archivo es PLTT raw, es decir, exclusivamente colores
            nclr.pltt.ID = "PLTT".ToCharArray();
            nclr.pltt.tamaño = file_size;
            nclr.pltt.profundidad = (file_size > 0x20) ? ColorDepth.Depth8Bit : ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.tamañoPaletas = file_size;
            nclr.pltt.paletas = new NTFP[1];
            // Rellenamos los colores en formato BGR555
            nclr.pltt.paletas[0].colores = Convertir.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            br.Dispose();
            return nclr;
        }

        public static Bitmap[] Mostrar(string file)
        {
            return Mostrar(Leer(file));
        }
        public static Bitmap[] Mostrar(NCLR nclr)
        {
            Bitmap[] paletas = new Bitmap[nclr.pltt.paletas.Length];

            for (int p = 0; p < paletas.Length; p++)
            {
                paletas[p] = new Bitmap(160, 160);
                bool fin = false;

                for (int i = 0; i < 16 & !fin; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (nclr.pltt.nColores == j + 16 * i)
                        {
                            fin = true;
                            break;
                        }

                        for (int k = 0; k < 10; k++)
                            for (int q = 0; q < 10; q++)
                                paletas[p].SetPixel(j * 10 + q, i * 10 + k,
                                    nclr.pltt.paletas[p].colores[j + 16 * i]);
                    }
                }
            }
            return paletas;
        }
    }
}
