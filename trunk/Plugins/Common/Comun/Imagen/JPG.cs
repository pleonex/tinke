using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Comun
{
    class JPG
    {
        string archivo;
        IPluginHost pluginHost;

        public JPG(IPluginHost pluginHost, string archivo)
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
