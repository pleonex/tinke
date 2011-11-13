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
            Console.WriteLine(Tools.Helper.GetTranslation("NCLR", "S0D"));

            nclr.header.id = br.ReadChars(4);
            nclr.header.endianess = br.ReadUInt16();
            if (nclr.header.endianess == 0xFFFE)
                nclr.header.id.Reverse<char>();
            nclr.header.constant = br.ReadUInt16();
            nclr.header.file_size = br.ReadUInt32();
            nclr.header.header_size = br.ReadUInt16();
            nclr.header.nSection = br.ReadUInt16();
            if (nclr.header.nSection < 1 || nclr.header.nSection > 2)
                Console.WriteLine('\t' + Tools.Helper.GetTranslation("NCLR", "S0E") + nclr.header.nSection.ToString());

            nclr.pltt = Seccion_PLTT(ref br);
            if (br.BaseStream.Length != br.BaseStream.Position)
            {
                if (new String(br.ReadChars(4)) == "PMCP")
                {
                    br.BaseStream.Position -= 4;
                    nclr.pmcp = Section_PMCP(ref br);

                    List<NTFP> palettes = new List<NTFP>();
                    for (int i = 0; i < nclr.pmcp.first_palette_num; i++)
                    {
                        NTFP ntfp = new NTFP();
                        ntfp.colors = new Color[0];
                        palettes.Add(ntfp);
                    }

                    palettes.AddRange(nclr.pltt.palettes);
                    nclr.pltt.palettes = palettes.ToArray();
                }
            }


            br.Close();

            return nclr;
        }
        public static TTLP Seccion_PLTT(ref BinaryReader br)
        {
            TTLP pltt = new TTLP();
            long posIni = br.BaseStream.Position;

            pltt.ID = br.ReadChars(4);
            pltt.length = br.ReadUInt32();
            pltt.depth = (br.ReadUInt16() == 0x00000003) ? ColorDepth.Depth4Bit : ColorDepth.Depth8Bit;
            br.ReadUInt16();
            pltt.unknown1 = br.ReadUInt32();
            pltt.paletteLength = br.ReadUInt32();
            uint colors_startOffset = br.ReadUInt32();
            pltt.nColors = (pltt.depth == ColorDepth.Depth4Bit) ? 0x10 : pltt.paletteLength / 2;

            pltt.palettes = new NTFP[pltt.paletteLength / (pltt.nColors * 2)];
            if (pltt.paletteLength > pltt.length || pltt.paletteLength == 0x00)
                pltt.palettes = new NTFP[pltt.length / (pltt.nColors * 2)];

            Console.WriteLine("\t" + pltt.palettes.Length + ' ' + Tools.Helper.GetTranslation("NCLR", "S0F") +
                ' ' + pltt.nColors + ' ' + Tools.Helper.GetTranslation("NCLR", "S10"));

            br.BaseStream.Position = 0x18 + colors_startOffset;
            for (int i = 0; i < pltt.palettes.Length; i++)
                pltt.palettes[i] = Paleta_NTFP(ref br, pltt.nColors);

            return pltt;
        }
        public static NTFP Paleta_NTFP(ref BinaryReader br, UInt32 colores)
        {
            NTFP ntfp = new NTFP();

            ntfp.colors = Convertir.BGR555(br.ReadBytes((int)colores * 2));

            return ntfp;
        }
        public static PMCP Section_PMCP(ref BinaryReader br)
        {
            PMCP pmcp = new PMCP();
            pmcp.ID = br.ReadChars(4);
            pmcp.blockSize = br.ReadUInt32();
            pmcp.unknown1 = br.ReadUInt16();
            pmcp.unknown2 = br.ReadUInt16();
            pmcp.unknown3 = br.ReadUInt32();
            pmcp.first_palette_num = br.ReadUInt16();

            return pmcp;
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
            nclr.header.id = "UNKN".ToCharArray();
            nclr.header.endianess = 0xFEFF;
            nclr.header.constant = 0x0100;
            nclr.header.file_size = file_size;
            nclr.header.header_size = 0x10;
            // El archivo es PLTT raw, es decir, exclusivamente colores
            nclr.pltt.ID = "UNKN".ToCharArray();
            nclr.pltt.length = file_size;
            nclr.pltt.depth = (file_size > 0x20) ? ColorDepth.Depth8Bit : ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.paletteLength = file_size;
            nclr.pltt.nColors = file_size / 2;
            nclr.pltt.palettes = new NTFP[1];
            // Rellenamos los colores en formato BGR555
            nclr.pltt.palettes[0].colors = Convertir.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            return nclr;
        }

        public static void Escribir(NCLR paleta, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write(paleta.header.id);
            bw.Write(paleta.header.endianess);
            bw.Write(paleta.header.constant);
            bw.Write(paleta.header.file_size);
            bw.Write(paleta.header.header_size);
            bw.Write(paleta.header.nSection);
            bw.Write(paleta.pltt.ID);
            bw.Write(paleta.pltt.length);
            bw.Write((ushort)(paleta.pltt.depth == ColorDepth.Depth4Bit ? 0x03 : 0x04));
            bw.Write((ushort)0x00);
            bw.Write((uint)0x00);
            bw.Write(paleta.pltt.paletteLength);
            bw.Write(0x10); // Colors start offset from 0x14
            for (int i = 0; i < paleta.pltt.palettes.Length; i++)
                bw.Write(Convertir.ColorToBGR555(paleta.pltt.palettes[i].colors));

            bw.Flush();
            bw.Close();
        }
        public static NCLR BitmapToPalette(string bitmap, int paletteIndex = 0)
        {
            NCLR paleta = new NCLR();
            BinaryReader br = new BinaryReader(File.OpenRead(bitmap));
            if (new String(br.ReadChars(2)) != "BM")
                throw new NotSupportedException(Tools.Helper.GetTranslation("NCLR", "S15"));

            paleta.header.id = "RLCN".ToCharArray();
            paleta.header.endianess = 0xFEFF;
            paleta.header.constant = 0x0100;
            paleta.header.header_size = 0x10;
            paleta.header.nSection = 0x01;

            br.BaseStream.Position = 0x1C;
            ushort profundidad = br.ReadUInt16();
            if (profundidad == 0x04)
                paleta.pltt.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            else if (profundidad == 0x08)
                paleta.pltt.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            else
                throw new NotSupportedException(String.Format(Tools.Helper.GetTranslation("NCLR", "S16"), profundidad.ToString()));

            br.BaseStream.Position += 0x10;
            paleta.pltt.nColors = br.ReadUInt32();
            if (paleta.pltt.nColors == 0x00)
                paleta.pltt.nColors = (uint)(profundidad == 0x04 ? 0x10 : 0x0100);

            br.BaseStream.Position += 0x04;
            paleta.pltt.palettes = new NTFP[paletteIndex + 1];
            paleta.pltt.palettes[paletteIndex].colors = new Color[(int)paleta.pltt.nColors];
            for (int i = 0; i < paleta.pltt.nColors; i++)
            {
                Byte[] color = br.ReadBytes(4);
                paleta.pltt.palettes[paletteIndex].colors[i] = Color.FromArgb(color[2], color[1], color[0]);
            }
            // Get the colors with BGR555 encoding (not all colours from bitmap are allowed)
            byte[] temp = Convertir.ColorToBGR555(paleta.pltt.palettes[paletteIndex].colors);
            paleta.pltt.palettes[paletteIndex].colors = Convertir.BGR555(temp);

            paleta.pltt.ID = "TTLP".ToCharArray();
            paleta.pltt.paletteLength = paleta.pltt.nColors * 2;
            paleta.pltt.unknown1 = 0x00;
            paleta.pltt.length = paleta.pltt.paletteLength + 0x18;
            paleta.header.file_size = paleta.pltt.length + paleta.header.header_size;

            br.Close();
            return paleta;
        }

        public static NCLR Read_WinPal(string file, ColorDepth depth)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCLR palette = new NCLR();

            palette.header.id = br.ReadChars(4);  // RIFF
            palette.header.file_size = br.ReadUInt32();
            palette.pltt.ID = br.ReadChars(4);  // PAL
            br.ReadChars(4);    // data
            br.ReadUInt32();   // unknown, always 0x00
            br.ReadUInt16();   // unknown, always 0x0300
            palette.pltt.nColors = br.ReadUInt16();
            palette.pltt.depth = depth;
            uint num_color_per_palette = (depth == ColorDepth.Depth4Bit ? 0x10 : palette.pltt.nColors);
            palette.pltt.paletteLength = num_color_per_palette * 2;

            palette.pltt.palettes = new NTFP[(depth == ColorDepth.Depth4Bit ? palette.pltt.nColors / 0x10 : 1)];
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
            {
                palette.pltt.palettes[i].colors = new Color[num_color_per_palette];
                for (int j = 0; j < num_color_per_palette; j++)
                {
                    Color newColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                    br.ReadByte(); // always 0x00
                    palette.pltt.palettes[i].colors[j] = newColor;
                }
            }

            br.Close();
            return palette;
        }
        public static Color[][] Read_WinPal2(string file, ColorDepth depth)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCLR palette = new NCLR();

            palette.header.id = br.ReadChars(4);  // RIFF
            palette.header.file_size = br.ReadUInt32();
            palette.pltt.ID = br.ReadChars(4);  // PAL
            br.ReadChars(4);    // data
            br.ReadUInt32();   // unknown, always 0x00
            br.ReadUInt16();   // unknown, always 0x0300
            palette.pltt.nColors = br.ReadUInt16();
            palette.pltt.depth = depth;
            uint num_color_per_palette = (depth == ColorDepth.Depth4Bit ? 0x10 : palette.pltt.nColors);
            palette.pltt.paletteLength = num_color_per_palette * 2;

            Color[][] colors = new Color[(depth == ColorDepth.Depth4Bit ? palette.pltt.nColors / 0x10 : 1)][];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color[num_color_per_palette];
                for (int j = 0; j < num_color_per_palette; j++)
                {
                    Color newColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                    br.ReadByte(); // always 0x00
                    colors[i][j] = newColor;
                }
            }

            br.Close();
            return colors;
        }
        public static void Write_WinPal(string fileout, NCLR palette)
        {
            if (File.Exists(fileout))
                File.Delete(fileout);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            int num_colors = 0;
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                num_colors += palette.pltt.palettes[i].colors.Length;

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });
            bw.Write((uint)(0x10 + num_colors * 4));
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });
            bw.Write(new char[] { 'd', 'a', 't', 'a' });
            bw.Write((uint)0x00);
            bw.Write((ushort)0x300);
            bw.Write((ushort)(num_colors));
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
            {
                for (int j = 0; j < palette.pltt.palettes[i].colors.Length; j++)
                {
                    bw.Write(palette.pltt.palettes[i].colors[j].R);
                    bw.Write(palette.pltt.palettes[i].colors[j].G);
                    bw.Write(palette.pltt.palettes[i].colors[j].B);
                    bw.Write((byte)0x00);
                    bw.Flush();
                }
            }

            bw.Close();
        }
        public static void Write_WinPal(string fileout, Color[][] palette)
        {
            if (File.Exists(fileout))
                File.Delete(fileout);
            
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            int num_colors = 0;
            for (int i = 0; i < palette.Length; i++)
                num_colors += palette[i].Length;

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });
            bw.Write((uint)(0x10 + num_colors * 4));
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });
            bw.Write(new char[] { 'd', 'a', 't', 'a' });
            bw.Write((uint)0x00);
            bw.Write((ushort)0x300);
            bw.Write((ushort)(num_colors));
            for (int i = 0; i < palette.Length; i++)
            {
                for (int j = 0; j < palette[i].Length; j++)
                {
                    bw.Write(palette[i][j].R);
                    bw.Write(palette[i][j].G);
                    bw.Write(palette[i][j].B);
                    bw.Write((byte)0x00);
                    bw.Flush();
                }
            }

            bw.Close();
        }

        public static Bitmap Show(Color[] colors)
        {
            Bitmap palette = new Bitmap(160, 160);
            
            // One color is 10x10
            bool end = false;
            for (int i = 0; i < 16 && !end; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (colors.Length == j + 16 * i)
                    {
                        end = true;
                        break;
                    }

                    // Zoom
                    for (int k = 0; k < 10; k++)
                        for (int q = 0; q < 10; q++)
                            palette.SetPixel(j * 10 + q, i * 10 + k,
                                colors[j + 16 * i]);
                }
            }
            return palette;
        }
        public static Bitmap[] Show(NCLR nclr)
        {
            Bitmap[] paletas = new Bitmap[nclr.pltt.palettes.Length];

            for (int p = 0; p < paletas.Length; p++)
            {
                paletas[p] = new Bitmap(160, 160);
                bool fin = false;

                for (int i = 0; i < 16 & !fin; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (nclr.pltt.palettes[p].colors.Length == j + 16 * i)
                        {
                            fin = true;
                            break;
                        }

                        for (int k = 0; k < 10; k++)
                            for (int q = 0; q < 10; q++)
                                paletas[p].SetPixel(j * 10 + q, i * 10 + k,
                                    nclr.pltt.palettes[p].colors[j + 16 * i]);
                    }
                }
            }
            return paletas;
        }
    }
}
