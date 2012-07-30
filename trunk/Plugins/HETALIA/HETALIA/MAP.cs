// ----------------------------------------------------------------------
// <copyright file="MAP.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>21/04/2012 2:26:33</date>
// -----------------------------------------------------------------------
using System;
using System.IO;
using Ekona;
using Ekona.Images;

namespace HETALIA
{
    class MAP : MapBase
    {
        IPluginHost pluginHost;

        public MAP(IPluginHost pluginHost, string file, int id, string fileName = "")
        {
            this.id = id;
            this.fileName = fileName;
            this.pluginHost = pluginHost;

            Read(file);
        }

        public override void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            br.ReadChars(4);    // MAP\0
            uint map_size = br.ReadUInt32();

            br.BaseStream.Position = 0x10;

            int width = br.ReadUInt16();
            int height = br.ReadUInt16();
            int tile_width = br.ReadUInt16();
            int tile_height = br.ReadUInt16();

            if (tile_height != tile_width)
                System.Windows.Forms.MessageBox.Show("Tile dimension doesn't agree!\nWidth:" +
                    tile_width.ToString() + " Height: " + tile_height.ToString());

            width *= tile_width;
            height *= tile_height;

            br.BaseStream.Position = 0x20;
            NTFS[] map = new NTFS[(map_size - 0x20) / 2];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = Actions.MapInfo(br.ReadUInt16());
                map[i].nPalette = 0;
            }

            // Get the IMY data
            br.BaseStream.Position = map_size;
            byte[] imy = br.ReadBytes((int)(br.BaseStream.Length - map_size));
            br.Close();

            string imy_file = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            File.WriteAllBytes(imy_file, imy);
            // Read the file
            new IMY(pluginHost, imy_file, id, fileName);

            // Change some image parameters
            ImageBase img = pluginHost.Get_Image();
            int tile_size = tile_width * tile_height * img.BPP / 8;
            // Set a zero tile in the beggining
            byte[] newTiles = new byte[img.Tiles.Length + tile_size];
            Array.Copy(img.Tiles, 0, newTiles, tile_size, img.Tiles.Length);
            img.Set_Tiles(newTiles);

            img.TileSize = tile_width;
            img.FormTile = TileForm.Horizontal;
            pluginHost.Set_Image(img);

            Set_Map(map, false, width, height);
            pluginHost.Set_Map(this);
        }

        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
