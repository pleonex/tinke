using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace SUBARASHIKI
{
    public static class PACK
    {        
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            // Copied from:
            // http://forum.xentax.com/viewtopic.php?f=18&t=3175&p=30296&hilit=pack+world+ends#p30296
            // Credits to McCuñao
            string packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "pack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint num_files = br.ReadUInt32();
            uint data_size = br.ReadUInt32();
            uint unknown = br.ReadUInt32(); // Unknown always 0x00
            br.ReadBytes(16); // Padding always 0x000

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32() + 0x20;
                newFile.size = br.ReadUInt32();
                newFile.path = packFile;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
    }
}
