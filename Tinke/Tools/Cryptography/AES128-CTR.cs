using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tinke.Tools.Cryptography
{
    using System.Security.Cryptography;

    class AES128_CTR
    {
        byte[] ctr = new byte[16];
        ICryptoTransform counterEncryptor;
        Aes aes = new AesManaged { Mode = CipherMode.ECB, Padding = PaddingMode.None, BlockSize = 128 };

        public AES128_CTR()
        {
            this.aes.GenerateKey();
            this.ctr = new byte[16];
            this.counterEncryptor = this.aes.CreateEncryptor(this.aes.Key, new byte[16]);
        }

        public AES128_CTR(byte[] key)
        {
            SetKey(key);
            this.ctr = new byte[16];
        }

        public AES128_CTR(byte[] key, byte[] ctr)
        {
            SetKey(key);
            SetCtr(ctr);
        }

        public void SetKey(byte[] key)
        {
            byte[] keyswap = new byte[16];

            for (int i = 0; i < 16; i++)
                keyswap[i] = key[15 - i];

            this.aes.KeySize = 128;
            this.counterEncryptor = this.aes.CreateEncryptor(keyswap, new byte[16]);
        }

        public void SetCtr(byte[] ctr)
        {
            for (int i = 0; i < 16; i++)
                this.ctr[i] = ctr[15 - i];
        }

        void AddCtr(uint carry)
        {
            uint[] counter = new uint[4];
            byte[] outctr = this.ctr;

            for (int i = 0; i < 4; i++)
                counter[i] = (uint)(outctr[i * 4 + 0] << 24) | (uint)(outctr[i * 4 + 1] << 16) |
                             (uint)(outctr[i * 4 + 2] << 8) | (uint)(outctr[i * 4 + 3] << 0);

            for (int i = 3; i >= 0; i--)
            {
                uint sum = counter[i] + carry;

                if (sum < counter[i])
                    carry = 1;
                else
                    carry = 0;

                counter[i] = sum;
            }

            for (int i = 0; i < 4; i++)
            {
                outctr[i * 4 + 0] = (byte)(counter[i] >> 24);
                outctr[i * 4 + 1] = (byte)(counter[i] >> 16);
                outctr[i * 4 + 2] = (byte)(counter[i] >> 8);
                outctr[i * 4 + 3] = (byte)(counter[i] >> 0);
            }
        }

        public byte[] CryptCtr(byte[] input, uint offset, uint len)
        {
            byte[] output = new byte[len];
            for (uint i = 0; i < len; i += 0x10)
                CryptCtrBlock(input, offset + i, output, i);

            return output;
        }

        public void CryptCtr(byte[] input, uint offset, uint len, byte[] output, uint outOffset)
        {
            for (uint i = 0; i < len; i += 0x10)
                CryptCtrBlock(input, offset + i, output, i);
        }

        public void CryptCtrBlock(byte[] input, uint inOffset, byte[] output, uint outOffset)
        {
            byte[] stream = new byte[16];

            this.counterEncryptor.TransformBlock(this.ctr, 0, 16, stream, 0);
            if (input != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    output[outOffset + i] = (byte)(stream[15 - i] ^ input[inOffset + i]);
                }
            }
            else
            {
                for (int i = 0; i < 16; i++) output[outOffset + i] = stream[15 - i];
            }

            AddCtr(1);
        }

        public byte[] CryptCtrBlock(byte[] input, uint inOffset)
        {
            byte[] output = new byte[16];
            byte[] stream = new byte[16];

            this.counterEncryptor.TransformBlock(this.ctr, 0, 16, stream, 0);
            if (input != null)
            {
                for (int i = 0; i < 16; i++)
                {
                    output[i] = (byte)(stream[15 - i] ^ input[inOffset + i]);
                }
            }
            else
            {
                for (int i = 0; i < 16; i++) output[i] = stream[15 - i];
            }

            AddCtr(1);
            return output;
        }
    }
}
