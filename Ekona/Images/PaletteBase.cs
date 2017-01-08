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

namespace Ekona.Images
{
    public abstract class PaletteBase
    {
        #region Variables
        protected IPluginHost pluginHost;
        protected String fileName;
        protected int id = -1;
        bool loaded;

        Byte[] original;
        int startByte;

        protected Color[][] palette;
        ColorFormat depth;
        bool canEdit;

        protected Object obj;
        #endregion

        public PaletteBase()
        {
            loaded = false;
        }
        public PaletteBase(Color[][] pal, bool editable, string fileName = "")
        {
            this.fileName = fileName;
            Set_Palette(pal, editable);
        }
        public PaletteBase(string fileIn, int id,  IPluginHost pluginHost, string fileName = "")
        {
            this.pluginHost = pluginHost;
            if (fileName == "")
                this.fileName = System.IO.Path.GetFileName(fileIn);
            else
                this.fileName = fileName;
            this.id = id;

            Read(fileIn);
        }
        public PaletteBase(string fileIn, int id, string fileName = "")
        {
            if (fileName == "")
                this.fileName = System.IO.Path.GetFileName(fileIn);
            else
                this.fileName = fileName;
            this.id = id;

            Read(fileIn);
        }


        public abstract void Read(string fileIn);
        public abstract void Write(string fileOut);

        public Image Get_Image(int index)
        {
            if (index >= palette.Length)
                return null;

            return Actions.Get_Image(palette[index]);
        }

        public void FillColors(int maxColors, int pal_index)
        {
            FillColors(maxColors, pal_index, Color.Black);
        }
        public void FillColors(int maxColors, int pal_index, Color color)
        {
            int old_length = palette[pal_index].Length;
            if (old_length >= maxColors)
                return;

            Color[] newpal = new Color[maxColors];
            Array.Copy(palette[pal_index], newpal, old_length);

            for (int i = old_length; i < maxColors; i++)
                newpal[i] = color;

            palette[pal_index] = newpal;
        }

        private void Change_PaletteDepth(ColorFormat newDepth)
        {
            if (newDepth == depth)
                return;

            depth = newDepth;
            if (depth == ColorFormat.colors256 || depth == ColorFormat.A3I5)
                palette = Actions.Palette_16To256(palette);
            else
                palette = Actions.Palette_256To16(palette);
        }
        private void Change_StartByte(int start)
        {
            if (start < 0 || start >= original.Length)
                return;

            startByte = start;

            // Get the new palette data
            int size = original.Length - start;
            if (size > 0x2000) size = 0x2000;

            Byte[] data = new byte[size];
            Array.Copy(original, start, data, 0, data.Length);
            // Convert it to colors
            List<Color> colors = new List<Color>();
            colors.AddRange(Actions.BGR555ToColor(data));

            int num_colors = (depth == ColorFormat.colors16 ? 0x10 : 0x100);
            bool isExact = (colors.Count % num_colors == 0 ? true : false);
            palette = new Color[(colors.Count / num_colors) + (isExact ? 0 : 1)][];
            for (int i = 0; i < palette.Length; i++)
            {
                int palette_length = i * num_colors + num_colors <= colors.Count ? num_colors : colors.Count - i * num_colors;
                palette[i] = new Color[palette_length];
                Array.Copy(colors.ToArray(), i * num_colors, palette[i], 0, palette_length);
            }
        }

        public void Set_Palette(Color[][] palette, bool editable)
        {
            this.palette = palette;
            canEdit = editable;
            if (palette[0].Length > 16)
                depth = ColorFormat.colors256;
            else
                depth = ColorFormat.colors16;

            loaded = true;

            if (depth == ColorFormat.colors16 && (palette.Length == 1 && palette[0].Length > 0x10))
            {
                Color[][] newColors = new Color[palette[0].Length / 0x10][];
                for (int i = 0; i < newColors.Length; i++)
                {
                    int pal_colors = 0x10;
                    if (i * 0x10 >= palette[0].Length)
                        pal_colors = palette[0].Length - (i - 1) * 0x10;
                    newColors[i] = new Color[pal_colors];
                    Array.Copy(palette[0], i * 0x10, newColors[i], 0, pal_colors);
                }
                this.palette = newColors;
            }

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = Actions.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }
        public void Set_Palette(Color[][] palette, ColorFormat depth, bool editable)
        {
            this.palette = palette;
            canEdit = editable;
            this.depth = depth;

            loaded = true;

            if (depth == ColorFormat.colors16 && (palette.Length == 1 && palette[0].Length > 0x10))
            {
                Color[][] newColors = new Color[palette[0].Length / 0x10][];
                for (int i = 0; i < newColors.Length; i++)
                {
                    int pal_colors = 0x10;
                    if (i * 0x10 >= palette[0].Length)
                        pal_colors = palette[0].Length - (i - 1) * 0x10;
                    newColors[i] = new Color[pal_colors];
                    Array.Copy(palette[0], i * 0x10, newColors[i], 0, pal_colors);
                }
                this.palette = newColors;
            }

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = Actions.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }
        public void Set_Palette(Color[] palette, ColorFormat depth, bool editable)
        {
            Set_Palette(new Color[][] { palette }, depth, editable);
        }
        public void Set_Palette(Color[] palette, int index)
        {
            this.palette[index] = palette;
        }
        public void Set_Palette(PaletteBase new_pal)
        {
            this.palette = new_pal.Palette;
            this.depth = new_pal.Depth;

            loaded = true;

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = Actions.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }
        public void Set_Palette(Color[][] palette)
        {
            this.palette = palette;
            if (palette[0].Length > 16)
                depth = ColorFormat.colors256;
            else
                depth = ColorFormat.colors16;

            loaded = true;


            if (depth == ColorFormat.colors16 && (palette.Length == 1 && palette[0].Length > 0x10))
            {
                Color[][] newColors = new Color[palette[0].Length / 0x10][];
                for (int i = 0; i < newColors.Length; i++)
                {
                    int pal_colors = 0x10;
                    if (i * 0x10 >= palette[0].Length)
                        pal_colors = palette[0].Length - (i - 1) * 0x10;
                    newColors[i] = new Color[pal_colors];
                    Array.Copy(palette[0], i * 0x10, newColors[i], 0, pal_colors);
                }
                this.palette = newColors;
            }

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = Actions.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }

        public bool Has_DuplicatedColors(int index)
        {
            for (int i = 0; i < palette[index].Length; i++)
                for (int j = 0; j < palette[index].Length; j++)
                    if (j != i && palette[index][i] == palette[index][j])
                        return true;

            return false;
        }

        #region Properties
        public int StartByte
        {
            get { return startByte; }
            set { Change_StartByte(value); }
        }
        public ColorFormat Depth
        {
            get { return depth; }
            set { Change_PaletteDepth(value); }
        }
        public int NumberOfPalettes
        {
            get { return palette.Length; }
        }
        public int NumberOfColors
        {
            get
            {
                if (depth == ColorFormat.colors256)
                    return palette[0].Length;
                else
                {
                    int colors = 0;
                    for (int i = 0; i < palette.Length; i++)
                        colors += palette[i].Length;
                    return colors;
                }
            }
        }
        public Color[][] Palette
        {
            get { return palette; }
        }
        public bool CanEdit
        {
            get { return canEdit; }
        }
        public bool Loaded
        {
            get { return loaded; }
        }
        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public int ID
        {
            get { return id; }
        }
        public Byte[] Original
        {
            set { original = value; }
            get { return original; }
        }
        #endregion
    }

}
