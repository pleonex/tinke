using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginInterface
{
    public interface IPlugin
    {
        void Inicializar(IPluginHost pluginHost);
        Formato Get_Formato(string nombre, char[] magic);

        void Show_Info(string archivo);
        void Leer(string archivo);
    }
}
