using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;

namespace Comun
{
    class BMP
    {
        string archivo;
        IPluginHost pluginHost;

        public BMP(IPluginHost pluginHost, string archivo)
        {
            this.pluginHost = pluginHost;
            this.archivo = archivo;  
        }

        public Control Show_Info()
        {
            return new BasicControl(archivo);
        }

    }
}
