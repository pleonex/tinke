using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Nintendo
{
    public class ntft
    {
        IPluginHost pluginsHost;
		string archivo;
		
		public ntft(IPluginHost pluginHost, string archivo)
		{
			this.pluginsHost = pluginHost;
			this.archivo = archivo;
		}
		
		public void Leer()
		{
			BinaryReader br = new BinaryReader(File.OpenRead(archivo));
			uint file_size = (uint)new FileInfo(archivo).Length;

			// Creamos un archivo NCGR genérico.
			NCGR ncgr = new NCGR();
			ncgr.cabecera.id = "NCGR".ToCharArray();
			ncgr.cabecera.nSection = 1;
			ncgr.cabecera.constant = 0x0100;
			ncgr.cabecera.file_size = file_size;
			// El archivo es NTFT raw, sin ninguna información.
			ncgr.orden = Orden_Tiles.No_Tiles;
			ncgr.rahc.nTiles = (ushort)(0xC000);
			ncgr.rahc.depth = System.Windows.Forms.ColorDepth.Depth8Bit;
			ncgr.rahc.nTilesX = 0x0100;
			ncgr.rahc.nTilesY = 0x00C0;
			ncgr.rahc.tiledFlag = 0x00000001;
			ncgr.rahc.size_section = file_size;
			ncgr.rahc.tileData = new NTFT();
			ncgr.rahc.tileData.nPaleta = new byte[ncgr.rahc.nTiles];
			ncgr.rahc.tileData.tiles = new byte[1][];
            ncgr.rahc.tileData.tiles[0] = br.ReadBytes(ncgr.rahc.nTiles);

			for (int i = 0; i < ncgr.rahc.nTiles; i++)
			{
				ncgr.rahc.tileData.nPaleta[i] = 0;
			}
			
			br.Close();
			pluginsHost.Set_NCGR(ncgr);
		}
    }
}
