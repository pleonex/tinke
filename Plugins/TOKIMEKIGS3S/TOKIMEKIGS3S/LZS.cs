// ----------------------------------------------------------------------
// <copyright file="LZS.cs" company="none">

// Copyright (C) 2012
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
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>29/06/2012 2:10:23</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace TOKIMEKIGS3S
{
    public static class LZS
    {
        public static sFile Decompress(string file, IPluginHost pluginHost)
        {
            sFile decompressed;

            string parent_name = Path.GetFileNameWithoutExtension(file).Substring(12);

            string temp = parent_name + ".resc";
            Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x10];
            Array.Copy(File.ReadAllBytes(file), 0x10, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(temp, compressFile);

            pluginHost.Decompress(temp);
            decompressed = pluginHost.Get_Files().files[0];
            File.Delete(temp);

            return decompressed;
        }

        public static String Compress(string fileIn, string originalFile, IPluginHost pluginHost)
        {
            String fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "new_" + Path.GetFileName(originalFile);

            // Read unknown header
            BinaryReader br = new BinaryReader(File.OpenRead(originalFile));
            byte[] header = br.ReadBytes(0x10);
            br.Close();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            bw.Write(header);

            // Compress the file with LZ11
            String tempFile = Path.GetTempFileName();
            pluginHost.Compress(fileIn, tempFile, FormatCompress.LZ11);
            bw.Write(File.ReadAllBytes(tempFile));
            bw.Flush();
            bw.Close();

            File.Delete(tempFile);
            return fileOut;
        }
    }
}
