using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginInterface
{
    public interface IGamePlugin
    {
        void Inicializar(IPluginHost pluginHost, string gameCode);

        bool EsCompatible();
        Formato Get_Formato(string nombre, byte[] magic, int id);

        Control Show_Info(string archivo, int id);  // Devuelve un control con toda la información posible. Este método llama a Leer
        void Leer(string archivo, int id);          // Abre el archivo y en caso de ser imagen guarda los datos
    }
}
