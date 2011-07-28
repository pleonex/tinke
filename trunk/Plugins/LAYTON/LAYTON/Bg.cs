using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
{
    public class Bg
    {
        IPluginHost pluginHost;
        string gameCode;
        string archivo;

        public Bg(IPluginHost pluginHost, string gameCode, string archivo)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
            this.archivo = archivo;
        }

        public Formato Get_Formato(string nombre)
        {
            if (nombre.EndsWith(".ARC"))
                return Formato.ImagenCompleta;

            return Formato.Desconocido;
        }

        public void Leer()
        {
        }
        public Control Show_Info()
        {
            // Los archivos tienen compresión LZ77, descomprimimos primero.
            string temp = archivo + "nn"; // Para que no sea detectado como narc
            File.Copy(archivo, temp);
            pluginHost.Descomprimir(temp);
            archivo = pluginHost.Get_Files().files[0].path;
            File.Delete(temp);

            InfoBG control = new InfoBG(pluginHost, Obtener_Background());
            File.Delete(archivo);

            return control;
        }

        public Bitmap Obtener_Background()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            // Paleta, NCLR sin cabecera
            NCLR paleta = new NCLR();
            paleta.pltt.nColores = br.ReadUInt32();
            paleta.pltt.paletas = new NTFP[1];
            paleta.pltt.paletas[0].colores =
                pluginHost.BGR555(br.ReadBytes((int)paleta.pltt.nColores * 2));

            // Tile, sin cabecera
            NCGR tile = new NCGR();
            tile.rahc.depth = ColorDepth.Depth8Bit;
            tile.rahc.nTiles = (ushort)br.ReadUInt32();
            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            for (int i = 0; i < tile.rahc.nTiles; i++)
                tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
            tile.orden = Orden_Tiles.Horizontal;

            // Tile Screen Info
            NSCR screen = new NSCR();
            screen.section.width = (ushort)(br.ReadUInt16() * 8);
            screen.section.height = (ushort)(br.ReadUInt16() * 8);
            tile.rahc.nTilesX = (ushort)(screen.section.width / 8);
            tile.rahc.nTilesY = (ushort)(screen.section.height / 8);
            screen.section.screenData = new NTFS[screen.section.width * screen.section.height / 64];
            for (int i = 0; i < screen.section.width * screen.section.height / 64; i++)
            {
                string bits = pluginHost.BytesToBits(br.ReadBytes(2));

                screen.section.screenData[i] = new NTFS();
                screen.section.screenData[i].nPalette = Convert.ToByte(bits.Substring(0, 4), 2);
                screen.section.screenData[i].yFlip = Convert.ToByte(bits.Substring(4, 1), 2);
                screen.section.screenData[i].xFlip = Convert.ToByte(bits.Substring(5, 1), 2);
                screen.section.screenData[i].nTile = Convert.ToUInt16(bits.Substring(6, 10), 2);
            }

            br.Close();

            tile.rahc.tileData = pluginHost.Transformar_NSCR(screen, tile.rahc.tileData);
            return pluginHost.Bitmap_NCGR(tile, paleta);
        }
    }
}
