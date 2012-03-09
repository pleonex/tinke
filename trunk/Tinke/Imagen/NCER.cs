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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NCER
    {
        //public static Byte[][] Change_ImageCell(Cell cell, uint blockSize, NCGR newTiles, NCGR oldImage)
        //{
        //    List<Byte[]> result = new List<byte[]>();
        //    List<Byte[]> newImage = new List<byte[]>();
        //    List<Byte> temp = new List<byte>();


        //    #region Get the tile data of the new Cell
        //    for (int ht = 0; ht < 512; ht++)
        //    {
        //        for (int wt = 0; wt < 512; wt++)
        //        {
        //            if (ht >= 256 + cell.obj0.yOffset && ht < 256 + cell.obj0.yOffset + cell.height)
        //            {
        //                if (wt >= 256 + cell.obj1.xOffset && wt < 256 + cell.obj1.xOffset + cell.width)
        //                {
        //                    // Get the tile data
        //                    temp.Add(
        //                    newTiles.rahc.tileData.tiles[0][wt + ht * 512]);
        //                }
        //            }
        //        }
        //    }
        //    if (oldImage.order == TileOrder.Horizontal)
        //        newImage.AddRange(Convertir.BytesToTiles_NoChanged(temp.ToArray(), cell.width / 0x08, cell.height / 0x08));
        //    else
        //        newImage.Add(temp.ToArray());
        //    temp.Clear();
        //    #endregion

        //    if (oldImage.order == TileOrder.Horizontal)
        //    {
        //        uint tileOffset = cell.obj2.tileOffset;
        //        if (blockSize > 4)
        //            blockSize = 4;
        //        if (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
        //            tileOffset = (uint)(tileOffset << (byte)blockSize);
        //        else
        //            tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

        //        for (int i = 0; i < tileOffset; i++)
        //            result.Add(oldImage.rahc.tileData.tiles[i]);

        //        result.AddRange(newImage);

        //        for (int i = (int)(tileOffset + (cell.width * cell.height) / 0x40); i < oldImage.rahc.tileData.tiles.Length; i++)
        //            result.Add(oldImage.rahc.tileData.tiles[i]);
        //    }
        //    else if (oldImage.order == TileOrder.NoTiled)
        //    {
        //        uint tileOffset = cell.obj2.tileOffset;
        //        if (blockSize > 4)
        //            blockSize = 4;
        //        if (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
        //            tileOffset = (uint)(tileOffset << (byte)blockSize);
        //        else
        //            tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

        //        for (int i = 0; i < tileOffset; i++)
        //            temp.Add(oldImage.rahc.tileData.tiles[0][i]);

        //        temp.AddRange(newImage[0]);

        //        for (int i = (int)(tileOffset + cell.width * cell.height); i < oldImage.rahc.tileData.tiles[0].Length; i++)
        //            temp.Add(oldImage.rahc.tileData.tiles[0][i]);

        //        result.Add(temp.ToArray());
        //    }

        //    return result.ToArray();
        //}
        //public static Byte[][] Change_ColorCell(Cell cell, uint blockSize, NCGR image, int oldIndex, int newIndex)
        //{
        //    List<Byte[]> result = new List<byte[]>();
        //    List<Byte> temp = new List<byte>();

        //    if (image.order == TileOrder.Horizontal)
        //    {
        //        uint tileOffset = cell.obj2.tileOffset;
        //        if (blockSize > 4)
        //            blockSize = 4;
        //        if (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
        //            tileOffset = (uint)(tileOffset << (byte)blockSize);
        //        else
        //            tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

        //        for (int i = 0; i < image.rahc.tileData.tiles.Length; i++)
        //        {
        //            if (i >= tileOffset && i < (int)(tileOffset + (cell.width * cell.height) / 0x40))
        //            {
        //                Byte[] tile = new Byte[64];
        //                for (int j = 0; j < 64; j++)
        //                    if (image.rahc.tileData.tiles[i][j] == oldIndex)
        //                        tile[j] = (byte)newIndex;
        //                    else if (image.rahc.tileData.tiles[i][j] == newIndex)
        //                        tile[j] = (byte)oldIndex;
        //                    else
        //                        tile[j] = image.rahc.tileData.tiles[i][j];

        //                result.Add(tile);
        //            }
        //            else
        //                result.Add(image.rahc.tileData.tiles[i]);

        //        }
        //    }
        //    else if (image.order == TileOrder.NoTiled)
        //    {
        //        uint tileOffset = cell.obj2.tileOffset;
        //        if (blockSize > 4)
        //            blockSize = 4;
        //        if (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
        //            tileOffset = (uint)(tileOffset << (byte)blockSize);
        //        else
        //            tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

        //        for (int i = 0; i < image.rahc.tileData.tiles[0].Length; i++)
        //        {
        //            if (i >= tileOffset && i < (int)(tileOffset + cell.width * cell.height))
        //            {
        //                if (image.rahc.tileData.tiles[0][i] == oldIndex)
        //                    temp.Add((byte)newIndex);
        //                else if (image.rahc.tileData.tiles[0][i] == newIndex)
        //                    temp.Add((byte)oldIndex);
        //                else
        //                    temp.Add(image.rahc.tileData.tiles[0][i]);
        //            }
        //            else
        //                temp.Add(image.rahc.tileData.tiles[0][i]);

        //        }
        //        result.Add(temp.ToArray());
        //    }

        //    return result.ToArray();
        //}

    }
}
