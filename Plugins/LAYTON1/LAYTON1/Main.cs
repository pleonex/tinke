using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;

namespace LAYTON1
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool EsCompatible()
        {
            if (gameCode == "A5FP" || gameCode == "A5FE")
                return true;
            else
                return false;
        }

        public Formato Get_Formato(string nombre, char[] magic, int id)
        {
            nombre = nombre.ToUpper();

            switch (gameCode[3])
            {
                case 'E':
                    if (id >= 0x0001 && id <= 0x02CB)
                        return Ani.Get_Formato(nombre);
                    break;
                case 'P':
                    if (id >= 0x0001 && id <= 0x04E7)
                        return Ani.Get_Formato(nombre);
                    break;
            }

            return Formato.Desconocido;
        }

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Leer(string archivo)
        {
            throw new NotImplementedException();
        }
        public void Show_Info(string archivo)
        {
            throw new NotImplementedException();
        }
    }
}
