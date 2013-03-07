//-----------------------------------------------------------------------
// <copyright file="Packer.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>02/03/2013</date>
//-----------------------------------------------------------------------
namespace Tokimemo1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Ekona;
    
    /// <summary>
    /// Description of Pack.
    /// </summary>
    public static class Packer
    {
        public static sFolder Unpack(sFile file, IPluginHost pluginHost)
        {
            string filename = Path.GetFileNameWithoutExtension(file.name);
            
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string decodedPath = file.path;
            byte b = br.ReadByte();
            br.Close();
            if (b == 0x10)
            {
                pluginHost.Decompress(file.path);
                decodedPath = pluginHost.Get_Files().files[0].path;
            }
            
            br = new BinaryReader(File.OpenRead(decodedPath));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            
            uint numFiles = br.ReadUInt32();
            for (int i = 0; i < numFiles; i++)
            {
                br.BaseStream.Position = 4 + i * 8;
                
                sFile newFile = new sFile();
                newFile.path = decodedPath;
                newFile.name = filename + "_" + i.ToString();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32();
                
                if (IsPack(br.BaseStream, newFile.offset, newFile.size))
                    newFile.name += ".pack";
                else
                    newFile.name += ".bin";
                
                unpacked.files.Add(newFile);
            }
            
            br.Close();
            br = null;
            return unpacked;
        }
        
        public static bool IsPack(Stream str, uint offset, uint size)
        {
            BinaryReader br = new BinaryReader(str);
            br.BaseStream.Position = offset;
            bool isPack = true;
            
            uint numFiles = br.ReadUInt32();
            if (numFiles > 0x400)
                return false;
            uint nextOffset = (uint)(numFiles * 8 + 4);
            if (nextOffset % 0x10 != 0)
                nextOffset += 0x10 - (nextOffset % 0x10);
            
            for (int i = 0; i < numFiles && isPack; i++)
            {
                if (nextOffset != br.ReadUInt32())
                    isPack = false;
                
                nextOffset += br.ReadUInt32();
                if (nextOffset % 0x10 != 0)
                    nextOffset += 0x10 - (nextOffset % 0x10);
            }
            
            if (nextOffset != size)
                isPack = false;
            
            br = null;
            return isPack;
        }
    }
}
