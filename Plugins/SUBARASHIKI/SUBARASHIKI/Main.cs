using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace SUBARASHIKI
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;


        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }
        public bool IsCompatible()
        {
            if (gameCode == "AWLJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            String ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "pack")
                return Format.Pack;

            return Format.Unknown;
        }



        public void Read(string file, int id)
        {
            Pack.Unpack(file, pluginHost);
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }
    }
}
