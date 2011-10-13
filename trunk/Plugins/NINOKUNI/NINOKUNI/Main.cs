using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace NINOKUNI
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
            if (gameCode == "B2KJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            String ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "NPCK")
                return Format.Pack;

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string file, int id)
        {
            return new System.Windows.Forms.Control();
        }

        public String Pack(sFolder unpacked, string file, int id)
        {
            if (file.ToUpper().EndsWith(".N2D"))
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                    "pack_" + System.IO.Path.GetFileName(file);
                
                NPCK.Pack(fileOut, id, pluginHost);
                return fileOut;
            }

            return null;
        }
        public sFolder Unpack(string file, int id)
        {
            if (file.ToUpper().EndsWith(".N2D"))
                return NPCK.Unpack(file, pluginHost);

            return new sFolder();
        }
    }
}
