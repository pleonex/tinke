/*
 * Copyright (C) 2011  pleoNeX, Tricky Upgrade
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
 * Programador: pleoNeX, Tricky Upgrade
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace BLOODBAHAMUT
{
    public static class DPK
    {
        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "pack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);
            byte[] fileData = Helper.ReadFile(file);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = br.ReadUInt32().ToString();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = packFile + "_" + newFile.name;

                // Get the file extension
                //long pos = br.BaseStream.Position;
                //br.BaseStream.Position = newFile.offset;
                //newFile.name += new String(br.ReadChars(4));
                //br.BaseStream.Position = pos;
                WriteFile(newFile.path, fileData, (int)newFile.offset, (int)newFile.size);

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static string Pack(sFolder unpacked, string file, int id)
        {
            uint numberOfFiles = (uint)unpacked.files.Count;

            MemoryStream packedFiles = new MemoryStream();
            MemoryStream headerData = new MemoryStream();
            MemoryStream packedFile = new MemoryStream();
            uint headerLength = 4 + 12 * numberOfFiles + Padding(numberOfFiles);
            uint offset = headerLength;

            for (int i = 0; i < numberOfFiles; i++)
            {
                byte[] subFile = Helper.ReadFile(unpacked.files[i].path);
                sFile newFile = unpacked.files[i];
                newFile.name = unpacked.files[i].name;
                newFile.size = (uint)subFile.Length;
                newFile.offset = offset;
                offset += newFile.size;
                unpacked.files[i] = newFile;
                packedFiles.Write(subFile, 0, subFile.Length);
            }

            BinaryWriter headerWriter = new BinaryWriter(headerData);
            headerWriter.Write(numberOfFiles);
            for (int i = 0; i < numberOfFiles; i++)
            {
                headerWriter.Write(uint.Parse(unpacked.files[i].name));
                headerWriter.Write(unpacked.files[i].offset);
                headerWriter.Write(unpacked.files[i].size);
            }
            byte padding = 0xEE;
            for (uint i = Padding(numberOfFiles); i > 0; i--)
            {
                headerWriter.Write(padding);
            }

            headerData.WriteTo(packedFile);
            packedFiles.WriteTo(packedFile);

            WriteFile(file, packedFile.ToArray(), 0, (int)packedFile.Length);

            return file;
        }

        public static void WriteFile(string fileName, byte[] data, int offset, int length)
        {
            FileStream fileStream;
            string directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            fileStream = null;
            try
            {
                fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fileStream.Write(data, offset, length);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public static uint Padding(uint numberOfFiles)
        {
            return (0x10 - ((4 + 12 * numberOfFiles) % 0x10)) % 0x10;
        }
    }
}
