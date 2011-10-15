using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PluginInterface;

namespace TOKIMEKIGS3S
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
            if (gameCode == "B3SJ")
                return true;

            return false;
        }

        public Format Get_Format(string fileName, byte[] magic, int id)
        {
            String ext = new string(Encoding.ASCII.GetChars(magic));

            if (fileName.ToUpper().EndsWith(".LZS"))
                return Format.Compressed;
            else if (ext == "RESC")
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(sFolder unpacked, string file, int id)
        {
            if (file.ToUpper().EndsWith(".LZS"))
                return LZS.Compress(unpacked.files[0].path, file, pluginHost);
            else if (file.ToUpper().EndsWith(".RESC"))
                return RESC.Pack(file, unpacked, pluginHost);

            return null;
        }
        public sFolder Unpack(string file, int id)
        {
            if (file.ToUpper().EndsWith(".LZS"))
            {
                sFolder decompressed = new sFolder();
                decompressed.files = new List<sFile>();
                decompressed.files.Add(LZS.Decompress(file, pluginHost));

                return decompressed;
            }
            else if (file.ToUpper().EndsWith(".RESC"))
            {
                return RESC.Unpack(file, pluginHost);
            }

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
