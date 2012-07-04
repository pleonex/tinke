using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace NINOKUNI
{
    public static class KPCN
    {
        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            // Header
            char[] type = br.ReadChars(4);
            uint first_fileOffset = br.ReadUInt32();
            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                uint unknown = br.ReadUInt32();

                sFile newFile = new sFile();
                newFile.name = name + '_' + i.ToString() + ".bin";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = file;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public static void Pack(string fileOut, string fileIn, ref sFolder unpacked, IPluginHost pluginHost)
        {
            // I need the original file to read all unknown values
            List<uint> unknowns = new List<uint>();
            #region Read unknowns values
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            br.BaseStream.Position += 8;
            uint num_files = br.ReadUInt32();
            for (int i = 0; i < num_files; i++)
            {
                unknowns.Add(br.ReadUInt32());
                br.BaseStream.Position += 8;
            }
            br.Close();
            #endregion

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));

            uint offset = (uint)unpacked.files.Count * 0x0C + 0x0C;
            bw.Write(new char[] { 'K', 'P', 'C', 'N' });
            bw.Write(offset);
            bw.Write((uint)unpacked.files.Count);

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                sFile newFile = unpacked.files[i];
                newFile.offset = offset;
                newFile.path = fileOut;
                unpacked.files[i] = newFile;

                bw.Write(unknowns[i]);
                bw.Write(offset);
                bw.Write(unpacked.files[i].size);
                offset += unpacked.files[i].size;
            }

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;

                bw.Write(br.ReadBytes((int)unpacked.files[i].size));
                br.Close();
            }

            bw.Flush();
            bw.Close();

        }
    }
}
