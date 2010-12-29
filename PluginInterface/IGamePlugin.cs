using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginInterface
{
    public interface IGamePlugin
    {
        bool EsCompatible();

        void Inicializar(IPluginHost pluginHost, string gameCode);
        Formato Get_Formato(string nombre, char[] magic, int id);
        void Show_Info(string archivo);
        void Leer(string archivo);
    }
}
