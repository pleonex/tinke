using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace NARC
{
    public class NARC : IPlugin 
    {
        IPluginHost pluginHost;
        string tempFolder;

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string id = new String(Encoding.ASCII.GetChars(magic));

            if (id == "NARC" || id == "CRAN")
                return Formato.Comprimido;

            return Formato.Desconocido;
        }


        public void Leer(string archivo)
        {
            tempFolder = pluginHost.Get_TempFolder();
            // Determinamos la subcarpeta donde guardar los archivos descomprimidos.
            string[] subFolders = Directory.GetDirectories(tempFolder);
            for (int n = 0; ; n++)
            {
                if (!subFolders.Contains<string>(tempFolder + "\\Temp" + n))
                {
                    tempFolder += "\\Temp" + n;
                    Directory.CreateDirectory(tempFolder);
                    break;
                }
            }

            ARC arc = new ARC();
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(archivo));

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

                if (main.offset < 0x8)  // No hay nombres, juegos como el pokemon
                {

                    for (int i = 0; i < arc.btaf.nFiles; i++)
                    {
                        Archivo currFile = new Archivo();
                        currFile.id = (ushort)idFile; idFile++;
                        currFile.name = "file" + idFile.ToString();
                        if (!(main.files is List<Archivo>))
                            main.files = new List<Archivo>();
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
                        Archivo currFile = new Archivo();
                        currFile.id = (ushort)idFile;
                        idFile++;
                        currFile.name = new String(br.ReadChars(id));
                        if (!(main.files is List<Archivo>))
                            main.files = new List<Archivo>();
                        main.files.Add(currFile);
                    }
                    else if (id > 0x80) // Es carpeta
                    {
                        Carpeta currFolder = new Carpeta();
                        currFolder.name = new String(br.ReadChars(id - 0x80));
                        currFolder.id = br.ReadUInt16();
                        if (!(main.folders is List<Carpeta>))
                            main.folders = new List<Carpeta>();
                        main.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                }
                arc.btnf.entries.Add(main);
                br.BaseStream.Position = posmain;

            } while (arc.btnf.entries[0].offset + pos != br.BaseStream.Position);

            br.BaseStream.Position = pos - 8 + arc.btnf.section_size;
            Carpeta root = Jerarquizar_Carpetas(arc.btnf.entries, 0xF000, "root");
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

            pluginHost.Set_Files(root);
        }
        public Carpeta Jerarquizar_Carpetas(List<BTNF_MainEntry> entries, int idFolder, string nameFolder)
        {
            Carpeta currFolder = new Carpeta();

            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = entries[idFolder & 0xFFF].files;

            if (entries[idFolder & 0xFFF].folders is List<Carpeta>) // Si tiene carpetas dentro.
            {
                currFolder.folders = new List<Carpeta>();

                foreach (Carpeta subFolder in entries[idFolder & 0xFFF].folders)
                    currFolder.folders.Add(Jerarquizar_Carpetas(entries, subFolder.id, subFolder.name));
            }

            return currFolder;
        }
        public void Asignar_Archivos(ref BinaryReader br, Carpeta currFolder, int idFile, UInt32 size)
        {
            if (currFolder.files is List<Archivo>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == idFile)
                    {
                        Archivo newFile = currFolder.files[i];
                        File.WriteAllBytes(tempFolder + "\\" + currFolder.files[i].name, br.ReadBytes((int)size));
                        newFile.path = tempFolder + "\\" + currFolder.files[i].name;
                        newFile.size = size;
                        currFolder.files.RemoveAt(i);
                        currFolder.files.Insert(i, newFile);
                        return;
                    }

                }
            }

            if (currFolder.folders is List<Carpeta>) // Si tiene carpetas dentro.
                foreach (Carpeta subFolder in currFolder.folders)
                    Asignar_Archivos(ref br, subFolder, idFile, size);
        }

        public Control Show_Info(string archivo)
        {
            throw new NotImplementedException();
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
            public List<Archivo> files;
            public List<Carpeta> folders;
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
