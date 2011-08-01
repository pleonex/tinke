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

            int idFin = BuscarArchivo("fnt.bin", root).id; // ID del último archivo de la rom

            for (int i = 0; i < idFin; i++)
            {
                UInt32 currOffset = br.ReadUInt32();
                UInt32 currSize = br.ReadUInt32() - currOffset;
                Asignar_Archivo(i, currOffset, currSize, root);
            }

            return root;
        }
        public static void EscribirFAT(string salida, Carpeta root, int lastID)
        {
            throw new NotImplementedException();
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            UInt32 size, offset = 0x00;
            

            for (int i = 0; i <= lastID; i++)
            {
                bw.Write(offset); // Offset de inicio del archivo

                //size = (UInt32)Obtener_Tamaño(i, root);
                offset += offset + size;
                bw.Write(offset); // Offset de fin del archivo

                offset++;
            }

            bw.Flush();
            bw.Close();
        }
        public static void EscribirFAT(string salida, Carpeta root, int nFiles, uint offsetFAT, uint offsetOverlay9,
            uint offsetOverlay7)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            Console.Write("File Allocation Table (FAT)...");

            UInt32 offset = (uint)(offsetFAT + nFiles * 0x08 + 0x240 + 0x600); // Comienzo de la sección de archivos (FAT+banner)

            for (int i = 0; i < nFiles; i++)
            {
                Archivo currFile = BuscarArchivo(i, root);
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
                    offsetOverlay9 += currFile.size;
                    bw.Write(offsetOverlay7);
                    continue;
                }

                bw.Write(offset); // Offset de inicio del archivo
                offset += currFile.size;
                bw.Write(offset); // Offset de fin del archivo
            }

            bw.Flush();
            bw.Close();
            Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S09"), new FileInfo(salida).Length);
        }

        private static Archivo BuscarArchivo(int id, Carpeta currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                Archivo folderFile = new Archivo();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                if (((String)currFolder.tag).Length != 16)
                    folderFile.path = ((string)currFolder.tag).Substring(8);
                else
                    folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8), 16);
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);

                return folderFile;
            }

            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Archivo currFile = BuscarArchivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new Archivo();
        }
        private static Archivo BuscarArchivo(string name, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.name == name)
                        return archivo;


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Archivo currFile = BuscarArchivo(name, subFolder);
                    if (currFile.name is String)
                        return currFile;
                }
            }

            return new Archivo();
        }

        private static void Asignar_Archivo(int id, UInt32 offset, UInt32 size, Carpeta currFolder)
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
