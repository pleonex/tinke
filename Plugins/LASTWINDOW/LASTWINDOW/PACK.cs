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

namespace LASTWINDOW
{
    public static class PACK
    {
        public static sFolder Unpack(IPluginHost pluginHost, string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

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
                newFile.path = file;
                newFile.size = pack.files[i].size;
                newFile.id = (ushort)i;

                br.BaseStream.Seek(pack.files[i].size, SeekOrigin.Current);
                descomprimidos.files.Add(newFile);
            }

            br.Close();
            return descomprimidos;
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
