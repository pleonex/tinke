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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;
using PluginInterface.Images;

namespace Images
{
	public class Main : IPlugin
	{
		IPluginHost pluginHost;
        NANR nanr;  // TEMP

		public void Initialize(IPluginHost pluginHost)
		{
			this.pluginHost = pluginHost;
		}
		
		public Format Get_Format(string name, byte[] magic, int id)
		{
            name = name.ToUpper();

            string ext = "";
            if (magic is Byte[])
                ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            // Palettes
            if (name.EndsWith(".NTFP") || name.EndsWith(".PLT"))
                return Format.Palette;
            if (ext == "NCLR" || ext == "RLCN")
                return Format.Palette;
            if (ext == "NCCL")
                return Format.Palette;
            if (name.EndsWith(".NBFP"))
                return Format.Palette;
            if (name.EndsWith(".NCL.L") && magic[0] != 0x10)
                return Format.Palette;

            // Tiles
            if (ext == "NCCG")
                return Format.Tile;
            if (ext == "RGCN" || ext == "RBCN")
                return Format.Tile;
            if (name.EndsWith(".NTFT") || name.EndsWith(".CHAR"))
                return Format.Tile;
            if (name.EndsWith(".NBFC"))
                return Format.Tile;
            if (name.EndsWith(".NCG.L") && magic[0] != 0x10)
                return Format.Tile;

            // Map
            if (ext == "NCSC")
                return Format.Map;
            if (ext == "RCSN")
                return Format.Map;
            if (name.EndsWith(".NBFS"))
                return Format.Map;
            if (name.EndsWith(".NSC.L") && magic[0] != 0x10)
                return Format.Map;

            // Sprites
            if (name.EndsWith(".NCE.L") && magic[0] != 0x10)
                return Format.Cell;
            if (ext == "RECN")
                return Format.Cell;
            if (ext == "NCOB")
                return Format.Cell;

            // Animations
            if (ext == "RNAN")
                return Format.Animation;

			return Format.Unknown;
		}
		
		public Control Show_Info(string file, int id)
		{
            Format format = Read2(file, id);

            if (format == Format.Palette)
                return new PaletteControl(pluginHost, pluginHost.Get_Palette());

            if (format == Format.Tile && pluginHost.Get_Palette().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette());

            if (format == Format.Map && pluginHost.Get_Palette().Loaded && pluginHost.Get_Image().Loaded)
                return new ImageControl(pluginHost, pluginHost.Get_Image(), pluginHost.Get_Palette(), pluginHost.Get_Map());

            if (format == Format.Cell && pluginHost.Get_Palette().Loaded && pluginHost.Get_Image().Loaded)
                return new SpriteControl(pluginHost, pluginHost.Get_Sprite());

            if (format == Format.Animation && pluginHost.Get_Palette().Loaded && pluginHost.Get_Image().Loaded && pluginHost.Get_Sprite().Loaded)
                return new AnimationControl(pluginHost, nanr);
			
			return new Control();
		}		
		public void Read(string file, int id)
		{
            Read2(file, id);
		}
        public Format Read2(string file, int id)
        {
            string ext = "";
            if (new FileInfo(file).Length >= 4)
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {
                    ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
                    br.Close();
                }
            }

