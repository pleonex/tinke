/*
 * Copyright (C) 2011  rafael1193
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
 * Programador: rafael1193
 * 
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace SDAT
{
    /// <summary>
    /// Operations with SWAR files
    /// </summary>
    class SWAR
    {
        /// <summary>
        /// Read a SWAR file and return a SWAR structure
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns>Structure of the file</returns>
        public static sSWAR Read(string path)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;

            sSWAR swar = new sSWAR();

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                br = new System.IO.BinaryReader(fs);

                // Common header
                swar.header.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                swar.header.magic = br.ReadUInt32();
                swar.header.nFileSize = br.ReadUInt32();
                swar.header.nSize = br.ReadUInt16();
                swar.header.nBlock = br.ReadUInt16();

                // DATA section
                swar.data.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                swar.data.nSize = br.ReadUInt32();
                swar.data.reserved = new uint[8];
                for (int i = 0; i < 8; i++) { swar.data.reserved[i] = br.ReadUInt32(); }
                swar.data.nSample = br.ReadUInt32();

                swar.data.nOffset = new uint[swar.data.nSample];
                for (int i = 0; i < swar.data.nSample; i++) { swar.data.nOffset[i] = br.ReadUInt32(); }

                swar.data.samples = new sSWAR.Data.Sample[swar.data.nSample];

                for (uint i = 0; i < swar.data.nSample; i++)
                {
                    // INFO structure
                    swar.data.samples[i].info.nWaveType = br.ReadByte();
                    swar.data.samples[i].info.bLoop = br.ReadByte();
                    swar.data.samples[i].info.nSampleRate = br.ReadUInt16();
                    swar.data.samples[i].info.nTime = br.ReadUInt16();
                    swar.data.samples[i].info.nLoopOffset = br.ReadUInt16();
                    swar.data.samples[i].info.nNonLoopLen = br.ReadUInt32();

                    // Calculation of data size
                    if (i < swar.data.nOffset.Length - 1)
                    {
                        swar.data.samples[i].data = new byte[swar.data.nOffset[i + 1] - swar.data.nOffset[i] - /*SWAVInfo size ->*/12];
                    }
                    else
                    {
                        swar.data.samples[i].data = new byte[br.BaseStream.Length - swar.data.nOffset[i] - /*SWAVInfo size ->*/12];
                    }

                    // Read DATA
                    for (uint j = 0; j < swar.data.samples[i].data.Length; j++)
                    {
                        swar.data.samples[i].data[j] = br.ReadByte();
                    }

                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (fs != null) fs.Close();
                if (br != null) br.Close();
            }

            return swar;
        }
        public static void Write(sSWAV[] sounds, string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            uint file_size = (uint)(0x10 + 0x10 + 0x04 * sounds.Length);
            for (int i = 0; i < sounds.Length; i++)
                file_size += (uint)sounds[i].data.data.Length + 0x0A;

            bw.Write(new char[] { 'S', 'W', 'A', 'R' });
            bw.Write((uint)0x0100FEFF);
            bw.Write(file_size);
            bw.Write((ushort)0x10);
            bw.Write((ushort)0x01);

            bw.Write(new char[] { 'D', 'A', 'T', 'A' });
            bw.Write((uint)(file_size - 0x10));
            for (int i = 0; i < 8; i++)
                bw.Write((uint)0x00);
            bw.Write((uint)sounds.Length);

            // Write offset
            uint currOffset = (uint)(0x3C + 0x04 * sounds.Length);
            for (int i = 0; i < sounds.Length; i++)
            {
                bw.Write(currOffset);
                currOffset += (uint)sounds[i].data.data.Length + 0x0A;
            }

            // Write data
            for (int i = 0; i < sounds.Length; i++)
            {
                bw.Write(sounds[i].data.info.nWaveType);
                bw.Write(sounds[i].data.info.bLoop);
                bw.Write(sounds[i].data.info.nSampleRate);
                bw.Write(sounds[i].data.info.nTime);
                bw.Write(sounds[i].data.info.nLoopOffset);
                bw.Write(sounds[i].data.info.nNonLoopLen);
                bw.Write(sounds[i].data.data);
            }

            bw.Flush();
            bw.Close();
        }

        public static Dictionary<String, String> Information(sSWAR swar, string lang)
        {
            Dictionary<String, String> info = new Dictionary<string, string>();

            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(System.Windows.Forms.Application.StartupPath +
                    Path.DirectorySeparatorChar + "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
                xml = xml.Element(lang).Element("Information");

                info.Add(xml.Element("S00").Value, xml.Element("S01").Value);
                info.Add(xml.Element("S02").Value, new String(swar.header.type));
                info.Add(xml.Element("S03").Value, "0x" + swar.header.magic.ToString("x"));
                info.Add(xml.Element("S04").Value, swar.header.nFileSize.ToString());

                info.Add(xml.Element("S05").Value, new String(swar.data.type));
                info.Add(xml.Element("S06").Value, swar.data.nSize.ToString());
                info.Add(xml.Element("S16").Value, swar.data.nSample.ToString());
            }
            catch { throw new Exception("There was an error reading the language file"); }

            return info;
        }

        /// <summary>
        /// Decompress the SWAR file in SWAV files.
        /// </summary>
        /// <param name="swar">SWAR structure to decompress</param>
        /// <returns>All the SWAV that are in it</returns>
        public static sSWAV[] ConvertToSWAV(sSWAR swar)
        {
            sSWAV[] swav = new sSWAV[swar.data.samples.Length];

            for (int i = 0; i < swav.Length; i++)
            {
                swav[i] = new sSWAV();

                swav[i].data.data = swar.data.samples[i].data;
                swav[i].data.info = swar.data.samples[i].info;
                swav[i].data.type = new char[] { 'D', 'A', 'T', 'A' };
                swav[i].data.nSize = (uint)swav[i].data.data.Length + 1 * sizeof(uint) + 4 * sizeof(byte) + (2 * sizeof(byte) + 3 * sizeof(ushort) + sizeof(uint));

                swav[i].header.type = new char[] { 'S', 'W', 'A', 'V' };
                swav[i].header.magic = 0x0100feff;
                swav[i].header.nSize = 16;
                swav[i].header.nBlock = 1;
                swav[i].header.nFileSize = 16 + ((uint)swav[i].data.data.Length + 1 * sizeof(uint) + 4 * sizeof(byte) + (2 * sizeof(byte) + 3 * sizeof(ushort) + sizeof(uint)));
            }

            return swav;
        }
    }

    /// <summary>
    /// Structure of a SWAR file
    /// </summary>
    public struct sSWAR
    {
        public Header header;
        public Data data;
        public struct Header
        {
            public char[] type;   // 'SWAR'
            public uint magic;	// 0x0100feff
            public uint nFileSize;	// Size of this SWAR file
            public ushort nSize;	// Size of this structure = 16
            public ushort nBlock;	// Number of Blocks = 1
        }
        public struct Data
        {
            public char[] type;		// 'DATA'
            public uint nSize;		// Size of this structure
            public uint[] reserved;	// 8 reserved 0s, for use in runtime
            public uint nSample;		// Number of Samples
            public uint[] nOffset;	// array of offsets of samples
            public Sample[] samples;

            public struct Sample
            {
                public sSWAV.Data.SWAVInfo info;
                public byte[] data;
            }
        }
    }
}
