using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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

            if (ext == "NPCK" || ext == "KPCN")
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
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "NPCK")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                    "pack_" + System.IO.Path.GetFileName(file);
                
                NPCK.Pack(fileOut, unpacked, pluginHost);
                return fileOut;
            }
            else if (ext == "KPCN")
            {
                string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                    "pack_" + System.IO.Path.GetFileName(file);

                KPCN.Pack(fileOut, file, unpacked, pluginHost);
                return fileOut;
            }

            return null;
        }
        public sFolder Unpack(string file, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "NPCK")
                return NPCK.Unpack(file, pluginHost);
            else if (ext == "KPCN")
                return KPCN.Unpack(file, pluginHost); 

            return new sFolder();
        }
    }
}
