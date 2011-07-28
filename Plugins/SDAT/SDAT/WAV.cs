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
 * Fecha: 15/07/2011
 * 
 */

using System;

namespace SDAT
{
    class WAV
    {
        public static void EscribirArchivo(ArchivoWAV wr, string path)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryWriter bw = null;
            try
            {
                fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
                bw = new System.IO.BinaryWriter(fs);

                bw.Write(System.Text.Encoding.ASCII.GetBytes(wr.chunkID));
                bw.Write(wr.chunkSize);
                bw.Write(System.Text.Encoding.ASCII.GetBytes(wr.format));
                bw.Write(System.Text.Encoding.ASCII.GetBytes(wr.wave.fmt.chunkID));
                bw.Write(wr.wave.fmt.chunkSize);
                bw.Write(Convert.ToUInt16(wr.wave.fmt.audioFormat));
                bw.Write(wr.wave.fmt.numChannels);
                bw.Write(wr.wave.fmt.sampleRate);
                bw.Write(wr.wave.fmt.byteRate);
                bw.Write(wr.wave.fmt.blockAlign);
                bw.Write(wr.wave.fmt.bitsPerSample);
                bw.Write(System.Text.Encoding.ASCII.GetBytes(wr.wave.data.chunkID));
                bw.Write(wr.wave.data.chunkSize);
                bw.Write(wr.wave.data.data);

                bw.Flush();
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
            archivo.wave.data.data = new byte[data.Length];
            archivo.wave.data.data = data;

            archivo.chunkSize = 4 + (8 + archivo.wave.fmt.chunkSize) + (8 + archivo.wave.data.chunkSize);

            return archivo;
        }

        public static ArchivoWAV GenerarWAVADPCM(ushort numChannels, uint sampleRate, ushort bitsPerSample, byte[] data)
        {
            ArchivoWAV archivo = new ArchivoWAV();

            if (numChannels == 1)
            {
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
                archivo.wave.data.data = new byte[data.Length];
                archivo.wave.data.data = data;

                archivo.chunkSize = 4 + (8 + archivo.wave.fmt.chunkSize) + (8 + archivo.wave.data.chunkSize);
            }

            if (numChannels == 2)
            {
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
                archivo.wave.data.data = new byte[data.Length];
                archivo.wave.data.data = data;

                archivo.chunkSize = (uint)(0x24 + data.Length);
            }

            return archivo;
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
                    public byte[] data;
                }
            }
        }
    }
}
