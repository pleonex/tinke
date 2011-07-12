using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Tinke.Nitro
{
    /// <summary>
    /// File Name Table.
    /// </summary>
    public static class FNT
    {
        /// <summary>
        /// Devuelve el sistema de archivos internos de la ROM
        /// </summary>
        /// <param name="file">Archivo ROM</param>
        /// <param name="offset">Offset donde comienza la FNT</param>
        /// <returns></returns>
        public static Carpeta LeerFNT(string file, UInt32 offset)
        {
            Carpeta root = new Carpeta();
            List<Estructuras.MainFNT> mains = new List<Estructuras.MainFNT>();

            BinaryReader br = new BinaryReader(File.OpenRead(file)); 
            br.BaseStream.Position = offset;
            
            long offsetSubTable = br.ReadUInt32();  // Offset donde comienzan las SubTable y terminan las MainTables.
            br.BaseStream.Position  = offset;       // Volvemos al principio de la primera MainTable
           
            while (br.BaseStream.Position < offset + offsetSubTable)
            {
                Estructuras.MainFNT main = new Estructuras.MainFNT();
                main.offset = br.ReadUInt32();
                main.idFirstFile = br.ReadUInt16();
                main.idParentFolder = br.ReadUInt16();
                
                long currOffset = br.BaseStream.Position;           // Posición guardada donde empieza la siguienta maintable
                br.BaseStream.Position = offset + main.offset;      // SubTable correspondiente
                
                // SubTable
                byte id = br.ReadByte();                            // Byte que identifica si es carpeta o archivo.
                ushort idFile = main.idFirstFile;

                while (id != 0x0)   // Indicador de fin de la SubTable
                {
                    if (id < 0x80)  // Archivo
                    {
                        Archivo currFile = new Archivo();

                        if (!(main.subTable.files is List<Archivo>))
                            main.subTable.files = new List<Archivo>();

                        int lengthName = id;
                        currFile.name = new String(br.ReadChars(lengthName));
                        currFile.id = idFile; idFile++;
                        
                        main.subTable.files.Add(currFile);
                    }
                    if (id > 0x80)  // Directorio
                    {
                        Carpeta currFolder = new Carpeta();

                        if (!(main.subTable.folders is List<Carpeta>))
                           main.subTable.folders = new List<Carpeta>();

                        int lengthName = id - 0x80;
                        currFolder.name = new String(br.ReadChars(lengthName));
                        currFolder.id = br.ReadUInt16();

                        main.subTable.folders.Add(currFolder);
                    }

                    id = br.ReadByte();
                } 

                mains.Add(main);
                br.BaseStream.Position = currOffset;
            } 

            root = Jerarquizar_Carpetas(mains, 0, "root");
            root.id = 0xF000;

            br.Close();

            return root;
        }

        public static Carpeta Jerarquizar_Carpetas(List<Estructuras.MainFNT> tables, int idFolder, string nameFolder)
        {
            Carpeta currFolder = new Carpeta();
            
            currFolder.name = nameFolder;
            currFolder.id = (ushort)idFolder;
            currFolder.files = tables[idFolder & 0xFFF].subTable.files;

            if (tables[idFolder & 0xFFF].subTable.folders is List<Carpeta>) // Si tiene carpetas dentro.
           {
                currFolder.folders = new List<Carpeta>();

                foreach (Carpeta subFolder in tables[idFolder & 0xFFF].subTable.folders)
                    currFolder.folders.Add(Jerarquizar_Carpetas(tables, subFolder.id, subFolder.name));
           }

            return currFolder;
        }
    }
}
