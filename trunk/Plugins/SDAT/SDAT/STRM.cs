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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SDAT
{
    /// <summary>
    /// Operations with STRM files
    /// </summary>
    public static class STRM
    {
        /// <summary>
        /// Read a STRM file and return the structure
        /// </summary>
        /// <param name="path">File to read</param>
        /// <returns>Structure from the file</returns>
        public static sSTRM Read(string path)
        {
            sSTRM strm = new sSTRM();
            BinaryReader br = new BinaryReader(File.OpenRead(path));

            // Common header
            strm.cabecera.id = br.ReadChars(4);
            strm.cabecera.constant = br.ReadUInt32();
            strm.cabecera.fileSize = br.ReadUInt32();
            strm.cabecera.headerSize = br.ReadUInt16();
            strm.cabecera.nBlocks = br.ReadUInt16();
            // HEAD section
            strm.head.id = br.ReadChars(4);
            strm.head.size = br.ReadUInt32();
            strm.head.waveType = br.ReadByte();
            strm.head.loop = br.ReadByte();
            strm.head.channels = br.ReadUInt16();
            strm.head.sampleRate = br.ReadUInt16();
            strm.head.time = br.ReadUInt16();
            strm.head.loopOffset = br.ReadUInt32();
            strm.head.nSamples = br.ReadUInt32();
            strm.head.dataOffset = br.ReadUInt32();
            strm.head.nBlocks = br.ReadUInt32();
            strm.head.blockLen = br.ReadUInt32();
            strm.head.blockSample = br.ReadUInt32();
            strm.head.lastBlocklen = br.ReadUInt32();
            strm.head.lastBlockSample = br.ReadUInt32();
            br.ReadBytes(32); // Reserved
            // DATA section
            strm.data.id = br.ReadChars(4);
            strm.data.size = br.ReadUInt32();
            strm.data.data = br.ReadBytes((int)strm.data.size - 0x08);

            br.Close();

            return strm;
        }
        /// <summary>
        /// Write a STRM structure to a file
        /// </summary>
        /// <param name="strm">STRM structure to write</param>
        /// <param name="path">File to write</param>
        public static void Write(sSTRM strm, string path)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryWriter bw = null;

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                bw = new System.IO.BinaryWriter(fs);

                // Common header
                bw.Write(Encoding.ASCII.GetBytes(strm.cabecera.id));
                bw.Write(strm.cabecera.constant);
                bw.Write(strm.cabecera.fileSize);
                bw.Write(strm.cabecera.headerSize);
                bw.Write(strm.cabecera.nBlocks);

                // HEAD section
                bw.Write(Encoding.ASCII.GetBytes(strm.head.id));
                bw.Write(strm.head.size);
                bw.Write(strm.head.waveType);
                bw.Write(strm.head.loop);
                bw.Write(strm.head.channels);
                bw.Write(strm.head.sampleRate);
                bw.Write(strm.head.time);
                bw.Write(strm.head.loopOffset);
                bw.Write(strm.head.nSamples);
                bw.Write(strm.head.dataOffset);
                bw.Write(strm.head.nBlocks);
                bw.Write(strm.head.blockLen);
                bw.Write(strm.head.blockSample);
                bw.Write(strm.head.lastBlocklen);
                bw.Write(strm.head.lastBlockSample);
                bw.Write(strm.head.reserved);

                // DATA section
                bw.Write(Encoding.ASCII.GetBytes(strm.data.id));
                bw.Write(strm.data.size);
                bw.Write(strm.data.data);

                bw.Flush();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (bw != null) bw.Close();
                if (fs != null) fs.Close();
            }
        }
        /// <summary>
        /// Get the information of the structure in an string array
        /// </summary>
        /// <param name="strm">Structure to show</param>
        /// <returns>Information</returns>
        public static Dictionary<String, String> Information(sSTRM strm)
        {
            Dictionary<String, String> info = new Dictionary<string, string>();
            info.Add("/bSection", "Standar header");
            info.Add("Type", new String(strm.cabecera.id));
            info.Add("Magic", strm.cabecera.constant.ToString());
            info.Add("File size", strm.cabecera.fileSize.ToString());

            info.Add("/bSection 1", new String(strm.head.id));
            info.Add("Size 1", strm.head.size.ToString());
            info.Add("Wave type", strm.head.waveType.ToString());
            info.Add("Loop flag", strm.head.loop.ToString());
            info.Add("Loop offset", "0x" + strm.head.loopOffset.ToString("x"));
            info.Add("Channels", strm.head.channels.ToString());
            info.Add("Sample rate", strm.head.sampleRate.ToString());
            info.Add("Time", strm.head.time.ToString());
            info.Add("Samples", strm.head.nSamples.ToString());
            info.Add("Data offset", "0x" + strm.head.dataOffset.ToString("x"));
            info.Add("Number of blocks", strm.head.nBlocks.ToString());
            info.Add("Block length", strm.head.blockLen.ToString());
            info.Add("Samples per block", strm.head.blockSample.ToString());
            info.Add("Last block length", strm.head.lastBlocklen.ToString());
            info.Add("Samples per last block", strm.head.lastBlockSample.ToString());
            //info.Add("Reserved", BitConverter.ToString(strm.head.reserved));

            info.Add("/bSection 2", new string(strm.data.id));
            info.Add("Size 2", strm.data.size.ToString());

            return info;
        }

        /// <summary>
        /// Convert a STRM structure to a WAV structure
        /// </summary>
        /// <param name="strm">STRM structure to convert</param>
        /// <param name="loop">If true, the new WAV data will start in the loop offset</param>
        /// <returns>WAV structure converted</returns>
        public static sWAV ConvertToWAV(sSTRM strm, bool loop)
        {
            sWAV wav = new sWAV();

            // Get the audio data
            if (strm.head.channels == 2)
            {
                // Get both channels and convert it to PCM-16
                strm.data.leftChannel = DivideChannels(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, true, strm.head.waveType);
                strm.data.rightChannel = DivideChannels(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, false, strm.head.waveType);
                Array.Clear(strm.data.data, 0, strm.data.data.Length);

                if (loop && strm.head.waveType == 0) // 8 bits per sample
                    strm.data.data = MergeChannels(strm.data.leftChannel, strm.data.rightChannel, (int)strm.head.loopOffset);
                else if (loop) // 16 bits per sample
                    strm.data.data = MergeChannels(strm.data.leftChannel, strm.data.rightChannel, (int)strm.head.loopOffset * 2);
                else // No loop
                    strm.data.data = MergeChannels(strm.data.leftChannel, strm.data.rightChannel);
            }
            else if (strm.head.channels == 1)
            {
                // Get the channel and convert it to PCM-16
                strm.data.data = MonoChannel(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, strm.head.waveType);

                if (strm.head.waveType == 0 && loop) // 8 bits per sample
                {
                    Byte[] data = new Byte[strm.data.data.Length - (int)strm.head.loopOffset];
                    Array.Copy(strm.data.data, strm.head.loopOffset, data, 0, data.Length);
                    strm.data.data = data;
                }
                else if (loop) // 16 bits per sample
                {
                    Byte[] data = new Byte[strm.data.data.Length - ((int)strm.head.loopOffset * 2)];
                    Array.Copy(strm.data.data, strm.head.loopOffset * 2, data, 0, data.Length);
                    strm.data.data = data;
                }
            }

            // Create the WAV structure from the STRM data
            wav = WAV.Create_WAV(strm.head.channels, strm.head.sampleRate, (ushort)(strm.head.waveType == 0 ? 8 : 16),
                                strm.data.data);

            return wav;
        }
        /// <summary>
        /// Convert a WAV structure to a STRM structure
        /// </summary>
        /// <param name="wav">WAV structure to convert</param>
        /// <returns>STRM structure converted</returns>
        public static sSTRM ConvertToSTRM(sWAV wav, int blockSize = 0x200, int waveType = 1)
        {
            if (wav.wave.fmt.audioFormat != WaveFormat.WAVE_FORMAT_PCM)
                throw new NotSupportedException();

            sSTRM strm = new sSTRM();
            strm.cabecera.id = "STRM".ToArray();
            strm.cabecera.constant = 0x0100FEFF;
            strm.cabecera.headerSize = 0x10;
            strm.cabecera.nBlocks = 0x02;

            strm.head.id = "HEAD".ToArray();
            strm.head.size = 0x50;
            strm.head.waveType = (byte)waveType;
            strm.head.loop = 1;
            strm.head.channels = wav.wave.fmt.numChannels;
            strm.head.sampleRate = (ushort)wav.wave.fmt.sampleRate;
            strm.head.time = (ushort)(1.0 / strm.head.sampleRate * 1.6756991e+7 / 32);
            strm.head.loopOffset = 0x00;
            strm.head.dataOffset = 0x68;
            strm.head.reserved = new Byte[32];

            strm.data.id = "DATA".ToArray();
            if (wav.wave.fmt.numChannels == 2)
            {
                strm.data.leftChannel = DivideChannels(wav.wave.data.data, true, 0);
                strm.data.rightChannel = DivideChannels(wav.wave.data.data, false, 0);
                byte[][] leftBlock = CreateBlocks(strm.data.leftChannel, blockSize);
                byte[][] rigthBlock = CreateBlocks(strm.data.rightChannel, blockSize);
                strm.data.data = MergeBlocks(leftBlock, rigthBlock);

                strm.head.blockLen = (uint)leftBlock[0].Length;
                strm.head.blockSample = strm.head.blockLen / 2;
                strm.head.lastBlocklen = (uint)leftBlock[leftBlock.Length - 1].Length;
                strm.head.lastBlockSample = strm.head.lastBlocklen / 2;
                strm.head.nBlocks = (uint)leftBlock.Length;
                strm.head.nSamples = strm.head.nBlocks * 0x100 + strm.head.lastBlockSample;
            }
            else
            {
                byte[][] blocks = CreateBlocks(wav.wave.data.data, blockSize);
                strm.head.blockLen = (uint)blocks[0].Length;
                strm.head.blockSample = strm.head.blockLen / 2;
                strm.head.lastBlocklen = (uint)blocks[blocks.Length - 1].Length;
                strm.head.lastBlockSample = strm.head.lastBlocklen / 2;
                strm.head.nBlocks = (uint)blocks.Length;
                strm.head.nSamples = strm.head.nBlocks * 0x100 + strm.head.lastBlockSample;
                strm.data.data = wav.wave.data.data;
            }

            strm.data.size = (uint)strm.data.data.Length + 0x08;
            strm.cabecera.fileSize = strm.data.size + strm.head.size + strm.cabecera.headerSize;

            return strm;
        }

        #region STRM to WAV methods
        /// <summary>
        /// Get one channels converted to PCM-16 from the audio data
        /// </summary>
        /// <param name="data">Audio data with two channels</param>
        /// <param name="nBlocks">Numbers of blocks in the audio data</param>
        /// <param name="blockLen">Block length in the audio data</param>
        /// <param name="lastBlockLen">Length of the last block</param>
        /// <param name="leftChannel">If true, return the left channel, else return the right channel</param>
        /// <param name="waveType">The wavetype of the audio data (compression)</param>
        /// <returns>Data of the channel</returns>
        static byte[] DivideChannels(byte[] data, uint nBlocks, uint blockLen, uint lastBlockLen, bool leftChannel, int waveType)
        {
            List<byte> resultado = new List<byte>();
            List<byte> datos = new List<byte>();
            datos.AddRange(data);
            int j = (leftChannel) ? 0 : (int)blockLen;

            byte[] blockData;
            for (int i = 0; i < nBlocks - 1; i++)
            {
                blockData = new byte[(int)blockLen];
                datos.CopyTo(i * (int)blockLen * 2 + j, blockData, 0, (int)blockLen);

                if (waveType == 2)
                    resultado.AddRange(Compression_ADPCM.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
                else if (waveType == 1)
                    resultado.AddRange(blockData);
                else if (waveType == 0)
                    resultado.AddRange(PCM8(blockData));
            }
            blockData = new byte[(int)lastBlockLen];
            j = (leftChannel) ? 0 : (int)lastBlockLen;
            datos.CopyTo((int)(nBlocks - 1) * (int)blockLen * 2 + j, blockData, 0, (int)lastBlockLen);

            if (waveType == 2)
                resultado.AddRange(Compression_ADPCM.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
            else if (waveType == 1)
                resultado.AddRange(blockData);
            else if (waveType == 0)
                resultado.AddRange(PCM8(blockData));

            return resultado.ToArray();
        }
        /// <summary>
        /// Create audio data from two channels
        /// </summary>
        /// <param name="leftChannel">Audio data from the left channel</param>
        /// <param name="rightChannel">Audio data from the right channel</param>
        /// <param name="loopSample">Sample where the audio data will start (used in loops)</param>
        /// <returns>Audio data</returns>
        static byte[] MergeChannels(byte[] leftChannel, byte[] rightChannel, int loopSample = 0)
        {
            List<byte> resultado = new List<byte>();

            for (int i = loopSample; i < leftChannel.Length; i += 2)
            {
                resultado.Add(leftChannel[i]);
                if (i + 1 < leftChannel.Length)
                    resultado.Add(leftChannel[i + 1]);

                resultado.Add(rightChannel[i]);
                if (i + 1 < leftChannel.Length)
                    resultado.Add(rightChannel[i + 1]);
            }

            return resultado.ToArray();
        }
        /// <summary>
        /// Get the channel data converted to PCM-16 from mono audio data
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <param name="nBlocks">Number of blocks</param>
        /// <param name="blockLen">Block length</param>
        /// <param name="lastBlockLen">Last block length</param>
        /// <param name="waveType">Wavetype of the audio data (compression)</param>
        /// <returns>Channel data</returns>
        static byte[] MonoChannel(byte[] data, uint nBlocks, uint blockLen, uint lastBlockLen, int waveType)
        {
            List<byte> resultado = new List<byte>();
            List<byte> datos = new List<byte>();
            datos.AddRange(data);

            byte[] blockData;
            for (int i = 0; i < nBlocks - 1; i++)
            {
                blockData = new byte[(int)blockLen];
                datos.CopyTo(i * (int)blockLen, blockData, 0, (int)blockLen);
                
                if (waveType == 2)
                    resultado.AddRange(Compression_ADPCM.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
                else if (waveType == 1)
                    resultado.AddRange(blockData);
                else if (waveType == 0)
                    resultado.AddRange(PCM8(blockData));
            }
            blockData = new byte[(int)lastBlockLen];
            datos.CopyTo((int)(nBlocks - 1) * (int)blockLen, blockData, 0, (int)lastBlockLen);
            

            if (waveType == 2)
            {
                if (blockData.Length < 4) // Archivos sin sonido
                {
                    resultado.AddRange(blockData);
                    return resultado.ToArray();
                }
                resultado.AddRange(Compression_ADPCM.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
            }
            else if (waveType == 1)
                resultado.AddRange(blockData);
            else if (waveType == 0)
                resultado.AddRange(PCM8(blockData));

            return resultado.ToArray();
        }
        #endregion
        #region WAV to STRM methods
        /// <summary>
        /// Divide the channels of PCM-16 audio data
        /// </summary>
        /// <param name="data">PCM-16 audio data</param>
        /// <param name="leftChannel">If true, returns the audio data of the left channel, else the right channel</param>
        /// <param name="waveType">The wave type of the audio data (compression)</param>
        /// <returns>A channel audio data</returns>
        static byte[] DivideChannels(byte[] data, bool leftChannel, int waveType)
        {
            List<Byte> channel = new List<byte>();

            // TODO: Add compressions (AD-PCM ,PCM-8)
            for (int i = (leftChannel ? 0 : 2); i < data.Length; i += 4)
            {
                channel.Add(data[i]);
                channel.Add(data[i + 1]);
            }

            return channel.ToArray();
        }
        /// <summary>
        /// Create a block data from audio data. Default length 512 bytes, 256 samples.
        /// </summary>
        /// <param name="channel">Channel audio data</param>
        /// <returns>Block data</returns>
        static byte[][] CreateBlocks(byte[] channel, int blockSize)
        {
            List<Byte[]> blocks = new List<Byte[]>();

            int nBlocks = channel.Length / blockSize;
            int lastBlockLength = channel.Length % blockSize;

            Byte[] block = new Byte[blockSize]; 
            for (int i = 0; i < nBlocks; i++)
            {
                block = new Byte[blockSize];
                Array.Copy(channel, i * blockSize, block, 0, blockSize);
                blocks.Add(block);
            }
            block = new Byte[lastBlockLength];
            Array.Copy(channel, nBlocks * blockSize, block, 0, lastBlockLength);
            blocks.Add(block);

            return blocks.ToArray();
        }
        /// <summary>
        /// Merge blocks of two channels.
        /// </summary>
        /// <param name="leftChannel">Left channel block</param>
        /// <param name="rightChannel">Right channel block</param>
        /// <returns>Return the audio data</returns>
        static byte[] MergeBlocks(byte[][] leftChannel, byte[][] rightChannel)
        {
            List<byte> data = new List<byte>();

            for (int i = 0; i < leftChannel.Length; i++)
            {
                data.AddRange(leftChannel[i]);
                data.AddRange(rightChannel[i]);
            }

            return data.ToArray();
        }
        #endregion

        /// <summary>
        /// Convert PCM-8 audio data to PCM-16 audio data
        /// </summary>
        /// <param name="data">PCM-8 bytes</param>
        /// <returns>PCM-16 bytes</returns>
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

    /// <summary>
    /// Structure of a STRM file
    /// </summary>
    public struct sSTRM
    {
        public Cabecera cabecera;
        public HEAD head;
        public DATA data;

        public struct Cabecera
        {
            public char[] id;
            public uint constant;
            public uint fileSize;
            public ushort headerSize;
            public ushort nBlocks;
        }
        public struct HEAD
        {
            public char[] id;
            public uint size;
            public byte waveType;
            public byte loop;
            public ushort channels;
            public ushort sampleRate;
            public ushort time;
            public uint loopOffset;
            public uint nSamples;
            public uint dataOffset;
            public uint nBlocks;
            public uint blockLen;
            public uint blockSample;
            public uint lastBlocklen;
            public uint lastBlockSample;
            public byte[] reserved; // 32 bytes. Always 0
        }
        public struct DATA
        {
            public char[] id;
            public uint size;
            public byte[] leftChannel;
            public byte[] rightChannel;
            public byte[] data;
        }
    }
}
