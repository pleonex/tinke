using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sounds
{
    public abstract class SoundBase
    {
        #region Variables
        protected string soundFile;
        protected int id;
        protected String format;
        protected String copyright;
        protected bool editable;

        byte[] pcm16;
        byte[] pcm16_loop;

        // Variables:
        protected bool loop_enabled;
        protected uint loop_begin_sample;
        protected uint loop_end_sample;

        protected uint total_samples;
        protected uint sample_rate;
        protected uint channels;
        protected uint block_size;
        protected uint sample_bitdepth;
        #endregion

        #region Properties
        public String Format
        {
            get { return format; }
        }
        public int ID
        {
            get { return id; }
        }
        public String Copyright
        {
            get { return copyright; }
        }
        public bool CanEdit
        {
            get { return editable; }
        }
        public bool CanLoop
        {
            get { return loop_enabled; }
        }
        public uint LoopBegin
        {
            get { return loop_begin_sample; }
        }
        public uint LoopEnd
        {
            get { return loop_end_sample; }
        }
        public uint NumberSamples
        {
            get { return total_samples; }
        }
        public uint SampleRate
        {
            get { return sample_rate; }
        }
        public uint Channels
        {
            get { return channels; }
        }
        public uint BlockSize
        {
            get { return block_size; }
        }
        public uint SampleBitDepth
        {
            get { return sample_bitdepth; }
        }
        #endregion

        public SoundBase(string soundFile, int id, string format, string copyright, bool editable)
        {
            this.soundFile = soundFile;
            this.id = id;
            this.format = format;
            this.copyright = copyright;
            this.editable = editable;
        }
        ~ SoundBase()
        {
            pcm16 = null;
            pcm16 = null;
        }

        /// <summary>
        /// Read the file and decode the audio
        /// </summary>
        public void Initialize()
        {
            // Fill this class
            byte[] encoded = Read_File();

            // Decode the audio data
            pcm16 = Decode(encoded, false);
            if (loop_enabled)
                pcm16_loop = Decode(encoded, true);

            encoded = null;
        }
        public abstract byte[] Read_File();
        public abstract byte[] Decode(byte[] encoded, bool loop_enabled);

        /// <summary>
        /// Import a WAV file
        /// </summary>
        /// <param name="fileIn">Path of the WAV file</param>
        public void Import(string fileIn)
        {
            sWAV wav = Read_WAV(fileIn);
            
            // Save the new values
            pcm16 = wav.wave.data.data;
            pcm16_loop = new byte[0];

            total_samples = (uint)(wav.wave.data.data.Length / ((wav.wave.fmt.bitsPerSample / 8) * wav.wave.fmt.numChannels));
            sample_rate = wav.wave.fmt.sampleRate;
            channels = wav.wave.fmt.numChannels;
            block_size = wav.wave.fmt.blockAlign;
            sample_bitdepth = wav.wave.fmt.bitsPerSample;

            loop_enabled = false;
            loop_begin_sample = 0;
            loop_end_sample = total_samples;
        }
        public abstract void Write_File(string fileOut, byte[] data);
        public byte[] Encode()
		{
			return this.Encode(this.pcm16);
		}
		protected abstract byte[] Encode(byte[] data);

        /// <summary>
        ///  It doesn't work... :(
        /// </summary>
        /// <returns></returns>
        public Stream Get_Stream()
        {
            uint byteRate = sample_rate * 0x10 * channels / 8;
            ushort blockAlign = (ushort)(channels * 0x10 / 8);

            MemoryStream ms = null;
            BinaryWriter bw = null;
            try
            {
                ms = new MemoryStream(0x2C + pcm16.Length);
                bw = new BinaryWriter(ms);

                bw.Write(new char[] { 'R', 'I', 'F', 'F' });
                bw.Write((uint)(0x28 + pcm16.Length));

                bw.Write(new char[] { 'W', 'A', 'V', 'E' });
                bw.Write(new char[] { 'f', 'm', 't', '\x20' });
                bw.Write((uint)0x10);
                bw.Write((ushort)0x01);
                bw.Write((ushort)channels);
                bw.Write(sample_rate);
                bw.Write(byteRate);
                bw.Write(blockAlign);
                bw.Write((ushort)0x10);

                bw.Write(new char[] { 'd', 'a', 't', 'a' });
                bw.Write((uint)pcm16.Length);
                bw.Write(pcm16);

                bw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return ms;
        }
        public void Save_WAV(string fileOut, bool loop)
        {
            uint byteRate = sample_rate * 0x10 * channels / 8;
            ushort blockAlign = (ushort)(channels * 0x10 / 8);

            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(fileOut, FileMode.Create);
                bw = new BinaryWriter(fs);

                bw.Write(new char[] { 'R', 'I', 'F', 'F' });
                if (loop)
                    bw.Write((uint)(0x28 + pcm16_loop.Length));
                else
                    bw.Write((uint)(0x28 + pcm16.Length));

                bw.Write(new char[] { 'W', 'A', 'V', 'E' });
                bw.Write(new char[] { 'f', 'm', 't', '\x20' });
                bw.Write((uint)0x10);
                bw.Write((ushort)0x01);
                bw.Write((ushort)channels);
                bw.Write(sample_rate);
                bw.Write(byteRate);
                bw.Write(blockAlign);
                bw.Write((ushort)0x10);

                bw.Write(new char[] { 'd', 'a', 't', 'a' });
                if (loop)
                {
                    bw.Write((uint)pcm16_loop.Length);
                    bw.Write(pcm16_loop);
                }
                else
                {
                    bw.Write((uint)pcm16.Length);
                    bw.Write(pcm16);
                }

                bw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            finally
            {
                if (bw != null) bw.Close();
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                bw = null;
                fs = null;
            }
        }


        private sWAV Read_WAV(string fileIn)
        {
            sWAV wav = new sWAV();
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

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
                wav.wave.data.data = Compression.PCM.PCM8UnsignedToPCM16(wav.wave.data.data);
            }

            return wav;
        }
    }
}
