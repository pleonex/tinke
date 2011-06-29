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
 * Fecha: 29/06/2011
 * 
 */
using System;
using System.IO;
using System.Text;

namespace SDAT
{
    public static class SWAV
    {
        public static void EscribirArchivo(ArchivoWAV wr, string path)
        {
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(path, System.IO.FileMode.Create);
                bw = new BinaryWriter(fs);

                bw.Write(System.Text.Encoding.ASCII.GetBytes(wr.chunkID));
                bw.Write(wr.chunkSize);
                bw.Write(Encoding.ASCII.GetBytes(wr.format));
                bw.Write(Encoding.ASCII.GetBytes(wr.wave.fmt.chunkID));
                bw.Write(wr.wave.fmt.chunkSize);
                bw.Write(Convert.ToUInt16(wr.wave.fmt.audioFormat));
                bw.Write(wr.wave.fmt.numChannels);
                bw.Write(wr.wave.fmt.sampleRate);
                bw.Write(wr.wave.fmt.byteRate);
                bw.Write(wr.wave.fmt.blockAlign);
                bw.Write(wr.wave.fmt.bitsPerSample);
                bw.Write(Encoding.ASCII.GetBytes(wr.wave.data.chunkID));
                bw.Write(wr.wave.data.chunkSize);
                for (int i = 0; i < wr.wave.data.data.Length; i++)
                {
                    bw.Write(wr.wave.data.data[i]);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null) fs.Close();
                if (bw != null) bw.Close();
            }

        }

        public static ArchivoWAV GenerarWAVPCM(ushort numChannels, uint sampleRate, ushort bitsPerSample, byte[] data)
        {

            ArchivoWAV archivo = new ArchivoWAV();

            archivo.chunkID = new char[] { 'R', 'I', 'F', 'F' };
            archivo.format = new char[] { 'W', 'A', 'V', 'E' };

            archivo.wave.fmt.chunkID = new char[] { 'f', 'm', 't', ' ' };
            archivo.wave.fmt.chunkSize = 16;
            archivo.wave.fmt.audioFormat = ArchivoWAV.WaveChunk.FmtChunk.WaveFormat.WAVE_FORMAT_PCM;
            archivo.wave.fmt.numChannels = numChannels;
            archivo.wave.fmt.sampleRate = sampleRate;
            archivo.wave.fmt.bitsPerSample = bitsPerSample;
            archivo.wave.fmt.byteRate = archivo.wave.fmt.sampleRate * archivo.wave.fmt.bitsPerSample * archivo.wave.fmt.numChannels / 8;
            archivo.wave.fmt.blockAlign = (ushort)(archivo.wave.fmt.numChannels * archivo.wave.fmt.bitsPerSample / (ushort)(8));

            archivo.wave.data.chunkID = new char[] { 'd', 'a', 't', 'a' };
            archivo.wave.data.chunkSize = (uint)data.Length;
            archivo.wave.data.data = new sbyte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                    archivo.wave.data.data[i] = unchecked ((sbyte)(data[i]^0x80));
            }



            archivo.chunkSize = 4 + (8 + archivo.wave.fmt.chunkSize) + (8 + archivo.wave.data.chunkSize);

            return archivo;
        }

        public static ArchivoSWAV LeerSWAV(string path)
        {
            /***Lectura del archivo SWAV***/
            FileStream fs = null;
            BinaryReader br = null;

            ArchivoSWAV swav = new ArchivoSWAV();

            try
            {
                fs = new FileStream(path, FileMode.Open);
                br = new BinaryReader(fs);

                //Leer Header
                swav.header.type = ConvertirBytesAChars(br.ReadBytes(4));
                swav.header.magic = br.ReadUInt32();
                swav.header.nFileSize = br.ReadUInt32();
                swav.header.nSize = br.ReadUInt16();
                swav.header.nBlock = br.ReadUInt16();

                //Leer Data
                swav.data.type = ConvertirBytesAChars(br.ReadBytes(4));
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
                uint tamañoDatos = (uint)(swav.data.nSize - (uint)20);
                swav.data.data = new byte[tamañoDatos];
                for (uint i = 0; i < tamañoDatos; i++)
                {
                    swav.data.data[i] = br.ReadByte();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null) fs.Close();
                if (br != null) br.Close();
            }

            return swav;
        }

        public static ArchivoWAV ConvertirSWAVaWAV(ArchivoSWAV swav)
        {
            ArchivoWAV wav = new ArchivoWAV();

            if (swav.data.info.nWaveType == 0)
            {
                wav = GenerarWAVPCM(1, swav.data.info.nSampleRate, 8, swav.data.data);
            }

            if (swav.data.info.nWaveType == 1)
            {
                wav = GenerarWAVPCM(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }

            if (swav.data.info.nWaveType >= 2)
            {
                throw new NotSupportedException("Only PCM8 and PCM16 formats are supported");
            }
            return wav;
        }

        public static ArchivoSWAV[] ConvertirSWARaSWAV(ArchivoSWAR swar)
        {
            return new ArchivoSWAV[1];
        }

        static Char[] ConvertirBytesAChars(Byte[] bytes)
        {
            Char[] ch = new char[bytes.Length];

            for (int i = 0; i < bytes.Length; ++i)
            {
                ch[i] = Convert.ToChar(bytes[i]);
            }

            return ch;
        }

        public struct ArchivoWAV
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

                    public enum WaveFormat : ushort
                    {
                        WAVE_FORMAT_PCM = 0x0001,
                        IBM_FORMAT_ADPCM = 0x0002,
                        IBM_FORMAT_MULAW = 0x0007,
                        IBM_FORMAT_ALAW = 0x0006,
                        WAVE_FORMAT_EXTENSIBLE = 0xFFFE
                    }
                }
                public struct DataChunk
                {
                    public char[] chunkID;
                    public uint chunkSize;
                    public sbyte[] data;
                }
            }
        }
        public struct ArchivoSWAV
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
        public struct ArchivoSWAR
        {
            public struct Header
            {
                char[] type;   // 'SWAR'
                uint magic;	// 0x0100feff
                uint nFileSize;	// Size of this SWAR file
                ushort nSize;	// Size of this structure = 16
                ushort nBlock;	// Number of Blocks = 1
            }
            public struct Data
            {
                char[] type;		// 'DATA'
                uint nSize;		// Size of this structure
                uint[] reserved;	// 8 reserved 0s, for use in runtime
                uint nSample;		// Number of Samples 
            }

            uint[] nOffset;	// array of offsets of samples
        }
    }
}
