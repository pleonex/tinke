using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Tinke.Imagen.Paleta
{
    public static class NCLR
    {
        public static Estructuras.NCLR Leer(string file)
        {
            Estructuras.NCLR nclr = new Estructuras.NCLR();
            FileStream fs = File.OpenRead(file);
            BinaryReader br = new BinaryReader(fs);
            Console.WriteLine("Analizando paleta NCLR: " + new FileInfo(file).Name);

            nclr.ID = br.ReadChars(4);
            nclr.endianness = br.ReadUInt16();
            if (nclr.endianness == 0xFFFE)
                nclr.ID.Reverse<char>();
            nclr.constante = br.ReadUInt16();
            nclr.tamaño = br.ReadUInt32();
            nclr.tamañoCabecera = br.ReadUInt16();
            if (nclr.tamañoCabecera != 0x10)
                Console.WriteLine("\tEl tamaño de la cabecera No es 0x10: " + nclr.tamañoCabecera.ToString());
            nclr.nSecciones = br.ReadUInt16();
            if (nclr.nSecciones < 1 || nclr.nSecciones > 2)
                Console.WriteLine("\tNo hay secciones o hay de más ¿?: " + nclr.nSecciones.ToString());
            br.BaseStream.Position = nclr.tamañoCabecera;
            char[] ID = br.ReadChars(4);
            if (new String(ID) == "PLTT" || new String(ID) == "TTLP")
                nclr.pltt = Seccion_PLTT(ref br);

            br.Close();
            br.Dispose();
            fs.Dispose();
            return nclr;
        }
        public static Estructuras.TTLP Seccion_PLTT(ref BinaryReader br)
        {
            Estructuras.TTLP pltt = new Estructuras.TTLP();
            long posIni = br.BaseStream.Position;

            pltt.ID = "PLTT".ToCharArray();
            pltt.tamaño = br.ReadUInt32();
            pltt.profundidad = (br.ReadUInt32() == 0x00000003) ? Depth.bits4 : Depth.bits8;
            pltt.constante = br.ReadUInt32();
                if (pltt.constante != 0x0) Console.WriteLine("\tLa constante PLTT errónea: " + pltt.constante.ToString());
            pltt.tamañoPaletas = br.ReadUInt32();
            pltt.nColores = br.ReadUInt32();
            pltt.paletas = new NTFP[(pltt.tamaño - 0x18) / (pltt.nColores * 2)];
            Console.WriteLine("\t" + pltt.paletas.Length + " paletas encontradas.");

            for (int i = 0; i < pltt.paletas.Length; i++)
            {
                pltt.paletas[i] = Paleta_NTFP(ref br, pltt.nColores);
            }

            return pltt;
        }
        public static NTFP Paleta_NTFP(ref BinaryReader br, UInt32 colores)
        {
            NTFP ntfp = new NTFP();
            
            ntfp.colores = Convertidor.BGR555(br.ReadBytes((int)colores * 2));
            
            return ntfp;
        }

        public static Bitmap Mostrar(string file)
        {
            Estructuras.NCLR nclr = Leer(file);

            Bitmap imagen = new Bitmap((int)nclr.pltt.nColores * 10, nclr.pltt.paletas.Length * 10);
            for (int b = 0; b < nclr.pltt.paletas.Length; b++)
            {
                Color[] colores = new Color[nclr.pltt.paletas.Length * nclr.pltt.nColores];
                for (int j = 0; j < nclr.pltt.paletas.Length; j++)
                    for (int i = 0; i < nclr.pltt.nColores; i++)
                        colores[i + j * nclr.pltt.nColores] = nclr.pltt.paletas[j].colores[i];
                    
                bool fin = false;

                for (int i = 0; i < 16 & !fin; i++)
                {
                    for (int j = 0; j < 16 & !fin; j++)
                    {
                        for (int k = 0; k < 10 & !fin; k++)
                        {
                            for (int q = 0; q < 10; q++)
                            {
                                try { imagen.SetPixel(j * 10 + q, i * 10 + k, colores[j + 16 * i]); }
                                catch { fin = true; imagen.SetPixel(j * 10, i * 10 + q, Color.White); }
                            }
                        }
                    }
                }
            }
            return imagen;
        }
        public static Bitmap Mostrar(Estructuras.NCLR nclr)
        {
            Bitmap imagen = new Bitmap((int)nclr.pltt.nColores * 10, nclr.pltt.paletas.Length * 10);
            for (int b = 0; b < nclr.pltt.paletas.Length; b++)
            {
                Color[] colores = new Color[nclr.pltt.paletas.Length * nclr.pltt.nColores];
                for (int j = 0; j < nclr.pltt.paletas.Length; j++)
                    for (int i = 0; i < nclr.pltt.nColores; i++)
                        colores[i + j * nclr.pltt.nColores] = nclr.pltt.paletas[j].colores[i];

                bool fin = false;

                for (int i = 0; i < 16 & !fin; i++)
                {
                    for (int j = 0; j < 16 & !fin; j++)
                    {
                        for (int k = 0; k < 10 & !fin; k++)
                        {
                            for (int q = 0; q < 10; q++)
                            {
                                try { imagen.SetPixel(j * 10 + q, i * 10 + k, colores[j + 16 * i]); }
                                catch { fin = true; }
                            }
                        }
                    }
                }
            }
            return imagen;
        }
    }
}
