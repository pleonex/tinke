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
        //public static NCLR BitmapToPalette(string bitmap, int paletteIndex = 0)
        //{
        //    NCLR paleta = new NCLR();
        //    BinaryReader br = new BinaryReader(File.OpenRead(bitmap));
        //    if (new String(br.ReadChars(2)) != "BM")
        //        throw new NotSupportedException(Tools.Helper.GetTranslation("NCLR", "S15"));

        //    paleta.header.id = "RLCN".ToCharArray();
        //    paleta.header.endianess = 0xFEFF;
        //    paleta.header.constant = 0x0100;
        //    paleta.header.header_size = 0x10;
        //    paleta.header.nSection = 0x01;

        //    br.BaseStream.Position = 0x1C;
        //    ushort profundidad = br.ReadUInt16();
        //    if (profundidad == 0x04)
        //        paleta.pltt.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
        //    else if (profundidad == 0x08)
        //        paleta.pltt.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
        //    else
        //        throw new NotSupportedException(String.Format(Tools.Helper.GetTranslation("NCLR", "S16"), profundidad.ToString()));

        //    br.BaseStream.Position += 0x10;
        //    paleta.pltt.nColors = br.ReadUInt32();
        //    if (paleta.pltt.nColors == 0x00)
        //        paleta.pltt.nColors = (uint)(profundidad == 0x04 ? 0x10 : 0x0100);

        //    br.BaseStream.Position += 0x04;
        //    paleta.pltt.palettes = new NTFP[paletteIndex + 1];
        //    paleta.pltt.palettes[paletteIndex].colors = new Color[(int)paleta.pltt.nColors];
        //    for (int i = 0; i < paleta.pltt.nColors; i++)
        //    {
        //        Byte[] color = br.ReadBytes(4);
        //        paleta.pltt.palettes[paletteIndex].colors[i] = Color.FromArgb(color[2], color[1], color[0]);
        //    }
        //    // Get the colors with BGR555 encoding (not all colours from bitmap are allowed)
        //    byte[] temp = Convertir.ColorToBGR555(paleta.pltt.palettes[paletteIndex].colors);
        //    paleta.pltt.palettes[paletteIndex].colors = Convertir.BGR555(temp);

        //    paleta.pltt.ID = "TTLP".ToCharArray();
        //    paleta.pltt.paletteLength = paleta.pltt.nColors * 2;
        //    paleta.pltt.unknown1 = 0x00;
        //    paleta.pltt.length = paleta.pltt.paletteLength + 0x18;
        //    paleta.header.file_size = paleta.pltt.length + paleta.header.header_size;

        //    br.Close();
        //    return paleta;
        //}

        public static Color[][] Read_WinPal2(string file, ColorDepth depth)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            br.ReadChars(4);  // RIFF
            br.ReadUInt32();
            br.ReadChars(4);  // PAL
            br.ReadChars(4);    // data
            br.ReadUInt32();   // unknown, always 0x00
            br.ReadUInt16();   // unknown, always 0x0300
            ushort nColors = br.ReadUInt16();
            uint num_color_per_palette = (depth == ColorDepth.Depth4Bit ? (uint)0x10 : nColors);
            uint paletteLength = num_color_per_palette * 2;

            Color[][] colors = new Color[(depth == ColorDepth.Depth4Bit ? nColors / 0x10 : 1)][];
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
        public static void Write_WinPal(string fileout, Color[] palette)
        {
            if (File.Exists(fileout))
                File.Delete(fileout);

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });        // "RIFF"
            bw.Write((uint)(0x10 + palette.Length * 4));        // file_length - 8
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });        // "PAL "
            bw.Write(new char[] { 'd', 'a', 't', 'a' });        // "data"
            bw.Write((uint)palette.Length * 4 + 4);             // data_size = file_length - 0x14
            bw.Write((ushort)0x0300);                           // version = 00 03
            bw.Write((ushort)(palette.Length));                 // num_colors
            for (int i = 0; i < palette.Length; i++)
            {
                bw.Write(palette[i].R);
                bw.Write(palette[i].G);
                bw.Write(palette[i].B);
                bw.Write((byte)0x00);
                bw.Flush();
            }

            bw.Close();
        }
    }
}
