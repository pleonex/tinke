using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Nintendo
{
    public class ntfp
    {
        IPluginHost pluginHost;
		string archivo;
		
		public ntfp(IPluginHost pluginHost, string archivo)
		{
			this.pluginHost = pluginHost;
			this.archivo = archivo;
		}

        public void Leer()
        {
            uint file_size = (uint)new FileInfo(archivo).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            NCLR nclr = new NCLR();
            // Ponemos una cabecera genérica
            nclr.cabecera.id = "NCLR".ToCharArray();
            nclr.cabecera.constant = 0x0100;
            nclr.cabecera.file_size = file_size;
            nclr.cabecera.header_size = 0x10;
            // El archivo es PLTT raw, es decir, exclusivamente colores
            nclr.pltt.ID = "PLTT".ToCharArray();
            nclr.pltt.tamaño = file_size;
            nclr.pltt.profundidad = (file_size > 0x20) ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.tamañoPaletas = file_size;
            nclr.pltt.paletas = new NTFP[1];
            // Rellenamos los colores en formato BGR555
            nclr.pltt.paletas[0].colores = pluginHost.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            pluginHost.Set_NCLR(nclr);
        }

    }
}
