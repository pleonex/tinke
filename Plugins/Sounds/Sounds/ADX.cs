using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sounds
{
    // Copied from: http://en.wikipedia.org/wiki/ADX_(file_format)
    public class ADX : SoundBase
    {
        ADX_Header adx_header;
        int past_samples;       // Previously decoded samples from each channel, zeroed at start (size = 2*channel_count)
        uint sample_index;      // sample_index is the index of sample set that needs to be decoded next
        double[] coefficient;


        public ADX()
        {

        }

        private void Read_Header()
        {

        }

        private void Initialize()
        {
            double a, b, c;
            a = Math.Sqrt(2.0) - Math.Cos(2.0 * Math.Acos(-1.0) * ((double)adx_header.highpass_frequency / adx_header.sample_rate));
            b = Math.Sqrt(2.0) - 1.0;
            c = (a - Math.Sqrt((a + b) * (a - b))) / b;

            coefficient = new double[2];
            coefficient[0] = c * 2.0;
            coefficient[1] = -(c * c);
        }

        /// <summary>
        /// Decode the standard format.
        /// Copied from: http://en.wikipedia.org/wiki/ADX_(file_format)
        /// </summary>
        /// <param name="buffer">buffer is where the decoded samples will be put</param>
        /// <param name="samples_needed">samples_needed states how many sample 'sets' (one sample from every channel) need to be decoded to fill the buffer</param>
        /// <param name="looping_enabled">looping_enabled is a boolean flag to control use of the built-in loop</param>
        /// <returns>Returns the number of sample 'sets' in the buffer that could not be filled (EOS)</returns>
        private uint Decode_Standard(List<short> buffer, uint samples_needed, bool looping_enabled)
        {
            uint samples_per_block = (uint)((adx_header.block_size - 2) * 8 / adx_header.sample_bitdepth);
            short[] scale = new short[adx_header.channel_count];

            if (looping_enabled && adx_header.loop_enabled == 0)
                looping_enabled = false;

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

            }
            /*    
     // Read the scale values from the start of each block in this frame
     for (unsigned i = 0 ; i < adx_header->channel_count ; ++i)
     {
        bitstream_seek( started_at + adx_header->block_size * i * 8 );
        scale[i] = ntohs( bitstream_read( 16 ) );
     }
 
     // Pre-calculate the stop value for sample_offset
     unsigned sample_endoffset = sample_offset + samples_can_get;
 
     // Save the bitstream address of the first sample immediately after the scale in the first block of the frame
     started_at += 16;
     while ( sample_offset < sample_endoffset )
     {
        for (unsigned i = 0 ; i < adx_header->channel_count ; ++i)
        {
           // Predict the next sample
           double sample_prediction = coefficient[0] * past_samples[i*2 + 0] + coefficient[1] * past_samples[i*2 + 1];
 
           // Seek to the sample offset, read and sign extend it to a 32bit integer
           // Implementing sign extension is left as an exercise for the reader
           // The sign extension will also need to include a endian adjustment if there are more than 8 bits
           bitstream_seek( started_at + adx_header->sample_bitdepth * sample_offset + \
                           adx_header->block_size * 8 * i );
           int_fast32_t sample_error = bitstream_read( adx_header->sample_bitdepth );
           sample_error = sign_extend( sample_error, adx_header->sample_bitdepth );
 
           // Scale the error correction value
           sample_error *= scale[i];
 
           // Calculate the sample by combining the prediction with the error correction
           int_fast32_t sample = sample_error + (int_fast32_t)sample_prediction;
 
           // Update the past samples with the newer sample
           past_samples[i*2 + 1] = past_samples[i*2 + 0];
           past_samples[i*2 + 0] = sample;
 
           // Clamp the decoded sample to the valid range for a 16bit integer
           if (sample > 32767)
              sample = 32767;
           else if (sample < -32768)
              sample = -32768;
 
           // Save the sample to the buffer then advance one place
           *buffer++ = sample;
        }
        ++sample_offset;  // We've decoded one sample from every block, advance block offset by 1
        ++sample_index;   // This also means we're one sample further into the stream
        --samples_needed; // And so there is one less set of samples that need to be decoded
    }
 
    // Check if we hit the loop end marker, if we did we need to jump to the loop start
    if (looping_enabled && sample_index == adx_header->loop_end_index)
       sample_index = adx_header->loop_start_index;
  }*/
 
  return samples_needed;
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

    }
}
