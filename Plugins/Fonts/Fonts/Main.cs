using System;
using System.Collections.Generic;
using System.Text;
using PluginInterface;

namespace Fonts
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

            if (ext == "NFTR" || ext == "RTFN")
                return Formato.Fuentes;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(archivo));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "NFTR" || ext == "RTFN")
            {
                return new FontControl(pluginHost, NFTR.Read(archivo, id));
            }

            return new System.Windows.Forms.Control();
        }
    }
}
