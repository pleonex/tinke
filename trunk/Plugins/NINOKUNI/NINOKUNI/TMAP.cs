// ----------------------------------------------------------------------
// <copyright file="TMAP.cs" company="none">

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
// <date>08/06/2012 2:08:13</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;

namespace NINOKUNI
{
    public class TMAP
    {
        uint tile_width;
        uint tile_height;
        Bitmap[][] tiles;

        int id;
        string fileName;
        IPluginHost pluginHost;

        public TMAP(sFile cfile, IPluginHost pluginHost)
        {
            this.id = cfile.id;
            this.fileName = Path.GetFileNameWithoutExtension(cfile.name);
            this.pluginHost = pluginHost;

            Read(cfile.path);
        }
        ~TMAP()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    if (tiles[i][j] is Bitmap)
                        tiles[i][j].Dispose();
                    tiles[i][j] = null;
                }
            }
        }

        void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            char[] type = br.ReadChars(4);
            if (new String(type) != "TMAP")
            {
                br.Close();
                br = null;
                throw new FormatException("Invalid header!");
            }

            uint imgs_layer = br.ReadUInt32();      // Number of images per layer
            tile_width = br.ReadUInt32();       // Number of horizontal images 
            tile_height = br.ReadUInt32();      // Number of vertical images 

            // Layer 1: background
            // Layer 2: layer 1 (it's printed after print the NPC and player, it's over the player)
            // Layer 3: layer 2
            // Layer 4: Collision

            tiles = new Bitmap[4][];
            for (int l = 0; l < 4; l++)
            {
                // For each layer get al the files
                tiles[l] = new Bitmap[imgs_layer];
                for (int i = 0; i < imgs_layer; i++)
                {
                    // Read FAT
                    br.BaseStream.Position = 0x10 + l * imgs_layer * 8 + i * 8;
                    uint offset = br.ReadUInt32();
                    uint size = br.ReadUInt32();

                    if (offset == 0x00 || size == 0x00)
                        continue;

                    // Read file
                    br.BaseStream.Position = offset;
                    byte[] data = br.ReadBytes((int)size);

                    string tempFile = null;
                    if (data[0] == 0x11 || data[0] == 0x30) // Decompress it, LZ11 for BTX0 and RLE for BMP
                    {
                        pluginHost.Decompress(data);
                        sFolder f = pluginHost.Get_Files();
                        if (!(f.files is List<sFile>) || f.files.Count != 1)    // Check if the decompression fails
                        {
                            Console.WriteLine("Problem decompressing file -> l: {0}, i: {1}", l.ToString(), i.ToString());
                            continue;
                        }
                        tempFile = f.files[0].path;

                        // Check if the decomprsesion fails
                        data = File.ReadAllBytes(tempFile);
                        if (data[0] == 0x11 || data[0] == 0x30)
                            continue;
                    }
                    else
                    {
                        tempFile = pluginHost.Get_TempFile();
                        File.WriteAllBytes(tempFile, data);
                    }

                    // Get image
                    string header = new string(Encoding.ASCII.GetChars(data, 0, 4));
                    if (header == "BTX0")
                    {
                        // Call to the 3D plugin to get a bitmap from the texture
                        string[] param = { tempFile, "_3DModels.Main", "", "BTX0" };
                        pluginHost.Call_Plugin(param, -1, 0);
                        if (pluginHost.Get_Object() is Bitmap[])
                        {
                            tiles[l][i] = ((Bitmap[])pluginHost.Get_Object())[0];
                            pluginHost.Set_Object(null);
                        }
                        else
                            Console.WriteLine("Problem getting bitmap image -> l: {0}, i: {1}", l.ToString(), i.ToString());
                    }
                    else if (header.StartsWith("BM"))   // BMP file
                    {
                        try { tiles[l][i] = (Bitmap)Image.FromFile(tempFile); }
                        catch { }
                    }
                    else
                        Console.WriteLine("Unknown file -> l: {0}, i: {1}", l.ToString(), i.ToString());

                }
            }

            br.Close();
            br = null;
        }

        public Bitmap Get_Map(int layer)
        {
            if (layer > 3 || layer < 0)
                return null;

            int width, height, tile_size;
            if (layer != 3)
                tile_size = 64;
            else
                tile_size = 32;
            width = (int)tile_width * tile_size;
            height = (int)tile_height * tile_size;

            Bitmap map = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(map);

            int x = 0, y = height - tile_size;
            for (int i = 0; i < tiles[layer].Length; i++)
            {
                if (tiles[layer][i] != null)
                    graphic.DrawImageUnscaled(tiles[layer][i], x, y);

                y -= tile_size;
                if (y < 0)
                {
                    y = height - tile_size;
                    x += tile_size;
                }
            }

            graphic = null;
            return map;
        }

        public String FileName
        {
            get { return fileName; }
        }
    }
}
