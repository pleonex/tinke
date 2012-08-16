// ----------------------------------------------------------------------
// <copyright file="HONZ_Books.cs" company="none">

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
// <date>13/08/2012 14:08:15</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace Pack.Games
{
    public class HONZ_Books : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "YBNP")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.id >= 0x2F && file.id <= 0x93)
                return Format.Pack;

            return Format.Unknown;
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
            if (file.id >= 0x23 && file.id <= 0x93)
            {
                sFolder unpack = new sFolder();
                unpack.files = new List<sFile>();
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));

                for (int i = 0; i < 0x17; i++)
                {
                    sFile cfile = new sFile();
                    cfile.name = "Block " + i.ToString() + ".bin";
                    cfile.path = file.path;

                    br.BaseStream.Position = 0x14 + i * 4;
                    cfile.offset = br.ReadUInt32();
                    br.BaseStream.Position = 0x70 + i * 4;
                    cfile.size = br.ReadUInt32();

                    unpack.files.Add(cfile);
                }

                br.Close();
                br = null;
                return unpack;
            }

            return new sFolder();
        }
        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.id >= 0x23 && file.id <= 0x93)
            {
                string output = pluginHost.Get_TempFile();
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(output));
                
                // Unknown first 0x14 bytes
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                bw.Write(br.ReadBytes(0x14));
                br.Close();
                br = null;

                // Write offset
                uint offset = 0xCC;
                List<byte> buffer = new List<byte>();
                for (int i = 0; i < 0x17; i++)
                {
                    bw.Write(offset);

                    // Write the file data to a temp buffer
                    sFile cfile = unpacked.files[i];
                    br = new BinaryReader(File.OpenRead(cfile.path));
                    br.BaseStream.Position = cfile.offset;
                    buffer.AddRange(br.ReadBytes((int)cfile.size));
                    br.Close();
                    br = null;

                    // Update file info
                    cfile.offset = offset;
                    cfile.path = output;
                    unpacked.files[i] = cfile;

                    offset += cfile.size;   // Update offset
                }
                // Write size
                for (int i = 0; i < 0x17; i++)
                    bw.Write(unpacked.files[i].size);
                bw.Flush();

                // Write data
                bw.Write(buffer.ToArray());

                // Close and return
                bw.Flush();
                bw.Close();
                bw = null;
                return output;
            }

            return null;
        }

    }
}
