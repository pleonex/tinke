using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tinke.Tools.Cryptography
{
    using System.IO;

    class SHA1
    {
        class sha1_ctx
        {
            public uint[] count; //[2];
            public uint[] hash;  //[5];
            public uint[] wbuf;  //[16];

            public sha1_ctx()
            {
                this.count = new uint[2];
                this.hash = new uint[5];
                this.wbuf = new uint[16];
            }
        }

        const uint SHA1_BLOCK_SIZE = 64;
        const uint SHA1_MASK = SHA1_BLOCK_SIZE - 1;
        const uint SHA1_DIGEST_SIZE = 20;
        const uint SHA2_GOOD = 0;
        const uint SHA2_BAD = 1;

        #region Private Static methods

        static class Helper
        {
            public static uint rotl32(uint x, int n)
            {
                return (((x) << n) | ((x) >> (32 - n)));
            }

            public static uint swap_b32(uint x)
            {
                return ((rotl32(x, 8) & 0x00ff00ffU) | (rotl32(x, 24) & 0xff00ff00U));
            }

            public static void rnd(uint f, uint k, uint wi, ref uint a, ref uint b, ref uint c, ref uint d, ref uint e)
            {
                uint t = a;
                a = rotl32(a, 5) + f + e + k + wi;
                e = d;
                d = c;
                c = rotl32(b, 30);
                b = t;
            }

            public static uint ch(uint x, uint y, uint z)
            {
                return (((x) & (y)) ^ (~(x) & (z)));
            }

            public static uint parity(uint x, uint y, uint z)
            {
                return ((x) ^ (y) ^ (z));
            }

            public static uint maj(uint x, uint y, uint z)
            {
                return (((x) & (y)) ^ ((x) & (z)) ^ ((y) & (z)));
            }
        }

        static void sha1_begin(ref sha1_ctx ctx)
        {
            ctx.count[0] = ctx.count[1] = 0;
            ctx.hash[0] = 0x67452301U;
            ctx.hash[1] = 0xefcdab89U;
            ctx.hash[2] = 0x98badcfeU;
            ctx.hash[3] = 0x10325476U;
            ctx.hash[4] = 0xc3d2e1f0U;
        }

        /* SHA1 hash data in an array of bytes into hash buffer and call the        */
        /* hash_compile function as required.                                       */
        static void sha1_hash(byte[] data, uint len, ref sha1_ctx ctx)
        {
            uint pos = (uint)(ctx.count[0] & SHA1_MASK),
            space = SHA1_BLOCK_SIZE - pos;
            byte[] sp = data;
            uint spi = 0;

            byte[] wbuf = new byte[ctx.wbuf.Length * 4];
            for (int j = 0; j < ctx.wbuf.Length; j++)
                Array.Copy(BitConverter.GetBytes(ctx.wbuf[j]), 0, wbuf, 4 * j, 4);

            if ((ctx.count[0] += len) < len) ++(ctx.count[1]);
            while(len >= space)     /* tranfer whole blocks while possible  */
            {
                Array.Copy(sp, spi, wbuf, pos, space);
                for (int j = 0; j < ctx.wbuf.Length; j++) ctx.wbuf[j] = BitConverter.ToUInt32(wbuf, 4 * j);
                spi += space; len -= space; space = SHA1_BLOCK_SIZE; pos = 0; 
                sha1_compile(ref ctx);
            }

            Array.Copy(sp, spi, wbuf, pos, len);
            for (int j = 0; j < ctx.wbuf.Length; j++) ctx.wbuf[j] = BitConverter.ToUInt32(wbuf, 4 * j);
        }

        static void sha1_compile(ref sha1_ctx ctx)
        {
            uint[] w = new uint[80];
            uint i, a, b, c, d, e, t;

            /* note that words are compiled from the buffer into 32-bit */
            /* words in big-endian order so an order reversal is needed */
            /* here on little endian machines                           */
            for (i = 0; i < SHA1_BLOCK_SIZE / 4; ++i)
                w[i] = Helper.swap_b32(ctx.wbuf[i]);

            for (i = SHA1_BLOCK_SIZE / 4; i < 80; ++i)
                w[i] = Helper.rotl32(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);

            a = ctx.hash[0];
            b = ctx.hash[1];
            c = ctx.hash[2];
            d = ctx.hash[3];
            e = ctx.hash[4];

            for (i = 0; i < 20; ++i)
            {
                Helper.rnd(Helper.ch(b, c, d), 0x5a827999U, w[i], ref a, ref b, ref c, ref d, ref e);
            }

            for (i = 20; i < 40; ++i)
            {
                Helper.rnd(Helper.parity(b, c, d), 0x6ed9eba1U, w[i], ref a, ref b, ref c, ref d, ref e);
            }

            for (i = 40; i < 60; ++i)
            {
                Helper.rnd(Helper.maj(b, c, d), 0x8f1bbcdcU, w[i], ref a, ref b, ref c, ref d, ref e);
            }

            for (i = 60; i < 80; ++i)
            {
                Helper.rnd(Helper.parity(b, c, d), 0xca62c1d6U, w[i], ref a, ref b, ref c, ref d, ref e);
            }

            ctx.hash[0] += a;
            ctx.hash[1] += b;
            ctx.hash[2] += c;
            ctx.hash[3] += d;
            ctx.hash[4] += e;
        }

        /* SHA1 final padding and digest calculation  */
        static uint[] mask = { 0x00000000, 0x000000ff, 0x0000ffff, 0x00ffffff };
        static uint[] bits = { 0x00000080, 0x00008000, 0x00800000, 0x80000000 };

        static void sha1_end(byte[] hval, ref sha1_ctx ctx)
        {
            uint i = (uint)(ctx.count[0] & SHA1_MASK);

            /* mask out the rest of any partial 32-bit word and then set    */
            /* the next byte to 0x80. On big-endian machines any bytes in   */
            /* the buffer will be at the top end of 32 bit words, on little */
            /* endian machines they will be at the bottom. Hence the AND    */
            /* and OR masks above are reversed for little endian systems    */
            /* Note that we can always add the first padding byte at this	*/
            /* because the buffer always contains at least one empty slot	*/
            ctx.wbuf[i >> 2] = (ctx.wbuf[i >> 2] & mask[i & 3]) | bits[i & 3];

            /* we need 9 or more empty positions, one for the padding byte  */
            /* (above) and eight for the length count.  If there is not     */
            /* enough space pad and empty the buffer                        */
            if (i > SHA1_BLOCK_SIZE - 9)
            {
                if (i < 60) ctx.wbuf[15] = 0;
                sha1_compile(ref ctx);
                i = 0;
            }
            else    /* compute a word index for the empty buffer positions  */
                i = (i >> 2) + 1;

            while (i < 14) /* and zero pad all but last two positions      */
                ctx.wbuf[i++] = 0;

            /* assemble the eight byte counter in in big-endian format		*/
            ctx.wbuf[14] = Helper.swap_b32((ctx.count[1] << 3) | (ctx.count[0] >> 29));
            ctx.wbuf[15] = Helper.swap_b32(ctx.count[0] << 3);

            sha1_compile(ref ctx);

            /* extract the hash value as bytes in case the hash buffer is   */
            /* misaligned for 32-bit words                                  */
            for (i = 0; i < SHA1_DIGEST_SIZE; ++i)
                hval[i] = (byte)(ctx.hash[i >> 2] >> 8 * (int)(~i & 3));
        }

        #endregion

        internal static byte[] ComputeHash(byte[] data, uint len)
        {
            byte[] hval = new byte[20];
            sha1_ctx cx = new sha1_ctx();
            sha1_begin(ref cx);
            sha1_hash(data, len, ref cx);
            sha1_end(hval, ref cx);
            return hval;
        }

        internal static byte[] Sha1Hmac(string f, byte[] hmac_sha1_key)
        {
            Stream str = File.OpenRead(f);
            byte[] hash = Sha1Hmac(str, 0, (uint)str.Length, hmac_sha1_key);
            str.Close();
            return hash;
        }

        internal static byte[] Sha1Hmac(byte[] data, uint pos, uint size, byte[] hmac_sha1_key)
        {
            MemoryStream str = new MemoryStream(data);
            byte[] hash = Sha1Hmac(str, pos, size, hmac_sha1_key);
            str.Close();
            return hash;
        }

        internal static byte[] Sha1Hmac(byte[] data, byte[] hmac_sha1_key)
        {
            MemoryStream str = new MemoryStream(data);
            byte[] hash = Sha1Hmac(str, 0, (uint)str.Length, hmac_sha1_key);
            str.Close();
            return hash;
        }

        internal static byte[] Sha1Hmac(Stream f, uint pos, uint size, byte[] hmac_sha1_key)
        {
            sha1_ctx cx = new sha1_ctx();
            byte[] readbuf = new byte[4096];
            byte[] keypad = new byte[0x40];
            for (int i = 0; i < 0x40; i++) keypad[i] = (byte)(hmac_sha1_key[i] ^ 0x36);
            sha1_begin(ref cx);
            sha1_hash(keypad, 0x40, ref cx);
            long tmp = f.Position;
            f.Seek(pos, SeekOrigin.Begin);
            while (size > 0)
            {
                int rdbytes = size > readbuf.Length ? readbuf.Length : (int)size;
                f.Read(readbuf, 0, rdbytes);
                sha1_hash(readbuf, (uint)rdbytes, ref cx);
                size -= (uint)rdbytes;
            }

            byte[] output = new byte[20];
            sha1_end(output, ref cx);

            for (int i = 0; i < 0x40; i++) keypad[i] = (byte)(hmac_sha1_key[i] ^ 0x5c);
            sha1_begin(ref cx);
            sha1_hash(keypad, 0x40, ref cx);
            sha1_hash(output, 20, ref cx);
            sha1_end(output, ref cx);
            f.Seek(tmp, SeekOrigin.Begin);
            return output;
        }
    }
}
