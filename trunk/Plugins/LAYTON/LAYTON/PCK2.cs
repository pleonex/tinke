using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace LAYTON
{
    public static class PCK2
    {
        public static void Read(string file, int id, IPluginHost pluginHost)
        {
            pluginHost.Descomprimir(file);
            string dec_file = pluginHost.Get_Files().files[0].path;

            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));
            Carpeta decompressed = new Carpeta();
            sPCK2 pck2 = new sPCK2();
            pck2.file_id = id;

            // Read the header
            pck2.header.header_size = br.ReadUInt32();
            pck2.header.file_size = br.ReadUInt32();
            pck2.header.id = br.ReadChars(4);
            pck2.header.unknown = br.ReadUInt32();

            // Read all files
            decompressed.files = new List<Archivo>();
            br.BaseStream.Position = pck2.header.header_size;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                long pos = br.BaseStream.Position;
                sPCK2.File newFile = new sPCK2.File();
                newFile.header_size = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.unknown = br.ReadUInt32();
                newFile.data_size = br.ReadUInt32();

                newFile.name = "";
                char c = br.ReadChar();
                while (c != '\x0')
                {
                    newFile.name += c;
                    c = br.ReadChar();
                }

                br.BaseStream.Position = pos + newFile.header_size;
                newFile.data = br.ReadBytes((int)newFile.data_size);
                br.BaseStream.Position = pos + newFile.size;

                // Save the new file
                Archivo savedFile = new Archivo();
                savedFile.name = newFile.name;
                savedFile.offset = 0x00;
                savedFile.size = newFile.data_size;
                savedFile.path = Path.GetTempPath() + Path.DirectorySeparatorChar + newFile.name;
                File.WriteAllBytes(savedFile.path, newFile.data);
                decompressed.files.Add(savedFile);
            }

            br.Close();
            pluginHost.Set_Files(decompressed);
        }
    }

    public struct sPCK2
    {
        public int file_id;
        public Header header;
        public List<File> files;

        public struct Header
        {
            public uint header_size;
            public uint file_size;
            public char[] id;
            public uint unknown; // always 0x00
        }
        public struct File
        {
            public uint header_size;
            public uint size;
            public uint unknown; // always 0x00
            public uint data_size;
            public string name;
            public byte[] data;
        }
    }
}
