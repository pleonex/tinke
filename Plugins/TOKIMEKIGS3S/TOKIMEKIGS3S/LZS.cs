using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace TOKIMEKIGS3S
{
    public static class LZS
    {
        public static sFile Decompress(string file, IPluginHost pluginHost)
        {
            sFile decompressed;

            string parent_name = Path.GetFileNameWithoutExtension(file).Substring(12);

            string temp = parent_name + ".resc";
            Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x10];
            Array.Copy(File.ReadAllBytes(file), 0x10, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(temp, compressFile);

            pluginHost.Decompress(temp);
            decompressed = pluginHost.Get_Files().files[0];
            File.Delete(temp);

            return decompressed;
        }

        public static String Compress(string fileIn, string originalFile, IPluginHost pluginHost)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "new_" + Path.GetFileName(originalFile);

            // Read unknown header
            BinaryReader br = new BinaryReader(File.OpenRead(originalFile));
            byte[] header = br.ReadBytes(0x10);
            br.Close();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            bw.Write(header);

            // Compress the file with LZ11
            String tempFile = Path.GetTempFileName();
            pluginHost.Compress(fileIn, tempFile, FormatCompress.LZ11);
            bw.Write(File.ReadAllBytes(tempFile));
            bw.Flush();
            bw.Close();

            File.Delete(tempFile);
            return fileOut;
        }
    }
}
