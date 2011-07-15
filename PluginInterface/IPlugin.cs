using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginInterface
{
    public interface IPlugin
    {
        void Inicializar(IPluginHost pluginHost);
        Formato Get_Formato(string nombre, byte[] magic);

        Control Show_Info(string archivo, int id);
        void Leer(string archivo, int id);
    }
}
