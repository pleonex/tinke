using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace TETRIS_DS
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;


        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "ATRP")
                return true;
            return false;
        }

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            if (nombre.ToUpper().EndsWith(".CZ"))
                return Format.Tile;
            else if (nombre.ToUpper().EndsWith(".BDZ") || nombre.ToUpper().EndsWith(".BLZ"))
                return Format.Tile;
            else if (nombre.ToUpper().EndsWith(".SLZ"))
                return Format.Map;
            else if (nombre.ToUpper().EndsWith(".PLZ"))
                return Format.Palette;
            else if (nombre.ToUpper().EndsWith(".CLZ") || nombre.ToUpper().EndsWith(".CHR"))
                return Format.Tile;
            else if (nombre.ToUpper().EndsWith(".OBJS"))
                return Format.Cell;
            else if (nombre.ToUpper().EndsWith(".SRL"))
                return Format.System;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".CZ"))
                CZ.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".BDZ") || archivo.ToUpper().EndsWith(".BLZ"))
                BDZ.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".SLZ"))
                SLZ.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".PLZ"))
                PLZ.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".CLZ") || archivo.ToUpper().EndsWith(".CHR"))
                CLZ.Read(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".OBJS"))
                OBJS.Read(archivo, pluginHost);
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            Read(archivo, id);

            if (archivo.ToUpper().EndsWith(".CZ") && pluginHost.Get_NCLR().header.file_size != 0x00)
                return new ImageControl(pluginHost, false);
            else if (archivo.ToUpper().EndsWith(".SLZ") &&
                pluginHost.Get_NCGR().header.file_size != 0x00 && pluginHost.Get_NCLR().header.file_size != 0x00)
                return new ImageControl(pluginHost, true);
            else if ((archivo.ToUpper().EndsWith(".CLZ") || archivo.ToUpper().EndsWith(".CHR")) && pluginHost.Get_NCLR().header.file_size != 0x00)
                return new ImageControl(pluginHost, false);
            else if (archivo.ToUpper().EndsWith(".PLZ"))
                return new PaletteControl(pluginHost);
            else if (archivo.ToUpper().EndsWith(".BDZ") || archivo.ToUpper().EndsWith(".BLZ"))
                return new ImageControl(pluginHost, false);
            else if (archivo.ToUpper().EndsWith(".OBJS"))
                return new CellControl(pluginHost);

            return new System.Windows.Forms.Control();
        }
    }
}
