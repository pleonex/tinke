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

namespace SDAT
{
    class SWAV
    {
        public static sSWAV LeerArchivo(string path)
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

        public static void EscribirArchivo(sSWAV swav, string path)
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

        public static sWAV ConvertirAWAV(sSWAV swav)
        {
            sWAV wav = new sWAV();


            if (swav.data.info.nWaveType == 0)
            {
                swav.data.data = PCM8(swav.data.data);
                wav = WAV.Create_WAV(1, swav.data.info.nSampleRate, 8, swav.data.data);
            }
            else if (swav.data.info.nWaveType == 1)
            {
                wav = WAV.Create_WAV(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }
            else if (swav.data.info.nWaveType >= 2)
            {
                swav.data.data = Compression_ADPCM.Decompress_ADPCM(swav.data.data);
                wav = WAV.Create_WAV(1, 33000, 16, swav.data.data);
            }
            return wav;
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
