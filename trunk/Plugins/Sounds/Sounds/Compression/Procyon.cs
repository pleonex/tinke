using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sounds.Compression
{
    /// <summary>
    /// Decode Procyon data. Credits to the author of VGMSTREAM
    /// </summary>
    public static class Procyon
    {
        static byte[][] proc_coef = new byte[][]
        {
            new byte[] {0x00,0x00},
            new byte[] {0x3C,0x00},
            new byte[] {0x73,0xCC},
            new byte[] {0x62,0xC9},
            new byte[] {0x7A,0xC4},
        };

        public static byte[] Decode(byte[] decoded, int offset, int samples_to_do, ref int[] hist, int channels)
        {
            int pos = 0;

            List<byte> buffer = new List<byte>();

            int first_sample = 0;
            int i = (int)first_sample;
            int sample_count;

            int framesin = first_sample / 30;

            pos = framesin * 16 + 15 + offset;
            sbyte header = (sbyte)decoded[pos];
            header = (sbyte)(header ^ 0x80);
            int scale = 12 - (header & 0xf);
            int coef_index = (header >> 4) & 0xf;
            int hist1 = hist[0];
            int hist2 = hist[1];
            int coef1 = 0;
            int coef2 = 0;

            if (coef_index > 4) coef_index = 0;
            coef1 = (sbyte)proc_coef[coef_index][0];
            coef2 = (sbyte)proc_coef[coef_index][1];
            first_sample = first_sample % 30;

            for (i = first_sample, sample_count = 0; i < first_sample + samples_to_do; i++, sample_count += (int)channels)
            {
                pos = framesin * 16 + offset + i / 2;
                int sample_byte = ((sbyte)decoded[pos] ^ 0x80);

                int sample = (int)((i & 1) != 0 ? Helper.get_high_nibble_signed((byte)sample_byte) : Helper.get_low_nibble_signed((byte)sample_byte)) * 64 * 64;

                if (scale < 0)
                    sample <<= -scale;
                else
                    sample >>= scale;

                sample = (hist1 * coef1 + hist2 * coef2 + 32) / 64 + (sample * 64);
                hist2 = hist1;
                hist1 = sample;

                short clamp = (short)(Helper.clamp16((sample + 32) / 64) / 64 * 64);
                buffer.AddRange(BitConverter.GetBytes(clamp));
            }

            hist[0] = hist1;
            hist[1] = hist2;

            return buffer.ToArray();
        }
    }
}
