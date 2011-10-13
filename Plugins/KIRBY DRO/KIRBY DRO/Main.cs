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

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }

        public bool IsCompatible()
        {
            if (gameCode == "AKWE")
                return true;
            else
                return false;
        }
        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".BIN"))
                return Format.FullImage;

            return Format.Unknown;
        }


        public void Read(string archivo, int id)
        {
        }
        public Control Show_Info(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".BIN"))
                return new Bin(archivo, pluginHost).Show_Info();

            return new Control();
        }

        public String Pack(sFolder unpacked, string file, int id) { return null; }
        public sFolder Unpack(string file, int id) { return new sFolder(); }
    }
}
