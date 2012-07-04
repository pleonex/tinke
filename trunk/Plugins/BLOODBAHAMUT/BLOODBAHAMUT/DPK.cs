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
 * By: pleoNeX, Tricky Upgrade
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace BLOODBAHAMUT
{
    public static class DPK
    {
        static IPluginHost pluginHost;

        public static sFolder Unpack(string file, IPluginHost pluginHost)
        {
            DPK.pluginHost = pluginHost;
            String packFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "pack_" + Path.GetFileName(file);
            File.Copy(file, packFile, true);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            uint num_files = br.ReadUInt32();

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.name = br.ReadUInt32().ToString() + ".bin";
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                newFile.path = packFile;

                // For Summon Night X Tears Crown game
                if (file.ToUpper().EndsWith("CBT.DPK") ||
                    file.ToUpper().EndsWith("CFL.DPK") ||
                    file.ToUpper().EndsWith("CFN.DPK") ||
                    file.ToUpper().EndsWith("ILLUEST.DPK"))
                        newFile.name += ".dpk";
                // Get the file extension
                //long pos = br.BaseStream.Position;
                //br.BaseStream.Position = newFile.offset;
                //newFile.name += new String(br.ReadChars(4));
                //br.BaseStream.Position = pos;

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }

        public static string Pack(ref sFolder unpacked, string file, int id)
        {
            unpacked.files.Sort(SortFiles);

            string fileOut = file + "new";
            Unpack(file, DPK.pluginHost);
            byte[] fileData;

            uint numberOfFiles = (uint)unpacked.files.Count;

            MemoryStream packedFiles = new MemoryStream();
            MemoryStream headerData = new MemoryStream();
            MemoryStream packedFile = new MemoryStream();
            uint headerLength = 4 + 12 * numberOfFiles + Padding(numberOfFiles);
            uint offset = headerLength;

            for (int i = 0; i < numberOfFiles; i++)
            {
                byte[] subFile = new byte[unpacked.files[i].size];
                fileData = Helper.ReadFile(unpacked.files[i].path);
                for (uint j = 0, k = unpacked.files[i].offset; j < unpacked.files[i].size; j++, k++)
                {
                    try
                    {
                        subFile[j] = fileData[k];
                    }
                    catch { }
                }

                sFile newFile = unpacked.files[i];
                //newFile.name = unpacked.files[i].name.Remove(unpacked.files[i].name.LastIndexOf('.'));
                newFile.size = (uint)subFile.Length;
                newFile.offset = offset;
                newFile.path = fileOut;
                offset += newFile.size;
                unpacked.files[i] = newFile;
                packedFiles.Write(subFile, 0, subFile.Length);
            }

            BinaryWriter headerWriter = new BinaryWriter(headerData);
            headerWriter.Write(numberOfFiles);
            for (int i = 0; i < numberOfFiles; i++)
            {
                headerWriter.Write(uint.Parse(unpacked.files[i].name.Remove(unpacked.files[i].name.LastIndexOf('.'))));
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

            Helper.WriteFile(fileOut, packedFile.ToArray(), 0, (int)packedFile.Length);

            return fileOut;
        }

        private static int SortFiles(sFile f1, sFile f2)
        {
            return f1.id.CompareTo(f2.id);
        }

        public static uint Padding(uint numberOfFiles)
        {
            return (0x10 - ((4 + 12 * numberOfFiles) % 0x10)) % 0x10;
        }
    }
}
