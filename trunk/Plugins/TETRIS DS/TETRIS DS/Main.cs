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


        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool EsCompatible()
        {
            if (gameCode == "ATRP")
                return true;
            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            if (nombre.ToUpper().EndsWith(".CZ"))
                return Formato.Imagen;
            else if (nombre.ToUpper().EndsWith(".BDZ") || nombre.ToUpper().EndsWith(".BLZ"))
                return Formato.Imagen;
            else if (nombre.ToUpper().EndsWith(".SLZ"))
                return Formato.Map;
            else if (nombre.ToUpper().EndsWith(".PLZ"))
                return Formato.Paleta;
            else if (nombre.ToUpper().EndsWith(".CLZ") || nombre.ToUpper().EndsWith(".CHR"))
                return Formato.Imagen;
            else if (nombre.ToUpper().EndsWith(".OBJS"))
                return Formato.Celdas;
            else if (nombre.ToUpper().EndsWith(".SRL"))
                return Formato.Sistema;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
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
            Leer(archivo, id);

            if (archivo.ToUpper().EndsWith(".CZ") && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                return new ImageControl(pluginHost, false);
            else if (archivo.ToUpper().EndsWith(".SLZ") &&
                pluginHost.Get_NCGR().cabecera.file_size != 0x00 && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                return new ImageControl(pluginHost, true);
            else if ((archivo.ToUpper().EndsWith(".CLZ") || archivo.ToUpper().EndsWith(".CHR")) && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
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
