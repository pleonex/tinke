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

namespace Ekona.Images
{
    public abstract class ImageBase
    {

        #region Variable definition
        protected IPluginHost pluginHost; // Optional
        protected string fileName;
        protected int id = -1;
        bool loaded;

        Byte[] original;
        int startByte;
        int zoom = 1;

        Byte[] tiles;
        Byte[] tilePal;
        int width, height;
        ColorFormat format;
        TileForm tileForm;
        int tile_size;      // Pixels heigth
        int bpp;
        bool canEdit;

        Object obj;
        #endregion

        public ImageBase()
        {
        }
        public ImageBase(Byte[] tiles, int width, int height, ColorFormat format,
            TileForm tileForm, bool editable, string fileName = "")
        {
            this.fileName = fileName;
            Set_Tiles(tiles, width, height, format, tileForm, editable);
        }
        public ImageBase(string file, int id, string fileName = "")
        {
            this.id = id;
            if (fileName == "")
                this.fileName = Path.GetFileName(file);
            else
                this.fileName = fileName;

            Read(file);
        }
        public ImageBase(string file, int id, IPluginHost pluginHost, string fileName = "")
        {
            this.id = id;
            this.pluginHost = pluginHost;
            if (fileName == "")
                this.fileName = Path.GetFileName(file);
            else
                this.fileName = fileName;

            Read(file);
        }


        public Image Get_Image(PaletteBase palette)
        {
            palette.Depth = format;
            Color[][] pal_colors = palette.Palette;

            Byte[] img_tiles;
            if (tileForm == Images.TileForm.Horizontal)
            {
                if (height < tile_size) height = tile_size;
                img_tiles = Actions.LinealToHorizontal(tiles, width, height, bpp, tile_size);
                tilePal = Actions.LinealToHorizontal(tilePal, width, height, 8, tile_size);
            }
            else
                img_tiles = tiles;

            return Actions.Get_Image(img_tiles, tilePal, pal_colors, format, width, height);
        }

        public abstract void Read(string fileIn);
        public abstract void Write(string fileOut, PaletteBase palette);

        public void Change_StartByte(int start)
        {
            if (start < 0 || start >= original.Length)
                return;

            startByte = start;

            tiles = new byte[original.Length - start];
            Array.Copy(original, start, tiles, 0, tiles.Length);
            tilePal = new byte[tiles.Length * (tile_size / bpp)];
        }

        public void Set_Tiles(Byte[] tiles, int width, int height, ColorFormat format,
            TileForm form, bool editable, int tile_size = 8)
        {
            this.tiles = tiles;
            this.format = format;
            this.tileForm = form;
            this.canEdit = editable;
            this.tile_size = tile_size;

            Width = width;
            Height = height;

            zoom = 1;
            //startByte = 0;
            loaded = true;

            bpp = 8;
            if (format == Images.ColorFormat.colors16)
                bpp = 4;
            else if (format == Images.ColorFormat.colors2)
                bpp = 1;
            else if (format == Images.ColorFormat.colors4)
                bpp = 2;
            else if (format == Images.ColorFormat.direct)
                bpp = 16;
            else if (format == Images.ColorFormat.BGRA32 || format == Images.ColorFormat.ABGR32)
                bpp = 32;

            tilePal = new byte[tiles.Length * (tile_size / bpp)];

            // Get the original data for changes in startByte
            original = (byte[])tiles.Clone();
        }
        public void Set_Tiles(ImageBase new_img)
        {
            this.tiles = new_img.Tiles;
            this.format = new_img.FormatColor;
            this.tileForm = new_img.FormTile;         
            this.tile_size = new_img.tile_size;

            Width = new_img.Width;
            Height = new_img.Height;

            zoom = 1;
            startByte = 0;
            loaded = true;

            bpp = 8;
            if (format == Images.ColorFormat.colors16)
                bpp = 4;
            else if (format == Images.ColorFormat.colors2)
                bpp = 1;
            else if (format == Images.ColorFormat.colors4)
                bpp = 2;
            else if (format == Images.ColorFormat.direct)
                bpp = 16;
            else if (format == Images.ColorFormat.BGRA32 || format == Images.ColorFormat.ABGR32)
                bpp = 32;

            tilePal = new byte[tiles.Length * (tile_size / bpp)];

            // Get the original data for changes in startByte
            original = (byte[])tiles.Clone();
        }
        public void Set_Tiles(Byte[] tiles)
        {
            this.tiles = tiles;

            zoom = 1;
            startByte = 0;
            loaded = true;

            tilePal = new byte[tiles.Length * (tile_size / bpp)];

            // Get the original data for changes in startByte
            original = (byte[])tiles.Clone();
        }


        #region Properties
        public int ID
        {
            get { return id; }
        }
        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
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
                if (tileForm == TileForm.Horizontal || tileForm == TileForm.Vertical)
                {
                    if (this.height < this.tile_size) this.height = this.tile_size;
                    if (this.height % this.tile_size != 0)
                        this.height += this.tile_size - (this.height % this.tile_size);
                }
            }
        }
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                if (tileForm == TileForm.Horizontal || tileForm == TileForm.Vertical)
                {
                    if (this.width < this.tile_size) this.width = this.tile_size;
                    if (this.width % this.tile_size != 0)
                        this.width += this.tile_size - (this.width % this.tile_size);
                }
            }
        }
        public ColorFormat FormatColor
        {
            get { return format; }
            set
            {
                format = value;
                if (format == Images.ColorFormat.colors16)
                    bpp = 4;
                else if (format == Images.ColorFormat.colors2)
                    bpp = 1;
                else if (format == Images.ColorFormat.colors4)
                    bpp = 2;
                else if (format == Images.ColorFormat.direct)
                    bpp = 16;
                else if (format == Images.ColorFormat.BGRA32 || format ==Images.ColorFormat.ABGR32)
                    bpp = 32;
                else
                    bpp = 8;

                Array.Resize(ref tilePal, tiles.Length * (tile_size / bpp));
            }
        }
        public TileForm FormTile
        {
            get { return tileForm; }
            set { tileForm = value; }
        }
        public Byte[] Tiles
        {
            get
            {
                return tiles;
            }
        }
        public Byte[] TilesPalette
        {
            get { return tilePal; }
            set { tilePal = value; }
        }
        public int BPP
        {
            get { return bpp; }
        }
        public int TileSize
        {
            get { return tile_size; }
            set
            {
                tile_size = value;
                Array.Resize(ref tilePal, tiles.Length * (tile_size / bpp));
            }
        }
        public Byte[] Original
        {
            get { return original; }
        }
        #endregion
    }

    public class TestImage : ImageBase
    {
        public TestImage()
            : base()
        {
        }

        public override void Read(string fileIn)
        {
            throw new NotImplementedException();
        }
        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
