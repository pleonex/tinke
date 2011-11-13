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
                newFile.name = "File" + i.ToString() + Get_Extension(num_files, i);
                newFile.offset = offset;
                newFile.size = size;
                newFile.path = packFile;

                unpack.files.Add(newFile);
            }

            br.Close();
            return unpack;
        }
        public static void Pack(string file, ref sFolder unpacked, IPluginHost pluginHost)
        {
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
                    currFile.offset = offset;

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

        public static string Get_Extension(uint packType, int num_file)
        {
            switch (packType)
            {
                case 9: // N2D (Nitro 2D), files with 2D images
                    switch (num_file)
                    {
                        case 0: return ".nclr";
                        case 1: return ".ncgr";
                        case 2: return ".ncgr";
                        case 3: return ".ncer";
                        case 5: return ".nanr";
                        case 6: return ".nscr";
                        default: return ".bin";
                    }

                case 6: // N3D (Nitro 3D), files with 3D models and animations
                    switch (num_file)
                    {
                        case 0: return ".bmd0";
                        case 1: return ".bca0";
                        case 2: return ".bva0";
                        case 3: return ".bma0";
                        case 4: return ".bta0";
                        case 5: return ".btp0";
                        default: return ".bin";
                    }

                case 2: // NPD (Nitro Pulse Digitial??), files with sounds
                    switch (num_file)
                    {
                        case 0: return ".sedl";
                        case 1: return ".swdl";
                        default: return ".bin";
                    }

                default:
                    return ".bin";
            }
        }
    }

    /* In N2D files there must be 9 offset in this order:
    * 
    * 0 - Palette       (nclr)
    * 1 - 1º Tiles      (ncgr)
    * 2 - 2º Tiles      (ncgr)
    * 3 - 1º Cell       (ncer)
    * 4 - Nothing
    * 5 - 1º Animation  (nanr)
    * 6 - 1º Map        (nscr)
    * 7 - Nothing
    * 8 - Nothing
    */

    /* In NPD files there must be 2 offset in this order:
     * 
     * 0 - sedl         (sedl)
     * 1 - swdl         (swdl)
     */

    /* In N3D files there must be 6 offset in this order:
     * 
     * 0 - BMD0
     * 1 - BCA0
     * 2 - BVA0
     * 3 - BMA0
     * 4 - BTA0
     * 5 - BTP0
     */
}
