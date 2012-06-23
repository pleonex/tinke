// ----------------------------------------------------------------------
// <copyright file="RawData.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>23/06/2012 19:04:27</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace PluginInterface.Images
{
    public class RawPalette : PaletteBase
    {
        // Unknown data
        byte[] prev_data;
        byte[] next_data;

        public RawPalette(IPluginHost pluginHost, string file, int id,
            bool editable, ColorFormat depth, int offset, int size)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.fileName = System.IO.Path.GetFileName(file);
            this.id = id;

            Read(file, editable, depth, offset, size);
        }
        public RawPalette(IPluginHost pluginHost, Color[][] colors, bool editable, ColorFormat depth)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            Set_Palette(colors, depth, editable);
        }
        public RawPalette(IPluginHost pluginHost, Color[] colors, bool editable, ColorFormat depth)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            Set_Palette(new Color[][] {colors}, depth, editable);
        }
        public RawPalette(IPluginHost pluginHost, string file, int id,
            bool editable, int offset, int size)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.fileName = System.IO.Path.GetFileName(file);
            this.id = id;

            Read(file, editable, offset, size);
        }


        public override void Read(string fileIn)
        {
            Read(fileIn, true, 0, -1);
        }
        public void Read(string fileIn, bool editable, ColorFormat depth, int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);

            if (fileSize <= 0) fileSize = (int)br.BaseStream.Length;
            if (fileSize > 0x2000) fileSize = 0x2000;

            int palette_length = 0x200;
            if (depth == ColorFormat.colors16 || fileSize < 0x200) palette_length = 0x20;

            // Color data
            Color[][] palette = new Color[fileSize / palette_length][];
            for (int i = 0; i < palette.Length; i++)
                palette[i] = Actions.BGR555ToColor(br.ReadBytes(palette_length));

            next_data = br.ReadBytes((int)(br.BaseStream.Length - fileSize));

            br.Close();

            Set_Palette(palette, depth, editable);
        }
        public void Read(string fileIn, bool editable, int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);

            if (fileSize <= 0)
                fileSize = (int)br.BaseStream.Length;
            int fileSize_ = fileSize;
            if (fileSize > 0x2000) fileSize = 0x2000;

            int palette_length = 0x200;
            if (fileSize < 0x200) palette_length = fileSize;

            // Color data
            Color[][] palette = new Color[fileSize / palette_length][];
            for (int i = 0; i < palette.Length; i++)
                palette[i] = Actions.BGR555ToColor(br.ReadBytes(palette_length));

            next_data = br.ReadBytes((int)(br.BaseStream.Length - fileSize));

            Set_Palette(palette, editable);

            br.BaseStream.Position = offset;
            this.Original = br.ReadBytes(fileSize_);
            br.Close();
        }

        public override void Write(string fileOut)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(prev_data);
            for (int i = 0; i < palette.Length; i++)
                bw.Write(Actions.ColorToBGR555(palette[i]));
            bw.Write(next_data);

            bw.Flush();
            bw.Close();
        }
    }

    public class RawImage : ImageBase
    {
        // Unknown data - Needed to write the file
        byte[] prev_data;
        byte[] next_data;

        public RawImage(IPluginHost pluginHost, String file, int id, TileForm form, ColorFormat format,
            bool editable, int offset, int size) : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = Path.GetFileName(file);

            Read(file, form, format, editable, offset, size);
        }
        public RawImage(IPluginHost pluginHost, String file, int id, TileForm form, ColorFormat format,
            int width, int height, bool editable, int offset, int size) : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = Path.GetFileName(file);

            Read(file, form, format, editable, offset, size);
            this.Width = width;
            this.Height = height;
        }
        public RawImage(IPluginHost pluginHost, byte[] tiles, TileForm form, ColorFormat format, int width, int height,
            bool editable)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            Set_Tiles(tiles, width, height, format, form, editable);
        }


        public override void Read(string fileIn)
        {
            Read(fileIn, TileForm.Horizontal, Images.ColorFormat.colors16, true, 0, -1);
        }
        public void Read(string fileIn, TileForm form, ColorFormat format, bool editable,
            int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);   // Save the previous data to write them then.

            if (fileSize <= 0)
                fileSize = (int)br.BaseStream.Length;

            // Read the tiles
            Byte[] tiles = br.ReadBytes(fileSize);

            next_data = br.ReadBytes((int)(br.BaseStream.Length - fileSize));   // Save the next data to write them then

            #region Calculate the image size
            int width = (fileSize < 0x100 ? fileSize : 0x0100);
            int height = fileSize / width;

            if (height == 0)
                height = 1;

            if (fileSize == 512)
                width = height = 32;
            #endregion

            br.Close();

            Set_Tiles(tiles, width, height, format, form, editable);
        }

        public override void Write(string fileOut, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(prev_data);
            bw.Write(Tiles);
            bw.Write(next_data);

            bw.Flush();
            bw.Close();
        }
    }

    public class RawMap : MapBase
    {
        // Unknown data
        byte[] prev_data;
        byte[] next_data;

        public RawMap(IPluginHost pluginHost, string file, int id,
            int offset, int size, bool editable)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = System.IO.Path.GetFileName(file);

            Read(file, offset, size, editable);
        }
        public RawMap(IPluginHost pluginHost, NTFS[] map, int width, int height, bool editable)
            : base(pluginHost, map, editable, width, height)
        {
        }

        public override void Read(string fileIn)
        {
            Read(fileIn, 0, -1, true);
        }
        public void Read(string fileIn, int offset, int size, bool editable)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);

            int file_size;
            if (size <= 0)
                file_size = (int)br.BaseStream.Length;
            else
                file_size = size;

            NTFS[] map = new NTFS[file_size / 2];
            for (int i = 0; i < map.Length; i++)
                map[i] = Actions.MapInfo(br.ReadUInt16());

            next_data = br.ReadBytes((int)(br.BaseStream.Length - file_size));

            int width = (map.Length * 8 >= 0x100 ? 0x100 : map.Length * 8);
            int height = (map.Length / (width / 8)) * 8;

            br.Close();
            Set_Map(map, editable, width, height);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            bw.Write(prev_data);
            for (int i = 0; i < Map.Length; i++)
                bw.Write(Actions.MapInfo(Map[i]));
            bw.Write(next_data);

            bw.Flush();
            bw.Close();
        }
    }

}
