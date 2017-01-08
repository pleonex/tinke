/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace LAYTON
{
    public static class PCK2
    {
        public static sFolder Read(string file, int id, IPluginHost pluginHost)
        {
            pluginHost.Decompress(file);
            string dec_file = pluginHost.Get_Files().files[0].path;

            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));
            sFolder decompressed = new sFolder();
            sPCK2 pck2 = new sPCK2();
            pck2.file_id = id;

            // Read the header
            pck2.header.header_size = br.ReadUInt32();
            pck2.header.file_size = br.ReadUInt32();
            pck2.header.id = br.ReadChars(4);
            pck2.header.unknown = br.ReadUInt32();

            // Read all files
            decompressed.files = new List<sFile>();
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
                sFile savedFile = new sFile();
                savedFile.name = newFile.name;
                savedFile.offset = 0x00;
                savedFile.size = newFile.data_size;
                savedFile.path = Path.GetTempPath() + Path.DirectorySeparatorChar + newFile.name;
                File.WriteAllBytes(savedFile.path, newFile.data);
                decompressed.files.Add(savedFile);
            }

            br.Close();

            return decompressed;
        }

        public static void Pack(string outPath, List<sFile> files)
        {
            // Update structure
            sPCK2 pack = new sPCK2();
            pack.header = new sPCK2.Header();
            pack.header.id = new[] { 'P', 'C', 'K', '2' };
            pack.header.header_size = 0x10;
            pack.header.unknown = 0;
            pack.header.file_size = pack.header.header_size;
            pack.files = new List<sPCK2.File>(files.Count);
            for (int i = 0; i < files.Count; i++)
            {
                sPCK2.File file = new sPCK2.File();
                file.unknown = 0;
                file.name = files[i].name;
                file.data_size = files[i].size;
                file.header_size = (uint)(0x10 + files[i].name.Length);
                file.header_size += 4 - file.header_size % 4;
                file.size = file.data_size + file.header_size;
                file.size += 4 - file.size % 4;
                pack.header.file_size += file.size;

                BinaryReader br = new BinaryReader(File.OpenRead(files[i].path));
                br.BaseStream.Position = files[i].offset;
                file.data = br.ReadBytes((int)files[i].size);
                br.Close();

                pack.files.Add(file);
            }

            // Write data
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(outPath), Encoding.ASCII);
            bw.BaseStream.SetLength(0);
            bw.Write(pack.header.header_size);
            bw.Write(pack.header.file_size);
            bw.Write(pack.header.id);
            bw.Write(pack.header.unknown);

            bw.BaseStream.Position = pack.header.header_size;
            for (int i = 0; i < files.Count; i++)
            {
                long pos = bw.BaseStream.Position;
                bw.Write(pack.files[i].header_size);
                bw.Write(pack.files[i].size);
                bw.Write(pack.files[i].unknown);
                bw.Write(pack.files[i].data_size);
                bw.Write(Encoding.ASCII.GetBytes(pack.files[i].name));
                bw.BaseStream.Position = pos + pack.files[i].header_size;
                bw.Write(pack.files[i].data);
                bw.BaseStream.Position = pos + pack.files[i].size;
            }

            bw.BaseStream.Position = bw.BaseStream.Length;
            bw.Write(new byte[pack.header.file_size - bw.BaseStream.Length]);
            bw.Flush();
            bw.Close();
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
