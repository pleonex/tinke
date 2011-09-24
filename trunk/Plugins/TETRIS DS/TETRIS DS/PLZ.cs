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
            pluginHost.Decompress(file);
            string dec_file;
            sFolder dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<sFile>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Decompress(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            uint file_size = (uint)new FileInfo(dec_file).Length;
            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));

            NCLR nclr = new NCLR();
            nclr.id = (uint)id;

            nclr.header.id = "PLZ ".ToCharArray();
            nclr.header.constant = 0x0100;
            nclr.header.file_size = file_size;
            nclr.header.header_size = 0x10;

            nclr.pltt.ID = "PLZ ".ToCharArray();
            nclr.pltt.length = file_size;
            nclr.pltt.depth = (file_size > 0x20) ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.paletteLength = file_size;
            nclr.pltt.nColors = file_size / 2;
            nclr.pltt.palettes = new NTFP[1];
            
            nclr.pltt.palettes[0].colors = pluginHost.BGR555(br.ReadBytes((int)file_size));

            br.Close();
            pluginHost.Set_NCLR(nclr);
        }
    }
}
