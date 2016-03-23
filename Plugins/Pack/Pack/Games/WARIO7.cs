//
//  AWAE.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ekona;

namespace Pack.Games
{
    public class WARIO7 : IGamePlugin
    {
        private string gameCode;
        private string[] arcNames = { "wtp_os.arc", "wtp_eu.arc" };

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.gameCode = gameCode;
        }

        public bool IsCompatible()
        {
            return gameCode == "AWAE" || gameCode == "AWAP";
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            return arcNames.Contains(file.name) ? Format.Pack : Format.Unknown;
        }

        public Control Show_Info(sFile file)
        {
            return new Control();
        }

        public void Read(sFile file)
        {
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            MessageBox.Show("Not implemented.");
            return null;
        }

        public sFolder Unpack(sFile file)
        {
            return arcNames.Contains(file.name) ? UnpackArc(file.path) : new sFolder();
        }

        private static sFolder UnpackArc(string fileIn)
        {
            Console.WriteLine("Unpacking ARC file:");
            var reader = new BinaryReader(File.OpenRead(fileIn));
            var unpacked = new sFolder();

            uint numFolders = reader.ReadUInt32();
            unpacked.folders = new List<sFolder>();
            for (int i = 0; i < numFolders; i++) {
                Console.WriteLine("Reading folder " + i);
                reader.BaseStream.Position = 4 + i * 8;

                uint fatOffset = reader.ReadUInt32();
                uint numFiles = reader.ReadUInt32();

                var subfolder = new sFolder();
                subfolder.name = "Folder" + i;
                subfolder.files = new List<sFile>();
                for (int j = 0; j < numFiles; j++) {
                    reader.BaseStream.Position = fatOffset + j * 4;
                    uint fileOffset = reader.ReadUInt32();

                    var subfile = new sFile();
                    subfile.name = "file" + i + "_" + j;
                    subfile.offset = fileOffset + 8;
                    subfile.path = fileIn;

                    reader.BaseStream.Position = fileOffset;
                    subfile.size = reader.ReadUInt32();
                    uint unknown = reader.ReadUInt32();
                    Console.WriteLine("\tunknown for {0} is {1}", subfile.name, unknown);
                    subfolder.files.Add(subfile);
                }

                unpacked.folders.Add(subfolder);
            }

            reader.Close();
            return unpacked;
        }
    }
}

