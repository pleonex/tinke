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
            paleta.header.id = br.ReadChars(4);
            paleta.header.endianess = 0xFFFE;
            paleta.header.constant = 0x0100;
            paleta.header.file_size = (uint)br.BaseStream.Length;
            paleta.header.header_size = 0x8;
            paleta.header.nSection = 1;
            // Paleta
            paleta.pltt.ID = paleta.header.id;
            paleta.pltt.length = paleta.header.file_size - 0x08;
            paleta.pltt.paletteLength = paleta.header.file_size - 0x08;
            paleta.pltt.nColors = br.ReadUInt16();
            paleta.pltt.depth = (br.ReadUInt16() == 0x04) ? System.Windows.Forms.ColorDepth.Depth4Bit : System.Windows.Forms.ColorDepth.Depth8Bit;
                        
            paleta.pltt.palettes = new NTFP[1];
            paleta.pltt.palettes[0].colors = pluginHost.BGR555(br.ReadBytes((int)(paleta.pltt.paletteLength)));
            pluginHost.Set_NCLR(paleta);

            br.Close();
        }
    }
}
