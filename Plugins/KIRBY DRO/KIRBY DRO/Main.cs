using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace KIRBY_DRO
{
    public class Main : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }

        public bool EsCompatible()
        {
            if (gameCode == "AKWE")
                return true;
            else
                return false;
        }
        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".BIN"))
                return Formato.ImagenCompleta;

            return Formato.Desconocido;
        }


        public void Leer(string archivo, int id)
        {
            throw new NotImplementedException();
        }

        public Control Show_Info(string archivo, int id)
        {
            if (new FileInfo(archivo).Name.EndsWith(".bin"))
                return new Bin(archivo, pluginHost).Show_Info();

            return new Control();
        }
    }
}
