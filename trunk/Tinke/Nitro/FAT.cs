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
        public static sFolder LeerFAT(string file, UInt32 offset, UInt32 size, sFolder root)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            for (int i = 0; i < size / 0x08; i++)
            {
                UInt32 currOffset = br.ReadUInt32();
                UInt32 currSize = br.ReadUInt32() - currOffset;
                Asignar_Archivo(i, currOffset, currSize, file, root);
            }

            br.Close();
            return root;
        }
        public static void EscribirFAT(string salida, sFolder root, int nFiles, uint offsetFAT, uint offsetOverlay9,
            uint offsetOverlay7)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            Console.Write("File Allocation Table (FAT)...");

            UInt32 offset = (uint)(offsetFAT + nFiles * 0x08 + 0x240 + 0x600); // Comienzo de la sección de archivos (FAT+banner)

            for (int i = 0; i < nFiles; i++)
            {
                sFile currFile = BuscarArchivo(i, root);
                if (currFile.name.StartsWith("overlay9"))
                {
                    bw.Write(offsetOverlay9);
                    offsetOverlay9 += currFile.size;
                    bw.Write(offsetOverlay9);
                    continue;
                }
                else if (currFile.name.StartsWith("overlay7"))
                {
                    bw.Write(offsetOverlay7);
                    offsetOverlay7 += currFile.size;
                    bw.Write(offsetOverlay7);
                    continue;
                }

                bw.Write(offset); // Offset de inicio del archivo
                offset += currFile.size;
                bw.Write(offset); // Offset de fin del archivo
            }

            bw.Flush();
            bw.Close();
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(salida).Length);
        }

        private static sFile BuscarArchivo(int id, sFolder currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                sFile folderFile = new sFile();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);
                folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8, 8), 16);
                folderFile.path = ((string)currFolder.tag).Substring(16);

                return folderFile;
            }

            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = BuscarArchivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }
        private static sFile BuscarArchivo(string name, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.name == name)
                        return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = BuscarArchivo(name, subFolder);
                    if (currFile.name is String)
                        return currFile;
                }
            }

            return new sFile();
        }

        private static void Asignar_Archivo(int id, UInt32 offset, UInt32 size, String romFile, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        sFile newFile = currFolder.files[i];
                        newFile.offset = offset;
                        newFile.size = size;
                        newFile.path = romFile;
                        currFolder.files.RemoveAt(i);
                        currFolder.files.Insert(i, newFile);
                        return;
                    }
                }
            }

            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    Asignar_Archivo(id, offset, size, romFile, subFolder);
        }

    }
}
