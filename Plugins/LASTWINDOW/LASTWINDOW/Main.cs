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

        public bool IsCompatible()
        {
            if (gameCode == "YLUP")
                return true;

            return false;
        }

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".PACK"))
                return Format.Compressed;

            return Format.Unknown;
        }

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Read(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".PACK"))
                PACK.Leer(pluginHost, archivo);
        }

        public Control Show_Info(string archivo, int id)
        {
            return new Control();
        }
    }
}
