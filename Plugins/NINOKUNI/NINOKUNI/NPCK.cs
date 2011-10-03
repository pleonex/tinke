using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace NINOKUNI
{
    public static class NPCK
    {
        public static void Unpack(string file, IPluginHost pluginHost)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "unpack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint header_size = br.ReadUInt32();
            uint unknown = br.ReadUInt32();

            int i = 0;
            while (br.BaseStream.Position != header_size)
            {
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();

                if (offset == 0x00 || size == 0x00)
                    continue;

                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".bin";
                i++;
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = packFile;
                unpack.files.Add(newFile);
            }

            br.Close();
            pluginHost.Set_Files(unpack);
        }
    }
}
