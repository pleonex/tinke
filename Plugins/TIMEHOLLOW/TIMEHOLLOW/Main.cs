// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

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

// <author>Daviex94</author>
// <email>david.iuffri94@hotmail.it</email>
// <date>17/07/2012 13:39:00:</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using Ekona;
using Ekona.Images;

namespace TIMEHOLLOW
{
    public class Main : IGamePlugin
    {
        string gameCode;
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
            this.pluginHost = pluginHost;
        }

        public bool IsCompatible()
        {
            if (gameCode == "YHLP" || gameCode == "YHLE")
                return true;
            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.ToUpper().EndsWith(".ARC"))
            {
               return Format.Pack;
            }
            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }

        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }

        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            throw new NotImplementedException();
        }

        public sFolder Unpack(sFile file)
        {
            uint num_files;
            uint offFNT; //Pointer fileName
            uint offFAT; //Pointer 
            uint offUnk;
            uint offFS;
            uint padding;

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));

            br.ReadUInt32(); //Header
            num_files = br.ReadUInt32(); //Number of files
            offFNT = br.ReadUInt32(); //Offset File Name
            offFAT = br.ReadUInt32(); //Offset File Allocation
            offUnk = br.ReadUInt32(); //Unknown
            offFS = br.ReadUInt32(); //Offset File Size
            padding = br.ReadUInt32(); //Padding

            br.BaseStream.Position = offFNT;
            uint size;
            uint numNames;
            byte[] names;
            string[] fileNames;

            size = br.ReadUInt32(); //Size Section
            numNames = br.ReadUInt32(); //Number of Entries
            names = br.ReadBytes((int)size);
            fileNames = new string[(int)numNames];

            int k = 0;

            for (int i = 0; i < numNames; i++)
            {
                for (; ; k++)
                {
                    if (names[k] == 0x00)
                    {
                        k++;
                        break;
                    }
                    fileNames[i] += (char)names[k];
                }
            }

            uint sizeFCmp;
            uint sizeFDcmp;
            uint offFile;

            br.BaseStream.Position = offFAT;

            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            for (int i = 0; i < num_files; i++)
            {
                sizeFCmp = br.ReadUInt32(); //Size File Compress
                sizeFDcmp = br.ReadUInt32(); //Size File Decompressed
                offFile = br.ReadUInt32(); //Offset to add to Pointer of File Section
                br.ReadUInt32(); // Unknown

                sFile newFile = new sFile();
                newFile.size = sizeFDcmp;
                newFile.name = fileNames[i];
                newFile.offset = 0;
                newFile.path = pluginHost.Get_TempFile();

                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = offFS + offFile;
                byte[] encrypted = br.ReadBytes((int)sizeFCmp);
                br.BaseStream.Position = currPos;

                byte[] decompress = Compression.Decompress(encrypted, sizeFDcmp);
                File.WriteAllBytes(newFile.path, decompress);

                unpacked.files.Add(newFile);
            }
            br.Close();
            return unpacked;
        }
    }
}
