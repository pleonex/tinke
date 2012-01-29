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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PluginInterface.Images
{
    public abstract class ImageBase
    {

        #region Variable definition
        protected string fileName;
        protected int id;
        protected IPluginHost pluginHost;
        bool loaded;

        Byte[] original;
        int startByte;
        int zoom = 1;

        Byte[] tiles;
        Byte[] tilePal;
        int width, height;
        ColorFormat format;
        TileForm tileForm;
        int tile_width;
        bool canEdit;

        bool custom_palette;
        PaletteBase palette;

        Object obj;
        #endregion

        public ImageBase(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public ImageBase(IPluginHost pluginHost, Byte[] tiles, int width, int height, ColorFormat format,
            TileForm tileForm, bool editable)
        {
            this.pluginHost = pluginHost;
            Set_Tiles(tiles, width, height, format, tileForm, editable);
        }
        public ImageBase(IPluginHost pluginHost, string file, int id)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = Path.GetFileName(file);

            Read(file);
        }

        public Image Get_Image()
        {
            Color[][] pal_colors;
            if (custom_palette)
                pal_colors = palette.Palette;
            else if (pluginHost.Get_Palette().Loaded)
                pal_colors = pluginHost.Get_Palette().Palette;
            else
            {
                MessageBox.Show("There isn't palette loaded");
                return new Bitmap(1, 1);
            }

            Byte[] img_tiles;
            if (tileForm == Images.TileForm.Horizontal)
                img_tiles = Actions.LinealToHorizontal(tiles, width / 8, height / 8, tile_width);
            else
                img_tiles = tiles;

            return Actions.Get_Image(img_tiles, tilePal, pal_colors, format, width, height);
        }

        public abstract void Read(string fileIn);
        public abstract void Write(string fileOut);

        public void Change_TileForm(TileForm newForm)
        {
            if (newForm == tileForm)
                return;

            tileForm = newForm;
        }
        public void Change_StartByte(int start)
        {
            if (start < 0 || start >= original.Length)
                return;

            startByte = start;

            Array.Copy(original, start, tiles, 0, original.Length - start);
            tilePal = new byte[tiles.Length];
        }

        public void Set_Tiles(Byte[] tiles, int width, int height, ColorFormat format,
            TileForm form, bool editable)
        {
            this.tiles = tiles;
            this.format = format;
            this.tileForm = form;
            Width = width;
            Height = height;
            this.canEdit = editable;

            tilePal = new byte[tiles.Length];

            zoom = 1;
            startByte = 0;
            loaded = true;

            tile_width = 8;
            if (format == Images.ColorFormat.colors16)
                tile_width = 4;
            else if (format == Images.ColorFormat.colors2)
                tile_width = 1;
            else if (format == Images.ColorFormat.colors4)
                tile_width = 2;
            else if (format == Images.ColorFormat.direct)
                tile_width = 16;


            // Get the original data for changes in startByte
            original = (byte[])tiles.Clone();
        }
        public void Set_Palette(PaletteBase palette)
        {
            this.palette = palette;
            custom_palette = true;
        }

        #region Properties
        public int ID
        {
            get { return id; }
        }
        public String FileName
        {
            get { return fileName; }
        }
        public bool Loaded
        {
            get { return loaded; }
        }
        public bool CanEdit
        {
            get { return canEdit; }
        }

        public int Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }
        public int StartByte
        {
            get { return startByte; }
            set { Change_StartByte(value); }
        }
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
            }
        }
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
            }
        }
        public ColorFormat ColorFormat
        {
            get { return format; }
            set
            {
                format = value;
                if (format == Images.ColorFormat.colors16)
                    tile_width = 4;
                else if (format == Images.ColorFormat.colors2)
                    tile_width = 1;
                else if (format == Images.ColorFormat.colors4)
                    tile_width = 2;
                else if (format == Images.ColorFormat.direct)
                    tile_width = 16;
                else
                    tile_width = 8;
            }
        }
        public TileForm TileForm
        {
            get { return tileForm; }
            set { Change_TileForm(value); }
        }
        public Byte[] Tiles
        {
            get { return tiles; }
        }
        public Byte[] TilesPalette
        {
            get { return tilePal; }
            set { tilePal = value; }
        }
        public int TileWidth
        {
            get { return tile_width; }
        }
        public bool CustomPalette
        {
            get { return custom_palette; }
        }
        public PaletteBase Palette
        {
            get { return palette; }
        }
        #endregion
    }

    public class TestImage : ImageBase
    {
        public TestImage(IPluginHost pluginHost)
            : base(pluginHost)
        {
        }

        public override void Read(string fileIn)
        {
            throw new NotImplementedException();
        }
        public override void Write(string fileOut)
        {
            throw new NotImplementedException();
        }
    }
}
