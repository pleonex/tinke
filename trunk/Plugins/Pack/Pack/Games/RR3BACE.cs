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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ekona;
using System.IO;

namespace Pack.Games
{
    /// <summary>
    /// MEGA MAN STAR FORCE 3 - BLACK ACE
    /// </summary>
    public class RR3BACE : IGamePlugin
    {
        IPluginHost pluginHost;
        String gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        public bool IsCompatible()
        {
            if (gameCode == "CRBE")
                return true;

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.id >= 0x9E && file.id <= 0xD8 && file.id != 0xBA)
                return Format.Pack;

            return Format.Unknown;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            if (file.id >= 0x9E && file.id <= 0xD8 && file.id != 0xBA)
            {
                string fileOut = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
                Pack(fileOut, ref unpacked);

                return fileOut;
            }

            return null;
        }
        public sFolder Unpack(sFile file)
        {
            if (file.id >= 0x9E && file.id <= 0xD8 && file.id != 0xBA)
                return Unpack(file.path, file.name);

            return new sFolder();
        }

        public void Read(sFile file)
        {
        }
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            return new System.Windows.Forms.Control();
        }

        public static sFolder Unpack(string file, string name)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();

            int num_files = (int)br.ReadUInt32() / 0x08;
            num_files--;    // The last offset indicates the end of the file
            br.BaseStream.Position = 0;

            for (int i = 0; i < num_files; i++)
            {
                sFile newFile = new sFile();
                newFile.offset = br.ReadUInt32();
                newFile.size = br.ReadUInt32() & 0x7FFFFFF;    // The last bit indicates if the file it's compressed
                newFile.path = file;
                newFile.name = name + '_' + i.ToString();

                #region Get the extension
                long currPos = br.BaseStream.Position;
                br.BaseStream.Position = newFile.offset;
                char[] ext;
                if (newFile.size < 4)
                    ext = Encoding.ASCII.GetChars(br.ReadBytes((int)newFile.size));
                else
                    ext = Encoding.ASCII.GetChars(br.ReadBytes(4));

                String extS = ".";
                for (int s = 0; s < ext.Length; s++)
                    if (Char.IsLetterOrDigit(ext[s]) || ext[s] == 0x20)
                        extS += ext[s];

                if (extS != "." && extS.Length == 5 && newFile.size >= 4)
                    newFile.name += extS;
                else
                    newFile.name += ".bin";
                br.BaseStream.Position = currPos;
                #endregion

                unpacked.files.Add(newFile);
            }

            br.Close();
            return unpacked;
        }
        public static void Pack(string file, ref sFolder unpacked)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(file));
            BinaryReader br;
            List<byte> buffer = new List<byte>();

            uint offset = (uint)(unpacked.files.Count + 1) * 8;

            for (int i = 0; i < unpacked.files.Count; i++)
            {
                // Update the file
                sFile newFile = unpacked.files[i];
                newFile.path = file;
                newFile.offset = offset;
                unpacked.files[i] = newFile;

                uint size = unpacked.files[i].size;

                // Write the file to the buffer
                br = new BinaryReader(File.OpenRead(unpacked.files[i].path));
                br.BaseStream.Position = unpacked.files[i].offset;
                buffer.AddRange(br.ReadBytes((int)size));

                // Get the first byte to know it's compressed
                br.BaseStream.Position = unpacked.files[i].offset;
                byte firstb = br.ReadByte();
                br.Close();

                bw.Write(offset);
                offset += size;

                if (unpacked.files[i].format == Format.Compressed && firstb == 0x11)
                    size += 0x80000000;
                bw.Write(unpacked.files[i].size);
            }

            bw.Write((uint)buffer.Count);
            bw.Write((uint)0x00);

            bw.Write(buffer.ToArray());

            bw.Close();
        }
    }
}
