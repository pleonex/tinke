using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace DBK_ULTIMATE
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
            if (gameCode == "TDBJ")
                return true;

            return false;
        }


        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            if (fileName == "archiveDBK.dsa")
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
            if (System.IO.Path.GetFileName(file) == "110archiveDBK.dsa")
                Archive.Unpack_archiveDBK(pluginHost, file);
        }

        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }
    }
}
