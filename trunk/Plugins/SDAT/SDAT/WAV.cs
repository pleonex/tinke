/*
 * Copyright (C) 2011
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
 * Programador: rafael1193, pleoNeX
 * 
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SDAT
{
    /// <summary>
    /// Static class with WAV files operations
    /// </summary>
    public static class WAV
    {
        /// <summary>
        /// Read a wave file and return its structure.
        /// </summary>
        /// <param name="filein">File to read</param>
        /// <returns>Structure of the wave file</returns>
        public static sWAV Read(string filein)
        {
            sWAV wav = new sWAV();
            BinaryReader br = new BinaryReader(File.OpenRead(filein));

            // RIFF header
            wav.chunkID = br.ReadChars(4);
            wav.chunkSize = br.ReadUInt32();
            wav.format = br.ReadChars(4);
            if (new String(wav.chunkID) != "RIFF" || new String(wav.format) != "WAVE")
                throw new NotSupportedException();

            // fmt sub-chunk
            wav.wave.fmt.chunkID = br.ReadChars(4);
            wav.wave.fmt.chunkSize = br.ReadUInt32();
            wav.wave.fmt.audioFormat = (WaveFormat)br.ReadUInt16();
            wav.wave.fmt.numChannels = br.ReadUInt16();
            wav.wave.fmt.sampleRate = br.ReadUInt32();
            wav.wave.fmt.byteRate = br.ReadUInt32();
            wav.wave.fmt.blockAlign = br.ReadUInt16();
            wav.wave.fmt.bitsPerSample = br.ReadUInt16();
            br.BaseStream.Position = 0x14 + wav.wave.fmt.chunkSize;
            String dataID = new String(br.ReadChars(4));
            while (dataID != "data")
            {
                br.BaseStream.Position += br.ReadUInt32() + 0x04;
                dataID = new String(br.ReadChars(4));
            }
            // data sub-chunk
            br.BaseStream.Position -= 4;
            wav.wave.data.chunkID = br.ReadChars(4);
            wav.wave.data.chunkSize = br.ReadUInt32();
            wav.wave.data.data = br.ReadBytes((int)wav.wave.data.chunkSize - 0x08);
            br.Close();

            // Convert the data to PCM16
            if (wav.wave.fmt.audioFormat != WaveFormat.WAVE_FORMAT_PCM)
                throw new NotSupportedException();

            if (wav.wave.fmt.audioFormat == WaveFormat.WAVE_FORMAT_PCM && wav.wave.fmt.bitsPerSample == 0x08) // PCM8
            {
                wav.wave.fmt.bitsPerSample = 0x10;
                wav.wave.fmt.blockAlign = (ushort)(wav.wave.fmt.numChannels * wav.wave.fmt.bitsPerSample / (ushort)(8));
                wav.wave.fmt.byteRate = wav.wave.fmt.sampleRate * wav.wave.fmt.bitsPerSample * wav.wave.fmt.numChannels / 8;
                wav.wave.data.data = PCM.PCM8UnsignedToPCM16(wav.wave.data.data);
            }

            return wav;
        }
        /// <summary>
        /// Write a WAV structure to a WAV file
        /// </summary>
        /// <param name="wav">WAV structu to write</param>
        /// <param name="fileout">File where the structure will be written</param>
        public static void Write(sWAV wav, string fileout)
        {
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(fileout, System.IO.FileMode.Create);
                bw = new BinaryWriter(fs);

                bw.Write(Encoding.ASCII.GetBytes(wav.chunkID));
                bw.Write(wav.chunkSize);
                bw.Write(Encoding.ASCII.GetBytes(wav.format));
                bw.Write(Encoding.ASCII.GetBytes(wav.wave.fmt.chunkID));
                bw.Write(wav.wave.fmt.chunkSize);
                bw.Write(Convert.ToUInt16(wav.wave.fmt.audioFormat));
                bw.Write(wav.wave.fmt.numChannels);
                bw.Write(wav.wave.fmt.sampleRate);
                bw.Write(wav.wave.fmt.byteRate);
                bw.Write(wav.wave.fmt.blockAlign);
                bw.Write(wav.wave.fmt.bitsPerSample);
                bw.Write(Encoding.ASCII.GetBytes(wav.wave.data.chunkID));
                bw.Write(wav.wave.data.chunkSize);
                bw.Write(wav.wave.data.data);

                bw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (fs != null) fs.Close();
                if (bw != null) bw.Close();
            }

        }

        /// <summary>
        /// Create a WAV structure using PCM audio format
        /// </summary>
        /// <param name="numChannels">Number of channels</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <param name="bitsPerSample">Bits per sample</param>
        /// <param name="data">Audio data</param>
        /// <returns>New wav structure</returns>
        public static sWAV Create_WAV(ushort numChannels, uint sampleRate, ushort bitsPerSample, byte[] data)
        {
            sWAV wav = new sWAV();

            wav.chunkID = new char[] { 'R', 'I', 'F', 'F' };
            wav.format = new char[] { 'W', 'A', 'V', 'E' };

            wav.wave.fmt.chunkID = new char[] { 'f', 'm', 't', '\x20' };
            wav.wave.fmt.chunkSize = 16;
            wav.wave.fmt.audioFormat = WaveFormat.WAVE_FORMAT_PCM;
            wav.wave.fmt.numChannels = numChannels;
            wav.wave.fmt.sampleRate = sampleRate;
            wav.wave.fmt.bitsPerSample = bitsPerSample;
            wav.wave.fmt.byteRate = wav.wave.fmt.sampleRate * wav.wave.fmt.bitsPerSample * wav.wave.fmt.numChannels / 8;
            wav.wave.fmt.blockAlign = (ushort)(wav.wave.fmt.numChannels * wav.wave.fmt.bitsPerSample / (ushort)(8));

            wav.wave.data.chunkID = new char[] { 'd', 'a', 't', 'a' };
            wav.wave.data.chunkSize = (uint)data.Length;
            wav.wave.data.data = new byte[data.Length];
            wav.wave.data.data = data;

            wav.chunkSize = (uint)(0x24 + data.Length);

            return wav;
        }
        public static Byte[] ConvertToMono(byte[] data, int numChannels, int bitsPerSample)
        {
            List<Byte> mono = new List<byte>();
            int sample_size = numChannels * bitsPerSample / 8;

            for (int i = 0; i < data.Length; i += bitsPerSample / 8)
            {
                int rChannel, lChannel;

                switch (bitsPerSample)
                {
                    case 8:
                        rChannel = data[i];
                        i += 1;
                        lChannel = data[i];
                        mono.Add((byte)((sbyte)rChannel + (sbyte)lChannel));
                        break;
                    case 16:
                        rChannel = BitConverter.ToInt16(data, i);
                        i += 2;
                        lChannel = BitConverter.ToInt16(data, i);
                        mono.AddRange(BitConverter.GetBytes((short)((short)rChannel + (short)lChannel)));
                        break;
                }
            }

            return mono.ToArray();
        }
    }

    public struct sWAV
    {
        public char[] chunkID;
        public uint chunkSize;
        public char[] format;
        public WaveChunk wave;

        public struct WaveChunk
        {
            public FmtChunk fmt;
            public DataChunk data;

            public struct FmtChunk
            {
                public char[] chunkID;
                public uint chunkSize;
                public WaveFormat audioFormat;
                public ushort numChannels;
                public uint sampleRate;
                public uint byteRate;
                public ushort blockAlign;
                public ushort bitsPerSample;
            }
            public struct DataChunk
            {
                public char[] chunkID;
                public uint chunkSize;
                public byte[] data;
            }
        }
    }
    public enum WaveFormat : ushort
    {
        WAVE_FORMAT_PCM = 0x0001,
        IBM_FORMAT_ADPCM = 0x0002,
        IBM_FORMAT_MULAW = 0x0007,
        IBM_FORMAT_ALAW = 0x0006,
        WAVE_FORMAT_EXTENSIBLE = 0xFFFE
    }

}
