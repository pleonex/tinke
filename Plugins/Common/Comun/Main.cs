using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Windows.Forms;

namespace Comun
{
    public class Main : IPlugin 
    {
        IPluginHost pluginHost;

        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".TGA") || nombre.EndsWith(".JPG") || nombre.EndsWith(".PNG") || nombre.EndsWith(".BMP"))
                return Formato.ImagenCompleta;
            else if (nombre.EndsWith(".WAV") || ext == "RIFF")
                return Formato.Sonido;
            
            return Formato.Desconocido;
        }

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public void Leer(string archivo, int id)
        {
            MessageBox.Show("Estos archivos no pueden guardar información.");
        }

        public Control Show_Info(string archivo, int id)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(System.IO.File.OpenRead(archivo));
            string ext = "";
            try { ext = new String(br.ReadChars(4)); }
            catch { }
            br.Close();

            if (archivo.ToUpper().EndsWith(".TGA"))
                return new TGA(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".JPG"))
                return new JPG(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".PNG"))
                return new PNG(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".WAV") || ext == "RIFF")
                return new WAV(pluginHost, archivo).Show_Info();
            else if (archivo.ToUpper().EndsWith(".BMP"))
                return new BMP(pluginHost, archivo).Show_Info();

            return new Control();
        }
    }
}
