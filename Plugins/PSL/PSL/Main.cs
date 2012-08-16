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
using Ekona;
using Ekona.Images;

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
			if (gameCode == "VPYJ" || gameCode == "VPYT" || gameCode == "VPYP")
				return true;
			
			return false;
		}
		
		public Format Get_Format(sFile file, byte[] magic)
		{
			string ext = new string(Encoding.ASCII.GetChars(magic));
			uint exti = BitConverter.ToUInt32(magic, 0);

            if (file.id == 0x1D9 || file.id == 0x1DA)
                return Format.FullImage;
            else if (ext == "LINK")
                return Format.Pack;
            else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
                return Format.Pack;
            else if (bigImages.Contains(file.id))
                return Format.Tile;
            else if (bigImages.Contains(file.id - 1))
                return Format.System;
            else if (file.id == 0x20F || file.id == 0x230 || file.id == 0x231)
                return Format.FullImage;
            else if (file.id == 0x210 || file.id == 0x232 || file.id == 0x233)
                return Format.System;

            if (file.id == 0xE6 || file.id == 0xE9)
                return Format.Tile;
            else if (file.id == 0xE7 || file.id == 0xEA)
                return Format.Palette;
			
			return Format.Unknown;
		}
		
		public System.Windows.Forms.Control Show_Info(sFile file)
		{
            // Images from still folder
            if (bigImages.Contains(file.id))
                return new TexSprites(pluginHost, file.path, pluginHost.Search_File(file.id + 1));
            else if (file.id == 0x20F)
                return new GBCS(pluginHost.Search_File(0x210), file.path, pluginHost);
            else if (file.id == 0x230)
                return new GBCS(pluginHost.Search_File(0x232), file.path, pluginHost);
            else if (file.id == 0x231)
                return new GBCS(pluginHost.Search_File(0x233), file.path, pluginHost);

            // Pokemon texture of attack and defense (POKEMON_ATX.ALL and POKEMON_DTX.ALL)
            if (file.id == 0x1D9 || file.id == 0x1DA)
                return new ATDTX(pluginHost, file.id);

            // Effect images
            if (file.id == 0xE7 || file.id == 0xEA)
            {
                Read(file);
                return new PaletteControl(pluginHost);
            }
            else if (file.id == 0xE6 || file.id == 0xE9)
            {
                Read(file);
                return new ImageControl(pluginHost, false);
            }

			return new System.Windows.Forms.Control();
		}		
		public void Read(sFile file)
		{
            if (file.id == 0xE7 || file.id == 0xEA)
            {
                RawPalette palette = new RawPalette(file.path, file.id, false, 0, -1);
                pluginHost.Set_Palette(palette);
            }
            else if (file.id == 0xE6 || file.id == 0xE9)
            {
                RawImage image = new RawImage(file.path, file.id, TileForm.Lineal, 
                    ColorFormat.colors16, false, 0, -1);
                image.Width = 0x80;
                image.Height = 0x200;
                pluginHost.Set_Image(image);
            }
		}
		
		public sFolder Unpack(sFile file)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
			byte[] header = br.ReadBytes(4);
			string ext = new String(Encoding.ASCII.GetChars(header));
			uint exti = BitConverter.ToUInt32(header, 0);
			
			if (ext == "LINK")
				return LINK.Unpack(file.path);
			else if (exti == 0x0040E3C4 || exti == 0x0040E3BC)
				return PAC.Unpack(file.path);
			
			return new sFolder();
		}		
		public string Pack(ref sFolder unpacked, sFile file)
		{
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(file.path));
            byte[] header = br.ReadBytes(4);
            string ext = new String(Encoding.ASCII.GetChars(header));
            uint exti = BitConverter.ToUInt32(header, 0);

            if (ext == "LINK")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetRandomFileName();
                LINK.Pack(file.path, fileOut, ref unpacked);
                return fileOut;
            }

            return null;
		}
	}
}