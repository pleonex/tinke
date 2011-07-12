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
 * Fecha: 11/07/2011
 * 
 */

using System;

namespace SDAT
{
    public static class SWAV
    {
        public static ArchivoSWAV LeerArchivo(string path)
        {
            /***Lectura del archivo SWAV***/
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;

            ArchivoSWAV swav = new ArchivoSWAV();

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                br = new System.IO.BinaryReader(fs);

                //Leer Header
                swav.header.type = Help.ConvertirBytesAChars(br.ReadBytes(4));
                swav.header.magic = br.ReadUInt32();
                swav.header.nFileSize = br.ReadUInt32();
                swav.header.nSize = br.ReadUInt16();
                swav.header.nBlock = br.ReadUInt16();

                //Leer Data
                swav.data.type = Help.ConvertirBytesAChars(br.ReadBytes(4));
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
                System.Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (fs != null) fs.Close();
                if (br != null) br.Close();
            }

            return swav;
        }

        public static void EscribirArchivo(ArchivoSWAV swav, string path)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryWriter bw = null;

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                bw = new System.IO.BinaryWriter(fs);

                //Escribir Header
                bw.Write(Help.ConvertirCharsABytes(swav.header.type));
                bw.Write(swav.header.magic);
                bw.Write(swav.header.nFileSize);
                bw.Write(swav.header.nSize);
                bw.Write(swav.header.nBlock);

                //Escribir Data
                bw.Write(Help.ConvertirCharsABytes(swav.data.type));
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

        public static WAV.ArchivoWAV ConvertirAWAV(ArchivoSWAV swav)
        {
            WAV.ArchivoWAV wav = new WAV.ArchivoWAV();

            if (swav.data.info.nWaveType == 0)
            {
                wav = WAV.GenerarWAVPCM(1, swav.data.info.nSampleRate, 8, swav.data.data);
            }

            if (swav.data.info.nWaveType == 1)
            {
                wav = WAV.GenerarWAVPCM(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }

            if (swav.data.info.nWaveType >= 2)
            {
                wav = WAV.GenerarWAVADPCM(1, swav.data.info.nSampleRate, 16, swav.data.data);
            }
            return wav;
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
    }
}
