using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Windows.Forms;

namespace Comun
{
    public class Main : IPlugin 
    {
        IPluginHost pluginHost;

        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".TGA") || nombre.EndsWith(".JPG"))
                return Formato.ImagenCompleta;
            else if (nombre.EndsWith(".WAV"))
                return Formato.Sonido;
            
            return Formato.Desconocido;
        }

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public void Leer(string archivo)
        {
            throw new NotImplementedException();
        }

        public Control Show_Info(string archivo)
        {
            if (archivo.ToUpper().EndsWith(".TGA"))
                return new TGA(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".JPG"))
                return new JPG(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".WAV"))
                return new WAV(pluginHost, archivo).Show_Info();

            return new Control();
        }
    }
}
