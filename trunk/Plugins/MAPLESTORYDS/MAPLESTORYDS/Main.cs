using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace MAPLESTORYDS
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "YMPK")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            if (fileName == "RESOURCE.NXARC")
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
            if (System.IO.Path.GetFileName(file).ToUpper() == "108RESOURCE.NXARC")
                Pack.Unpack(file, pluginHost);
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }
    }
}
