using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace LASTWINDOW
{
    public static class PACK
    {
        public static void Leer(IPluginHost pluginHost, string archivo)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            File.Copy(archivo, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            string tempFolder = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(archivo);
            Directory.CreateDirectory(tempFolder);

            sFolder descomprimidos = new sFolder(); // Donde guardaremos los archivos descomprimidos
            sPACK pack = new sPACK();

            pack.unknown1 = br.ReadBytes(7);
            pack.nFiles = br.ReadByte();
            pack.unknown2 = br.ReadBytes(8);

            pack.files = new sPACK.File[pack.nFiles];
            for (int i = 0; i < pack.nFiles; i++) // Lectura de nombres y tamaño de archivos
            {
                pack.files[i] = new sPACK.File();

                byte nameSize = br.ReadByte();
                pack.files[i].name = new String(br.ReadChars((int)nameSize));
                pack.files[i].size = BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
            }

            descomprimidos.files = new List<sFile>();
            for (int i = 0; i < pack.nFiles; i++)
            {
                sFile newFile = new sFile();
                newFile.name = pack.files[i].name;
                newFile.offset = (uint)br.BaseStream.Position;
                newFile.path = packFile;
                newFile.size = pack.files[i].size;
                newFile.id = (ushort)i;

                br.BaseStream.Seek(pack.files[i].size, SeekOrigin.Current);
                descomprimidos.files.Add(newFile);
            }

            pluginHost.Set_Files(descomprimidos);
            br.Close();
        }
    }

    public struct sPACK
    {
        public byte[] unknown1; // First 6 bytes always 0x00 ¿?
        public byte nFiles;
        public byte[] unknown2;
        public File[] files;

        public struct File
        {
            public string name;
            public uint size;
        }
    }
}
