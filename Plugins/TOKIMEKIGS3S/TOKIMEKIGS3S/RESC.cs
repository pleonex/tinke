using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace TOKIMEKIGS3S
{
    public static class RESC
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "unpack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            char[] type = br.ReadChars(4);
            uint num_files = br.ReadUInt32();
            uint unknown1 = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = new String(br.ReadChars(0x08)).Replace("\0", "");
                newFile.name += '.' + new String(br.ReadChars(0x08)).Replace("\0", "");
                newFile.name += '.' + new String(br.ReadChars(0x08)).Replace("\0", "");
                newFile.size = br.ReadUInt32();
                newFile.offset = br.ReadUInt32();
                newFile.path = packFile;

                ulong unknown3 = br.ReadUInt64();
                uint unknown4 = br.ReadUInt32();
                uint unknown5 = br.ReadUInt32();

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static String Pack(string original_file, ref sFolder unpacked, IPluginHost pluginHost)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "new_" + Path.GetFileName(original_file);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            BinaryReader br = new BinaryReader(File.OpenRead(original_file));

            // Write header
            bw.Write(br.ReadBytes(0x10));

            // Write pointer table
            uint offset = (uint)unpacked.files.Count * 0x30 + 0x10;
            uint[] offset_files = new uint[unpacked.files.Count];
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                offset_files[i] = offset;
                bw.Write(br.ReadBytes(0x18));   // File name and extension
                bw.Write(unpacked.files[i].size);
                bw.Write(offset);
                offset += unpacked.files[i].size;

                br.BaseStream.Seek(8, SeekOrigin.Current);
                bw.Write(br.ReadBytes(0x10));   // Unknown values
            }
            bw.Flush();
            br.Close();

            // Write file data
            for (int i = 0; i < unpacked.files.Count; i++)
            {
                br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;

                bw.Write(br.ReadBytes((int)unpacked.files[i].size));
                bw.Flush();
                br.Close();

                sFile newFile = unpacked.files[i];
                newFile.offset = offset_files[i];
                newFile.path = fileOut;
                unpacked.files[i] = newFile;
            }
            bw.Flush();
            bw.Close();

            return fileOut;
        }
    }
}
