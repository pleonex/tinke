using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PluginInterface;

namespace _3DModels
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

            if (ext == "BTX0" || ext == "BMD0")
                return Formato.Texture;

            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
        }
        public System.Windows.Forms.Control Show_Info(string archivo, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            string ext = new String(br.ReadChars(4));
            br.Close();

            if (ext == "BTX0" || ext == "BMD0")
                return new TextureControl(pluginHost, BTX0.Read(archivo, id, pluginHost));
            //else if (ext == "BMD0")
            //    return new TextureControl(pluginHost, BMD0.Read(archivo, id, pluginHost));

            return new System.Windows.Forms.Control();
        }
    }
}
