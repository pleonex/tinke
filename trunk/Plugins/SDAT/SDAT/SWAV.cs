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

namespace SDAT
{
    class SWAV
    {
        public static sSWAV Read(string path)
        {
            /***Lectura del archivo SWAV***/
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;

            sSWAV swav = new sSWAV();

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                br = new System.IO.BinaryReader(fs);

                //Leer Header
                swav.header.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                swav.header.magic = br.ReadUInt32();
                swav.header.nFileSize = br.ReadUInt32();
                swav.header.nSize = br.ReadUInt16();
                swav.header.nBlock = br.ReadUInt16();

                //Leer Data
                swav.data.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                swav.data.nSize = br.ReadUInt32();
                {//Leer Info
                    swav.data.info.nWaveType = br.ReadByte();
                    swav.data.info.bLoop = br.ReadByte();
                    swav.data.info.nSampleRate = br.ReadUInt16();
                    swav.data.info.nTime = br.ReadUInt16();
                    swav.data.info.nLoopOffset = br.ReadUInt16();
                    swav.data.info.nNonLoopLen = br.ReadUInt32();
                }
                //Leer resto de Data
                uint tamañoDatos = (uint)(br.BaseStream.Length - br.BaseStream.Position);
                swav.data.data = new byte[tamañoDatos];
                for (uint i = 0; i < tamañoDatos; i++)
                {
                    swav.data.data[i] = br.ReadByte();

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

            return swav;
        }
        public static void Write(sSWAV swav, string path)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryWriter bw = null;

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                bw = new System.IO.BinaryWriter(fs);

                //Escribir Header
                bw.Write(Encoding.ASCII.GetBytes(swav.header.type));
                bw.Write(swav.header.magic);
                bw.Write(swav.header.nFileSize);
                bw.Write(swav.header.nSize);
                bw.Write(swav.header.nBlock);

                //Escribir Data
                bw.Write(Encoding.ASCII.GetBytes(swav.data.type));
                bw.Write(swav.data.nSize);

                //Escribir Info
                bw.Write(swav.data.info.nWaveType);
                bw.Write(swav.data.info.bLoop);
                bw.Write(swav.data.info.nSampleRate);
                bw.Write(swav.data.info.nTime);
                bw.Write(swav.data.info.nLoopOffset);
                bw.Write(swav.data.info.nNonLoopLen);

                //Escribir data
                bw.Write(swav.data.data);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (fs != null) fs.Close();
                if (bw != null) bw.Close();
            }
        }

        public static Dictionary<String, String> Information(sSWAV swav)
        {
            Dictionary<String, String> info = new Dictionary<string, string>();

            info.Add("/bSection", "Standar header");
            info.Add("Type", new String(swav.header.type));
            info.Add("Magic", "0x" + swav.header.magic.ToString("x"));
            info.Add("File size", swav.header.nFileSize.ToString());

            info.Add("/bSection 1", "SWAV Info");
            info.Add("Wave type", swav.data.info.nWaveType.ToString());
            info.Add("Loop flag", swav.data.info.bLoop.ToString());
            info.Add("Loop offset", "0x" + swav.data.info.nLoopOffset.ToString("x"));
            info.Add("Non loop length", "0x" + swav.data.info.nNonLoopLen.ToString("x"));
            info.Add("Sample rate", swav.data.info.nSampleRate.ToString());
            info.Add("Time", swav.data.info.nTime.ToString());

            info.Add("/bSection 2", new String(swav.data.type));
            info.Add("Size", swav.data.nSize.ToString());

            return info;
        }

