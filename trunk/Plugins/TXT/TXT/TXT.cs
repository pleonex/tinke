using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace TXT
{
    public class TXT : IPlugin
    {
        IPluginHost pluginHost;

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if ((nombre.EndsWith("LZ.TXT") || nombre.EndsWith("LZ.XML")) && magic[0] == 0x10)
                return Formato.Desconocido;

            if (nombre.EndsWith(".TXT") || nombre.EndsWith(".SADL") || nombre.EndsWith(".XML")
                || nombre.EndsWith(".INI") || nombre.EndsWith(".H") || nombre.EndsWith(".XSADL")
                || nombre.EndsWith(".BAT") || nombre.EndsWith(".SARC") || nombre.EndsWith(".SBDL")
                || nombre.EndsWith(".C") || nombre.EndsWith("MAKEFILE") || nombre.EndsWith(".BSF")
                || nombre.EndsWith(".LUA") || nombre.EndsWith(".CSV") || nombre.EndsWith(".SMAP")
                || nombre.EndsWith("BUILDTIME"))
                return Formato.Texto;
            else if (ext == "MESG")
                return Formato.Texto;

            return Formato.Desconocido;
        }


        public void Leer(string archivo, int id)
        {
            Console.WriteLine("Este archivo no contiene información para guardar.");
        }

        public Control Show_Info(string archivo, int id)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            if (new String(Encoding.ASCII.GetChars(br.ReadBytes(4))) == "MESG")
            {
                br.Close();
                return new bmg(pluginHost, archivo).ShowInfo();
            }
            else
                br.BaseStream.Position = 0x00;

            byte[] txt = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();

            return new iTXT(txt, pluginHost, id);
        }
    }
}
