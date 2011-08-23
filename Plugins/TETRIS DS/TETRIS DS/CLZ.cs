using System;
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
            pluginHost.Descomprimir(file);
            string dec_file;
            Carpeta dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<Archivo>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Descomprimir(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            uint file_size = (uint)new FileInfo(dec_file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));

            NCGR ncgr = new NCGR();
            ncgr.id = (uint)id;
            ncgr.cabecera.id = "CLZ ".ToCharArray();
            ncgr.cabecera.nSection = 1;
            ncgr.cabecera.constant = 0x0100;
            ncgr.cabecera.file_size = file_size;

            ncgr.orden = Orden_Tiles.Horizontal;
            ncgr.rahc.nTiles = (file_size / 0x40);
            ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
            ncgr.rahc.nTilesX = 0x0020;
            ncgr.rahc.nTilesY = (ushort)(ncgr.rahc.nTiles / ncgr.rahc.nTilesX);
            ncgr.rahc.tiledFlag = 0x00000001;
            ncgr.rahc.size_section = file_size;
            ncgr.rahc.tileData = new NTFT();
            ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
            ncgr.rahc.tileData.tiles = new byte[ncgr.rahc.nTiles][];

            for (int i = 0; i < ncgr.rahc.nTiles; i++)
            {
                ncgr.rahc.tileData.tiles[i] = br.ReadBytes(0x40);
                ncgr.rahc.tileData.nPaleta[i] = 0;
            }

            br.Close();
            pluginHost.Set_NCGR(ncgr);
        }
    }
}
