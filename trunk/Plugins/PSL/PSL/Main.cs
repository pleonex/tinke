/*
 * Copyright (C) 2012  pleonex
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
 * Date: 16/03/2012
 *
 */
using System;
using System.Text;
using System.Collections.Generic;
using PluginInterface;
using PluginInterface.Images;

namespace PSL
{

	public class Main : IGamePlugin
	{
		IPluginHost pluginHost;
		string gameCode;
        List<int> bigImages;

		public void Initialize(IPluginHost pluginHost, string gameCode)
		{
			this.pluginHost = pluginHost;
			this.gameCode = gameCode;

            bigImages = new List<int>();
            bigImages.Add(0x1E8);
            bigImages.Add(0x1EB);
            bigImages.Add(0x1ED);
            bigImages.Add(0x1EE);
            bigImages.Add(0x1F0);
            bigImages.Add(0x1F3);
            bigImages.Add(0x1F5);
            bigImages.Add(0x1F8);
            bigImages.Add(0x1FA);
            bigImages.Add(0x1FD);
            bigImages.Add(0x1FF);
            bigImages.Add(0x202);
            bigImages.Add(0x204);
            bigImages.Add(0x207);
            bigImages.Add(0x20A);
            bigImages.Add(0x20D);
            bigImages.Add(0x212);
            bigImages.Add(0x214);
            bigImages.Add(0x217);
            bigImages.Add(0x21A);
            bigImages.Add(0x21C);
            bigImages.Add(0x21F);
            bigImages.Add(0x221);
            bigImages.Add(0x224);
            bigImages.Add(0x226);
            bigImages.Add(0x229);
            bigImages.Add(0x22B);
            bigImages.Add(0x22E);
            bigImages.Add(0x235);
		}
		
		public bool IsCompatible()
		{
			if (gameCode == "VPYJ")
				return true;
			
			return false;
		}
		
		public Format Get_Format(string fileName, byte[] magic, int id)
		{
			string ext = new string(Encoding.ASCII.GetChars(magic));
			uint exti = BitConverter.ToUInt32(magic, 0);

            if (id == 0x1D9 || id == 0x1DA)
                return Format.FullImage;
            else if (ext == "LINK")
                return Format.Pack;
            else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
                return Format.Pack;
            else if (bigImages.Contains(id))
                return Format.Tile;
            else if (bigImages.Contains(id - 1))
                return Format.System;
            else if (id == 0x20F || id == 0x230 || id == 0x231)
                return Format.FullImage;
            else if (id == 0x210 || id == 0x232 || id == 0x233)
                return Format.System;

            if (id == 0xE6 || id == 0xE9)
                return Format.Tile;
            else if (id == 0xE7 || id == 0xEA)
                return Format.Palette;
			
			return Format.Unknown;
		}
		
		public System.Windows.Forms.Control Show_Info(string file, int id)
		{
            // Images from still folder
            if (bigImages.Contains(id))
                return new TexSprites(pluginHost, file, pluginHost.Search_File(id + 1));
            else if (id == 0x20F)
                return new GBCS(pluginHost.Search_File(0x210), file, pluginHost);
            else if (id == 0x230)
                return new GBCS(pluginHost.Search_File(0x232), file, pluginHost);
            else if (id == 0x231)
                return new GBCS(pluginHost.Search_File(0x233), file, pluginHost);

            // Pokemon texture of attack and defense (POKEMON_ATX.ALL and POKEMON_DTX.ALL)
            if (id == 0x1D9 || id == 0x1DA)
                return new ATDTX(pluginHost, id);

            // Effect images
            if (id == 0xE7 || id == 0xEA)
            {
                Read(file, id);
                return new PaletteControl(pluginHost);
            }
            else if (id == 0xE6 || id == 0xE9)
            {
                Read(file, id);
                return new ImageControl(pluginHost, false);
            }

			return new System.Windows.Forms.Control();
		}		
		public void Read(string file, int id)
		{
            if (id == 0xE7 || id == 0xEA)
            {
                RawPalette palette = new RawPalette(pluginHost, file, id, false, 0, -1);
                pluginHost.Set_Palette(palette);
            }
            else if (id == 0xE6 || id == 0xE9)
            {
                RawImage image = new RawImage(pluginHost, file, id, TileForm.Lineal, 
                    ColorFormat.colors16, false, 0, -1);
                image.Width = 0x80;
                image.Height = 0x200;
                pluginHost.Set_Image(image);
            }
		}
		
		public sFolder Unpack(string file, int id)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
			byte[] header = br.ReadBytes(4);
			string ext = new String(Encoding.ASCII.GetChars(header));
			uint exti = BitConverter.ToUInt32(header, 0);
			
			if (ext == "LINK")
				return LINK.Unpack(file);
			else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
				return PAC.Unpack(file);
			
			return new sFolder();
		}		
		public string Pack(ref sFolder unpacked, string file, int id)
		{
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file));
            byte[] header = br.ReadBytes(4);
            string ext = new String(Encoding.ASCII.GetChars(header));
            uint exti = BitConverter.ToUInt32(header, 0);

            if (ext == "LINK")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetRandomFileName() +
                    System.IO.Path.GetFileName(file);
                LINK.Pack(file, fileOut, ref unpacked);
                return fileOut;
            }

            return null;
		}
	}
}