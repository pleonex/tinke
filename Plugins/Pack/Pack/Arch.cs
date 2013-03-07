//-----------------------------------------------------------------------
// <copyright file="Arch.cs" company="none">
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
// <date>06/03/2013</date>
//-----------------------------------------------------------------------
namespace Pack
{
    /*
        Header:
        0x00 - 0x04 -> Header
        0x04 - 0x08 -> Number of files
        0x08 - 0x0C -> Offset to FNT (file name table) section
        0x0C - 0x10 -> Offset to FAT (file allocation table) section
        0x10 - 0x14 -> Offset to NAT section
        0x14 - 0x18 -> Offset to File section
        0x18 - 0x20 -> Padding
        
        FNT section:
        0x00 - 0x04 -> Size of the section
        0x04 - 0x08 -> Number of entries
        0x08 - .... -> File names. Each name end with 0x00 byte
        
        FAT section:
        0x10 bytes per file:
        0x00 - 0x04 -> Size of file
        0x04 - 0x08 -> Size of file decoded (if no encoding -> 0)
        0x08 - 0x0C -> Relative offset
        0x0C - 0x0E -> Relative offset to filename (again)
        0x0E - 0x10 -> Flag to indicate whethever the file is encoded.
        
        NAT section:
        0x04 bytes per file:
        0x00 - file ID
        0x02 - relative offset to filename
     */
    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Ekona;
    
    /// <summary>
    /// Operations with ARCH pack file.
    /// </summary>
    public class Arch : IPlugin
    {
        private const int Padding = 0x10;
        private const string MagicStamp = "ARCH";
        private static readonly Encoding DefaultEncoding = Encoding.ASCII;

        private IPluginHost pluginHost;
                        
        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            if (Encoding.ASCII.GetString(magic) == MagicStamp)
                return Format.Pack;

            return Format.Unknown;
        }

        public sFolder Unpack(sFile file)
        {
            Stream strIn = File.OpenRead(file.path);
            BinaryReader br = new BinaryReader(strIn);
            sFolder unpacked = new sFolder();
            unpacked.files = new List<sFile>();
            unpacked.folders = new List<sFolder>();

            // Read header
            string magicStamp = new string(br.ReadChars(4));
            uint numFiles = br.ReadUInt32();
            uint fntOffset = br.ReadUInt32();
            uint fatOffset = br.ReadUInt32();
            uint natOffset = br.ReadUInt32();
            uint filesOffset = br.ReadUInt32();

            // Extract files
            for (int i = 0; i < numFiles; i++)
            {
                strIn.Position = natOffset;
                SetNameOffset(strIn, i);
                ushort nameOffset = br.ReadUInt16();

                strIn.Position = fntOffset + nameOffset;
                string filename = ReadString(strIn);

                strIn.Position = fatOffset + (0x10 * i);
                int encodedSize = br.ReadInt32();
                int decodedSize = br.ReadInt32();
                uint fileOffset = br.ReadUInt32() + filesOffset;
                ushort nameOffset2 = br.ReadUInt16();
                bool isEncoded = br.ReadUInt16() == 1;

                bool result;
                Console.Write("{0} file {1}... ", isEncoded ? "Decoding" : "Saving", filename);
                sFile newFile = new sFile();
                if (isEncoded)
                {
                    string decodedPath = pluginHost.Get_TempFile();
                    newFile.offset = 0;
                    newFile.size = (uint)decodedSize;
                    newFile.path = decodedPath;

                    Decoder dec = new Decoder(strIn, fileOffset, encodedSize, decodedSize);
                    result = dec.Decode(decodedPath);
                }
                else
                {
                    newFile.offset = fileOffset;
                    newFile.path = file.path;
                    newFile.size = (uint)encodedSize;

                    result = true;
                }

                Console.Write("{0}", result ? "Ok" : "Fail");
                AddFile(unpacked, newFile, filename);
            }

            br.Close();
            br = null;
            return unpacked;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            sFile[] files = GetFiles(unpacked);
            int numFiles = files.Length;

            // Write the sections       
            BinaryWriter bw;

            // A) Fnt
            MemoryStream fntStr = new MemoryStream();
            bw = new BinaryWriter(fntStr);
            ushort[] namesOffsets = new ushort[numFiles];

            bw.Write(0x00);     // I'll write later the section size
            bw.Write(numFiles);
            for (int i = 0; i < numFiles; i++)
            {
                namesOffsets[i] = (ushort)fntStr.Position;
                WriteString(fntStr, files[i].name);
            }

            WritePadding(fntStr);

            // Now write section size
            fntStr.Position = 0;
            bw.Write((uint)fntStr.Length);
            bw.Flush();
            bw = null;

            // B) Fat
            MemoryStream fatStr = new MemoryStream();
            bw = new BinaryWriter(fatStr);

            uint offset = 0x00;
            for (int i = 0; i < numFiles; i++)
            {
                bw.Write(files[i].size);
                bw.Write(0x00);
                bw.Write(offset);
                bw.Write(namesOffsets[i]);
                bw.Write((ushort)0x00);     // No encoding

                offset = AddPadding(offset + files[i].size);
            }

            bw.Flush();
            bw = null;

            // C) Nat
            MemoryStream natStr = new MemoryStream();
            bw = new BinaryWriter(natStr);

            for (int i = 0; i < numFiles; i++)
            {
                bw.Write((ushort)i);
                bw.Write(namesOffsets[i]);
            }

            WritePadding(natStr);
            bw.Flush();
            bw = null;

            // D) Write file
            string outFile = pluginHost.Get_TempFile();
            Stream strOut = File.OpenWrite(outFile);
            bw = new BinaryWriter(strOut);

            // Calculate section offsets
            uint fntOffset = 0x20;                              // After header
            uint fatOffset = fntOffset + (uint)fntStr.Length;   // After Fnt
            uint natOffset = fatOffset + (uint)fatStr.Length;   // After Fat
            uint filesOffset = natOffset + (uint)natStr.Length; // After Nat

            // Write header
            bw.Write(Encoding.ASCII.GetBytes(MagicStamp));
            bw.Write(numFiles);
            bw.Write(fntOffset);
            bw.Write(fatOffset);
            bw.Write(natOffset);
            bw.Write(filesOffset);
            WritePadding(strOut);
            bw.Flush();

            // Write sections
            fntStr.WriteTo(strOut);
            fatStr.WriteTo(strOut);
            natStr.WriteTo(strOut);
            bw.Flush();

            // Write files
            BinaryReader br;
            for (int i = 0; i < numFiles; i++)
            {
                br = new BinaryReader(File.OpenRead(files[i].path));
                br.BaseStream.Position = files[i].offset;
                bw.Write(br.ReadBytes((int)files[i].size));
                br.Close();
                br = null;

                WritePadding(strOut);
                bw.Flush();
            }

            bw.Flush();
            bw.Close();
            bw = null;
            return outFile;
        }

