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
 * By: pleoNeX
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

namespace AI_IGO_DS
{
    public class R00 : MapBase
    {
        PaletteBase palette;
        ImageBase image;
        IPluginHost pluginHost;

        public R00(IPluginHost pluginHost, string file, int id, string fileName = "") : base(file, id, fileName) { this.pluginHost = pluginHost; }

        public Control Get_Control()
        {
            return new ImageControl(pluginHost, image, palette, this);
        }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            // Header
            uint paletaOffset = br.ReadUInt32() * 4;
            uint tileOffset = br.ReadUInt32() * 4;
            uint mapOffset = br.ReadUInt32() * 4;

            // Paleta
            br.BaseStream.Position = paletaOffset;
            uint pCabeceraSize = br.ReadUInt32() * 4;
            uint pSize = br.ReadUInt32() * 4;

            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes((int)(pSize - 0x08)));
            palette = new RawPalette(colors, false, ColorFormat.colors256);

            // Image data
            br.BaseStream.Position = tileOffset;
            uint tCabeceraSize = br.ReadUInt32() * 4;
            uint tSize = br.ReadUInt32() * 4;

            byte[] tiles = br.ReadBytes((int)(tSize - 0x08));
            image = new RawImage(tiles, TileForm.Horizontal, ColorFormat.colors256, 256, 192, false);

            // Map
            br.BaseStream.Position = mapOffset;
            uint mCabeceraSize = br.ReadUInt32() * 4;
            uint mSize = br.ReadUInt32() * 4;

            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();

            NTFS[] map = new NTFS[(mSize - 0x08) / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            br.Close();
            Set_Map(map, false, width * 8, height * 8);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
