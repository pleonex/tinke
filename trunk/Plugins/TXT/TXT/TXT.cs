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

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if ((nombre.EndsWith("LZ.TXT") || nombre.EndsWith("LZ.XML")) && magic[0] == 0x10)
                return Format.Unknown;

            if (nombre.EndsWith(".TXT") || nombre.EndsWith(".SADL") || nombre.EndsWith(".XML")
                || nombre.EndsWith(".INI") || nombre.EndsWith(".H") || nombre.EndsWith(".XSADL")
                || nombre.EndsWith(".BAT") || nombre.EndsWith(".SARC") || nombre.EndsWith(".SBDL")
                || nombre.EndsWith(".C") || nombre.EndsWith("MAKEFILE") || nombre.EndsWith(".BSF")
                || nombre.EndsWith(".LUA") || nombre.EndsWith(".CSV") || nombre.EndsWith(".SMAP")
                || nombre.EndsWith("BUILDTIME") || nombre.EndsWith(".LUA~") || nombre.EndsWith(".INI.TEMPLATE")
                || nombre.EndsWith("LUA.BAK"))
                return Format.Text;
            else if (ext == "MESG")
                return Format.Text;

            return Format.Unknown;
        }


        public void Read(string archivo, int id)
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
