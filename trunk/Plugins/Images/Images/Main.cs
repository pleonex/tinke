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

namespace Images
{
	public class Main : IPlugin
	{
		IPluginHost pluginHost;

        PaletteBase palette;
        ImageBase image;
        MapBase map;

		public void Initialize(IPluginHost pluginHost)
		{
			this.pluginHost = pluginHost;
		}
		
		public Format Get_Format(string name, byte[] magic)
		{
            name = name.ToUpper();

            string ext = "";
            if (magic is Byte[])
                ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            // Palettes
            if (name.EndsWith(".NTFP") || name.EndsWith(".PLT"))
                return Format.Palette;
            if (ext == "NCCL")
                return Format.Palette;
            if (name.EndsWith(".NBFP"))
                return Format.Palette;

            // Tiles
            if (ext == "NCCG")
                return Format.Tile;
            if (name.EndsWith(".NTFT") || name.EndsWith(".CHAR"))
                return Format.Tile;
            if (name.EndsWith(".NBFC"))
                return Format.Tile;

            // Map
            if (ext == "NCSC")
                return Format.Map;
            if (name.EndsWith(".NBFS"))
                return Format.Map;
			
			return Format.Unknown;
		}
		
		public Control Show_Info(string file, int id)
		{
            Format format = Read2(file, id);

            if (format == Format.Palette)
                return new PaletteControl(pluginHost, palette);

            if (format == Format.Tile && palette.Loaded)
                return new ImageControl(pluginHost, image, palette);

            if (format == Format.Map && palette.Loaded && image.Loaded)
                return new ImageControl(pluginHost, image, palette, map);
			
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
            if (ext == "NCCL")
            {
                palette = new NCCL(pluginHost, file, id);
                return Format.Palette;
            }
            else if (file.ToUpper().EndsWith(".NTFP") || file.ToUpper().EndsWith(".PLT"))
            {
                palette = new ntfp(pluginHost, file, id);
                return Format.Palette;
            }
            else if (file.ToUpper().EndsWith(".NBFP"))
            {
                palette = new nbfp(pluginHost, file, id);
                return Format.Palette;
            }

            // Tile
            if (ext == "NCCG")
            {
                image = new NCCG(pluginHost, file, id);
                return Format.Tile;
            }
            else if (file.ToUpper().EndsWith(".NTFT") || file.ToUpper().EndsWith(".CHAR"))
            {
                image = new ntft(pluginHost, file, id);
                if (palette.Depth == ColorDepth.Depth4Bit)
                    image.Depth = ColorDepth.Depth4Bit;

                return Format.Tile;
            }
            else if (file.ToUpper().EndsWith(".NBFC"))
            {
                image = new nbfc(pluginHost, file, id);
                if (palette.Depth == ColorDepth.Depth4Bit)
                    image.Depth = ColorDepth.Depth4Bit;

                return Format.Tile;
            }

            // Map
            if (ext == "NCSC")
            {
                map = new NCSC(pluginHost, file, id);
                if (map.Width != 0)
                    image.Width = map.Width;
                if (map.Height != 0)
                    image.Height = map.Height;

                return Format.Map;
            }
            else if (file.ToUpper().EndsWith(".NBFS"))
            {
                map = new nbfs(pluginHost, file, id);
                if (map.Width != 0)
                    image.Width = map.Width;
                if (map.Height != 0)
                    image.Height = map.Height;

                return Format.Map;
            }

            return Format.Unknown;
        }

        public String Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
	}
}