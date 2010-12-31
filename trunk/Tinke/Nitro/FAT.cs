using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Tinke.Nitro
{
    public static class FAT
    {
        public static Carpeta LeerFAT(string file, UInt32 offset, UInt32 size, Carpeta root)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            for (int i = 0; ; i++)
            {
                if (br.BaseStream.Position > offset + size)
                    break;

                UInt32 currOffset = br.ReadUInt32();
                UInt32 currSize = br.ReadUInt32() - currOffset;
                Asignar_Archivo(i, currOffset, currSize, root);
            }

            return root;
        }

        public static void Asignar_Archivo(int id, UInt32 offset, UInt32 size, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        Archivo newFile = currFolder.files[i];
                        newFile.offset = offset;
                        newFile.size = size;
                        currFolder.files.RemoveAt(i);
                        currFolder.files.Insert(i, newFile);
                        return;
                    }
                }
            }

            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    Asignar_Archivo(id, offset, size, subFolder);
        }

    }
}
