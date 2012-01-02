using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PluginInterface;

namespace Sounds
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(string nombre, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "sadl")
                return Format.Sound;

            return Format.Unknown;
        }

        public void Read(string archivo, int id)
        {
        }

        public Control Show_Info(string archivo, int id)
        {
            Thread waiting = new Thread(Thread_Waiting);
            waiting.Start((new String[] { "S00", pluginHost.Get_Language() }));

            if (archivo.ToUpper().EndsWith(".SAD"))
            {
                sWAV wav = SADL.ConvertToWAV(SADL.Read(archivo, id, pluginHost.Get_Language()));

                waiting.Abort();
                return new SoundControl(wav, pluginHost);
            }

            waiting.Abort();
            return new Control();
        }
        void Thread_Waiting(Object e)
        {
            String[] args = (String[])e;
            Waiting waitWind = new Waiting(args[0], args[1]);
            waitWind.Show();
        }

        public String Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
