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
    class SWAR
    {
        public static ArchivoSWAR LeerArchivo(string path)
        {
            /***Lectura del archivo SWAR***/
            System.IO.FileStream fs = null;
            System.IO.BinaryReader br = null;

            ArchivoSWAR swar = new ArchivoSWAR();

            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                br = new System.IO.BinaryReader(fs);

                //Leer Header
                swar.header.type = Help.BytesToChars(br.ReadBytes(4));
                swar.header.magic = br.ReadUInt32();
                swar.header.nFileSize = br.ReadUInt32();
                swar.header.nSize = br.ReadUInt16();
                swar.header.nBlock = br.ReadUInt16();

                //Leer Data
                swar.data.type = Help.BytesToChars(br.ReadBytes(4));
                swar.data.nSize = br.ReadUInt32();
                swar.data.reserved = new uint[8];
                for (int i = 0; i < 8; i++) { swar.data.reserved[i] = br.ReadUInt32(); }
                swar.data.nSample = br.ReadUInt32();

                swar.data.nOffset = new uint[swar.data.nSample];
                for (int i = 0; i < swar.data.nSample; i++) { swar.data.nOffset[i] = br.ReadUInt32(); }
 
                swar.data.samples = new ArchivoSWAR.Data.Sample[swar.data.nSample];

                for (uint i = 0; i < swar.data.nSample; i++)
                {
                    //Leer Info
                    swar.data.samples[i].info.nWaveType = br.ReadByte();
                    swar.data.samples[i].info.bLoop = br.ReadByte();
                    swar.data.samples[i].info.nSampleRate = br.ReadUInt16();
                    swar.data.samples[i].info.nTime = br.ReadUInt16();
                    swar.data.samples[i].info.nLoopOffset = br.ReadUInt16();
                    swar.data.samples[i].info.nNonLoopLen = br.ReadUInt32();

                    //Calcular tamaño de data
                    if (i < swar.data.nOffset.Length - 1)
                    {
                        swar.data.samples[i].data = new byte[swar.data.nOffset[i + 1] - swar.data.nOffset[i] - /*tamaño de SWAVInfo ->*/12];
                    }
                    else
                    {
                        swar.data.samples[i].data = new byte[br.BaseStream.Length - swar.data.nOffset[i] - /*tamaño de SWAVInfo ->*/12];
                    }

                    //Leer data
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

        public static SWAV.ArchivoSWAV[] ConvertirASWAV(ArchivoSWAR swar)
        {
            SWAV.ArchivoSWAV[] swav = new SWAV.ArchivoSWAV[swar.data.samples.Length];

            for (int i = 0; i < swav.Length; i++)
            {
                swav[i] = new SWAV.ArchivoSWAV();

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


        public struct ArchivoSWAR
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
                    public SWAV.ArchivoSWAV.Data.SWAVInfo info;
                    public byte[] data;
                }
            }




        }
    }



}