            // Palette
            if (file.ToUpper().EndsWith(".NTFP") || file.ToUpper().EndsWith(".PLT"))
            {
                RawPalette palette = new RawPalette(pluginHost, file, id, false, 0, -1);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (ext == "RLCN")
            {
                PaletteBase palette = new NCLR(pluginHost, file, id);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (ext == "NCCL")
            {
                NCCL palette = new NCCL(pluginHost, file, id);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.ToUpper().EndsWith(".NBFP"))
            {
                RawPalette palette = new RawPalette(pluginHost, file, id, false, 0, -1);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.ToUpper().EndsWith(".NCL.L") && ext[0] != '\x10')
            {
                RawPalette palette = new RawPalette(pluginHost, file, id, false, 0, -1);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }


            // Tile
            if (file.ToUpper().EndsWith(".NTFT"))
            {
                RawImage image = new RawImage(pluginHost, file, id, TileForm.Lineal, ColorFormat.colors256, false,
                    0, -1);
                if (pluginHost.Get_Palette().Depth == ColorFormat.colors16)
                {
                    image.ColorFormat = ColorFormat.colors16;
                    if (image.Height != 32 && image.Width != 32)
                        image.Height *= 2;
                }
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (ext == "RGCN" || ext == "RBCN")
            {
                NCGR ncgr = new NCGR(pluginHost, file, id);
                pluginHost.Set_Image(ncgr);
                return Format.Tile;
            }
            else if (ext == "NCCG")
            {
                NCCG image = new NCCG(pluginHost, file, id);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (file.ToUpper().EndsWith(".NBFC") || file.ToUpper().EndsWith(".CHAR"))
            {
                RawImage image = new RawImage(pluginHost, file, id, TileForm.Horizontal, ColorFormat.colors256, false,
                    0, -1);
                if (pluginHost.Get_Palette().Depth == ColorFormat.colors16)
                {
                    image.ColorFormat = ColorFormat.colors16;
                    if (image.Height != 32 && image.Width != 32)
                        image.Height *= 2;
                }
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (file.ToUpper().EndsWith(".NCG.L") && ext[0] != '\x10')
            {
                RawImage image = new RawImage(pluginHost, file, id, TileForm.Horizontal, ColorFormat.colors256, false,
                    0, -1);
                if (pluginHost.Get_Palette().Depth == ColorFormat.colors16)
                    image.ColorFormat = ColorFormat.colors16;
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            // Map
            if (file.ToUpper().EndsWith(".NBFS"))
            {
                RawMap map = new RawMap(pluginHost, file, id,
                    0, -1, false);
                ImageBase image = pluginHost.Get_Image();

                if (map.Width != 0)
                    image.Width = map.Width;
                if (map.Height != 0)
                    image.Height = map.Height;

                pluginHost.Set_Map(map);
                pluginHost.Set_Image(image);
                return Format.Map;
            }
            else if (ext == "RCSN")
            {
                NSCR nscr = new NSCR(pluginHost, file, id);
                pluginHost.Set_Map(nscr);
                return Format.Map;
            }
            else if (ext == "NCSC")
            {
                NCSC map = new NCSC(pluginHost, file, id);
                ImageBase image = pluginHost.Get_Image();

                if (map.Width != 0)
                    image.Width = map.Width;
                if (map.Height != 0)
                    image.Height = map.Height;

                pluginHost.Set_Image(image);
                pluginHost.Set_Map(map);
                return Format.Map;
            }
            else if (file.ToUpper().EndsWith(".NSC.L") && ext[0] != '\x10')
            {
                RawMap map = new RawMap(pluginHost, file, id, 0, -1, false);
                ImageBase image = pluginHost.Get_Image();

                if (map.Width != 0)
                    image.Width = map.Width;
                if (map.Height != 0)
                    image.Height = map.Height;

                pluginHost.Set_Map(map);
                pluginHost.Set_Image(image);
                return Format.Map;
            }

            // Sprite
            if (ext == "NCOB")
            {
                NCOB sprite = new NCOB(pluginHost, file, id);
                pluginHost.Set_Sprite(sprite);
                return Format.Cell;
            }
            else if (ext == "RECN")
            {
                NCER ncer = new NCER(pluginHost, file, id);
                pluginHost.Set_Sprite(ncer);
                return Format.Cell;
            }

            // Animation
            if (ext == "RNAN")
            {
                nanr = new NANR(pluginHost, file, id);
                return Format.Animation;
            }

            return Format.Unknown;
        }

        public String Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
	}
}