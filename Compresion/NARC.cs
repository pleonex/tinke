using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    //  Nintendo ARChive --> by pleoNeX
    public static class NARC
    {
        public static Folder Leer(string file)
        {
            ARC arc = new ARC();
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(file));

            // Lee cabecera genérica e inicial:
            arc.id = br.ReadChars(4);
            arc.id_endian = br.ReadUInt16();
            if (arc.id_endian == 0xFFFE)
                arc.id.Reverse<Char>();
            arc.constant = br.ReadUInt16();
            arc.file_size = br.ReadUInt32();
            arc.header_size = br.ReadUInt16();
            arc.nSections = br.ReadUInt16();

            // Lee primera sección BTAF (File Allocation TaBle)
            arc.btaf.id = br.ReadChars(4);
            arc.btaf.section_size = br.ReadUInt32();
            arc.btaf.nFiles = br.ReadUInt32();
            arc.btaf.entries = new BTAF_Entry[arc.btaf.nFiles];
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                arc.btaf.entries[i].start_offset = br.ReadUInt32();
                arc.btaf.entries[i].end_offset = br.ReadUInt32();
            }

            // Lee la segunda sección BTNF (File Name TaBle)
            arc.btnf.id = br.ReadChars(4);
            arc.btnf.section_size = br.ReadUInt32();
            arc.btnf.entries = new List<BTNF_MainEntry>();
            long pos = br.BaseStream.Position;
            #region Obtener carpeta root
            do
            {
                BTNF_MainEntry main = new BTNF_MainEntry();
                main.offset = br.ReadUInt32();
                main.first_pos = br.ReadUInt16();
                main.parent = br.ReadUInt16();
                uint idFile = main.first_pos;


                int id = br.ReadByte();

                while (id != 0x0)   // Indicador de fin de subtable
                {
                    if (id < 0x80)  // Es archivo
                    {
                        File currFile = new File();
                        currFile.id = idFile;
                        idFile++;
                        currFile.name = new String(br.ReadChars(id));
                        if (!(main.files is List<File>))
                            main.files = new List<File>();
                        main.files.Add(currFile);
                    }
                    else if (id > 0x80) // Es carpeta
                    {
                        Folder currFolder = new Folder();
                        currFolder.name = new String(br.ReadChars(id - 0x80));
                        currFolder.id = br.ReadUInt16();
                        if (!(main.folders is List<Folder>))
                            main.folders = new List<Folder>();
                        main.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                }
                arc.btnf.entries.Add(main);

            } while (arc.btnf.entries[0].offset == br.BaseStream.Position);
            while (br.BaseStream.Position == (pos - 8) + arc.btnf.section_size) { br.ReadByte(); }    // Suele terminar la sección con 0xFFFFFF
            
            Folder root = Jerarquizar_Carpetas(arc.btnf.entries, 0xF000, "root");
            #endregion
            
            // Lee tercera sección GMIF (File IMaGe)
            arc.gmif.id = br.ReadChars(4);
            arc.gmif.section_size = br.ReadUInt32();
            pos = br.BaseStream.Position;
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                br.BaseStream.Position = pos + arc.btaf.entries[i].start_offset;
                Asignar_Archivos(ref br, root, i, arc.btaf.entries[i].end_offset);
            }

            br.Close();
            br.Dispose();

            return root;
        }
        public static Folder Leer(string file, out ARC arc)
        {
            Inicio:
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(file));

            // Lee cabecera genérica e inicial:
            arc.id = br.ReadChars(4);
            if (arc.id[0] == '\x10')    // Archivo con compresión LZ77
            {
                br.Close();
                br.Dispose();
                Basico.Decompress(file, file + ".un", false);
                System.IO.File.Copy(file + ".un", file, true);
                System.IO.File.Delete(file + ".un");
                goto Inicio;
            }
            arc.id_endian = br.ReadUInt16();
            if (arc.id_endian == 0xFFFE)
                arc.id.Reverse<Char>();
            arc.constant = br.ReadUInt16();
            arc.file_size = br.ReadUInt32();
            arc.header_size = br.ReadUInt16();
            arc.nSections = br.ReadUInt16();

            // Lee primera sección BTAF (File Allocation TaBle)
            arc.btaf.id = br.ReadChars(4);
            arc.btaf.section_size = br.ReadUInt32();
            arc.btaf.nFiles = br.ReadUInt32();
            arc.btaf.entries = new BTAF_Entry[arc.btaf.nFiles];
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                arc.btaf.entries[i].start_offset = br.ReadUInt32();
                arc.btaf.entries[i].end_offset = br.ReadUInt32();
            }

            // Lee la segunda sección BTNF (File Name TaBle)
            arc.btnf.id = br.ReadChars(4);
            arc.btnf.section_size = br.ReadUInt32();
            arc.btnf.entries = new List<BTNF_MainEntry>();
            long pos = br.BaseStream.Position;
            #region Obtener carpeta root
            do
            {
                BTNF_MainEntry main = new BTNF_MainEntry();
                main.offset = br.ReadUInt32();
                main.first_pos = br.ReadUInt16();
                main.parent = br.ReadUInt16();
                uint idFile = main.first_pos;

                if (main.offset < 0x8)  // No hay nombres, juegos como el pokemon
                {
                    
                    for (int i = 0; i < arc.btaf.nFiles; i++)
                    {
                        File currFile = new File();
                        currFile.id = idFile; idFile++;
                        currFile.name = "file" + idFile.ToString();
                        if (!(main.files is List<File>))
                            main.files = new List<File>();
                        main.files.Add(currFile);
                    }
                    br.BaseStream.Position = main.offset + pos; // Para que funcione la condición while
                    arc.btnf.entries.Add(main);
                    continue;
                }
                long posmain = br.BaseStream.Position;
                br.BaseStream.Position = main.offset + pos;
                int id = br.ReadByte();

                while (id != 0x0)   // Indicador de fin de subtable
                {
                    if (id < 0x80)  // Es archivo
                    {
                        File currFile = new File();
                        currFile.id = idFile;
                        idFile++;
                        currFile.name = new String(br.ReadChars(id));
                        if (!(main.files is List<File>))
                            main.files = new List<File>();
                        main.files.Add(currFile);
                    }
                    else if (id > 0x80) // Es carpeta
                    {
                        Folder currFolder = new Folder();
                        currFolder.name = new String(br.ReadChars(id - 0x80));
                        currFolder.id = br.ReadUInt16();
                        if (!(main.folders is List<Folder>))
                            main.folders = new List<Folder>();
                        main.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                }
                arc.btnf.entries.Add(main);
                br.BaseStream.Position = posmain;

            } while (arc.btnf.entries[0].offset + pos != br.BaseStream.Position);

            br.BaseStream.Position = pos - 8 + arc.btnf.section_size;
            Folder root = Jerarquizar_Carpetas(arc.btnf.entries, 0xF000, "root");
            #endregion

            // Lee tercera sección GMIF (File IMaGe)
            arc.gmif.id = br.ReadChars(4);
            arc.gmif.section_size = br.ReadUInt32();
            pos = br.BaseStream.Position;
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                br.BaseStream.Position = pos + arc.btaf.entries[i].start_offset;
                Asignar_Archivos(ref br, root, i, arc.btaf.entries[i].end_offset - arc.btaf.entries[i].start_offset);
            }
            br.Close();
            br.Dispose();

            return root;
        }
        public static Folder Jerarquizar_Carpetas(List<BTNF_MainEntry> entries, int idFolder, string nameFolder)
        {
            Folder currFolder = new Folder();

            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = entries[idFolder & 0xFFF].files;

            if (entries[idFolder & 0xFFF].folders is List<Folder>) // Si tiene carpetas dentro.
            {
                currFolder.subfolders = new List<Folder>();

                foreach (Folder subFolder in entries[idFolder & 0xFFF].folders)
                    currFolder.subfolders.Add(Jerarquizar_Carpetas(entries, subFolder.id, subFolder.name));
            }

            return currFolder;
        }
        public static void Asignar_Archivos(ref BinaryReader br, Folder currFolder, int idFile, UInt32 size)
        {
            if (currFolder.files is List<File>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == idFile)
                    {
                        File newFile = currFolder.files[i];
                        newFile.bytes = br.ReadBytes((int)size);
                        currFolder.files.RemoveAt(i);
                        currFolder.files.Insert(i, newFile);
                        return;
                    }

                }
            }

            if (currFolder.subfolders is List<Folder>) // Si tiene carpetas dentro.
                foreach (Folder subFolder in currFolder.subfolders)
                   Asignar_Archivos(ref br, subFolder, idFile, size);
        }

        public static void Descomprimir(string file, string outFolder)
        {
            ARC arc = new ARC();
            Folder root = Leer(file, out arc);

            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                File currFile = Recursivo_Archivo(i, root);
                System.IO.File.WriteAllBytes(outFolder + '\\' + currFile.name, currFile.bytes);
            }
        }
        private static File Recursivo_Archivo(int id, Folder currFolder)
        {
            if (currFolder.files is List<File>)
                foreach (File archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.subfolders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.subfolders)
                {
                    File currFile = Recursivo_Archivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new File();
        }



        #region Estructuras
        public struct ARC
        {
            public char[] id;           // Always NARC = 0x4E415243
            public UInt16 id_endian;    // Si 0xFFFE hay que darle la vuelta al id
            public UInt16 constant;     // Always 0x0100
            public UInt32 file_size;
            public UInt16 header_size;  // Siempre 0x0010
            public UInt16 nSections;    // En este caso siempre 0x0003

            public BTAF btaf;
            public BTNF btnf;
            public GMIF gmif;
        }
        public struct BTAF
        {
            public char[] id;
            public UInt32 section_size;
            public UInt32 nFiles;
            public BTAF_Entry[] entries;
        }
        public struct BTAF_Entry
        {
            // Ambas son relativas a la sección GMIF
            public UInt32 start_offset;
            public UInt32 end_offset;
        }
        public struct BTNF
        {
            public char[] id;
            public UInt32 section_size;
            public List<BTNF_MainEntry> entries;
        }
        public struct BTNF_MainEntry
        {
            public UInt32 offset;       // Relativo a la primera entrada
            public UInt32 first_pos;    // ID del primer archivo.
            public UInt32 parent;       // En el caso de root, número de carpetas;
            public List<File> files;
            public List<Folder> folders;
        }
        public struct File
        {
            public UInt32 id;
            public string name;
            public byte[] bytes;
        }
        public struct Folder
        {
            public UInt16 id;
            public string name;
            public List<File> files;
            public List<Folder> subfolders;
        }
        public struct GMIF
        {
            public char[] id;
            public UInt32 section_size;
            // Datos de los archivos....
        }
        #endregion
    }
}
