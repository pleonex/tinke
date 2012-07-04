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
using System.Drawing;
using System.IO;
using Ekona;
using Ekona.Images;

namespace TOTTEMPEST
{
    public class NBM : ImageBase
    {
        PaletteBase palette;
        IPluginHost pluginHost;

        public NBM(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            uint file_size = (uint)br.BaseStream.Length;

            // Read header values
            ushort depth = br.ReadUInt16();
            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            ushort unknown = br.ReadUInt16();
            ColorFormat format = (depth == 0x01) ? ColorFormat.colors256 : ColorFormat.colors16;

            // Palette
            int palette_length = (depth == 0x01) ? 0x200 : 0x20;
            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes(palette_length));
            palette = new RawPalette(colors, false, format);

            // Tiles
            int tiles_length = width * height;
            if (depth == 0)
                tiles_length /= 2;
            Byte[] tiles = br.ReadBytes(tiles_length);

            br.Close();
            Set_Tiles(tiles, width, height, format, TileForm.Lineal, true);
        }
        public override void Write(string fileout, PaletteBase palette)
        {
            this.palette = palette;
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Header
            bw.Write((ushort)(FormatColor == ColorFormat.colors256 ? 0x01 : 0x00));
            bw.Write((ushort)Width);
            bw.Write((ushort)Height);
            bw.Write((ushort)0x00);

            // Palette section
            bw.Write(Actions.ColorToBGR555(palette.Palette[0]));

            // Tile section
            if (FormatColor == ColorFormat.colors16)
                bw.Write(Ekona.Helper.BitsConverter.BytesToBit4(Tiles));
            else
                bw.Write(Tiles);

            bw.Flush();
            bw.Close();
        }

        public System.Windows.Forms.Control Get_Control()
        {
            return new ImageControl(pluginHost, this, palette);
        }
    }
}
