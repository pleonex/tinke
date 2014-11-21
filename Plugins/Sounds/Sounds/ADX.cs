using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sounds
{
    // Copied from: http://en.wikipedia.org/wiki/ADX_(file_format)
    public class ADX : SoundBase
    {
        ADX_Header adx_header;
        int[] past_samples;       // Previously decoded samples from each channel, zeroed at start (size = 2*channel_count)
        uint sample_index;      // sample_index is the index of sample set that needs to be decoded next
        double[] coefficient;

        public ADX(string file, int id) : base(file, id, "ADX", "Wikipedia", false)  { }


        public override byte[] Read_File()
        {
            adx_header = new ADX_Header();

            // Read the file
            BinaryReader br = new BinaryReader(File.OpenRead(soundFile));

            byte b1 = br.ReadByte();  // 0x80
            byte b2 = br.ReadByte();  // 0x00

            adx_header.copyright_offset = Helper.Get_ushort(ref br);
            adx_header.encoding = br.ReadByte();
            adx_header.block_size = br.ReadByte();
            adx_header.sample_bitdepth = br.ReadByte();
            adx_header.channel_count = br.ReadByte();
            adx_header.sample_rate = Helper.Get_uint(ref br);
            adx_header.total_samples = Helper.Get_uint(ref br);
            adx_header.highpass_frequency = Helper.Get_ushort(ref br);
            adx_header.version = br.ReadByte();
            adx_header.flags = br.ReadByte();
            adx_header.unknown = Helper.Get_uint(ref br);

            switch (adx_header.version)
            {
                case 3:
                    adx_header.loop_enabled = Helper.Get_uint(ref br);
                    adx_header.loop_begin_sample_index = Helper.Get_uint(ref br);
                    adx_header.loop_begin_byte_index = Helper.Get_uint(ref br);
                    adx_header.loop_end_sample_index = Helper.Get_uint(ref br);
                    adx_header.loop_end_byte_index = Helper.Get_uint(ref br);
                    break;

                case 4:
                    br.BaseStream.Position += 0x0C;
                    adx_header.loop_enabled = Helper.Get_uint(ref br);
                    adx_header.loop_begin_sample_index = Helper.Get_uint(ref br);
                    adx_header.loop_begin_byte_index = Helper.Get_uint(ref br);
                    adx_header.loop_end_sample_index = Helper.Get_uint(ref br);
                    adx_header.loop_end_byte_index = Helper.Get_uint(ref br);
                    break;
            }

            br.BaseStream.Position = adx_header.copyright_offset - 2;
            adx_header.copyright = br.ReadChars(6);

            br.BaseStream.Position = 0;
            byte[] buffer = br.ReadBytes((int)br.BaseStream.Length);

            br.Close();

            // Save values
            loop_enabled = Convert.ToBoolean(adx_header.loop_enabled);
            loop_begin_sample = adx_header.loop_begin_sample_index;
            loop_end_sample = adx_header.loop_end_sample_index;

            total_samples = adx_header.total_samples;
            sample_rate = adx_header.sample_rate;
            channels = adx_header.channel_count;
            block_size = adx_header.block_size;
            sample_bitdepth = adx_header.sample_bitdepth;

            return buffer;
        }


        private void Initialize_Decode()
        {
            double a, b, c;
            a = Math.Sqrt(2.0) - Math.Cos(2.0 * Math.Acos(-1.0) * ((double)adx_header.highpass_frequency / adx_header.sample_rate));
            b = Math.Sqrt(2.0) - 1.0;
            c = (a - Math.Sqrt((a + b) * (a - b))) / b;

            coefficient = new double[2];
            coefficient[0] = c * 2.0;
            coefficient[1] = -(c * c);

            past_samples = new int[2 * adx_header.channel_count];
        }

        /// <summary>
        /// Decode the standard format.
        /// Copied from: "http://en.wikipedia.org/wiki/ADX_(file_format)"
        /// </summary>
        /// <param name="buffer">buffer is where the decoded samples will be put</param>
        /// <param name="samples_needed">samples_needed states how many sample 'sets' (one sample from every channel) need to be decoded to fill the buffer</param>
        /// <param name="looping_enabled">looping_enabled is a boolean flag to control use of the built-in loop</param>
        /// <returns>Returns the number of sample 'sets' in the buffer that could not be filled (EOS)</returns>
        public override byte[] Decode(byte[] encoded, bool looping_enabled)
        {
            BitReader bit_reader = new BitReader(encoded, true);
            List<byte> bufferOut = new List<byte>();

            Initialize_Decode();

            // Param:
            uint samples_needed = adx_header.total_samples;

            uint samples_per_block = (uint)((adx_header.block_size - 2) * 8 / adx_header.sample_bitdepth);
            short[] scale = new short[adx_header.channel_count];

            if (looping_enabled && adx_header.loop_enabled == 0)
                looping_enabled = false;

            if (looping_enabled)    // Mine
                sample_index = adx_header.loop_begin_sample_index;

            // Loop until the requested number of samples are decoded, or the end of file is reached
            while (samples_needed > 0 && sample_index < adx_header.total_samples)
            {
                // Calculate the number of samples that are left to be decoded in the current block
                uint sample_offset = sample_index % samples_per_block;
                uint samples_can_get = samples_per_block - sample_offset;

                // Clamp the samples we can get during this run if they won't fit in the buffer
                if (samples_can_get > samples_needed)
                    samples_can_get = samples_needed;

                // Clamp the number of samples to be acquired if the stream isn't long enough or the loop trigger is nearby
                if (looping_enabled && sample_index + samples_can_get > adx_header.loop_end_sample_index)
                    samples_can_get = adx_header.loop_end_sample_index - sample_index;
                else if (sample_index + samples_can_get > adx_header.total_samples)
                    samples_can_get = adx_header.total_samples - sample_index;

                // Calculate the bit address of the start of the frame that sample_index resides in and record that location
                ulong started_at = (ulong)(adx_header.copyright_offset + 4 +
                    sample_index / samples_per_block * adx_header.block_size * adx_header.channel_count) * 8;

                // Read the scale values from the start of each block in this frame
                for (uint i = 0; i < adx_header.channel_count; ++i)
                {
                    bit_reader.Seek((int)(started_at + adx_header.block_size * i * 8));
                    scale[i] = bit_reader.Read_Short();
                }

                // Pre-calculate the stop value for sample_offset
                uint sample_endoffset = sample_offset + samples_can_get;

                // Save the bitstream address of the first sample immediately after the scale in the first block of the frame
                started_at += 16;
                while (sample_offset < sample_endoffset)
                {
                    for (uint i = 0; i < adx_header.channel_count; ++i)
                    {
                        // Predict the next sample
                        double sample_prediction = coefficient[0] * past_samples[i * 2 + 0] + coefficient[1] * past_samples[i * 2 + 1];

                        // Seek to the sample offset, read and sign extend it to a 32bit integer
                        bit_reader.Seek((int)((int)started_at + adx_header.sample_bitdepth * sample_offset + adx_header.block_size * 8 * i));
                        int sample_error = 0;
                        if (adx_header.sample_bitdepth == 4)
                            sample_error = bit_reader.Read_4Bits();

                        // Scale the error correction value
                        sample_error *= scale[i];

                        // Calculate the sample by combining the prediction with the error correction
                        int sample = sample_error + (int)sample_prediction;

                        // Update the past samples with the newer sample
                        past_samples[i * 2 + 1] = past_samples[i * 2 + 0];
                        past_samples[i * 2 + 0] = sample;

                        // Clamp the decoded sample to the valid range for a 16bit integer
                        if (sample > 32767)
                            sample = 32767;
                        else if (sample < -32768)
                            sample = -32768;

                        bufferOut.AddRange(BitConverter.GetBytes((short)sample));
                    }

                    ++sample_offset;  // We've decoded one sample from every block, advance block offset by 1
                    ++sample_index;   // This also means we're one sample further into the stream
                    --samples_needed; // And so there is one less set of samples that need to be decoded
                }

                // Check if we hit the loop end marker, if we did we need to jump to the loop start
                if (looping_enabled && sample_index == adx_header.loop_end_sample_index)
                    //sample_index = adx_header.loop_begin_sample_index;
                    break;
            }

            //return samples_needed;
            return bufferOut.ToArray();
        }

        public struct ADX_Header
        {
            public ushort copyright_offset;
            public byte encoding;
            public byte block_size;
            public byte sample_bitdepth;
            public byte channel_count;
            public uint sample_rate;
            public uint total_samples;
            public ushort highpass_frequency;
            public byte version;
            public byte flags;
            public uint unknown;
            public uint loop_enabled;
            public uint loop_begin_sample_index;
            public uint loop_begin_byte_index;
            public uint loop_end_sample_index;
            public uint loop_end_byte_index;
            public char[] copyright;
        }

        public enum Encoding : byte
        {
            Standard_ADX = 0x03,
            ADX_exponential_scale = 0x4,
            AHX = 0x10,
            AHX2 = 0x11
        }

        public override void Write_File(string fileOut, byte[] data)
        {
            throw new NotImplementedException();
        }
		protected override byte[] Encode(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
