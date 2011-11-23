using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Windows.Forms;

namespace Comun
{
    class WAV
    {
        string archivo;
        IPluginHost pluginHost;

        public WAV(IPluginHost pluginHost, string archivo)
        {
            this.pluginHost = pluginHost;
            this.archivo = archivo;  
        }


        public Control Show_Info()
        {
            return new iWav(archivo);
        }

    }
}
