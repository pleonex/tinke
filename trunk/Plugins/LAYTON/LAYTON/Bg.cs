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
    public class Bg : MapBase
    {
        ImageBase image;
        PaletteBase palette;
        IPluginHost pluginHost;

        public Bg(IPluginHost pluginHost, string file, int id, string fileName = "") : base(file, id, fileName) { this.pluginHost = pluginHost;  }

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
            pluginHost.Decompress(temp);

            // Get the decompressed file
            fileIn = pluginHost.Get_Files().files[0].path;
            File.Delete(temp);

            Get_Image(fileIn);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }

        public void Get_Image(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Palette
            uint num_colors = br.ReadUInt32();
            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes((int)num_colors * 2));

            // Image data
            uint num_tiles = (ushort)br.ReadUInt32();
            byte[] tiles = br.ReadBytes((int)num_tiles * 0x40);

            // Map Info
            ushort width = (ushort)(br.ReadUInt16() * 8);
            ushort height = (ushort)(br.ReadUInt16() * 8);
            NTFS[] map = new NTFS[width * height / 0x40];

            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();

            palette = new RawPalette(colors, false, ColorFormat.colors256);
            image = new RawImage(tiles, TileForm.Horizontal, ColorFormat.colors256, width, height, false);
            Set_Map(map, false, width, height);
        }
        public Control Get_Control()
        {
            return new ImageControl(pluginHost, image, palette, this);
        }
    }
}
