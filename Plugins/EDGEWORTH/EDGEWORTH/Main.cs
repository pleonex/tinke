using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace EDGEWORTH
{
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public bool EsCompatible()
        {
            if (gameCode == "C32P")
                return true;

            return false;
        }

        public Formato Get_Formato(string nombre, byte[] magic, int id)
        {
            nombre = nombre.ToUpper();

            if (nombre == "ROMFILE.BIN")
                return Formato.Pack;

            return Formato.Desconocido;
        }

        public void Inicializar(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public void Leer(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith("ROMFILE.BIN"))
            {
                System.Threading.Thread waiting = new System.Threading.Thread(ThreadWait);
                String lang = "";
                try
                {
                    System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                        "Plugins" + System.IO.Path.DirectorySeparatorChar + "EDGEWORTHLang.xml");
                    lang = xml.Element(pluginHost.Get_Language()).Element("S02").Value;
                }
                catch { throw new NotSupportedException("There was an error reading the language file"); }
                waiting.Start(lang);

                PACK.Read(archivo, pluginHost);

                waiting.Abort();
            }
        }

        public Control Show_Info(string archivo, int id)
        {
            if (archivo.ToUpper().EndsWith("ROMFILE.BIN"))
                return new PackControl(pluginHost);

            return new Control();
        }

        private void ThreadWait(object name)
        {
            Espera wait = new Espera((string)name);
            wait.ShowDialog();
        }
    }
}
