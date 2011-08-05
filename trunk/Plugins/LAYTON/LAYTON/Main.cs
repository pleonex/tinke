using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public bool EsCompatible()
        {
            if (gameCode == "A5FP" || gameCode == "A5FE")
                return true;
            else
                return false;
        }
        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            switch (gameCode[3])
            {
                case 'E':
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
                case 'P':
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
            }

            if (nombre.ToUpper().EndsWith(".TXT"))
                return Formato.Texto;
            
            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
            throw new NotImplementedException();
        }
        public Control Show_Info(string archivo, int id)
        {
            switch (gameCode[3])
            {
                case 'E':
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
                case 'P':
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
            }

            if (archivo.ToUpper().EndsWith(".TXT"))
                return new Text(pluginHost, gameCode, archivo).Show_Info(id);

            return new Control();
        }
    }
}
