using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sounds.Compression
{
    public static class PCM
    {
        public static byte[] PCM8SignedToPCM16(byte[] data)
        {
            List<byte> resul = new List<byte>();

            data = Bit8ToBit16(data);
            for (int i = 0; i < data.Length; i += 2)
            {
                short sample = BitConverter.ToInt16(data, i);
                short pcm16 = (short)(sample & 0x7F);
                pcm16 <<= 8;
                if ((sample >> 7) != 0)
                    pcm16 -= 0x7FFF;

                resul.AddRange(BitConverter.GetBytes((short)pcm16));
            }

            return resul.ToArray();
        }
        public static byte[] PCM8UnsignedToPCM16(byte[] data)
        {
            List<byte> resul = new List<byte>();

            data = Bit8ToBit16(data);
            for (int i = 0; i < data.Length; i += 2)
            {
                short sample = BitConverter.ToInt16(data, i);
                short pcm16 = (short)(sample & 0xFF);
                pcm16 <<= 8;
                pcm16 += 0x7FFF;

                resul.AddRange(BitConverter.GetBytes((short)pcm16));
            }

            return resul.ToArray();
        }
        public static byte[] PCM16ToPCM8(byte[] data)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < data.Length; i += 2)
            {
                short pcm16 = BitConverter.ToInt16(data, i);
                bool negative = (pcm16 < 0 ? true : false);
                if (negative)
                    pcm16 += 0x7FFF;
                pcm16 >>= 8;
                if (negative)
                    pcm16 += 0x80;
                result.Add((byte)pcm16);
            }

            return result.ToArray();
        }

        public static byte[] Bit8ToBit16(byte[] data)
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                result.AddRange(BitConverter.GetBytes((short)(data[i])));
            }

            return result.ToArray();
        }

    }
}
