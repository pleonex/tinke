using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace DEATHNOTEDS
{
    public static class Packs
    {
        public static sFolder Unpack_data(string file)
        {
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            uint num_files = br.ReadUInt32();
            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = "File " + i.ToString();
                newFile.offset = br.ReadUInt32() * 4;
                newFile.size = br.ReadUInt32();
                newFile.path = file;
                
                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
    }
}
