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
            if (gameCode == "A5FP" || gameCode == "A5FE" || gameCode == "YLTS")
                return true;
            else
                return false;
        }
        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            switch (gameCode)
            {
                case "A5FE":
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, "").Get_Formato(nombre);
                    else if (id >= 0x409 & id <= 0x808)
                        return new Bg(pluginHost, gameCode, "").Get_Formato(nombre);
                    break;
            }

            if (nombre.ToUpper().EndsWith(".TXT"))
                return Formato.Texto;
            if (nombre.ToUpper().EndsWith(".PLZ"))
                return Formato.Comprimido;
            
            return Formato.Desconocido;
        }

        public void Leer(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith(".PLZ"))
                PCK2.Read(archivo, id, pluginHost);
        }
        public Control Show_Info(string archivo, int id)
        {
            switch (gameCode)
            {
                case "A5FE":
                    if (id >= 0x0001 && id <= 0x02CA)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x02CD && id <= 0x0765)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
                case "A5FP":
                    if (id >= 0x0001 && id <= 0x04E7)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x04E8 && id <= 0x0B72)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
                case "YLTS":
                    if (id >= 0x37 && id <= 0x408)
                        return new Ani(pluginHost, gameCode, archivo).Show_Info();
                    else if (id >= 0x409 && id <= 0x808)
                        return new Bg(pluginHost, gameCode, archivo).Show_Info();
                    break;
            }

            if (archivo.ToUpper().EndsWith(".TXT"))
                return new Text(pluginHost, gameCode, archivo).Show_Info(id);
            else if (archivo.ToUpper().EndsWith(".PLZ"))
                return new Control();

            return new Control();
        }
    }
}
