﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;

namespace Comun
{
    class PNG
    {
        string archivo;
        IPluginHost pluginHost;

        public PNG(IPluginHost pluginHost, string archivo)
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
