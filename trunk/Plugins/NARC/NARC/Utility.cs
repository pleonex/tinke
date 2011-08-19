using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace NARC
{
    public class Utility
    {
        string tempFolder;
        IPluginHost pluginHost;

        public Utility(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public ARC Leer(string archivo, int idArchivo)
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
            arc.file_id = idArchivo;
            BinaryReader br = new BinaryReader(System.IO.File.OpenRead(archivo));

            uint fntOffset = br.ReadUInt32();
            uint fntSize = br.ReadUInt32();
            uint fatOffset = br.ReadUInt32();
            uint fatSize = br.ReadUInt32();

            // FAT (File Allocation TaBle)
            br.BaseStream.Position = fatOffset;

            arc.btaf.nFiles = fatSize / 0x08;
            arc.btaf.entries = new BTAF_Entry[arc.btaf.nFiles];
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                arc.btaf.entries[i].start_offset = br.ReadUInt32();
                arc.btaf.entries[i].end_offset = br.ReadUInt32();
            }

            // FNT (File Name TaBle)
            br.BaseStream.Position = fntOffset;
            arc.btnf.entries = new List<BTNF_MainEntry>();
     
            #region Obtener carpeta root
            do
            {
                BTNF_MainEntry main = new BTNF_MainEntry();
                main.offset = br.ReadUInt32();
                main.first_pos = br.ReadUInt16();
                main.parent = br.ReadUInt16();
                uint idFile = main.first_pos;

                long currOffset = br.BaseStream.Position;
                br.BaseStream.Position = main.offset + fntOffset;
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
                br.BaseStream.Position = currOffset;

            } while (fntOffset + arc.btnf.entries[0].offset != br.BaseStream.Position);

            Carpeta root = Jerarquizar_Carpetas(arc.btnf.entries, 0xF000, "root");
            #endregion

            // Archivos
            br.BaseStream.Position = fatOffset + fatSize;

            //pos = br.BaseStream.Position;
            for (int i = 0; i < arc.btaf.nFiles; i++)
            {
                br.BaseStream.Position = arc.btaf.entries[i].start_offset;
                Asignar_Archivos(ref br, root, i, arc.btaf.entries[i].end_offset - arc.btaf.entries[i].start_offset);
            }
            br.Close();

            pluginHost.Set_Files(root);
            return arc;
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
    }
}
