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
        public static NCLR Leer(string file, int id)
        {
            NCLR nclr = new NCLR();
            nclr.id = (uint)id;

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Console.WriteLine(Tools.Helper.ObtenerTraduccion("NCLR","S0D"));

            nclr.cabecera.id = br.ReadChars(4);
            nclr.cabecera.endianess = br.ReadUInt16();
            if (nclr.cabecera.endianess == 0xFFFE)
                nclr.cabecera.id.Reverse<char>();
            nclr.cabecera.constant = br.ReadUInt16();
            nclr.cabecera.file_size = br.ReadUInt32();
            nclr.cabecera.header_size = br.ReadUInt16();
            nclr.cabecera.nSection = br.ReadUInt16();
            if (nclr.cabecera.nSection < 1 || nclr.cabecera.nSection > 2)
                Console.WriteLine('\t' + Tools.Helper.ObtenerTraduccion("NCLR", "S0E") + nclr.cabecera.nSection.ToString());

            nclr.pltt = Seccion_PLTT(ref br);

            br.Close();

            return nclr;
        }
        public static TTLP Seccion_PLTT(ref BinaryReader br)
        {
            TTLP pltt = new TTLP();
            long posIni = br.BaseStream.Position;

            pltt.ID = br.ReadChars(4);
            pltt.tamaño = br.ReadUInt32();
            pltt.profundidad = (br.ReadUInt16() == 0x00000003) ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;
            br.ReadUInt16();
            pltt.unknown1 = br.ReadUInt32();
            pltt.tamañoPaletas = br.ReadUInt32();
            pltt.nColores = br.ReadUInt32();
            if (pltt.profundidad == ColorDepth.Depth8Bit) pltt.nColores = pltt.tamañoPaletas / 2;

            pltt.paletas = new NTFP[pltt.tamañoPaletas / (pltt.nColores * 2)];
            if (pltt.tamañoPaletas > pltt.tamaño || pltt.tamañoPaletas == 0x00) 
                pltt.paletas = new NTFP[pltt.tamaño / (pltt.nColores * 2)];

            Console.WriteLine("\t" + pltt.paletas.Length + ' ' + Tools.Helper.ObtenerTraduccion("NCLR", "S0F") +
                ' ' + pltt.nColores + ' ' + Tools.Helper.ObtenerTraduccion("NCLR", "S10"));

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
        public static NCLR Leer_Basico(string archivo, int id)
        {
            uint file_size = (uint)new FileInfo(archivo).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            NCLR nclr = new NCLR();
            nclr.id = (uint)id;
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
            nclr.pltt.nColores = file_size / 2;
            nclr.pltt.paletas = new NTFP[1];
            // Rellenamos los colores en formato BGR555
            nclr.pltt.paletas[0].colores = Convertir.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            return nclr;
        }

        public static void Escribir(NCLR paleta, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write(paleta.cabecera.id);
            bw.Write(paleta.cabecera.endianess);
            bw.Write(paleta.cabecera.constant);
            bw.Write(paleta.cabecera.file_size);
            bw.Write(paleta.cabecera.header_size);
            bw.Write(paleta.cabecera.nSection);
            bw.Write(paleta.pltt.ID);
            bw.Write(paleta.pltt.tamaño);
            bw.Write((ushort)(paleta.pltt.profundidad == ColorDepth.Depth4Bit ? 0x03 : 0x04));
            bw.Write((ushort)0x00);
            bw.Write((uint)0x00);
            bw.Write(paleta.pltt.tamañoPaletas);
            bw.Write(paleta.pltt.nColores);
            for (int i = 0; i < paleta.pltt.paletas.Length; i++)
                bw.Write(Convertir.ColorToBGR555(paleta.pltt.paletas[i].colores));

            bw.Flush();
            bw.Close();
        }
        public static NCLR BitmapToPalette(string bitmap)
        {
            NCLR paleta = new NCLR();
            BinaryReader br = new BinaryReader(File.OpenRead(bitmap));
            if (new String(br.ReadChars(2)) != "BM")
                throw new NotSupportedException("Archivo no soportado, no es BITMAP");
            
            paleta.cabecera.id = "RLCN".ToCharArray();
            paleta.cabecera.endianess = 0xFEFF;
            paleta.cabecera.constant = 0x0100;
            paleta.cabecera.header_size = 0x10;
            paleta.cabecera.nSection = 0x01;

            br.BaseStream.Position = 0x1C;
            ushort profundidad = br.ReadUInt16();
            if (profundidad == 0x04)
                paleta.pltt.profundidad = System.Windows.Forms.ColorDepth.Depth4Bit;
            else if (profundidad == 0x08)
                paleta.pltt.profundidad = System.Windows.Forms.ColorDepth.Depth8Bit;
            else
                throw new NotSupportedException("Esta imagen bitmap no contiene paleta de colores pues su profundidad es " + profundidad.ToString());

            br.BaseStream.Position += 0x10;
            paleta.pltt.nColores = br.ReadUInt32();
            if (paleta.pltt.nColores == 0x00)
                paleta.pltt.nColores = (uint)(profundidad == 0x04 ? 0x10 : 0x0100);

            br.BaseStream.Position += 0x04;
            paleta.pltt.paletas = new NTFP[1];
            paleta.pltt.paletas[0].colores = new Color[(int)paleta.pltt.nColores];
            for (int i = 0; i < paleta.pltt.nColores; i++)
            {
                Byte[] color = br.ReadBytes(4);
                paleta.pltt.paletas[0].colores[i] = Color.FromArgb(color[2], color[1], color[0]);
            }

            paleta.pltt.ID = "TTLP".ToCharArray();
            paleta.pltt.tamañoPaletas = paleta.pltt.nColores * 2;
            paleta.pltt.unknown1 = 0x00;
            paleta.pltt.tamaño = paleta.pltt.tamañoPaletas + 0x18;
            paleta.cabecera.file_size = paleta.pltt.tamaño + paleta.cabecera.header_size;

            br.Close();
            return paleta;
        }


        public static Bitmap[] Mostrar(string file)
        {
            return Mostrar(Leer(file, -1));
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
                        if (nclr.pltt.paletas[p].colores.Length == j + 16 * i)
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
