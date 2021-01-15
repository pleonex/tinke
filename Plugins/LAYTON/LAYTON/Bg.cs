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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Ekona;
using Ekona.Images;

namespace LAYTON
{
    using System.Drawing.Imaging;

    public class Bg : MapBase
    {
        ImageBase image;
        PaletteBase palette;
        IPluginHost pluginHost;

        public Bg(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.pluginHost = pluginHost;
            this.fileName = fileName;
            this.id = id;

            Read(file);
        }

        public Format Get_Formato(string nombre)
        {
            if (nombre.EndsWith(".ARC") || nombre.EndsWith(".BGX") || nombre.EndsWith(".ARB"))
                return Format.FullImage;

            return Format.Unknown;
        }

        public override void Read(string fileIn)
        {
            // The file is compressed
            string temp = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Byte[] compressFile = new Byte[(new FileInfo(fileIn).Length) - 4];
            Array.Copy(File.ReadAllBytes(fileIn), 4, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(temp, compressFile);
            //pluginHost.Decompress(temp);

            ////// Get the decompressed file
            //fileIn = pluginHost.Get_Files().files[0].path;
            fileIn = Path.GetDirectoryName(temp) + Path.DirectorySeparatorChar + "de" + Path.DirectorySeparatorChar + Path.GetFileName(temp);
            Directory.CreateDirectory(Path.GetDirectoryName(fileIn));
            DSDecmp.Main.Decompress(temp, fileIn, DSDecmp.Main.Get_Format(temp));
            File.Delete(temp);

            Get_Image(fileIn);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            if (image.FormatColor != ColorFormat.colors256)
                throw new Exception("Only 256 colors (16Bpp) images support!");

            NTFS[] map = base.Map;
            byte[] tiles = image.Tiles;
            byte[] colorsData = Actions.ColorToBGR555(palette.Palette[0]);
            int srcColorsCount = palette.Original.Length / 2;
            if (srcColorsCount != palette.NumberOfColors || !Actions.Compare_Array(palette.Original, colorsData))
            {
                // Replaced palette
                if (srcColorsCount <= palette.NumberOfColors
                    && MessageBox.Show(
                        "The changed palette has more colors than the original.\r\n"
                        + "In some cases this can lead to an incorrect display of the game.\r\n\r\n"
                        + "Try to force swap at the original palette?",
                        "Layton Image Import",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Force swaping to the Original palette
                    Color[] colors = Actions.BGR555ToColor(palette.Original);
                    Actions.Swap_Palette(ref tiles, colors, palette.Palette[0], ColorFormat.colors256, decimal.MaxValue);
                    colorsData = palette.Original;
                    palette.Palette[0] = colors;
                }
                else
                {
                    // Add transparent color to the Replaced palette
                    byte[] newColorsData = new byte[colorsData.Length + 2];
                    Array.Copy(palette.Original, 0, newColorsData, 0, 2);
                    Array.Copy(colorsData, 0, newColorsData, 2, colorsData.Length);
                    colorsData = newColorsData;

                    Color[] newColors = new Color[palette.Palette[0].Length + 1];
                    newColors[0] = Actions.BGR555ToColor(palette.Original[0], palette.Original[1]);
                    Array.Copy(palette.Palette[0], 0, newColors, 1, palette.Palette[0].Length);
                    palette.Palette[0] = newColors;

                    for (long i = 0; i < tiles.LongLength; i++) tiles[i]++;
                }
            }

            // Write data
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            bw.BaseStream.SetLength(0);
            bw.Write((uint)(colorsData.LongLength / 2));
            bw.Write(colorsData);
            bw.Write((uint)(tiles.LongLength / 0x40));
            bw.Write(tiles);
            bw.Write((ushort)(image.Width / 8));
            bw.Write((ushort)(image.Height / 8));
            for (int i = 0; i < map.Length; i++) bw.Write(Actions.MapInfo(map[i]));
            bw.Close();

            // Compress data
            string compressedFile = this.pluginHost.Get_TempFile();
            this.pluginHost.Compress(fileOut, compressedFile, FormatCompress.LZ10);

            bw = new BinaryWriter(File.OpenWrite(fileOut));
            bw.BaseStream.SetLength(0);
            bw.Write(2);
            bw.Write(File.ReadAllBytes(compressedFile));
            bw.Close();
            File.Delete(compressedFile);
        }

        public void Get_Image(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Palette
            uint num_colors = br.ReadUInt32();
            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes((int)num_colors * 2));

            // Image data
            uint num_tiles = /*(ushort)*/br.ReadUInt32();
            byte[] tiles = br.ReadBytes((int)num_tiles * 0x40);

            // Map Info
            ushort width = (ushort)(br.ReadUInt16() * 8);
            ushort height = (ushort)(br.ReadUInt16() * 8);
            NTFS[] map = new NTFS[width * height / 0x40];

            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();

            palette = new RawPalette(colors, true, ColorFormat.colors256);
            image = new RawImage(tiles, TileForm.Horizontal, ColorFormat.colors256, width, height, true);
            Set_Map(map, true, width, height);

        }
        public Control Get_Control()
        {
            return new ImageControl(pluginHost, image, palette, this);
        }
    }
}
