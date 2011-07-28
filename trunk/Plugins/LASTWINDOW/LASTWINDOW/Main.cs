using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace LASTWINDOW
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool EsCompatible()
        {
            if (gameCode == "YLUP")
                return true;

            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".PACK"))
                return Formato.Comprimido;

            return Formato.Desconocido;
        }

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Leer(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".PACK"))
                PACK.Leer(pluginHost, archivo);
        }

        public Control Show_Info(string archivo, int id)
        {
            throw new NotImplementedException();
        }
    }
}
