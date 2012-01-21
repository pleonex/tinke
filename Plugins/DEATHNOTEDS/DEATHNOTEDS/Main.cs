using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace DEATHNOTEDS
{
    public class Main : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "YDNJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            if (gameCode == "YDNJ" && id == 0x1)
                return Format.Pack;

            return Format.Unknown;
        }


        public string Pack(ref sFolder unpacked, string file, int id)
        {
            System.Windows.Forms.MessageBox.Show("TODO ;)\nIf you need it please contact with me");
            return "";
        }
        public sFolder Unpack(string file, int id)
        {
            if (gameCode == "YDNJ" && id == 0x01)
                return Packs.Unpack_data(file);

            return new sFolder();
        }

        public void Read(string file, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }

    }
}
