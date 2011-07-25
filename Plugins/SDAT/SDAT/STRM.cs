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
 * Fecha: 16/07/2011
 * 
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SDAT
{
    class STRM
    {
        public static sSTRM LeerArchivo(string path)
        {
            sSTRM strm = new sSTRM();
            BinaryReader br = new BinaryReader(File.OpenRead(path));

            // Genérico
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

        public static void EscribirArchivo(sSTRM strm, string path)
        {
            /*System.IO.FileStream fs = null;
            System.IO.BinaryWriter bw = null;

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                bw = new System.IO.BinaryWriter(fs);

                //Escribir File
                bw.Write(Encoding.ASCII.GetBytes(strm.file.type));
                bw.Write(strm.file.magic);
                bw.Write(strm.file.nFileSize);
                bw.Write(strm.file.nSize);
                bw.Write(strm.file.nBlock);

                //Escribir Header
                bw.Write(Encoding.ASCII.GetBytes(strm.head.type));
                bw.Write(strm.head.nSize);
                bw.Write(strm.head.nWaveType);
                bw.Write(strm.head.bLoop);
                bw.Write(strm.head.nChannel);
                bw.Write(strm.head.nSampleRate);
                bw.Write(strm.head.nTime);
                bw.Write(strm.head.nLoopOffset);
                bw.Write(strm.head.nSample);
                bw.Write(strm.head.nDataOffset);
                bw.Write(strm.head.nBlock);
                bw.Write(strm.head.nBlockLen);
                bw.Write(strm.head.nBlockSample);
                bw.Write(strm.head.nLastBlockLen);
                bw.Write(strm.head.nLastBlockSample);
                bw.Write(strm.head.reserved);

                //Escribir Data
                bw.Write(Encoding.ASCII.GetBytes(strm.data.type));
                bw.Write(strm.data.nSize);
                bw.Write(strm.data.data);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (fs != null) fs.Close();
                if (bw != null) bw.Close();
            }*/
        }

        public static WAV.ArchivoWAV ConvertirAWAV(sSTRM strm)
        {
            WAV.ArchivoWAV wav = new WAV.ArchivoWAV();

            if (strm.head.channels == 2)
            {
                strm.data.leftChannel = SepararCanales(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, true, strm.head.waveType);
                strm.data.rightChannel = SepararCanales(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, false, strm.head.waveType);
                Array.Clear(strm.data.data, 0, strm.data.data.Length); // Borramos datos no necesarios

                strm.data.data = AlternarCanales(strm.data.leftChannel, strm.data.rightChannel);
            }
            else if (strm.head.channels == 1)
            {
                strm.data.data = ObtenerCanal(strm.data.data, strm.head.nBlocks, strm.head.blockLen,
                    strm.head.lastBlocklen, strm.head.waveType);
            }

            if (strm.head.waveType == 0)
            {
                wav = WAV.GenerarWAVPCM(strm.head.channels, strm.head.sampleRate, 8, strm.data.data);
            }

            if (strm.head.waveType == 1)
            {
                wav = WAV.GenerarWAVPCM(strm.head.channels, strm.head.sampleRate, 16, strm.data.data);
            }

            if (strm.head.waveType >= 2)
            {
                wav = WAV.GenerarWAVADPCM(strm.head.channels, strm.head.sampleRate, 16, strm.data.data);
            }
            return wav;
        }

        static byte[] SepararCanales(byte[] data, uint nBlocks, uint blockLen, uint lastBlockLen, bool leftChannel, int waveType)
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
                    resultado.AddRange(AdpcmDecompressor.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
                else if (waveType == 1)
                    resultado.AddRange(blockData);
                else if (waveType == 0)
                    resultado.AddRange(PCM8(blockData));
            }
            blockData = new byte[(int)lastBlockLen];
            j = (leftChannel) ? 0 : (int)lastBlockLen;
            datos.CopyTo((int)(nBlocks - 1) * (int)blockLen * 2 + j, blockData, 0, (int)lastBlockLen);

            if (waveType == 2)
                resultado.AddRange(AdpcmDecompressor.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
            else if (waveType == 1)
                resultado.AddRange(blockData);
            else if (waveType == 0)
                resultado.AddRange(PCM8(blockData));

            return resultado.ToArray();
        }
        static byte[] AlternarCanales(byte[] leftChannel, byte[] rightChannel)
        {
            List<byte> resultado = new List<byte>();

            for (int i = 0; i < leftChannel.Length; i += 2)
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
        static byte[] ObtenerCanal(byte[] data, uint nBlocks, uint blockLen, uint lastBlockLen, int waveType)
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
                    resultado.AddRange(AdpcmDecompressor.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
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
                resultado.AddRange(AdpcmDecompressor.DecompressBlock_ADPCM(blockData, BitConverter.ToInt16(blockData, 0), BitConverter.ToInt16(blockData, 2)));
            }
            else if (waveType == 1)
                resultado.AddRange(blockData);
            else if (waveType == 0)
                resultado.AddRange(PCM8(blockData));

            return resultado.ToArray();
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
                // Reserved[32] always 0
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
}
