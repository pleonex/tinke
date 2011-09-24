﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace TETRIS_DS
{
    public static class CLZ
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            pluginHost.Decompress(file);
            string dec_file;
            sFolder dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<sFile>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Decompress(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            uint file_size = (uint)new FileInfo(dec_file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));

            NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;
            ncgr.header.id = "CLZ ".ToCharArray();
            ncgr.header.nSection = 1;
            ncgr.header.constant = 0x0100;
            ncgr.header.file_size = file_size;

            ncgr.order = TileOrder.Horizontal;
            ncgr.rahc.nTiles = (file_size / 0x20);
            ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth4Bit;
            ncgr.rahc.nTilesX = 0x0020;
            ncgr.rahc.nTilesY = (ushort)(ncgr.rahc.nTiles / ncgr.rahc.nTilesX);
            if (ncgr.rahc.nTilesY == 0x00)
                ncgr.rahc.nTilesY = 0x01;
            ncgr.rahc.tiledFlag = 0x00000001;
            ncgr.rahc.size_section = file_size;
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPalette = new byte[ncgr.rahc.nTiles];
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                ncgr.rahc.tileData.tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(0x20));
                ncgr.rahc.tileData.nPalette[i] = 0;
            }

            br.Close();
            pluginHost.Set_NCGR(ncgr);
        }
    }
}
