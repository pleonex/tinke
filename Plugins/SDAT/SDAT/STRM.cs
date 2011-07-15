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

namespace SDAT
{
    class STRM
    {
        public static ArchivoSTRM LeerArchivo(string path)
        {
            /***Lectura del archivo SWAV***/
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;

            ArchivoSTRM strm = new ArchivoSTRM();

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                br = new System.IO.BinaryReader(fs);

                //Leer File
                strm.file.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                strm.file.magic = br.ReadUInt32();
                strm.file.nFileSize = br.ReadUInt32();
                strm.file.nSize = br.ReadUInt16();
                strm.file.nBlock = br.ReadUInt16();

                //Leer Header
                strm.head.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                strm.head.nSize = br.ReadUInt32();
                strm.head.nWaveType = br.ReadByte();
                strm.head.bLoop = br.ReadByte();
                strm.head.nChannel = br.ReadUInt16();
                strm.head.nSampleRate = br.ReadUInt16();
                strm.head.nTime = br.ReadUInt16();
                strm.head.nLoopOffset = br.ReadUInt16();
                strm.head.nSample = br.ReadUInt32();
                strm.head.nDataOffset = br.ReadUInt32();
                strm.head.nBlock = br.ReadUInt32();
                strm.head.nBlockLen = br.ReadUInt32();
                strm.head.nBlockSample = br.ReadUInt32();
                strm.head.nLastBlockLen = br.ReadUInt32();
                strm.head.nLastBlockSample = br.ReadUInt32();
                strm.head.reserved = br.ReadBytes(34);

                //Leer Data
                strm.data.type = Encoding.ASCII.GetChars(br.ReadBytes(4));
                strm.data.nSize = br.ReadUInt32();
                strm.data.data = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
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

            return strm;
        }

        public static void EscribirArchivo(ArchivoSTRM strm, string path)
        {
            System.IO.FileStream fs = null;
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
            }
        }

        public static WAV.ArchivoWAV ConvertirAWAV(ArchivoSTRM strm)
        {
            WAV.ArchivoWAV wav = new WAV.ArchivoWAV();

            if (strm.head.nWaveType == 0)
            {
                wav = WAV.GenerarWAVPCM(strm.head.nChannel, strm.head.nSampleRate, 8, strm.data.data);
            }

            if (strm.head.nWaveType == 1)
            {
                wav = WAV.GenerarWAVPCM(strm.head.nChannel, strm.head.nSampleRate, 16, strm.data.data);
            }

            if (strm.head.nWaveType >= 2)
            {
                wav = WAV.GenerarWAVADPCM(strm.head.nChannel, strm.head.nSampleRate, 16, strm.data.data);
            }
            return wav;
        }

        public struct ArchivoSTRM
        {
            public File file;
            public Head head;
            public Data data;

            public struct File
            {
                public char[] type;   // 'STRM'
                public uint magic;	// 0x0100feff
                public uint nFileSize;	// Size of this STRM file
                public ushort nSize;	// Size of this structure = 16
                public ushort nBlock;	// Number of Blocks = 2
            }

            public struct Head
            {
                public char[] type;		// 'HEAD'
                public uint nSize;		// Size of this structure
                public byte nWaveType;		// 0 = PCM8, 1 = PCM16, 2 = (IMA-)ADPCM
                public byte bLoop;		// Loop flag = TRUE|FALSE
                public ushort nChannel;		// Channels
                public ushort nSampleRate;	// Sampling Rate (perhaps resampled from the original) 
                public ushort nTime;		// (1.0 / rate * ARM7_CLOCK / 32) [ARM7_CLOCK: 33.513982MHz / 2 = 1.6756991e7]
                public uint nLoopOffset;	// Loop Offset (samples) 
                public uint nSample;		// Number of Samples 
                public uint nDataOffset;	// Data Offset (always 68h)
                public uint nBlock;		// Number of Blocks 
                public uint nBlockLen;		// Block Length (Per Channel) 
                public uint nBlockSample;	// Samples Per Block (Per Channel)
                public uint nLastBlockLen;	// Last Block Length (Per Channel)
                public uint nLastBlockSample;	// Samples Per Last Block (Per Channel)
                public byte[] reserved;	// always 32 zeros
            }

            public struct Data
            {
                public char[] type;		// 'DATA'
                public uint nSize;		// Size of this structure
                public byte[] data;		// Arrays of wave data
            }
        }
    }
}
