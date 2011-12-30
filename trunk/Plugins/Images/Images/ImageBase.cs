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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace Images
{
    public abstract class ImageBase
    {

        #region Variable definition
        protected string fileName;
        protected int id;
        protected IPluginHost pluginHost;
        bool loaded;

        Byte[] original;
        int startByte;
        int zoom = 1;

        Byte[][] tiles;
        Byte[] tilePal;
        int width, height;
        ColorDepth depth;
        TileOrder tileOrder;
        bool canEdit;

        Object obj;
        #endregion

        public ImageBase(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public ImageBase(IPluginHost pluginHost, Byte[][] tiles, int width, int height, ColorDepth depth,
            TileOrder tileOrder, bool editable)
        {
            this.pluginHost = pluginHost;
            Set_Tiles(tiles, width, height, depth, tileOrder, editable);
        }
        public ImageBase(IPluginHost pluginHost, string file, int id)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = Path.GetFileName(file);

            Read(file);
        }

        public NCGR Get_NCGR()
        {
            NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;
            ncgr.order = tileOrder;

            // Generic header
            ncgr.header.id = "RGCN".ToCharArray();
            ncgr.header.endianess = 0xFFFE;
            ncgr.header.constant = 0x0100;
            ncgr.header.file_size = 1;
            ncgr.header.header_size = 0x10;
            ncgr.header.nSection = 1;

            // RAHC section
            ncgr.rahc.depth = depth;
            ncgr.rahc.id = "RACH".ToCharArray();
            ncgr.rahc.nTiles = (uint)tiles.Length;
            ncgr.rahc.nTilesX = (ushort)width;
            ncgr.rahc.nTilesY = (ushort)height;
            ncgr.rahc.size_section = 1;
            ncgr.rahc.size_tiledata = 1;
            ncgr.rahc.tileData = Get_NTFT();

            return ncgr;
        }
        public NTFT Get_NTFT()
        {
            NTFT ntft = new NTFT();
            ntft.nPalette = tilePal;
            ntft.tiles = tiles;
            return ntft;
        }

        public Image Get_Image(Color[][] palette)
        {
            return pluginHost.Bitmap_NTFT(Get_NTFT(), palette, tileOrder, 0, width, height, zoom);
        }

        public abstract void Read(string fileIn);
        public abstract void Write_Tiles(string fileOut);

        public void Change_TileDepth(ColorDepth newDepth)
        {
            if (newDepth == depth)
                return;

            Byte[] rawTiles = new byte[0];
            if (depth == ColorDepth.Depth4Bit)
                rawTiles = pluginHost.Bit4ToBit8(pluginHost.TilesToBytes(tiles));
            else if (depth == ColorDepth.Depth8Bit)
                rawTiles = pluginHost.TilesToBytes(tiles);
            else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                rawTiles = pluginHost.BitsToBytes(pluginHost.TilesToBytes(tiles));

            depth = newDepth;

            Byte[] newData = new byte[0];
            if (depth == ColorDepth.Depth4Bit)
                newData = pluginHost.Bit8ToBit4(rawTiles);
            else if (depth == ColorDepth.Depth8Bit)
                newData = rawTiles;
            else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                newData = pluginHost.BytesToBits(rawTiles);

            if (tileOrder != TileOrder.NoTiled)
            {
                tiles = pluginHost.BytesToTiles(newData);
                tilePal = new byte[tiles.Length];
            }
            else
            {
                tiles = new Byte[1][];
                tiles[0] = newData;
                tilePal = new byte[tiles[0].Length];
            }


            pluginHost.Set_NCGR(Get_NCGR());
        }
        public void Change_TileOrder(TileOrder newTileOrder)
        {
            if (newTileOrder == tileOrder)
                return;
            this.tileOrder = newTileOrder;

            if (tileOrder == TileOrder.NoTiled)
            {
                Byte[] newData = pluginHost.TilesToBytes(tiles);
                tiles = new byte[1][];
                tiles[0] = newData;
                tilePal = new byte[tiles[0].Length];
                width *= 8;
                height *= 8;
            }
            else
            {
                tiles = pluginHost.BytesToTiles(tiles[0]);
                tilePal = new byte[tiles.Length];
                width /= 8;
                height /= 8;
            }

            pluginHost.Set_NCGR(Get_NCGR());
        }
        public void Change_StartByte(int start)
        {
            if (start < 0 || start >= original.Length)
                return;

            startByte = start;

            // Get the original data and convert it to the correct depth
            Byte[] newData = new byte[0];
            if (depth == ColorDepth.Depth4Bit)
                newData = pluginHost.Bit8ToBit4(original);
            else if (depth == ColorDepth.Depth8Bit)
                newData = original;
            else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                newData = pluginHost.BytesToBits(original);


            if (tileOrder == TileOrder.NoTiled)
            {
                tiles = new byte[1][];
                tiles[0] = new byte[newData.Length - start];
                Array.Copy(newData, start, tiles[0], 0, tiles[0].Length);
                tilePal = new byte[tiles[0].Length];
            }
            else if (tileOrder == TileOrder.Horizontal)
            {
                Byte[] linealData = new byte[newData.Length - start];
                Array.Copy(newData, start, linealData, 0, linealData.Length);
                tiles = pluginHost.BytesToTiles(linealData);
                tilePal = new byte[tiles.Length];
            }

            pluginHost.Set_NCGR(Get_NCGR());
        }

        public void Set_Tiles(Byte[][] tiles, int width, int height, ColorDepth depth,
            TileOrder tileOrder, bool editable)
        {
            this.tiles = tiles;
            this.depth = depth;
            this.tileOrder = tileOrder;
            Width = width;
            Height = height;
            this.canEdit = editable;
            if (tileOrder == TileOrder.Horizontal)
                tilePal = new byte[tiles.Length];
            else
                tilePal = new byte[tiles[0].Length];

            zoom = 1;
            startByte = 0;
            loaded = true;

            // Get the original data for changes in startByte
            List<Byte> tempBytes = new List<byte>();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (depth == ColorDepth.Depth4Bit)
                    tempBytes.AddRange(pluginHost.Bit4ToBit8(tiles[i]));
                else if (depth == ColorDepth.Depth8Bit)
                    tempBytes.AddRange(tiles[i]);
                else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                    tempBytes.AddRange(pluginHost.BitsToBytes(tiles[i]));
            }
            original = tempBytes.ToArray();

            pluginHost.Set_NCGR(Get_NCGR());
        }

        #region Properties
        public int ID
        {
            get { return id; }
        }
        public String FileName
        {
            get { return fileName; }
        }
        public bool Loaded
        {
            get { return loaded; }
        }
        public bool CanEdit
        {
            get { return canEdit; }
        }

        public int Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }
        public int StartByte
        {
            get { return startByte; }
            set { Change_StartByte(value); }
        }
        public int Height
        {
            get { return height * 8; }
            set
            {
                height = value;
                if (tileOrder == TileOrder.Horizontal)
                    height /= 8;

                pluginHost.Set_NCGR(Get_NCGR());
            }
        }
        public int Width
        {
            get { return width * 8; }
            set
            {
                width = value;
                if (tileOrder == TileOrder.Horizontal)
                    width /= 8;

                pluginHost.Set_NCGR(Get_NCGR());
            }
        }
        public ColorDepth Depth
        {
            get { return depth; }
            set { Change_TileDepth(value); }
        }
        public TileOrder TileForm
        {
            get { return tileOrder; }
            set { Change_TileOrder(value); }
        }
        public Byte[][] Tiles
        {
            get { return tiles; }
        }
        public Byte[] TilesPalette
        {
            get { return tilePal; }
            set
            {
                if (tileOrder == TileOrder.NoTiled)
                {
                    if (value.Length == tiles[0].Length)
                        tilePal = value;
                }
                else if (tileOrder == TileOrder.Horizontal)
                    if (value.Length == tiles.Length)
                        tilePal = value;
            }
        }
        #endregion
    }

    public class TestImage : ImageBase
    {
        public TestImage(IPluginHost pluginHost)
            : base(pluginHost)
        {
        }

        public override void Read(string fileIn)
        {
            throw new NotImplementedException();
        }
        public override void Write_Tiles(string fileOut)
        {
            throw new NotImplementedException();
        }
    }

    public class RawImage : ImageBase
    {
        // Unknown data - Needed to write the file
        byte[] prev_data;
        byte[] next_data;

        public RawImage(IPluginHost pluginHost, String file, int id,
            TileOrder tileOrder, ColorDepth depth, bool editable,
            int offset, int size)
            : base(pluginHost)
        {
            this.pluginHost = pluginHost;
            this.id = id;
            this.fileName = Path.GetFileName(file);

            Read(file, tileOrder, depth, editable, offset, size);
        }

        public override void Read(string fileIn)
        {
            Read(fileIn, TileOrder.Horizontal, ColorDepth.Depth4Bit, false, 0, -1);
        }
        public void Read(string fileIn, TileOrder tileOrder, ColorDepth depth, bool editable,
            int offset, int fileSize)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            prev_data = br.ReadBytes(offset);   // Save the previous data to write them then.

            if (fileSize <= 0)
                fileSize = (int)br.BaseStream.Length;

            // Read the tiles
            Byte[][] tiles = new byte[0][];

            if (tileOrder == TileOrder.NoTiled)
            {
                tiles = new byte[1][];

                if (depth == ColorDepth.Depth4Bit)
                    tiles[0] = pluginHost.Bit8ToBit4(br.ReadBytes(fileSize));
                else if (depth == ColorDepth.Depth8Bit)
                    tiles[0] = br.ReadBytes(fileSize);
                else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                    tiles[0] = pluginHost.BytesToBits(br.ReadBytes(fileSize));
            }
            else if (tileOrder == TileOrder.Horizontal)
            {
                if (depth == ColorDepth.Depth4Bit)
                {
                    tiles = new byte[fileSize / 0x20][];
                    for (int i = 0; i < tiles.Length; i++)
                        tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(0x20));
                }
                else if (depth == ColorDepth.Depth8Bit)
                {
                    tiles = new byte[fileSize / 0x40][];
                    for (int i = 0; i < tiles.Length; i++)
                        tiles[i] = br.ReadBytes(0x40);
                }
                else if (depth == ColorDepth.Depth32Bit)    // 1 bpp
                {
                    tiles = new byte[fileSize / 0x08][];
                    for (int i = 0; i < tiles.Length; i++)
                        tiles[i] = pluginHost.BytesToBits(br.ReadBytes(0x08));
                }
            }

            next_data = br.ReadBytes((int)(br.BaseStream.Length - fileSize));   // Save the next data to write them then

            #region Calculate the image size
            int width = (fileSize < 0x100 ? fileSize : 0x0100);
            int height = fileSize / width;

            if (height == 0)
                height = 1;

            if (fileSize == 512)
                width = height = 32;
            #endregion

            br.Close();

            Set_Tiles(tiles, width, height, depth, tileOrder, editable);

        }

        public override void Write_Tiles(string fileOut)
        {
            // TODO: Write raw images
            throw new NotImplementedException();
        }
    }
}
