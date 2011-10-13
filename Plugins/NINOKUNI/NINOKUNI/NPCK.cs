using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace NINOKUNI
{
    public static class NPCK
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "unpack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpack = new sFolder();
            unpack.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint header_size = br.ReadUInt32();
            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();

                if (offset == 0x00 || size == 0x00)
                    continue;

                sFile newFile = new sFile();
                newFile.name = "File" + i.ToString() + ".bin";
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = packFile;
                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
        public static void Pack(string file, int id, IPluginHost pluginHost)
        {
            /* In N2D files there must be 9 offset in this order:
             * 
             * 0 - Palette
             * 1 - 1º Tiles
             * 2 - 2º Tiles
             * 3 - 1º Cell
             * 4 - ??
             * 5 - 1º Animation
             * 6 - 1º Map
             * 8 - ??
             * 9 - ??
             */

            sFolder unpacked = pluginHost.Get_DecompressedFiles(id);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(file));
            uint offset = (uint)0x09 * 8 + 0xC;

            bw.Write(new char[] { 'N', 'P', 'C', 'K' });
            bw.Write(offset);
            bw.Write((uint)0x09);

            for (int i = 0; i < 9; i++)
            {
                sFile currFile = Search_File("File" + i.ToString() + ".bin", unpacked);

                if (currFile.name is String)
                {
                    bw.Write(offset);
                    bw.Write(currFile.size);
                    offset += currFile.size;
                }
                else
                {
                    bw.Write((uint)0x00);   // Null offset
                    bw.Write((uint)0x00);   // Null size
                }
            }
            for (int i = 0; i < 9; i++)
            {
                sFile currFile = Search_File("File" + i.ToString() + ".bin", unpacked);

                if (currFile.name is String)
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
                    br.BaseStream.Position = currFile.offset;
                    bw.Write(br.ReadBytes((int)currFile.size));
                    br.Close();
                }
            }

            bw.Flush();
            bw.Close();
        }

        public static sFile Search_File(string name, sFolder folder)
        {
            for (int i = 0; i < folder.files.Count; i++)
                if (folder.files[i].name == name)
                    return folder.files[i];

            return new sFile();
        }
    }
}
