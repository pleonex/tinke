using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sounds
{
    public static class Helper
    {
        public static byte[] MergeChannels(byte[] leftChannel, byte[] rightChannel, int loopSample = 0)
        {
            List<byte> resultado = new List<byte>();

            for (int i = loopSample; i < leftChannel.Length; i += 2)
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
        public static byte[][] DividieChannels(byte[] data)
        {
            List<Byte> leftChannel = new List<byte>();
            List<Byte> rightChannel = new List<byte>();

            for (int i = 0; i < data.Length / 4; i += 4)
            {
                leftChannel.Add(data[i]);
                leftChannel.Add(data[i + 1]);

                rightChannel.Add(data[i + 2]);
                rightChannel.Add(data[i + 3]);
            }

            return new Byte[][] { leftChannel.ToArray(), rightChannel.ToArray() };
        }

        public static byte[] Bit8ToBit4(byte[] data)
        {
            List<byte> bit4 = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                bit4.Add((byte)(data[i] & 0x0F));
                bit4.Add((byte)((data[i] & 0xF0) >> 4));
            }

            return bit4.ToArray();
        }

    }
}
