using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace AI_IGO_DS
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool EsCompatible()
        {
            if (gameCode == "AIIJ")
                return true;

            return false;
        }
        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".ANCL"))
                return Formato.Paleta;
            else if (nombre.EndsWith(".ANCG"))
                return Formato.Imagen;
            else if (nombre.EndsWith(".ATEX"))
                return Formato.Imagen;
            else if (nombre.EndsWith(".ANSC"))
                return Formato.Map;
            else if (nombre.EndsWith("FNT.BIN") || nombre.EndsWith(".SRL") || nombre.EndsWith("FAT.BIN") ||
                nombre.EndsWith("ARM9.BIN") || nombre.EndsWith("ARM7.BIN"))
                return Formato.Sistema;
            else if (nombre.EndsWith(".BIN") || nombre.EndsWith(".R00"))
                return Formato.ImagenCompleta;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".ANCL"))
                ANCL.Leer(archivo, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ANCG"))
                ANCG.Leer(archivo, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ANSC"))
                ANSC.Leer(archivo, id, pluginHost);
            else if (archivo.ToUpper().EndsWith(".ATEX"))
                ATEX.Leer(archivo, pluginHost);
        }
        public Control Show_Info(string archivo, int id)
        {
            Leer(archivo, id);

            if (archivo.ToUpper().EndsWith(".ANCL"))
                return new PaletteControl(pluginHost);
            if (archivo.ToUpper().EndsWith(".ANCG") && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                return new BinControl(pluginHost, false);
            if (archivo.ToUpper().EndsWith(".ANSC") && pluginHost.Get_NCLR().cabecera.file_size != 0x00 &&
                pluginHost.Get_NCGR().cabecera.file_size != 0x00)
                return new BinControl(pluginHost, true);
            if (archivo.ToUpper().EndsWith(".BIN"))
                return BIN.Leer(archivo, pluginHost);
            if (archivo.ToUpper().EndsWith(".R00"))
                return R00.Leer(archivo, pluginHost);
            if (archivo.ToUpper().EndsWith(".ATEX") && pluginHost.Get_NCLR().cabecera.file_size != 0x00)
                return new BinControl(pluginHost, false);

            return new Control();
        }
    }
}
