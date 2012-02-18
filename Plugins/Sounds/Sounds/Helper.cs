using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public static ushort Get_ushort(ref BinaryReader br)
        {
            return BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
        }
        public static uint Get_uint(ref BinaryReader br)
        {
            return BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
        }

        /* signed nibbles come up a lot */
        public static int[] nibble_to_int = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, -8, -7, -6, -5, -4, -3, -2, -1 };
        public static int get_high_nibble_signed(byte n)
        {
            /*return ((n&0x70)-(n&0x80))>>4;*/
            return nibble_to_int[n >> 4];
        }
        public static int get_low_nibble_signed(byte n)
        {
            /*return (n&7)-(n&8);*/
            return nibble_to_int[n & 0xf];
        }
        public static int clamp16(int val)
        {
            if (val > 32767) return 32767;
            if (val < -32768) return -32768;
            return val;
        }

    }

    public class BitReader
    {
        bool big_endian;
        byte[] buffer;
        int pos;

        public BitReader(byte[] buffer, bool big_endian)
        {
            this.buffer = BytesToBits(buffer);
            pos = 0;
            this.big_endian = big_endian;
        }

        public void Seek(int length)
        {
            if (length < buffer.Length)
                pos = length;
        }

        public byte[] Read(int length)
        {
            byte[] read = new byte[length];
            for (int i = 0; i < length; i++)
                if (pos < buffer.Length)
                    read[i] = buffer[pos++];

            return read;
        }

        public short Read_Short()
        {
            if (big_endian)
                return BitsToShort_BigEndian(Read(16));
            else
                return BitsToShort(Read(16));
        }
        public int Read_Int()
        {
            if (big_endian)
                return BitsToInt_BigEndian(Read(32));
            else
                return BitsToInt(Read(32));
        }
        public int Read_4Bits()
        {
            byte[] bits = Read(4);

            byte b = (byte)(bits[0] << 3);
            b += (byte)(bits[1] << 2);
            b += (byte)(bits[2] << 1);
            b += (byte)bits[3];

            // Sign
            if (bits[0] == 1)
                return b-16;
            else
                return b;
        }

        private short BitsToShort_BigEndian(byte[] bits)
        {
            byte[] bytes = new byte[2];
            for (int i = 0; i < 16; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes[i / 8] = newByte;
            }

            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        private short BitsToShort(byte[] bits)
        {
            byte[] bytes = new byte[2];
            for (int i = 0; i < 16; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes[i / 8] = newByte;
            }

            return BitConverter.ToInt16(bytes, 0);
        }

        private int BitsToInt(byte[] bits)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 32; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes[i / 8] = newByte;
            }

            return BitConverter.ToInt32(bytes, 0);
        }
        private int BitsToInt_BigEndian(byte[] bits)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 32; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes[i / 8] = newByte;
            }

            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }


        private Byte[] BytesToBits(Byte[] bytes)
        {
            List<Byte> bits = new List<byte>();

            for (int i = 0; i < bytes.Length; i++)
                for (int j = 7; j >= 0; j--)
                    bits.Add((byte)((bytes[i] >> j) & 1));

            return bits.ToArray();
        }
        private Byte[] BitsToBytes(Byte[] bits)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes.Add(newByte);
            }

            return bytes.ToArray();
        }
    }
}