        public static sWAV ConvertToWAV(sSWAV swav, bool loop)
        {
            sWAV wav = new sWAV();

            if (swav.data.info.nWaveType == 0) // 8 Bits per sample, PCM-8
            {
                swav.data.data = PCM8(swav.data.data);
                if (loop)
                {
                    Byte[] data = new Byte[(int)swav.data.info.nNonLoopLen];
                    Array.Copy(swav.data.data, swav.data.info.nLoopOffset, data, 0, data.Length);
                    swav.data.data = data;
                }

                wav = WAV.Create_WAV(1, swav.data.info.nSampleRate, 8, swav.data.data);
            }
            else if (swav.data.info.nWaveType == 1) // 16 Bits per sample, PCM-16
            {
                if (loop)
                {
                    Byte[] data = new Byte[(int)swav.data.info.nNonLoopLen * 2];
                    Array.Copy(swav.data.data, swav.data.info.nLoopOffset * 2, data, 0, data.Length);
                    swav.data.data = data;
                }

                wav = WAV.Create_WAV(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }
            else if (swav.data.info.nWaveType >= 2) // 16 Bits per sample, IMA-ADPCM
            {
                swav.data.data = Compression_ADPCM.Decompress_ADPCM(swav.data.data);
                if (loop)
                {
                    Byte[] data = new Byte[swav.data.data.Length - ((int)swav.data.info.nLoopOffset * 2)];
                    Array.Copy(swav.data.data, swav.data.info.nLoopOffset * 2, data, 0, data.Length);
                    swav.data.data = data;
                }

                wav = WAV.Create_WAV(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }
            return wav;
        }
        public static sSWAV ConvertToSWAV(sWAV wav, int waveType = 1)
        {
            if (wav.wave.fmt.audioFormat != WaveFormat.WAVE_FORMAT_PCM)
                throw new NotSupportedException();

            sSWAV swav = new sSWAV();
            swav.header.type = "SWAV".ToCharArray();
            swav.header.magic = 0x0100FEFF;
            swav.header.nSize = 0x10;
            swav.header.nBlock = 0x01;

            swav.data.type = "DATA".ToCharArray();
            swav.data.info.nWaveType = (byte)waveType;
            swav.data.info.bLoop = 1;
            if (wav.wave.fmt.bitsPerSample == 0x10)
                swav.data.info.nSampleRate = (ushort)wav.wave.fmt.sampleRate;
            else if (wav.wave.fmt.bitsPerSample == 0x08)
                swav.data.info.nSampleRate = (ushort)(wav.wave.fmt.sampleRate / 2);
            swav.data.info.nTime = (ushort)(1.0 / swav.data.info.nSampleRate * 1.6756991e+7 / 32);
            swav.data.info.nLoopOffset = 0x01;

            if (wav.wave.fmt.numChannels > 1)
                wav.wave.data.data = WAV.ConvertToMono(wav.wave.data.data, wav.wave.fmt.numChannels, wav.wave.fmt.bitsPerSample);

            if (wav.wave.fmt.bitsPerSample == 0x10)
                swav.data.data = wav.wave.data.data;
            else if (wav.wave.fmt.bitsPerSample == 0x08)
                swav.data.data = PCM8(wav.wave.data.data);

            swav.data.nSize = (uint)swav.data.data.Length + 0x0A;
            swav.data.info.nNonLoopLen = (uint)swav.data.data.Length;
            swav.header.nFileSize = swav.data.nSize + swav.header.nSize;

            return swav;
        }

        static byte[] PCM8(byte[] data)
        {
            byte[] resul = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                resul[i] = unchecked((byte)(data[i] ^ 0x80));
            }

            return resul;
        }
    }

    public struct sSWAV
    {
        public Header header;
        public Data data;

        public struct Header
        {
            public char[] type;   // 'SWAV'
            public uint magic;	// 0x0100feff
            public uint nFileSize;	// Size of this SWAV file
            public ushort nSize;	// Size of this structure = 16
            public ushort nBlock;	// Number of Blocks = 1
        }
        public struct Data
        {
            public char[] type;	// 'DATA'
            public uint nSize;	// Size of this structure
            public SWAVInfo info;	// info about the sample
            public byte[] data;	// array of binary data

            // info about the sample
            public struct SWAVInfo
            {
                public byte nWaveType;		// 0 = PCM8, 1 = PCM16, 2 = (IMA-)ADPCM
                public byte bLoop;		// Loop flag = TRUE|FALSE
                public ushort nSampleRate;	// Sampling Rate
                public ushort nTime;		// (ARM7_CLOCK / nSampleRate) [ARM7_CLOCK: 33.513982MHz / 2 = 1.6756991 E +7]
                public ushort nLoopOffset;	// Loop Offset (expressed in words (32-bits))
                public uint nNonLoopLen;	// Non Loop Length (expressed in words (32-bits))
            }
        }
    }
}
