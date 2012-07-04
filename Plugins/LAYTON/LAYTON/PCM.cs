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
    public static class PCM
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sPCM pcm = new sPCM();

            pcm.header_size = br.ReadUInt32();
            pcm.file_size = br.ReadUInt32();
            pcm.nFiles = br.ReadUInt32();
            pcm.id = br.ReadChars(4);

            uint localizador;
            pcm.files = new KCPL_File[pcm.nFiles];
            sFile[] files = new sFile[pcm.nFiles];

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
                pcm.files[i].offset = (uint)br.BaseStream.Position;

                br.BaseStream.Seek(pcm.files[i].data_size, SeekOrigin.Current);

                files[i].name = pcm.files[i].name;
                files[i].path = packFile;
                files[i].offset = pcm.files[i].offset;
                files[i].size = pcm.files[i].data_size;
            }
            br.Close();

            sFolder carpeta = new sFolder();
            carpeta.files = new List<sFile>();
            carpeta.files.AddRange(files);

            return carpeta;
        }
    }
    #region Structures
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
        public uint offset;
    }
    #endregion

}