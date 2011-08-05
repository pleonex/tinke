using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace LAYTON
{
    class Text
    {
        IPluginHost pluginHost;
        string gameCode;
        string archivo;

        public Text(IPluginHost pluginHost, string gameCode, string archivo)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
            this.archivo = archivo;
        }

        public Control Show_Info(int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            string txt = new String(Encoding.UTF8.GetChars(br.ReadBytes((int)br.BaseStream.Length)));
            br.Close();

            return new iText(pluginHost, txt, id);
        }
    }
}
