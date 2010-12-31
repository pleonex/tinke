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

        public static Bitmap Mostrar(string file)
        {
            NCLR nclr = Leer(file);

            Bitmap imagen = new Bitmap((int)nclr.pltt.nColores * 10, nclr.pltt.paletas.Length * 10);
            for (int b = 0; b < nclr.pltt.paletas.Length; b++)
            {
                Color[] colores = new Color[nclr.pltt.paletas.Length * nclr.pltt.nColores];
                for (int j = 0; j < nclr.pltt.paletas.Length; j++)
                    for (int i = 0; i < nclr.pltt.nColores; i++)
                        colores[i + j * nclr.pltt.nColores] = nclr.pltt.paletas[j].colores[i];

                // TODO: Imagen_NCLR.Mostrar    
            }
            return imagen;
        }
        public static Bitmap Mostrar(NCLR nclr)
        {
            Bitmap imagen = new Bitmap((int)nclr.pltt.nColores * 10, nclr.pltt.paletas.Length * 10);
            for (int b = 0; b < nclr.pltt.paletas.Length; b++)
            {
                Color[] colores = new Color[nclr.pltt.paletas.Length * nclr.pltt.nColores];
                for (int j = 0; j < nclr.pltt.paletas.Length; j++)
                    for (int i = 0; i < nclr.pltt.nColores; i++)
                        colores[i + j * nclr.pltt.nColores] = nclr.pltt.paletas[j].colores[i];

                // TODO: Imagen_NCLR.Mostrar
            }
            return imagen;
        }
    }
}
