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

namespace PluginInterface
{
    public abstract class PaletteBase
    {
        #region Variables
        protected String fileName;
        protected int id;
        protected IPluginHost pluginHost;
        bool loaded;

        Byte[] original;
        int startByte;

        Color[][] palette;
        ColorDepth depth;
        bool canEdit;

        Object obj;
        #endregion

        public PaletteBase(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
            loaded = false;
        }
        public PaletteBase(IPluginHost pluginHost, Color[][] pal, bool editable)
        {
            this.pluginHost = pluginHost;
            Set_Palette(pal, editable);
        }
        public PaletteBase(IPluginHost pluginHost, string fileIn, int id)
        {
            this.pluginHost = pluginHost;
            this.fileName = System.IO.Path.GetFileName(fileIn);
            this.id = id;

            Read(fileIn);
        }

        public abstract void Read(string fileIn);
        public abstract void Write_Palette(string fileOut);

        public NCLR Get_NCLR()
        {
            if (palette.Length == 0)
                return new NCLR();

            NCLR nclr = new NCLR();

            // Nitro generic header
            nclr.id = (uint)id;
            nclr.header.id = "NCLR".ToCharArray();
            nclr.header.endianess = 0xFFFE;
            nclr.header.constant = 0x0100;
            nclr.header.file_size = (uint)palette[0].Length * 2;
            nclr.header.header_size = 0x10;
            nclr.header.nSection = 1;

            // PLTT section
            nclr.pltt.ID = "PLTT".ToCharArray();
            nclr.pltt.length = nclr.header.file_size;
            nclr.pltt.depth = (palette.Length == 1 ? ColorDepth.Depth8Bit : ColorDepth.Depth4Bit);
            nclr.pltt.nColors = (uint)palette[0].Length;
            nclr.pltt.paletteLength = (uint)palette.Length * nclr.pltt.nColors;

            // Colors
            nclr.pltt.palettes = new NTFP[palette.Length];
            for (int i = 0; i < palette.Length; i++)
                nclr.pltt.palettes[i].colors = palette[i];

            return nclr;
        }

        public Image Get_PaletteImage(int index)
        {
            if (index >= palette.Length)
                return null;

            return pluginHost.Bitmaps_NCLR(palette[index]);
        }

        private void Change_PaletteDepth(ColorDepth newDepth)
        {
            if (newDepth == depth)
                return;

            depth = newDepth;
            if (depth == ColorDepth.Depth8Bit)
                palette = pluginHost.Palette_4bppTo8bpp(palette);
            else
                palette = pluginHost.Palette_8bppTo4bpp(palette);

            pluginHost.Set_NCLR(Get_NCLR());
        }
        private void Change_StartByte(int start)
        {
            if (start < 0 || start >= original.Length)
                return;

            startByte = start;

            // Get the new palette data
            Byte[] data = new byte[original.Length - start];
            Array.Copy(original, start, data, 0, data.Length);
            // Convert it to colors
            List<Color> colors = new List<Color>();
            colors.AddRange(pluginHost.BGR555ToColor(data));

            if (depth == ColorDepth.Depth4Bit)
            {
                bool isExact = (colors.Count % 16 == 0 ? true : false);
                palette = new Color[(colors.Count / 16) + (isExact ? 0 : 1)][];
                for (int i = 0; i < palette.Length; i++)
                {
                    int palette_length = i * 16 + 16 <= colors.Count ? 16 : colors.Count - i * 16;
                    palette[i] = new Color[palette_length];
                    Array.Copy(colors.ToArray(), i * 16, palette[i], 0, palette_length);
                }
            }
            else
            {
                palette = new Color[1][];
                palette[0] = colors.ToArray();
            }

            pluginHost.Set_NCLR(Get_NCLR());
        }

        public void Set_Palette(Color[][] palette, bool editable)
        {
            this.palette = palette;
            canEdit = editable;
            if (palette.Length == 1 && palette[0].Length > 16)
                depth = ColorDepth.Depth8Bit;
            else
                depth = ColorDepth.Depth4Bit;

            loaded = true;
            pluginHost.Set_NCLR(Get_NCLR());

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = pluginHost.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }
        public void Set_Palette(Color[][] palette, ColorDepth depth, bool editable)
        {
            this.palette = palette;
            canEdit = editable;
            this.depth = depth;

            loaded = true;
            pluginHost.Set_NCLR(Get_NCLR());

            // Convert the palette to bytes, to store the original palette
            List<Color> colors = new List<Color>();
            for (int i = 0; i < palette.Length; i++)
                colors.AddRange(palette[i]);
            original = pluginHost.ColorToBGR555(colors.ToArray());
            startByte = 0;
        }

        #region Properties
        public int StartByte
        {
            get { return startByte; }
            set { Change_StartByte(value); }
        }
        public ColorDepth Depth
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
            get { return palette[0].Length; }
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
        }
        public int ID
        {
            get { return id; }
        }
        #endregion
    }

    public class RawPalette : PaletteBase
    {
        // Unknown data
        byte[] prev_data;
        byte[] next_data;

        public RawPalette(IPluginHost pluginHost, string file, int id,
            bool editable, ColorDepth depth, int offset, int size)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.fileName = System.IO.Path.GetFileName(file);
            this.id = id;

            Read(file, editable, depth, offset, size);
        }
        public RawPalette(IPluginHost pluginHost, string file, int id,
            bool editable, int offset, int size)     : base(pluginHost)
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
        public void Read(string fileIn, bool editable, ColorDepth depth, int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);

            if (fileSize <= 0)
                fileSize = (int)br.BaseStream.Length;

            // Color data
            Color[][] palette = new Color[0][];
            if (depth == ColorDepth.Depth8Bit)
            {
                palette = new Color[1][];
                palette[0] = pluginHost.BGR555ToColor(br.ReadBytes(fileSize));
            }
            else if (depth == ColorDepth.Depth4Bit)
            {
                palette = new Color[fileSize / 0x20][];
                for (int i = 0; i < palette.Length; i++)
                    palette[i] = pluginHost.BGR555ToColor(br.ReadBytes(0x20));
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
            palette[0] = pluginHost.BGR555ToColor(br.ReadBytes(fileSize));

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
}
