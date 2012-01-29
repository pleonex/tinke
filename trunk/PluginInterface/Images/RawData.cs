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
            Read(fileIn, false, 0, -1);
        }
        public void Read(string fileIn, bool editable, ColorFormat depth, int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);

            if (fileSize <= 0)
                fileSize = (int)br.BaseStream.Length;

            // Color data
            Color[][] palette = new Color[0][];
            if (depth == ColorFormat.colors256)
            {
                palette = new Color[1][];
                palette[0] = Actions.BGR555ToColor(br.ReadBytes(fileSize));
            }
            else if (depth == ColorFormat.colors16)
            {
                palette = new Color[fileSize / 0x20][];
                for (int i = 0; i < palette.Length; i++)
                    palette[i] = Actions.BGR555ToColor(br.ReadBytes(0x20));
            }

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

            // Color data
            Color[][] palette = new Color[1][];
            palette[0] = Actions.BGR555ToColor(br.ReadBytes(fileSize));

            if (palette[0].Length < 0x100)
                palette = pluginHost.Palette_8bppTo4bpp(palette);

            next_data = br.ReadBytes((int)(br.BaseStream.Length - fileSize));

            br.Close();

            Set_Palette(palette, editable);
        }

        public override void Write_Palette(string fileOut)
        {
            // TODO: write raw palette.
            throw new NotImplementedException();
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

        public override void Read(string fileIn)
        {
            Read(fileIn, TileForm.Horizontal, Images.ColorFormat.colors16, false, 0, -1);
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

        public override void Write(string fileOut)
        {
            // TODO: Write raw images
            throw new NotImplementedException();
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

        public override void Read(string fileIn)
        {
            Read(fileIn, 0, -1, false);
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
                map[i] = pluginHost.MapInfo(br.ReadUInt16());

            next_data = br.ReadBytes((int)(br.BaseStream.Length - file_size));

            int width = (map.Length * 8 >= 0x100 ? 0x100 : map.Length * 8);
            int height = (map.Length / (width / 8)) * 8;

            br.Close();
            Set_Map(map, editable, width, height);
        }

        public override void Write(string fileOut)
        {
            // TODO: write raw map
            throw new NotImplementedException();
        }
    }

}
