using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace AI_IGO_DS
{
    public static class ANCL
    {
        public static void Leer(string archivo, IPluginHost pluginHost)
        {
            NCLR paleta = new NCLR();
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            // Cabecera típica
            paleta.cabecera.id = br.ReadChars(4);
            paleta.cabecera.endianess = 0xFFFE;
            paleta.cabecera.constant = 0x0100;
            paleta.cabecera.file_size = (uint)br.BaseStream.Length;
            paleta.cabecera.header_size = 0x8;
            paleta.cabecera.nSection = 1;
            // Paleta
            paleta.pltt.ID = paleta.cabecera.id;
            paleta.pltt.tamaño = paleta.cabecera.file_size - 0x08;
            paleta.pltt.tamañoPaletas = paleta.cabecera.file_size - 0x08;
            paleta.pltt.nColores = br.ReadUInt16();
            paleta.pltt.profundidad = (br.ReadUInt16() == 0x04) ? System.Windows.Forms.ColorDepth.Depth4Bit : System.Windows.Forms.ColorDepth.Depth8Bit;
            
            if (paleta.pltt.profundidad != System.Windows.Forms.ColorDepth.Depth4Bit) // TODO: quitar esto
                System.Windows.Forms.MessageBox.Show("Diferente");
            
            paleta.pltt.paletas = new NTFP[1];
            paleta.pltt.paletas[0].colores = pluginHost.BGR555(br.ReadBytes((int)(paleta.pltt.tamañoPaletas)));
            pluginHost.Set_NCLR(paleta);

            br.Close();
        }
    }
}
