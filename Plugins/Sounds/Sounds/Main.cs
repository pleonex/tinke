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

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Formato Get_Formato(string nombre, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "sadl")
                return Formato.Sonido;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
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
    }
}
