using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class PCM
    {
        public static void Descomprimir(string file, string folderOut)
        {
            // Comprobación de si está comprimido con LZ77 o Huffman
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            byte id = br.ReadByte();
            br.Close();
            br.Dispose();
            if (id == 0x10 || id == 0x20)
            {
                Basico.Decompress(file, file + ".un", false);
                file += ".un";
            }

            br = new BinaryReader(File.OpenRead(file));
            sPCM pcm = new sPCM();

            pcm.header_size = br.ReadUInt32();
            pcm.file_size = br.ReadUInt32();
            pcm.nFiles = br.ReadUInt32();
            pcm.id = br.ReadChars(4);

            uint localizador;
            pcm.files = new KCPL_File[pcm.nFiles];

            for (int i = 0; i < pcm.nFiles; i++)
            {
                // Busca el localizador 
                do
                    localizador = br.ReadByte();
                while (localizador != 0x20);
                br.ReadBytes(3);

                pcm.files[i].file_size = br.ReadUInt32();
                pcm.files[i].unknown = br.ReadUInt32();
                pcm.files[i].data_size = br.ReadUInt32();
                pcm.files[i].name = new String(br.ReadChars(16)).Replace("\0", "");

                BinaryWriter bw = new BinaryWriter(new FileStream(folderOut + '\\' + pcm.files[i].name, FileMode.Create, FileAccess.Write));
                bw.Write(br.ReadBytes((int)pcm.files[i].data_size));
                bw.Flush();
                bw.Close();
            }
        }

        public struct sPCM
        {
            public uint header_size;
            public uint file_size;
            public uint nFiles;
            public char[] id;
            public KCPL_File[] files;
        }
        public struct KCPL_File
        {
            public uint file_size;
            public uint unknown;
            public uint data_size;
            public string name;
        }
    }
}