        public void Read(sFile file)
        {
        }

        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            throw new NotImplementedException();
        }

        // ------------------------------------------------------------------//

        private static void AddFile(sFolder folder, sFile file, string filePath)
        {
            if (filePath.Contains("\\"))
            {
                string folderName = filePath.Substring(0, filePath.IndexOf('\\'));
                sFolder subfolder = new sFolder();
                foreach (sFolder f in folder.folders)
                {
                    if (f.name == folderName)
                    {
                        subfolder = f;
                    }
                }

                if (string.IsNullOrEmpty(subfolder.name))
                {
                    subfolder.name = folderName;
                    subfolder.folders = new List<sFolder>();
                    subfolder.files = new List<sFile>();
                    folder.folders.Add(subfolder);
                }

                AddFile(subfolder, file, filePath.Substring(filePath.IndexOf('\\') + 1));
            }
            else
            {
                file.name = filePath;
                folder.files.Add(file);
            }
        }

        private static sFile[] GetFiles(sFolder folder)
        {
            List<sFile> files = new List<sFile>();
            Queue<sFolder> queue = new Queue<sFolder>();
            folder.name = string.Empty;
            queue.Enqueue(folder);

            do
            {
                sFolder currentFolder = queue.Dequeue();
                foreach (sFolder f in currentFolder.folders)
                {
                    sFolder subfolder = f;
                    if (!string.IsNullOrEmpty(currentFolder.name))
                    {
                        subfolder.name = currentFolder.name + '\\' + subfolder.name;
                    }

                    queue.Enqueue(subfolder);
                }

                foreach (sFile f in currentFolder.files)
                {
                    sFile file = f;
                    if (!string.IsNullOrEmpty(currentFolder.name))
                        file.name = currentFolder.name + '\\' + file.name;
                    files.Add(file);
                }
            } while (queue.Count != 0);

            return files.ToArray();
        }

        private static void WritePadding(Stream str)
        {
            while (str.Position % Padding != 0)
            {
                str.WriteByte(0x00);
            }
        }

        private static uint AddPadding(uint val)
        {
            if (val % Padding != 0)
            {
                val += Padding - (val % Padding);
            }

            return val;
        }

        private static string ReadString(Stream str)
        {
            string s = string.Empty;
            List<byte> data = new List<byte>();

            while (str.ReadByte() != 0)
            {
                str.Position--;
                data.Add((byte)str.ReadByte());
            }

            s = DefaultEncoding.GetString(data.ToArray());
            data.Clear();
            data = null;

            return s;
        }

        private static void WriteString(Stream str, string s)
        {
            byte[] data = DefaultEncoding.GetBytes(s + '\0');
            str.Write(data, 0, data.Length);
        }

        private static void SetNameOffset(Stream str, int fileId)
        {
            BinaryReader br = new BinaryReader(str);
            while (br.ReadUInt16() != fileId)
            {
                br.ReadUInt16();
            }

            br = null;
        }
    }

    /// <summary>
    /// Decode Arch files.
    /// </summary>
    public class Decoder
    {
        private Stack<byte> nextSamples = new Stack<byte>(0x80);
        private byte[] buffer1 = new byte[0x100];
        private byte[] buffer2 = new byte[0x100];

        private Stream str;
        private int encodedSize;
        private int decodedSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder" /> class.
        /// </summary>
        /// <param name="file">File to decode.</param>
        public Decoder(string file)
            : this(File.OpenRead(file), -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder" /> class.
        /// </summary>
        /// <param name="str">Stream with the data encoded.</param>
        /// <param name="decodedSize">Size of the decoded file.</param>
        public Decoder(Stream str, int decodedSize)
            : this(str, 0, (int)str.Length, decodedSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decoder" /> class.
        /// </summary>
        /// <param name="str">Stream with the data encoded</param>
        /// <param name="offset">Offset to the data encoded.</param>
        /// <param name="encodedSize">Size of the encoded file.</param>
        /// <param name="decodedSize">Size of the decoded file.</param>
        public Decoder(Stream str, uint offset, int encodedSize, int decodedSize)
        {
            str.Position = offset;
            this.str = str;
            this.encodedSize = encodedSize;
            this.decodedSize = decodedSize;
        }

        /// <summary>
        /// Decode the data.
        /// </summary>
        /// <param name="fileOut">Path to the output file.</param>
        /// <returns>A value indicating whether the operation was successfully.</returns>
        public bool Decode(string fileOut)
        {
            if (File.Exists(fileOut))
            {
                File.Delete(fileOut);
            }

            FileStream fs = new FileStream(fileOut, FileMode.Create, FileAccess.Write);

            bool result = this.Decode(fs);

            fs.Flush();
            fs.Close();
            fs.Dispose();
            fs = null;

            return result;
        }

        /// <summary>
        /// Decode the data.
        /// </summary>
        /// <param name="strOut">Stream to the output file.</param>
        /// <returns>A value indicating whether the operation was successfully.</returns>
        public bool Decode(Stream strOut)
        {
            long startReading = this.str.Position;
            long startWriting = strOut.Position;

            while (this.str.Position - startReading < this.encodedSize)
            {
                InitBuffer(this.buffer2);
                this.FillBuffer();

                this.Process(strOut);
            }

            if (this.decodedSize != -1)
            {
                return (strOut.Position - startWriting) == this.decodedSize;
            }
            else
            {
                return true;
            }
        }

        private static void InitBuffer(byte[] buffer)
        {
            if (buffer.Length > 0x100)
            {
                throw new ArgumentException("Invalid buffer length", "buffer");
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;
            }
        }

        private void FillBuffer()
        {
            int index = 0;

            while (index != 0x100)
            {
                int id = this.str.ReadByte();
                int numLoops = id;

                if (id > 0x7F)
                {
                    numLoops = 0;
                    int skipPositions = id - 0x7F;
                    index += skipPositions;
                }

                if (index == 0x100)
                {
                    break;
                }

                // It's in the ARM code but... really?
                if (numLoops < 0)
                {
                    continue;
                }

                for (int i = 0; i <= numLoops; i++)
                {
                    byte b = (byte)this.str.ReadByte();
                    this.buffer2[index] = b;

                    // It'll write
                    if (b != index)
                    {
                        this.buffer1[index] = (byte)this.str.ReadByte();
                    }

                    index++;
                }
            }
        }

        private void Process(Stream strOut)
        {
            int numLoops = (this.str.ReadByte() << 8) + this.str.ReadByte();
            this.nextSamples.Clear();
            int index;

            while (true)
            {
                if (this.nextSamples.Count == 0)
                {
                    if (numLoops == 0)
                    {
                        return;
                    }

                    numLoops--;
                    index = this.str.ReadByte();
                }
                else
                {
                    index = this.nextSamples.Pop();
                }

                if (this.buffer2[index] == index)
                {
                    strOut.WriteByte((byte)index);
                }
                else
                {
                    this.nextSamples.Push(this.buffer1[index]);
                    this.nextSamples.Push(this.buffer2[index]);
                    index = this.nextSamples.Count;
                }
            }
        }
    }
}
