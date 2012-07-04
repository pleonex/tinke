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
using Ekona;
using Ekona.Images;

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
		
		public Format Get_Format(sFile file, byte[] magic)
		{
            file.name = file.name.ToUpper();

            string ext = "";
            if (magic is Byte[])
                ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            // Palettes
            if (file.name.EndsWith(".NTFP") || file.name.EndsWith(".PLT"))
                return Format.Palette;
            if (ext == "NCLR" || ext == "RLCN")
                return Format.Palette;
            if (ext == "NCCL")
                return Format.Palette;
            if (file.name.EndsWith(".NBFP"))
                return Format.Palette;
            if (file.name.EndsWith(".NCL.L") && magic[0] != 0x10)
                return Format.Palette;

            // Tiles
            if (ext == "NCCG")
                return Format.Tile;
            if (ext == "RGCN" || ext == "RBCN")
                return Format.Tile;
            if (file.name.EndsWith(".NTFT") || file.name.EndsWith(".CHAR"))
                return Format.Tile;
            if (file.name.EndsWith(".NBFC"))
                return Format.Tile;
            if (file.name.EndsWith(".NCG.L") && magic[0] != 0x10)
                return Format.Tile;

            // Map
            if (ext == "NCSC")
                return Format.Map;
            if (ext == "RCSN")
                return Format.Map;
            if (file.name.EndsWith(".NBFS"))
                return Format.Map;
            if (file.name.EndsWith(".NSC.L") && magic[0] != 0x10)
                return Format.Map;

            // Sprites
            if (file.name.EndsWith(".NCE.L") && magic[0] != 0x10)
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
		
		public Control Show_Info(sFile file)
		{
            Format format = Read2(file);

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
		public void Read(sFile file)
		{
            Read2(file);
		}
        public Format Read2(sFile file)
        {
            string ext = "";
            if (file.size >= 4)
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file.path)))
                {
                    ext = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
                    br.Close();
                }
            }

            // Palette
            if (file.name.ToUpper().EndsWith(".NTFP") || file.name.ToUpper().EndsWith(".PLT"))
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (ext == "RLCN")
            {
                PaletteBase palette = new NCLR(file.path, file.id, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (ext == "NCCL")
            {
                NCCL palette = new NCCL(file.path, file.id, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.name.ToUpper().EndsWith(".NBFP"))
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }
            else if (file.name.ToUpper().EndsWith(".NCL.L") && ext[0] != '\x10')
            {
                RawPalette palette = new RawPalette(file.path, file.id, true, 0, -1, file.name);
                pluginHost.Set_Palette(palette);
                return Format.Palette;
            }


            // Tile
            ColorFormat depth = ColorFormat.colors256;
            if (pluginHost.Get_Palette().Loaded)
                depth = pluginHost.Get_Palette().Depth;

            if (file.name.ToUpper().EndsWith(".NTFT"))
            {

                RawImage image = new RawImage(file.path, file.id, TileForm.Lineal, depth, true, 0, -1, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (ext == "RGCN" || ext == "RBCN")
            {
                NCGR ncgr = new NCGR(file.path, file.id, file.name);
                pluginHost.Set_Image(ncgr);
                return Format.Tile;
            }
            else if (ext == "NCCG")
            {
                NCCG image = new NCCG(file.path, file.id, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (file.name.ToUpper().EndsWith(".NBFC") || file.name.ToUpper().EndsWith(".CHAR"))
            {
                RawImage image = new RawImage(file.path, file.id, TileForm.Horizontal, depth, true, 0, -1, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }
            else if (file.name.ToUpper().EndsWith(".NCG.L") && ext[0] != '\x10')
            {
                RawImage image = new RawImage(file.path, file.id, TileForm.Horizontal, depth, true, 0, -1, file.name);
                pluginHost.Set_Image(image);
                return Format.Tile;
            }

            // Map
            if (file.name.ToUpper().EndsWith(".NBFS"))
            {
                RawMap map = new RawMap(file.path, file.id, 0, -1, true, file.name);
                pluginHost.Set_Map(map);
                return Format.Map;
            }
            else if (ext == "RCSN")
            {
                NSCR nscr = new NSCR(file.path, file.id, file.name);
                pluginHost.Set_Map(nscr);
                return Format.Map;
            }
            else if (ext == "NCSC")
            {
                NCSC map = new NCSC(file.path, file.id, file.name);
                pluginHost.Set_Map(map);
                return Format.Map;
            }
            else if (file.name.ToUpper().EndsWith(".NSC.L") && ext[0] != '\x10')
            {
                RawMap map = new RawMap(file.path, file.id, 0, -1, true, file.name);
                pluginHost.Set_Map(map);
                return Format.Map;
            }

            // Sprite
            if (ext == "NCOB")
            {
                NCOB sprite = new NCOB(file.path, file.id, file.name);
                pluginHost.Set_Sprite(sprite);
                pluginHost.Set_Image(sprite.Image);
                return Format.Cell;
            }
            else if (ext == "RECN")
            {
                NCER ncer = new NCER(file.path, file.id, file.name);
                pluginHost.Set_Sprite(ncer);
                return Format.Cell;
            }

            // Animation
            if (ext == "RNAN")
            {
                nanr = new NANR(pluginHost, file.path, file.id);
                return Format.Animation;
            }

            return Format.Unknown;
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
	}
}