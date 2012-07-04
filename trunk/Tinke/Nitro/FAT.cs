using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Tinke.Nitro
{
    public static class FAT
    {

        public static Estructuras.sFAT[] ReadFAT(string romFile, uint fatOffset, uint fatSize)
        {
            Estructuras.sFAT[] fat = new Estructuras.sFAT[fatSize / 0x08];    // Number of files

            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = fatOffset;

            for (int i = 0; i < fat.Length; i++)
            {
                fat[i].offset = br.ReadUInt32();
                fat[i].size = br.ReadUInt32() - fat[i].offset;
            }

            br.Close();
            return fat;
        }
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
                sFile currFile = Search_File(i, root);
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
        public static void Write(string fileOut, sFolder root, uint offsetFAT, ushort[] sortedIDs, uint offset_ov9, uint offset_ov7)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            Console.Write("File Allocation Table (FAT)...");

            int num_files = sortedIDs.Length;

            // Set the first file offset
            uint offset = (uint)(offsetFAT + num_files * 0x08);
            if ((offset % 0x200) != 0)
                offset += 0x200 - (offset % 0x200);
            offset += 0xA00;

            byte[] buffer = new byte[num_files * 8];
            int zero_files = 0;
            byte[] temp;
            for (int i = 0; i < num_files; i++)
            {
                sFile currFile = Search_File(sortedIDs[i], root);

                if (!(currFile.name is string))
                    zero_files++;
                else if (currFile.name.StartsWith("overlay9"))
                {
                    temp = BitConverter.GetBytes(offset_ov9);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8, 4);
                    offset_ov9 += currFile.size;
                    temp = BitConverter.GetBytes(offset_ov9);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8 + 4, 4);

                    if (offset_ov9 % 0x200 != 0)
                        offset_ov9 += 0x200 - (offset_ov9 % 0x200);
                }
                else if (currFile.name.StartsWith("overlay7"))
                {
                    temp = BitConverter.GetBytes(offset_ov7);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8, 4);
                    offset_ov7 += currFile.size;
                    temp = BitConverter.GetBytes(offset_ov7);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8 + 4, 4);

                    if (offset_ov7 % 0x200 != 0)
                        offset_ov7 += 0x200 - (offset_ov7 % 0x200);
                }
                else
                {
                    temp = BitConverter.GetBytes(offset);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8, 4);
                    offset += currFile.size;
                    temp = BitConverter.GetBytes(offset);
                    Array.Copy(temp, 0, buffer, sortedIDs[i] * 8 + 4, 4);

                    if (offset % 0x200 != 0)
                        offset += 0x200 - (offset % 0x200);
                }
            }

            bw.Write(buffer);

            temp = BitConverter.GetBytes((uint)0);
            for (int i = 0; i < zero_files; i++)
            {
                Array.Copy(temp, 0, buffer, sortedIDs[i] * 8, 4);
                Array.Copy(temp, 0, buffer, sortedIDs[i] * 8 + 4, 4);
            }

            int rem = (int)bw.BaseStream.Position % 0x200;
            if (rem != 0)
            {
                while (rem < 0x200)
                {
                    bw.Write((byte)0xFF);
                    rem++;
                }
            }

            bw.Flush();
            bw.Close();
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(fileOut).Length);
        }


        private static sFile Search_File(int id, sFolder currFolder)
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
                    sFile currFile = Search_File(id, subFolder);
                    if (currFile.name is string)
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

        public static ushort[] SortByOffset(Estructuras.sFAT[] fat)
        {
            List<OffsetID> lfat = new List<OffsetID>();
            lfat.Sort(Sort);

            for (ushort i = 0; i < fat.Length; i++)
                lfat.Add(new OffsetID { offset = fat[i].offset, id = i });

            lfat.Sort(Sort);

            ushort[] ids = new ushort[fat.Length];
            for (int i = 0; i < fat.Length; i++)
                ids[i] = lfat[i].id;

            return ids;
        }
        private struct OffsetID
        {
            public uint offset;
            public ushort id;
        }
        private static int Sort(OffsetID f1, OffsetID f2)
        {
            if (f1.offset > f2.offset)
                return 1;
            else if (f1.offset < f2.offset)
                return -1;
            else
            {
                if (f1.id > f2.id)
                    return 1;
                else if (f1.id < f2.id)
                    return -1;
                else  // Impossible
                    return 0;
            }
        }
    }
}
