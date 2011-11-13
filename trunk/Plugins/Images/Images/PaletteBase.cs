using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace Images
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
            SetPalette(pal, editable);
        }
        public abstract void WritePalette(string fileOut);

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

        public Image GetPaletteImage(int index)
        {
            if (index >= palette.Length)
                return null;

            return pluginHost.Bitmaps_NCLR(palette[index]);
        }

        private void ChangePaletteDepth(ColorDepth newDepth)
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
        private void ChangeStartByte(int start)
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

        public void SetPalette(Color[][] palette, bool editable)
        {
            this.palette = palette;
            canEdit = editable;
            if (palette.Length == 1)
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

        #region Properties
        public int StartByte
        {
            get { return startByte; }
            set { ChangeStartByte(value); }
        }
        public ColorDepth Depth
        {
            get { return depth; }
            set { ChangePaletteDepth(value); }
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
}
