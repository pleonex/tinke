using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace TETRIS_DS
{
    public static class PLZ
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            pluginHost.Descomprimir(file);
            string dec_file;
            Carpeta dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<Archivo>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Descomprimir(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            uint file_size = (uint)new FileInfo(dec_file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));

            NCLR nclr = new NCLR();
            nclr.id = (uint)id;

            nclr.cabecera.id = "PLZ ".ToCharArray();
            nclr.cabecera.constant = 0x0100;
            nclr.cabecera.file_size = file_size;
            nclr.cabecera.header_size = 0x10;

            nclr.pltt.ID = "PLZ ".ToCharArray();
            nclr.pltt.tamaño = file_size;
            nclr.pltt.profundidad = (file_size > 0x20) ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.tamañoPaletas = file_size;
            nclr.pltt.nColores = file_size / 2;
            nclr.pltt.paletas = new NTFP[1];
            
            nclr.pltt.paletas[0].colores = pluginHost.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            pluginHost.Set_NCLR(nclr);
        }
    }
}
