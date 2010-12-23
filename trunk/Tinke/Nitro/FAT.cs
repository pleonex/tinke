using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tinke.Nitro
{
    public static class FAT
    {
        public static Nitro.Estructuras.Folder LeerFAT(string file, UInt32 offset, UInt32 size, Estructuras.Folder root)
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

        public static void Asignar_Archivo(int id, UInt32 offset, UInt32 size, Estructuras.Folder currFolder)
        {
            if (currFolder.files is List<Estructuras.File>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        Estructuras.File newFile = currFolder.files[i];
                        newFile.offset = offset;
                        newFile.size = size;
                        currFolder.files.RemoveAt(i);
                        currFolder.files.Insert(i, newFile);
                        return;
                    }
                }
            }

            if (currFolder.folders is List<Estructuras.Folder>)
                foreach (Estructuras.Folder subFolder in currFolder.folders)
                    Asignar_Archivo(id, offset, size, subFolder);
        }

    }
}
